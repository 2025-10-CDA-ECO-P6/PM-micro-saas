// Chaîne du site de documentation : publie le corpus de docs/ avec DocFX, utilisé comme
// bibliothèque (aucun outil dotnet à installer). Ce projet publie le corpus, il ne le
// vérifie pas : la vérification du corpus reste celle de tools/.
//
// Usage : SiteDocumentation [--servir] [docfx.json]
// Sans argument, lit docs-site/docfx.json depuis la racine du dépôt. Étapes :
//   1. copie docs/ dans docs-site/_work/docs en réécrivant les liens que GitHub accepte
//      et que DocFX signale (CorpusPublie) ;
//   2. génère les sommaires depuis l'arborescence (TableDesMatieres) ;
//   3. construit avec DocFX dans docs-site/_site : tout avertissement ou erreur fait
//      sortir en code 1 ;
//   4. copie chaque README.html en index.html (URL de dossier) ;
//   5. contrôle les liens et les ancres du HTML construit (ControleSite) ;
//   6. avec --servir, sert le site sur http://localhost:8080/ (Ctrl+C pour arrêter).
//
// L'URL du dépôt n'est écrite qu'une fois, dans docfx.json (_repoUrl) : cette chaîne
// la lit, ne la recopie pas. Un changement d'organisation ou de nom se fait sur cette
// seule ligne ; l'URL du site publié est celle que GitHub Pages attribue au dépôt.

using System.Globalization;
using System.Text.Json;
using Docfx;
using Docfx.Common;
using SiteDocumentation;

CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

bool servir = args.Contains("--servir");
string configuration = Path.GetFullPath(args.FirstOrDefault(a => !a.StartsWith("--")) ?? Path.Combine("docs-site", "docfx.json"));
string dossierSite = Path.GetDirectoryName(configuration)!;
string racineDepot = Path.GetFullPath(Path.Combine(dossierSite, ".."));
string travail = Path.Combine(dossierSite, "_work");
string sortie = Path.Combine(dossierSite, "_site");

try
{
    using JsonDocument json = JsonDocument.Parse(File.ReadAllText(configuration));
    string urlDepot = json.RootElement
        .GetProperty("build").GetProperty("globalMetadata").GetProperty("_repoUrl").GetString()!
        .TrimEnd('/');

    if (Directory.Exists(travail))
    {
        Directory.Delete(travail, true);
    }

    int liensReecrits = CorpusPublie.Copier(Path.Combine(racineDepot, "docs"), Path.Combine(travail, "docs"), racineDepot, urlDepot);
    List<string> entreesInutiles = TableDesMatieres.Generer(Path.Combine(travail, "docs"), Path.Combine(dossierSite, "titres-sections.txt"));
    foreach (string chemin in entreesInutiles)
    {
        Console.Error.WriteLine($"titres-sections.txt : « {chemin} » est sans objet (dossier absent, ou doté d'un README.md qui donne déjà son titre)");
    }

    if (entreesInutiles.Count > 0)
    {
        return 1;
    }

    Console.WriteLine($"liens réécrits : {liensReecrits}");

    if (Directory.Exists(sortie))
    {
        Directory.Delete(sortie, true);
    }

    await Docset.Build(configuration);
    int defautsDocfx = Logger.WarningCount + Logger.ErrorCount;
    if (defautsDocfx > 0)
    {
        Console.Error.WriteLine($"construction stricte : {defautsDocfx} avertissement(s) ou erreur(s) DocFX");
        return 1;
    }

    foreach (string readme in Directory.GetFiles(sortie, "README.html", SearchOption.AllDirectories))
    {
        File.Copy(readme, Path.Combine(Path.GetDirectoryName(readme)!, "index.html"), true);
    }

    int defautsLiens = ControleSite.Controler(sortie);
    if (defautsLiens > 0)
    {
        return 1;
    }

    if (servir)
    {
        await Serveur.Servir(sortie, 8080);
    }

    return 0;
}
catch (Exception imprevue) when (imprevue is IOException or JsonException or KeyNotFoundException or InvalidOperationException)
{
    Console.Error.WriteLine($"panne d'exécution : {imprevue.Message}");
    return 2;
}
