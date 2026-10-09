using System.Diagnostics;

namespace CaracterisationOutils;

/// <summary>
/// Compile une fois, avant les tests, chaque outil en fichier unique que les tests
/// lancent (tools/FICHIER-UNIQUE.md) dans <see cref="Helpers.DossierOutils"/> :
/// les tests lancent ensuite l'exécutable natif, jamais <c>dotnet run</c>, dont
/// les lancements parallèles se disputeraient les sorties de compilation.
/// <c>--no-incremental</c> écarte le piège du cache, qui se fie à la date de
/// modification des sources : un binaire compilé depuis une autre version ne
/// peut pas être testé à la place de la source courante.
/// </summary>
internal static class CompilationDesOutils
{
    /// <summary>Renvoie <c>null</c> si tout est compilé, le diagnostic du premier échec sinon.</summary>
    public static string? Compiler()
    {
        if (Directory.Exists(Helpers.DossierOutils)) Directory.Delete(Helpers.DossierOutils, recursive: true);

        foreach (var source in Helpers.SourcesDesOutils)
        {
            string sortie = Path.Combine(Helpers.DossierOutils, Path.GetFileNameWithoutExtension(source));
            var depart = new ProcessStartInfo("dotnet")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                UseShellExecute = false,
            };
            foreach (var a in new[] { "build", source, "-o", sortie, "--no-incremental", "--nologo", "-v", "q" }) depart.ArgumentList.Add(a);

            using var processus = Process.Start(depart)
                ?? throw new InvalidOperationException("dotnet n'a pas pu être démarré");
            processus.StandardInput.Close();
            var tacheSortie = processus.StandardOutput.ReadToEndAsync();
            var tacheErreur = processus.StandardError.ReadToEndAsync();
            processus.WaitForExit();
            string stdout = tacheSortie.GetAwaiter().GetResult();
            string stderr = tacheErreur.GetAwaiter().GetResult();
            if (processus.ExitCode != 0)
            {
                return $"compilation de {source} en échec (code {processus.ExitCode}) :{Environment.NewLine}{stdout}{stderr}";
            }
        }
        return null;
    }
}
