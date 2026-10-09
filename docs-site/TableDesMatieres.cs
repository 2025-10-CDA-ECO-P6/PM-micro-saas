using System.Text;
using System.Text.RegularExpressions;

namespace SiteDocumentation;

/// <summary>
/// Génère les sommaires du site (toc.yml) depuis l'arborescence du corpus. Le toc.yml de la
/// racine alimente la barre de navigation : un fichier .md par entrée, nommée d'après son
/// premier titre, et une entrée par dossier de premier niveau. Chacun de ces dossiers reçoit
/// son propre toc.yml, qui alimente le sommaire latéral et le fil d'Ariane des pages qu'il
/// contient (le modèle « modern » compose le fil d'Ariane de l'entrée de navigation active et
/// des ancêtres de la page dans le sommaire le plus proche : un toc.yml unique le fausse).
/// Le nom d'un dossier est le premier titre de son README.md, sans la mention « Haversack » ;
/// à défaut de README.md, la table des titres de sections, puis le nom du dossier.
/// </summary>
internal static class TableDesMatieres
{
    private static readonly Regex MotifMentionDuProduit =
        new(@"^Haversack — |( — (MVP )?Haversack( MVP)?)$", RegexOptions.CultureInvariant);

    /// <summary>
    /// Écrit les toc.yml dans <paramref name="dossier"/>, la copie de travail du corpus, et rend les
    /// entrées de la table des titres qui ne servent à rien (dossier absent, ou doté d'un README.md).
    /// </summary>
    public static List<string> Generer(string dossier, string fichierTitres)
    {
        Dictionary<string, string> titres = File.ReadAllLines(fichierTitres)
            .Where(l => l.Contains(" = ", StringComparison.Ordinal) && !l.StartsWith('#'))
            .Select(l => l.Split(" = ", 2))
            .ToDictionary(p => p[0].Trim(), p => p[1].Trim());
        List<string> inutiles = titres.Keys
            .Where(chemin => !Directory.Exists(Path.Combine(dossier, chemin)) || File.Exists(Path.Combine(dossier, chemin, "README.md")))
            .Order(StringComparer.Ordinal)
            .ToList();

        StringBuilder navigation = new();
        foreach (string fichier in Directory.GetFiles(dossier, "*.md").Order(StringComparer.Ordinal))
        {
            string nom = Path.GetFileName(fichier);
            navigation.AppendLine($"- name: {Guillemets(nom == "README.md" ? "Accueil" : PremierTitre(fichier) ?? Path.GetFileNameWithoutExtension(fichier))}").AppendLine($"  href: {nom}");
        }

        foreach (string section in Directory.GetDirectories(dossier).Order(StringComparer.Ordinal))
        {
            string nom = Path.GetFileName(section);
            navigation.AppendLine($"- name: {Guillemets(TitreDeSection(section, nom, titres))}").AppendLine($"  href: {nom}/");
            if (File.Exists(Path.Combine(section, "README.md")))
            {
                navigation.AppendLine($"  topicHref: {nom}/README.md");
            }

            File.WriteAllText(Path.Combine(section, "toc.yml"), Entrees(section, nom, 0, titres));
        }

        File.WriteAllText(Path.Combine(dossier, "toc.yml"), navigation.ToString());
        return inutiles;
    }

    // Les href d'un toc.yml sont relatifs à ce fichier : ceux d'une section le sont à son dossier,
    // alors que la table des titres est indexée par le chemin depuis docs/ (cheminCorpus).
    private static string Entrees(string dossier, string cheminCorpus, int niveau, Dictionary<string, string> titres, string prefixe = "")
    {
        StringBuilder sb = new();
        string marge = new(' ', niveau * 4);

        foreach (string fichier in Directory.GetFiles(dossier, "*.md").Order(StringComparer.Ordinal))
        {
            string nom = Path.GetFileName(fichier);
            if (nom == "README.md" && niveau > 0)
            {
                continue;
            }

            string titre = nom == "README.md" ? "Présentation de la section" : PremierTitre(fichier) ?? Path.GetFileNameWithoutExtension(fichier);
            sb.AppendLine($"{marge}- name: {Guillemets(titre)}").AppendLine($"{marge}  href: {prefixe}{nom}");
        }

        foreach (string sousDossier in Directory.GetDirectories(dossier).Order(StringComparer.Ordinal))
        {
            string nom = Path.GetFileName(sousDossier);
            string chemin = $"{cheminCorpus}/{nom}";
            sb.AppendLine($"{marge}- name: {Guillemets(TitreDeSection(sousDossier, chemin, titres))}");
            if (File.Exists(Path.Combine(sousDossier, "README.md")))
            {
                sb.AppendLine($"{marge}  href: {prefixe}{nom}/README.md");
            }

            string enfants = Entrees(sousDossier, chemin, niveau + 1, titres, $"{prefixe}{nom}/");
            if (enfants.Length > 0)
            {
                sb.AppendLine($"{marge}  items:").Append(enfants);
            }
        }

        return sb.ToString();
    }

    private static string TitreDeSection(string dossier, string chemin, Dictionary<string, string> titres)
    {
        string readme = Path.Combine(dossier, "README.md");
        string? titre = File.Exists(readme) ? PremierTitre(readme) : null;
        if (titre is not null)
        {
            string sans = MotifMentionDuProduit.Replace(titre, string.Empty).Trim();
            return sans.Length > 0 ? sans : titre;
        }

        if (titres.TryGetValue(chemin, out string? titreDeLaTable))
        {
            return titreDeLaTable;
        }

        string nom = Path.GetFileName(chemin).Replace('-', ' ');
        return char.ToUpperInvariant(nom[0]) + nom[1..];
    }

    private static string? PremierTitre(string fichier)
    {
        bool dansBlocDeCode = false;
        foreach (string ligne in File.ReadLines(fichier))
        {
            if (ligne.StartsWith("```", StringComparison.Ordinal))
            {
                dansBlocDeCode = !dansBlocDeCode;
            }

            if (!dansBlocDeCode && ligne.StartsWith("# ", StringComparison.Ordinal))
            {
                return ligne[2..].Trim();
            }
        }

        return null;
    }

    private static string Guillemets(string texte) =>
        "\"" + texte.Replace("\\", "\\\\", StringComparison.Ordinal).Replace("\"", "\\\"", StringComparison.Ordinal) + "\"";
}
