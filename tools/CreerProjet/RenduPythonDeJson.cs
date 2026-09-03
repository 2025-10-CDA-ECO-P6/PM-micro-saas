using System.Text;
using System.Text.Json;
using CreerIssues;

namespace CreerProjet;

/// <summary>
/// Rendu d'un <see cref="JsonElement"/> comme le ferait l'interpolation
/// f-string Python d'une valeur décodée par <c>json.loads</c> (<c>f"{valeur}"</c>
/// appelle <c>str()</c>, qui reprend le rendu de <c>repr()</c> sur chaque
/// chaîne et chaque conteneur qu'elle porte) : guillemet simple par défaut
/// pour une chaîne (bascule sur le double via <see cref="ReprPython"/>),
/// <c>True</c>/<c>False</c>/<c>None</c> pour un booléen ou un null, ordre des
/// clés d'objet préservé (celui du document JSON source, comme
/// <c>json.loads</c> préserve l'ordre d'insertion). Site unique : le message
/// d'erreur GraphQL de <see cref="Graphql"/> (<c>reponse['errors']</c> côté
/// Python) — éprouvé sur des valeurs textuelles simples uniquement, jamais
/// sur un nombre à virgule flottante (voir le rapport de portage).
/// </summary>
internal static class RenduPythonDeJson
{
    public static string Rendre(JsonElement valeur) => valeur.ValueKind switch
    {
        JsonValueKind.String => ReprPython.Chaine(valeur.GetString() ?? string.Empty),
        JsonValueKind.True => "True",
        JsonValueKind.False => "False",
        JsonValueKind.Null => "None",
        JsonValueKind.Array => RendreListe(valeur),
        JsonValueKind.Object => RendreObjet(valeur),
        // Nombre : la forme textuelle du document JSON source, jamais
        // recalculée — non éprouvé face à la mise en forme d'un flottant
        // Python (repr(1.0) == "1.0", une notation exponentielle Python et
        // JSON pouvant diverger).
        _ => valeur.GetRawText(),
    };

    private static string RendreListe(JsonElement tableau)
    {
        var sb = new StringBuilder("[");
        bool premier = true;
        foreach (var element in tableau.EnumerateArray())
        {
            if (!premier) sb.Append(", ");
            premier = false;
            sb.Append(Rendre(element));
        }
        sb.Append(']');
        return sb.ToString();
    }

    private static string RendreObjet(JsonElement objet)
    {
        var sb = new StringBuilder("{");
        bool premier = true;
        foreach (var propriete in objet.EnumerateObject())
        {
            if (!premier) sb.Append(", ");
            premier = false;
            sb.Append(ReprPython.Chaine(propriete.Name)).Append(": ").Append(Rendre(propriete.Value));
        }
        sb.Append('}');
        return sb.ToString();
    }
}
