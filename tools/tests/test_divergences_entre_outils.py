"""Tests d'INVARIANT — distincts des golden masters — exposant quatre
divergences mesurées AUJOURD'HUI entre `verif_maille_agregat.py` (le
vérificateur) et `creer-issues.py` (le générateur), sur des entrées que le
plan de travail réel ne porte pas encore. Un différentiel de sortie est
aveugle par construction à ces divergences : le plan actuel ne déclenche
aucune des quatre, donc aucun golden master construit dessus ne les
verrait jamais. Ces tests les rendent visibles sur des entrées CONSTRUITES,
qui portent chacune le déclencheur, avant que la capture ne les grave dans
l'angle mort.

Chaque test affirme l'invariant SOUHAITÉ (les deux outils s'accordent sur la
même entrée) et échoue INTENTIONNELLEMENT sur le code Python actuel —
`@unittest.expectedFailure` le marque explicitement comme attendu-rouge :
son échec ici n'est pas une régression du harnais, c'est la divergence
elle-même, rendue visible.

Portée : ce fichier EXPOSE, il ne CORRIGE rien. L'arbitrage de laquelle des
deux implémentations est juste — ou, pour la divergence D, du choix de
modélisation à retenir — appartient au donneur d'ordre.
"""
import os
import sys
import tempfile
import unittest

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
import helpers

METHODE_MD = """# Méthode de ticket (fixture de test)

## §3 Taxonomie de libellés

**Par jalon**

| Valeur | Note |
|---|---|
| J0 | Fondations |
| J1 | Traversée verticale |

**Par nature de tranche**

| Valeur | Note |
|---|---|
| Comportement | Agrégat de domaine |
| Couche cliente | Module client |
| Outillage | Projet d'outillage |
| Socle | Module transverse |
| Surface | Fiche d'écran |
| Hors maille | Aucun code produit |

**Par nature de vérification**

| Valeur | Note |
|---|---|
| Fonctionnel | Comportement observable |

**Interdiction nommée** : aucun axe de priorité n'est posé sur les issues (fixture de test).
"""

PLAN_PREAMBULE = """# Plan de travail (fixture de test)

## §2 Maille de désignation

**Les six modules de la couche cliente et leur ancrage.**

| `service d'accès au store` | Accès IndexedDB |
| `châssis` | Disposition générale |
| `bandeaux transversaux` | Bandeaux d'état |
| `sanitisation` | Nettoyage HTML |
| `politique de sécurité de contenu` | CSP |
| `projection d'export` | Rendu d'export |

**L'agrégat couvre ses deux moitiés.**

Le domaine et son module d'accès forment la même unité d'écriture.

- un **module d'outillage** : `la configuration de la solution`, `la configuration d'intégration continue`
- un **module transverse nommé** : `SharedKernel`, `Haversack.Infrastructure.Notifications`

### Tâches

"""

DOMAINE_MD = """# Personnage (domaine)

## Agrégats

### Personnage

Racine d'agrégat, décrit un personnage joueur.
"""


def _construire_depot(depot, plan_md):
    """Dépôt git jetable, méthode et modèle de domaine valides, plan fourni
    par l'appelant. `origin` pointe vers un dépôt fictif et la révision est
    simulée « déjà poussée » (cf. RegimesDeLaRevisionEnModeABlanc dans
    test_creer_issues.py) — uniquement pour que l'AVERTISSEMENT sur la
    révision, hors sujet ici, n'ajoute pas de bruit à la démonstration."""
    helpers.initialiser_depot_git(depot)
    helpers.ecrire(os.path.join(depot, "docs/gestion-projet/methode-de-ticket.md"), METHODE_MD)
    helpers.ecrire(os.path.join(depot, "docs/gestion-projet/plan-de-travail.md"), plan_md)
    helpers.ecrire(os.path.join(depot, "docs/conception/domain/personnage.md"), DOMAINE_MD)
    commit = helpers.commiter_tout(depot, "fixture de divergence")
    helpers.executer_git(depot, "remote", "add", "origin", "git@github.com:exemple-org/exemple-depot.git")
    branche = helpers.executer_git(depot, "rev-parse", "--abbrev-ref", "HEAD").stdout.strip()
    helpers.executer_git(depot, "update-ref", f"refs/remotes/origin/{branche}", commit)


