// Caractérise `tools/CreerProjet` sur son binaire compilé, en sous-processus,
// même stratégie que CreerIssuesTests.cs (son modèle assumé) : `gh` est
// toujours remplacé par le faux `gh` de `tools/Harnais/FauxGh`, jamais le vrai
// — aucun de ces tests ne contacte GitHub. Champ couvert : l'idempotence des
// deux champs de projet (Statut, Épique) posés par `Orchestrateur.Appliquer`
// — un champ n'est réécrit que si sa valeur cible diverge de la valeur déjà
// posée sur l'élément, prouvé par le JOURNAL d'appels du faux `gh`, jamais par
// une valeur recalculée (un appel réellement parti vers le service ne se
// verrait pas dans une simple assertion sur un compteur).
using System.Text.Json;
using Xunit;

namespace CaracterisationOutils;

/// <summary>
/// Fixtures et réponses partagées par les classes de ce fichier. Le socle
/// « lecture du plan » de CreerProjet est plus petit que celui de CreerIssues :
/// ni maille de désignation (§2), ni taxonomie de libellés (§3), ni
/// wireframes — seuls `plan-de-travail.md` (tâches, dont leur champ « Épique »)
/// et `methode-de-ticket.md §5` (le schéma des états) sont lus.
/// </summary>
internal static class FixturesCreerProjet
{
    public const string UrlOrigineFictive = "git@github.com:exemple-org/exemple-depot.git";
    public const string TitreProjet = "PM-micro-saas - Haversack";
    public const int NumeroProjet = 3;
    public const string IdProjet = "PID1";
    public const string UrlProjet = "https://github.com/orgs/exemple-org/projects/3";
    public const string IdChampStatut = "FID_STATUT";
    public const string IdChampEpique = "FID_EPIQUE";
    public const string IdOptionOuvertNonPret = "O1";
    public const string IdOptionPret = "O2";
    public const string IdOptionFondations = "E1";
    public const string IdItem = "ITEM1";
    public const int NumeroIssue = 7;
    public const string TitreIssueTb014 = "TB-014 — Création d'un personnage joueur";

    // §5 minimal mais complet pour EtatsCycleDeVie.Lire : cinq états séparés
    // par des flèches, puis une liste à puces qui borne le bloc lu (même
    // convention que le corpus réel — la liste elle-même n'est jamais
    // parsée, seul son premier marqueur « \n- ** » borne la lecture).
    public const string MethodeMd =
        "# Méthode de ticket (fixture de test)\n\n" +
        "## §5 Cycle de vie\n\n" +
        "**États et transitions**\n\n" +
        "Ouvert, non prêt → Prêt → Pris → En cours → Terminé\n\n" +
        "- **Ouvert, non prêt** : rien n'a démarré.\n" +
        "- **Prêt** : prêt à être pris.\n" +
        "- **Pris** : quelqu'un s'en charge.\n" +
        "- **En cours** : en cours de réalisation.\n" +
        "- **Terminé** : fini.\n";

    // Une seule tâche, sans dépendance ni conflit, périmètre et critères
    // sans marqueur TROU : StatutCalcule y rend systématiquement « Prêt »
    // (methode-de-ticket.md §2) — cible déterministe, jamais recalculée à la
    // main dans les tests eux-mêmes.
    public const string PlanMd =
        "# Plan de travail (fixture de test)\n\n" +
        "##### TB-014 — Création d'un personnage joueur\n\n" +
        "| Champ | Valeur |\n|---|---|\n" +
        "| Périmètre d'écriture | Personnage |\n" +
        "| Dépend de | — |\n" +
        "| En conflit avec | — |\n" +
        "| Épique | Fondations |\n";

    public static void EcrireDepotMinimal(string depot)
    {
        Helpers.InitialiserDepotGit(depot);
        Helpers.Ecrire(Path.Combine(depot, "docs/gestion-projet/methode-de-ticket.md"), MethodeMd);
        Helpers.Ecrire(Path.Combine(depot, "docs/gestion-projet/plan-de-travail.md"), PlanMd);
    }

