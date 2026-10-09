using System.Text.RegularExpressions;

namespace VerifCouvertureRecette;

/// <summary>
/// Lecture de la section « Vue des épiques » du plan de travail : associe chaque
/// épique portée par un use case (<c>US-UC-NN</c>) au jalon lu dans sa ligne
/// de tableau. Une épique <c>EP-nn</c> (aucun use case ne la porte) est
/// également indexée mais n'est jamais interrogée par ce contrôle — seule la
/// couverture des cas de recette, dérivée des UC, l'intéresse.
/// </summary>
internal static class VueDesEpiques
{
    // La section se reconnaît à son intitulé, pas au niveau de son titre : le
    // numéro d'ordre est facultatif, le niveau (1 à 6 dièses) est relevé pour
    // borner la section.
    private static readonly Regex MotifDebutSection = new(
        @"^(?<niveau>#{1,6})[ \t]+(?:\d+\.[ \t]+)?Vue des épiques[ \t\r]*$",
        RegexOptions.Multiline | RegexOptions.CultureInvariant);

    private static readonly Regex MotifIdentifiantEpique = new(
        @"^(US-UC-\d+|EP-\d+)\b", RegexOptions.CultureInvariant);

    /// <summary>
    /// Clé : identifiant d'épique (<c>US-UC-01</c>, <c>EP-04</c>…). Valeur : le
    /// contenu brut de sa colonne <c>Jalon</c> (<c>J0</c>, <c>J1</c>, <c>J0-J1</c>,
    /// <c>J2</c>, <c>J3</c>…), non interprété ici — à charge de l'appelant d'en
    /// décider le sens (courant vs ultérieur).
    /// </summary>
    public static Dictionary<string, string> ExtraireJalonParEpique(string texte)
    {
        var debut = MotifDebutSection.Match(texte);
        if (!debut.Success)
        {
            throw new PanneExecutionException("section « Vue des épiques » introuvable dans le plan de travail");
        }

        int positionApresEntete = debut.Index + debut.Length;

        // Fin : le prochain titre de niveau égal ou supérieur à celui de la section.
        int niveau = debut.Groups["niveau"].Length;
        var motifFin = new Regex(@$"^#{{1,{niveau}}}[ \t]", RegexOptions.Multiline | RegexOptions.CultureInvariant);
        var fin = motifFin.Match(texte, positionApresEntete);
        string section = fin.Success ? texte[positionApresEntete..fin.Index] : texte[positionApresEntete..];

        string[] lignes = section.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
        var resultat = new Dictionary<string, string>(StringComparer.Ordinal);

        int colEpique = -1;
        int colJalon = -1;

        for (int i = 0; i < lignes.Length; i++)
        {
            string ligne = lignes[i].Trim();
            if (!ligne.StartsWith('|') || !ligne.EndsWith('|')) continue;
            if (TableauMarkdown.EstLigneSeparatrice(ligne)) continue;

            string[] cellules = TableauMarkdown.DecouperCellules(ligne);
            if (cellules.Length == 0) continue;

            bool estEntete = i + 1 < lignes.Length && TableauMarkdown.EstLigneSeparatrice(lignes[i + 1].Trim());
            if (estEntete)
            {
                colEpique = Array.FindIndex(cellules, c => c.Contains("Épique", StringComparison.Ordinal));
                colJalon = Array.FindIndex(cellules, c => c.Contains("Jalon", StringComparison.Ordinal));
                continue;
            }

            if (colEpique < 0 || colJalon < 0) continue;
            if (colEpique >= cellules.Length || colJalon >= cellules.Length) continue;

            var idMatch = MotifIdentifiantEpique.Match(cellules[colEpique]);
            if (!idMatch.Success) continue;

            resultat[idMatch.Groups[1].Value] = cellules[colJalon];
        }

        return resultat;
    }
}
