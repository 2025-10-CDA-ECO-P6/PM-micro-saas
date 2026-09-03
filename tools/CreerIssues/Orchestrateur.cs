using System.Text.RegularExpressions;

namespace CreerIssues;

/// <summary>
/// Corps du programme : quatre opérations dans l'ordre — libellés, jalons,
/// une issue par tâche, puis un compte rendu. Reproduit `executer_operations()`
/// du script Python d'origine (`creer-issues.py`), y compris son ordre
/// d'impression exact (critère de différentiel de sortie standard, sans
/// aucun filtrage).
/// </summary>
internal static class Orchestrateur
{
    // \b : frontière de mot Python (LimiteMotPython) — apparie un identifiant
    // en tête du titre d'une issue GitHub déjà existante.
    private static readonly Regex MotifIdentifiantTitreIssue = new(@"^(TB-\d+)", RegexOptions.CultureInvariant);

    private sealed class TacheResolue
    {
        public required Tache Tache { get; init; }
        public string? Jalon { get; set; }
        public HashSet<string> Couches { get; set; } = new(StringComparer.Ordinal);
        public List<string> SegmentsResiduels { get; set; } = new();
    }

    /// <summary>
    /// Exécute les quatre opérations. Renvoie vrai si l'opérateur doit agir
    /// sur quelque chose que cette exécution a rapporté (code de sortie 1
    /// arbitré) — une exception est déjà la voie des abandons immédiats
    /// (corpus malformé, identifiants inconnus, révision non joignable sous
    /// <c>--appliquer</c>, `gh` absent ou non authentifié sous <c>--appliquer</c>).
    /// </summary>
    public static bool Executer(Arguments args, TextWriter sortie)
    {
        string racine = RacineDepot.Trouver();
        string cheminPlan = Path.Combine(racine, Chemins.PlanRelatif);
        string cheminMethode = Path.Combine(racine, Chemins.MethodeRelatif);
        foreach (var (chemin, nom) in new[] { (cheminPlan, "plan de travail"), (cheminMethode, "méthode de ticket") })
        {
            if (!File.Exists(chemin))
            {
                throw new ErreurCorpusException($"{nom} introuvable : {chemin}");
            }
        }

        string contenuPlan = LecteurTexte.LireTexteIntegral(cheminPlan);
        string contenuMethode = LecteurTexte.LireTexteIntegral(cheminMethode);

        var taxonomie = Taxonomie.Extraire(contenuMethode);
        var maille = MailleDuPlan.Extraire(contenuPlan, racine);
        var slugsWireframes = WireframeSlugs.Lister(racine);
        var taches = PlanDeTravail.ExtraireTaches(contenuPlan);

        sortie.WriteLine($"Racine du dépôt          : {racine}");
        sortie.WriteLine($"Tâches lues dans le plan : {taches.Count}");
        sortie.WriteLine($"Libellés « par jalon »   : {ReprPython.Liste(taxonomie.Jalon)}");
        sortie.WriteLine($"Libellés « par nature de tranche » : {ReprPython.Liste(taxonomie.NatureTranche)}");
        sortie.WriteLine($"Libellés « par nature »  : {ReprPython.Liste(taxonomie.Nature)}");
        var slugsTries = slugsWireframes.ToList();
        slugsTries.Sort(ComparateurCodePoints.Instance);
        sortie.WriteLine($"Slugs de wireframes lus sur disque ({slugsWireframes.Count}) : {ReprPython.Liste(slugsTries)}");

        // Résolution jalon / couche de chaque tâche, une fois pour toutes.
        var naturesTrancheValides = new HashSet<string>(taxonomie.NatureTranche, StringComparer.Ordinal);
        var resolues = new List<TacheResolue>();
        var tachesSansCouche = new List<(string Id, string Raison)>();
        var tachesSegmentsResiduels = new List<(string Id, List<string> Residuels)>();

        foreach (var tache in taches)
        {
            var resolue = new TacheResolue { Tache = tache };

            string valeurJalon = tache.Champs.GetValueOrDefault("Jalon", "");
            var (jalon, _) = ResolutionMaille.ResoudreJalon(valeurJalon, taxonomie.Jalon);
            resolue.Jalon = jalon;

            string valeurPerimetre = tache.Champs.GetValueOrDefault("Périmètre d'écriture", "");
            var (segments, marqueur) = ResolutionMaille.DecouperPerimetre(valeurPerimetre);
            var (libellesCouche, residuels) = ResolutionMaille.MapperNature(
                segments, marqueur, maille, slugsWireframes, naturesTrancheValides);
            resolue.Couches = libellesCouche;
            resolue.SegmentsResiduels = residuels;

            if (libellesCouche.Count == 0)
            {
                string raison = marqueur is not null ? valeurPerimetre : "aucun segment reconnu dans le périmètre d'écriture";
                tachesSansCouche.Add((tache.Id, raison));
            }
            else if (residuels.Count > 0)
            {
                tachesSegmentsResiduels.Add((tache.Id, residuels));
            }

            resolues.Add(resolue);
        }

        sortie.WriteLine($"\nTâches sans libellé de nature déductible ({tachesSansCouche.Count}) :");
        foreach (var (tbId, raison) in tachesSansCouche)
        {
            sortie.WriteLine($"  - {tbId} : {raison}");
        }
        if (tachesSegmentsResiduels.Count > 0)
        {
            sortie.WriteLine($"\nTâches avec un périmètre partiellement déductible ({tachesSegmentsResiduels.Count}) :");
            foreach (var (tbId, residuels) in tachesSegmentsResiduels)
            {
                foreach (var r in residuels)
                {
                    sortie.WriteLine($"  - {tbId} : {r}");
                }
            }
        }

        sortie.WriteLine("\nAucun libellé de nature n'est posé sur les issues : les fiches de "
            + "tâche ne portent aucun champ correspondant. Les libellés de cet axe "
            + "sont créés pour un usage ultérieur, non déductible ici.");

        if (!args.Appliquer)
        {
            sortie.WriteLine("\nMODE À BLANC — rien n'est créé ni modifié sur GitHub.");
        }

        bool disponible = GhCli.Present();
        bool authentifie = disponible && GhCli.Authentifie();

        if (args.Appliquer)
        {
            if (!disponible)
            {
                throw new PanneEnvironnementException(
                    "gh (GitHub CLI) n'est pas installé. Installez-le "
                    + "(voir https://cli.github.com/) puis relancez ce script.");
            }
            if (!authentifie)
            {
                throw new PanneEnvironnementException(
                    "gh n'est pas authentifié. Lancez « gh auth login » puis relancez ce script.");
            }
        }
        else
        {
            if (!disponible)
            {
                sortie.WriteLine("\ngh n'est pas installé : l'existant sur GitHub ne peut pas être "
                    + "vérifié. L'aperçu ci-dessous ne reflète que le plan et la méthode "
                    + "de ticket, pas ce qui existe déjà côté GitHub.");
            }
            else if (!authentifie)
            {
                sortie.WriteLine("\ngh n'est pas authentifié : l'existant sur GitHub ne peut pas être "
                    + "vérifié. L'aperçu ci-dessous ne reflète que le plan et la méthode "
                    + "de ticket, pas ce qui existe déjà côté GitHub.");
            }
        }

        bool peutVerifierExistant = disponible && authentifie;

        string urlOrigin = OrigineGit.LireUrl();
        var (owner, repo) = OrigineGit.DeduireOwnerRepo(urlOrigin);
        string branche = RevisionGuard.LireBrancheCourante();
        string baseHttps = OrigineGit.DeduireBaseHttps(urlOrigin);
        string commit = RevisionGuard.LireCommitCourant();
        string urlBlobPlan = $"{baseHttps}/blob/{commit}/{Chemins.PlanRelatif}";
        string revisionPlan = RevisionGuard.LireRevisionPlan();
        sortie.WriteLine($"\nDépôt distant             : {owner}/{repo}");
        sortie.WriteLine($"Lien de tâche utilisé      : {urlBlobPlan} (identifiant de tâche en tête de titre)");
        sortie.WriteLine($"Révision du plan (estampille de provenance) : {revisionPlan}");

        bool signalOperateur = false;

        if (!RevisionGuard.CommitPresentSurDistant(commit, branche))
        {
            var refsVerifiees = new List<string> { $"origin/{branche}", "origin/main" }.Distinct().ToList();
            string message = $"la révision {commit} n'est pas (encore) présente sur le distant "
                + $"({string.Join(" ou ", refsVerifiees)}) : le lien de tâche ci-dessus renverrait "
                + "une page introuvable tant qu'elle n'aura pas été poussée.";
            if (args.Appliquer)
            {
                throw new ErreurCorpusException(
                    message + " Poussez cette révision avant de créer des tickets, ou "
                    + "relancez sans --appliquer pour un aperçu.");
            }
            sortie.WriteLine($"\nAVERTISSEMENT : {message}");
            signalOperateur = true;
        }

        // ── 1. Libellés ──────────────────────────────────────────────────
        var toutesValeursLibelles = taxonomie.Jalon.Select(v => (Valeur: v, Axe: "jalon"))
            .Concat(taxonomie.NatureTranche.Select(v => (Valeur: v, Axe: "nature_tranche")))
            .Concat(taxonomie.Nature.Select(v => (Valeur: v, Axe: "nature")))
            .ToList();

        var libellesExistants = new HashSet<string>(StringComparer.Ordinal);
        if (peutVerifierExistant)
        {
            foreach (var item in GhCli.ApiListe($"repos/{owner}/{repo}/labels"))
            {
                libellesExistants.Add(item.GetProperty("name").GetString() ?? "");
            }
        }

        var compteLibelles = new CompteRendu();
        foreach (var (nomLibelle, axe) in toutesValeursLibelles)
        {
            if (libellesExistants.Contains(nomLibelle))
            {
                compteLibelles.Existant($"{nomLibelle} ({axe})");
                continue;
            }
            if (!args.Appliquer)
            {
                compteLibelles.Creation($"{nomLibelle} ({axe}) [à créer]");
                continue;
            }
            try
            {
                GhCli.ApiCreer($"repos/{owner}/{repo}/labels", new Dictionary<string, string> { ["name"] = nomLibelle });
                compteLibelles.Creation($"{nomLibelle} ({axe})");
            }
            catch (ErreurCorpusException erreur)
            {
                compteLibelles.Echec($"{nomLibelle} ({axe})", erreur.Message);
            }
        }
        compteLibelles.Afficher(sortie, "Libellés");

        // ── 2. Jalons (milestones) ──────────────────────────────────────
        var jalonsExistants = new Dictionary<string, int>(StringComparer.Ordinal);
        if (peutVerifierExistant)
        {
            foreach (var item in GhCli.ApiListe($"repos/{owner}/{repo}/milestones?state=all"))
            {
                jalonsExistants[item.GetProperty("title").GetString() ?? ""] = item.GetProperty("number").GetInt32();
            }
        }

        var compteJalons = new CompteRendu();
        var numeroJalon = new Dictionary<string, int>(jalonsExistants, StringComparer.Ordinal);
        foreach (var nomJalon in taxonomie.Jalon)
        {
            if (jalonsExistants.ContainsKey(nomJalon))
            {
                compteJalons.Existant(nomJalon);
                continue;
            }
            if (!args.Appliquer)
            {
                compteJalons.Creation($"{nomJalon} [à créer]");
                continue;
            }
            try
            {
                using var cree = GhCli.ApiCreer($"repos/{owner}/{repo}/milestones", new Dictionary<string, string> { ["title"] = nomJalon });
                numeroJalon[nomJalon] = cree!.RootElement.GetProperty("number").GetInt32();
                compteJalons.Creation(nomJalon);
            }
            catch (ErreurCorpusException erreur)
            {
                compteJalons.Echec(nomJalon, erreur.Message);
            }
        }
        compteJalons.Afficher(sortie, "Jalons");

        // ── 3. Issues, une par tâche ─────────────────────────────────────
        var idsIssuesExistantes = new HashSet<string>(StringComparer.Ordinal);
        var numeroIssueParTb = new Dictionary<string, int>(StringComparer.Ordinal);
        var corpsExistantParTb = new Dictionary<string, string>(StringComparer.Ordinal);
        if (peutVerifierExistant)
        {
            foreach (var item in GhCli.ApiListe($"repos/{owner}/{repo}/issues?state=all&per_page=100"))
            {
                if (item.TryGetProperty("pull_request", out _)) continue;

                string titre = item.TryGetProperty("title", out var t) ? t.GetString() ?? "" : "";
                var m = MotifIdentifiantTitreIssue.Match(titre);
                if (m.Success && LimiteMotPython.FrontiereApres(titre, m.Length))
                {
                    string tbIdTrouve = m.Groups[1].Value;
                    idsIssuesExistantes.Add(tbIdTrouve);
                    numeroIssueParTb[tbIdTrouve] = item.GetProperty("number").GetInt32();
                    string? corps = item.TryGetProperty("body", out var b) && b.ValueKind == System.Text.Json.JsonValueKind.String
                        ? b.GetString()
                        : null;
                    corpsExistantParTb[tbIdTrouve] = corps ?? "";
                }
            }
        }

        var compteIssues = new CompteRendu();

        HashSet<string>? retenues = null;
        if (!string.IsNullOrEmpty(args.Seulement))
        {
            retenues = new HashSet<string>(
                args.Seulement.Split(',').Select(TexteUnicode.StripPython).Where(s => s.Length > 0),
                StringComparer.Ordinal);
            var connues = new HashSet<string>(taches.Select(t => t.Id), StringComparer.Ordinal);
            var inconnues = retenues.Where(r => !connues.Contains(r)).ToList();
            inconnues.Sort(ComparateurCodePoints.Instance);
            if (inconnues.Count > 0)
            {
                throw new ErreurCorpusException(
                    "ces identifiants ne désignent aucune tâche du plan : " + string.Join(", ", inconnues));
            }
            var retenuesTriees = retenues.ToList();
            retenuesTriees.Sort(ComparateurCodePoints.Instance);
            sortie.WriteLine($"\nRestriction demandée : {retenues.Count} tâche(s) sur {taches.Count} "
                + $"— {string.Join(", ", retenuesTriees)}");
        }

        foreach (var resolue in resolues)
        {
            var tache = resolue.Tache;
            string tbId = tache.Id;
            if (retenues is not null && !retenues.Contains(tbId)) continue;

            string titreIssue = $"{tbId} — {tache.Titre}";

            bool issueExistante = idsIssuesExistantes.Contains(tbId);
            string corps = CorpsIssue.Construire(
                tbId, urlBlobPlan, revisionPlan, tache.Champs, numeroIssueParTb,
                corpsExistantParTb.GetValueOrDefault(tbId));
            // Idempotence : une issue existante dont le corps engendré tombe
            // déjà juste ne déclenche aucun appel `gh`, sous --appliquer comme
            // en mode à blanc — seule une divergence réelle vaut régénération.
            bool corpsDivergent = issueExistante
                && !string.Equals(corps, corpsExistantParTb.GetValueOrDefault(tbId, ""), StringComparison.Ordinal);

            if (!string.IsNullOrEmpty(args.Seulement))
            {
                string etatCorps = !issueExistante
                    ? "à créer"
                    : corpsDivergent ? "issue existante — corps à régénérer" : "issue existante — corps déjà à jour";
                sortie.WriteLine($"\n── Corps de l'issue {tbId} ({etatCorps}) ──\n{corps}\n── fin du corps {tbId} ──");
            }

            if (issueExistante)
            {
                if (!corpsDivergent)
                {
                    compteIssues.Existant(titreIssue);
                    continue;
                }

                if (!args.Appliquer)
                {
                    compteIssues.MiseAJour($"{titreIssue} [corps à régénérer]");
                    continue;
                }

                // Seul le corps est modifié ici — jamais l'état, les
                // libellés, le jalon ni le titre d'une issue existante
                // (règle rappelée en fin d'exécution, plus bas).
                try
                {
                    int numeroIssue = numeroIssueParTb[tbId];
                    GhCli.ApiModifier(
                        $"repos/{owner}/{repo}/issues/{numeroIssue}",
                        new Dictionary<string, string> { ["body"] = corps });
                    compteIssues.MiseAJour(titreIssue);
                }
                catch (ErreurCorpusException erreur)
                {
                    compteIssues.Echec(titreIssue, erreur.Message);
                }
                continue;
            }

            var libellesIssue = resolue.Couches.ToList();
            libellesIssue.Sort(ComparateurCodePoints.Instance);
            if (resolue.Jalon is not null)
            {
                libellesIssue.Add(resolue.Jalon);
            }

            if (!args.Appliquer)
            {
                string renduLibelles = libellesIssue.Count > 0 ? ReprPython.Liste(libellesIssue) : "aucun";
                compteIssues.Creation($"{titreIssue} [à créer — libellés : {renduLibelles}]");
                continue;
            }

            // `gh api` attend un « -f labels[]=valeur » répété pour un tableau ;
            // un dictionnaire à clé unique ne peut pas porter plusieurs valeurs
            // pour « labels[] », d'où la commande construite directement ici
            // plutôt que via GhCli.ApiCreer.
            try
            {
                var arguments = new List<string>
                {
                    "api", "-X", "POST", $"repos/{owner}/{repo}/issues",
                    "-f", $"title={titreIssue}",
                    "-f", $"body={corps}",
                };
                foreach (var libelle in libellesIssue)
                {
                    arguments.Add("-f");
                    arguments.Add($"labels[]={libelle}");
                }
                if (resolue.Jalon is not null && numeroJalon.TryGetValue(resolue.Jalon, out int numeroDuJalon))
                {
                    arguments.Add("-F");
                    arguments.Add($"milestone={numeroDuJalon}");
                }
                var resultat = Processus.Executer("gh", arguments.ToArray());
                if (resultat.Code != 0)
                {
                    string message = TexteUnicode.StripPython(resultat.Stderr);
                    throw new ErreurCorpusException(message.Length > 0 ? message : "échec sans détail renvoyé par gh");
                }
                compteIssues.Creation(titreIssue);
            }
            catch (ErreurCorpusException erreur)
            {
                compteIssues.Echec(titreIssue, erreur.Message);
            }
        }
        compteIssues.Afficher(sortie, "Issues (une par tâche)");

        sortie.WriteLine("\nRègle rappelée : le plan de travail reste la source de vérité ; sur une "
            + "issue existante, seule la zone engendrée du corps est régénérée quand elle diverge — "
            + "aucune issue existante n'a été fermée, réétiquetée, déplacée de jalon ni renommée par ce script.");

        // Signal arbitré, propre au portage C# : le script Python d'origine ne
        // compte ni les tâches sans libellé de nature déductible ni
        // l'avertissement de révision non joignable dans son propre code de
        // retour (seuls les échecs `gh` y comptent) — le contrat de sortie
        // arbitré pour ce portage étend délibérément ce qui vaut signal.
        return signalOperateur
            || tachesSansCouche.Count > 0
            || compteLibelles.Echecs + compteJalons.Echecs + compteIssues.Echecs > 0;
    }
}