    public static string ReponseProjetExistant() => JsonSerializer.Serialize(new
    {
        projects = new[] { new { title = TitreProjet, number = NumeroProjet, id = IdProjet, url = UrlProjet } },
    });

    // Les deux champs existent déjà : Appliquer lit alors leurs options par
    // GraphQL (ChampListe, Orchestrateur.cs) plutôt que de les créer — d'où
    // les deux réponses GraphQL distinctes qui suivent celle-ci dans la
    // séquence de rangs.
    public static string ReponseChampsExistants() => JsonSerializer.Serialize(new
    {
        fields = new object[]
        {
            new { id = IdChampStatut, name = "Statut", options = new[]
            {
                new { id = IdOptionOuvertNonPret, name = "Ouvert, non prêt" },
                new { id = IdOptionPret, name = "Prêt" },
            } },
            new { id = IdChampEpique, name = "Épique", options = new[]
            {
                new { id = IdOptionFondations, name = "Fondations" },
            } },
        },
    });

    public static string ReponseGraphqlOptionsStatut() => JsonSerializer.Serialize(new
    {
        data = new
        {
            node = new
            {
                id = IdChampStatut,
                options = new[]
                {
                    new { id = IdOptionOuvertNonPret, name = "Ouvert, non prêt" },
                    new { id = IdOptionPret, name = "Prêt" },
                },
            },
        },
    });

    public static string ReponseGraphqlOptionsEpique() => JsonSerializer.Serialize(new
    {
        data = new { node = new { id = IdChampEpique, options = new[] { new { id = IdOptionFondations, name = "Fondations" } } } },
    });

    public static string ReponseIssueExistante() => JsonSerializer.Serialize(new[]
    {
        new { number = NumeroIssue, title = TitreIssueTb014, url = $"https://github.com/exemple-org/exemple-depot/issues/{NumeroIssue}", state = "OPEN" },
    });

    public static string ReponseItemListeVide() => JsonSerializer.Serialize(new { items = Array.Empty<object>() });

    public static string ReponseItemAjoute() => JsonSerializer.Serialize(new { id = IdItem });

    /// <summary>
    /// Un seul élément déjà posé dans le projet, portant les valeurs de champ
    /// données — c'est le NOM d'option (jamais son identifiant) que
    /// `gh project item-list --format json` projette, sous une clé en
    /// camelCase du nom du champ (« statut », « épique »).
    /// </summary>
    public static string ReponseItemExistant(string statut, string epique) => JsonSerializer.Serialize(new
    {
        items = new[]
        {
            new Dictionary<string, object>
            {
                ["id"] = IdItem,
                ["content"] = new { title = TitreIssueTb014 },
                ["statut"] = statut,
                ["épique"] = epique,
            },
        },
    });

    /// <summary>
    /// Écrit le fichier de réponses ordonnées par rang d'appel — séquence
    /// commune aux deux tests de ce fichier jusqu'au rang 4 inclus (projet et
    /// champs déjà présents, une issue déjà ouverte) : (0) projet, (1) champs,
    /// (2) options Statut, (3) options Épique, (4) issues, puis
    /// <paramref name="suite"/> à partir du rang 5 (lecture des éléments du
    /// projet, et au-delà selon le scénario).
    /// </summary>
    public static string EcrireReponses(string dossier, params string[] suite)
    {
        string chemin = Path.Combine(dossier, $"reponses-{Guid.NewGuid():N}.json");
        var reponses = new List<string>
        {
            ReponseProjetExistant(),
            ReponseChampsExistants(),
            ReponseGraphqlOptionsStatut(),
            ReponseGraphqlOptionsEpique(),
            ReponseIssueExistante(),
        };
        reponses.AddRange(suite);
        Helpers.Ecrire(chemin, JsonSerializer.Serialize(reponses.ToArray()));
        return chemin;
    }

