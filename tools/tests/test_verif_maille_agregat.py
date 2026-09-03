"""Tests de caractérisation — mode Analyse `+refacto` — pour
`tools/verif_maille_agregat.py`.

Ce script ne dépend d'aucun état git ni réseau : il prend une racine et un
chemin de plan en arguments positionnels et lit directement les fichiers.
Les dépôts de fixture ci-dessous sont donc de simples arborescences de
fichiers dans un répertoire temporaire — pas des dépôts git.

Le CODE DE SORTIE est hors oracle : ce script ne contient aucun `sys.exit`
ni `exit()` (mesuré : `grep -c` renvoie 0) — le processus se termine toujours
en 0, qu'un item ne se résolve pas ou non. Ce n'est pas un comportement à
figer, c'est un défaut mesuré ; les tests ci-dessous n'assertent donc jamais
sur `resultat.returncode`, seulement sur le contenu de la sortie standard.
Voir `ContratFuturCodeDeSortie` en fin de fichier pour le contrat que la
réécriture C# devra satisfaire.
"""
import os
import sys
import tempfile
import unittest

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
import helpers

# Fragment de modèle de domaine partagé par les fixtures qui ont besoin d'un
# agrégat résolu nommé « Personnage » — le nom lui-même n'importe pas au
# script, seule sa présence sous `## Agrégats` compte.
MODELE_DOMAINE = "# Personnage (domaine)\n\n## Agrégats\n\n### Personnage\n\nRacine d'agrégat.\n"


# `taches()` découpe sur `\n#{4,6}\s+(?=TB-)` : un match exige un `\n` AVANT le
# marqueur de titre. Sans texte de préambule avant le premier titre `#####
# TB-…`, ce premier titre n'est précédé d'aucun `\n` et son bloc entier est
# alors le [0] jeté par `[1:]` — silencieusement absorbé comme s'il s'agissait
# du préambule du §2. D'où ce préambule minimal, obligatoire dans toute
# fixture de plan pour ce script (comportement du script, pas un choix de
# fixture arbitraire).
PREAMBULE_PLAN = "# Plan de travail (fixture)\n\n§2 — Maille de désignation, non pertinente pour ce test.\n\n"


def _construire_racine(tmp, plan_md, avec_domaine=True):
    if avec_domaine:
        helpers.ecrire(os.path.join(tmp, "docs/conception/domain/personnage.md"), MODELE_DOMAINE)
    helpers.ecrire(os.path.join(tmp, "docs/gestion-projet/plan-de-travail.md"), PREAMBULE_PLAN + plan_md)


class SurPlanReel(unittest.TestCase):
    """Golden master — sortie nominale mesurée aujourd'hui (2026-09-03) sur le
    plan de travail réel : 52 tâches, 47 périmètres entièrement résolus, 4
    HORS MAILLE exclus, 0 TROU, 1 en attente de J0, 0 item non résolu, 0 écart
    de conflit."""

    def test_devrait_reproduire_exactement_les_decomptes_nominaux_mesures_sur_le_plan_reel(self):
        resultat = helpers.executer_script("verif_maille_agregat.py", [helpers.RACINE_DEPOT])
        stdout = resultat.stdout
        self.assertIn("tâches du plan                        : 52", stdout, resultat.stderr)
        self.assertIn("périmètre entièrement résolu        : 47", stdout)
        self.assertIn("périmètre HORS MAILLE (exclu)       : 4", stdout)
        self.assertIn("périmètre TROU (non nommable)       : 0", stdout)
        self.assertIn("périmètre en attente de J0 (rôle)   : 1", stdout)
        self.assertIn("item ne se résolvant pas la maille  : 0", stdout)
        self.assertIn("écarts entre `En conflit avec` déclaré et l'intersection recalculée : 0", stdout)


