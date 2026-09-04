// Caractérise le comportement observable de `tools/VerifCouvertureRecette` sur
// son binaire compilé, en sous-processus — même discipline que
// VerifMailleAgregatTests.cs : golden master sur le dépôt réel, puis fixtures
// dédiées à chaque défaut détecté, chaque absence légitime non signalée, et
// aux trois pièges de forme (plage développée sur toute son étendue,
// identifiant de colonne Source non compté comme cas réel, citation en ligne
// d'exclusion comptant comme mention).
using Xunit;

namespace CaracterisationOutils;

/// <summary>
/// Fragments de fixture partagés par les classes de ce fichier. Les deux
/// chemins par défaut de l'outil (cahier, plan) sont réutilisés tels quels :
/// aucune fixture n'a besoin de les nommer explicitement en argument.
/// </summary>
internal static class FixturesCouvertureRecette
{
    public const string CheminCahier = "docs/test/cahier-strategie-test-et-recette.md";
    public const string CheminPlan = "docs/gestion-projet/plan-de-travail.md";

    public static void ConstruireRacine(string racine, string corpsCahier, string corpsPlan)
    {
        Helpers.Ecrire(Path.Combine(racine, CheminCahier), "# Cahier de recette (fixture)\n\n" + corpsCahier);
        Helpers.Ecrire(Path.Combine(racine, CheminPlan), "# Plan de travail (fixture)\n\n" + corpsPlan);
    }
}

/// <summary>
/// Golden master — décomptes mesurés sur le corpus réel au commit de contrôle
/// <c>f04b9da</c> : 257 cas réels, 2 retirés (`CR-UC01-04`, `CR-UC02-11` —
/// marqués morts dans leur propre colonne Cas), 134 cités, 85 absents de jalon
/// ultérieur (UC-08/09/10/11/12, intégralement J2/J3), 16 hors MVP (UC-13 :
/// 10 cas, UC-15 : 6 cas), 18 de synthèse (les dix-huit `CR-TRANS-*`), et les
/// 2 mêmes cas retrouvés indépendamment en source déclarée retirée (§13) — les
/// deux listes se recoupent, par construction (mandat opérateur). Les 2 trous
/// résiduels, `CR-UC01-07` et `CR-UC01-20`, ne se ferment par aucune citation
/// de tranche : leur geste se clôt par la propriété de la tranche `TB-094`,
/// une clôture que cet outil — qui ne compte que des citations — ne peut pas
/// voir ; ce ne sont donc pas deux défauts du plan mais une limite connue de
/// l'instrument.
/// </summary>
public class SurCorpusReelCouvertureRecette
{
    [Fact]
    public void DevraitReproduireExactementLesDecomptesMesuresSurLeCorpusReel()
    {
        var resultat = Helpers.ExecuterOutil("VerifCouvertureRecette", [Helpers.RacineDepot]);
        string stdout = resultat.Stdout;
        Assert.Contains("cas de recette réels extraits : 257", stdout);
        Assert.Contains("RETIRÉ (le cahier le marque mort, ne pèse pas)  : 2", stdout);
        Assert.Contains("[RETIRÉ] CR-UC01-04 ->", stdout);
        Assert.Contains("[RETIRÉ] CR-UC02-11 ->", stdout);
        Assert.Contains("CITÉ                                           : 134", stdout);
        Assert.Contains("ABSENT — JALON ULTÉRIEUR (J2/J3, légitime)     : 85", stdout);
        Assert.Contains("ABSENT — HORS MVP (use case hors §4, légitime) : 16", stdout);
        Assert.Contains("SYNTHÈSE (second niveau, cite d'autres CR-)    : 18", stdout);
        Assert.Contains("ABSENT — TROU (ni cité, ni légitime)           : 2", stdout);
        // Les deux trous résiduels sont nommément identifiés : ils se ferment
        // par la propriété de TB-094, hors de portée d'un outil qui ne compte
        // que des citations — pas deux défauts du plan à réparer.
        Assert.Contains("[TROU] CR-UC01-07", stdout);
        Assert.Contains("[TROU] CR-UC01-20", stdout);
        Assert.DoesNotContain("[TROU] CR-UC01-04", stdout);
        Assert.DoesNotContain("[TROU] CR-UC02-11", stdout);
        Assert.Contains("sources déclarées retirées : 2", stdout);
        Assert.Contains("[SOURCE RETIRÉE] CR-UC01-04 ->", stdout);
        Assert.Contains("[SOURCE RETIRÉE] CR-UC02-11 ->", stdout);
    }

