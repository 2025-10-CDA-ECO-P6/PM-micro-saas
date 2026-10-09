using System.Text.RegularExpressions;

namespace SiteDocumentation;

/// <summary>
/// Prépare la copie de travail du corpus que DocFX construit. Le corpus est écrit pour
/// GitHub : certains liens y sont justes et DocFX les signale. Ils sont réécrits dans la
/// copie, jamais dans docs/ : un lien vers un dossier pointe son README.md, un lien qui
/// sort de docs/ (ou vise un dossier sans README.md) pointe le dépôt sur GitHub.
/// </summary>
internal static class CorpusPublie
{
    // Dossier local au poste, hors suivi : jamais publié.
    private const string DossierExclu = "workstreams";

    // Branche sur laquelle pointent les liens qui sortent de docs/.
    private const string BrancheDuDepot = "main";

    private static readonly Regex MotifLien =
        new(@"\]\(([^)\s#]+)(#[^)\s]*)?\)", RegexOptions.CultureInvariant);

    private static readonly Regex MotifLienCibleMarkdownDansHtml =
        new("(href=\"[^\"#]+)\\.md([#\"])", RegexOptions.CultureInvariant);

    private static readonly Regex MotifSchemaOuRacine =
        new(@"^([a-z][a-z0-9+.-]*:|/)", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

    /// <summary>Copie <paramref name="source"/> dans <paramref name="destination"/> et rend le nombre de liens réécrits.</summary>
    public static int Copier(string source, string destination, string racineDepot, string urlDepot)
    {
        int reecrits = 0;
        foreach (string fichier in Directory.EnumerateFiles(source, "*", SearchOption.AllDirectories))
        {
            string relatif = Path.GetRelativePath(source, fichier);
            if (relatif.StartsWith(DossierExclu + Path.DirectorySeparatorChar, StringComparison.Ordinal))
            {
                continue;
            }

            string cible = Path.Combine(destination, relatif);
            Directory.CreateDirectory(Path.GetDirectoryName(cible)!);

            if (fichier.EndsWith(".html", StringComparison.Ordinal))
            {
                // Les maquettes sont copiées telles quelles, sauf leurs liens vers un .md (publié en .html).
                int dansMaquette = 0;
                string html = MotifLienCibleMarkdownDansHtml.Replace(File.ReadAllText(fichier), m =>
                {
                    dansMaquette++;
                    return $"{m.Groups[1].Value}.html{m.Groups[2].Value}";
                });
                File.WriteAllText(cible, html);
                reecrits += dansMaquette;
            }
            else if (fichier.EndsWith(".md", StringComparison.Ordinal))
            {
                reecrits += CopierMarkdown(fichier, cible, source, racineDepot, urlDepot);
            }
            else
            {
                File.Copy(fichier, cible, true);
            }
        }

        return reecrits;
    }

    private static int CopierMarkdown(string fichier, string cible, string source, string racineDepot, string urlDepot)
    {
        int reecrits = 0;
        bool dansBlocDeCode = false;
        string dossier = Path.GetDirectoryName(fichier)!;
        List<string> lignes = [];
        foreach (string ligne in File.ReadAllLines(fichier))
        {
            if (ligne.StartsWith("```", StringComparison.Ordinal))
            {
                dansBlocDeCode = !dansBlocDeCode;
            }

            lignes.Add(dansBlocDeCode
                ? ligne
                : MotifLien.Replace(ligne, m => Reecrire(m, dossier, source, racineDepot, urlDepot, ref reecrits)));
        }

        File.WriteAllLines(cible, lignes);
        return reecrits;
    }

    private static string Reecrire(Match lien, string dossier, string source, string racineDepot, string urlDepot, ref int reecrits)
    {
        string destination = lien.Groups[1].Value;
        if (MotifSchemaOuRacine.IsMatch(destination))
        {
            return lien.Value;
        }

        string chemin = Path.GetFullPath(Path.Combine(dossier, Uri.UnescapeDataString(destination)));
        bool dansDocs = chemin.StartsWith(source + Path.DirectorySeparatorChar, StringComparison.Ordinal);
        string? neuf = null;
        if (dansDocs && File.Exists(chemin))
        {
            return lien.Value;
        }

        if (dansDocs && Directory.Exists(chemin) && File.Exists(Path.Combine(chemin, "README.md")))
        {
            neuf = destination.TrimEnd('/') + "/README.md";
        }
        else if (File.Exists(chemin) || Directory.Exists(chemin))
        {
            string genre = Directory.Exists(chemin) ? "tree" : "blob";
            neuf = $"{urlDepot}/{genre}/{BrancheDuDepot}/{Path.GetRelativePath(racineDepot, chemin).Replace('\\', '/')}";
        }

        if (neuf is null)
        {
            return lien.Value;
        }

        reecrits++;
        return $"]({neuf}{lien.Groups[2].Value})";
    }
}
