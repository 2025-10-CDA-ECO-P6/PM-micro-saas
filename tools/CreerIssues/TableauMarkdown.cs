namespace CreerIssues;

/// <summary>
/// Lecture générique d'un tableau Markdown à deux colonnes — la forme que
/// portent aussi bien les champs d'une tâche du plan que les valeurs d'un axe
/// de la taxonomie de libellés. Fait partie du socle « lecture du plan » :
/// <see cref="PlanDeTravail"/> en dépend directement ; <c>Taxonomie</c> (hors
/// socle isolé, lecture de `methode-de-ticket.md`) le réutilise aussi, sans
/// que cela retire quoi que ce soit à l'indépendance du socle lui-même — rien
/// ici ne dépend en retour de <c>Taxonomie</c> ni du reste de l'outil.
/// </summary>
internal static class TableauMarkdown
{
    /// <summary>
    /// Renvoie les paires (colonne 1, colonne 2) des lignes de tableau
    /// Markdown d'un bloc de texte, en-tête et ligne de séparation exclues.
    /// </summary>
    public static List<(string Premiere, string Seconde)> AnalyserLignes(string bloc)
    {
        var lignes = new List<(string, string)>();
        foreach (var ligneBrute in TexteUnicode.SepererLignesPython(bloc))
        {
            string ligne = TexteUnicode.StripPython(ligneBrute);
            if (!(ligne.StartsWith('|') && ligne.EndsWith('|'))) continue;

            var cellules = ligne[1..^1].Split('|').Select(TexteUnicode.StripPython).ToList();
            if (cellules.Count < 2) continue;

            string premiere = cellules[0];
            bool estSeparateur = premiere.Length > 0 && premiere.EnumerateRunes().All(r => r.Value == '-');
            if (premiere is "Champ" or "Valeur" || estSeparateur) continue;

            lignes.Add((cellules[0], cellules[1]));
        }
        return lignes;
    }
}
