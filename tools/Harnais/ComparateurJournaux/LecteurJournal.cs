using System.Text.Json;

namespace ComparateurJournaux;

/// Lit un journal d'invocations du faux gh : un objet JSON par ligne, dans
/// l'ordre d'écriture — cet ordre porte l'ordre d'appel réel, jamais
/// retrié.
internal static class LecteurJournal
{
    private static readonly JsonSerializerOptions Options = new() { PropertyNameCaseInsensitive = true };

    public static IReadOnlyList<EntreeJournal> Lire(string chemin)
    {
        string contenu;
        try
        {
            contenu = File.ReadAllText(chemin);
        }
        catch (IOException erreur)
        {
            throw new PanneExecutionException($"journal illisible ({chemin}) : {erreur.Message}");
        }
        catch (UnauthorizedAccessException erreur)
        {
            throw new PanneExecutionException($"journal illisible ({chemin}) : {erreur.Message}");
        }

        var entrees = new List<EntreeJournal>();
        int numeroLigne = 0;
        foreach (string ligne in contenu.Split('\n'))
        {
            numeroLigne++;
            if (ligne.Trim().Length == 0)
            {
                continue;
            }
            try
            {
                EntreeJournal? entree = JsonSerializer.Deserialize<EntreeJournal>(ligne, Options);
                if (entree is null)
                {
                    throw new PanneExecutionException($"{chemin}:{numeroLigne} : ligne JSON vide (« null »)");
                }
                entrees.Add(entree);
            }
            catch (JsonException erreur)
            {
                throw new PanneExecutionException($"{chemin}:{numeroLigne} : ligne JSON invalide — {erreur.Message}");
            }
        }
        return entrees;
    }
}
