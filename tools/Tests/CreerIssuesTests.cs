// Portage C# du script Python d'origine, `tools/tests/test_creer_issues.py` —
// caractérise le comportement observable de `tools/CreerIssues` sur son binaire compilé, en
// sous-processus, comme les autres fichiers de ce projet.
//
// Différence de méthode assumée avec l'original : le script Python appelait
// `construire_corps_issue` directement (import du module, boîte blanche).
// Ce projet ne teste que des binaires compilés en sous-processus (même
// stratégie que VerifCorpusTests.cs / VerifMailleAgregatTests.cs) — la
// construction du corps est donc exercée ici en bout en bout, via le
// drapeau `--seulement TB-nnn`, qui imprime sur la sortie standard le corps
// exact que `CorpsIssue.Construire` aurait produit (`Orchestrateur.cs`,
// bloc « ── Corps de l'issue … ── »). Aucun de ces tests n'écrit dans le
// dépôt réel ni ne contacte GitHub : `gh` est soit absent du PATH du
// sous-processus, soit remplacé par le faux `gh` de `tools/Harnais/FauxGh`
// (jamais le vrai, quelle que soit l'option exercée — y compris `--appliquer`).
using System.Text.Json;
using System.Text.RegularExpressions;
using Xunit;

namespace CaracterisationOutils;

/// <summary>
/// Fixtures et utilitaires partagés par les classes de ce fichier — même
/// esprit que <c>FixturesMailleAgregat</c> dans <c>VerifMailleAgregatTests.cs</c>.
/// </summary>
internal static class FixturesCreerIssues
{
    public const string MethodeMd =
        "# Méthode de ticket (fixture de test)\n\n" +
        "## §3 Taxonomie de libellés\n\n" +
        "**Par jalon**\n\n" +
        "| Valeur | Note |\n|---|---|\n" +
        "| J0 | Fondations |\n" +
        "| J1 | Traversée verticale |\n\n" +
        "**Par nature de tranche**\n\n" +
        "| Valeur | Note |\n|---|---|\n" +
        "| Comportement | Agrégat de domaine |\n" +
        "| Couche cliente | Module client |\n" +
        "| Outillage | Projet d'outillage |\n" +
        "| Socle | Module transverse |\n" +
        "| Surface | Fiche d'écran |\n" +
        "| Hors maille | Aucun code produit |\n\n" +
        "**Par nature de vérification**\n\n" +
        "| Valeur | Note |\n|---|---|\n" +
        "| Fonctionnel | Comportement observable |\n\n" +
        "**Interdiction nommée** : aucun axe de priorité n'est posé sur les issues (fixture de test).\n";

    // §2 minimal mais complet : les quatre familles que MailleDuPlan.Extraire
    // exige non vides (agrégats, couche cliente, outillage, transverses),
    // faute de quoi l'outil lève avant même d'atteindre la moindre tâche.
    public const string PlanPreambule =
        "# Plan de travail (fixture de test)\n\n" +
        "## §2 Maille de désignation\n\n" +
        "**Les six modules de la couche cliente et leur ancrage.**\n\n" +
        "| `service d'accès au store` | Accès IndexedDB |\n" +
        "| `châssis` | Disposition générale |\n" +
        "| `bandeaux transversaux` | Bandeaux d'état |\n" +
        "| `sanitisation` | Nettoyage HTML |\n" +
        "| `politique de sécurité de contenu` | CSP |\n" +
        "| `projection d'export` | Rendu d'export |\n\n" +
        "**L'agrégat couvre ses deux moitiés.**\n\n" +
        "Le domaine et son module d'accès forment la même unité d'écriture.\n\n" +
        "- un **module d'outillage** : `la configuration de la solution`, `la configuration d'intégration continue`\n" +
        "- un **module transverse nommé** : `SharedKernel`, `Haversack.Infrastructure.Notifications`\n\n" +
        "### Tâches\n\n";

    public const string DomaineMd =
        "# Personnage (domaine)\n\n## Agrégats\n\n### Personnage\n\nRacine d'agrégat, décrit un personnage joueur.\n";

    public const string UrlOrigineFictive = "git@github.com:exemple-org/exemple-depot.git";

