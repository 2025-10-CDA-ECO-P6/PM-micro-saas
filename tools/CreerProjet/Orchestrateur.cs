using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using CreerIssues;

namespace CreerProjet;

/// <summary>
/// Corps du programme : lecture du plan et de la méthode, aperçu en mode à
/// blanc, ou report réel vers un projet GitHub sous <c>--appliquer</c>.
/// Reproduit <c>main()</c>/<c>appliquer()</c> du script Python d'origine
/// (<c>creer-projet.py</c>), y compris leur ordre d'impression exact (critère
/// de différentiel de sortie standard, sans aucun filtrage).
/// </summary>
internal static class Orchestrateur
{
    // Le titre porte le dépôt et le produit : le dépôt s'appelle
    // PM-micro-saas, le corpus nomme le produit Haversack. C'est le titre
    // réellement porté par le projet sur GitHub — le modifier ici sans le
    // modifier là-bas ferait créer un doublon à la prochaine exécution.
    public const string TitreProjet = "PM-micro-saas - Haversack";

    private static readonly string[] Couleurs =
        { "GRAY", "BLUE", "GREEN", "YELLOW", "ORANGE", "RED", "PURPLE", "PINK" };

    // re.match ancre implicitement en tête de chaîne côté Python ; Match de
    // .NET cherche — ancrage explicite, même convention que le reste de cet
    // outillage C#. Trois chiffres exacts : la forme littérale des trois
    // occurrences de `r"(TB-\d{3})"` dans le script Python d'origine
    // (creer-projet.py), jamais `\d+`.
    private static readonly Regex MotifIdentifiantEnTete = new(@"^(TB-\d{3})", RegexOptions.CultureInvariant);

    // re.findall n'ancre jamais : cherche n'importe où dans le texte.
    private static readonly Regex MotifIdentifiantDansTexte = new(@"TB-\d{3}", RegexOptions.CultureInvariant);

    private static readonly HashSet<string> EtatsTerminaux = new(StringComparer.Ordinal) { "Terminé" };
    private static readonly HashSet<string> EtatsEnCours = new(StringComparer.Ordinal) { "Pris", "En cours" };

    // Requêtes GraphQL — reproduites caractère pour caractère : le texte de
    // la requête est une valeur de CHAÎNE au sein de la charge JSON envoyée à
    // `gh`, jamais normalisée par le comparateur de journaux (voir
    // Graphql.cs) — un espacement différent y resterait une divergence.
    private const string RequeteCreerChampListe =
        "\n        mutation($p:ID!,$n:String!,$o:[ProjectV2SingleSelectFieldOptionInput!]!){\n" +
        "          createProjectV2Field(input:{projectId:$p,dataType:SINGLE_SELECT,name:$n,singleSelectOptions:$o}){\n" +
        "            projectV2Field{ ... on ProjectV2SingleSelectField { id name options { id name } } } } }\n" +
        "    ";

    private const string RequeteLireChampListe =
        "query($id:ID!){node(id:$id){\n" +
        "                ... on ProjectV2SingleSelectField{ id options{ id name } } } }";