def _executer_generateur(depot):
    with tempfile.TemporaryDirectory() as bindir:
        env = dict(os.environ)
        env["PATH"] = helpers.repertoire_bin_sans_gh(bindir)
        return helpers.executer_script("creer-issues.py", [], cwd=depot, env=env)


class DivergenceA_FormatDuTitreDeTranche(unittest.TestCase):
    """Divergence A — les deux outils analysent le titre de tranche avec des
    règles différentes :
      creer-issues.py:244          ^##### (TB-\\d+) — (.+)$   (5 dièses EXACTS,
                                                                tiret cadratin obligatoire)
      verif_maille_agregat.py:91   \\n#{4,6}\\s+(?=TB-)         (4 à 6 dièses,
                                                                aucune contrainte de tiret)

    EN PRATIQUE : une tranche titrée avec 4 (ou 6) dièses, ou un tiret court
    à la place du cadratin, est VUE et contrôlée par le vérificateur — mais
    INVISIBLE, sans aucun signalement, pour le générateur : elle ne recevrait
    jamais d'issue GitHub, silencieusement, alors même que le plan la déclare
    conforme. Mesuré : sur ce plan à une seule tâche ainsi titrée, le
    vérificateur rapporte 1 tâche, le générateur 0."""

    def setUp(self):
        self._tmp = tempfile.TemporaryDirectory()
        self.depot = self._tmp.name
        plan = PLAN_PREAMBULE + (
            "#### TB-050 – Tâche à quatre dièses et tiret court\n\n"
            "| Champ | Valeur |\n|---|---|\n"
            "| Jalon | J1 — traversée verticale |\n"
            "| Périmètre d'écriture | Personnage |\n"
            "| Dépend de | — |\n"
            "| En conflit avec | — |\n"
        )
        _construire_depot(self.depot, plan)

    def tearDown(self):
        self._tmp.cleanup()

    @unittest.expectedFailure
    def test_une_tranche_vue_par_le_verificateur_devrait_aussi_etre_lue_par_le_generateur(self):
        resultat_verificateur = helpers.executer_script(
            "verif_maille_agregat.py", [self.depot, "docs/gestion-projet/plan-de-travail.md"]
        )
        # Prémisse, vraie aujourd'hui : le vérificateur voit bien la tâche.
        self.assertIn("tâches du plan                        : 1", resultat_verificateur.stdout)

        resultat_generateur = _executer_generateur(self.depot)
        # Invariant souhaité — ÉCHOUE aujourd'hui : le générateur en lit 0.
        self.assertIn("Tâches lues dans le plan : 1", resultat_generateur.stdout)


