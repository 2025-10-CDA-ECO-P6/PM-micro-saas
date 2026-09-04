// Portage C# du script Python d'origine, `tools/tests/test_verif_maille_agregat.py` — caractérise le
// comportement observable de `tools/VerifMailleAgregat` sur son binaire
// compilé, en sous-processus, comme le faisait le script Python sur le sien.
//
// Trois différences assumées avec l'original, arbitrées par l'opérateur :
//
// 1. Le contrat de code de sortie n'est plus hors oracle (même arbitrage que
//    VerifCorpus, cf. VerifCorpusTests.cs) : `ContratCodeDeSortie` était
//    `@expectedFailure` côté Python, devient un test ordinaire ici.
// 2. Le code de sortie 2 (panne d'environnement — plan introuvable, UTF-8
//    invalide) est un comportement neuf, sans équivalent Python à porter.
// 3. Les deux compteurs de la partie B (§11 — disjonction fiches vivantes /
//    identifiants retirés) sont un comportement neuf du portage : le script
//    Python ne les calculait pas. `IdentifiantAmbigu` et
//    `RenvoiVersIdentifiantRetire` les couvrent dans les deux sens (le
//    golden master `SurPlanReel` couvre déjà le sens zéro sur données
//    réelles ; les deux classes ci-dessous couvrent le sens détection sur
//    fixtures dédiées).
using Xunit;

namespace CaracterisationOutils;

/// <summary>
/// Fragments de fixture partagés par plusieurs classes de ce fichier — mêmes
/// constantes que le script Python d'origine (cf. l'en-tête de ce fichier),
/// mêmes garde-fous rappelés en commentaire.
/// </summary>
internal static class FixturesMailleAgregat
{
    public const string ModeleDomaine = "# Personnage (domaine)\n\n## Agrégats\n\n### Personnage\n\nRacine d'agrégat.\n";

    // `ExtraireTaches` découpe sur `\n#{4,6}\s+(?=TB-)` : un match exige un `\n`
    // AVANT le marqueur de titre. Sans texte de préambule avant le premier
    // titre `##### TB-…`, ce premier titre n'est précédé d'aucun `\n` et son
    // bloc entier devient le [0] jeté par la boucle `for idx = 1..` —
    // silencieusement absorbé comme s'il s'agissait du préambule. D'où ce
    // préambule minimal, obligatoire dans toute fixture de plan pour cet
    // outil (comportement de l'outil, pas un choix de fixture arbitraire).
    public const string PreambulePlan = "# Plan de travail (fixture)\n\n§2 — Maille de désignation, non pertinente pour ce test.\n\n";

    public static void ConstruireRacine(string racine, string planMd, bool avecDomaine = true)
    {
        if (avecDomaine)
        {
            Helpers.Ecrire(Path.Combine(racine, "docs/conception/domain/personnage.md"), ModeleDomaine);
        }
        Helpers.Ecrire(Path.Combine(racine, "docs/gestion-projet/plan-de-travail.md"), PreambulePlan + planMd);
    }
}

/// <summary>
/// Golden master — sortie nominale mesurée sur le plan de travail réel au
/// commit de contrôle <c>f04b9da</c> : 61 tâches, 55 périmètres entièrement
/// résolus, 4 HORS MAILLE exclus, 1 TROU (non nommable), 1 en attente de J0,
/// 0 item non résolu, 0 écart de conflit — plus les deux compteurs neufs de
/// la partie B, mesurés également à zéro sur le plan réel.
/// </summary>
public class SurPlanReel
{
    [Fact]
    public void DevraitReproduireExactementLesDecomptesNominauxMesuresSurLePlanReel()
    {
        var resultat = Helpers.ExecuterOutil("VerifMailleAgregat", [Helpers.RacineDepot]);
        string stdout = resultat.Stdout;
        Assert.Contains("tâches du plan                        : 61", stdout);
        Assert.Contains("périmètre entièrement résolu        : 55", stdout);
        Assert.Contains("périmètre HORS MAILLE (exclu)       : 4", stdout);
        Assert.Contains("périmètre TROU (non nommable)       : 1", stdout);
        Assert.Contains("périmètre en attente de J0 (rôle)   : 1", stdout);
        Assert.Contains("item ne se résolvant pas la maille  : 0", stdout);
        Assert.Contains("écarts entre `En conflit avec` déclaré et l'intersection recalculée : 0", stdout);
        Assert.Contains("identifiants à la fois fiche vivante et retirés (intersection)     : 0", stdout);
        Assert.Contains("renvois `Dépend de` / `En conflit avec` vers un identifiant retiré : 0", stdout);
    }

