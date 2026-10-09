using System.Net;

namespace SiteDocumentation;

/// <summary>Sert le site construit en local, pour l'aperçu : fichiers statiques seulement.</summary>
internal static class Serveur
{
    private static readonly Dictionary<string, string> TypesDeContenu = new(StringComparer.Ordinal)
    {
        [".html"] = "text/html; charset=utf-8",
        [".js"] = "text/javascript; charset=utf-8",
        [".css"] = "text/css; charset=utf-8",
        [".json"] = "application/json; charset=utf-8",
        [".svg"] = "image/svg+xml",
        [".woff2"] = "font/woff2",
        [".yml"] = "text/plain; charset=utf-8",
        [".ico"] = "image/x-icon",
    };

    /// <summary>Sert <paramref name="racine"/> sur <c>http://localhost:port/</c> jusqu'à l'interruption du processus.</summary>
    public static async Task Servir(string racine, int port)
    {
        using HttpListener ecouteur = new();
        ecouteur.Prefixes.Add($"http://localhost:{port}/");
        ecouteur.Start();
        Console.WriteLine($"site servi sur http://localhost:{port}/ (Ctrl+C pour arrêter)");
        while (true)
        {
            HttpListenerContext contexte = await ecouteur.GetContextAsync();
            string chemin = Path.GetFullPath(Path.Combine(racine, Uri.UnescapeDataString(contexte.Request.Url!.AbsolutePath).TrimStart('/')));
            if (Directory.Exists(chemin))
            {
                chemin = Path.Combine(chemin, "index.html");
            }

            // Un chemin qui remonte hors du site est refusé comme un fichier absent.
            if (!chemin.StartsWith(racine, StringComparison.Ordinal) || !File.Exists(chemin))
            {
                contexte.Response.StatusCode = 404;
            }
            else
            {
                contexte.Response.ContentType = TypesDeContenu.GetValueOrDefault(Path.GetExtension(chemin), "application/octet-stream");
                await contexte.Response.OutputStream.WriteAsync(await File.ReadAllBytesAsync(chemin));
            }

            contexte.Response.Close();
        }
    }
}
