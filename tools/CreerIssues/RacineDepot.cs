namespace CreerIssues;

/// <summary>
/// Localisation de la racine du dépôt git courant. Fait partie du socle
/// « lecture du plan » : sans dépendance à la construction de corps d'issue,
/// aux appels `gh`, ni à quoi que ce soit d'autre dans cet outil — la vague
/// suivante (report du plan vers un projet GitHub) lie ce fichier tel quel.
/// </summary>
internal static class RacineDepot
{
    /// <summary>
    /// Équivalent de <c>git rev-parse --show-toplevel</c>, résolu depuis le
    /// répertoire de travail courant du processus — comme <c>subprocess.run</c>
    /// côté Python, invoqué sans changer de répertoire.
    /// </summary>
    public static string Trouver() =>
        TexteUnicode.StripPython(Processus.ExecuterGit("rev-parse", "--show-toplevel"));
}
