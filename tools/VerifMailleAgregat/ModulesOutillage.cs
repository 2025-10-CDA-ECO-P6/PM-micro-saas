using System.Text.RegularExpressions;

namespace VerifMailleAgregat;

/// <summary>
/// Les modules d'outillage admis dans un item de « Périmètre d'écriture » :
/// jamais recopiés en dur ici, lus dans le § 2 du plan, sur la puce
/// « un **module d'outillage** ». Même règle de lecture que
/// <c>MailleDuPlan</c> de CreerIssues (première ligne du plan portant le
/// marqueur, noms entre accents graves, libellés de renvoi markdown écartés) :
/// chaque outil garde son lecteur, et DivergencesEntreOutilsTests.cs vérifie
/// qu'ils tirent la même liste d'un même plan.
/// </summary>
internal static class ModulesOutillage
{
    private const string Marqueur = "un **module d'outillage**";

    private static readonly Regex MotifRenvoiMarkdown = new(
        @"\[`[^`]+`\]\([^)]*\)", RegexOptions.CultureInvariant);
    private static readonly Regex MotifNomEntreAccentsGraves = new(
        @"`([^`]+)`", RegexOptions.CultureInvariant);

    public static HashSet<string> Lire(string textePlan)
    {
        string? ligne = TexteUnicode.SepererLignesPython(textePlan)
            .FirstOrDefault(l => l.Contains(Marqueur, StringComparison.Ordinal));
        string sansRenvois = ligne is null ? "" : MotifRenvoiMarkdown.Replace(ligne, "");
        var modules = new HashSet<string>(StringComparer.Ordinal);
        foreach (Match m in MotifNomEntreAccentsGraves.Matches(sansRenvois))
        {
            modules.Add(m.Groups[1].Value);
        }
        if (modules.Count == 0)
        {
            throw new PanneExecutionException(
                "la maille de désignation du plan (§ 2) n'a pas pu être lue : aucun module d'outillage "
                + $"sur une ligne portant « {Marqueur} » — le document a changé de forme, ce vérificateur doit être corrigé");
        }
        return modules;
    }
}
