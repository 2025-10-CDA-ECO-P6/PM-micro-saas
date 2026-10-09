using System.Text.RegularExpressions;

namespace VerifMailleAgregat;

/// <summary>
/// Les modules admis dans un item de « Périmètre d'écriture » que le § 2 du
/// plan nomme lui-même : modules d'outillage, modules transverses nommés,
/// modules de la couche cliente, namespaces de bounded context et tranche qui
/// les crée. Jamais recopiés en dur ici, lus dans le § 2 du plan. Même règle de
/// lecture que <c>MailleDuPlan</c> de CreerIssues (première ligne du plan portant
/// le marqueur, noms entre accents graves, libellés de renvoi markdown écartés) :
/// chaque outil garde son lecteur, car un <c>#:include</c> entre les deux ne
/// marche pas (des fichiers de même nom n'y ont pas le même contenu), et
/// DivergencesEntreOutilsTests.cs vérifie qu'ils tirent les mêmes listes d'un
/// même plan. Un paragraphe du § 2 introuvable est une panne d'exécution qui le nomme.
/// </summary>
internal static class ModulesOutillage
{
    private const string MarqueurOutillage = "un **module d'outillage**";
    private const string MarqueurTransverses = "un **module transverse nommé**";
    private const string MarqueurClauseNamespaces = "sur la seule tranche qui les crée";
    private const string DebutCoucheCliente = "**Les six modules de la couche cliente et leur ancrage.**";
    private const string FinCoucheCliente = "**L'agrégat couvre ses deux moitiés.**";

    // Même classe de blanc que BlancPython.Classe de CreerIssues (le \s de Python), pour que les
    // deux outils coupent aux mêmes endroits.
    private const string Blanc = "[\\t\\n\\v\\f\\r\\x1c\\x1d\\x1e\\x1f \\x85\\p{Zs}\\p{Zl}\\p{Zp}]";

    private static readonly Regex MotifRenvoiMarkdown = new(
        @"\[`[^`]+`\]\([^)]*\)", RegexOptions.CultureInvariant);
    private static readonly Regex MotifNomEntreAccentsGraves = new(
        @"`([^`]+)`", RegexOptions.CultureInvariant);
    private static readonly Regex MotifLigneCoucheCliente = new(
        $"^\\|{Blanc}*`([^`]+)`{Blanc}*\\|", RegexOptions.Multiline | RegexOptions.CultureInvariant);
    private static readonly Regex MotifTrancheCreatrice = new(
        $"{MarqueurClauseNamespaces}{Blanc}*\\((TB-\\d+)\\)", RegexOptions.CultureInvariant);

    public static ModulesDuPlan LireTous(string textePlan)
    {
        // L'outillage d'abord : un plan sans aucun des paragraphes du § 2 se plaint de lui en premier.
        var outillage = Lire(textePlan);
        var namespaces = LireNamespaces(textePlan);
        var transverses = NomsDeLaPuce(textePlan, MarqueurTransverses);
        // La ligne du module transverse nommé énumère aussi les namespaces de contexte, que le
        // § 2 n'admet que sur la tranche qui les crée : ils ne sont pas des modules transverses.
        transverses.ExceptWith(namespaces.Noms);
        return new ModulesDuPlan(outillage, transverses, LireClient(textePlan), namespaces);
    }

    public static HashSet<string> Lire(string textePlan) => NomsDeLaPuce(textePlan, MarqueurOutillage);

    private static HashSet<string> LireClient(string textePlan)
    {
        int debut = textePlan.IndexOf(DebutCoucheCliente, StringComparison.Ordinal);
        int fin = debut != -1 ? textePlan.IndexOf(FinCoucheCliente, debut, StringComparison.Ordinal) : -1;
        var modules = new HashSet<string>(StringComparer.Ordinal);
        if (debut != -1 && fin != -1)
        {
            foreach (Match m in MotifLigneCoucheCliente.Matches(textePlan[debut..fin]))
            {
                modules.Add(m.Groups[1].Value);
            }
        }
        if (modules.Count == 0)
        {
            throw Illisible($"aucun module de la couche cliente entre « {DebutCoucheCliente} » et « {FinCoucheCliente} »");
        }
        return modules;
    }

    private static NamespacesDuPlan LireNamespaces(string textePlan)
    {
        string? ligne = LigneDePuce(textePlan, MarqueurTransverses);
        int debutClause = ligne?.IndexOf(MarqueurClauseNamespaces, StringComparison.Ordinal) ?? -1;
        var noms = ligne is null || debutClause == -1
            ? new HashSet<string>(StringComparer.Ordinal)
            : NomsEntreAccentsGraves(ligne[debutClause..]);
        if (noms.Count == 0)
        {
            throw Illisible(
                $"aucun namespace de bounded context nommé après « {MarqueurClauseNamespaces} » "
                + $"sur une ligne portant « {MarqueurTransverses} »");
        }
        var tranche = MotifTrancheCreatrice.Match(textePlan);
        if (!tranche.Success)
        {
            throw Illisible($"la tranche qui crée les namespaces n'est pas nommée (« {MarqueurClauseNamespaces} (TB-nnn) »)");
        }
        return new NamespacesDuPlan(noms, tranche.Groups[1].Value);
    }

    private static string? LigneDePuce(string textePlan, string marqueur) =>
        TexteUnicode.SepererLignesPython(textePlan)
            .FirstOrDefault(l => l.Contains(marqueur, StringComparison.Ordinal));

    private static HashSet<string> NomsEntreAccentsGraves(string texte)
    {
        string sansRenvois = MotifRenvoiMarkdown.Replace(texte, "");
        var noms = new HashSet<string>(StringComparer.Ordinal);
        foreach (Match m in MotifNomEntreAccentsGraves.Matches(sansRenvois))
        {
            noms.Add(m.Groups[1].Value);
        }
        return noms;
    }

    private static HashSet<string> NomsDeLaPuce(string textePlan, string marqueur)
    {
        string? ligne = LigneDePuce(textePlan, marqueur);
        var noms = ligne is null ? new HashSet<string>(StringComparer.Ordinal) : NomsEntreAccentsGraves(ligne);
        if (noms.Count == 0)
        {
            throw Illisible($"aucun module sur une ligne portant « {marqueur} »");
        }
        return noms;
    }

    private static PanneExecutionException Illisible(string detail) => new(
        $"la maille de désignation du plan (§ 2) n'a pas pu être lue : {detail} "
        + "— le document a changé de forme, ce vérificateur doit être corrigé");
}