    [Fact]
    public void DevraitTerminerEnCodeDeSortieZeroSurLePlanReel()
    {
        // Contrat neuf (absent du Python) : le plan réel ne porte aujourd'hui
        // aucun des quatre défauts arbitrés — le code de sortie doit le refléter.
        var resultat = Helpers.ExecuterOutil("VerifMailleAgregat", [Helpers.RacineDepot]);
        Assert.Equal(0, resultat.CodeSortie);
    }
}

/// <summary>
/// Comportement n°2 — HORS MAILLE est exclu de la liste des items, pas
/// seulement reconnu comme résoluble : une tranche qui le porte est comptée
/// à part, jamais dans « résolu ».
/// </summary>
public class ExclusionHorsMaille : IDisposable
{
    private readonly RepertoireTemporaire _tmp = new();

    public ExclusionHorsMaille()
    {
        string plan =
            "##### TB-014 — Création d'un personnage joueur\n\n" +
            "| Périmètre d'écriture | Personnage |\n" +
            "| En conflit avec | — |\n\n" +
            "##### TB-777 — Tâche sans code produit\n\n" +
            "| Périmètre d'écriture | HORS MAILLE — recette manuelle uniquement |\n" +
            "| En conflit avec | — |\n";
        FixturesMailleAgregat.ConstruireRacine(_tmp.Chemin, plan);
    }

    public void Dispose() => _tmp.Dispose();

    [Fact]
    public void DevraitExclureLaTacheHorsMailleDuDecompteResoluEtLaCompterAPart()
    {
        var resultat = Helpers.ExecuterOutil(
            "VerifMailleAgregat", [_tmp.Chemin, "docs/gestion-projet/plan-de-travail.md"]);
        string stdout = resultat.Stdout;
        Assert.Contains("tâches du plan                        : 2", stdout);
        Assert.Contains("périmètre entièrement résolu        : 1", stdout);
        Assert.Contains("périmètre HORS MAILLE (exclu)       : 1", stdout);
    }
}

/// <summary>
/// Comportement n°3 — deux tranches portant un libellé HORS MAILLE identique
/// ne produisent aucun conflit fantôme ; le zéro-conflit ne dépend pas de la
/// variété des libellés.
/// </summary>
public class ZeroConflitFantomeEntreHorsMailleIdentiques : IDisposable
{
    private readonly RepertoireTemporaire _tmp = new();

    public ZeroConflitFantomeEntreHorsMailleIdentiques()
    {
        const string libellePartage = "HORS MAILLE — recette manuelle uniquement, sans code produit";
        string plan =
            $"##### TB-777 — Recette manuelle du parcours d'accueil\n\n" +
            $"| Périmètre d'écriture | {libellePartage} |\n" +
            $"| En conflit avec | — |\n\n" +
            $"##### TB-778 — Recette manuelle du parcours de sortie\n\n" +
            $"| Périmètre d'écriture | {libellePartage} |\n" +
            $"| En conflit avec | — |\n";
        FixturesMailleAgregat.ConstruireRacine(_tmp.Chemin, plan, avecDomaine: false);
    }

    public void Dispose() => _tmp.Dispose();

    [Fact]
    public void NeDevraitProduireAucunEcartDeConflitEntreDeuxTachesHorsMailleAuLibelleIdentique()
    {
        var resultat = Helpers.ExecuterOutil(
            "VerifMailleAgregat", [_tmp.Chemin, "docs/gestion-projet/plan-de-travail.md"]);
        Assert.Contains(
            "écarts entre `En conflit avec` déclaré et l'intersection recalculée : 0", resultat.Stdout);
    }
}

/// <summary>
/// Comportement n°4 — un module que le corpus ne nomme pas est REFUSÉ, pas
/// absorbé.
/// </summary>
public class RejetDuModuleFantome : IDisposable
{
    private readonly RepertoireTemporaire _tmp = new();

