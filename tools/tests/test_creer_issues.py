"""Tests de caractérisation — mode Analyse `+refacto` — pour
`tools/creer-issues.py`.

Ce script appelle `gh` (réseau) et lit l'état d'un dépôt distant qui peut
changer : sa sortie n'est pas reproductible dans le temps. Ce qui EST
reproductible, et donc caractérisable en golden master, est isolé ici :

  - la construction du corps d'issue (`construire_corps_issue` et les
    fonctions dont elle dépend) — pure, sans I/O, testée par appel direct ;
  - les deux régimes du mode à blanc face à une révision non poussée
    (avertissement / silence) — testés en bout en bout, mais en mode à blanc
    STRICT : `--appliquer` n'est invoqué NULLE PART dans ce fichier, et `gh`
    est rendu injoignable (PATH sans `gh`) pour garantir qu'aucun appel réseau
    vers l'API GitHub n'est jamais tenté, quel que soit l'état d'authentification
    `gh` de la machine hôte.

Aucun de ces tests n'écrit dans le dépôt réel : chaque dépôt de fixture est un
répertoire git temporaire et jetable, jamais `tools/` ni le dépôt suivi.
"""
import os
import sys
import tempfile
import unittest

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
import helpers

CI = helpers.charger_module("creer-issues.py", "outil_issues_sous_test")

TB_ID = "TB-014"
URL_BLOB_PLAN = (
    "https://github.com/exemple-org/exemple-depot/blob/"
    "a3f9c21e7b6d4185f2309ce77ac1b4e2d8f9a001/docs/gestion-projet/plan-de-travail.md"
)
REVISION_PLAN = "a3f9c21e7b6d4185f2309ce77ac1b4e2d8f9a001"
CHAMPS_NOMINAUX = {"Dépend de": "TB-003", "En conflit avec": "—"}
NUMERO_ISSUE_PAR_TB_NOMINAL = {"TB-003": 42}


# ──────────────────────────────────────────────────────────────────────────
# Construction du corps d'issue — fonctions pures, appelées directement
# ──────────────────────────────────────────────────────────────────────────

class ConstructionDuCorpsNominal(unittest.TestCase):
    """Comportement n°6 — le corps porte le renvoi sur une révision immuable
    (jamais la branche), `Dépend de` et `En conflit avec` projetés en renvois,
    l'estampille de provenance, le marqueur de séparation, puis la zone
    manuelle par défaut quand aucun corps n'existait déjà."""

    def test_devrait_construire_la_zone_engendree_avec_les_renvois_et_l_estampille_de_provenance(self):
        corps = CI.construire_corps_issue(
            TB_ID, URL_BLOB_PLAN, REVISION_PLAN, CHAMPS_NOMINAUX,
            NUMERO_ISSUE_PAR_TB_NOMINAL, corps_existant=None,
        )
        self.assertIn(
            f"**Tâche de plan** : [`docs/gestion-projet/plan-de-travail.md`]({URL_BLOB_PLAN}) — {TB_ID}",
            corps,
        )
        self.assertIn("**Dépend de** : TB-003 (#42)", corps)
        self.assertIn("**En conflit avec** : —", corps)
        self.assertIn(
            f"**Estampille de provenance** : docs/gestion-projet/plan-de-travail.md@{REVISION_PLAN} — {TB_ID}",
            corps,
        )
        self.assertIn(CI.MARQUEUR_SEPARATION_CORPS, corps)
        self.assertTrue(corps.endswith(CI.ZONE_MANUELLE_PAR_DEFAUT))


class RenvoisDeTranches(unittest.TestCase):
    """Comportement n°9 — une tranche référencée sans issue rend `TB-nnn (pas
    encore d'issue)`, jamais un lien mort ni un numéro inventé. Bornes : champ
    vide et doublon d'identifiant dans un même champ."""

    def test_devrait_rendre_pas_encore_d_issue_pour_une_tranche_sans_numero_connu(self):
        rendu = CI.construire_renvois_tranches(["TB-099"], {})
        self.assertEqual(rendu, "TB-099 (pas encore d'issue)")

    def test_devrait_rendre_un_tiret_quand_aucune_tranche_n_est_referencee(self):
        rendu = CI.construire_renvois_tranches([], {})
        self.assertEqual(rendu, "—")

    def test_ne_devrait_pas_dupliquer_le_renvoi_quand_le_meme_identifiant_apparait_deux_fois(self):
        rendu = CI.construire_renvois_tranches(["TB-003", "TB-003"], {"TB-003": 10})
        self.assertEqual(rendu, "TB-003 (#10)")