    /// <summary>
    /// Séquence de réponses propre à l'aperçu à blanc — distincte de
    /// <see cref="EcrireReponses"/> : <c>ApercuModeABlanc</c> ne lit jamais
    /// les options de champ par GraphQL (seul <c>Appliquer</c> en a besoin,
    /// pour écrire un identifiant d'option), donc quatre rangs seulement :
    /// (0) projet, (1) champs, (2) issues, (3) éléments du projet.
    /// </summary>
    public static string EcrireReponsesApercu(string dossier, string reponseItemListe)
    {
        string chemin = Path.Combine(dossier, $"reponses-{Guid.NewGuid():N}.json");
        var reponses = new[]
        {
            ReponseProjetExistant(),
            ReponseChampsExistants(),
            ReponseIssueExistante(),
            reponseItemListe,
        };
        Helpers.Ecrire(chemin, JsonSerializer.Serialize(reponses));
        return chemin;
    }

    public static List<string[]> LireJournal(string chemin) =>
        File.Exists(chemin)
            ? File.ReadAllLines(chemin)
                .Where(l => l.Trim().Length > 0)
                .Select(l => JsonDocument.Parse(l).RootElement.GetProperty("args")
                    .EnumerateArray().Select(e => e.GetString() ?? "").ToArray())
                .ToList()
            : new List<string[]>();
}

/// <summary>
/// Comportement central de cette correction — l'idempotence des champs de
/// projet. Deux passes sous faux `gh`, contre le même dépôt de fixture,
/// jamais poussé à nouveau entre les deux (rien ne le requiert : CreerProjet
/// ne lit aucune révision, à la différence de CreerIssues) : la première
/// pose l'élément (absent du projet), la seconde retrouve exactement les
/// valeurs que la première a posées — prouvé en relisant le JOURNAL de la
/// seconde passe, jamais un compteur recalculé : AUCUN appel `item-add` ni
/// `item-edit` n'y figure.
/// </summary>
public class IdempotenceDesChampsDeProjetSousAppliquer : IDisposable
{
    private readonly RepertoireTemporaire _tmpDepot = new();
    private readonly RepertoireTemporaire _tmpBin = new();
    private readonly string _cheminBin;

    public IdempotenceDesChampsDeProjetSousAppliquer()
    {
        FixturesCreerProjet.EcrireDepotMinimal(_tmpDepot.Chemin);
        Helpers.CommiterTout(_tmpDepot.Chemin, "fixture idempotence CreerProjet");
        Helpers.ConfigurerOrigineFictive(_tmpDepot.Chemin, FixturesCreerProjet.UrlOrigineFictive, pousser: false);
        _cheminBin = Helpers.RepertoireBinAvecFauxGh(_tmpBin.Chemin);
    }

    public void Dispose()
    {
        _tmpDepot.Dispose();
        _tmpBin.Dispose();
    }

