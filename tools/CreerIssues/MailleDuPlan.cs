using System.Text.RegularExpressions;

namespace CreerIssues;

/// <summary>
/// Extraction de la maille de désignation du plan (§2) : les natures de
/// module qu'elle a la charge de résoudre — jamais recopiées en dur ici,
/// lues dans le plan et dans les sources qu'il désigne. La nature « fiche
/// d'écran » (Surface) n'en fait pas partie : elle se résout ailleurs, par
/// <see cref="ResolutionMaille.MapperNature"/>, via les slugs de wireframes
/// lus sur disque (<see cref="WireframeSlugs"/>). Fait partie du socle
/// « lecture du plan » : la vague suivante (report vers un projet GitHub) ne
/// consomme pas cette extraction aujourd'hui, mais elle vit ici, isolée du
/// reste de l'outil, à la même place que les autres lectures du plan.
/// </summary>
internal static class MailleDuPlan
{
    private static readonly Regex MotifH2 = new(@"^##\s+(.*?)\s*$", RegexOptions.CultureInvariant);
    private static readonly Regex MotifAgregatUnique = new(
        @"^Agrégat unique\s*:\s*`?([A-Za-z]+)`?", RegexOptions.CultureInvariant);
    private static readonly Regex MotifH3 = new(
        @"^###\s+([A-Za-z]+)\s*(?:\(([^)]*)\))?\s*$", RegexOptions.CultureInvariant);

    // Deux occurrences de \s dans ce motif, propre à ce fichier (aucun
    // vérificateur existant ne lit la table de la couche cliente) : classe de
    // blanc Python appliquée pour la même raison que dans ResolutionMaille.
    // Seul le TRAITEMENT est arbitré ici : reproduire telle quelle, sans
    // anticiper un futur lecteur de cette table. Le FOND ne se pose pas
    // aujourd'hui, faute d'un second lecteur à comparer — il s'ouvrirait le
    // jour où un vérificateur lirait cette table avec une forme différente.
    private static readonly Regex MotifLigneCoucheCliente = new(
        $"^\\|{BlancPython.Classe}*`([^`]+)`{BlancPython.Classe}*\\|",
        RegexOptions.Multiline | RegexOptions.CultureInvariant);

    private static readonly Regex MotifRenvoiMarkdown = new(
        @"\[`[^`]+`\]\([^)]*\)", RegexOptions.CultureInvariant);
    private static readonly Regex MotifNomEntreAccentsGraves = new(
        @"`([^`]+)`", RegexOptions.CultureInvariant);

    // La clause du § 2 qui verrouille les namespaces de bounded context : portée par la
    // ligne du module transverse nommé (qui les énumère) puis nommant la tranche qui les
    // crée dans le paragraphe « Aucun namespace de bounded context… ».
    private const string MarqueurClauseNamespaces = "sur la seule tranche qui les crée";
    private static readonly Regex MotifTrancheCreatrice = new(
        $"{MarqueurClauseNamespaces}{BlancPython.Classe}*\\((TB-\\d+)\\)", RegexOptions.CultureInvariant);

    public static Dictionary<string, HashSet<string>> Extraire(string contenuPlan, string racine)
    {
        var agregats = ExtraireAgregats(racine);

        int debut = contenuPlan.IndexOf(
            "**Les six modules de la couche cliente et leur ancrage.**", StringComparison.Ordinal);
        int fin = debut != -1
            ? contenuPlan.IndexOf("**L'agrégat couvre ses deux moitiés.**", debut, StringComparison.Ordinal)
            : -1;
        var coucheCliente = new HashSet<string>(StringComparer.Ordinal);
        if (debut != -1 && fin != -1)
        {
            foreach (Match m in MotifLigneCoucheCliente.Matches(contenuPlan[debut..fin]))
            {
                coucheCliente.Add(m.Groups[1].Value);
            }
        }

        var outillage = NomsDePuce(contenuPlan, "un **module d'outillage**");
        var transverses = NomsDePuce(contenuPlan, "un **module transverse nommé**");
        // La ligne du module transverse nommé énumère aussi les quatre namespaces de bounded
        // context, que le § 2 n'admet que sur la tranche qui les crée : ils ne sont pas des
        // modules transverses, ExtraireNamespaces les rend à part.
        transverses.ExceptWith(ExtraireNamespaces(contenuPlan).Noms);

        if (agregats.Count == 0 || coucheCliente.Count == 0 || outillage.Count == 0 || transverses.Count == 0)
        {
            throw new ErreurCorpusException(
                $"la maille de désignation de {Chemins.PlanRelatif} §2 n'a pas pu être lue "
                + "en entier — le document a changé de forme, ce script doit être corrigé "
                + $"(agrégats={agregats.Count}, couche cliente={coucheCliente.Count}, "
                + $"outillage={outillage.Count}, transverses={transverses.Count})");
        }

        return new Dictionary<string, HashSet<string>>(StringComparer.Ordinal)
        {
            ["Comportement"] = agregats,
            ["Couche cliente"] = coucheCliente,
            ["Outillage"] = outillage,
            ["Socle"] = transverses,
        };
    }

