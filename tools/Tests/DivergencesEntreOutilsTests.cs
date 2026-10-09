// Tests d'ACCORD entre `tools/VerifMailleAgregat` (le vérificateur) et
// `tools/CreerIssues` (le générateur) — distincts des golden masters et des
// caractérisations comportementales du reste de ce projet. Ils portent sur
// quatre points (A à D) où les deux outils lisaient autrefois le plan de
// travail avec des règles différentes, sur des entrées que le plan réel ne
// déclenche pas (sauf D, sur TB-003). Un différentiel de sortie sur le plan
// réel est aveugle à ces écarts : ces tests les exercent sur des entrées
// CONSTRUITES, portant chacune le déclencheur, et affirment que les deux
// outils concluent pareil.
//
// Arbitrage : le plan de travail est la source de vérité des tâches
// (ADR-019 § 7), donc chaque outil applique ce que le plan dit, n'invente
// aucune règle qu'il ne porte pas, et signale une forme hors règle au lieu de
// la sauter en silence.
//   A. Une tâche se titre `##### TB-<chiffres> — <intitulé>` (cinq dièses,
//      tiret cadratin) ; un titre de niveau 4 à 6 qui commence par `TB-` sans
//      cette forme est un défaut (code de sortie 1 des deux outils, ligne
//      nommée).
//   B. L'identifiant est lu en entier, sans plafond de chiffres ; un
//      identifiant défini deux fois est un défaut, jamais un écrasement.
//   C. La maille d'outillage se lit dans le § 2 du plan, sans liste en dur.
//   D. Un namespace de bounded context ne se résout que sur la seule tranche
//      qui les crée, dans les deux outils.
//   E. Les modules transverses, les modules client et les namespaces se lisent
//      dans le § 2, dans les deux outils : un module ajouté au § 2 est admis par
//      les deux.
//   F. La tranche qui crée les namespaces se lit dans le § 2, dans les deux
//      outils : changer son identifiant dans le § 2 déplace le verrou.
//
// Historique : ces tests affirmaient auparavant le DÉSACCORD lui-même
// (verts tant que les deux outils différaient, rouges au jour de
// l'arbitrage). Chacun affirme maintenant l'accord, et échoue sur l'ancien
// code des deux outils.
using System.Text.RegularExpressions;
using Xunit;

namespace CaracterisationOutils;

/// <summary>Fixtures partagées par les quatre accords de ce fichier.</summary>
internal static class FixturesDivergences
{
    public const string DomaineMd =
        "# Personnage (domaine)\n\n## Agrégats\n\n### Personnage\n\nRacine d'agrégat, décrit un personnage joueur.\n";

    // Reprend la forme du § 2 du plan réel : les quatre namespaces et la clause
    // « sur la seule tranche qui les crée » sont portés par la ligne du module
    // transverse nommé, et la tranche qui les crée est nommée par le paragraphe
    // « Aucun namespace de bounded context… ». Identique, pour le reste, à
    // FixturesCreerIssues.PlanPreambule (CreerIssuesTests.cs) — dupliqué plutôt
    // que partagé : ce fichier compare deux outils, une dépendance croisée vers
    // les fixtures de l'un d'eux irait à l'encontre de cette neutralité.
    public const string LigneOutillageParDefaut =
        "- un **module d'outillage** : `la configuration de la solution`, `la configuration d'intégration continue`\n";

    public const string ModulesClientParDefaut =
        "| `service d'accès au store` | Accès IndexedDB |\n"
        + "| `châssis` | Disposition générale |\n"
        + "| `bandeaux transversaux` | Bandeaux d'état |\n"
        + "| `sanitisation` | Nettoyage HTML |\n"
        + "| `politique de sécurité de contenu` | CSP |\n"
        + "| `projection d'export` | Rendu d'export |\n";

    public const string TransversesParDefaut = "`SharedKernel`, `Haversack.Infrastructure.Notifications`";

    public const string NamespacesParDefaut = "`IdentityAccess`, `SpaceManagement`, `ContentLibrary`, `SessionConduct`";

