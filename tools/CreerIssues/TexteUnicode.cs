using System.Globalization;
using System.Text;

namespace CreerIssues;

/// <summary>
/// Classifications et primitives Unicode point-de-code-par-point-de-code
/// alignées sur les définitions Python (<c>str.strip()</c>, <c>str.isalnum()</c>,
/// la traduction universelle des sauts de ligne), qui ne coïncident pas
/// exactement avec leurs équivalents .NET les plus proches. Forme miroir de
/// <c>VerifCorpus.TexteUnicode</c> et <c>VerifMailleAgregat.TexteUnicode</c>,
/// dupliquée plutôt que partagée — ce dépôt ne fait jamais référencer un
/// projet d'outillage par un autre. Fait partie du socle « lecture du plan » :
/// <see cref="PlanDeTravail"/> et <see cref="MailleDuPlan"/> en dépendent, et
/// la vague suivante (report vers un projet GitHub) en aura besoin de la
/// même façon.
/// </summary>
internal static class TexteUnicode
{
    private static readonly HashSet<UnicodeCategory> CategoriesAlphanumeriques = new()
    {
        // Lu, Ll, Lt, Lm, Lo
        UnicodeCategory.UppercaseLetter,
        UnicodeCategory.LowercaseLetter,
        UnicodeCategory.TitlecaseLetter,
        UnicodeCategory.ModifierLetter,
        UnicodeCategory.OtherLetter,
        // Nd, Nl, No — c'est précisément ce que char.IsLetterOrDigit ne couvre
        // pas (il s'arrête à Nd) ; str.isalnum() de Python couvre les trois.
        UnicodeCategory.DecimalDigitNumber,
        UnicodeCategory.LetterNumber,
        UnicodeCategory.OtherNumber,
    };

    /// <summary>
    /// Vrai pour un point de code où <c>str.isalnum()</c> de Python rend vrai.
    /// C'est cette appartenance — pas la notion de caractère de mot du moteur
    /// .NET (<c>\w</c>, qui exclut Nl/No et inclut les marques combinantes) —
    /// que suit la frontière de mot Python reproduite par
    /// <see cref="LimiteMotPython"/>.
    /// </summary>
    public static bool EstAlphanumeriquePython(Rune caractere) =>
        CategoriesAlphanumeriques.Contains(CharUnicodeInfo.GetUnicodeCategory(caractere.Value));

    /// <summary>
    /// Vrai pour un point de code où <c>str.isspace()</c> de Python rend vrai :
    /// les catégories Unicode Zs/Zl/Zp, plus un jeu explicite de contrôles ASCII
    /// (dont U+001C–U+001F, absents de la notion .NET d'espace et de la classe
    /// <c>\s</c> du moteur de expressions régulières .NET).
    /// </summary>
    public static bool EstEspacePython(Rune caractere)
    {
        int v = caractere.Value;
        if (v is 0x09 or 0x0A or 0x0B or 0x0C or 0x0D or 0x1C or 0x1D or 0x1E or 0x1F or 0x20 or 0x85)
        {
            return true;
        }
        var categorie = CharUnicodeInfo.GetUnicodeCategory(v);
        return categorie is UnicodeCategory.SpaceSeparator
            or UnicodeCategory.LineSeparator
            or UnicodeCategory.ParagraphSeparator;
    }

    /// <summary>
    /// Équivalent de <c>str.strip()</c> sans argument : <see cref="string.Trim()"/>
    /// de .NET retiendrait un jeu de blancs plus étroit (sans U+001C–U+001F), ce
    /// qui déplacerait la frontière d'une clé ou d'une valeur de champ, ou d'une
    /// cellule de tableau, commençant ou finissant par l'un de ces caractères.
    /// </summary>
    public static string StripPython(string texte)
    {
        var runes = texte.EnumerateRunes().ToList();
        int debut = 0, fin = runes.Count;
        while (debut < fin && EstEspacePython(runes[debut])) debut++;
        while (fin > debut && EstEspacePython(runes[fin - 1])) fin--;

        var resultat = new StringBuilder();
        for (int i = debut; i < fin; i++) resultat.Append(runes[i]);
        return resultat.ToString();
    }

    /// <summary>
    /// Équivalent de <c>str.splitlines()</c> : reconnaît huit frontières de
    /// ligne (<c>\n</c>, <c>\r</c>, <c>\r\n</c> compté une seule fois, <c>\v</c>,
    /// <c>\f</c>, U+001C–U+001E, U+0085, U+2028, U+2029) — un jeu strictement
    /// plus large que le seul <c>\n</c> que <see cref="string.Split(char[])"/>
    /// reconnaîtrait. Mesuré : un blanc de contrôle U+001C-U+001E au milieu
    /// d'une cellule de tableau ou d'une puce du §2 y coupe une « ligne »
    /// Python en deux avant même l'analyse de son contenu — un écart qui ne
    /// se limite pas à la classe de blanc d'une expression régulière, décelé
    /// en le mesurant plutôt qu'en le supposant négligeable. Aucune chaîne
    /// vide finale n'est produite pour un texte se terminant par une frontière,
    /// comme <c>str.splitlines()</c>.
    /// </summary>
    public static List<string> SepererLignesPython(string texte)
    {
        var resultat = new List<string>();
        int debut = 0, i = 0;
        while (i < texte.Length)
        {
            char c = texte[i];
            if (c is '\n' or '\r' or '\v' or '\f' or '\x1c' or '\x1d' or '\x1e' or '\x85' or '\u2028' or '\u2029')
            {
                resultat.Add(texte[debut..i]);
                i++;
                if (c == '\r' && i < texte.Length && texte[i] == '\n') i++; // \r\n compté une seule fois
                debut = i;
                continue;
            }
            i++;
        }
        if (debut < texte.Length)
        {
            resultat.Add(texte[debut..]);
        }
        return resultat;
    }

    /// <summary>
    /// Convertit un retour chariot isolé (non suivi d'un saut de ligne) en saut
    /// de ligne, seul cas que la lecture texte universelle de Python traduit
    /// silencieusement et qu'aucune lecture .NET brute ne traduit. Une paire
    /// <c>\r\n</c> n'est jamais touchée ici.
    /// </summary>
    public static string ConvertirRetourChariotIsole(string texte)
    {
        if (!texte.Contains('\r')) return texte;

        var resultat = new StringBuilder(texte.Length);
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
}