class PreservationDeLaZoneManuelle(unittest.TestCase):
    """Comportement n°7 — le test le plus important des trois : un corps
    existant portant du texte sous le marqueur doit être régénéré sans que ce
    texte bouge d'un caractère. Si le marqueur est absent (corps antérieur au
    modèle), le corps existant est reporté intégralement, jamais écrasé."""

    def test_devrait_preserver_caractere_pour_caractere_le_texte_ecrit_a_la_main_sous_le_marqueur(self):
        ancien_corps = (
            "**Tâche de plan** : [`docs/gestion-projet/plan-de-travail.md`](url-perimee) — TB-014\n"
            "**Estampille de provenance** : ancienne révision, à écraser\n\n"
            + CI.MARQUEUR_SEPARATION_CORPS
            + "\n\n## Pris par\n\nPierre-Marie, à partir du 2026-08-20.\n\n"
              "## Notes d'exécution\n\nBloqué par la disponibilité de l'environnement de recette.\n\n"
              "## Écart constaté\n\nAucun à ce jour.\n"
        )
        zone_manuelle_attendue = ancien_corps[
            ancien_corps.find(CI.MARQUEUR_SEPARATION_CORPS) + len(CI.MARQUEUR_SEPARATION_CORPS):
        ]

        nouveau_corps = CI.construire_corps_issue(
            TB_ID, URL_BLOB_PLAN, REVISION_PLAN, CHAMPS_NOMINAUX,
            NUMERO_ISSUE_PAR_TB_NOMINAL, corps_existant=ancien_corps,
        )

        zone_manuelle_obtenue = nouveau_corps[
            nouveau_corps.find(CI.MARQUEUR_SEPARATION_CORPS) + len(CI.MARQUEUR_SEPARATION_CORPS):
        ]
        self.assertEqual(zone_manuelle_obtenue, zone_manuelle_attendue)

    def test_devrait_reporter_integralement_un_corps_anterieur_sans_marqueur_plutot_que_l_ecraser(self):
        ancien_corps_sans_marqueur = (
            "Notes libres antérieures au modèle de corps, écrites avant que ce "
            "script n'existe.\nDeuxième ligne, à conserver aussi."
        )
        nouveau_corps = CI.construire_corps_issue(
            TB_ID, URL_BLOB_PLAN, REVISION_PLAN, CHAMPS_NOMINAUX,
            NUMERO_ISSUE_PAR_TB_NOMINAL, corps_existant=ancien_corps_sans_marqueur,
        )
        attendu = "\n\n" + ancien_corps_sans_marqueur.strip("\n") + "\n"
        self.assertTrue(nouveau_corps.endswith(attendu))


class Idempotence(unittest.TestCase):
    """Comportement n°8 — régénérer deux fois de suite sans changement du plan
    produit un résultat IDENTIQUE, pas seulement équivalent. Un défaut de
    non-idempotence (un saut de ligne dupliqué à chaque passe) a été trouvé et
    corrigé le 2026-09-03 : ce test aurait dû le voir rougir."""

    def test_devrait_produire_un_corps_identique_a_la_seconde_regeneration_sans_changement(self):
        corps_premiere_passe = CI.construire_corps_issue(
            TB_ID, URL_BLOB_PLAN, REVISION_PLAN, CHAMPS_NOMINAUX,
            NUMERO_ISSUE_PAR_TB_NOMINAL, corps_existant=None,
        )
        corps_seconde_passe = CI.construire_corps_issue(
            TB_ID, URL_BLOB_PLAN, REVISION_PLAN, CHAMPS_NOMINAUX,
            NUMERO_ISSUE_PAR_TB_NOMINAL, corps_existant=corps_premiere_passe,
        )
        self.assertEqual(corps_premiere_passe, corps_seconde_passe)


# ──────────────────────────────────────────────────────────────────────────
# Régimes du mode à blanc face à la joignabilité de la révision (bout en bout)
# ──────────────────────────────────────────────────────────────────────────

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

PLAN_MD = """# Plan de travail (fixture de test)

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

##### TB-014 — Création d'un personnage joueur

| Champ | Valeur |
|---|---|
| Jalon | J1 — traversée verticale |
| Périmètre d'écriture | Personnage |
| Dépend de | — |
| En conflit avec | — |

---

##### TB-091 — Écran de création de personnage

| Champ | Valeur |
|---|---|
| Jalon | J1 — traversée verticale |
| Périmètre d'écriture | fiche `creation-personnage` |
| Dépend de | TB-014 — module partagé : Personnage |
| En conflit avec | — |
"""

DOMAINE_MD = """# Personnage (domaine)

## Agrégats

### Personnage

Racine d'agrégat, décrit un personnage joueur.

### Scenario (référence)

Référence vers un autre agrégat.
"""