    /// <summary>
    /// Le § 2 de la fixture. Chaque argument est un morceau du § 2 que les deux outils lisent :
    /// la ligne des modules d'outillage, les lignes de la table de la couche cliente, les noms
    /// de la ligne du module transverse nommé, les namespaces de contexte de cette même ligne,
    /// et la tranche que le paragraphe « Aucun namespace… » désigne comme seule à les créer.
    /// </summary>
    public static string PlanPreambule(
        string ligneOutillage = LigneOutillageParDefaut,
        string modulesClient = ModulesClientParDefaut,
        string transverses = TransversesParDefaut,
        string namespaces = NamespacesParDefaut,
        string trancheCreatrice = "TB-003") =>
        "# Plan de travail (fixture de test)\n\n" +
        "## §2 Maille de désignation\n\n" +
        "**Les six modules de la couche cliente et leur ancrage.**\n\n" +
        modulesClient + "\n" +
        "**L'agrégat couvre ses deux moitiés.**\n\n" +
        "Le domaine et son module d'accès forment la même unité d'écriture.\n\n" +
        ligneOutillage +
        $"- un **module transverse nommé** : {transverses}, ou — "
        + $"**sur la seule tranche qui les crée** — l'un des quatre namespaces de bounded context {namespaces}\n\n" +
        $"**Aucun namespace de bounded context n'apparaît comme module**, sauf sur la seule tranche qui les crée ({trancheCreatrice}) : "
        + "un namespace est un tiroir, pas une unité d'écriture.\n\n" +
        "### Tâches\n\n";

    /// <summary>Une fiche de tâche ; <paramref name="titre"/> est la ligne de titre entière.</summary>
    public static string Fiche(string titre, string perimetre) =>
        titre + "\n\n" +
        "| Champ | Valeur |\n|---|---|\n" +
        "| Jalon | J1 — traversée verticale |\n" +
        $"| Périmètre d'écriture | {perimetre} |\n" +
        "| Dépend de | — |\n" +
        "| En conflit avec | — |\n\n";

    public static string Tache(string id, string perimetre) =>
        Fiche($"##### {id} — Tâche de fixture {id}", perimetre);

    public static void ConstruireDepot(string depot, string planMd)
    {
        Helpers.InitialiserDepotGit(depot);
        Helpers.Ecrire(Path.Combine(depot, "docs/projet/methode-de-ticket.md"), FixturesCreerIssues.MethodeMd);
        Helpers.Ecrire(Path.Combine(depot, "docs/projet/plan-de-travail.md"), planMd);
        Helpers.Ecrire(Path.Combine(depot, "docs/domaine/personnage/context.md"), DomaineMd);
        Helpers.CommiterTout(depot, "fixture d'accord");
        // Révision simulée déjà poussée uniquement pour que l'AVERTISSEMENT
        // sur la révision, hors sujet ici, n'ajoute pas de bruit à la
        // démonstration (cf. RegimesDeLaRevisionEnModeABlanc, CreerIssuesTests.cs).
        Helpers.ConfigurerOrigineFictive(depot, FixturesCreerIssues.UrlOrigineFictive, pousser: true);
    }

    public static ResultatExecution ExecuterGenerateur(string depot, string racineBin)
    {
        var environnement = new Dictionary<string, string> { ["PATH"] = Helpers.RepertoireBinSansGh(racineBin) };
        return Helpers.ExecuterOutilAvecEnvironnement("CreerIssues", [], depot, environnement);
    }

    public static ResultatExecution ExecuterVerificateur(string depot) =>
        Helpers.ExecuterOutil("VerifMailleAgregat", [depot, "docs/projet/plan-de-travail.md"]);

    /// <summary>Numéro (à partir de 1) de la première ligne du plan qui commence par <paramref name="debut"/>.</summary>
    public static int NumeroDeLigne(string planMd, string debut) =>
        Array.FindIndex(planMd.Split('\n'), l => l.StartsWith(debut, StringComparison.Ordinal)) + 1;

    /// <summary>Identifiants que le vérificateur déclare non résolus.</summary>
    public static List<string> NonResolusDuVerificateur(string stdout) =>
        Regex.Matches(stdout, @"\[NON RÉSOLU\] (TB-\d+)").Select(m => m.Groups[1].Value).ToList();

    /// <summary>Identifiants que le générateur classe sans libellé de nature déductible.</summary>
    public static List<string> SansNatureDuGenerateur(string stdout) =>
        Regex.Matches(stdout, @"^  - (TB-\d+) : ", RegexOptions.Multiline).Select(m => m.Groups[1].Value).ToList();
}