    // Copie littérale de CreerIssues.CorpsIssue.{MarqueurSeparation,ZoneManuelleParDefaut}
    // (internal, hors d'atteinte de ce projet — stratégie boîte noire assumée
    // pour tout ce fichier). Un changement de ces littéraux dans la source
    // rougit ce test, ce qui EST le comportement souhaité pour une
    // caractérisation.
    public const string MarqueurSeparationCorps =
        "── engendré — régénérable, écrasé à chaque exécution — "
        + "sous cette ligne : écrit à la main ──";
    public const string ZoneManuelleParDefaut = "\n\n## Pris par\n\n## Notes d'exécution\n\n## Écart constaté\n";

    public static void EcrireDepotMinimal(string depot, string planMd)
    {
        Helpers.InitialiserDepotGit(depot);
        Helpers.Ecrire(Path.Combine(depot, "docs/gestion-projet/methode-de-ticket.md"), MethodeMd);
        Helpers.Ecrire(Path.Combine(depot, "docs/gestion-projet/plan-de-travail.md"), planMd);
        Helpers.Ecrire(Path.Combine(depot, "docs/conception/domain/personnage.md"), DomaineMd);
    }

    /// <summary>
    /// Réponse `gh` à injecter via <c>FAUX_GH_SORTIE</c> pour représenter une
    /// unique issue déjà existante. Le faux gh renvoie ce même contenu quel
    /// que soit le point d'API interrogé (labels, milestones, issues) : les
    /// quatre champs portés ici couvrent simultanément les trois formes
    /// attendues (labels lit <c>name</c> ; milestones lit <c>title</c>/<c>number</c> ;
    /// issues lit <c>title</c>/<c>number</c>/<c>body</c>) sans que cela ne perturbe
    /// aucune des trois lectures : un libellé et un jalon fictifs
    /// supplémentaires apparaissent, jamais consultés par les tests d'ici.
    /// </summary>
    public static string PayloadIssueExistante(string titre, int numero, string? corps) =>
        JsonSerializer.Serialize(new object?[]
        {
            new { name = "libelle-fixture-sans-effet", title = titre, number = numero, body = corps },
        });

    private static readonly Regex MotifLienDeTache = new(@"Lien de tâche utilisé      : (\S+) \(", RegexOptions.CultureInvariant);
    private static readonly Regex MotifRevisionPlan = new(@"Révision du plan \(estampille de provenance\) : (\S+)", RegexOptions.CultureInvariant);

    /// <summary>
    /// Relit dans la sortie standard le lien de tâche et la révision que
    /// l'exécution a elle-même calculés depuis le dépôt git réel de la
    /// fixture — jamais figés en dur ici, à la différence des constantes du
    /// script Python d'origine (URL/révision fabriquées à la main pour un
    /// appel direct de fonction pure) : ce portage est en bout en bout, la
    /// révision est un vrai hash git.
    /// </summary>
    public static (string UrlBlobPlan, string RevisionPlan) ExtraireContexteDeSortie(string stdout)
    {
        var mUrl = MotifLienDeTache.Match(stdout);
        var mRev = MotifRevisionPlan.Match(stdout);
        if (!mUrl.Success || !mRev.Success)
        {
            throw new InvalidOperationException($"contexte (lien de tâche / révision) introuvable dans la sortie :\n{stdout}");
        }
        return (mUrl.Groups[1].Value, mRev.Groups[1].Value);
    }

    /// <summary>
    /// Isole le corps d'une issue depuis le bloc de prévisualisation qu'imprime
    /// <c>--seulement</c> (<c>Orchestrateur.cs</c>) — le corps exact que
    /// <c>CorpsIssue.Construire</c> a produit, sans le texte d'état qui
    /// l'entoure.
    /// </summary>
    public static string ExtraireCorpsIssue(string stdout, string tbId)
    {
        string ouvre = $"── Corps de l'issue {tbId} (";
        int iOuvre = stdout.IndexOf(ouvre, StringComparison.Ordinal);
        if (iOuvre == -1)
        {
            throw new InvalidOperationException($"bloc de corps de {tbId} introuvable dans la sortie :\n{stdout}");
        }
        const string finEtat = ") ──\n";
        int iDebutCorps = stdout.IndexOf(finEtat, iOuvre, StringComparison.Ordinal);
        if (iDebutCorps == -1)
        {
            throw new InvalidOperationException($"fin d'état du bloc de corps de {tbId} introuvable dans la sortie :\n{stdout}");
        }
        iDebutCorps += finEtat.Length;
        string ferme = $"\n── fin du corps {tbId} ──";
        int iFinCorps = stdout.IndexOf(ferme, iDebutCorps, StringComparison.Ordinal);
        if (iFinCorps == -1)
        {
            throw new InvalidOperationException($"marqueur de fin du bloc de corps de {tbId} introuvable dans la sortie :\n{stdout}");
        }
        return stdout[iDebutCorps..iFinCorps];
    }

