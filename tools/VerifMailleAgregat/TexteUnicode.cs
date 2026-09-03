using System.Text;

namespace VerifMailleAgregat;

/// <summary>
/// Équivalents point-de-code-par-point-de-code de primitives Python (<c>str.strip()</c>,
/// la traduction universelle des sauts de ligne), qui ne coïncident pas exactement
/// avec leurs équivalents .NET les plus proches. Itère par <see cref="Rune"/> (point
/// de code), jamais par <see langword="char"/> (unité UTF-16).
/// </summary>
internal static class TexteUnicode
{
    /// <summary>
    /// Vrai pour un point de code où <c>str.isspace()</c> de Python rend vrai :
    /// les catégories Unicode Zs/Zl/Zp, plus un jeu explicite de contrôles ASCII
    /// (dont U+001C–U+001F, absents de la notion .NET d'espace).
    /// </summary>
    private static bool EstEspacePython(Rune caractere)
    {
        int v = caractere.Value;
        if (v is 0x09 or 0x0A or 0x0B or 0x0C or 0x0D or 0x1C or 0x1D or 0x1E or 0x1F or 0x20 or 0x85)
        {
            return true;
        }
        var categorie = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(v);
        return categorie is System.Globalization.UnicodeCategory.SpaceSeparator
            or System.Globalization.UnicodeCategory.LineSeparator
            or System.Globalization.UnicodeCategory.ParagraphSeparator;
    }

    /// <summary>
    /// Équivalent de <c>str.strip()</c> sans argument : <see cref="string.Trim()"/>
    /// de .NET retiendrait un jeu de blancs plus étroit (sans U+001C–U+001F), ce
    /// qui déplacerait la frontière d'une clé ou d'une valeur de champ commençant
    /// ou finissant par l'un de ces caractères.
    /// </summary>
    public static string StripPython(string texte)
    {
        var runes = texte.EnumerateRunes().ToList();
        int debut = 0, fin = runes.Count;
        while (debut < fin && EstEspacePython(runes[debut])) debut++;
        while (fin > debut && EstEspacePython(runes[fin - 1])) fin--;

        var resultat = new System.Text.StringBuilder();
        for (int i = debut; i < fin; i++) resultat.Append(runes[i]);
        return resultat.ToString();
    }

    /// <summary>
    /// Convertit un retour chariot isolé (non suivi d'un saut de ligne) en saut
    /// de ligne, seul cas que la lecture texte universelle de Python traduit
    /// silencieusement et qu'aucune lecture .NET brute ne traduit. Une paire
    /// <c>\r\n</c> n'est jamais touchée ici — les motifs de ce fichier
    /// consomment un <c>\r</c> résiduel via <c>\s*</c> en fin de ligne, donc
    /// laisser la paire intacte ne change aucun résultat produit.
    /// </summary>
    public static string ConvertirRetourChariotIsole(string texte)
    {
        if (!texte.Contains('\r')) return texte;

        var resultat = new System.Text.StringBuilder(texte.Length);
        for (int i = 0; i < texte.Length; i++)
        {
            char c = texte[i];
            if (c == '\r' && (i + 1 >= texte.Length || texte[i + 1] != '\n'))
            {
                resultat.Append('\n');
            }
            else
            {
                resultat.Append(c);
            }
        }
        return resultat.ToString();
    }

    /// <summary>
    /// Équivalent de <c>str.format</c> avec spécificateur <c>{:N}</c> (largeur fixe,
    /// alignement à gauche) : remplit à droite jusqu'à N points de code, pas N unités
    /// UTF-16 — <see cref="string.PadRight(int)"/> compterait pour deux un caractère
    /// représenté par une paire de substituts en UTF-16, pour un seul en Python.
    /// </summary>
    public static string RemplirCodePoints(string texte, int largeur)
    {
        int nombreCodePoints = texte.EnumerateRunes().Count();
        return nombreCodePoints >= largeur
            ? texte
            : texte + new string(' ', largeur - nombreCodePoints);
    }
}