/// <summary>
/// Accord A — le titre de tâche. Les deux outils reconnaissent une tâche par
/// <c>##### TB-&lt;chiffres&gt; — &lt;intitulé&gt;</c> et signalent, en nommant la
/// ligne, un titre de niveau 4 à 6 qui commence par <c>TB-</c> sans cette forme.
/// Avant l'arbitrage, le vérificateur voyait une tâche titrée à quatre dièses
/// et tiret court que le générateur sautait sans rien dire : elle ne recevait
/// jamais d'issue.
/// </summary>
public class AccordA_FormatDuTitreDeTache : IDisposable
{
    private readonly RepertoireTemporaire _tmpDepot = new();
    private readonly RepertoireTemporaire _tmpBin = new();
    private readonly int _ligneDuTitreMalForme;

    public AccordA_FormatDuTitreDeTache()
    {
        string plan = FixturesDivergences.PlanPreambule()
            + FixturesDivergences.Tache("TB-009", "Personnage")
            + FixturesDivergences.Fiche("#### TB-010 - Intitulé à quatre dièses et tiret court", "châssis");
        _ligneDuTitreMalForme = FixturesDivergences.NumeroDeLigne(plan, "#### TB-010 - ");
        FixturesDivergences.ConstruireDepot(_tmpDepot.Chemin, plan);
    }

    public void Dispose()
    {
        _tmpDepot.Dispose();
        _tmpBin.Dispose();
    }

    [Fact]
    public void LeVerificateurCompteLeTitreMalFormeCommeUnDefautEtNommeLaLigne()
    {
        var resultat = FixturesDivergences.ExecuterVerificateur(_tmpDepot.Chemin);
        Assert.Equal(1, resultat.CodeSortie);
        Assert.Contains($"ligne {_ligneDuTitreMalForme}", resultat.Stdout);
        Assert.Contains("#### TB-010 - Intitulé", resultat.Stdout);
    }

    [Fact]
    public void LeGenerateurSArreteSurLeTitreMalFormeEtNommeLaLigne()
    {
        var resultat = FixturesDivergences.ExecuterGenerateur(_tmpDepot.Chemin, _tmpBin.Chemin);
        Assert.Equal(1, resultat.CodeSortie);
        Assert.Contains($"ligne {_ligneDuTitreMalForme}", resultat.Stderr);
        Assert.Contains("#### TB-010 - Intitulé", resultat.Stderr);
    }
}

/// <summary>
/// Accord B — l'identifiant. Les deux outils lisent l'identifiant entier :
/// <c>TB-1000</c> n'est pas <c>TB-100</c>. Avant l'arbitrage, le vérificateur
/// tronquait <c>TB-1000</c> à trois chiffres et la tâche écrasait <c>TB-100</c>
/// dans son dictionnaire, sans rien signaler. Un identifiant défini deux fois
/// est un défaut des deux outils, jamais un écrasement.
/// </summary>
public class AccordB_IdentifiantEntier : IDisposable
{
    private readonly RepertoireTemporaire _tmpDepot = new();
    private readonly RepertoireTemporaire _tmpBin = new();

    public void Dispose()
    {
        _tmpDepot.Dispose();
        _tmpBin.Dispose();
    }

    [Fact]
    public void LesDeuxOutilsVoientDeuxTachesDistinctesPourTb100EtTb1000()
    {
        FixturesDivergences.ConstruireDepot(_tmpDepot.Chemin,
            FixturesDivergences.PlanPreambule()
            + FixturesDivergences.Tache("TB-100", "Personnage")
            + FixturesDivergences.Tache("TB-1000", "châssis"));

        var resultatVerificateur = FixturesDivergences.ExecuterVerificateur(_tmpDepot.Chemin);
        Assert.Contains("tâches du plan                        : 2", resultatVerificateur.Stdout);
        Assert.Equal(0, resultatVerificateur.CodeSortie);

        var resultatGenerateur = FixturesDivergences.ExecuterGenerateur(_tmpDepot.Chemin, _tmpBin.Chemin);
        Assert.Contains("Tâches lues dans le plan : 2", resultatGenerateur.Stdout);
    }