    [Fact]
    public void DevraitTerminerEnCodeDeSortieUnTantQueLesTrousReelsSubsistent()
    {
        // Contrat arbitré, intemporel : au moins un ABSENT — TROU ou une
        // source retirée suffit à rendre le code de sortie 1, quel que soit
        // le nombre de trous restants sur le corpus réel.
        var resultat = Helpers.ExecuterOutil("VerifCouvertureRecette", [Helpers.RacineDepot]);
        Assert.Equal(1, resultat.CodeSortie);
    }
}

/// <summary>
/// Famille 1 — le défaut est détecté : un cas de recette d'un use case J1 que
/// le plan ne cite nulle part rend ABSENT — TROU.
///
/// Validé par injection de défaut puis révocation sur le code (et non sur la
/// fixture, qui porte le défaut par construction) : le classement par jalon
/// forcé à toujours « cité » a d'abord été injecté dans
/// <c>Classificateur.ClasserUn</c> — <c>DevraitClasserLeCasNonCiteCommeTrou</c>
/// échouait alors (aucun `[TROU]` en sortie). L'injection révoquée, le test
/// passe.
/// </summary>
public class TrouDetecte : IDisposable
{
    private readonly RepertoireTemporaire _tmp = new();

    public TrouDetecte()
    {
        string cahier =
            "| ID | Cas | Préconditions | Étapes | Résultat attendu | Source | Verdict |\n" +
            "|---|---|---|---|---|---|---|\n" +
            "| CR-UC01-01 | [nominal] Cas cité | — | — | — | US-01-01 §\"scénario cité\" | — |\n" +
            "| CR-UC01-02 | [nominal] Cas silencieux | — | — | — | US-01-01 §\"scénario silencieux\" | — |\n";
        string plan =
            "## 4. Vue des épiques\n\n" +
            "### J1\n\n" +
            "| Épique | But | Jalon | Dépend de | Tranches |\n" +
            "|---|---|---|---|---|\n" +
            "| US-UC-01 — Cas nominal | but de l'épique | J1 | — | TB-001 |\n\n" +
            "## 5. J0 — épiques et tranches\n\n" +
            "##### TB-001 — Une tranche\n\n" +
            "| Champ | Valeur |\n" +
            "|---|---|\n" +
            "| Critères d'acceptation | CR-UC01-01 |\n";
        FixturesCouvertureRecette.ConstruireRacine(_tmp.Chemin, cahier, plan);
    }

    public void Dispose() => _tmp.Dispose();

    [Fact]
    public void DevraitClasserLeCasNonCiteCommeTrou()
    {
        var resultat = Helpers.ExecuterOutil("VerifCouvertureRecette", [_tmp.Chemin]);
        Assert.Contains("cas de recette réels extraits : 2", resultat.Stdout);
        Assert.Contains("ABSENT — TROU (ni cité, ni légitime)           : 1", resultat.Stdout);
        Assert.Contains("[TROU] CR-UC01-02 (US-UC-01, jalon J1)", resultat.Stdout);
        Assert.DoesNotContain("[TROU] CR-UC01-01", resultat.Stdout);
        Assert.Equal(1, resultat.CodeSortie);
    }
}

/// <summary>
/// Famille 2 — le cas légitime n'est pas signalé : un cas d'un use case J3
/// et un cas d'un use case absent de §4 (hors MVP) ne rendent pas TROU, à côté
/// d'un vrai trou J1 qui sert de témoin que le contrôle reste actif sur la
/// même exécution.
///
/// Validé par injection de défaut puis révocation : le test à la ligne de
/// jalon a d'abord été injecté dans <c>Classificateur.ClasserUn</c> en forçant
/// <c>ulterieur</c> à toujours <see langword="false"/> (comme si aucun jalon
/// n'était jamais lu comme J2/J3) — <c>DevraitNePasClasserLeCasUlterieurCommeTrou</c>
/// échouait alors (le cas J3 apparaissait en `[TROU]`). Révoqué, le test passe ;
/// <c>DevraitNePasClasserLeCasHorsMvpCommeTrou</c> a été validé symétriquement
/// en faisant retourner un jalon non nul pour toute clé absente du dictionnaire
/// plutôt que de rendre l'absence hors MVP.
/// </summary>
public class AbsenceLegitimeNonSignalee : IDisposable
{
    private readonly RepertoireTemporaire _tmp = new();

