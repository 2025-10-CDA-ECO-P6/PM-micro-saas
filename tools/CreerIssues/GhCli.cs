using System.Text.Json;

namespace CreerIssues;

/// <summary>
/// Appels à `gh` — présence, authentification, lecture paginée, création.
/// N'invoque jamais `gh` directement en dehors de ces points d'entrée, pour
/// que la garde d'identité du harnais (`tools/Harnais/GardeIdentiteGh`) reste
/// la seule porte vers un exercice réel de `--appliquer`.
/// </summary>
internal static class GhCli
{
    /// <summary>
    /// Équivalent de <c>shutil.which("gh") is not None</c> : une résolution
    /// PATH, jamais une invocation — `gh` n'est jamais lancé par cette
    /// méthode.
    /// </summary>
    public static bool Present()
    {
        string? chemin = Environment.GetEnvironmentVariable("PATH");
        if (string.IsNullOrEmpty(chemin)) return false;

        string[] noms = OperatingSystem.IsWindows() ? new[] { "gh.exe", "gh.cmd", "gh.bat", "gh" } : new[] { "gh" };
        foreach (var dossier in chemin.Split(Path.PathSeparator))
        {
            if (dossier.Length == 0) continue;
            foreach (var nom in noms)
            {
                string candidat = Path.Combine(dossier, nom);
                if (File.Exists(candidat) && EstExecutable(candidat)) return true;
            }
        }
        return false;
    }

    private static bool EstExecutable(string chemin)
    {
        if (OperatingSystem.IsWindows()) return true;
        try
        {
            var mode = File.GetUnixFileMode(chemin);
            return (mode & (UnixFileMode.UserExecute | UnixFileMode.GroupExecute | UnixFileMode.OtherExecute)) != 0;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public static bool Authentifie() => Processus.Executer("gh", "auth", "status").Code == 0;

    /// <summary>Équivalent de <c>gh api &lt;chemin&gt; --paginate</c>, décodé et aplati.</summary>
    public static List<JsonElement> ApiListe(string cheminApi)
    {
        var resultat = Processus.Executer("gh", "api", cheminApi, "--paginate");
        if (resultat.Code != 0)
        {
            throw new ErreurCorpusException(
                $"lecture de {cheminApi} échouée : {TexteUnicode.StripPython(resultat.Stderr)}");
        }
        return DecodeurJsonConcatene.Decoder(resultat.Stdout);
    }

    /// <summary>
    /// Équivalent de <c>gh api -X POST &lt;chemin&gt; -f clé=valeur…</c>. Renvoie
    /// le document JSON décodé de la réponse, ou <see langword="null"/> si la
    /// sortie standard est vide — comme <c>json.loads(stdout) if stdout.strip() else {}</c>
    /// côté Python, dont seul l'appelant qui crée un jalon exploite le
    /// résultat (<c>cree["number"]</c>).
    /// </summary>
    public static JsonDocument? ApiCreer(string cheminApi, IReadOnlyDictionary<string, string> champsTexte)
    {
        var arguments = new List<string> { "api", "-X", "POST", cheminApi };
        foreach (var (cle, valeur) in champsTexte)
        {
            arguments.Add("-f");
            arguments.Add($"{cle}={valeur}");
        }
        var resultat = Processus.Executer("gh", arguments.ToArray());
        if (resultat.Code != 0)
        {
            string message = TexteUnicode.StripPython(resultat.Stderr);
            throw new ErreurCorpusException(message.Length > 0 ? message : "échec sans détail renvoyé par gh");
        }
        return TexteUnicode.StripPython(resultat.Stdout).Length > 0 ? JsonDocument.Parse(resultat.Stdout) : null;
    }

    /// <summary>
    /// Équivalent de <c>gh api -X PATCH &lt;chemin&gt; -f clé=valeur…</c> — seule
    /// méthode de ce module qui modifie une ressource déjà existante plutôt
    /// que d'en créer une. Même contrat d'erreur que <see cref="ApiCreer"/> ;
    /// la réponse n'est jamais exploitée par l'appelant (régénération du
    /// corps d'une issue), donc pas décodée ici.
    /// </summary>
    public static void ApiModifier(string cheminApi, IReadOnlyDictionary<string, string> champsTexte)
    {
        var arguments = new List<string> { "api", "-X", "PATCH", cheminApi };
        foreach (var (cle, valeur) in champsTexte)
        {
            arguments.Add("-f");
            arguments.Add($"{cle}={valeur}");
        }
        var resultat = Processus.Executer("gh", arguments.ToArray());
        if (resultat.Code != 0)
        {
            string message = TexteUnicode.StripPython(resultat.Stderr);
            throw new ErreurCorpusException(message.Length > 0 ? message : "échec sans détail renvoyé par gh");
        }
    }
}