class DivergenceB_NombreDeChiffresDansLidentifiant(unittest.TestCase):
    """Divergence B — le nombre de chiffres admis dans l'identifiant :
      verif_maille_agregat.py:92,106   TB-\\d{3}   exactement trois chiffres
      creer-issues.py:244,454,689      TB-\\d+     n'importe quel nombre

    Le plan réel va aujourd'hui jusqu'à TB-095, donc muette : trois chiffres
    couvrent tout, la divergence ne se déclenche jamais. Le déclencheur
    construit ici est une tâche `TB-1000`, à quatre chiffres.

    EN PRATIQUE : `verif_maille_agregat.py::taches()` extrait l'identifiant
    via `re.match(r'(TB-\\d{3})', bloc)` — non ancré en fin de motif, il
    capture seulement les trois premiers chiffres. `TB-1000` est donc lu
    comme `TB-100` : si une tâche `TB-100` existe par ailleurs dans le même
    plan, la seconde écrase silencieusement la première dans le dictionnaire
    des tâches (`out[i] = ...`, i identique pour les deux) — une tâche
    entière disparaît du décompte, remplacée par les données de l'autre.
    `creer-issues.py::extraire_taches()`, qui construit une LISTE et non un
    dictionnaire indexé par identifiant tronqué, ne connaît pas cette
    collision et voit les deux tâches. Mesuré : le vérificateur rapporte 1
    tâche là où le générateur en rapporte 2, sur un plan qui en porte
    réellement deux."""

    def setUp(self):
        self._tmp = tempfile.TemporaryDirectory()
        self.depot = self._tmp.name
        plan = PLAN_PREAMBULE + (
            "##### TB-100 — Tâche à trois chiffres\n\n"
            "| Champ | Valeur |\n|---|---|\n"
            "| Jalon | J1 — traversée verticale |\n"
            "| Périmètre d'écriture | Personnage |\n"
            "| Dépend de | — |\n"
            "| En conflit avec | — |\n\n"
            "---\n\n"
            "##### TB-1000 — Tâche à quatre chiffres\n\n"
            "| Champ | Valeur |\n|---|---|\n"
            "| Jalon | J1 — traversée verticale |\n"
            "| Périmètre d'écriture | châssis |\n"
            "| Dépend de | — |\n"
            "| En conflit avec | — |\n"
        )
        _construire_depot(self.depot, plan)

    def tearDown(self):
        self._tmp.cleanup()

    @unittest.expectedFailure
    def test_deux_tranches_a_trois_et_quatre_chiffres_devraient_etre_comptees_deux_par_les_deux_outils(self):
        resultat_generateur = _executer_generateur(self.depot)
        # Prémisse, vraie aujourd'hui : le générateur (\\d+) distingue bien
        # les deux identifiants et voit deux tâches.
        self.assertIn("Tâches lues dans le plan : 2", resultat_generateur.stdout)

        resultat_verificateur = helpers.executer_script(
            "verif_maille_agregat.py", [self.depot, "docs/gestion-projet/plan-de-travail.md"]
        )
        # Invariant souhaité — ÉCHOUE aujourd'hui : TB-1000 est tronqué en
        # TB-100 et écrase la vraie TB-100 dans le dictionnaire des tâches ;
        # le vérificateur n'en compte plus qu'une.
        self.assertIn("tâches du plan                        : 2", resultat_verificateur.stdout)


class DivergenceC_MailleLueOuCodeeEnDur(unittest.TestCase):
    """Divergence C — la maille de désignation, dans les deux sens :
    `creer-issues.py::extraire_maille` LIT le §2 du plan à chaque exécution ;
    `verif_maille_agregat.py::OUTILLAGE` la porte EN DUR (quatre libellés
    figés dans le code, lignes 57-61).

    EN PRATIQUE : si le §2 gagne un cinquième libellé d'outillage — ce que
    le donneur d'ordre indique comme prescrit et à venir — le générateur le
    résout (il le lit) tandis que le vérificateur le refuse toujours
    (`NON RÉSOLU`), quand bien même le corpus l'autorise désormais : une
    fausse alerte de non-conformité sur un module pourtant légitime. Mesuré
    avec « le pipeline de publication npm » comme cinquième libellé, ajouté
    au §2 mais absent du dictionnaire `OUTILLAGE` codé en dur."""

    def setUp(self):
        self._tmp = tempfile.TemporaryDirectory()
        self.depot = self._tmp.name
        plan = PLAN_PREAMBULE.replace(
            "- un **module d'outillage** : `la configuration de la solution`, `la configuration d'intégration continue`",
            "- un **module d'outillage** : `la configuration de la solution`, `la configuration d'intégration continue`, "
            "`le pipeline de publication npm`",
        ) + (
            "##### TB-060 — Publication automatisée du paquet npm\n\n"
            "| Champ | Valeur |\n|---|---|\n"
            "| Jalon | J1 — traversée verticale |\n"
            "| Périmètre d'écriture | le pipeline de publication npm |\n"
            "| Dépend de | — |\n"
            "| En conflit avec | — |\n"
        )
        _construire_depot(self.depot, plan)

    def tearDown(self):
        self._tmp.cleanup()

    @unittest.expectedFailure
    def test_un_cinquieme_libelle_d_outillage_ajoute_au_plan_devrait_etre_resolu_par_les_deux_outils(self):
        resultat_generateur = _executer_generateur(self.depot)
        # Prémisse, vraie aujourd'hui : le générateur (lecture dynamique du
        # §2) résout la tâche sans réserve.
        self.assertIn("Tâches sans libellé de nature déductible (0)", resultat_generateur.stdout)

        resultat_verificateur = helpers.executer_script(
            "verif_maille_agregat.py", [self.depot, "docs/gestion-projet/plan-de-travail.md"]
        )
        # Invariant souhaité — ÉCHOUE aujourd'hui : le vérificateur (maille
        # codée en dur) refuse toujours le nouveau libellé.
        self.assertIn("item ne se résolvant pas la maille  : 0", resultat_verificateur.stdout)