    [Fact]
    public void LesDeuxOutilsSignalentUnIdentifiantDefiniDeuxFois()
    {
        string plan = FixturesDivergences.PlanPreambule()
            + FixturesDivergences.Tache("TB-100", "Personnage")
            + FixturesDivergences.Tache("TB-100", "châssis");
        FixturesDivergences.ConstruireDepot(_tmpDepot.Chemin, plan);

        var resultatVerificateur = FixturesDivergences.ExecuterVerificateur(_tmpDepot.Chemin);
        Assert.Equal(1, resultatVerificateur.CodeSortie);
        Assert.Contains("TB-100", resultatVerificateur.Stdout.Split('\n')
            .Single(l => l.Contains("[IDENTIFIANT EN DOUBLE]", StringComparison.Ordinal)));

        var resultatGenerateur = FixturesDivergences.ExecuterGenerateur(_tmpDepot.Chemin, _tmpBin.Chemin);
        Assert.Equal(1, resultatGenerateur.CodeSortie);
        Assert.Contains("TB-100", resultatGenerateur.Stderr);
        Assert.Contains("deux fois", resultatGenerateur.Stderr);
    }
}

/// <summary>
/// Accord C — la maille d'outillage. Les deux outils lisent les modules
/// d'outillage dans le § 2 du plan : un cinquième libellé ajouté au § 2 est
/// accepté par les deux. Avant l'arbitrage, le vérificateur codait quatre
/// libellés en dur et refusait le nouveau, une fausse alerte sur un module
/// pourtant légitime. Chaque outil garde son propre lecteur du § 2 (le
/// générateur tire trop de choses de son dossier pour qu'un fichier commun
/// soit inclus par les deux) : le second test vérifie qu'ils en tirent la même
/// liste, y compris sur un libellé de renvoi markdown, que ni l'un ni l'autre
/// ne compte comme module.
/// </summary>
public class AccordC_MailleLueDansLePlan : IDisposable
{
    private readonly RepertoireTemporaire _tmpDepot = new();
    private readonly RepertoireTemporaire _tmpBin = new();

    public void Dispose()
    {
        _tmpDepot.Dispose();
        _tmpBin.Dispose();
    }

    [Fact]
    public void LesDeuxOutilsAcceptentUnCinquiemeModuleDOutillageAjouteAuPlan()
    {
        string plan = FixturesDivergences.PlanPreambule(
                FixturesDivergences.LigneOutillageParDefaut.TrimEnd('\n') + ", `le pipeline de publication npm`\n")
            + FixturesDivergences.Tache("TB-060", "le pipeline de publication npm");
        FixturesDivergences.ConstruireDepot(_tmpDepot.Chemin, plan);

        var resultatVerificateur = FixturesDivergences.ExecuterVerificateur(_tmpDepot.Chemin);
        Assert.Contains("item ne se résolvant pas la maille  : 0", resultatVerificateur.Stdout);
        Assert.Contains("périmètre entièrement résolu        : 1", resultatVerificateur.Stdout);
        Assert.Equal(0, resultatVerificateur.CodeSortie);

        var resultatGenerateur = FixturesDivergences.ExecuterGenerateur(_tmpDepot.Chemin, _tmpBin.Chemin);
        Assert.Contains("Tâches sans libellé de nature déductible (0)", resultatGenerateur.Stdout);
    }

    [Fact]
    public void LesDeuxOutilsTirentLaMemeListeDeLaLigneOutillageDuPlan()
    {
        string ligne = "- un **module d'outillage** : [`libellé de renvoi`](autre.md), `premier module`, `second module`\n";
        string plan = FixturesDivergences.PlanPreambule(ligne)
            + FixturesDivergences.Tache("TB-070", "premier module")
            + FixturesDivergences.Tache("TB-071", "second module")
            + FixturesDivergences.Tache("TB-072", "libellé de renvoi")
            + FixturesDivergences.Tache("TB-073", "module absent du plan");
        FixturesDivergences.ConstruireDepot(_tmpDepot.Chemin, plan);

        var refusesParLeVerificateur = FixturesDivergences.NonResolusDuVerificateur(
            FixturesDivergences.ExecuterVerificateur(_tmpDepot.Chemin).Stdout);
        var refusesParLeGenerateur = FixturesDivergences.SansNatureDuGenerateur(
            FixturesDivergences.ExecuterGenerateur(_tmpDepot.Chemin, _tmpBin.Chemin).Stdout);

        Assert.Equal(["TB-072", "TB-073"], refusesParLeVerificateur);
        Assert.Equal(refusesParLeVerificateur, refusesParLeGenerateur);
    }
}