    [Fact]
    public void DevraitEcrireALaPremierePasseEtNeRienEcrireALaSecondePasseIdentique()
    {
        // ── Première passe : l'élément n'est pas encore dans le projet —
        // item-add puis les deux item-edit (épique, statut) doivent partir.
        string journal1 = Path.Combine(_tmpBin.Chemin, "journal-passe-1.jsonl");
        var environnement1 = new Dictionary<string, string>
        {
            ["PATH"] = _cheminBin,
            ["JOURNAL_GH"] = journal1,
            ["FAUX_GH_REPONSES"] = FixturesCreerProjet.EcrireReponses(
                _tmpBin.Chemin,
                FixturesCreerProjet.ReponseItemListeVide(),
                FixturesCreerProjet.ReponseItemAjoute(),
                "{}", // item-edit épique
                "{}"), // item-edit statut
        };
        var passe1 = Helpers.ExecuterOutilAvecEnvironnement("CreerProjet", ["--appliquer"], _tmpDepot.Chemin, environnement1);
        Assert.Equal(0, passe1.CodeSortie);

        var invocations1 = FixturesCreerProjet.LireJournal(journal1);
        Assert.Contains(invocations1, a => a.Contains("item-add"));
        Assert.Equal(2, invocations1.Count(a => a.Contains("item-edit")));

        // ── Seconde passe : le projet simulé porte désormais exactement les
        // valeurs que la première passe a posées (« Prêt », « Fondations »)
        // — aucune divergence, donc AUCUN item-add ni item-edit ne doit
        // partir.
        string journal2 = Path.Combine(_tmpBin.Chemin, "journal-passe-2.jsonl");
        var environnement2 = new Dictionary<string, string>
        {
            ["PATH"] = _cheminBin,
            ["JOURNAL_GH"] = journal2,
            ["FAUX_GH_REPONSES"] = FixturesCreerProjet.EcrireReponses(
                _tmpBin.Chemin,
                FixturesCreerProjet.ReponseItemExistant(statut: "Prêt", epique: "Fondations")),
        };
        var passe2 = Helpers.ExecuterOutilAvecEnvironnement("CreerProjet", ["--appliquer"], _tmpDepot.Chemin, environnement2);
        Assert.Equal(0, passe2.CodeSortie);

        var invocations2 = FixturesCreerProjet.LireJournal(journal2);
        Assert.DoesNotContain(invocations2, a => a.Contains("item-add"));
        Assert.DoesNotContain(invocations2, a => a.Contains("item-edit"));
        Assert.Equal(6, invocations2.Count);
    }
}

/// <summary>
/// Non-régression — un champ dont la valeur diverge de la cible calculée
/// reste réécrit. L'élément existe déjà dans le projet avec un Statut
/// périmé (« Terminé », alors que la cible calculée est « Prêt ») mais une
/// Épique déjà correcte (« Fondations ») : seul le champ Statut diverge, et
/// seul lui doit produire un appel `item-edit` — la correction ne retire
/// jamais une écriture utile, et n'en ajoute pas non plus sur le champ déjà
/// à jour.
/// </summary>
public class NonRegressionUnChampDivergentResteReecrit : IDisposable
{
    private readonly RepertoireTemporaire _tmpDepot = new();
    private readonly RepertoireTemporaire _tmpBin = new();
    private readonly string _cheminBin;
    private readonly string _journal;

    public NonRegressionUnChampDivergentResteReecrit()
    {
        FixturesCreerProjet.EcrireDepotMinimal(_tmpDepot.Chemin);
        Helpers.CommiterTout(_tmpDepot.Chemin, "fixture non-régression CreerProjet");
        Helpers.ConfigurerOrigineFictive(_tmpDepot.Chemin, FixturesCreerProjet.UrlOrigineFictive, pousser: false);
        _cheminBin = Helpers.RepertoireBinAvecFauxGh(_tmpBin.Chemin);
        _journal = Path.Combine(_tmpBin.Chemin, "journal-gh.jsonl");
    }

    public void Dispose()
    {
        _tmpDepot.Dispose();
        _tmpBin.Dispose();
    }

    [Fact]
    public void DevraitReecrireLeSeulChampStatutDivergentSansToucherAuChampEpiqueDejaAJour()
    {
        var environnement = new Dictionary<string, string>
        {
            ["PATH"] = _cheminBin,
            ["JOURNAL_GH"] = _journal,
            ["FAUX_GH_REPONSES"] = FixturesCreerProjet.EcrireReponses(
                _tmpBin.Chemin,
                FixturesCreerProjet.ReponseItemExistant(statut: "Terminé", epique: "Fondations"),
                "{}"), // item-edit statut, seul appel de mise à jour attendu
        };
        var resultat = Helpers.ExecuterOutilAvecEnvironnement("CreerProjet", ["--appliquer"], _tmpDepot.Chemin, environnement);
        Assert.Equal(0, resultat.CodeSortie);

        var invocations = FixturesCreerProjet.LireJournal(_journal);
        var appelsItemEdit = invocations.Where(a => a.Contains("item-edit")).ToList();
        Assert.Single(appelsItemEdit);
        Assert.DoesNotContain(invocations, a => a.Contains("item-add"));

        int indexFieldId = Array.IndexOf(appelsItemEdit[0], "--field-id");
        Assert.True(indexFieldId >= 0);
        Assert.Equal(FixturesCreerProjet.IdChampStatut, appelsItemEdit[0][indexFieldId + 1]);

        int indexOptionId = Array.IndexOf(appelsItemEdit[0], "--single-select-option-id");
        Assert.True(indexOptionId >= 0);
        Assert.Equal(FixturesCreerProjet.IdOptionPret, appelsItemEdit[0][indexOptionId + 1]);
    }
}