    public RejetDuModuleFantome()
    {
        string plan =
            "##### TB-091 — Tâche au périmètre fantaisiste\n\n" +
            "| Périmètre d'écriture | le projet Domaine fantôme qui n'existe pas |\n" +
            "| En conflit avec | — |\n";
        FixturesMailleAgregat.ConstruireRacine(_tmp.Chemin, plan, avecDomaine: false);
    }

    public void Dispose() => _tmp.Dispose();

    [Fact]
    public void DevraitRefuserUnModuleAbsentDuCorpusEtLeClasserNonResolu()
    {
        var resultat = Helpers.ExecuterOutil(
            "VerifMailleAgregat", [_tmp.Chemin, "docs/gestion-projet/plan-de-travail.md"]);
        string stdout = resultat.Stdout;
        Assert.Contains("item ne se résolvant pas la maille  : 1", stdout);
        Assert.Contains("[NON RÉSOLU] TB-091 -> [\"le projet Domaine fantôme qui n'existe pas\"]", stdout);
        Assert.Contains("périmètre entièrement résolu        : 0", stdout);
    }
}

/// <summary>
/// Comportement n°5 — la désignation de rôle sur Infrastructure/Présentation
/// atterrit dans « en attente de J0 », jamais dans « résolu » ni « non
/// résolu ». Les deux formes canoniques « le projet Domaine unique » et
/// « le projet Application unique » se résolvent, elles.
/// </summary>
public class DesignationDeRoleEnAttenteDeJ0 : IDisposable
{
    private readonly RepertoireTemporaire _tmp = new();

    public DesignationDeRoleEnAttenteDeJ0()
    {
        string plan =
            "##### TB-002 — Rôle du projet Domaine unique\n\n" +
            "| Périmètre d'écriture | le projet Domaine unique |\n" +
            "| En conflit avec | — |\n\n" +
            "##### TB-003 — Rôle du projet Application unique\n\n" +
            "| Périmètre d'écriture | le projet Application unique |\n" +
            "| En conflit avec | — |\n\n" +
            "##### TB-004 — Rôle non clôturable avant J0\n\n" +
            "| Périmètre d'écriture | les autres projets d'Infrastructure et de Présentation, " +
            "dont le nombre et les noms relèvent de J0 |\n" +
            "| En conflit avec | — |\n";
        FixturesMailleAgregat.ConstruireRacine(_tmp.Chemin, plan, avecDomaine: false);
    }

    public void Dispose() => _tmp.Dispose();

    [Fact]
    public void DevraitResoudreLesDeuxFormesCanoniquesEtMettreEnAttenteJ0LaMentionDeRoleInfrastructurePresentation()
    {
        var resultat = Helpers.ExecuterOutil(
            "VerifMailleAgregat", [_tmp.Chemin, "docs/gestion-projet/plan-de-travail.md"]);
        string stdout = resultat.Stdout;
        Assert.Contains("périmètre entièrement résolu        : 2", stdout);
        Assert.Contains("périmètre en attente de J0 (rôle)   : 1", stdout);
        Assert.Contains("item ne se résolvant pas la maille  : 0", stdout);
        Assert.Contains("[EN ATTENTE J0] TB-004 ->", stdout);
    }
}

/// <summary>
/// Contrat de code de sortie — anciennement <c>ContratFuturCodeDeSortie</c>
/// côté Python, marqué <c>@expectedFailure</c>. Le portage C# implémente le
/// contrat arbitré : ce test est désormais un test ordinaire, vert.
/// </summary>
public class ContratCodeDeSortieVerifMailleAgregat : IDisposable
{
    private readonly RepertoireTemporaire _tmp = new();

    public ContratCodeDeSortieVerifMailleAgregat()
    {
        string plan =
            "##### TB-091 — Tâche au périmètre fantaisiste\n\n" +
            "| Périmètre d'écriture | le projet Domaine fantôme qui n'existe pas |\n" +
            "| En conflit avec | — |\n";
        FixturesMailleAgregat.ConstruireRacine(_tmp.Chemin, plan, avecDomaine: false);
    }

    public void Dispose() => _tmp.Dispose();

