using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace VerifMarqueursIdentifiants;

/// <summary>
/// Lit le corpus sous <c>docs/</c> : la liste des <c>*.md</c> que git connaît
/// (suivis, plus neufs non ignorés — comme VerifCorpus) et leurs lignes
/// hors des blocs de code.
/// </summary>
internal static class CorpusMarkdown
{
    // UTF-8 strict : une séquence invalide lève au lieu d'être remplacée par U+FFFD,
    // qui masquerait un marqueur ou un identifiant altéré.
    private static readonly UTF8Encoding Utf8Strict = new(
        encoderShouldEmitUTF8Identifier: false,
        throwOnInvalidBytes: true);

    private static readonly Regex MotifFence = new(
        @"^\s*(```+|~~~+)",
        RegexOptions.CultureInvariant);

    public static List<string> ListerFichiers(string racine)
    {
        var ensemble = new HashSet<string>(StringComparer.Ordinal);
        foreach (var p in InvoquerGit(racine, "ls-files", "--", "docs"))
        {
            if (p.EndsWith(".md", StringComparison.Ordinal)) ensemble.Add(p);
        }
        foreach (var p in InvoquerGit(racine, "ls-files", "--others", "--exclude-standard", "--", "docs"))
        {
            if (p.EndsWith(".md", StringComparison.Ordinal)) ensemble.Add(p);
        }

        var liste = ensemble.ToList();
        liste.Sort(StringComparer.Ordinal);
        return liste;
    }

    /// <summary>
    /// Les lignes du fichier, numérotées à partir de 1, sans celles d'un bloc de
    /// code (fences comprises) : un exemple de syntaxe n'est pas un emploi.
    /// </summary>
    public static IEnumerable<(int Numero, string Texte)> LignesHorsCode(string racine, string rel)
    {
        string texte;
        try
        {
            texte = Utf8Strict.GetString(File.ReadAllBytes(Path.Combine(racine, rel)));
        }
        catch (Exception cause) when (cause is DecoderFallbackException or IOException)
        {
            throw new PanneExecutionException($"lecture impossible de {rel} : {cause.Message}", cause);
        }

        bool dansBloc = false;
        char caractereFence = ' ';
        int numero = 0;
        using var lecteur = new StringReader(texte);
        string? ligne;
        while ((ligne = lecteur.ReadLine()) is not null)
        {
            numero++;
            var fence = MotifFence.Match(ligne);
            if (fence.Success)
            {
                char courant = fence.Groups[1].Value[0];
                if (!dansBloc)
                {
                    dansBloc = true;
                    caractereFence = courant;
                }
                else if (courant == caractereFence)
                {
                    dansBloc = false;
                }
                continue;
            }
            if (!dansBloc) yield return (numero, ligne);
        }
    }

    private static List<string> InvoquerGit(string racine, params string[] argumentsGit)
    {
        var depart = new ProcessStartInfo("git")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            StandardOutputEncoding = Encoding.UTF8,
        };
        // Chemins non ASCII rendus tels quels, sans échappement octal.
        depart.ArgumentList.Add("-c");
        depart.ArgumentList.Add("core.quotepath=false");
        depart.ArgumentList.Add("-C");
        depart.ArgumentList.Add(racine);
        foreach (var a in argumentsGit) depart.ArgumentList.Add(a);

        Process processus;
        try
        {
            processus = Process.Start(depart)
                ?? throw new PanneExecutionException("git n'a pas pu être démarré");
        }
        catch (Win32Exception cause)
        {
            throw new PanneExecutionException("git est introuvable sur ce système", cause);
        }

        using (processus)
        {
            var tacheSortie = processus.StandardOutput.ReadToEndAsync();
            var tacheErreur = processus.StandardError.ReadToEndAsync();
            processus.WaitForExit();
            string sortieGit = tacheSortie.GetAwaiter().GetResult();
            string erreurGit = tacheErreur.GetAwaiter().GetResult();

            if (processus.ExitCode != 0)
            {
                throw new PanneExecutionException(
                    $"git a échoué (code {processus.ExitCode}) sur '{racine}' : {erreurGit.Trim()}");
            }

            return sortieGit.Split('\n').Where(p => p.Length > 0).ToList();
        }
    }
}