    public static string TacheTb014(string dependDe, string enConflitAvec) =>
        "##### TB-014 — Création d'un personnage joueur\n\n" +
        "| Champ | Valeur |\n|---|---|\n" +
        "| Jalon | J1 — traversée verticale |\n" +
        "| Périmètre d'écriture | Personnage |\n" +
        $"| Dépend de | {dependDe} |\n" +
        $"| En conflit avec | {enConflitAvec} |\n";
}

/// <summary>
/// Comportement n°6 — la zone engendrée porte le renvoi sur la révision
/// immuable (jamais la branche), les renvois `Dépend de` / `En conflit avec`,
/// l'estampille de provenance, le marqueur de séparation, puis la zone
/// manuelle par défaut quand aucun corps n'existait déjà. Couvre aussi le cas
/// « tranche référencée sans issue connue » (`TB-099 (pas encore d'issue)`),
/// gh absent : la lecture de l'existant GitHub est alors impossible par
/// construction, jamais seulement par absence de correspondance.
/// </summary>
public class ZoneEngendreeSansIssueExistantePourLaDependance : IDisposable
{
    private readonly RepertoireTemporaire _tmpDepot = new();
    private readonly RepertoireTemporaire _tmpBin = new();

    public ZoneEngendreeSansIssueExistantePourLaDependance()
    {
        FixturesCreerIssues.EcrireDepotMinimal(
            _tmpDepot.Chemin,
            FixturesCreerIssues.PlanPreambule + FixturesCreerIssues.TacheTb014("TB-099", "—"));
        Helpers.CommiterTout(_tmpDepot.Chemin, "fixture corps engendré");
        Helpers.ConfigurerOrigineFictive(_tmpDepot.Chemin, FixturesCreerIssues.UrlOrigineFictive, pousser: true);
    }

    public void Dispose()
    {
        _tmpDepot.Dispose();
        _tmpBin.Dispose();
    }

    [Fact]
    public void DevraitPorterLesRenvoisLEstampilleEtLeMarqueurEtPreserverLaZoneManuelleParDefaut()
    {
        var environnement = new Dictionary<string, string> { ["PATH"] = Helpers.RepertoireBinSansGh(_tmpBin.Chemin) };
        var resultat = Helpers.ExecuterOutilAvecEnvironnement(
            "CreerIssues", ["--seulement", "TB-014"], _tmpDepot.Chemin, environnement);

        Assert.Equal(0, resultat.CodeSortie);
        var (urlBlobPlan, revisionPlan) = FixturesCreerIssues.ExtraireContexteDeSortie(resultat.Stdout);
        string corps = FixturesCreerIssues.ExtraireCorpsIssue(resultat.Stdout, "TB-014");

        Assert.Contains(
            $"**Tâche de plan** : [`docs/gestion-projet/plan-de-travail.md`]({urlBlobPlan}) — TB-014", corps);
        Assert.Contains("**Dépend de** : TB-099 (pas encore d'issue)", corps);
        Assert.Contains("**En conflit avec** : —", corps);
        Assert.Contains(
            $"**Estampille de provenance** : docs/gestion-projet/plan-de-travail.md@{revisionPlan} — TB-014", corps);
        Assert.Contains(FixturesCreerIssues.MarqueurSeparationCorps, corps);
        Assert.EndsWith(FixturesCreerIssues.ZoneManuelleParDefaut, corps);
    }
}

/// <summary>
/// Comportement n°9 (moitié « numéro connu ») — une tranche référencée par
/// `Dépend de` qui a déjà une issue GitHub se projette en renvoi `#&lt;numéro&gt;`,
/// jamais un lien mort ni un texte recopié.
/// </summary>
public class RenvoiVersUneIssueExistante : IDisposable
{
    private readonly RepertoireTemporaire _tmpDepot = new();
    private readonly RepertoireTemporaire _tmpBin = new();

