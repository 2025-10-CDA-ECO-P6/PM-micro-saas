using System.Text;

namespace VerifMailleAgregat;

/// <summary>
/// Lecture de fichier en UTF-8 strict et non consommateur de marque d'ordre
/// d'octets : throwOnInvalidBytes lève sur toute séquence invalide, là où le
/// décodage .NET par défaut la remplacerait silencieusement par U+FFFD ; et
/// aucune détection d'encodage par en-tête n'efface un U+FEFF de tête.
/// </summary>
internal static class LecteurTexte
{
    private static readonly UTF8Encoding Utf8Strict = new(
        encoderShouldEmitUTF8Identifier: false,
        throwOnInvalidBytes: true);

    /// <summary>
    /// Équivalent de <c>open(chemin, encoding="utf-8").read()</c> : décodage UTF-8
    /// strict puis conversion du seul retour chariot isolé, comme la lecture
    /// texte universelle de Python.
    /// </summary>
    public static string LireTexteIntegral(string chemin)
    {
        byte[] octets = File.ReadAllBytes(chemin);
        string texte = Utf8Strict.GetString(octets);
        return TexteUnicode.ConvertirRetourChariotIsole(texte);
    }

    /// <summary>
    /// Équivalent de l'itération ligne à ligne d'<c>open(chemin, encoding="utf-8")</c> :
    /// <see cref="StreamReader.ReadLine"/> reconnaît déjà les trois séparateurs
    /// (<c>\r\n</c>, <c>\r</c>, <c>\n</c>) de la lecture universelle de Python, sans
    /// conversion préalable nécessaire.
    /// </summary>
    public static IEnumerable<string> LireLignes(string chemin)
    {
        using var flux = File.OpenRead(chemin);
        // detectEncodingFromByteOrderMarks: false — cohérent avec LireTexteIntegral.
        using var lecteur = new StreamReader(flux, Utf8Strict, detectEncodingFromByteOrderMarks: false);
        string? ligne;
        while ((ligne = lecteur.ReadLine()) is not null)
        {
            yield return ligne;
        }
    }
}
