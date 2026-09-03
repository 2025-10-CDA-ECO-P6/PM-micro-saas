"""Tests de caractérisation — mode Analyse `+refacto` — pour `tools/verif_corpus.py`.

But : figer le comportement OBSERVABLE actuel du script avant sa réécriture en
C#. Ces tests ne jugent pas si le comportement est souhaitable ; ils
constatent ce que le script fait aujourd'hui, sur le corpus réel mesuré le
2026-09-03 et sur des dépôts de fixture synthétiques et jetables (jamais le
dépôt réel n'est modifié — chaque dépôt de fixture est un répertoire
temporaire propre à son test, détruit à la fin de celui-ci).

Framework : `unittest` (bibliothèque standard) — `pytest` n'est pas installé
sur cette machine et aucun accès réseau n'est supposé pour l'installer ;
hypothèse consignée dans la note de clôture.

Le CODE DE SORTIE est hors oracle : ce script ne contient aucun `sys.exit`
ni `exit()` (mesuré : `grep -c` renvoie 0) — le processus se termine toujours
en 0, qu'un défaut soit rapporté ou non. Ce n'est pas un comportement à
figer, c'est un défaut mesuré ; les tests ci-dessous n'assertent donc jamais
sur `resultat.returncode`, seulement sur le contenu de la sortie standard
(le seul canal qui porte le verdict). Voir `ContratFuturCodeDeSortie` en fin
de fichier pour le contrat que la réécriture C# devra satisfaire.
"""
import os
import re
import sys
import tempfile
import unittest

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
import helpers


class SurCorpusReel(unittest.TestCase):
    """Golden master n°1 — comportement nominal mesuré aujourd'hui sur le
    corpus réel du dépôt, en lecture seule (aucune écriture, `git ls-files`
    et lecture de fichiers uniquement)."""

    def test_devrait_ne_rapporter_aucun_lien_casse_aucune_ancre_cassee_aucun_orphelin_et_aucune_fence_desequilibree_sur_le_corpus_reel(self):
        resultat = helpers.executer_script("verif_corpus.py", [helpers.RACINE_DEPOT])
        self.assertIn("liens à cible inexistante     : 0", resultat.stdout, resultat.stderr)
        self.assertIn("liens à ancre inexistante     : 0", resultat.stdout)
        self.assertIn("fichiers orphelins            : 0", resultat.stdout)
        self.assertIn("blocs de code déséquilibrés   : 0", resultat.stdout)

    def test_devrait_indexer_un_nombre_non_nul_de_fichiers_markdown_sous_docs_sur_le_corpus_reel(self):
        # Le décompte exact de fichiers/liens est volatil (le corpus évolue) —
        # exclu du golden master. Seule la non-nullité est vérifiée : un
        # décompte à zéro trahirait un balayage pointant sur le mauvais
        # répertoire, régression bien plus grave qu'une dérive de décompte.
        resultat = helpers.executer_script("verif_corpus.py", [helpers.RACINE_DEPOT])
        correspondance = re.search(r"sous docs/ : (\d+)", resultat.stdout)
        self.assertIsNotNone(correspondance, resultat.stdout)
        self.assertGreater(int(correspondance.group(1)), 0)