    public static int Executer(bool appliquer, TextWriter sortie)
    {
        string racine = RacineDepot.Trouver();
        var (owner, repo) = OrigineGit.DeduireOwnerRepo(OrigineGit.LireUrl());
        string contenuPlan = LecteurTexte.LireTexteIntegral(Path.Combine(racine, Chemins.PlanRelatif));
        var taches = PlanDeTravail.ExtraireTaches(contenuPlan);
        var etats = EtatsCycleDeVie.Lire(racine);

        var epiquesBrutes = new HashSet<string>(StringComparer.Ordinal);
        foreach (var t in taches)
        {
            string valeur = TexteUnicode.StripPython(t.Champs.GetValueOrDefault("Épique", ""));
            if (valeur.Length > 0) epiquesBrutes.Add(valeur);
        }
        var epiques = epiquesBrutes.ToList();
        epiques.Sort(ComparateurCodePoints.Instance);

        sortie.WriteLine($"Dépôt          : {owner}/{repo}");
        sortie.WriteLine($"Projet         : {TitreProjet}");
        sortie.WriteLine($"Tranches lues  : {taches.Count}");
        sortie.WriteLine($"États lus dans methode-de-ticket §5 ({etats.Count}) : {string.Join(" → ", etats)}");
        sortie.WriteLine($"Épiques lues dans le plan ({epiques.Count}) : {string.Join(", ", epiques)}");
        sortie.WriteLine();
        sortie.WriteLine("Champs qui NE seront PAS créés, et pourquoi : taille, dépendances,");
        sortie.WriteLine("périmètre d'écriture, critères d'acceptation — methode-de-ticket §1 les");
        sortie.WriteLine("nomme comme champs qu'un ticket ne recopie pas.");
        sortie.WriteLine();

        return appliquer
            ? Appliquer(owner, repo, taches, etats, epiques, sortie)
            : ApercuModeABlanc(owner, repo, taches, etats, epiques, sortie);
    }

    private static int ApercuModeABlanc(
        string owner, string repo, List<Tache> taches, List<string> etats, List<string> epiques, TextWriter sortie)
    {
        sortie.WriteLine("MODE À BLANC — rien n'est créé sur GitHub.");
        sortie.WriteLine($"  + projet « {TitreProjet} » sous {owner}");
        sortie.WriteLine($"  + lien du projet au dépôt {repo}");
        sortie.WriteLine($"  + champ « Statut » (liste, {etats.Count} valeurs)");
        sortie.WriteLine($"  + champ « Épique » (liste, {epiques.Count} valeurs)");

        // subprocess.run(...).stdout or "[]" — jamais gh() : cette lecture
        // tolère un `gh` en échec (authentification absente, réseau, etc. —
        // stdout vide, substitué), à l'identique du script Python d'origine
        // (creer-projet.py). Seule
        // l'absence TOTALE du binaire `gh` sur PATH reste classée
        // ENVIRONNEMENT : c'est Processus.Executer qui lève avant que cette
        // tolérance n'entre en jeu (voir GhCli.InvoquerBrut).
        string sortieBrute = GhCli.InvoquerBrut(
            "issue", "list", "--repo", $"{owner}/{repo}", "--limit", "300", "--state", "all", "--json", "number,title");
        using var document = JsonDocument.Parse(sortieBrute.Length > 0 ? sortieBrute : "[]");

        var presentes = new HashSet<string>(StringComparer.Ordinal);
        foreach (var i in document.RootElement.EnumerateArray())
        {
            var m = MotifIdentifiantEnTete.Match(i.GetProperty("title").GetString() ?? "");
            if (m.Success) presentes.Add(m.Groups[1].Value);
        }
        var auPlan = new HashSet<string>(taches.Select(t => t.Id), StringComparer.Ordinal);

        int intersection = presentes.Count(auPlan.Contains);
        sortie.WriteLine($"  + {intersection} élément(s) ajouté(s) — une issue existe "
            + $"pour {intersection} des {auPlan.Count} tranches du plan");

        var sansTranche = presentes.Where(p => !auPlan.Contains(p)).ToList();
        if (sansTranche.Count > 0)
        {
            sansTranche.Sort(ComparateurCodePoints.Instance);
            sortie.WriteLine($"  ! issues sans tranche correspondante : {ReprPython.Liste(sansTranche)}");
        }

        var manquantes = auPlan.Where(t => !presentes.Contains(t)).ToList();
        manquantes.Sort(ComparateurCodePoints.Instance);
        if (manquantes.Count > 0)
        {
            sortie.WriteLine($"  = {manquantes.Count} tranche(s) sans issue : elles n'entreront pas "
                + "dans le projet tant que leur issue n'est pas ouverte");
        }
        return 0;
    }