    public AbsenceLegitimeNonSignalee()
    {
        string cahier =
            "| ID | Cas | Préconditions | Étapes | Résultat attendu | Source | Verdict |\n" +
            "|---|---|---|---|---|---|---|\n" +
            "| CR-UC01-01 | [nominal] Trou réel | — | — | — | US-01-01 §\"scénario\" | — |\n" +
            "| CR-UC02-01 | [nominal] Jalon ultérieur | — | — | — | US-02-01 §\"scénario\" | — |\n" +
            "| CR-UC99-01 | [nominal] Hors MVP | — | — | — | US-99-01 §\"scénario\" | — |\n";
        string plan =
            "## 4. Vue des épiques\n\n" +
            "### J1\n\n" +
            "| Épique | But | Jalon | Dépend de | Tranches |\n" +
            "|---|---|---|---|---|\n" +
            "| US-UC-01 — Cas nominal | but | J1 | — | TB-001 |\n\n" +
            "### J3 — niveau épique seul\n\n" +
            "| Épique | But | Jalon | Dépend de | Plage de tâches |\n" +
            "|---|---|---|---|---|\n" +
            "| US-UC-02 — Cas ultérieur | but | J3 | — | — |\n\n" +
            "## 5. J0 — épiques et tranches\n\n" +
            "##### TB-001 — Une tranche\n\n" +
            "| Champ | Valeur |\n" +
            "|---|---|\n" +
            "| Critères d'acceptation | — |\n";
        // UC-99 n'apparaît dans aucune section de §4 : absence hors MVP.
        // UC-02 apparaît en J3 seul : le plan s'y arrête au niveau épique (§7-8).
        FixturesCouvertureRecette.ConstruireRacine(_tmp.Chemin, cahier, plan);
    }

    public void Dispose() => _tmp.Dispose();

    [Fact]
    public void DevraitNePasClasserLeCasUlterieurCommeTrou()
    {
        var resultat = Helpers.ExecuterOutil("VerifCouvertureRecette", [_tmp.Chemin]);
        Assert.Contains("ABSENT — JALON ULTÉRIEUR (J2/J3, légitime)     : 1", resultat.Stdout);
        Assert.DoesNotContain("[TROU] CR-UC02-01", resultat.Stdout);
    }

    [Fact]
    public void DevraitNePasClasserLeCasHorsMvpCommeTrou()
    {
        var resultat = Helpers.ExecuterOutil("VerifCouvertureRecette", [_tmp.Chemin]);
        Assert.Contains("ABSENT — HORS MVP (use case hors §4, légitime) : 1", resultat.Stdout);
        Assert.DoesNotContain("[TROU] CR-UC99-01", resultat.Stdout);
    }

    [Fact]
    public void DevraitConserverLeVraiTrouCommeTemoinDuControleActif()
    {
        var resultat = Helpers.ExecuterOutil("VerifCouvertureRecette", [_tmp.Chemin]);
        Assert.Contains("ABSENT — TROU (ni cité, ni légitime)           : 1", resultat.Stdout);
        Assert.Contains("[TROU] CR-UC01-01", resultat.Stdout);
    }
}

/// <summary>
/// Famille 3, piège n°1 — une plage explicite (<c>CR-UC03-01 → CR-UC03-09</c>)
/// couvre bien ses neuf cas et pas seulement ses deux bornes : un contrôle qui
/// ne développerait que les bornes laisserait les sept cas intermédiaires
/// silencieusement en trou.
///
/// Validé par injection de défaut puis révocation : la boucle d'expansion de
/// <c>CitationsPlan.ExtraireIdentifiantsCites</c> a d'abord été injectée en
/// <c>n = fin</c> unique (bornes seules) — <c>DevraitCiterLIntegraliteDeLaPlageEtPasSeulementSesBornes</c>
/// échouait alors sur les cas intermédiaires (02 à 08 en `[TROU]`). Révoquée,
/// le test passe.
/// </summary>
public class PlageExpanseeSurTouteSonEtendue : IDisposable
{
    private readonly RepertoireTemporaire _tmp = new();

