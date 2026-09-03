using System.Text.Json.Serialization;

namespace FauxGh;

/// Une invocation du faux `gh`, telle qu'écrite dans le journal : le vecteur
/// d'arguments et le contenu lu sur l'entrée standard — jamais d'horodatage
/// ni de chemin absolu, seuls éléments qui rendraient deux captures de la
/// même entrée non rejouables à l'identique.
internal sealed record EntreeJournal(
    [property: JsonPropertyName("args")] string[] Args,
    [property: JsonPropertyName("stdin")] string Stdin);
