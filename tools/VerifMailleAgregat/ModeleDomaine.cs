using System.Text.RegularExpressions;

namespace VerifMailleAgregat;

/// <summary>
/// Extrait la liste fermée des agrégats depuis <c>docs/conception/domain/*.md</c> —
/// la source de vérité que le reste de la maille de désignation consulte.
/// </summary>
internal static class ModeleDomaine
{
    private static readonly Regex MotifH2 = new(
        @"^##\s+(.*?)\s*$", RegexOptions.CultureInvariant);

    private static readonly Regex MotifAgregatUnique = new(
        @"^Agrégat unique\s*:\s*`?([A-Za-z]+)`?", RegexOptions.CultureInvariant);

    private static readonly Regex MotifH3 = new(
        @"^###\s+([A-Za-z]+)\s*(?:\(([^)]*)\))?\s*$", RegexOptions.CultureInvariant);

    public static Dictionary<string, string> Extraire(string racine)
    {
        var trouves = new Dictionary<string, string>(StringComparer.Ordinal);
        string dossier = Path.Combine(racine, "docs/conception/domain");
        if (!Directory.Exists(dossier)) return trouves;

        // glob.glob ignore par défaut les entrées de tête pointée sur le segment
        // générique « *.md » — une énumération .NET brute ne les exclut pas.
        var fichiers = Directory.EnumerateFiles(dossier, "*.md")
            .Where(f => !Path.GetFileName(f).StartsWith('.'))
            .ToList();
        fichiers.Sort(ComparateurCodePoints.Instance);

        foreach (var chemin in fichiers)
        {
            bool sousAgregats = false;
            foreach (var ligne in LecteurTexte.LireLignes(chemin))
            {
                var h2 = MotifH2.Match(ligne);
                if (h2.Success)
                {
                    string titre = h2.Groups[1].Value;
                    sousAgregats = titre.StartsWith("Agrégats", StringComparison.Ordinal);

                    var m = MotifAgregatUnique.Match(titre);
                    if (m.Success)
                    {
                        trouves[m.Groups[1].Value] = Path.GetFileName(chemin);
                    }
                    continue;
                }

                var h3 = MotifH3.Match(ligne);
                if (h3.Success && sousAgregats)
                {
                    string nom = h3.Groups[1].Value;
                    string qual = h3.Groups[2].Success ? h3.Groups[2].Value : string.Empty;

                    // Entité / VO interne : pas une unité d'écriture.
                    if (qual.Contains(" dans ", StringComparison.Ordinal)) continue;

                    if (qual.Length == 0
                        || qual.Contains("agrégat", StringComparison.Ordinal)
                        || qual.Contains("référence", StringComparison.Ordinal))
                    {
                        trouves[nom] = Path.GetFileName(chemin);
                    }
                }
            }
        }
        return trouves;
    }
}