/// <summary>
/// Accord D — le verrou du namespace de bounded context. Le § 2 du plan
/// n'admet un namespace comme module que « sur la seule tranche qui les
/// crée » (TB-003). Les deux outils appliquent cette clause : TB-003 résout
/// son namespace, une autre tranche qui le nomme ne résout pas. Avant
/// l'arbitrage, le vérificateur appliquait la clause et le générateur résolvait
/// le namespace sur n'importe quelle tranche, parce que la ligne du module
/// transverse nommé, qu'il lit en bloc, énumère aussi les quatre namespaces.
/// </summary>
public class AccordD_VerrouDuNamespaceSurLaTrancheDeCreation : IDisposable
{
    private readonly RepertoireTemporaire _tmpDepot = new();
    private readonly RepertoireTemporaire _tmpBin = new();

    public AccordD_VerrouDuNamespaceSurLaTrancheDeCreation()
    {
        string plan = FixturesDivergences.PlanPreambule()
            + FixturesDivergences.Fiche("##### TB-003 — Création du namespace IdentityAccess", "IdentityAccess")
            + FixturesDivergences.Tache("TB-050", "SpaceManagement");
        FixturesDivergences.ConstruireDepot(_tmpDepot.Chemin, plan);
    }

    public void Dispose()
    {
        _tmpDepot.Dispose();
        _tmpBin.Dispose();
    }

    [Fact]
    public void LesDeuxOutilsResolventLeNamespaceSurTb003EtLeRefusentSurUneAutreTranche()
    {
        var resultatVerificateur = FixturesDivergences.ExecuterVerificateur(_tmpDepot.Chemin);
        Assert.Contains("périmètre entièrement résolu        : 1", resultatVerificateur.Stdout);
        Assert.Equal(["TB-050"], FixturesDivergences.NonResolusDuVerificateur(resultatVerificateur.Stdout));

        var resultatGenerateur = FixturesDivergences.ExecuterGenerateur(_tmpDepot.Chemin, _tmpBin.Chemin);
        Assert.Equal(["TB-050"], FixturesDivergences.SansNatureDuGenerateur(resultatGenerateur.Stdout));
    }

    [Fact]
    public void LeGenerateurPoseUnLibelleDeNatureSurTb003()
    {
        var resultatGenerateur = FixturesDivergences.ExecuterGenerateur(_tmpDepot.Chemin, _tmpBin.Chemin);
        Assert.Matches(@"\+ TB-003 — [^\n]*libellés : \[[^\]\n]*'Socle'", resultatGenerateur.Stdout);
    }
}

/// <summary>
/// Accord E — les listes du § 2. Les modules transverses nommés, les modules de la
/// couche cliente et les namespaces de contexte se lisent dans le § 2 du plan, dans
/// les deux outils. Avant B8, le vérificateur gardait ces trois listes en dur : un
/// module ajouté au § 2 était refusé par lui et admis par le générateur. Chaque
/// outil garde son lecteur (un <c>#:include</c> entre les deux ne marche pas) : le
/// second test vérifie qu'ils en tirent les mêmes listes, y compris sur un libellé
/// de renvoi markdown dans la ligne du module transverse nommé.
/// </summary>
public class AccordE_ListesDuParagraphe2 : IDisposable
{
    private readonly RepertoireTemporaire _tmpDepot = new();
    private readonly RepertoireTemporaire _tmpBin = new();

    public void Dispose()
    {
        _tmpDepot.Dispose();
        _tmpBin.Dispose();
    }

    [Fact]
    public void LesDeuxOutilsAdmettentUnModuleTransverseUnModuleClientEtUnNamespaceAjoutesAuParagraphe2()
    {
        string plan = FixturesDivergences.PlanPreambule(
                modulesClient: FixturesDivergences.ModulesClientParDefaut + "| `journal de bord` | Trace locale |\n",
                transverses: FixturesDivergences.TransversesParDefaut + ", `le noyau de journalisation`",
                namespaces: FixturesDivergences.NamespacesParDefaut + ", `Reporting`")
            + FixturesDivergences.Tache("TB-080", "le noyau de journalisation")
            + FixturesDivergences.Tache("TB-081", "journal de bord")
            + FixturesDivergences.Fiche("##### TB-003 — Création du namespace Reporting", "Reporting")
            + FixturesDivergences.Tache("TB-082", "module absent du plan");
        FixturesDivergences.ConstruireDepot(_tmpDepot.Chemin, plan);

        var refusesParLeVerificateur = FixturesDivergences.NonResolusDuVerificateur(
            FixturesDivergences.ExecuterVerificateur(_tmpDepot.Chemin).Stdout);
        var refusesParLeGenerateur = FixturesDivergences.SansNatureDuGenerateur(
            FixturesDivergences.ExecuterGenerateur(_tmpDepot.Chemin, _tmpBin.Chemin).Stdout);

        Assert.Equal(["TB-082"], refusesParLeVerificateur);
        Assert.Equal(refusesParLeVerificateur, refusesParLeGenerateur);
    }