class ExclusionHorsMaille(unittest.TestCase):
    """Comportement n°2 — HORS MAILLE est exclu de la liste des items, pas
    seulement reconnu comme résoluble : une tranche qui le porte est comptée
    à part, jamais dans « résolu »."""

    def setUp(self):
        self._tmp = tempfile.TemporaryDirectory()
        self.tmp = self._tmp.name
        plan = (
            "##### TB-014 — Création d'un personnage joueur\n\n"
            "| Périmètre d'écriture | Personnage |\n"
            "| En conflit avec | — |\n\n"
            "##### TB-777 — Tâche sans code produit\n\n"
            "| Périmètre d'écriture | HORS MAILLE — recette manuelle uniquement |\n"
            "| En conflit avec | — |\n"
        )
        _construire_racine(self.tmp, plan)

    def tearDown(self):
        self._tmp.cleanup()

    def test_devrait_exclure_la_tache_hors_maille_du_decompte_resolu_et_la_compter_a_part(self):
        resultat = helpers.executer_script(
            "verif_maille_agregat.py", [self.tmp, "docs/gestion-projet/plan-de-travail.md"]
        )
        stdout = resultat.stdout
        self.assertIn("tâches du plan                        : 2", stdout, resultat.stderr)
        self.assertIn("périmètre entièrement résolu        : 1", stdout)
        self.assertIn("périmètre HORS MAILLE (exclu)       : 1", stdout)


class ZeroConflitFantomeEntreHorsMailleIdentiques(unittest.TestCase):
    """Comportement n°3 — deux tranches portant un libellé HORS MAILLE
    identique ne produisent aucun conflit fantôme ; le zéro-conflit ne dépend
    pas de la variété des libellés."""

    def setUp(self):
        self._tmp = tempfile.TemporaryDirectory()
        self.tmp = self._tmp.name
        libelle_partage = "HORS MAILLE — recette manuelle uniquement, sans code produit"
        plan = (
            f"##### TB-777 — Recette manuelle du parcours d'accueil\n\n"
            f"| Périmètre d'écriture | {libelle_partage} |\n"
            f"| En conflit avec | — |\n\n"
            f"##### TB-778 — Recette manuelle du parcours de sortie\n\n"
            f"| Périmètre d'écriture | {libelle_partage} |\n"
            f"| En conflit avec | — |\n"
        )
        _construire_racine(self.tmp, plan, avec_domaine=False)

    def tearDown(self):
        self._tmp.cleanup()

    def test_ne_devrait_produire_aucun_ecart_de_conflit_entre_deux_taches_hors_maille_au_libelle_identique(self):
        resultat = helpers.executer_script(
            "verif_maille_agregat.py", [self.tmp, "docs/gestion-projet/plan-de-travail.md"]
        )
        self.assertIn(
            "écarts entre `En conflit avec` déclaré et l'intersection recalculée : 0",
            resultat.stdout,
            resultat.stderr,
        )


class RejetDuModuleFantome(unittest.TestCase):
    """Comportement n°4 — un module que le corpus ne nomme pas est REFUSÉ,
    pas absorbé. Défaut réel trouvé et corrigé le 2026-09-03 : le vérificateur
    l'acceptait avant."""

    def setUp(self):
        self._tmp = tempfile.TemporaryDirectory()
        self.tmp = self._tmp.name
        plan = (
            "##### TB-091 — Tâche au périmètre fantaisiste\n\n"
            "| Périmètre d'écriture | le projet Domaine fantôme qui n'existe pas |\n"
            "| En conflit avec | — |\n"
        )
        _construire_racine(self.tmp, plan, avec_domaine=False)

    def tearDown(self):
        self._tmp.cleanup()

    def test_devrait_refuser_un_module_absent_du_corpus_et_le_classer_non_resolu(self):
        resultat = helpers.executer_script(
            "verif_maille_agregat.py", [self.tmp, "docs/gestion-projet/plan-de-travail.md"]
        )
        stdout = resultat.stdout
        self.assertIn("item ne se résolvant pas la maille  : 1", stdout, resultat.stderr)
        self.assertIn("[NON RÉSOLU] TB-091 -> [\"le projet Domaine fantôme qui n'existe pas\"]", stdout)
        self.assertIn("périmètre entièrement résolu        : 0", stdout)


