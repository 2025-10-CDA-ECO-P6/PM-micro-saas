using System.Diagnostics;

namespace GardeIdentiteGh;

/// <summary>
/// Établit — ou refuse d'établir — que le « gh » résolu depuis PATH est bien
/// le faux gh de ce harnais. La preuve vient du programme lui-même (sa
/// réponse à un argument sentinelle que le vrai gh ne peut pas satisfaire),
/// jamais d'une comparaison de chemins : un chemin ne voit ni une résolution
/// par le shell, ni un alias, ni un lien.
///
/// Échoue fermé : toute impossibilité d'établir la preuve — y compris une
/// cause imprévue à l'écriture de cette garde — vaut refus, jamais passage.
/// Invoque cette garde avant toute exécution sous harnais qui doit avoir un
/// effet réel (--appliquer) : un refus signifie que l'exécution qui devait
/// suivre ne doit jamais avoir lieu.
/// </summary>
internal static class VerificateurIdentite
{
    private const int DelaiMaxMillisecondes = 10_000;

    public static bool GhResoluEstLeFaux(TextWriter diagnostic)
    {
        try
        {
            return TenterVerification(diagnostic);
        }
        catch (Exception erreur)
        {
            // Échec fermé : une cause non prévue (binaire introuvable,
            // permission refusée, pipe rompu…) vaut refus, jamais passage —
            // c'est le mode de défaillance que cette garde existe pour
            // exclure, pas une exception à sa règle.
            diagnostic.WriteLine($"refus : exception imprévue lors de l'appel à « gh » — {erreur.Message}");
            return false;
        }
    }

    private static bool TenterVerification(TextWriter diagnostic)
    {
        var demarrage = new ProcessStartInfo
        {
            FileName = "gh",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            RedirectStandardInput = true,
            UseShellExecute = false,
        };
        demarrage.ArgumentList.Add(IdentiteSentinelle.Argument);

        using Process? processus = Process.Start(demarrage);
        if (processus is null)
        {
            diagnostic.WriteLine("refus : le système n'a pas pu démarrer de processus « gh » résolu depuis PATH");
            return false;
        }

        // Fermée aussitôt : rien à lui envoyer, et un binaire qui attendrait
        // la fin de l'entrée standard avant de répondre ne doit pas bloquer
        // cette vérification.
        processus.StandardInput.Close();

        // Lues en tâche de fond pendant l'attente de fin de processus :
        // vider un canal jusqu'au bout avant l'autre peut bloquer
        // indéfiniment si l'autre remplit son tampon pendant ce temps.
        Task<string> tacheSortie = processus.StandardOutput.ReadToEndAsync();
        Task<string> tacheErreur = processus.StandardError.ReadToEndAsync();

        if (!processus.WaitForExit(DelaiMaxMillisecondes))
        {
            TenterTuer(processus);
            diagnostic.WriteLine("refus : le « gh » résolu depuis PATH n'a pas rendu la main dans le délai imparti");
            return false;
        }

        string sortieStandard = tacheSortie.GetAwaiter().GetResult();
        // Consommée pour ne pas laisser le tampon bloquer le processus,
        // jamais interprétée : seule la sortie standard porte la preuve.
        tacheErreur.GetAwaiter().GetResult();

        if (processus.ExitCode != 0)
        {
            diagnostic.WriteLine(
                $"refus : code de sortie {processus.ExitCode} sur l'argument sentinelle (attendu 0)");
            return false;
        }

        if (sortieStandard != IdentiteSentinelle.Reponse)
        {
            diagnostic.WriteLine(
                "refus : la réponse à l'argument sentinelle ne correspond pas à celle du faux gh de ce harnais");
            return false;
        }

        diagnostic.WriteLine("identité établie : le « gh » résolu depuis PATH est le faux gh de ce harnais.");
        return true;
    }

    private static void TenterTuer(Process processus)
    {
        try
        {
            processus.Kill(entireProcessTree: true);
        }
        catch (Exception)
        {
            // Le processus a pu se terminer entre-temps — sans incidence sur
            // le verdict, déjà arbitré au refus par le délai dépassé.
        }
    }
}
