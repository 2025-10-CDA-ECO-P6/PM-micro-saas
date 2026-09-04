using System.Text.RegularExpressions;

namespace VerifCouvertureRecette;

/// <summary>
/// Découpage minimal d'une ligne de tableau Markdown en cellules, et
/// reconnaissance de la ligne séparatrice <c>|---|---|</c> — partagés par les
/// deux lectures de tableau de cet outil (cahier de recette, vue des épiques
/// du plan) plutôt que dupliqués deux fois pour la même syntaxe.
/// </summary>
internal static class TableauMarkdown
{
    private static readonly Regex MotifLigneSeparatrice = new(
        @"^\|[\s:|-]+\|$", RegexOptions.CultureInvariant);

    public static bool EstLigneSeparatrice(string ligne) => MotifLigneSeparatrice.IsMatch(ligne);

    /// <summary>
    /// Une ligne de tableau commence et finit par <c>|</c> : le split brut sur
    /// l'intérieur produit une cellule par colonne, sans les deux cellules
    /// vides que produirait un split de la ligne entière.
    /// </summary>
    public static string[] DecouperCellules(string ligneTableau)
    {
        string interieur = ligneTableau[1..^1];
        return interieur.Split('|').Select(c => c.Trim()).ToArray();
    }
}