class DecouverteDesFichiers(unittest.TestCase):
    """Comportement n°12 — l'indexation ne recense que les `*.md`, via
    `git ls-files` ET `git ls-files --others --exclude-standard` : un `.md`
    neuf non commité est vu, un `.md` d'un répertoire ignoré ne l'est pas."""

    def setUp(self):
        self._tmp = tempfile.TemporaryDirectory()
        self.depot = self._tmp.name
        helpers.initialiser_depot_git(self.depot)
        helpers.ecrire(os.path.join(self.depot, "README.md"), "# Dépôt de fixture\n")
        helpers.ecrire(os.path.join(self.depot, "docs/guide.md"), "# Guide\n\nContenu du guide.\n")
        helpers.ecrire(os.path.join(self.depot, ".gitignore"), "/ignore-audit/\n")
        helpers.commiter_tout(self.depot, "corpus initial")
        # Après le commit : un .md neuf non suivi (doit être vu), et un .md
        # dans un répertoire ignoré par .gitignore (ne doit pas être vu).
        helpers.ecrire(os.path.join(self.depot, "docs/nouveau.md"), "# Note fraîche\n\nPas encore commitée.\n")
        helpers.ecrire(os.path.join(self.depot, "ignore-audit/notes.md"), "# Notes d'audit\n\nIgnorées par construction.\n")

    def tearDown(self):
        self._tmp.cleanup()

    def test_devrait_voir_un_md_neuf_non_commite_et_ignorer_un_md_dans_un_repertoire_exclu_par_gitignore(self):
        resultat = helpers.executer_script("verif_corpus.py", [self.depot])
        # Décompte exact : README.md + docs/guide.md (suivis) + docs/nouveau.md
        # (neuf, non ignoré) = 3, dont 2 sous docs/. ignore-audit/notes.md
        # n'entre dans aucun des deux décomptes.
        self.assertIn("fichiers markdown             : 3  (sous docs/ : 2)", resultat.stdout, resultat.stderr)
        self.assertIn("docs/nouveau.md", resultat.stdout)
        self.assertNotIn("notes.md", resultat.stdout)


class DetectionLienACibleInexistante(unittest.TestCase):
    def setUp(self):
        self._tmp = tempfile.TemporaryDirectory()
        self.depot = self._tmp.name
        helpers.initialiser_depot_git(self.depot)
        helpers.ecrire(os.path.join(self.depot, "README.md"), "# Dépôt de fixture\n")
        helpers.ecrire(
            os.path.join(self.depot, "docs/page-a.md"),
            "# Page A\n\nVoir [la page absente](page-inexistante.md) pour la suite.\n",
        )
        helpers.commiter_tout(self.depot, "lien vers cible inexistante")

    def tearDown(self):
        self._tmp.cleanup()

    def test_devrait_signaler_un_lien_dont_la_cible_relative_n_existe_pas_sur_le_disque(self):
        resultat = helpers.executer_script("verif_corpus.py", [self.depot])
        self.assertIn("liens à cible inexistante     : 1", resultat.stdout, resultat.stderr)
        self.assertIn("[CIBLE CASSÉE] docs/page-a.md -> page-inexistante.md", resultat.stdout)


class DetectionLienAAncreInexistante(unittest.TestCase):
    def setUp(self):
        self._tmp = tempfile.TemporaryDirectory()
        self.depot = self._tmp.name
        helpers.initialiser_depot_git(self.depot)
        helpers.ecrire(os.path.join(self.depot, "README.md"), "# Dépôt de fixture\n")
        helpers.ecrire(
            os.path.join(self.depot, "docs/page-a.md"),
            "# Page A\n\nVoir [la section](page-b.md#section-absente).\n",
        )
        helpers.ecrire(
            os.path.join(self.depot, "docs/page-b.md"),
            "# Page B\n\n## Une autre section\n\nContenu de la page B.\n",
        )
        helpers.commiter_tout(self.depot, "lien vers ancre inexistante")

    def tearDown(self):
        self._tmp.cleanup()

    def test_devrait_signaler_un_lien_dont_l_ancre_ne_correspond_a_aucun_titre_ni_ancre_nommee_de_la_cible(self):
        resultat = helpers.executer_script("verif_corpus.py", [self.depot])
        self.assertIn("liens à ancre inexistante     : 1", resultat.stdout, resultat.stderr)
        self.assertIn("[ANCRE CASSÉE] docs/page-a.md -> page-b.md#section-absente", resultat.stdout)