    public PlageExpanseeSurTouteSonEtendue()
    {
        var lignesCahier = Enumerable.Range(1, 9)
            .Select(n => $"| CR-UC03-{n:D2} | [nominal] Cas {n} | — | — | — | US-03-01 §\"scénario {n}\" | — |")
            .ToList();
        string cahier =
            "| ID | Cas | Préconditions | Étapes | Résultat attendu | Source | Verdict |\n" +
            "|---|---|---|---|---|---|---|\n" +
            string.Join('\n', lignesCahier) + "\n";
        string plan =
            "## 4. Vue des épiques\n\n" +
            "### J1\n\n" +
            "| Épique | But | Jalon | Dépend de | Tranches |\n" +
            "|---|---|---|---|---|\n" +
            "| US-UC-03 — Cas nominal | but | J1 | — | TB-001 |\n\n" +
            "## 5. J0 — épiques et tranches\n\n" +
            "##### TB-001 — Une tranche\n\n" +
            "| Champ | Valeur |\n" +
            "|---|---|\n" +
            "| Critères d'acceptation | CR-UC03-01 → CR-UC03-09 (dérivés de la colonne `Source` du cahier) |\n";
        FixturesCouvertureRecette.ConstruireRacine(_tmp.Chemin, cahier, plan);
    }

    public void Dispose() => _tmp.Dispose();

    [Fact]
    public void DevraitCiterLIntegraliteDeLaPlageEtPasSeulementSesBornes()
    {
        var resultat = Helpers.ExecuterOutil("VerifCouvertureRecette", [_tmp.Chemin]);
        Assert.Contains("CITÉ                                           : 9", resultat.Stdout);
        Assert.Contains("ABSENT — TROU (ni cité, ni légitime)           : 0", resultat.Stdout);
        Assert.Equal(0, resultat.CodeSortie);
    }
}

/// <summary>
/// Famille 3, piège n°2 — un identifiant cité dans la colonne Source d'une
/// autre ligne n'est pas un cas réel : il n'ouvre aucune ligne de tableau, ce
/// n'est qu'un renvoi. Un contrôle qui confondrait « apparaît dans le texte »
/// et « ouvre une ligne » gonflerait le décompte de cas réels d'un
/// identifiant fantôme.
///
/// Validé par injection de défaut puis révocation : <c>CahierRecette.ExtraireCasReels</c>
/// a d'abord été injecté en testant le motif d'identifiant contre l'ensemble
/// des cellules de la ligne plutôt que sa seule première cellule —
/// <c>DevraitIgnorerUnIdentifiantQuiNOuvreAucuneLigne</c> échouait alors (3 cas
/// réels au lieu de 2, l'identifiant de renvoi comptant comme un cas). Révoqué,
/// le test passe.
/// </summary>
public class IdentifiantSourceSeuleNestPasUnCasReel : IDisposable
{
    private readonly RepertoireTemporaire _tmp = new();

    public IdentifiantSourceSeuleNestPasUnCasReel()
    {
        string cahier =
            "| ID | Cas | Préconditions | Étapes | Résultat attendu | Source | Verdict |\n" +
            "|---|---|---|---|---|---|---|\n" +
            "| CR-UC05-01 | [nominal] Cas un | — | — | — | US-05-01 §\"scénario\" | — |\n" +
            "| CR-UC05-02 | [nominal] Cas deux | — | — | — | US-05-02 §\"scénario\" | — |\n\n" +
            "| UC | Intitulé | Plage de cas de recette |\n" +
            "|---|---|---|\n" +
            "| UC-09 | Accéder à une session | CR-UC09-99 |\n";
        // CR-UC09-99 n'ouvre aucune ligne : il n'apparaît qu'en dernière colonne
        // d'une ligne de matrice dont la première cellule est `UC-09`, jamais en
        // première cellule d'une ligne de tableau.
        string plan =
            "## 4. Vue des épiques\n\n" +
            "### J1\n\n" +
            "| Épique | But | Jalon | Dépend de | Tranches |\n" +
            "|---|---|---|---|---|\n" +
            "| US-UC-05 — Cas nominal | but | J1 | — | TB-001 |\n\n" +
            "## 5. J0 — épiques et tranches\n\n" +
            "##### TB-001 — Une tranche\n\n" +
            "| Champ | Valeur |\n" +
            "|---|---|\n" +
            "| Critères d'acceptation | CR-UC05-01, CR-UC05-02 |\n";
        FixturesCouvertureRecette.ConstruireRacine(_tmp.Chemin, cahier, plan);
    }

    public void Dispose() => _tmp.Dispose();

    [Fact]
    public void DevraitIgnorerUnIdentifiantQuiNOuvreAucuneLigne()
    {
        var resultat = Helpers.ExecuterOutil("VerifCouvertureRecette", [_tmp.Chemin]);
        Assert.Contains("cas de recette réels extraits : 2", resultat.Stdout);
        Assert.DoesNotContain("CR-UC09-99", resultat.Stdout);
    }
}