def _construire_depot(depot, pousser):
    """Dépôt git jetable portant un plan et une méthode de ticket minimaux
    mais bien formés (les extracteurs du script exigent une structure
    précise). `origin` pointe vers un dépôt fictif jamais contacté : `gh` est
    rendu injoignable par le PATH du sous-processus, donc aucune requête —
    même en lecture — n'est jamais tentée contre l'API GitHub réelle."""
    helpers.initialiser_depot_git(depot)
    helpers.ecrire(os.path.join(depot, "docs/gestion-projet/plan-de-travail.md"), PLAN_MD)
    helpers.ecrire(os.path.join(depot, "docs/gestion-projet/methode-de-ticket.md"), METHODE_MD)
    helpers.ecrire(os.path.join(depot, "docs/conception/domain/personnage.md"), DOMAINE_MD)
    helpers.ecrire(
        os.path.join(depot, "docs/conception/interface/wireframes/sv1/creation-personnage/maquette.txt"),
        "placeholder\n",
    )
    commit = helpers.commiter_tout(depot, "corpus fixture pour creer-issues.py")
    helpers.executer_git(depot, "remote", "add", "origin", "git@github.com:exemple-org/exemple-depot.git")
    if pousser:
        branche = helpers.executer_git(depot, "rev-parse", "--abbrev-ref", "HEAD").stdout.strip()
        # Crée la référence de suivi distant localement, sans aucun accès
        # réseau : c'est exactement ce que lirait `commit_present_sur_distant`
        # après un vrai `git fetch` d'une révision déjà poussée.
        helpers.executer_git(depot, "update-ref", f"refs/remotes/origin/{branche}", commit)
    return commit


class RegimesDeLaRevisionEnModeABlanc(unittest.TestCase):
    """Comportement n°10 (moitié mode à blanc) — le script avertit en mode à
    blanc quand la révision courante n'est pas joignable sur le distant, et
    reste silencieux sur ce point quand elle l'est déjà. Les deux régimes
    sont exercés ici sans jamais passer `--appliquer` (constrainte de
    session) et avec `gh` rendu injoignable (PATH restreint à `git` et
    `python3`), pour qu'aucun appel réseau — même en lecture — ne soit
    jamais tenté."""

    def setUp(self):
        self._tmp_depot = tempfile.TemporaryDirectory()
        self._tmp_bin = tempfile.TemporaryDirectory()
        self.depot = self._tmp_depot.name
        self.env = dict(os.environ)
        self.env["PATH"] = helpers.repertoire_bin_sans_gh(self._tmp_bin.name)

    def tearDown(self):
        self._tmp_depot.cleanup()
        self._tmp_bin.cleanup()

    def test_devrait_avertir_en_mode_a_blanc_quand_la_revision_n_est_pas_encore_joignable_sur_le_distant(self):
        _construire_depot(self.depot, pousser=False)
        resultat = helpers.executer_script("creer-issues.py", [], cwd=self.depot, env=self.env)
        self.assertEqual(resultat.returncode, 0, resultat.stderr)
        self.assertIn("MODE À BLANC — rien n'est créé ni modifié sur GitHub.", resultat.stdout)
        self.assertIn("AVERTISSEMENT :", resultat.stdout)
        self.assertIn("n'est pas (encore) présente sur le distant", resultat.stdout)

    def test_ne_devrait_pas_avertir_quand_la_revision_est_deja_joignable_sur_le_distant(self):
        _construire_depot(self.depot, pousser=True)
        resultat = helpers.executer_script("creer-issues.py", [], cwd=self.depot, env=self.env)
        self.assertEqual(resultat.returncode, 0, resultat.stderr)
        self.assertNotIn("AVERTISSEMENT", resultat.stdout)


class CodeDeSortieSurErreurScript(unittest.TestCase):
    """Comportement n°10 (le code de sortie, cette fois réel) — à la
    différence des deux vérificateurs déterministes, `creer-issues.py` PORTE
    un vrai code de sortie : `main()` capture toute `ErreurScript` et rend 1
    (`sys.exit(main())`, avec message sur stderr, jamais de traceback Python).
    Exercé ici via un `methode-de-ticket.md` malformé (marqueur de taxonomie
    absent) — l'échec d'extraction survient avant toute lecture réseau ou
    tentative `gh`, donc sans risque même sans restreindre le PATH."""

    def setUp(self):
        self._tmp_depot = tempfile.TemporaryDirectory()
        self.depot = self._tmp_depot.name

    def tearDown(self):
        self._tmp_depot.cleanup()

    def test_devrait_rendre_le_code_un_et_un_message_d_erreur_sur_stderr_quand_la_methode_est_malformee(self):
        helpers.initialiser_depot_git(self.depot)
        helpers.ecrire(os.path.join(self.depot, "docs/gestion-projet/plan-de-travail.md"), PLAN_MD)
        # Méthode de ticket sans le marqueur « **Par jalon** » attendu par
        # extraire_taxonomie() — structure changée, extraction impossible.
        helpers.ecrire(
            os.path.join(self.depot, "docs/gestion-projet/methode-de-ticket.md"),
            "# Méthode de ticket (fixture volontairement malformée)\n\nAucune taxonomie ici.\n",
        )
        helpers.commiter_tout(self.depot, "méthode malformée")

        resultat = helpers.executer_script("creer-issues.py", [], cwd=self.depot)

        self.assertEqual(resultat.returncode, 1)
        self.assertIn("Erreur :", resultat.stderr)
        self.assertEqual(resultat.stdout.strip(), "")


if __name__ == "__main__":
    unittest.main()