    public RenvoiVersUneIssueExistante()
    {
        FixturesCreerIssues.EcrireDepotMinimal(
            _tmpDepot.Chemin,
            FixturesCreerIssues.PlanPreambule + FixturesCreerIssues.TacheTb014("TB-003", "—"));
        Helpers.CommiterTout(_tmpDepot.Chemin, "fixture renvoi vers issue existante");
        Helpers.ConfigurerOrigineFictive(_tmpDepot.Chemin, FixturesCreerIssues.UrlOrigineFictive, pousser: true);
    }

    public void Dispose()
    {
        _tmpDepot.Dispose();
        _tmpBin.Dispose();
    }

    [Fact]
    public void DevraitRendreLeNumeroDIssueExistantEntreParenthesesPourUneTrancheDejaCreee()
    {
        var environnement = new Dictionary<string, string>
        {
            ["PATH"] = Helpers.RepertoireBinAvecFauxGh(_tmpBin.Chemin),
            ["FAUX_GH_SORTIE"] = FixturesCreerIssues.PayloadIssueExistante("TB-003 — Espace personnage (fixture)", 42, null),
        };
        var resultat = Helpers.ExecuterOutilAvecEnvironnement(
            "CreerIssues", ["--seulement", "TB-014"], _tmpDepot.Chemin, environnement);

        Assert.Equal(0, resultat.CodeSortie);
        string corps = FixturesCreerIssues.ExtraireCorpsIssue(resultat.Stdout, "TB-014");
        Assert.Contains("**Dépend de** : TB-003 (#42)", corps);
    }
}

/// <summary>
/// Comportement n°9 (moitié « dédoublonnage ») — le même identifiant cité
/// deux fois dans un même champ ne produit qu'un seul renvoi.
/// </summary>
public class DedoublonnageDuRenvoiVersLaMemeTranche : IDisposable
{
    private readonly RepertoireTemporaire _tmpDepot = new();
    private readonly RepertoireTemporaire _tmpBin = new();

    public DedoublonnageDuRenvoiVersLaMemeTranche()
    {
        FixturesCreerIssues.EcrireDepotMinimal(
            _tmpDepot.Chemin,
            FixturesCreerIssues.PlanPreambule + FixturesCreerIssues.TacheTb014("TB-003, TB-003", "—"));
        Helpers.CommiterTout(_tmpDepot.Chemin, "fixture dédoublonnage");
        Helpers.ConfigurerOrigineFictive(_tmpDepot.Chemin, FixturesCreerIssues.UrlOrigineFictive, pousser: true);
    }

    public void Dispose()
    {
        _tmpDepot.Dispose();
        _tmpBin.Dispose();
    }

    [Fact]
    public void NeDevraitPasDupliquerLeRenvoiQuandLeMemeIdentifiantEstCiteDeuxFoisDansLeMemeChamp()
    {
        var environnement = new Dictionary<string, string>
        {
            ["PATH"] = Helpers.RepertoireBinAvecFauxGh(_tmpBin.Chemin),
            ["FAUX_GH_SORTIE"] = FixturesCreerIssues.PayloadIssueExistante("TB-003 — Espace personnage (fixture)", 10, null),
        };
        var resultat = Helpers.ExecuterOutilAvecEnvironnement(
            "CreerIssues", ["--seulement", "TB-014"], _tmpDepot.Chemin, environnement);

        Assert.Equal(0, resultat.CodeSortie);
        string corps = FixturesCreerIssues.ExtraireCorpsIssue(resultat.Stdout, "TB-014");
        Assert.Contains("**Dépend de** : TB-003 (#10)", corps);
    }
}

/// <summary>
/// Comportement n°8 — régénérer un corps sans changement du plan ni de
/// l'existant GitHub entre les deux passes produit un résultat IDENTIQUE,
/// pas seulement équivalent.
/// </summary>
public class IdempotenceDeLaRegeneration : IDisposable
{
    private readonly RepertoireTemporaire _tmpDepot = new();
    private readonly RepertoireTemporaire _tmpBin = new();

