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

    // « color »/« description » désormais lus par Orchestrateur.ChampListe
    // (une mise à jour de champ liste réécrit l'intégralité du tableau
    // d'options — voir RequeteMettreAJourChampListe — et doit donc pouvoir
    // reprendre la couleur/description ACTUELLE d'une option non touchée) :
    // absents d'une réponse fixture, `GetProperty("color")` lèverait.
    public static string ReponseGraphqlOptionsStatut() => JsonSerializer.Serialize(new
    {
        data = new
        {
            node = new
            {
                id = IdChampStatut,
                options = new[]
                {
                    new { id = IdOptionOuvertNonPret, name = "Ouvert, non prêt", color = "GRAY", description = "" },
                    new { id = IdOptionPret, name = "Prêt", color = "BLUE", description = "" },
                },
            },
        },
    });

    public static string ReponseGraphqlOptionsEpique() => JsonSerializer.Serialize(new
    {
        data = new { node = new { id = IdChampEpique, options = new[] { new { id = IdOptionFondations, name = "Fondations", color = "GRAY", description = "" } } } },
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

    /// <summary>
    /// Comme <see cref="LireJournal"/>, en conservant aussi l'entrée standard
    /// de chaque appel (jamais lue par <see cref="LireJournal"/>) — seul
    /// moyen de vérifier CE QUI PART RÉELLEMENT vers `gh` pour un appel
    /// GraphQL (le vecteur d'arguments d'un appel GraphQL vaut toujours
    /// <c>["api","graphql","--input","-"]</c>, quelle que soit la requête :
    /// la charge, elle, ne voyage que sur l'entrée standard — voir Graphql.cs).
    /// </summary>
    public static List<(string[] Args, string Stdin)> LireJournalAvecEntreeStandard(string chemin) =>
        File.Exists(chemin)
            ? File.ReadAllLines(chemin)
                .Where(l => l.Trim().Length > 0)
                .Select(l =>
                {
                    var racine = JsonDocument.Parse(l).RootElement;
                    var args = racine.GetProperty("args").EnumerateArray().Select(e => e.GetString() ?? "").ToArray();
                    return (args, racine.GetProperty("stdin").GetString() ?? "");
                })
                .ToList()
            : new List<(string[], string)>();
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

/// <summary>
/// Fixtures propres au renommage des options du champ « Épique » — un plan
/// portant, à la différence de <see cref="FixturesCreerProjet"/>, une section
/// « Vue des épiques » (plan-de-travail.md §4), seule source du libellé
/// complet (clé + intitulé) et du but posés sur une option. Deux tâches :
/// TB-100 référence une clé DÉCRITE par cette section (EP-02, dont le nom
/// d'option historique — avant toute migration — vaut la clé seule) ; TB-200
/// référence une clé ABSENTE de cette section (EP-88), dont le libellé retombe
/// donc sur la clé seule (repli, voir Orchestrateur.LireLibellesEpiques).
/// </summary>
internal static class FixturesRenommageEpique
{
    public const string LibelleEp02 = "EP-02 — Échafaudage de la solution .NET";
    public const string ButEp02 = "les projets .NET existent et s'assemblent";
    public const string LibelleEp88 = "EP-88"; // repli sur la clé seule : aucune ligne EP-88 dans la Vue des épiques.
    public const string IdOptionEp02 = "E_EP02";
    public const string IdOptionFondations = "E_FONDATIONS_RELIQUAT";
    public const string IdOptionEp88 = "E_EP88";
    public const string TitreIssueTb100 = "TB-100 — Tâche de test pour le renommage";
    public const string TitreIssueTb200 = "TB-200 — Tâche de test pour la création";
    public const string IdItem100 = "ITEM100";
    public const string IdItem200 = "ITEM200";

    // Section 4 bornée par un titre de niveau 2 suivant (même convention que
    // le plan réel, où « ## 5. J0 — épiques et tranches » borne la Vue des
    // épiques) — preuve que LireLibellesEpiques ne lit jamais au-delà dans
    // les fiches de tâches qui suivent. « Fondations » n'y figure pas : c'est
    // le reliquat non apparié à aucune clé courante (voir
    // ReconcilierOptionsEpique), jamais référencé par aucune tâche de ce plan.
    public const string PlanMd =
        "# Plan de travail (fixture de test)\n\n" +
        "## 4. Vue des épiques\n\n" +
        "### J0\n\n" +
        "| Épique | But | Jalon | Dépend de | Tranches |\n|---|---|---|---|---|\n" +
        $"| {LibelleEp02} | {ButEp02} | J0 | — | TB-100 |\n\n" +
        "## 5. Tranches\n\n" +
        "##### TB-100 — Tâche de test pour le renommage\n\n" +
        "| Champ | Valeur |\n|---|---|\n" +
        "| Périmètre d'écriture | Test |\n" +
        "| Dépend de | — |\n" +
        "| En conflit avec | — |\n" +
        "| Épique | EP-02 |\n\n" +
        "##### TB-200 — Tâche de test pour la création\n\n" +
        "| Champ | Valeur |\n|---|---|\n" +
        "| Périmètre d'écriture | Test |\n" +
        "| Dépend de | — |\n" +
        "| En conflit avec | — |\n" +
        "| Épique | EP-88 |\n";

    public static void EcrireDepotMinimal(string depot)
    {
        Helpers.InitialiserDepotGit(depot);
        Helpers.Ecrire(Path.Combine(depot, "docs/gestion-projet/methode-de-ticket.md"), FixturesCreerProjet.MethodeMd);
        Helpers.Ecrire(Path.Combine(depot, "docs/gestion-projet/plan-de-travail.md"), PlanMd);
    }

    public static string ReponseChampsExistants() => JsonSerializer.Serialize(new
    {
        fields = new object[]
        {
            new { id = FixturesCreerProjet.IdChampStatut, name = "Statut" },
            new { id = FixturesCreerProjet.IdChampEpique, name = "Épique" },
        },
    });

    /// <summary>Avant migration : « EP-02 » (forme brute historique) et un reliquat « Fondations » non référencé par aucune tâche de ce plan.</summary>
    public static string ReponseGraphqlEpiqueAvantMigration() => JsonSerializer.Serialize(new
    {
        data = new
        {
            node = new
            {
                id = FixturesCreerProjet.IdChampEpique,
                options = new[]
                {
                    new { id = IdOptionEp02, name = "EP-02", color = "GRAY", description = "" },
                    new { id = IdOptionFondations, name = "Fondations", color = "BLUE", description = "" },
                },
            },
        },
    });

    /// <summary>Après migration : EP-02 renommé (identifiant préservé), Fondations inchangé, EP-88 créé.</summary>
    public static string ReponseGraphqlEpiqueApresMigration() => JsonSerializer.Serialize(new
    {
        data = new
        {
            node = new
            {
                id = FixturesCreerProjet.IdChampEpique,
                options = new[]
                {
                    new { id = IdOptionEp02, name = LibelleEp02, color = "GRAY", description = ButEp02 },
                    new { id = IdOptionFondations, name = "Fondations", color = "BLUE", description = "" },
                    new { id = IdOptionEp88, name = LibelleEp88, color = "GRAY", description = "" },
                },
            },
        },
    });

    public static string ReponseMutationMiseAJourChamp() => JsonSerializer.Serialize(new
    {
        data = new
        {
            updateProjectV2Field = new
            {
                projectV2Field = new
                {
                    id = FixturesCreerProjet.IdChampEpique,
                    name = "Épique",
                    options = new[]
                    {
                        new { id = IdOptionEp02, name = LibelleEp02 },
                        new { id = IdOptionFondations, name = "Fondations" },
                        new { id = IdOptionEp88, name = LibelleEp88 },
                    },
                },
            },
        },
    });

    public static string ReponseIssuesOuvertes() => JsonSerializer.Serialize(new object[]
    {
        new { number = 101, title = TitreIssueTb100, url = "https://github.com/exemple-org/exemple-depot/issues/101", state = "OPEN" },
        new { number = 201, title = TitreIssueTb200, url = "https://github.com/exemple-org/exemple-depot/issues/201", state = "OPEN" },
    });

    public static string ReponseItemsExistants(string epique100, string epique200) => JsonSerializer.Serialize(new
    {
        items = new object[]
        {
            new Dictionary<string, object>
            {
                ["id"] = IdItem100,
                ["content"] = new { title = TitreIssueTb100 },
                ["statut"] = "Prêt",
                ["épique"] = epique100,
            },
            new Dictionary<string, object>
            {
                ["id"] = IdItem200,
                ["content"] = new { title = TitreIssueTb200 },
                ["statut"] = "Prêt",
                ["épique"] = epique200,
            },
        },
    });
}

/// <summary>
/// Point critique de la correction — un RENOMMAGE d'option, jamais une
/// recréation : les 52 affectations déjà posées sur des éléments du projet
/// pointent l'IDENTIFIANT d'une option, jamais son libellé. Deux passes sous
/// faux `gh` : la première migre (EP-02 renommé, identifiant préservé ;
/// Fondations — un reliquat non référencé par aucune tâche — repris tel
/// quel ; EP-88 créé, clé absente de tout état antérieur) ; la seconde,
/// contre un état distant déjà migré, ne doit plus émettre AUCUNE mutation
/// de champ — l'idempotence au niveau des OPTIONS elles-mêmes, distincte de
/// l'idempotence déjà couverte par <see cref="IdempotenceDesChampsDeProjetSousAppliquer"/>
/// (qui porte sur la VALEUR posée sur un élément, jamais sur le libellé de
/// l'option elle-même).
/// </summary>
public class RenommageEtCreationDesOptionsEpiqueSousAppliquer : IDisposable
{
    private readonly RepertoireTemporaire _tmpDepot = new();
    private readonly RepertoireTemporaire _tmpBin = new();
    private readonly string _cheminBin;

    public RenommageEtCreationDesOptionsEpiqueSousAppliquer()
    {
        FixturesRenommageEpique.EcrireDepotMinimal(_tmpDepot.Chemin);
        Helpers.CommiterTout(_tmpDepot.Chemin, "fixture renommage épique CreerProjet");
        Helpers.ConfigurerOrigineFictive(_tmpDepot.Chemin, FixturesCreerProjet.UrlOrigineFictive, pousser: false);
        _cheminBin = Helpers.RepertoireBinAvecFauxGh(_tmpBin.Chemin);
    }

    public void Dispose()
    {
        _tmpDepot.Dispose();
        _tmpBin.Dispose();
    }

    [Fact]
    public void DevraitRenommerEnPreservantLIdentifiantEtCreerPourUneCleNouvelle()
    {
        string journal = Path.Combine(_tmpBin.Chemin, "journal-passe-1.jsonl");
        var reponses = new[]
        {
            FixturesCreerProjet.ReponseProjetExistant(),          // 0 : project list
            FixturesRenommageEpique.ReponseChampsExistants(),      // 1 : field-list
            FixturesCreerProjet.ReponseGraphqlOptionsStatut(),     // 2 : graphql lecture Statut
            FixturesRenommageEpique.ReponseGraphqlEpiqueAvantMigration(), // 3 : graphql lecture Épique
            FixturesRenommageEpique.ReponseMutationMiseAJourChamp(),      // 4 : graphql mutation Épique
            FixturesRenommageEpique.ReponseIssuesOuvertes(),       // 5 : issue list
            FixturesCreerProjet.ReponseItemListeVide(),            // 6 : item-list (aucun élément posé)
            FixturesCreerProjet.ReponseItemAjoute(),                // 7 : item-add TB-100 (id générique dans la fixture partagée)
            "{}", "{}",                                             // 8-9 : item-edit épique, statut (TB-100)
            FixturesCreerProjet.ReponseItemAjoute(),                // 10 : item-add TB-200
            "{}", "{}",                                             // 11-12 : item-edit épique, statut (TB-200)
        };
        string cheminReponses = Path.Combine(_tmpBin.Chemin, $"reponses-{Guid.NewGuid():N}.json");
        Helpers.Ecrire(cheminReponses, JsonSerializer.Serialize(reponses));

        var environnement = new Dictionary<string, string>
        {
            ["PATH"] = _cheminBin,
            ["JOURNAL_GH"] = journal,
            ["FAUX_GH_REPONSES"] = cheminReponses,
        };
        var resultat = Helpers.ExecuterOutilAvecEnvironnement("CreerProjet", ["--appliquer"], _tmpDepot.Chemin, environnement);
        Assert.Equal(0, resultat.CodeSortie);

        var invocations = FixturesCreerProjet.LireJournalAvecEntreeStandard(journal);

        // Preuve par le JOURNAL D'APPELS de ce qui part réellement vers le
        // service — jamais par une valeur recalculée localement : les trois
        // appels GraphQL partagent le même vecteur d'arguments
        // (["api","graphql","--input","-"]), seule l'entrée standard les
        // distingue. La mutation est repérée par le nom de la mutation
        // GraphQL qu'elle porte dans sa charge, jamais par un rang fixé à la
        // main (fragile au moindre appel GraphQL ajouté ailleurs dans le flux).
        var mutations = invocations.Where(a => a.Stdin.Contains("updateProjectV2Field", StringComparison.Ordinal)).ToList();
        Assert.Single(mutations);

        using var charge = JsonDocument.Parse(mutations[0].Stdin);
        var optionsEnvoyees = charge.RootElement.GetProperty("variables").GetProperty("o").EnumerateArray().ToList();

        var optionEp02 = optionsEnvoyees.Single(o => o.GetProperty("name").GetString() == FixturesRenommageEpique.LibelleEp02);
        Assert.Equal(FixturesRenommageEpique.IdOptionEp02, optionEp02.GetProperty("id").GetString());

        var optionFondations = optionsEnvoyees.Single(o => o.GetProperty("name").GetString() == "Fondations");
        Assert.Equal(FixturesRenommageEpique.IdOptionFondations, optionFondations.GetProperty("id").GetString());

        var optionEp88 = optionsEnvoyees.Single(o => o.GetProperty("name").GetString() == FixturesRenommageEpique.LibelleEp88);
        Assert.False(optionEp88.TryGetProperty("id", out _), "une option neuve ne doit jamais réémettre d'identifiant.");

        Assert.Equal(3, optionsEnvoyees.Count);
    }

    [Fact]
    public void NeDevraitEmettreAucuneMutationDeChampQuandLesLibellesSontDejaAJour()
    {
        string journal = Path.Combine(_tmpBin.Chemin, "journal-passe-2.jsonl");
        var reponses = new[]
        {
            FixturesCreerProjet.ReponseProjetExistant(),
            FixturesRenommageEpique.ReponseChampsExistants(),
            FixturesCreerProjet.ReponseGraphqlOptionsStatut(),
            FixturesRenommageEpique.ReponseGraphqlEpiqueApresMigration(),
            FixturesRenommageEpique.ReponseIssuesOuvertes(),
            FixturesRenommageEpique.ReponseItemsExistants(
                epique100: FixturesRenommageEpique.LibelleEp02,
                epique200: FixturesRenommageEpique.LibelleEp88),
        };
        string cheminReponses = Path.Combine(_tmpBin.Chemin, $"reponses-{Guid.NewGuid():N}.json");
        Helpers.Ecrire(cheminReponses, JsonSerializer.Serialize(reponses));

        var environnement = new Dictionary<string, string>
        {
            ["PATH"] = _cheminBin,
            ["JOURNAL_GH"] = journal,
            ["FAUX_GH_REPONSES"] = cheminReponses,
        };
        var resultat = Helpers.ExecuterOutilAvecEnvironnement("CreerProjet", ["--appliquer"], _tmpDepot.Chemin, environnement);
        Assert.Equal(0, resultat.CodeSortie);

        var invocations = FixturesCreerProjet.LireJournalAvecEntreeStandard(journal);
        Assert.DoesNotContain(invocations, a => a.Stdin.Contains("updateProjectV2Field", StringComparison.Ordinal));
        Assert.DoesNotContain(invocations, a => a.Args.Contains("item-add") || a.Args.Contains("item-edit"));
        Assert.Equal(6, invocations.Count);
    }
}
