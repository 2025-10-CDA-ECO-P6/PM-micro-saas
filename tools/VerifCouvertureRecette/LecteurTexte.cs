using System.Text;

namespace VerifCouvertureRecette;

/// <summary>
/// Lecture de fichier en UTF-8 strict et non consommateur de marque d'ordre
/// d'octets : throwOnInvalidBytes lève sur toute séquence invalide, là où le
/// décodage .NET par défaut la remplacerait silencieusement par U+FFFD — une
/// substitution muette est exactement ce qui ferait manquer un identifiant
/// accentué mal encodé plutôt que de signaler une panne d'exécution.
/// </summary>
internal static class LecteurTexte
{
    private static readonly UTF8Encoding Utf8Strict = new(
        encoderShouldEmitUTF8Identifier: false,
        throwOnInvalidBytes: true);

    public static string LireTexteIntegral(string chemin)
    {
        byte[] octets = File.ReadAllBytes(chemin);
        try
        {
            return Utf8Strict.GetString(octets);
        }
        catch (DecoderFallbackException erreur)
        {
            throw new PanneExecutionException($"UTF-8 invalide dans {chemin}", erreur);
        }
    }
}
