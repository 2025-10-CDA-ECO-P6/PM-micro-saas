using System.Globalization;
using System.Text;

namespace VerifMailleAgregat;

/// <summary>
/// Reproduit <c>repr()</c> de Python sur une chaîne et sur une liste de chaînes,
/// tel qu'interpolé par une f-string Python (<c>f"{une_liste}"</c> applique
/// <c>repr</c> à la liste, qui applique lui-même <c>repr</c> à chaque élément).
/// C'est ce rendu, et non une sérialisation JSON ni un <c>ToString()</c> de
/// collection .NET, que porte chaque ligne de détail de la sortie de référence.
/// </summary>
internal static class ReprPython
{
    public static string Liste(IReadOnlyList<string> items)
    {
        var sb = new StringBuilder("[");
        for (int i = 0; i < items.Count; i++)
        {
            if (i > 0) sb.Append(", ");
            sb.Append(Chaine(items[i]));
        }
        sb.Append(']');
        return sb.ToString();
    }

    /// <summary>
    /// Reproduit <c>repr(str)</c> : guillemet simple par défaut, bascule sur le
    /// guillemet double si la chaîne porte une apostrophe et aucun guillemet
    /// double. Un point de code non imprimable (au sens de <c>str.isprintable()</c>)
    /// est échappé en <c>\xXX</c>, <c>\uXXXX</c> ou <c>\UXXXXXXXX</c> selon sa
    /// plage ; un point de code imprimable, y compris accentué, reste tel quel.
    /// </summary>
    public static string Chaine(string texte)
    {
        char guillemet = texte.Contains('\'') && !texte.Contains('"') ? '"' : '\'';

        var sb = new StringBuilder();
        sb.Append(guillemet);
        foreach (var rune in texte.EnumerateRunes())
        {
            EchapperRune(sb, rune, guillemet);
        }
        sb.Append(guillemet);
        return sb.ToString();
    }

    private static void EchapperRune(StringBuilder sb, Rune rune, char guillemet)
    {
        if (rune.Value == '\\') { sb.Append("\\\\"); return; }
        if (rune.Value == guillemet) { sb.Append('\\').Append(guillemet); return; }
        if (rune.Value == '\n') { sb.Append("\\n"); return; }
        if (rune.Value == '\r') { sb.Append("\\r"); return; }
        if (rune.Value == '\t') { sb.Append("\\t"); return; }
        if (EstImprimablePython(rune)) { sb.Append(rune.ToString()); return; }

        if (rune.Value <= 0xFF) sb.Append("\\x").Append(rune.Value.ToString("x2"));
        else if (rune.Value <= 0xFFFF) sb.Append("\\u").Append(rune.Value.ToString("x4"));
        else sb.Append("\\U").Append(rune.Value.ToString("x8"));
    }

    /// <summary>
    /// Équivalent de <c>str.isprintable()</c> pour un point de code isolé : faux
    /// pour les catégories Unicode « Autre » (Cc/Cf/Cs/Co/Cn) et « Séparateur »
    /// (Zs/Zl/Zp), sauf l'espace ASCII U+0020, explicitement exempté par Python.
    /// </summary>
    private static bool EstImprimablePython(Rune rune)
    {
        if (rune.Value == 0x20) return true;
        return Rune.GetUnicodeCategory(rune) switch
        {
            UnicodeCategory.Control => false,
            UnicodeCategory.Format => false,
            UnicodeCategory.Surrogate => false,
            UnicodeCategory.PrivateUse => false,
            UnicodeCategory.OtherNotAssigned => false,
            UnicodeCategory.SpaceSeparator => false,
            UnicodeCategory.LineSeparator => false,
            UnicodeCategory.ParagraphSeparator => false,
            _ => true,
        };
    }
}
