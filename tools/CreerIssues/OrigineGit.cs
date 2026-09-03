using System.Text.RegularExpressions;

namespace CreerIssues;

/// <summary>
/// Lecture de l'URL d'origine et déduction du propriétaire, du dépôt et de la
/// base HTTPS qui en découlent. Fait partie du socle « lecture du plan »,
/// lié tel quel par la vague suivante (report du plan vers un projet GitHub).
/// </summary>
internal static class OrigineGit
{
    // Ancrage explicite : Match de .NET cherche, re.match de Python ancre
    // implicitement en tête de chaîne (même convention que le reste du corpus
    // C# de cet outillage).
    private static readonly Regex MotifSsh = new(@"^git@([^:]+):(.+)$", RegexOptions.CultureInvariant);

    private static readonly Regex MotifOwnerRepo = new(
        @"github\.com[:/]+([^/]+)/([^/.]+?)(?:\.git)?/?$", RegexOptions.CultureInvariant);

    /// <summary>Équivalent de <c>git remote get-url origin</c>.</summary>
    public static string LireUrl() => TexteUnicode.StripPython(Processus.ExecuterGit("remote", "get-url", "origin"));

    public static string DeduireBaseHttps(string urlOrigin)
    {
        string url = TexteUnicode.StripPython(urlOrigin);
        if (url.EndsWith(".git", StringComparison.Ordinal))
        {
            url = url[..^4];
        }
        var correspondance = MotifSsh.Match(url);
        return correspondance.Success
            ? $"https://{correspondance.Groups[1].Value}/{correspondance.Groups[2].Value}"
            : url;
    }

    /// <summary>
    /// L'URL distante ne se déduit pas en propriétaire/dépôt GitHub : `git` a
    /// réussi, c'est ce qu'il a renvoyé qui ne convient pas — <see cref="ErreurCorpusException"/>,
    /// jamais <see cref="PanneEnvironnementException"/>.
    /// </summary>
    public static (string Owner, string Repo) DeduireOwnerRepo(string urlOrigin)
    {
        var correspondance = MotifOwnerRepo.Match(TexteUnicode.StripPython(urlOrigin));
        if (!correspondance.Success)
        {
            throw new ErreurCorpusException(
                "impossible de déduire le propriétaire et le dépôt depuis l'URL distante "
                + ReprPython.Chaine(urlOrigin));
        }
        return (correspondance.Groups[1].Value, correspondance.Groups[2].Value);
    }
}