    public IdempotenceDeLaRegeneration()
    {
        FixturesCreerIssues.EcrireDepotMinimal(
            _tmpDepot.Chemin,
            FixturesCreerIssues.PlanPreambule + FixturesCreerIssues.TacheTb014("—", "—"));
        Helpers.CommiterTout(_tmpDepot.Chemin, "fixture idempotence");
        Helpers.ConfigurerOrigineFictive(_tmpDepot.Chemin, FixturesCreerIssues.UrlOrigineFictive, pousser: true);
    }

    public void Dispose()
    {
        _tmpDepot.Dispose();
        _tmpBin.Dispose();
    }

    [Fact]
    public void DevraitProduireUnCorpsIdentiqueALaSecondeRegenerationSansChangement()
    {
        string bin = Helpers.RepertoireBinAvecFauxGh(_tmpBin.Chemin);

        var premierePasse = Helpers.ExecuterOutilAvecEnvironnement(
            "CreerIssues", ["--seulement", "TB-014"], _tmpDepot.Chemin,
            new Dictionary<string, string> { ["PATH"] = bin, ["FAUX_GH_SORTIE"] = "[]" });
        Assert.Equal(0, premierePasse.CodeSortie);
        string corpsPremierePasse = FixturesCreerIssues.ExtraireCorpsIssue(premierePasse.Stdout, "TB-014");

        var secondePasse = Helpers.ExecuterOutilAvecEnvironnement(
            "CreerIssues", ["--seulement", "TB-014"], _tmpDepot.Chemin,
            new Dictionary<string, string>
            {
                ["PATH"] = bin,
                ["FAUX_GH_SORTIE"] = FixturesCreerIssues.PayloadIssueExistante("TB-014 — Corps existant (fixture)", 7, corpsPremierePasse),
            });
        Assert.Equal(0, secondePasse.CodeSortie);
        string corpsSecondePasse = FixturesCreerIssues.ExtraireCorpsIssue(secondePasse.Stdout, "TB-014");

        Assert.Equal(corpsPremierePasse, corpsSecondePasse);
    }
}

/// <summary>
/// Comportement n°7 — le test le plus important des trois de préservation :
/// un corps existant portant du texte écrit à la main sous le marqueur doit
/// être régénéré sans que ce texte bouge d'un caractère.
/// </summary>
public class PreservationCaracterePourCaractereDeLaZoneManuelle : IDisposable
{
    private readonly RepertoireTemporaire _tmpDepot = new();
    private readonly RepertoireTemporaire _tmpBin = new();

    public PreservationCaracterePourCaractereDeLaZoneManuelle()
    {
        FixturesCreerIssues.EcrireDepotMinimal(
            _tmpDepot.Chemin,
            FixturesCreerIssues.PlanPreambule + FixturesCreerIssues.TacheTb014("—", "—"));
        Helpers.CommiterTout(_tmpDepot.Chemin, "fixture préservation zone manuelle");
        Helpers.ConfigurerOrigineFictive(_tmpDepot.Chemin, FixturesCreerIssues.UrlOrigineFictive, pousser: true);
    }

    public void Dispose()
    {
        _tmpDepot.Dispose();
        _tmpBin.Dispose();
    }

    [Fact]
    public void DevraitPreserverCaracterePourCaractereLeTexteEcritALaMainSousLeMarqueur()
    {
        string ancienCorps =
            "**Tâche de plan** : [`docs/gestion-projet/plan-de-travail.md`](url-perimee) — TB-014\n"
            + "**Estampille de provenance** : ancienne révision, à écraser\n\n"
            + FixturesCreerIssues.MarqueurSeparationCorps
            + "\n\n## Pris par\n\nPierre-Marie, à partir du 2026-08-20.\n\n"
            + "## Notes d'exécution\n\nBloqué par la disponibilité de l'environnement de recette.\n\n"
            + "## Écart constaté\n\nAucun à ce jour.\n";
        string zoneManuelleAttendue = ancienCorps[
            (ancienCorps.IndexOf(FixturesCreerIssues.MarqueurSeparationCorps, StringComparison.Ordinal)
                + FixturesCreerIssues.MarqueurSeparationCorps.Length)..];

        var resultat = Helpers.ExecuterOutilAvecEnvironnement(
            "CreerIssues", ["--seulement", "TB-014"], _tmpDepot.Chemin,
            new Dictionary<string, string>
            {
                ["PATH"] = Helpers.RepertoireBinAvecFauxGh(_tmpBin.Chemin),
                ["FAUX_GH_SORTIE"] = FixturesCreerIssues.PayloadIssueExistante("TB-014 — Peu importe (fixture)", 7, ancienCorps),
            });
        Assert.Equal(0, resultat.CodeSortie);
        string nouveauCorps = FixturesCreerIssues.ExtraireCorpsIssue(resultat.Stdout, "TB-014");
        string zoneManuelleObtenue = nouveauCorps[
            (nouveauCorps.IndexOf(FixturesCreerIssues.MarqueurSeparationCorps, StringComparison.Ordinal)
                + FixturesCreerIssues.MarqueurSeparationCorps.Length)..];

        Assert.Equal(zoneManuelleAttendue, zoneManuelleObtenue);
    }
}

