using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;

namespace SiteDocumentation;

/// <summary>
/// Contrôle le HTML construit, indépendamment de DocFX : DocFX ne vérifie pas les ancres.
/// Chaque lien interne doit viser un fichier présent du site, et chaque ancre un
/// <c>id</c> (ou un <c>name</c>) réellement présent dans la page visée.
/// </summary>
internal static class ControleSite
{
    /// <summary>Contrôle <paramref name="racine"/>, écrit chaque défaut sur la sortie d'erreur et rend leur nombre.</summary>
    public static int Controler(string racine)
    {
        HtmlParser analyseur = new();
        Dictionary<string, IHtmlDocument> pages = new(StringComparer.Ordinal);
        Dictionary<string, HashSet<string>> identifiants = new(StringComparer.Ordinal);
        foreach (string fichier in Directory.EnumerateFiles(racine, "*.html", SearchOption.AllDirectories))
        {
            string relatif = Path.GetRelativePath(racine, fichier).Replace('\\', '/');
            IHtmlDocument page = analyseur.ParseDocument(File.ReadAllText(fichier));
            pages[relatif] = page;
            HashSet<string> ids = new(StringComparer.Ordinal);
            foreach (IElement element in page.QuerySelectorAll("[id]"))
            {
                ids.Add(element.GetAttribute("id")!);
            }

            foreach (IElement element in page.QuerySelectorAll("a[name]"))
            {
                ids.Add(element.GetAttribute("name")!);
            }

            identifiants[relatif] = ids;
        }

        int liens = 0;
        int defauts = 0;
        foreach ((string relatif, IHtmlDocument page) in pages.OrderBy(p => p.Key, StringComparer.Ordinal))
        {
            foreach (IElement element in page.QuerySelectorAll("a[href], link[href], script[src], img[src]"))
            {
                string href = element.GetAttribute("href") ?? element.GetAttribute("src")!;
                if (href.Length == 0 || Uri.TryCreate(href, UriKind.Absolute, out _) || href.StartsWith("//", StringComparison.Ordinal))
                {
                    continue;
                }

                liens++;
                string? defaut = Defaut(racine, relatif, href, identifiants);
                if (defaut is not null)
                {
                    defauts++;
                    Console.Error.WriteLine($"{defaut} : {relatif} -> {href}");
                }
            }
        }

        Console.WriteLine($"contrôle du site : {pages.Count} pages, {liens} liens internes, {defauts} défaut(s)");
        return defauts;
    }

    private static string? Defaut(string racine, string page, string href, Dictionary<string, HashSet<string>> identifiants)
    {
        int diese = href.IndexOf('#', StringComparison.Ordinal);
        string chemin = diese >= 0 ? href[..diese] : href;
        string ancre = diese >= 0 ? Uri.UnescapeDataString(href[(diese + 1)..]) : string.Empty;

        string cible = page;
        if (chemin.Length > 0)
        {
            int requete = chemin.IndexOf('?', StringComparison.Ordinal);
            if (requete >= 0)
            {
                chemin = chemin[..requete];
            }

            string dossier = Path.GetDirectoryName(page)?.Replace('\\', '/') ?? string.Empty;
            string complet = Path.GetFullPath(Path.Combine(racine, dossier, Uri.UnescapeDataString(chemin)));
            cible = Path.GetRelativePath(racine, complet).Replace('\\', '/');
            if (Directory.Exists(complet))
            {
                cible += "/index.html";
            }
        }

        if (!File.Exists(Path.Combine(racine, cible)))
        {
            return "fichier introuvable";
        }

        if (ancre.Length == 0 || ancre == "top")
        {
            return null;
        }

        return identifiants.TryGetValue(cible, out HashSet<string>? ids) && ids.Contains(ancre) ? null : "ancre introuvable";
    }
}
