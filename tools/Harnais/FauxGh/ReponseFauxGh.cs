using System.Text.Json;

namespace FauxGh;

/// Résout la réponse à rendre sur la sortie standard pour l'invocation
/// courante. Deux formes, dans cet ordre de priorité :
///
/// - `FAUX_GH_REPONSES` : chemin d'un fichier JSON portant un tableau de
///   chaînes — une réponse par rang d'appel (voir <see cref="RangAppel"/>),
///   la dernière entrée se répétant pour tout rang qui la dépasse. Permet à
///   une même exécution de servir des formes JSON incompatibles à des
///   appels successifs (un tableau nu à l'un, un objet à l'autre) — ce
///   qu'une réponse unique pour tout le processus ne peut jamais faire.
/// - à défaut, `FAUX_GH_SORTIE` (réponse unique pour toute l'exécution,
///   verbatim) — ou, à défaut aussi, `[]`.
///
/// `FAUX_GH_REPONSES` positionnée vers un fichier absent, illisible, ou dont
/// le contenu ne se décode pas en tableau non vide de chaînes : erreur
/// explicite — jamais un repli silencieux vers `FAUX_GH_SORTIE`, qui
/// masquerait une configuration de test cassée plutôt que de la signaler.
internal static class ReponseFauxGh
{
    public static string Resoudre(int rang)
    {
        string? cheminReponses = Environment.GetEnvironmentVariable("FAUX_GH_REPONSES");
        if (string.IsNullOrEmpty(cheminReponses))
        {
            return Environment.GetEnvironmentVariable("FAUX_GH_SORTIE") ?? "[]";
        }

        string[] reponses = LireReponses(cheminReponses);
        if (reponses.Length == 0)
        {
            throw new InvalidOperationException(
                $"FAUX_GH_REPONSES ({cheminReponses}) porte un tableau vide — au moins une réponse est requise");
        }

        int index = Math.Min(rang, reponses.Length - 1);
        return reponses[index];
    }

    private static string[] LireReponses(string cheminReponses)
    {
        try
        {
            return JsonSerializer.Deserialize<string[]>(File.ReadAllText(cheminReponses))
                ?? throw new InvalidOperationException("le tableau JSON décodé vaut « null »");
        }
        catch (Exception erreur) when (erreur is IOException or JsonException or InvalidOperationException)
        {
            throw new InvalidOperationException(
                $"FAUX_GH_REPONSES ({cheminReponses}) illisible ou mal formé : {erreur.Message}", erreur);
        }
    }
}