/// <summary>
/// Second volet du comportement n°7 — si le marqueur est absent (corps
/// antérieur au modèle), le corps existant est reporté intégralement plutôt
/// qu'écrasé.
/// </summary>
public class ReportIntegralDUnCorpsAnterieurSansMarqueur : IDisposable
{
    private readonly RepertoireTemporaire _tmpDepot = new();
    private readonly RepertoireTemporaire _tmpBin = new();

    public ReportIntegralDUnCorpsAnterieurSansMarqueur()
    {
        FixturesCreerIssues.EcrireDepotMinimal(
            _tmpDepot.Chemin,
            FixturesCreerIssues.PlanPreambule + FixturesCreerIssues.TacheTb014("—", "—"));
        Helpers.CommiterTout(_tmpDepot.Chemin, "fixture report intégral");
        Helpers.ConfigurerOrigineFictive(_tmpDepot.Chemin, FixturesCreerIssues.UrlOrigineFictive, pousser: true);
    }

    public void Dispose()
    {
        _tmpDepot.Dispose();
        _tmpBin.Dispose();
    }

    [Fact]
    public void DevraitReporterIntegralementUnCorpsAnterieurSansMarqueurPlutotQueLEcraser()
    {
        const string ancienCorpsSansMarqueur =
            "Notes libres antérieures au modèle de corps, écrites avant que ce "
            + "script n'existe.\nDeuxième ligne, à conserver aussi.";

        var resultat = Helpers.ExecuterOutilAvecEnvironnement(
            "CreerIssues", ["--seulement", "TB-014"], _tmpDepot.Chemin,
            new Dictionary<string, string>
            {
                ["PATH"] = Helpers.RepertoireBinAvecFauxGh(_tmpBin.Chemin),
                ["FAUX_GH_SORTIE"] = FixturesCreerIssues.PayloadIssueExistante("TB-014 — Peu importe (fixture)", 7, ancienCorpsSansMarqueur),
            });
        Assert.Equal(0, resultat.CodeSortie);
        string nouveauCorps = FixturesCreerIssues.ExtraireCorpsIssue(resultat.Stdout, "TB-014");

        string attendu = "\n\n" + ancienCorpsSansMarqueur + "\n";
        Assert.EndsWith(attendu, nouveauCorps);
    }
}

/// <summary>
/// Comportement n°10 (moitié mode à blanc) — avertit quand la révision
/// courante n'est pas joignable sur le distant. `gh` est rendu injoignable
/// (PATH restreint à `git` seul) pour garantir qu'aucun appel réseau n'est
/// jamais tenté, quel que soit l'état d'authentification `gh` de la machine
/// hôte.
/// </summary>
public class AvertissementRevisionNonJoignable : IDisposable
{
    private readonly RepertoireTemporaire _tmpDepot = new();
    private readonly RepertoireTemporaire _tmpBin = new();

    public AvertissementRevisionNonJoignable()
    {
        FixturesCreerIssues.EcrireDepotMinimal(
            _tmpDepot.Chemin,
            FixturesCreerIssues.PlanPreambule + FixturesCreerIssues.TacheTb014("—", "—"));
        Helpers.CommiterTout(_tmpDepot.Chemin, "fixture révision non poussée");
        Helpers.ConfigurerOrigineFictive(_tmpDepot.Chemin, FixturesCreerIssues.UrlOrigineFictive, pousser: false);
    }

    public void Dispose()
    {
        _tmpDepot.Dispose();
        _tmpBin.Dispose();
    }

