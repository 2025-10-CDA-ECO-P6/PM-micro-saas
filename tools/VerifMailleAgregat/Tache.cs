namespace VerifMailleAgregat;

/// <summary>
/// Une tranche du plan de travail, telle qu'extraite de sa fiche : les items
/// de son « Périmètre d'écriture », les deux drapeaux d'exclusion de la maille,
/// et les identifiants référencés par ses champs « Dépend de » et « En conflit
/// avec ».
/// </summary>
internal sealed record Tache(
    List<string> Items,
    bool HorsMaille,
    bool Trou,
    HashSet<string> Conflits,
    HashSet<string> DependDe);
