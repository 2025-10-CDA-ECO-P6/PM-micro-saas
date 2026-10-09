using System.Globalization;
using System.Text;

namespace VerifMarqueursIdentifiants;

/// <summary>
/// Les écarts que le corpus porte déjà et que la liste de référence accepte,
/// une ligne par entrée : <c>clé|nombre</c>. Une ligne vide ou commençant par
/// <c>#</c> est ignorée. Le nombre est celui des constats que la clé a le droit
/// de porter : en trouver davantage est un écart nouveau, en trouver moins
/// signifie que la liste peut être réduite.
/// </summary>
internal static class ListeReference
{
    private static readonly UTF8Encoding Utf8Strict = new(
        encoderShouldEmitUTF8Identifier: false,
        throwOnInvalidBytes: true);

    /// <summary>Un fichier absent vaut une liste vide : tout écart est alors nouveau.</summary>
    public static Dictionary<string, int> Lire(string chemin)
    {
        var acceptes = new Dictionary<string, int>(StringComparer.Ordinal);
        if (!File.Exists(chemin)) return acceptes;

        string texte;
        try
        {
            texte = Utf8Strict.GetString(File.ReadAllBytes(chemin));
        }
        catch (Exception cause) when (cause is DecoderFallbackException or IOException)
        {
            throw new PanneExecutionException($"liste de référence illisible ({chemin}) : {cause.Message}", cause);
        }

        int numero = 0;
        foreach (var brute in texte.Split('\n'))
        {
            numero++;
            string ligne = brute.Trim();
            if (ligne.Length == 0 || ligne.StartsWith('#')) continue;

            int separateur = ligne.LastIndexOf('|');
            if (separateur <= 0
                || !int.TryParse(ligne[(separateur + 1)..], NumberStyles.None, CultureInfo.InvariantCulture, out int nombre)
                || nombre < 1)
            {
                throw new PanneExecutionException($"{chemin}:{numero} : ligne attendue « clé|nombre », lu « {ligne} »");
            }
            acceptes[ligne[..separateur]] = nombre;
        }
        return acceptes;
    }
}