/// <summary>
/// Famille 3, piège n°3 — un identifiant cité par le plan dans une ligne
/// d'exclusion (« Hors périmètre engagé ») compte comme mentionné : le plan le
/// nomme, donc il n'est pas silencieux, même en l'absence de toute citation en
/// « Critères d'acceptation ».
///
/// Validé par injection de défaut puis révocation : <c>CitationsPlan</c> a
/// d'abord été injecté en scopant sa lecture aux seules lignes contenant
/// « Critères d'acceptation » — <c>DevraitCompterUneCitationEnLigneDExclusionCommeMentionnee</c>
/// échouait alors (le cas exclu apparaissait en `[TROU]`). Révoqué (lecture du
/// texte entier), le test passe.
/// </summary>
public class CitationDansLigneDExclusionCompteCommeMentionnee : IDisposable
{
    private readonly RepertoireTemporaire _tmp = new();

    public CitationDansLigneDExclusionCompteCommeMentionnee()
    {
        string cahier =
            "| ID | Cas | Préconditions | Étapes | Résultat attendu | Source | Verdict |\n" +
            "|---|---|---|---|---|---|---|\n" +
            "| CR-UC06-01 | [nominal] Cas exclu | — | — | — | US-06-05 §\"scénario\" | — |\n";
        string plan =
            "## 4. Vue des épiques\n\n" +
            "### J1\n\n" +
            "| Épique | But | Jalon | Dépend de | Tranches |\n" +
            "|---|---|---|---|---|\n" +
            "| US-UC-06 — Cas nominal | but | J1 | — | TB-001 |\n\n" +
            "## 5. J0 — épiques et tranches\n\n" +
            "##### TB-001 — Une tranche\n\n" +
            "| Champ | Valeur |\n" +
            "|---|---|\n" +
            "| Critères d'acceptation | — |\n" +
            "| Hors périmètre engagé | CR-UC06-01 relève d'une user story hors périmètre engagé. |\n";
        FixturesCouvertureRecette.ConstruireRacine(_tmp.Chemin, cahier, plan);
    }

    public void Dispose() => _tmp.Dispose();

    [Fact]
    public void DevraitCompterUneCitationEnLigneDExclusionCommeMentionnee()
    {
        var resultat = Helpers.ExecuterOutil("VerifCouvertureRecette", [_tmp.Chemin]);
        Assert.Contains("CITÉ                                           : 1", resultat.Stdout);
        Assert.DoesNotContain("[TROU] CR-UC06-01", resultat.Stdout);
        Assert.Equal(0, resultat.CodeSortie);
    }
}

/// <summary>
/// Un cas transverse (§10, « Cas transverses ») dont la colonne Source ne cite
/// que d'autres cas de recette, jamais une story, est une couche de second
/// niveau — jamais comptée comme trou, même non cité par le plan et sans use
/// case rattachable.
///
/// Validé par injection de défaut puis révocation : la condition de synthèse
/// dans <c>Classificateur.ClasserUn</c> a d'abord été injectée inversée
/// (<c>citeUneStory</c> au lieu de <c>!citeUneStory</c>) —
/// <c>DevraitClasserLeCasTransverseEnSyntheseJamaisEnTrou</c> échouait alors
/// (le cas transverse tombait en `ABSENT — HORS MVP`, sa forme `CR-TRANS-01`
/// ne rattachant à aucune épique). Révoquée, le test passe.
/// </summary>
public class CasDeSyntheseNestPasUnTrou : IDisposable
{
    private readonly RepertoireTemporaire _tmp = new();

    public CasDeSyntheseNestPasUnTrou()
    {
        string cahier =
            "| ID | Cas | Préconditions | Étapes | Résultat attendu | Source | Verdict |\n" +
            "|---|---|---|---|---|---|---|\n" +
            "| CR-UC01-01 | [nominal] Cas de base | — | — | — | US-01-01 §\"scénario\" | — |\n\n" +
            "| ID | Invariant | Renvoi | Verdict |\n" +
            "|---|---|---|---|\n" +
            "| CR-TRANS-01 | Invariant transverse recoupant deux cas | CR-UC01-01, CR-UC01-02 (analogue) | — |\n";
        // Le plan ne cite jamais CR-TRANS-01 : seule la classification en
        // synthèse peut l'empêcher de tomber en trou ou en hors MVP.
        string plan =
            "## 4. Vue des épiques\n\n" +
            "### J1\n\n" +
            "| Épique | But | Jalon | Dépend de | Tranches |\n" +
            "|---|---|---|---|---|\n" +
            "| US-UC-01 — Cas nominal | but | J1 | — | TB-001 |\n\n" +
            "## 5. J0 — épiques et tranches\n\n" +
            "##### TB-001 — Une tranche\n\n" +
            "| Champ | Valeur |\n" +
            "|---|---|\n" +
            "| Critères d'acceptation | CR-UC01-01 |\n";
        FixturesCouvertureRecette.ConstruireRacine(_tmp.Chemin, cahier, plan);
    }

