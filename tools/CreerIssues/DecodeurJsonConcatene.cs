using System.Text;
using System.Text.Json;

namespace CreerIssues;

/// <summary>
/// `gh api --paginate` peut renvoyer plusieurs documents JSON concaténés
/// (une page par appel). On les décode un à un et on aplatit les listes,
/// sans dépendre d'aucun paquet tiers — équivalent du décodage manuel par
/// <c>json.JSONDecoder().raw_decode</c> côté Python.
/// </summary>
internal static class DecodeurJsonConcatene
{
    public static List<JsonElement> Decoder(string texte)
    {
        // Python ne saute, avant la boucle, que les quatre blancs ASCII
        // littéralement écrits dans le code (" \t\r\n") via str.strip() sans
        // argument sur le texte entier — TexteUnicode.StripPython() retire un
        // jeu plus large (Zs/Zl/Zp, U+001C-U+001F, U+0085) : reproduit tel
        // quel plutôt que restreint, cette étape ne visant qu'à retirer une
        // marge autour d'un flux dont le contenu utile reste un document
        // JSON, jamais un texte porteur de ces séparateurs de contrôle.
        byte[] octets = Encoding.UTF8.GetBytes(TexteUnicode.StripPython(texte));

        var documents = new List<JsonElement>();
        int position = 0;
        while (position < octets.Length)
        {
            while (position < octets.Length && EstBlancDeBoucle(octets[position])) position++;
            if (position >= octets.Length) break;

            var lecteur = new Utf8JsonReader(octets.AsSpan(position), isFinalBlock: true, state: default);
            using var document = JsonDocument.ParseValue(ref lecteur);
            documents.Add(document.RootElement.Clone());
            position += (int)lecteur.BytesConsumed;
        }

        var aplatis = new List<JsonElement>();
        foreach (var document in documents)
        {
            if (document.ValueKind == JsonValueKind.Array)
            {
                aplatis.AddRange(document.EnumerateArray());
            }
            else
            {
                aplatis.Add(document);
            }
        }
        return aplatis;
    }

    /// <summary>
    /// Les quatre blancs ASCII littéralement testés par la boucle Python
    /// (<c>" \t\r\n"</c>) — jamais <see cref="TexteUnicode.EstEspacePython"/>,
    /// plus large, qui n'est pas ce que cette boucle-ci teste.
    /// </summary>
    private static bool EstBlancDeBoucle(byte b) => b is (byte)' ' or (byte)'\t' or (byte)'\r' or (byte)'\n';
}