    /// <summary>
    /// Les namespaces de bounded context du § 2 et la seule tranche sur laquelle ils se
    /// résolvent, lus dans le plan comme le reste de la maille. Aucun namespace nommé
    /// sous la clause : ensemble vide. Des namespaces nommés sans tranche qui les crée :
    /// <see cref="ErreurCorpusException"/> — le document a changé de forme.
    /// </summary>
    public static NamespacesDeContexte ExtraireNamespaces(string contenuPlan)
    {
        string? ligne = LigneDePuce(contenuPlan, "un **module transverse nommé**");
        int debutClause = ligne?.IndexOf(MarqueurClauseNamespaces, StringComparison.Ordinal) ?? -1;
        if (ligne is null || debutClause == -1)
        {
            return new NamespacesDeContexte(new HashSet<string>(StringComparer.Ordinal), string.Empty);
        }

        var noms = NomsEntreAccentsGraves(ligne[debutClause..]);
        var tranche = MotifTrancheCreatrice.Match(contenuPlan);
        if (noms.Count > 0 && !tranche.Success)
        {
            throw new ErreurCorpusException(
                $"{Chemins.PlanRelatif} §2 nomme des namespaces de bounded context « {MarqueurClauseNamespaces} » "
                + "sans nommer cette tranche (« … qui les crée (TB-nnn) ») — le document a changé de forme, "
                + "ce script doit être corrigé");
        }
        return new NamespacesDeContexte(noms, tranche.Success ? tranche.Groups[1].Value : string.Empty);
    }

    private static HashSet<string> ExtraireAgregats(string racine)
    {
        var agregats = new HashSet<string>(StringComparer.Ordinal);
        string dossier = Path.Combine(racine, "docs/domaine");
        if (!Directory.Exists(dossier)) return agregats;

        // Un contexte a son dossier docs/domaine/<contexte>/ : sa description est
        // context.md, celle du noyau partagé kernel.md.
        var fichiers = Directory.EnumerateFiles(dossier, "*.md")
            .Concat(Directory.EnumerateDirectories(dossier)
                .SelectMany(d => new[] { "context.md", "kernel.md" }.Select(n => Path.Combine(d, n)))
                .Where(File.Exists))
            .Where(f => !Path.GetFileName(f).StartsWith('.'))
            .ToList();
        fichiers.Sort(ComparateurCodePoints.Instance);

        foreach (var chemin in fichiers)
        {
            bool sousAgregats = false;
            foreach (var ligne in LecteurTexte.LireLignes(chemin))
            {
                var titre2 = MotifH2.Match(ligne);
                if (titre2.Success)
                {
                    string titre = titre2.Groups[1].Value;
                    sousAgregats = titre.StartsWith("Agrégats", StringComparison.Ordinal);

                    var unique = MotifAgregatUnique.Match(titre);
                    if (unique.Success)
                    {
                        agregats.Add(unique.Groups[1].Value);
                    }
                    continue;
                }

                var titre3 = MotifH3.Match(ligne);
                if (titre3.Success && sousAgregats)
                {
                    string nom = titre3.Groups[1].Value;
                    string qualificatif = titre3.Groups[2].Success ? titre3.Groups[2].Value : string.Empty;

                    if (qualificatif.Contains(" dans ", StringComparison.Ordinal)) continue;

                    if (qualificatif.Length == 0
                        || qualificatif.Contains("agrégat", StringComparison.Ordinal)
                        || qualificatif.Contains("référence", StringComparison.Ordinal))
                    {
                        agregats.Add(nom);
                    }
                }
            }
        }
        return agregats;
    }

    /// <summary>
    /// Première ligne du plan portant <paramref name="marqueur"/> comme
    /// sous-chaîne — jamais un renvoi de document (libellé de lien markdown
    /// <c>[`texte`](url)</c>, dont la forme le distingue d'un nom de module).
    /// </summary>
    private static string? LigneDePuce(string contenuPlan, string marqueur) =>
        TexteUnicode.SepererLignesPython(contenuPlan)
            .FirstOrDefault(l => l.Contains(marqueur, StringComparison.Ordinal));

    /// <summary>
    /// Les noms entre accents graves de <paramref name="texte"/>, renvois de
    /// document écartés.
    /// </summary>
    private static HashSet<string> NomsEntreAccentsGraves(string texte)
    {
        string sansRenvois = MotifRenvoiMarkdown.Replace(texte, "");
        var resultat = new HashSet<string>(StringComparer.Ordinal);
        foreach (Match m in MotifNomEntreAccentsGraves.Matches(sansRenvois))
        {
            resultat.Add(m.Groups[1].Value);
        }
        return resultat;
    }

    private static HashSet<string> NomsDePuce(string contenuPlan, string marqueur)
    {
        string? ligne = LigneDePuce(contenuPlan, marqueur);
        return ligne is null ? new HashSet<string>(StringComparer.Ordinal) : NomsEntreAccentsGraves(ligne);
    }
}