    [Fact]
    public void DevraitTerminerEnCodeDeSortieNonNulQuandUnItemNeSeResoutPas()
    {
        var resultat = Helpers.ExecuterOutil(
            "VerifMailleAgregat", [_tmp.Chemin, "docs/gestion-projet/plan-de-travail.md"]);
        // Le défaut est bien rapporté en sortie standard — et, désormais, le
        // code de sortie le reflète.
        Assert.Contains("item ne se résolvant pas la maille  : 1", resultat.Stdout);
        Assert.NotEqual(0, resultat.CodeSortie);
    }
}

/// <summary>
/// Plan sain, construit indépendamment du dépôt réel : zéro défaut sur les
/// quatre familles arbitrées, code de sortie 0. Complète le sens « détecte »
/// des fixtures de ce fichier par le sens « se tait à bon escient ».
/// </summary>
public class PlanSain : IDisposable
{
    private readonly RepertoireTemporaire _tmp = new();

    public PlanSain()
    {
        string plan =
            "##### TB-901 — Tâche résolue proprement\n\n" +
            "| Périmètre d'écriture | Personnage |\n" +
            "| En conflit avec | — |\n";
        FixturesMailleAgregat.ConstruireRacine(_tmp.Chemin, plan);
    }

    public void Dispose() => _tmp.Dispose();

    [Fact]
    public void DevraitTerminerEnCodeDeSortieZeroQuandAucunDefautNEstRapporte()
    {
        var resultat = Helpers.ExecuterOutil(
            "VerifMailleAgregat", [_tmp.Chemin, "docs/gestion-projet/plan-de-travail.md"]);
        Assert.Contains("item ne se résolvant pas la maille  : 0", resultat.Stdout);
        Assert.Contains("écarts entre `En conflit avec` déclaré et l'intersection recalculée : 0", resultat.Stdout);
        Assert.Contains("identifiants à la fois fiche vivante et retirés (intersection)     : 0", resultat.Stdout);
        Assert.Contains("renvois `Dépend de` / `En conflit avec` vers un identifiant retiré : 0", resultat.Stdout);
        Assert.Equal(0, resultat.CodeSortie);
    }
}

/// <summary>
/// Partie B (§11), sens détection n°1 — un identifiant à la fois fiche
/// vivante (porte une tâche `#####`) et listé dans la section « Identifiants
/// retirés » est compté dans l'intersection. Comportement neuf du portage,
/// sans équivalent Python. Identifiant fictif TB-901, choisi hors de toute
/// plage réelle du plan pour ne rien citer du corpus réel.
/// </summary>
public class IdentifiantAmbigu : IDisposable
{
    private readonly RepertoireTemporaire _tmp = new();

    public IdentifiantAmbigu()
    {
        string plan =
            "##### TB-901 — Tâche vivante et retirée à la fois\n\n" +
            "| Périmètre d'écriture | Personnage |\n" +
            "| En conflit avec | — |\n\n" +
            "## Identifiants retirés\n\n" +
            "| Identifiant | Motif | Repris par |\n" +
            "| --- | --- | --- |\n" +
            "| TB-901 | Fusionné (fixture) | TB-999 |\n";
        FixturesMailleAgregat.ConstruireRacine(_tmp.Chemin, plan);
    }

    public void Dispose() => _tmp.Dispose();

    [Fact]
    public void DevraitCompterUnIdentifiantALaFoisFicheVivanteEtRetireDansLIntersection()
    {
        var resultat = Helpers.ExecuterOutil(
            "VerifMailleAgregat", [_tmp.Chemin, "docs/gestion-projet/plan-de-travail.md"]);
        Assert.Contains(
            "identifiants à la fois fiche vivante et retirés (intersection)     : 1", resultat.Stdout);
        // Aucun renvoi mort dans cette fixture : les deux compteurs de la
        // partie B varient bien indépendamment l'un de l'autre.
        Assert.Contains(
            "renvois `Dépend de` / `En conflit avec` vers un identifiant retiré : 0", resultat.Stdout);
        // Un défaut dans les données, pas une panne d'environnement — code 1.
        Assert.Equal(1, resultat.CodeSortie);
    }
}

/// <summary>
/// Partie B (§11), sens détection n°2 — une fiche vivante qui cite, dans
/// « Dépend de » ou « En conflit avec », un identifiant listé comme retiré,
/// est comptée à part, avec une ligne de détail par occurrence (un par champ
/// touché). Comportement neuf du portage, sans équivalent Python.
/// Identifiants fictifs TB-902/903/904, choisis hors de toute plage réelle.
/// </summary>
public class RenvoiVersIdentifiantRetire : IDisposable
{
    private readonly RepertoireTemporaire _tmp = new();

