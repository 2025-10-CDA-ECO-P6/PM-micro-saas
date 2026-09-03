using System.Text.Json;
using CreerIssues;

namespace CreerProjet;

/// <summary>
/// Appels à `gh` propres à cet outil — présence, authentification, et le
/// point d'entrée générique <see cref="ExecuterTexte"/>/<see cref="ExecuterJson"/>
/// qui reproduit <c>gh(*args, json_sortie=False)</c> du script Python
/// d'origine (<c>creer-projet.py</c>). N'invoque jamais `gh` directement en
/// dehors de ces points d'entrée (et de
/// <see cref="Graphql"/>, pour la seule charge qui passe sur l'entrée
/// standard), pour que la garde d'identité du harnais
/// (`tools/Harnais/GardeIdentiteGh`) reste la seule porte vers un exercice
/// réel de <c>--appliquer</c>.
/// </summary>
internal static class GhCli
{
    /// <summary>
    /// Équivalent de <c>shutil.which("gh") is not None</c> : une résolution
    /// PATH, jamais une invocation — `gh` n'est jamais lancé par cette
    /// méthode. Forme miroir de <c>CreerIssues.GhCli.Present</c>, dupliquée
    /// plutôt que partagée (hors du socle « lecture du plan » lié).
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

    /// <summary>
    /// Équivalent de <c>gh(*args, json_sortie=False)</c> : lève
    /// <see cref="ErreurCorpusException"/> — jamais <see cref="PanneEnvironnementException"/> —
    /// sur un code de sortie non nul de `gh` lui-même. `gh` introuvable sur
    /// PATH reste, lui, classé ENVIRONNEMENT : c'est <see cref="Processus.Executer"/>
    /// qui lève avant même que le code de sortie ne soit consulté ici.
    /// </summary>
    public static string ExecuterTexte(params string[] arguments) =>
        TexteUnicode.StripPython(VerifierEtRenvoyerBrut(arguments).Stdout);

    /// <summary>Comme <see cref="ExecuterTexte"/>, en décodant la sortie standard comme un document JSON unique.</summary>
    public static JsonElement ExecuterJson(params string[] arguments)
    {
        using var document = JsonDocument.Parse(VerifierEtRenvoyerBrut(arguments).Stdout);
        return document.RootElement.Clone();
    }

    private static Processus.Resultat VerifierEtRenvoyerBrut(string[] arguments)
    {
        var resultat = Processus.Executer("gh", arguments);
        if (resultat.Code != 0)
        {
            // Python : f"gh {' '.join(args[:3])}… a échoué : {r.stderr.strip()}"
            // — les TROIS premiers arguments seulement, jamais le vecteur
            // entier, et le caractère unique « … » (U+2026), jamais trois
            // points ASCII.
            string troisPremiers = string.Join(' ', arguments.Take(3));
            throw new ErreurCorpusException(
                $"gh {troisPremiers}… a échoué : {TexteUnicode.StripPython(resultat.Stderr)}");
        }
        return resultat;
    }

    /// <summary>
    /// Invocation brute de `gh`, qui n'adjuge jamais son code de sortie —
    /// équivalent du seul <c>subprocess.run([...], capture_output=True, text=True)</c>
    /// direct qu'écrivait le script Python d'origine (<c>creer-projet.py</c>),
    /// aux deux points qui lisent la liste des issues sans passer par le
    /// <c>gh()</c> encapsulé. `gh`
    /// introuvable sur PATH reste classé ENVIRONNEMENT (voir <see cref="ExecuterTexte"/>) :
    /// seule l'adjudication du CODE DE SORTIE est absente ici, pas celle du
    /// lancement du processus lui-même.
    /// </summary>
    public static string InvoquerBrut(params string[] arguments) => Processus.Executer("gh", arguments).Stdout;
}