    private static int Appliquer(
        string owner, string repo, List<Tache> taches, List<string> etats, List<string> epiques, TextWriter sortie)
    {
        // Python n'a, pour --appliquer, aucune garde explicite d'exécution :
        // `gh` absent produit une trace Python non interceptée (1 par
        // défaut, Python n'ayant pas de classification ENVIRONNEMENT/CORPUS)
        // ; `gh` non authentifié fait échouer le premier appel `gh()`, classé
        // CORPUS (1) comme tout autre échec de `gh`. Le contrat arbitré de ce
        // portage classe « gh absent ou non authentifié avec --appliquer »
        // ENVIRONNEMENT (2) — divergence délibérée, nommée dans le rapport de
        // portage. `gh` absent reste géré SANS code ici : c'est
        // Processus.Executer qui lève PanneEnvironnementException avant même
        // le premier appel ci-dessous. Seule la reclassification « gh non
        // authentifié » a besoin d'un geste explicite — et seulement en cas
        // d'ÉCHEC du premier appel : une garde inconditionnelle en tête
        // ajouterait un appel `gh auth status` que le script Python d'origine
        // (creer-projet.py) ne faisait jamais, décalant tout le journal d'un
        // cran sur le chemin nominal.
        JsonElement existants;
        try
        {
            // 1. le projet — réutilisé s'il existe déjà sous ce titre, jamais dupliqué.
            existants = GhCli.ExecuterJson("project", "list", "--owner", owner, "--limit", "100", "--format", "json")
                .GetProperty("projects");
        }
        catch (ErreurCorpusException) when (!GhCli.Authentifie())
        {
            throw new PanneEnvironnementException(
                "gh n'est pas authentifié. Lancez « gh auth login » puis relancez ce script.");
        }

        JsonElement? trouve = null;
        foreach (var p in existants.EnumerateArray())
        {
            if (p.GetProperty("title").GetString() == TitreProjet) { trouve = p; break; }
        }

        JsonElement projet;
        if (trouve is null)
        {
            projet = GhCli.ExecuterJson("project", "create", "--owner", owner, "--title", TitreProjet, "--format", "json");
            sortie.WriteLine($"  + projet #{projet.GetProperty("number").GetInt32()} créé : {projet.GetProperty("url").GetString()}");
            GhCli.ExecuterTexte("project", "link", projet.GetProperty("number").GetInt32().ToString(),
                "--owner", owner, "--repo", $"{owner}/{repo}");
            sortie.WriteLine($"  + projet lié au dépôt {repo}");
        }
        else
        {
            projet = trouve.Value;
            sortie.WriteLine($"  = projet #{projet.GetProperty("number").GetInt32()} déjà présent, réutilisé : {projet.GetProperty("url").GetString()}");
        }

        int numero = projet.GetProperty("number").GetInt32();
        string idProjet = projet.GetProperty("id").GetString()!;
        string url = projet.GetProperty("url").GetString()!;

        // 2. les champs — noms et valeurs lus du corpus, réutilisés s'ils existent.
        var champsExistants = new Dictionary<string, JsonElement>(StringComparer.Ordinal);
        foreach (var c in GhCli.ExecuterJson("project", "field-list", numero.ToString(), "--owner", owner, "--format", "json")
                     .GetProperty("fields").EnumerateArray())
        {
            champsExistants[c.GetProperty("name").GetString()!] = c;
        }

        (string Id, Dictionary<string, string> Options) ChampListe(string nom, IReadOnlyList<string> valeurs)
        {
            if (champsExistants.TryGetValue(nom, out var champExistant))
            {
                var variables = new JsonObject { ["id"] = champExistant.GetProperty("id").GetString() };
                var data = Graphql.Executer(RequeteLireChampListe, variables).GetProperty("node");
                var optionsExistantes = data.GetProperty("options");
                sortie.WriteLine($"  = champ « {nom} » déjà présent ({optionsExistantes.GetArrayLength()} valeurs)");
                var opts = new Dictionary<string, string>(StringComparer.Ordinal);
                foreach (var o in optionsExistantes.EnumerateArray())
                {
                    opts[o.GetProperty("name").GetString()!] = o.GetProperty("id").GetString()!;
                }
                return (data.GetProperty("id").GetString()!, opts);
            }
            var (idCree, optsCreees) = CreerChampListe(idProjet, nom, valeurs);
            sortie.WriteLine($"  + champ « {nom} » ({valeurs.Count} valeurs)");
            return (idCree, optsCreees);
        }

        var (idStatut, optsStatut) = ChampListe("Statut", etats);
        var (idEpique, optsEpique) = ChampListe("Épique", epiques);

        // 3. les issues déjà ouvertes. Python n'adjuge pas le code de sortie
        // ICI (pas de `or "[]"` non plus, à la différence du même appel en
        // mode à blanc) : un `gh` en échec à ce point précis produirait,
        // côté Python, un `json.loads("")` non intercepté — reproduit tel
        // quel, sans tolérance ajoutée, plutôt que « réparé » en silence
        // (voir le rapport de portage, code de sortie).
        string sortieIssues = GhCli.InvoquerBrut(
            "issue", "list", "--repo", $"{owner}/{repo}", "--limit", "300",
            "--state", "all", "--json", "number,title,url,state");
        using var documentIssues = JsonDocument.Parse(sortieIssues);

        var parTb = new Dictionary<string, JsonElement>(StringComparer.Ordinal);
        foreach (var i in documentIssues.RootElement.EnumerateArray())
        {
            var m = MotifIdentifiantEnTete.Match(i.GetProperty("title").GetString() ?? "");
            if (m.Success) parTb[m.Groups[1].Value] = i;
        }
        sortie.WriteLine($"  = {parTb.Count} issue(s) trouvée(s) sur le dépôt, sur {taches.Count} tranches au plan");

        var etatsParTb = new Dictionary<string, string?>(StringComparer.Ordinal);
        foreach (var (tb, i) in parTb)
        {
            etatsParTb[tb] = i.GetProperty("state").GetString() == "CLOSED" ? "Terminé" : null;
        }

        // Éléments déjà présents dans le projet — appariés par l'identifiant
        // de tâche en tête du titre de l'issue liée, jamais par un autre
        // champ. Le commentaire du script Python à ce même endroit évoque un
        // appariement par SUFFIXE sur la clé « épique » à cause d'un octet
        // abîmé renvoyé par gh — aucun code de la fonction Python ne fait
        // cela : c'est un commentaire du corpus qui périme sans plus
        // correspondre à ce que la fonction fait réellement (voir le
        // rapport de portage, Discoveries). Ce portage reproduit le CODE,
        // pas le commentaire.
        var deja = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var it in GhCli.ExecuterJson("project", "item-list", numero.ToString(), "--owner", owner,
                     "--limit", "300", "--format", "json").GetProperty("items").EnumerateArray())
        {
            var m = MotifIdentifiantEnTete.Match(TitreDuContenu(it));
            if (m.Success) deja[m.Groups[1].Value] = it.GetProperty("id").GetString()!;
        }

