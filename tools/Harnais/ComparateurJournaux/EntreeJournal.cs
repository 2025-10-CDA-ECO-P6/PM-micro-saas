using System.Text.Json.Serialization;

namespace ComparateurJournaux;

/// Une invocation du faux gh, telle que lue depuis une ligne du journal —
/// forme miroir de `FauxGh.EntreeJournal`, dupliquée plutôt que partagée : ce
/// dépôt ne fait pas référencer un projet d'outillage par un autre (aucun
/// fichier de solution, aucune référence de projet entre les deux).
internal sealed record EntreeJournal(
    [property: JsonPropertyName("args")] string[] Args,
    [property: JsonPropertyName("stdin")] string Stdin);
