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

    // Titre de la section qui porte le libellé et le but de chaque épique
    // (plan-de-travail.md §4) — jamais une clause du corpus reprise en dur
    // ailleurs : le champ « Épique » d'une fiche de tâche (PlanDeTravail) ne
    // porte, lui, que la clé seule (ex. « EP-02 »), sans l'intitulé.
    private const string MarqueurVueDesEpiques = "## 4. Vue des épiques";

    // Sépare la clé de l'intitulé dans la première cellule d'une ligne de la
    // Vue des épiques (« EP-02 — Échafaudage de la solution .NET ») — même
    // séparateur (tiret cadratin entouré d'un espace de chaque côté) que
    // PlanDeTravail.MotifTitre pour le titre d'une fiche de tâche.
    private static readonly Regex MotifCleEtIntitule = new(@"^(.+?) — (.+)$", RegexOptions.CultureInvariant);

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

    // « color » et « description » lus en plus de « id »/« name » (au-delà du
    // seul besoin de lecture d'origine) : une mise à jour de champ liste
    // écrase l'INTÉGRALITÉ du tableau d'options (voir RequeteMettreAJourChampListe) ;
    // toute option réémise sans divergence voulue doit donc reprendre sa
    // couleur et sa description ACTUELLES, jamais une valeur inventée ici.
    private const string RequeteLireChampListe =
        "query($id:ID!){node(id:$id){\n" +
        "                ... on ProjectV2SingleSelectField{ id options{ id name color description } } } }";

    // Écrase l'intégralité des options d'un champ liste existant — jamais une
    // création d'option isolée, qui n'existe pas côté API (mesuré par
    // introspection du schéma : seules `createProjectV2Field`, pour un champ
    // neuf, et `updateProjectV2Field`, en réécriture complète, existent).
    // Toute option omise ici serait supprimée ; en conserver l'identifiant
    // (`id`) est ce qui la RENOMME au lieu de la recréer — le champ `id` de
    // `ProjectV2SingleSelectFieldOptionInput` porte, dans sa propre
    // description lue par introspection, exactement cette clause : « Include
    // this to preserve the option's identity during updates, preventing item
    // field values from being cleared. » Voir ReconcilierOptionsEpique.
    private const string RequeteMettreAJourChampListe =
        "\n        mutation($f:ID!,$o:[ProjectV2SingleSelectFieldOptionInput!]!){\n" +
        "          updateProjectV2Field(input:{fieldId:$f,singleSelectOptions:$o}){\n" +
        "            projectV2Field{ ... on ProjectV2SingleSelectField { id name options { id name } } } } }\n" +
        "    ";

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

        // Cible (libellé complet, but) de chaque épique référencée par au
        // moins une tâche — dérivée de plan-de-travail.md §4 quand cette
        // clé y est décrite ; repli sur la clé brute, sans but, sinon
        // (absence tolérée : voir LireLibellesEpiques). C'est cette cible,
        // jamais la clé seule, qui porte désormais le NOM posé sur l'option
        // du champ « Épique ».
        var libellesEpiques = LireLibellesEpiques(contenuPlan);
        var ciblesEpiques = epiques
            .Select(cle => libellesEpiques.TryGetValue(cle, out var trouve)
                ? new CibleOption(cle, trouve.Libelle, trouve.But)
                : new CibleOption(cle, cle, ""))
            .ToList();

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
            ? Appliquer(owner, repo, taches, etats, ciblesEpiques, sortie)
            : ApercuModeABlanc(owner, repo, taches, etats, ciblesEpiques, sortie);
    }

    /// <summary>
    /// Lit plan-de-travail.md §4 (« Vue des épiques ») et en tire, pour
    /// chaque clé d'épique qui y apparaît, son libellé complet (clé +
    /// intitulé, dans la forme exacte du plan) et son but (colonne voisine).
    /// Absence de la section tolérée — rend un dictionnaire vide plutôt que
    /// de lever : un plan de fixture minimal (tests de caractérisation) n'a
    /// jamais à porter cette section pour rester lisible par cet outil, et
    /// une clé alors non trouvée retombe sur elle-même comme libellé (voir
    /// l'appelant). Une clé répétée sous plusieurs jalons (ex. une épique
    /// « J0-J1 », présente à la fois sous J0 et sous J1) y est lue plusieurs
    /// fois ; la dernière occurrence lue l'emporte silencieusement — sans
    /// conséquence mesurée sur le corpus réel, où aucune clé ne varie
    /// d'intitulé ni de but d'une occurrence à l'autre.
    /// </summary>
    private static Dictionary<string, (string Libelle, string But)> LireLibellesEpiques(string contenuPlan)
    {
        var resultat = new Dictionary<string, (string, string)>(StringComparer.Ordinal);

        int debut = contenuPlan.IndexOf(MarqueurVueDesEpiques, StringComparison.Ordinal);
        if (debut == -1) return resultat;

        // Bornée par le prochain titre de section de premier niveau (« ## »),
        // jamais par la fin du document : une section suivante ne doit
        // jamais être lue comme si elle appartenait à la Vue des épiques.
        int finRecherche = debut + MarqueurVueDesEpiques.Length;
        int fin = contenuPlan.IndexOf("\n## ", finRecherche, StringComparison.Ordinal);
        string section = fin != -1 ? contenuPlan[debut..fin] : contenuPlan[debut..];

        // TableauMarkdown.AnalyserLignes ne rend que les deux premières
        // cellules de chaque ligne de tableau (ici : « clé — intitulé » et
        // « but ») — exactement ce qu'il faut, les colonnes Jalon/Dépend
        // de/Tranches restant hors de propos ici. Sa ligne d'en-tête
        // (« Épique | But | … ») n'y est pas filtrée par nom (elle ne
        // s'appelle ni « Champ » ni « Valeur ») mais ne porte jamais de tiret
        // cadratin : MotifCleEtIntitule ne s'y apparie jamais, elle est donc
        // écartée par construction, tout comme la ligne de séparation.
        foreach (var (premiere, seconde) in TableauMarkdown.AnalyserLignes(section))
        {
            var m = MotifCleEtIntitule.Match(premiere);
            if (!m.Success) continue;
            string cle = m.Groups[1].Value;
            resultat[cle] = ($"{cle} — {m.Groups[2].Value}", seconde);
        }
        return resultat;
    }

    private static int ApercuModeABlanc(
        string owner, string repo, List<Tache> taches, List<string> etats, List<CibleOption> ciblesEpiques, TextWriter sortie)
    {
        sortie.WriteLine("MODE À BLANC — rien n'est créé sur GitHub.");

        // Consultation en lecture seule de l'existant distant — jamais
        // GhCli.ExecuterJson : ces lectures tolèrent un `gh` en échec
        // (authentification absente, réseau, etc. — stdout vide, substitué),
        // à l'identique du script Python d'origine (creer-projet.py) pour la
        // lecture des issues plus bas. Seule l'absence TOTALE du binaire
        // `gh` sur PATH reste classée ENVIRONNEMENT : c'est
        // Processus.Executer qui lève avant que cette tolérance n'entre en
        // jeu (voir GhCli.InvoquerBrut). Sans cette consultation, cet aperçu
        // ne pourrait qu'annoncer la voie de création sans jamais savoir ce
        // qui sera réellement fait.
        JsonElement? projetExistant = LireProjetExistantTolerant(owner);
        if (projetExistant is { } projet)
        {
            sortie.WriteLine($"  = projet « {TitreProjet} » déjà présent sous {owner} : {projet.GetProperty("url").GetString()}");
        }
        else
        {
            sortie.WriteLine($"  + projet « {TitreProjet} » sous {owner} [à créer]");
            sortie.WriteLine($"  + lien du projet au dépôt {repo} [à créer]");
        }
        int? numeroProjet = projetExistant?.GetProperty("number").GetInt32();

        var champsExistants = numeroProjet.HasValue
            ? LireChampsExistantsTolerant(numeroProjet.Value, owner)
            : new Dictionary<string, JsonElement>(StringComparer.Ordinal);
        AnnoncerChamp(sortie, "Statut", etats.Count, champsExistants);
        AnnoncerChamp(sortie, "Épique", ciblesEpiques.Count, champsExistants);

        // Aperçu du renommage/de la création d'options du champ « Épique » —
        // jamais un appel GraphQL supplémentaire (ce mode ne doit jamais
        // perdre sa tolérance à un `gh` non authentifié) : les options déjà
        // posées sont ici relues depuis `champsExistants`, déjà obtenu par
        // LireChampsExistantsTolerant (`gh project field-list`, tolérant lui
        // aussi). Couleur/description n'y sont jamais nécessaires : cet
        // aperçu ne construit aucune charge d'écriture, seul le NOM de
        // chaque option compte pour la décision renommer/créer/rien.
        if (champsExistants.TryGetValue("Épique", out var champEpiqueExistant))
        {
            var optionsExistantes = champEpiqueExistant.GetProperty("options").EnumerateArray()
                .Select(o => new OptionExistante(o.GetProperty("id").GetString()!, o.GetProperty("name").GetString()!, "", ""))
                .ToList();
            var (_, renommees, creees) = ReconcilierOptionsEpique(optionsExistantes, ciblesEpiques);
            foreach (var (ancien, nouveau) in renommees)
            {
                sortie.WriteLine($"    ~ option « {ancien} » sera renommée en « {nouveau} »");
            }
            foreach (var creee in creees)
            {
                sortie.WriteLine($"    + option « {creee} » sera créée");
            }
        }

        string sortieBrute = GhCli.InvoquerBrut(
            "issue", "list", "--repo", $"{owner}/{repo}", "--limit", "300", "--state", "all", "--json", "number,title,state");
        using var document = JsonDocument.Parse(sortieBrute.Length > 0 ? sortieBrute : "[]");

        var presentes = new HashSet<string>(StringComparer.Ordinal);
        var etatsParTb = new Dictionary<string, string?>(StringComparer.Ordinal);
        foreach (var i in document.RootElement.EnumerateArray())
        {
            var m = MotifIdentifiantEnTete.Match(i.GetProperty("title").GetString() ?? "");
            if (!m.Success) continue;
            string tb = m.Groups[1].Value;
            presentes.Add(tb);
            etatsParTb[tb] = i.GetProperty("state").GetString() == "CLOSED" ? "Terminé" : null;
        }
        var auPlan = new HashSet<string>(taches.Select(t => t.Id), StringComparer.Ordinal);

        // Clé brute d'épique (telle que portée par le champ « Épique » d'une
        // fiche de tâche) → libellé complet posé comme NOM d'option — la
        // valeur qu'un élément déjà à jour porte réellement, une fois
        // l'option renommée (voir Appliquer, ReconcilierOptionsEpique).
        var libelleParCle = ciblesEpiques.ToDictionary(c => c.Cle, c => c.Nom, StringComparer.Ordinal);

        // Éléments déjà posés dans le projet — vide si le projet n'existe pas
        // encore (aucun élément ne peut y être posé sans lui). Même lecture
        // qu'Appliquer (ExtraireChampsItem, partagé) : seule la tolérance à
        // l'échec de `gh` diffère.
        var itemsExistants = numeroProjet.HasValue
            ? LireItemsExistantsTolerant(numeroProjet.Value, owner)
            : new Dictionary<string, ChampsItem>(StringComparer.Ordinal);

        int aAjouter = 0, dejaAJour = 0, aMettreAJour = 0;
        foreach (var tache in taches)
        {
            string tb = tache.Id;
            if (!presentes.Contains(tb)) continue;

            if (!itemsExistants.TryGetValue(tb, out var existant))
            {
                aAjouter++;
                continue;
            }

            string epiqueBrute = TexteUnicode.StripPython(tache.Champs.GetValueOrDefault("Épique", ""));
            string epiqueCible = epiqueBrute.Length > 0 ? libelleParCle.GetValueOrDefault(epiqueBrute, epiqueBrute) : "";
            bool epiqueDivergente = epiqueCible.Length > 0
                && !string.Equals(existant.Epique, epiqueCible, StringComparison.Ordinal);
            string statutCible = StatutCalcule(tache, etatsParTb);
            bool statutDivergent = !string.Equals(existant.Statut, statutCible, StringComparison.Ordinal);

            if (epiqueDivergente || statutDivergent) aMettreAJour++;
            else dejaAJour++;
        }
        int intersection = aAjouter + dejaAJour + aMettreAJour;

        sortie.WriteLine($"  {intersection} tranche(s) du plan ont déjà une issue, sur {auPlan.Count} au total.");
        sortie.WriteLine($"  + {aAjouter} élément(s) à ajouter au projet");
        sortie.WriteLine($"  = {dejaAJour} élément(s) déjà présent(s), champs déjà à jour");
        sortie.WriteLine($"  ~ {aMettreAJour} élément(s) déjà présent(s), au moins un champ (Statut/Épique) à mettre à jour");

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

    /// <summary>Recherche le projet par titre — pendant à blanc du bloc 1 d'<see cref="Appliquer"/>, sans jamais écrire.</summary>
    private static JsonElement? LireProjetExistantTolerant(string owner)
    {
        string sortieBrute = GhCli.InvoquerBrut("project", "list", "--owner", owner, "--limit", "100", "--format", "json");
        using var document = JsonDocument.Parse(sortieBrute.Length > 0 ? sortieBrute : "{}");
        if (!document.RootElement.TryGetProperty("projects", out var projets)) return null;
        foreach (var p in projets.EnumerateArray())
        {
            if (p.GetProperty("title").GetString() == TitreProjet) return p.Clone();
        }
        return null;
    }

    /// <summary>Liste les champs déjà posés sur le projet — pendant à blanc du bloc 2 d'<see cref="Appliquer"/>, sans jamais écrire.</summary>
    private static Dictionary<string, JsonElement> LireChampsExistantsTolerant(int numero, string owner)
    {
        var resultat = new Dictionary<string, JsonElement>(StringComparer.Ordinal);
        string sortieBrute = GhCli.InvoquerBrut("project", "field-list", numero.ToString(), "--owner", owner, "--format", "json");
        using var document = JsonDocument.Parse(sortieBrute.Length > 0 ? sortieBrute : "{}");
        if (!document.RootElement.TryGetProperty("fields", out var champs)) return resultat;
        foreach (var c in champs.EnumerateArray())
        {
            resultat[c.GetProperty("name").GetString()!] = c.Clone();
        }
        return resultat;
    }

    /// <summary>Liste les éléments déjà posés dans le projet, avec leurs champs actuels — pendant à blanc du bloc 3 d'<see cref="Appliquer"/>, sans jamais écrire.</summary>
    private static Dictionary<string, ChampsItem> LireItemsExistantsTolerant(int numero, string owner)
    {
        var resultat = new Dictionary<string, ChampsItem>(StringComparer.Ordinal);
        string sortieBrute = GhCli.InvoquerBrut(
            "project", "item-list", numero.ToString(), "--owner", owner, "--limit", "300", "--format", "json");
        using var document = JsonDocument.Parse(sortieBrute.Length > 0 ? sortieBrute : "{}");
        if (!document.RootElement.TryGetProperty("items", out var items)) return resultat;
        foreach (var it in items.EnumerateArray())
        {
            if (ExtraireChampsItem(it) is { } trouve) resultat[trouve.Tb] = trouve.Champs;
        }
        return resultat;
    }

    /// <summary>
    /// Annonce la voie « champ déjà présent » ou « à créer » — même formulation
    /// que les deux messages symétriques d'<see cref="Appliquer"/> (bloc 2),
    /// pour que l'aperçu et l'exécution réelle se lisent comme un seul et
    /// même contrat.
    /// </summary>
    private static void AnnoncerChamp(
        TextWriter sortie, string nom, int valeursPlan, IReadOnlyDictionary<string, JsonElement> champsExistants)
    {
        if (champsExistants.TryGetValue(nom, out var champ))
        {
            sortie.WriteLine($"  = champ « {nom} » déjà présent ({champ.GetProperty("options").GetArrayLength()} valeurs)");
        }
        else
        {
            sortie.WriteLine($"  + champ « {nom} » (liste, {valeursPlan} valeurs) [à créer]");
        }
    }

    private static int Appliquer(
        string owner, string repo, List<Tache> taches, List<string> etats, List<CibleOption> ciblesEpiques, TextWriter sortie)
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

        // reconcilierOptions distingue les deux champs : « Statut » reste en
        // lecture seule quand il existe déjà (comportement d'origine, jamais
        // analysé pour un renommage — les six états de methode-de-ticket.md
        // §5 ne sont jamais recomposés en clé + intitulé) ; seule « Épique »
        // emprunte le chemin de réconciliation (voir ReconcilierOptionsEpique).
        (string Id, Dictionary<string, string> Options) ChampListe(string nom, IReadOnlyList<CibleOption> cibles, bool reconcilierOptions)
        {
            if (champsExistants.TryGetValue(nom, out var champExistant))
            {
                var variables = new JsonObject { ["id"] = champExistant.GetProperty("id").GetString() };
                var data = Graphql.Executer(RequeteLireChampListe, variables).GetProperty("node");
                string idChamp = data.GetProperty("id").GetString()!;
                var optionsExistantes = data.GetProperty("options").EnumerateArray()
                    .Select(o => new OptionExistante(
                        o.GetProperty("id").GetString()!,
                        o.GetProperty("name").GetString()!,
                        o.GetProperty("color").GetString()!,
                        o.GetProperty("description").GetString() ?? ""))
                    .ToList();

                if (!reconcilierOptions)
                {
                    sortie.WriteLine($"  = champ « {nom} » déjà présent ({optionsExistantes.Count} valeurs)");
                    var optsLecture = new Dictionary<string, string>(StringComparer.Ordinal);
                    foreach (var o in optionsExistantes) optsLecture[o.Nom] = o.Id;
                    return (idChamp, optsLecture);
                }

                sortie.WriteLine($"  = champ « {nom} » déjà présent ({optionsExistantes.Count} valeurs)");
                var (finales, renommees, creees) = ReconcilierOptionsEpique(optionsExistantes, cibles);
                Dictionary<string, string> optsEcriture;
                if (renommees.Count == 0 && creees.Count == 0)
                {
                    // Idempotence : aucune divergence de libellé, aucune
                    // clé nouvelle — pas d'écriture, les options existantes
                    // font foi telles quelles.
                    optsEcriture = new Dictionary<string, string>(StringComparer.Ordinal);
                    foreach (var o in optionsExistantes) optsEcriture[o.Nom] = o.Id;
                }
                else
                {
                    optsEcriture = EcrireOptionsChamp(idChamp, finales);
                    foreach (var (ancien, nouveau) in renommees)
                    {
                        sortie.WriteLine($"    ~ option « {ancien} » renommée en « {nouveau} »");
                    }
                    foreach (var creee in creees)
                    {
                        sortie.WriteLine($"    + option « {creee} » créée");
                    }
                }
                return (idChamp, optsEcriture);
            }
            var (idCree, optsCreees) = CreerChampListe(idProjet, nom, cibles);
            sortie.WriteLine($"  + champ « {nom} » ({cibles.Count} valeurs)");
            return (idCree, optsCreees);
        }

        var ciblesStatut = etats.Select(e => new CibleOption(e, e, "")).ToList();
        var (idStatut, optsStatut) = ChampListe("Statut", ciblesStatut, reconcilierOptions: false);
        var (idEpique, optsEpique) = ChampListe("Épique", ciblesEpiques, reconcilierOptions: true);

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
        // champ (ExtraireChampsItem, partagé avec l'aperçu à blanc). Le
        // commentaire du script Python à ce même endroit évoquait un
        // appariement par SUFFIXE sur la clé « épique » à cause d'un octet
        // abîmé renvoyé par gh — aucun code de la fonction Python ne le
        // faisait alors (voir le rapport de portage, Discoveries) ; ce
        // suffixe existe désormais réellement, mais pour une autre raison :
        // lire la valeur ACTUELLE des champs (ValeurChampActuelle), un besoin
        // que la fonction Python d'origine n'avait jamais eu.
        var itemsExistants = new Dictionary<string, ChampsItem>(StringComparer.Ordinal);
        foreach (var it in GhCli.ExecuterJson("project", "item-list", numero.ToString(), "--owner", owner,
                     "--limit", "300", "--format", "json").GetProperty("items").EnumerateArray())
        {
            if (ExtraireChampsItem(it) is { } trouveItem) itemsExistants[trouveItem.Tb] = trouveItem.Champs;
        }

        // Clé brute d'épique → libellé complet posé comme NOM d'option —
        // même correspondance que ApercuModeABlanc (voir libelleParCle
        // là-bas) : c'est ce libellé, jamais la clé seule, que porte
        // désormais optsEpique (Nom → Id des options telles qu'écrites
        // ci-dessus).
        var libelleParCle = ciblesEpiques.ToDictionary(c => c.Cle, c => c.Nom, StringComparer.Ordinal);

        int ajoutes = 0, reutilises = 0;
        foreach (var tache in taches)
        {
            string tb = tache.Id;
            if (!parTb.ContainsKey(tb)) continue;

            string idItem;
            ChampsItem? existant = itemsExistants.TryGetValue(tb, out var trouveExistant) ? trouveExistant : null;
            if (existant is { } e)
            {
                idItem = e.Id;
                reutilises++;
            }
            else
            {
                idItem = GhCli.ExecuterJson("project", "item-add", numero.ToString(), "--owner", owner,
                    "--url", parTb[tb].GetProperty("url").GetString()!, "--format", "json").GetProperty("id").GetString()!;
                ajoutes++;
            }

            // Idempotence : un champ n'est réécrit que si sa valeur cible
            // diverge de la valeur actuellement posée sur l'élément — un
            // élément tout juste ajouté (existant == null) n'a par
            // construction aucune valeur actuelle, donc diverge toujours.
            string epiqueBrute = TexteUnicode.StripPython(tache.Champs.GetValueOrDefault("Épique", ""));
            string epique = epiqueBrute.Length > 0 ? libelleParCle.GetValueOrDefault(epiqueBrute, epiqueBrute) : "";
            if (optsEpique.TryGetValue(epique, out var idOptionEpique)
                && !string.Equals(existant?.Epique, epique, StringComparison.Ordinal))
            {
                GhCli.ExecuterTexte("project", "item-edit", "--id", idItem, "--project-id", idProjet,
                    "--field-id", idEpique, "--single-select-option-id", idOptionEpique);
            }

            string statut = StatutCalcule(tache, etatsParTb);
            if (!string.Equals(existant?.Statut, statut, StringComparison.Ordinal))
            {
                GhCli.ExecuterTexte("project", "item-edit", "--id", idItem, "--project-id", idProjet,
                    "--field-id", idStatut, "--single-select-option-id", optsStatut[statut]);
            }

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

    /// <summary>
    /// Élément de projet déjà posé, tel que retrouvé sur
    /// <c>project item-list</c> : son identifiant (pour être réutilisé sans
    /// nouvel appel <c>item-add</c>) et la valeur ACTUELLE de ses deux champs
    /// liste — <c>null</c> quand le champ n'a encore jamais été renseigné sur
    /// cet élément. Sert la décision d'idempotence : un champ n'est réécrit
    /// que si sa valeur cible diverge de cette valeur actuelle.
    /// </summary>
    private readonly record struct ChampsItem(string Id, string? Statut, string? Epique);

    /// <summary>
    /// Lit la valeur actuelle d'un champ liste posé sur un élément de projet.
    /// <c>gh project item-list --format json</c> projette le NOM de l'option
    /// choisie (jamais son identifiant) sous une clé en camelCase du nom du
    /// champ — absente de l'objet tant qu'aucune valeur n'a été posée.
    /// Appariée par SUFFIXE ASCII (ex. <c>"pique"</c> pour « Épique »)
    /// plutôt que par une clé accentuée écrite en dur : mesuré, cette clé
    /// arrive parfois mal encodée sur la sortie JSON de `gh` — seul le
    /// caractère accentué en tête en est affecté, jamais la queue ASCII du
    /// nom du champ.
    /// </summary>
    private static string? ValeurChampActuelle(JsonElement item, string suffixeAscii)
    {
        foreach (var propriete in item.EnumerateObject())
        {
            if (propriete.Value.ValueKind == JsonValueKind.String
                && propriete.Name.EndsWith(suffixeAscii, StringComparison.OrdinalIgnoreCase))
            {
                return propriete.Value.GetString();
            }
        }
        return null;
    }

    /// <summary>
    /// Apparie un élément de <c>project item-list</c> à sa tranche de plan —
    /// par l'identifiant en tête du titre de l'issue liée, jamais par un
    /// autre champ (même convention que l'ancien appariement d'<c>Appliquer</c>,
    /// désormais partagée avec l'aperçu à blanc) — et en extrait au passage
    /// les valeurs actuelles de ses deux champs liste. <c>null</c> si le
    /// titre ne porte aucun identifiant reconnu.
    /// </summary>
    private static (string Tb, ChampsItem Champs)? ExtraireChampsItem(JsonElement it)
    {
        var m = MotifIdentifiantEnTete.Match(TitreDuContenu(it));
        if (!m.Success) return null;
        return (m.Groups[1].Value, new ChampsItem(
            it.GetProperty("id").GetString()!,
            ValeurChampActuelle(it, "statut"),
            ValeurChampActuelle(it, "pique")));
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
        string idProjet, string nom, IReadOnlyList<CibleOption> cibles)
    {
        var options = new JsonArray();
        for (int i = 0; i < cibles.Count; i++)
        {
            options.Add(new JsonObject
            {
                ["name"] = cibles[i].Nom,
                ["color"] = Couleurs[i % Couleurs.Length],
                ["description"] = cibles[i].Description,
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

    // La description d'une option n'est jamais tronquée ici : l'introspection
    // du schéma GraphQL (champ `description` de
    // `ProjectV2SingleSelectFieldOptionInput`) ne documente aucune longueur
    // maximale — à la différence de son champ `id`, dont la description
    // porte, elle, une clause explicite (voir RequeteMettreAJourChampListe).
    // Vérifier une limite RÉELLE exigerait une mutation contre un projet
    // réel, hors de portée de cette correction (voir le rapport de portage).
    /// <summary>Cible d'option dérivée du plan pour une clé d'épique : son libellé complet (« clé — intitulé », ou la clé seule à défaut d'intitulé connu — voir <see cref="LireLibellesEpiques"/>) et sa description (le but de l'épique, vide à défaut).</summary>
    private readonly record struct CibleOption(string Cle, string Nom, string Description);

    /// <summary>Option de champ liste telle que lue à distance (GraphQL, <see cref="RequeteLireChampListe"/>), avec tout le nécessaire à un réemploi fidèle lors d'une réécriture complète du tableau d'options.</summary>
    private readonly record struct OptionExistante(string Id, string Nom, string Couleur, string Description);

    /// <summary>Option à envoyer dans la charge d'une mutation de champ liste — <c>Id</c> nul pour une option neuve (jamais réémis, l'API lui en attribue un), renseigné pour réémettre une option existante et en préserver l'identité.</summary>
    private readonly record struct OptionAEnvoyer(string? Id, string Nom, string Couleur, string Description);

    /// <summary>
    /// Réconcilie les options actuellement posées sur le champ « Épique »
    /// avec les cibles dérivées du plan — un RENOMMAGE, jamais une
    /// recréation d'option : toute option déjà posée qui s'apparie à une clé
    /// courante REND SON IDENTIFIANT dans <see cref="OptionAEnvoyer.Id"/>,
    /// pour que les affectations déjà posées sur des éléments du projet ne
    /// se perdent jamais (52 éléments en portent une au moment de cette
    /// correction). L'appariement se fait par le NOM actuel de l'option,
    /// contre soit la clé brute (forme historique, avant toute migration,
    /// ex. « EP-02 »), soit le libellé complet déjà migré (ex.
    /// « EP-02 — Échafaudage de la solution .NET ») — pour qu'une exécution
    /// répétée après une première migration reconnaisse ses propres options
    /// déjà renommées et n'émette alors plus aucune divergence.
    ///
    /// Une option non appariée à aucune clé courante — un reliquat, un cas
    /// que la mission n'a jamais nommé — est reprise TELLE QUELLE, jamais
    /// supprimée ni renommée : solution la plus conservative faute d'un sens
    /// arbitré pour ce cas, absent du corpus mesuré au moment de cette
    /// correction. Une clé sans option appariée reçoit une option neuve,
    /// ajoutée en fin de liste.
    ///
    /// La divergence qui déclenche un renommage porte sur le NOM seul —
    /// jamais sur la description d'une option dont le nom est déjà à jour :
    /// c'est la lecture littérale retenue pour l'idempotence attendue
    /// (« si les libellés sont déjà à jour, aucune écriture ») ; une
    /// description qui divergerait seule, sans divergence de nom, ne
    /// déclenche donc aucune écriture — cas absent du corpus mesuré (la
    /// description n'est posée qu'au moment même où le nom l'est).
    /// </summary>
    private static (List<OptionAEnvoyer> Finales, List<(string Ancien, string Nouveau)> Renommees, List<string> Creees)
        ReconcilierOptionsEpique(IReadOnlyList<OptionExistante> existantes, IReadOnlyList<CibleOption> cibles)
    {
        var finales = new List<OptionAEnvoyer>();
        var renommees = new List<(string, string)>();
        var appariees = new HashSet<int>();

        foreach (var existante in existantes)
        {
            int index = -1;
            for (int i = 0; i < cibles.Count; i++)
            {
                if (appariees.Contains(i)) continue;
                if (string.Equals(existante.Nom, cibles[i].Nom, StringComparison.Ordinal)
                    || string.Equals(existante.Nom, cibles[i].Cle, StringComparison.Ordinal))
                {
                    index = i;
                    break;
                }
            }

            if (index == -1)
            {
                finales.Add(new OptionAEnvoyer(existante.Id, existante.Nom, existante.Couleur, existante.Description));
                continue;
            }

            appariees.Add(index);
            var cible = cibles[index];
            if (string.Equals(existante.Nom, cible.Nom, StringComparison.Ordinal))
            {
                finales.Add(new OptionAEnvoyer(existante.Id, existante.Nom, existante.Couleur, existante.Description));
            }
            else
            {
                renommees.Add((existante.Nom, cible.Nom));
                finales.Add(new OptionAEnvoyer(existante.Id, cible.Nom, existante.Couleur, cible.Description));
            }
        }

        var creees = new List<string>();
        for (int i = 0; i < cibles.Count; i++)
        {
            if (appariees.Contains(i)) continue;
            var cible = cibles[i];
            finales.Add(new OptionAEnvoyer(null, cible.Nom, Couleurs[creees.Count % Couleurs.Length], cible.Description));
            creees.Add(cible.Nom);
        }

        return (finales, renommees, creees);
    }

    /// <summary>
    /// Réécrit l'intégralité des options d'un champ liste existant
    /// (<see cref="RequeteMettreAJourChampListe"/>) et rend les identifiants
    /// (nouveaux ou préservés) de chaque option telle que l'API la rend après
    /// écriture — jamais recalculés localement : c'est la seule source fiable
    /// de l'identifiant d'une option tout juste créée.
    /// </summary>
    private static Dictionary<string, string> EcrireOptionsChamp(string idChamp, IReadOnlyList<OptionAEnvoyer> options)
    {
        var tableauOptions = new JsonArray();
        foreach (var option in options)
        {
            var noeud = new JsonObject
            {
                ["name"] = option.Nom,
                ["color"] = option.Couleur,
                ["description"] = option.Description,
            };
            if (option.Id is not null) noeud["id"] = option.Id;
            tableauOptions.Add(noeud);
        }

        var variables = new JsonObject { ["f"] = idChamp, ["o"] = tableauOptions };
        var champ = Graphql.Executer(RequeteMettreAJourChampListe, variables)
            .GetProperty("updateProjectV2Field").GetProperty("projectV2Field");

        var opts = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var o in champ.GetProperty("options").EnumerateArray())
        {
            opts[o.GetProperty("name").GetString()!] = o.GetProperty("id").GetString()!;
        }
        return opts;
    }
}