    [Fact]
    public void LesDeuxOutilsTirentLesMemesListesDuParagraphe2SansCompterUnLibelleDeRenvoi()
    {
        string plan = FixturesDivergences.PlanPreambule(
                modulesClient: "| `premier module client` | A |\n| `second module client` | B |\n",
                transverses: "[`libellé de renvoi`](autre.md), `premier transverse`, `second transverse`",
                namespaces: "`PremierNamespace`")
            + FixturesDivergences.Tache("TB-090", "premier module client")
            + FixturesDivergences.Tache("TB-091", "second module client")
            + FixturesDivergences.Tache("TB-092", "premier transverse")
            + FixturesDivergences.Tache("TB-093", "second transverse")
            + FixturesDivergences.Tache("TB-094", "libellé de renvoi")
            + FixturesDivergences.Tache("TB-095", "SharedKernel")
            + FixturesDivergences.Tache("TB-096", "châssis")
            + FixturesDivergences.Tache("TB-097", "IdentityAccess");
        FixturesDivergences.ConstruireDepot(_tmpDepot.Chemin, plan);

        var refusesParLeVerificateur = FixturesDivergences.NonResolusDuVerificateur(
            FixturesDivergences.ExecuterVerificateur(_tmpDepot.Chemin).Stdout);
        var refusesParLeGenerateur = FixturesDivergences.SansNatureDuGenerateur(
            FixturesDivergences.ExecuterGenerateur(_tmpDepot.Chemin, _tmpBin.Chemin).Stdout);

        Assert.Equal(["TB-094", "TB-095", "TB-096", "TB-097"], refusesParLeVerificateur);
        Assert.Equal(refusesParLeVerificateur, refusesParLeGenerateur);
    }
}

/// <summary>
/// Accord F — la tranche qui crée les namespaces. Quand le § 2 la désigne autrement
/// que par l'identifiant d'hier, les deux outils résolvent les namespaces sur cette
/// tranche et les refusent sur l'ancienne. Avant B8, le vérificateur gardait cet
/// identifiant en dur et le générateur le lisait dans le plan.
/// </summary>
public class AccordF_TrancheQuiCreeLesNamespaces : IDisposable
{
    private readonly RepertoireTemporaire _tmpDepot = new();
    private readonly RepertoireTemporaire _tmpBin = new();

    public AccordF_TrancheQuiCreeLesNamespaces()
    {
        string plan = FixturesDivergences.PlanPreambule(trancheCreatrice: "TB-020")
            + FixturesDivergences.Fiche("##### TB-003 — Tranche qui n'est plus celle des namespaces", "IdentityAccess")
            + FixturesDivergences.Fiche("##### TB-020 — Création des namespaces", "SpaceManagement");
        FixturesDivergences.ConstruireDepot(_tmpDepot.Chemin, plan);
    }

    public void Dispose()
    {
        _tmpDepot.Dispose();
        _tmpBin.Dispose();
    }

    [Fact]
    public void LesDeuxOutilsResolventLesNamespacesSurLaTrancheDuParagraphe2EtLesRefusentSurTb003()
    {
        var resultatVerificateur = FixturesDivergences.ExecuterVerificateur(_tmpDepot.Chemin);
        Assert.Contains("périmètre entièrement résolu        : 1", resultatVerificateur.Stdout);
        Assert.Equal(["TB-003"], FixturesDivergences.NonResolusDuVerificateur(resultatVerificateur.Stdout));

        var resultatGenerateur = FixturesDivergences.ExecuterGenerateur(_tmpDepot.Chemin, _tmpBin.Chemin);
        Assert.Equal(["TB-003"], FixturesDivergences.SansNatureDuGenerateur(resultatGenerateur.Stdout));
    }
}
