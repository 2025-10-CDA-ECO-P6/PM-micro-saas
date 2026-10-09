namespace VerifMailleAgregat;

/// <summary>
/// Ce que la lecture du plan rend des tâches : les tâches elles-mêmes, par
/// identifiant, et les deux espèces de défaut de lecture que le vérificateur
/// compte plutôt que de sauter — un titre de niveau 4 à 6 qui commence par
/// <c>TB-</c> sans respecter la forme du plan (numéro de ligne, texte du titre),
/// et un identifiant défini plusieurs fois (numéros des lignes de ses titres).
/// </summary>
internal sealed record LectureDesTaches(
    Dictionary<string, Tache> Taches,
    List<(int Ligne, string Titre)> TitresNonConformes,
    List<(string Id, List<int> Lignes)> IdentifiantsEnDouble);