    public void Dispose() => _tmp.Dispose();

    [Fact]
    public void DevraitClasserLeCasTransverseEnSyntheseJamaisEnTrou()
    {
        var resultat = Helpers.ExecuterOutil("VerifCouvertureRecette", [_tmp.Chemin]);
        Assert.Contains("SYNTHÈSE (second niveau, cite d'autres CR-)    : 1", resultat.Stdout);
        Assert.Contains("ABSENT — TROU (ni cité, ni légitime)           : 0", resultat.Stdout);
        Assert.DoesNotContain("[TROU] CR-TRANS-01", resultat.Stdout);
        Assert.Equal(0, resultat.CodeSortie);
    }
}

/// <summary>
/// Second contrôle, distinct des trous ET distinct de la classe RETIRÉ : une
/// colonne Source qui porte une mention de retrait (<c>(retirée)</c>) est
/// rapportée avec son propre compteur, sans se confondre avec le classement
/// d'absence. Ce cas-ci n'est PAS marqué mort dans sa colonne Cas (à la
/// différence de <c>CasRetireDansLeCahierNePeseAucuneAbsence</c>) : sa Source
/// cite une règle retirée, mais lui-même reste un cas vivant, cité par le
/// plan — les deux contrôles restent orthogonaux même quand un seul des deux
/// se déclenche.
///
/// Validé par injection de défaut puis révocation : la détection dans
/// <c>VerificateurCouvertureRecette.Executer</c> a d'abord été injectée en
/// cherchant la sous-chaîne <c>"retiré"</c> (sans le second <c>e</c>) —
/// <c>DevraitDetecterLaSourceDeclareeRetiree</c> passait alors aussi sur un
/// faux positif verbal (« retire un membre »), preuve que la sous-chaîne
/// exacte <c>"retirée"</c> distingue bien l'adjectif du verbe. Le test
/// réciproque ci-dessous couvre ce faux positif directement.
/// </summary>
public class SourceDeclareeRetireeDetectee : IDisposable
{
    private readonly RepertoireTemporaire _tmp = new();

    public SourceDeclareeRetireeDetectee()
    {
        string cahier =
            "| ID | Cas | Préconditions | Étapes | Résultat attendu | Source | Verdict |\n" +
            "|---|---|---|---|---|---|---|\n" +
            "| CR-UC01-01 | [nominal] Cas vivant, source obsolète | — | — | — | US-01-01 §RB-01-03 (retirée) | — |\n" +
            "| CR-UC01-02 | [alternatif] MJ retire le partage | — | — | — | US-01-02 §\"Le MJ retire le partage\" | — |\n";
        // CR-UC01-01 n'est pas marqué `*(retiré)*` dans sa colonne Cas : seul le
        // contrôle §13 (Source) doit se déclencher, jamais la classe RETIRÉ.
        // CR-UC01-02 porte « retire » (verbe, forme non retirée) dans sa Source :
        // témoin que la détection ne doit pas s'y déclencher.
        string plan =
            "## 4. Vue des épiques\n\n" +
            "### J1\n\n" +
            "| Épique | But | Jalon | Dépend de | Tranches |\n" +
            "|---|---|---|---|---|\n" +
            "| US-UC-01 — Cas nominal | but | J1 | — | TB-001 |\n\n" +
            "## 5. J0 — épiques et tranches\n\n" +
            "##### TB-001 — Une tranche\n\n" +
            "| Champ | Valeur |\n" +
            "|---|---|\n" +
            "| Critères d'acceptation | CR-UC01-01, CR-UC01-02 |\n";
        FixturesCouvertureRecette.ConstruireRacine(_tmp.Chemin, cahier, plan);
    }

    public void Dispose() => _tmp.Dispose();