class DetectionFichierOrphelin(unittest.TestCase):
    def setUp(self):
        self._tmp = tempfile.TemporaryDirectory()
        self.depot = self._tmp.name
        helpers.initialiser_depot_git(self.depot)
        helpers.ecrire(os.path.join(self.depot, "README.md"), "# Dépôt de fixture\n\nAucun lien vers docs/.\n")
        helpers.ecrire(os.path.join(self.depot, "docs/isole.md"), "# Page isolée\n\nAucun fichier n'y renvoie.\n")
        helpers.commiter_tout(self.depot, "fichier sans lien entrant")

    def tearDown(self):
        self._tmp.cleanup()

    def test_devrait_signaler_un_fichier_md_vers_lequel_aucun_autre_fichier_ne_renvoie(self):
        resultat = helpers.executer_script("verif_corpus.py", [self.depot])
        self.assertIn("fichiers orphelins            : 1", resultat.stdout, resultat.stderr)
        self.assertIn("[ORPHELIN] docs/isole.md", resultat.stdout)


class DetectionBlocDeCodeDesequilibre(unittest.TestCase):
    def setUp(self):
        self._tmp = tempfile.TemporaryDirectory()
        self.depot = self._tmp.name
        helpers.initialiser_depot_git(self.depot)
        helpers.ecrire(os.path.join(self.depot, "README.md"), "# Dépôt de fixture\n")
        helpers.ecrire(
            os.path.join(self.depot, "docs/code.md"),
            "# Extrait de code\n\n```python\ndef f():\n    return 1\n",
        )
        helpers.commiter_tout(self.depot, "fence jamais refermée")

    def tearDown(self):
        self._tmp.cleanup()

    def test_devrait_signaler_un_fichier_dont_le_bloc_de_code_ouvert_n_est_jamais_referme(self):
        resultat = helpers.executer_script("verif_corpus.py", [self.depot])
        self.assertIn("blocs de code déséquilibrés   : 1", resultat.stdout, resultat.stderr)
        self.assertIn("[FENCE] docs/code.md", resultat.stdout)


class ContratFuturCodeDeSortie(unittest.TestCase):
    """Le code de sortie ne reflète aujourd'hui AUCUN verdict : mesuré sur un
    corpus portant un défaut, le script le rapporte fidèlement en sortie
    standard mais termine quand même en 0 (aucun `sys.exit`/`exit()` dans le
    fichier). C'est un défaut, pas un comportement à figer.

    Ce test documente le contrat que la réécriture C# (et une éventuelle
    correction du Python) devra satisfaire : code de sortie non nul dès
    qu'un défaut est rapporté. Il échoue INTENTIONNELLEMENT sur le code
    Python actuel — `@expectedFailure` le marque explicitement comme
    attendu-rouge, pour qu'il ne soit jamais lu comme une régression du
    harnais. Le jour où il passera (correction du Python, ou portage
    conforme), unittest le signalera comme « unexpected success »."""

    def setUp(self):
        self._tmp = tempfile.TemporaryDirectory()
        self.depot = self._tmp.name
        helpers.initialiser_depot_git(self.depot)
        helpers.ecrire(os.path.join(self.depot, "README.md"), "# Dépôt de fixture\n")
        helpers.ecrire(
            os.path.join(self.depot, "docs/page-a.md"),
            "# Page A\n\nVoir [la page absente](page-inexistante.md) pour la suite.\n",
        )
        helpers.commiter_tout(self.depot, "lien vers cible inexistante")

    def tearDown(self):
        self._tmp.cleanup()

    @unittest.expectedFailure
    def test_le_code_de_sortie_devrait_etre_non_nul_quand_un_defaut_est_rapporte(self):
        resultat = helpers.executer_script("verif_corpus.py", [self.depot])
        # Le défaut est bien rapporté en sortie standard (ceci, seul, passe
        # aujourd'hui) — mais le code de sortie ne le reflète pas.
        self.assertIn("liens à cible inexistante     : 1", resultat.stdout, resultat.stderr)
        self.assertNotEqual(resultat.returncode, 0)


if __name__ == "__main__":
    unittest.main()
