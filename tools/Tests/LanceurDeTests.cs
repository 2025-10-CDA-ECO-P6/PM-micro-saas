using System.Diagnostics;
using System.Reflection;

namespace CaracterisationOutils;

/// <summary>
/// Lanceur de la suite : découverte, compilation des outils, exécution en série,
/// compte, code de sortie. Codes : 0 tout passe ; 1 au moins un test échoue ;
/// 2 panne du lanceur (outil non compilé, aucun test découvert, construction non
/// prise en charge, option inconnue) — jamais confondue avec un échec de test.
/// </summary>
internal static class LanceurDeTests
{
    public static int Executer(string[] args)
    {
        if (args.Length > 1 || (args.Length == 1 && args[0] != "--lister"))
        {
            Console.Error.WriteLine("usage : dotnet run --file tools/Tests/Tests.cs [-- --lister]");
            return 2;
        }

        IReadOnlyList<CasDeTest> cas;
        try
        {
            cas = DecouverteDeTests.Decouvrir(typeof(LanceurDeTests).Assembly);
        }
        catch (NotSupportedException e)
        {
            Console.Error.WriteLine($"Découverte impossible : {e.Message}");
            return 2;
        }
        if (cas.Count == 0)
        {
            Console.Error.WriteLine("Aucun test découvert : un compte nul n'est pas un succès.");
            return 2;
        }

        if (args.Length == 1)
        {
            foreach (var c in cas) Console.WriteLine(c.Nom);
            return 0;
        }

        string? panne = CompilationDesOutils.Compiler();
        if (panne is not null)
        {
            Console.Error.WriteLine(panne);
            return 2;
        }

        var chrono = Stopwatch.StartNew();
        int reussis = 0;
        var echecs = new List<(CasDeTest Cas, Exception Erreur)>();
        foreach (var c in cas)
        {
            var erreur = ExecutionDeTests.Executer(c);
            if (erreur is null) reussis++;
            else echecs.Add((c, erreur));
        }
        chrono.Stop();

        foreach (var (c, erreur) in echecs)
        {
            Console.WriteLine($"[ÉCHEC] {c.Nom}");
            Console.WriteLine($"  {erreur.GetType().Name} : {erreur.Message.ReplaceLineEndings("\n  ")}");
        }
        // Ligne de compte stable : la garde du dépôt la lit par le motif
        // `[Tt]otal[[:space:]]*:[[:space:]]*[0-9]+`. Aucune autre ligne n'y ressemble.
        Console.WriteLine(
            $"{(echecs.Count == 0 ? "Réussi" : "ÉCHEC")} — Réussis: {reussis}, Échoués: {echecs.Count}, Total: {cas.Count}, Durée: {(int)chrono.Elapsed.TotalSeconds} s");
        return echecs.Count == 0 ? 0 : 1;
    }
}