class DesignationDeRoleEnAttenteDeJ0(unittest.TestCase):
    """Comportement n°5 — la désignation de rôle sur Infrastructure/
    Présentation atterrit dans « en attente de J0 », jamais dans « résolu »
    ni « non résolu ». Les deux formes canoniques « le projet Domaine
    unique » et « le projet Application unique » se résolvent, elles."""

    def setUp(self):
        self._tmp = tempfile.TemporaryDirectory()
        self.tmp = self._tmp.name
        plan = (
            "##### TB-002 — Rôle du projet Domaine unique\n\n"
            "| Périmètre d'écriture | le projet Domaine unique |\n"
            "| En conflit avec | — |\n\n"
            "##### TB-003 — Rôle du projet Application unique\n\n"
            "| Périmètre d'écriture | le projet Application unique |\n"
            "| En conflit avec | — |\n\n"
            "##### TB-004 — Rôle non clôturable avant J0\n\n"
            "| Périmètre d'écriture | les autres projets d'Infrastructure et de Présentation, "
            "dont le nombre et les noms relèvent de J0 |\n"
            "| En conflit avec | — |\n"
        )
        _construire_racine(self.tmp, plan, avec_domaine=False)

    def tearDown(self):
        self._tmp.cleanup()

    def test_devrait_resoudre_les_deux_formes_canoniques_et_mettre_en_attente_j0_la_mention_de_role_infrastructure_presentation(self):
        resultat = helpers.executer_script(
            "verif_maille_agregat.py", [self.tmp, "docs/gestion-projet/plan-de-travail.md"]
        )
        stdout = resultat.stdout
        self.assertIn("périmètre entièrement résolu        : 2", stdout, resultat.stderr)
        self.assertIn("périmètre en attente de J0 (rôle)   : 1", stdout)
        self.assertIn("item ne se résolvant pas la maille  : 0", stdout)
        self.assertIn("[EN ATTENTE J0] TB-004 ->", stdout)


class ContratFuturCodeDeSortie(unittest.TestCase):
    """Le code de sortie ne reflète aujourd'hui AUCUN verdict : mesuré sur un
    plan portant un item non résolu, le script le rapporte fidèlement en
    sortie standard mais termine quand même en 0 (aucun `sys.exit`/`exit()`
    dans le fichier). C'est un défaut, pas un comportement à figer.

    Ce test documente le contrat que la réécriture C# (et une éventuelle
    correction du Python) devra satisfaire : code de sortie non nul dès
    qu'un item ne se résout pas dans la maille. Il échoue INTENTIONNELLEMENT
    sur le code Python actuel — `@expectedFailure` le marque explicitement
    comme attendu-rouge, pour qu'il ne soit jamais lu comme une régression
    du harnais. Le jour où il passera, unittest le signalera comme
    « unexpected success »."""

    def setUp(self):
        self._tmp = tempfile.TemporaryDirectory()
        self.tmp = self._tmp.name
        plan = (
            "##### TB-091 — Tâche au périmètre fantaisiste\n\n"
            "| Périmètre d'écriture | le projet Domaine fantôme qui n'existe pas |\n"
            "| En conflit avec | — |\n"
        )
        _construire_racine(self.tmp, plan, avec_domaine=False)

    def tearDown(self):
        self._tmp.cleanup()

    @unittest.expectedFailure
    def test_le_code_de_sortie_devrait_etre_non_nul_quand_un_item_ne_se_resout_pas(self):
        resultat = helpers.executer_script(
            "verif_maille_agregat.py", [self.tmp, "docs/gestion-projet/plan-de-travail.md"]
        )
        # Le défaut est bien rapporté en sortie standard (ceci, seul, passe
        # aujourd'hui) — mais le code de sortie ne le reflète pas.
        self.assertIn("item ne se résolvant pas la maille  : 1", resultat.stdout, resultat.stderr)
        self.assertNotEqual(resultat.returncode, 0)


if __name__ == "__main__":
    unittest.main()
