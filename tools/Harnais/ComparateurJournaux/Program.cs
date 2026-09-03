// Cet outil n'est pas un vérificateur du corpus documentaire : c'est un
// harnais de test qui compare deux journaux d'invocations du faux gh
// (`../FauxGh`), pour le portage Python → C# de `tools/creer-issues.py` et
// `tools/creer-projet.py`. Il ne relève donc pas de la maille de désignation
// du plan de travail (`plan-de-travail.md §2`) — ce n'est pas lui-même une
// tâche du plan. Des conventions de code C# du corpus
// (guide-conventions-et-dod.md §2), n'applique que les clauses dont l'objet
// nommé existe ici — le style et la version du SDK, jamais le modèle de
// domaine.

using System.Globalization;
using System.Text;
using ComparateurJournaux;

CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

using var sortie = new StreamWriter(Console.OpenStandardOutput(), new UTF8Encoding(encoderShouldEmitUTF8Identifier: false))
{
    NewLine = "\n",
};

try
{
    if (args.Length != 2)
    {
        throw new PanneExecutionException(
            "usage : ComparateurJournaux <journal-A> <journal-B>");
    }

    var journalA = LecteurJournal.Lire(args[0]);
    var journalB = LecteurJournal.Lire(args[1]);

    var verdict = Comparateur.Executer(journalA, journalB, sortie);
    sortie.Flush();
    return verdict.ADesDefauts ? 1 : 0;
}
catch (PanneExecutionException panne)
{
    sortie.Flush();
    Console.Error.WriteLine($"panne d'exécution : {panne.Message}");
    return 2;
}
catch (Exception imprevue)
{
    // Filet de sécurité : toute panne non anticipée rend tout de même un
    // code de sortie du contrat arbitré plutôt qu'une trace non gérée.
    sortie.Flush();
    Console.Error.WriteLine($"panne d'exécution imprévue : {imprevue.Message}");
    return 2;
}
