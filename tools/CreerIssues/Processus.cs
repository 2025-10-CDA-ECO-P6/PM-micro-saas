using System.ComponentModel;
using System.Diagnostics;

namespace CreerIssues;

/// <summary>
/// Invocation d'un sous-processus, capture texte des deux flux — substrat
/// partagé par <see cref="RacineDepot"/> et <see cref="OrigineGit"/> (lecture
/// du dépôt). Fait partie du socle « lecture du plan » : ne dépend d'aucun
/// autre fichier de cet outil, et n'est lui-même utilisé que par ce socle et
/// par <see cref="RevisionGuard"/> (garde de révision, hors socle isolé).
/// </summary>
internal static class Processus
{
    public readonly record struct Resultat(string Stdout, string Stderr, int Code);

    /// <summary>
    /// Équivalent de <c>subprocess.run(commande, capture_output=True, text=True)</c> :
    /// n'écrit jamais directement sur les flux du processus courant, ne lève
    /// jamais elle-même sur un code de sortie non nul — c'est à l'appelant
    /// d'adjuger, comme le fait le <c>verifier</c> optionnel côté Python.
    /// </summary>
    public static Resultat Executer(string fichier, params string[] arguments)
    {
        var depart = new ProcessStartInfo(fichier)
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };
        foreach (var a in arguments) depart.ArgumentList.Add(a);

        Process processus;
        try
        {
            processus = Process.Start(depart)
                ?? throw new PanneEnvironnementException($"« {fichier} » n'a pas pu être démarré");
        }
        catch (Win32Exception cause)
        {
            throw new PanneEnvironnementException($"« {fichier} » est introuvable sur ce système", cause);
        }

        using (processus)
        {
            var tacheSortie = processus.StandardOutput.ReadToEndAsync();
            var tacheErreur = processus.StandardError.ReadToEndAsync();
            processus.WaitForExit();
            string sortie = tacheSortie.GetAwaiter().GetResult();
            string erreur = tacheErreur.GetAwaiter().GetResult();
            return new Resultat(sortie, erreur, processus.ExitCode);
        }
    }

    /// <summary>
    /// Équivalent de l'<c>executer(commande)</c> Python (le <c>verifier=True</c>
    /// par défaut) : toute commande <c>git</c> qui échoue lève — c'est
    /// l'environnement qui est en cause, jamais le contenu du corpus, d'où
    /// <see cref="PanneEnvironnementException"/> plutôt que
    /// <see cref="ErreurCorpusException"/>. Le message reproduit à la lettre
    /// <c>f"la commande {' '.join(commande)!r} a échoué : {stderr.strip()}"</c>.
    /// </summary>
    public static string ExecuterGit(params string[] arguments)
    {
        var resultat = Executer("git", arguments);
        if (resultat.Code != 0)
        {
            throw new PanneEnvironnementException(
                $"la commande {ReprPython.Chaine("git " + string.Join(' ', arguments))} a échoué : "
                + TexteUnicode.StripPython(resultat.Stderr));
        }
        return resultat.Stdout;
    }
}