    [Fact]
    public void DevraitAvertirEnModeABlancQuandLaRevisionNEstPasEncoreJoignableSurLeDistant()
    {
        var environnement = new Dictionary<string, string> { ["PATH"] = Helpers.RepertoireBinSansGh(_tmpBin.Chemin) };
        var resultat = Helpers.ExecuterOutilAvecEnvironnement("CreerIssues", [], _tmpDepot.Chemin, environnement);

        // Différence assumée avec le Python d'origine (qui rendait toujours 0,
        // faute de sys.exit) : le contrat de sortie arbitré pour ce portage
        // compte explicitement cet avertissement comme un signal opérateur
        // (Orchestrateur.cs, `signalOperateur = true` sur ce chemin) — mesuré
        // ici, code 1.
        Assert.Equal(1, resultat.CodeSortie);
        Assert.Contains("MODE À BLANC — rien n'est créé ni modifié sur GitHub.", resultat.Stdout);
        Assert.Contains("AVERTISSEMENT :", resultat.Stdout);
        Assert.Contains("n'est pas (encore) présente sur le distant", resultat.Stdout);
    }
}

/// <summary>
/// Comportement n°10 (second régime) — reste silencieux sur ce point quand la
/// révision courante est déjà joignable sur le distant.
/// </summary>
public class SilenceQuandLaRevisionEstDejaJoignable : IDisposable
{
    private readonly RepertoireTemporaire _tmpDepot = new();
    private readonly RepertoireTemporaire _tmpBin = new();

    public SilenceQuandLaRevisionEstDejaJoignable()
    {
        FixturesCreerIssues.EcrireDepotMinimal(
            _tmpDepot.Chemin,
            FixturesCreerIssues.PlanPreambule + FixturesCreerIssues.TacheTb014("—", "—"));
        Helpers.CommiterTout(_tmpDepot.Chemin, "fixture révision poussée");
        Helpers.ConfigurerOrigineFictive(_tmpDepot.Chemin, FixturesCreerIssues.UrlOrigineFictive, pousser: true);
    }

    public void Dispose()
    {
        _tmpDepot.Dispose();
        _tmpBin.Dispose();
    }

    [Fact]
    public void NeDevraitPasAvertirQuandLaRevisionEstDejaJoignableSurLeDistant()
    {
        var environnement = new Dictionary<string, string> { ["PATH"] = Helpers.RepertoireBinSansGh(_tmpBin.Chemin) };
        var resultat = Helpers.ExecuterOutilAvecEnvironnement("CreerIssues", [], _tmpDepot.Chemin, environnement);

        Assert.Equal(0, resultat.CodeSortie);
        Assert.DoesNotContain("AVERTISSEMENT", resultat.Stdout);
    }
}

/// <summary>
/// Comportement neuf — le garde de révision se comporte différemment sous
/// <c>--appliquer</c> : il REFUSE d'écrire (code de sortie 1, message sur
/// stderr) plutôt que d'avertir puis continuer. Sans équivalent Python
/// (l'original n'exerçait jamais <c>--appliquer</c>). `gh` doit être présent
/// et authentifié pour atteindre ce garde (il est vérifié avant la lecture de
/// la révision) : le faux `gh` de `tools/Harnais/FauxGh` en tient lieu,
/// jamais le vrai — l'identité du « gh » résolu par le PATH du
/// sous-processus est établie AVANT tout exercice du drapeau, via
/// `GardeIdentiteGh`. Prouve aussi que le refus intervient AVANT toute
/// tentative d'écriture : la seule invocation journalisée du faux `gh` est
/// l'authentification, jamais une création de libellé, de jalon ou d'issue.
/// </summary>
public class RefusEcritureSousAppliquerQuandLaRevisionNEstPasJoignable : IDisposable
{
    private readonly RepertoireTemporaire _tmpDepot = new();
    private readonly RepertoireTemporaire _tmpBin = new();
    private readonly string _cheminBin;
    private readonly string _journal;

