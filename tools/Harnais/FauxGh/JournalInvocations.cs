using System.Text;
using System.Text.Json;

namespace FauxGh;

/// Écrit une entrée de journal par invocation du faux gh, un objet JSON par
/// ligne, dans le fichier désigné par la variable d'environnement
/// `JOURNAL_GH`. Cette variable absente : aucune écriture — le faux gh reste
/// utilisable sans journalisation (exploration manuelle, par exemple).
internal static class JournalInvocations
{
    private static readonly JsonSerializerOptions Options = new() { WriteIndented = false };

    public static void Enregistrer(string? chemin, string[] arguments, string entreeStandard)
    {
        if (string.IsNullOrEmpty(chemin))
        {
            return;
        }

        string ligne = JsonSerializer.Serialize(new EntreeJournal(arguments, entreeStandard), Options);

        // Une invocation = un processus qui se termine aussitôt après avoir
        // écrit sa ligne : les invocations successives d'un même journal ne
        // se recouvrent jamais dans le temps, l'ajout en fin de fichier
        // suffit sans verrou ni ouverture partagée.
        File.AppendAllText(chemin, ligne + "\n", new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
    }
}