class DivergenceD_VerrouDuNamespaceSurLaTrancheDeCreation(unittest.TestCase):
    """Divergence D — NON un défaut, une différence de MODÉLISATION — sur le
    verrou du namespace de bounded context :
      verif_maille_agregat.py:117-118  implémente la clause du §2 « sur la
                                       seule tranche qui les crée » : un
                                       namespace (IdentityAccess,
                                       SpaceManagement, ContentLibrary,
                                       SessionConduct) ne résout que sur
                                       TB-003.
      creer-issues.py::mapper_nature  n'implémente PAS cette clause : sa
                                       résolution ne prend aucun paramètre
                                       d'identifiant de tâche, donc ne peut
                                       distinguer TB-003 d'aucune autre
                                       tranche.

    EN PRATIQUE, mesuré sur TB-003 elle-même (`Périmètre d'écriture =
    IdentityAccess`) : le vérificateur la compte résolue (« périmètre
    entièrement résolu : 1 », la clause du §2 appliquée) ; le générateur la
    classe sans libellé de nature déductible (« aucun segment reconnu dans
    le périmètre d'écriture ») — n'ayant pas la notion de namespace, il ne
    le résout NULLE PART, pas même sur la tranche qui le crée. Les deux
    outils désaccordent donc sur la tranche même que le §2 déclare sans
    ambiguïté légitime : ce n'est pas que le générateur « admette le
    namespace partout » (il ne le résout nulle part), c'est qu'il traite
    TB-003 comme n'importe quelle autre tranche, faute d'implémenter la
    clause d'exception que le vérificateur, lui, porte."""

    def setUp(self):
        self._tmp = tempfile.TemporaryDirectory()
        self.depot = self._tmp.name
        plan = PLAN_PREAMBULE + (
            "##### TB-003 — Création du namespace IdentityAccess\n\n"
            "| Champ | Valeur |\n|---|---|\n"
            "| Jalon | J0 — fondations |\n"
            "| Périmètre d'écriture | IdentityAccess |\n"
            "| Dépend de | — |\n"
            "| En conflit avec | — |\n"
        )
        _construire_depot(self.depot, plan)

    def tearDown(self):
        self._tmp.cleanup()

    @unittest.expectedFailure
    def test_le_namespace_cree_par_tb_003_devrait_etre_resolu_de_la_meme_facon_par_les_deux_outils(self):
        resultat_verificateur = helpers.executer_script(
            "verif_maille_agregat.py", [self.depot, "docs/gestion-projet/plan-de-travail.md"]
        )
        # Prémisse, vraie aujourd'hui : le vérificateur résout TB-003.
        self.assertIn("périmètre entièrement résolu        : 1", resultat_verificateur.stdout)

        resultat_generateur = _executer_generateur(self.depot)
        # Invariant souhaité — ÉCHOUE aujourd'hui : le générateur classe
        # TB-003 sans libellé de nature déductible.
        self.assertNotIn(
            "TB-003 : aucun segment reconnu dans le périmètre d'écriture",
            resultat_generateur.stdout,
        )


if __name__ == "__main__":
    unittest.main()