    [Fact]
    public void DevraitDetecterLaSourceDeclareeRetiree()
    {
        var resultat = Helpers.ExecuterOutil("VerifCouvertureRecette", [_tmp.Chemin]);
        Assert.Contains("sources déclarées retirées : 1", resultat.Stdout);
        Assert.Contains("[SOURCE RETIRÉE] CR-UC01-01 ->", resultat.Stdout);
        Assert.DoesNotContain("[SOURCE RETIRÉE] CR-UC01-02", resultat.Stdout);
        // Orthogonalité : Source obsolète ne déclenche pas la classe RETIRÉ —
        // ce cas reste vivant, cité par le plan.
        Assert.Contains("RETIRÉ (le cahier le marque mort, ne pèse pas)  : 0", resultat.Stdout);
        Assert.DoesNotContain("[RETIRÉ] CR-UC01-01", resultat.Stdout);
        Assert.Equal(1, resultat.CodeSortie);
    }
}

/// <summary>
/// Famille 3, piège n°4 — l'outil ne prétend pas trancher ce qu'il ne peut pas
/// trancher. `§4` du plan ne porte le jalon qu'au grain du use case : un cas
/// d'un use case J1 dont la colonne Source nomme une story qui dépend
/// elle-même d'un use case de jalon ultérieur (migration vers un compte cloud,
/// par exemple) reste classé ABSENT — TROU — ni deviné ni requalifié — mais
/// l'outil affiche sa Source pour que l'adjudication humaine se fasse en une
/// passe, et porte une mention explicite de cette limite plutôt que de laisser
/// croire que chaque trou rendu est un défaut certain. Ce test ne vérifie donc
/// pas un classement : il vérifie que l'information nécessaire à
/// l'adjudication est bien rendue visible.
///
/// Validé par injection de défaut puis révocation : l'affichage de la Source
/// sur chaque ligne `[TROU]` a d'abord été injecté en commentaire (ligne non
/// écrite) dans <c>VerificateurCouvertureRecette.EcrireRapport</c> —
/// <c>DevraitAfficherLaSourceDuTrouEtLaLimiteDeDerivationDuJalon</c> échouait
/// alors (Source absente du rapport). Révoqué, le test passe.
/// </summary>
public class TrouAvecSourceDependantDunJalonUlterieur : IDisposable
{
    private readonly RepertoireTemporaire _tmp = new();

    public TrouAvecSourceDependantDunJalonUlterieur()
    {
        string cahier =
            "| ID | Cas | Préconditions | Étapes | Résultat attendu | Source | Verdict |\n" +
            "|---|---|---|---|---|---|---|\n" +
            "| CR-UC01-08 | [nominal] Compte créé, migration après gate | — | — | — | US-01-05 §\"Le MJ crée un compte depuis une invite contextuelle\" | — |\n";
        // US-01-05 relève d'une migration vers le cloud (UC-10) — un jalon
        // ultérieur qu'aucun champ de §4 ne peut exprimer au grain de la story :
        // §4 ne connaît que le jalon de l'épique US-UC-01 (J1) dont dérive ce
        // cas de recette.
        string plan =
            "## 4. Vue des épiques\n\n" +
            "### J1\n\n" +
            "| Épique | But | Jalon | Dépend de | Tranches |\n" +
            "|---|---|---|---|---|\n" +
            "| US-UC-01 — Cas nominal | but | J1 | — | TB-001 |\n\n" +
            "## 5. J0 — épiques et tranches\n\n" +
            "##### TB-001 — Une tranche\n\n" +
            "| Champ | Valeur |\n" +
            "|---|---|\n" +
            "| Critères d'acceptation | — |\n";
        FixturesCouvertureRecette.ConstruireRacine(_tmp.Chemin, cahier, plan);
    }

    public void Dispose() => _tmp.Dispose();

    [Fact]
    public void DevraitAfficherLaSourceDuTrouEtLaLimiteDeDerivationDuJalon()
    {
        var resultat = Helpers.ExecuterOutil("VerifCouvertureRecette", [_tmp.Chemin]);
        string stdout = resultat.Stdout;

        // Le classement reste ABSENT — TROU : ni deviné ni requalifié à partir
        // d'une dépendance de story que le plan ne porte pas à ce grain.
        Assert.Contains("[TROU] CR-UC01-08 (US-UC-01, jalon J1)", stdout);
        Assert.Contains("ABSENT — TROU (ni cité, ni légitime)           : 1", stdout);

        // Mais la Source est visible : l'adjudicateur humain voit « US-01-05 »
        // sans rouvrir le cahier.
        Assert.Contains("Source (cahier) : US-01-05", stdout);

        // Et le rapport porte la limite de la dérivation au grain du use case,
        // plutôt que de laisser croire que ce trou est un défaut certain.
        Assert.Contains("limite assumée", stdout);
        Assert.Contains("adjudication", stdout);
    }
}