    public RefusEcritureSousAppliquerQuandLaRevisionNEstPasJoignable()
    {
        FixturesCreerIssues.EcrireDepotMinimal(
            _tmpDepot.Chemin,
            FixturesCreerIssues.PlanPreambule + FixturesCreerIssues.TacheTb014("—", "—"));
        Helpers.CommiterTout(_tmpDepot.Chemin, "fixture refus --appliquer");
        Helpers.ConfigurerOrigineFictive(_tmpDepot.Chemin, FixturesCreerIssues.UrlOrigineFictive, pousser: false);

        _cheminBin = Helpers.RepertoireBinAvecFauxGh(_tmpBin.Chemin);
        _journal = Path.Combine(_tmpBin.Chemin, "journal-gh.jsonl");
    }

    public void Dispose()
    {
        _tmpDepot.Dispose();
        _tmpBin.Dispose();
    }

    [Fact]
    public void DevraitRefuserDEcrireSousAppliquerEtNAppellerGhQuePourLAuthentification()
    {
        var environnementGarde = new Dictionary<string, string> { ["PATH"] = _cheminBin };

        // Séquence obligatoire avant tout exercice de --appliquer : établir
        // que le « gh » résolu depuis ce PATH est bien le faux gh de ce
        // harnais, code 0 constaté sans tube. Un refus ici doit interrompre
        // le test plutôt que de risquer un appel réseau réel.
        var garde = Helpers.ExecuterOutilAvecEnvironnement("GardeIdentiteGh", [], _tmpDepot.Chemin, environnementGarde);
        Assert.Equal(0, garde.CodeSortie);

        var environnementCreerIssues = new Dictionary<string, string>
        {
            ["PATH"] = _cheminBin,
            ["JOURNAL_GH"] = _journal,
        };
        var resultat = Helpers.ExecuterOutilAvecEnvironnement(
            "CreerIssues", ["--appliquer"], _tmpDepot.Chemin, environnementCreerIssues);

        Assert.Equal(1, resultat.CodeSortie);
        Assert.Contains("Erreur :", resultat.Stderr);
        Assert.Contains("n'est pas (encore) présente sur le distant", resultat.Stderr);
        Assert.Contains("Poussez cette révision avant de créer des tickets", resultat.Stderr);

        string[] lignesJournal = File.ReadAllLines(_journal);
        var invocations = lignesJournal
            .Select(l => JsonDocument.Parse(l).RootElement.GetProperty("args").EnumerateArray().Select(e => e.GetString()).ToArray())
            .ToList();
        Assert.Single(invocations);
        Assert.Equal(new[] { "auth", "status" }, invocations[0]);
    }
}

/// <summary>
/// Comportement n°10 (le code de sortie réel) — à la différence des deux
/// vérificateurs déterministes, `CreerIssues` PORTE un vrai code de sortie
/// non trivial : toute `ErreurCorpusException` (corpus malformé) rend 1, avec
/// message sur stderr. Exercé ici via un `methode-de-ticket.md` malformé
/// (marqueur de taxonomie absent) — l'échec d'extraction survient avant toute
/// lecture réseau ou tentative `gh`, donc sans risque même sans restreindre
/// le PATH.
/// </summary>
public class CodeDeSortieUnSurMethodeDeTicketMalformee : IDisposable
{
    private readonly RepertoireTemporaire _tmp = new();

    public CodeDeSortieUnSurMethodeDeTicketMalformee()
    {
        Helpers.InitialiserDepotGit(_tmp.Chemin);
        Helpers.Ecrire(Path.Combine(_tmp.Chemin, "docs/gestion-projet/plan-de-travail.md"), "# Plan de travail (fixture)\n");
        // Méthode de ticket sans le marqueur « **Par jalon** » attendu par
        // Taxonomie.Extraire() — structure changée, extraction impossible.
        Helpers.Ecrire(
            Path.Combine(_tmp.Chemin, "docs/gestion-projet/methode-de-ticket.md"),
            "# Méthode de ticket (fixture volontairement malformée)\n\nAucune taxonomie ici.\n");
        Helpers.CommiterTout(_tmp.Chemin, "méthode malformée");
    }

    public void Dispose() => _tmp.Dispose();

    [Fact]
    public void DevraitRendreLeCodeUnEtUnMessageDErreurSurStderrQuandLaMethodeEstMalformee()
    {
        var resultat = Helpers.ExecuterOutil("CreerIssues", Array.Empty<string>(), _tmp.Chemin);
        Assert.Equal(1, resultat.CodeSortie);
        Assert.Contains("Erreur :", resultat.Stderr);
        Assert.Equal(string.Empty, resultat.Stdout.Trim());
    }
}