    public RenvoiVersIdentifiantRetire()
    {
        string plan =
            "##### TB-902 — Tâche citant des identifiants retirés\n\n" +
            "| Périmètre d'écriture | Personnage |\n" +
            "| Dépend de | TB-903 |\n" +
            "| En conflit avec | TB-904 |\n\n" +
            "## Identifiants retirés\n\n" +
            "| Identifiant | Motif | Repris par |\n" +
            "| --- | --- | --- |\n" +
            "| TB-903 | Fusionné dans TB-902 (fixture) | — |\n" +
            "| TB-904 | Abandonné (fixture) | — |\n";
        FixturesMailleAgregat.ConstruireRacine(_tmp.Chemin, plan);
    }

    public void Dispose() => _tmp.Dispose();

    [Fact]
    public void DevraitSignalerUnRenvoiMortParChampToucheVersUnIdentifiantRetire()
    {
        var resultat = Helpers.ExecuterOutil(
            "VerifMailleAgregat", [_tmp.Chemin, "docs/gestion-projet/plan-de-travail.md"]);
        string stdout = resultat.Stdout;
        // Un renvoi mort par champ touché : « Dépend de » et « En conflit
        // avec » comptent chacun pour un, jamais fusionnés en une seule ligne.
        Assert.Contains(
            "renvois `Dépend de` / `En conflit avec` vers un identifiant retiré : 2", stdout);
        Assert.Contains("[RENVOI MORT] TB-902 (Dépend de) -> ['TB-903']", stdout);
        Assert.Contains("[RENVOI MORT] TB-902 (En conflit avec) -> ['TB-904']", stdout);
        // TB-902 lui-même n'est pas retiré : l'intersection reste vide malgré
        // les renvois morts — les deux compteurs varient indépendamment.
        Assert.Contains(
            "identifiants à la fois fiche vivante et retirés (intersection)     : 0", stdout);
        // Un défaut dans les données, pas une panne d'environnement — code 1.
        Assert.Equal(1, resultat.CodeSortie);
    }
}

/// <summary>
/// Volet « panne d'environnement » (code 2) du contrat arbitré — comportement
/// neuf du portage, sans équivalent Python.
/// </summary>
public class PanneEnvironnement : IDisposable
{
    private readonly RepertoireTemporaire _tmp = new();

    public void Dispose() => _tmp.Dispose();

    [Fact]
    public void DevraitTerminerEnCodeDeSortieDeuxQuandLePlanEstIntrouvable()
    {
        // Racine existante mais sans plan-de-travail.md à l'emplacement attendu.
        var resultat = Helpers.ExecuterOutil(
            "VerifMailleAgregat", [_tmp.Chemin, "docs/gestion-projet/plan-de-travail.md"]);
        Assert.Equal(2, resultat.CodeSortie);
        Assert.Contains("panne d'exécution", resultat.Stderr);
    }
}

public class PanneUtf8InvalideDansLePlan : IDisposable
{
    private readonly RepertoireTemporaire _tmp = new();

    public PanneUtf8InvalideDansLePlan()
    {
        // Séquence UTF-8 invalide (0xFF n'est un octet de tête d'aucune
        // séquence UTF-8 valide) — décodage strict, throwOnInvalidBytes.
        Helpers.EcrireOctets(
            Path.Combine(_tmp.Chemin, "docs/gestion-projet/plan-de-travail.md"),
            [0x23, 0x20, 0x50, 0x6C, 0x61, 0x6E, 0x0A, 0x0A, 0xFF, 0x0A]);
    }

    public void Dispose() => _tmp.Dispose();

    [Fact]
    public void DevraitTerminerEnCodeDeSortieDeuxQuandLePlanContientUneSequenceUtf8Invalide()
    {
        var resultat = Helpers.ExecuterOutil(
            "VerifMailleAgregat", [_tmp.Chemin, "docs/gestion-projet/plan-de-travail.md"]);
        Assert.Equal(2, resultat.CodeSortie);
        Assert.Contains("panne d'exécution", resultat.Stderr);
    }
}