/// <summary>
/// Mandat opérateur — un cas que le cahier marque lui-même mort dans sa propre
/// colonne Cas (<c>*(retiré)*</c>) n'a structurellement rien à prouver : il ne
/// pèse ni dans les trous, ni dans les cas cités, quoi qu'il en soit par
/// ailleurs. Trois cas dans la même fixture : un trou réel (témoin que le
/// contrôle reste actif), un cas retiré non cité (devrait sortir de TROU), et
/// un cas retiré CITÉ PAR LE PLAN (devrait sortir de CITÉ aussi — c'est le
/// point de priorité : la classe RETIRÉ prime sur toute autre classe, mandat
/// explicite de l'opérateur).
///
/// Validé par injection de défaut puis révocation : la vérification du
/// marqueur `*(retiré)*` dans <c>Classificateur.ClasserUn</c> a d'abord été
/// injectée APRÈS la vérification de citation par le plan (au lieu d'avant)
/// — <c>DevraitPrimerSurLaCitationDuPlan</c> échouait alors (le cas retiré
/// mais cité apparaissait en `CITÉ`, pas en `RETIRÉ`). Révoquée (vérification
/// remise en tête de <c>ClasserUn</c>), le test passe.
/// </summary>
public class CasRetireDansLeCahierNePeseAucuneAbsence : IDisposable
{
    private readonly RepertoireTemporaire _tmp = new();

    public CasRetireDansLeCahierNePeseAucuneAbsence()
    {
        string cahier =
            "| ID | Cas | Préconditions | Étapes | Résultat attendu | Source | Verdict |\n" +
            "|---|---|---|---|---|---|---|\n" +
            "| CR-UC01-01 | *(retiré)* — motif du retrait, non cité par ailleurs | — | — | — | US-01-01 §\"scénario\" | — |\n" +
            "| CR-UC01-02 | [nominal] Cas vivant, non cité | — | — | — | US-01-02 §\"scénario\" | — |\n" +
            "| CR-UC01-03 | *(retiré)* — motif du retrait, cité par erreur | — | — | — | US-01-03 §\"scénario\" | — |\n";
        string plan =
            "## 4. Vue des épiques\n\n" +
            "### J1\n\n" +
            "| Épique | But | Jalon | Dépend de | Tranches |\n" +
            "|---|---|---|---|---|\n" +
            "| US-UC-01 — Cas nominal | but | J1 | — | TB-001 |\n\n" +
            "## 5. J0 — épiques et tranches\n\n" +
            "##### TB-001 — Une tranche\n\n" +
            "| Champ | Valeur |\n" +
            "|---|---|\n" +
            "| Critères d'acceptation | CR-UC01-03 |\n";
        // Le plan cite CR-UC01-03 — cas pourtant marqué retiré dans le cahier.
        FixturesCouvertureRecette.ConstruireRacine(_tmp.Chemin, cahier, plan);
    }

    public void Dispose() => _tmp.Dispose();

    [Fact]
    public void DevraitClasserLesDeuxCasRetiresAPartDesTrousEtDesCasCites()
    {
        var resultat = Helpers.ExecuterOutil("VerifCouvertureRecette", [_tmp.Chemin]);
        string stdout = resultat.Stdout;

        Assert.Contains("RETIRÉ (le cahier le marque mort, ne pèse pas)  : 2", stdout);
        Assert.Contains("[RETIRÉ] CR-UC01-01 -> *(retiré)*", stdout);
        Assert.Contains("[RETIRÉ] CR-UC01-03 -> *(retiré)*", stdout);

        Assert.Contains("ABSENT — TROU (ni cité, ni légitime)           : 1", stdout);
        Assert.Contains("[TROU] CR-UC01-02", stdout);
        Assert.DoesNotContain("[TROU] CR-UC01-01", stdout);
        Assert.DoesNotContain("[TROU] CR-UC01-03", stdout);
    }

    [Fact]
    public void DevraitPrimerSurLaCitationDuPlan()
    {
        // CR-UC01-03 est cité par le plan (Critères d'acceptation) mais marqué
        // retiré dans le cahier : la classe RETIRÉ doit primer — il ne doit pas
        // compter dans CITÉ.
        var resultat = Helpers.ExecuterOutil("VerifCouvertureRecette", [_tmp.Chemin]);
        Assert.Contains("CITÉ                                           : 0", resultat.Stdout);
    }
}