/// <summary>
/// Mode à blanc — troisième propriété exigée : sur un tableau où le projet,
/// les deux champs et l'unique élément existent déjà et portent déjà les
/// bonnes valeurs, l'aperçu annonce des voies « déjà présent » partout,
/// jamais « à créer » ni « à ajouter ». Ce mode n'écrit jamais rien sur
/// GitHub : le journal ne doit porter que des lectures (aucun
/// <c>item-add</c>, <c>item-edit</c>, <c>project create</c> ni appel
/// GraphQL — <c>ApercuModeABlanc</c> n'en émet d'ailleurs jamais).
/// </summary>
public class ModeABlancRefleteLEtatDistantReel : IDisposable
{
    private readonly RepertoireTemporaire _tmpDepot = new();
    private readonly RepertoireTemporaire _tmpBin = new();
    private readonly string _cheminBin;
    private readonly string _journal;

    public ModeABlancRefleteLEtatDistantReel()
    {
        FixturesCreerProjet.EcrireDepotMinimal(_tmpDepot.Chemin);
        Helpers.CommiterTout(_tmpDepot.Chemin, "fixture aperçu à blanc CreerProjet");
        Helpers.ConfigurerOrigineFictive(_tmpDepot.Chemin, FixturesCreerProjet.UrlOrigineFictive, pousser: false);
        _cheminBin = Helpers.RepertoireBinAvecFauxGh(_tmpBin.Chemin);
        _journal = Path.Combine(_tmpBin.Chemin, "journal-gh.jsonl");
    }

    public void Dispose()
    {
        _tmpDepot.Dispose();
        _tmpBin.Dispose();
    }

    [Fact]
    public void NeDevraitAnnoncerAucuneCreationQuandToutExisteDejaEtEstDejaAJour()
    {
        var environnement = new Dictionary<string, string>
        {
            ["PATH"] = _cheminBin,
            ["JOURNAL_GH"] = _journal,
            ["FAUX_GH_REPONSES"] = FixturesCreerProjet.EcrireReponsesApercu(
                _tmpBin.Chemin,
                FixturesCreerProjet.ReponseItemExistant(statut: "Prêt", epique: "Fondations")),
        };
        // Pas de --appliquer : mode à blanc.
        var resultat = Helpers.ExecuterOutilAvecEnvironnement("CreerProjet", [], _tmpDepot.Chemin, environnement);
        Assert.Equal(0, resultat.CodeSortie);

        Assert.DoesNotContain("à créer", resultat.Stdout);
        Assert.Contains($"= projet « {FixturesCreerProjet.TitreProjet} » déjà présent", resultat.Stdout);
        Assert.Contains("= champ « Statut » déjà présent (2 valeurs)", resultat.Stdout);
        Assert.Contains("= champ « Épique » déjà présent (1 valeurs)", resultat.Stdout);
        Assert.Contains("+ 0 élément(s) à ajouter au projet", resultat.Stdout);
        Assert.Contains("= 1 élément(s) déjà présent(s), champs déjà à jour", resultat.Stdout);
        Assert.Contains("~ 0 élément(s) déjà présent(s)", resultat.Stdout);

        var invocations = FixturesCreerProjet.LireJournal(_journal);
        Assert.Equal(4, invocations.Count);
        Assert.DoesNotContain(invocations, a => a.Contains("item-add") || a.Contains("item-edit")
            || a.Contains("create") || a.Contains("link") || a.Contains("graphql"));
    }
}