        int ajoutes = 0, reutilises = 0;
        foreach (var tache in taches)
        {
            string tb = tache.Id;
            if (!parTb.ContainsKey(tb)) continue;

            string idItem;
            if (deja.TryGetValue(tb, out var idExistant))
            {
                idItem = idExistant;
                reutilises++;
            }
            else
            {
                idItem = GhCli.ExecuterJson("project", "item-add", numero.ToString(), "--owner", owner,
                    "--url", parTb[tb].GetProperty("url").GetString()!, "--format", "json").GetProperty("id").GetString()!;
                ajoutes++;
            }

            string epique = TexteUnicode.StripPython(tache.Champs.GetValueOrDefault("Épique", ""));
            if (optsEpique.TryGetValue(epique, out var idOptionEpique))
            {
                GhCli.ExecuterTexte("project", "item-edit", "--id", idItem, "--project-id", idProjet,
                    "--field-id", idEpique, "--single-select-option-id", idOptionEpique);
            }

            string statut = StatutCalcule(tache, etatsParTb);
            GhCli.ExecuterTexte("project", "item-edit", "--id", idItem, "--project-id", idProjet,
                "--field-id", idStatut, "--single-select-option-id", optsStatut[statut]);

            sortie.WriteLine($"    · {tb} — épique {(epique.Length > 0 ? epique : "—")}, statut « {statut} »");
        }

        sortie.WriteLine($"\n  {ajoutes} élément(s) ajouté(s), {reutilises} déjà présent(s) — tous renseignés.");
        sortie.WriteLine($"  Projet : {url}");
        sortie.WriteLine("\n  Les VUES ne sont pas créées : « gh project » n'a aucune sous-commande");
        sortie.WriteLine("  pour cela. Recette dans l'interface, trois clics chacune — voir le");
        sortie.WriteLine("  compte rendu de fin d'exécution.");
        return 0;
    }

    /// <summary>
    /// Équivalent de <c>(it.get("content") or {}).get("title", "")</c> :
    /// une clé absente, une valeur nulle ou un objet sans <c>title</c>
    /// rendent tous la chaîne vide.
    /// </summary>
    private static string TitreDuContenu(JsonElement item)
    {
        if (item.TryGetProperty("content", out var contenu)
            && contenu.ValueKind == JsonValueKind.Object
            && contenu.TryGetProperty("title", out var titre)
            && titre.ValueKind == JsonValueKind.String)
        {
            return titre.GetString() ?? "";
        }
        return "";
    }

    private static bool EtatDansEnsemble(string? etat, HashSet<string> ensemble) => etat is not null && ensemble.Contains(etat);

    /// <summary>
    /// Applique les conditions de methode-de-ticket §2. Renvoie « Prêt » ou
    /// « Ouvert, non prêt » — jamais une estimation.
    /// </summary>
    private static string StatutCalcule(Tache tache, IReadOnlyDictionary<string, string?> etatsParTb)
    {
        string perimetre = tache.Champs.GetValueOrDefault("Périmètre d'écriture", "");
        string criteres = tache.Champs.GetValueOrDefault("Critères d'acceptation", "");
        if (perimetre.StartsWith("TROU", StringComparison.Ordinal)) return "Ouvert, non prêt";
        if (criteres.Contains("TROU", StringComparison.Ordinal)) return "Ouvert, non prêt";

        var deps = MotifIdentifiantDansTexte.Matches(tache.Champs.GetValueOrDefault("Dépend de", "")).Select(m => m.Value);
        if (deps.Any(d => !EtatDansEnsemble(etatsParTb.GetValueOrDefault(d), EtatsTerminaux)))
        {
            return "Ouvert, non prêt";
        }

        var conflits = MotifIdentifiantDansTexte.Matches(tache.Champs.GetValueOrDefault("En conflit avec", "")).Select(m => m.Value);
        if (conflits.Any(c => EtatDansEnsemble(etatsParTb.GetValueOrDefault(c), EtatsEnCours)))
        {
            return "Ouvert, non prêt";
        }
        return "Prêt";
    }

    private static (string Id, Dictionary<string, string> Options) CreerChampListe(
        string idProjet, string nom, IReadOnlyList<string> valeurs)
    {
        var options = new JsonArray();
        for (int i = 0; i < valeurs.Count; i++)
        {
            options.Add(new JsonObject
            {
                ["name"] = valeurs[i],
                ["color"] = Couleurs[i % Couleurs.Length],
                ["description"] = "",
            });
        }

        var variables = new JsonObject { ["p"] = idProjet, ["n"] = nom, ["o"] = options };
        var champ = Graphql.Executer(RequeteCreerChampListe, variables)
            .GetProperty("createProjectV2Field").GetProperty("projectV2Field");

        var opts = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var o in champ.GetProperty("options").EnumerateArray())
        {
            opts[o.GetProperty("name").GetString()!] = o.GetProperty("id").GetString()!;
        }
        return (champ.GetProperty("id").GetString()!, opts);
    }
}
