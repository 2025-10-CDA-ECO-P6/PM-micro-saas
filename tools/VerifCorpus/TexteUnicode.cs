using System.Globalization;
using System.Text;

namespace VerifCorpus;

/// <summary>
/// Classifications Unicode point-de-code-par-point-de-code alignées sur les
/// définitions Python (<c>str.isspace()</c>, <c>str.isalnum()</c>), qui ne
/// coïncident pas exactement avec leurs équivalents .NET les plus proches.
/// Toutes deux itèrent par <see cref="Rune"/> (point de code), jamais par
/// <see langword="char"/> (unité UTF-16) : un caractère supplémentaire itéré en
/// <see langword="char"/> se présenterait comme deux demi-substituts, chacun
/// hors de toute catégorie alphanumérique.
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

    public static bool EstAlphanumeriquePython(Rune caractere) =>
        CategoriesAlphanumeriques.Contains(CharUnicodeInfo.GetUnicodeCategory(caractere.Value));

    // La propriété Unicode Cased, qu'emploie la règle Final_Sigma ci-dessous
    // pour reconnaître une « lettre à casse », dépasse Lu/Ll/Lt : elle inclut
    // aussi Other_Uppercase et Other_Lowercase, une extension donnée en
    // plages de points de code plutôt qu'en catégories générales — un point
    // de code de ces plages peut relever d'une catégorie hors Lu/Ll/Lt (Lo,
    // Nl, So) et rester néanmoins une lettre à casse pour cette règle. Lm
    // (lettre modificative) n'y figure jamais : mesuré exhaustivement, tout
    // point de code Lm est traversé comme indifférent à la casse par
    // EstIndifferentCasse ci-dessous, y compris quand il porte par ailleurs
    // Other_Lowercase (U+02B0).
    private static readonly (int Debut, int Fin)[] PlagesOtherCased =
    {
        (0x00AA, 0x00AA), // FEMININE ORDINAL INDICATOR
        (0x00BA, 0x00BA), // MASCULINE ORDINAL INDICATOR
        (0x2160, 0x217F), // chiffres romains, majuscules et minuscules
        (0x24B6, 0x24E9), // lettres latines encerclées, majuscules et minuscules
        (0x1F130, 0x1F149), // lettres latines encadrées
        (0x1F150, 0x1F169), // lettres latines encerclées en négatif
        (0x1F170, 0x1F189), // lettres latines encadrées en négatif
    };

    private static bool EstLettreACasse(Rune caractere)
    {
        var categorie = CharUnicodeInfo.GetUnicodeCategory(caractere.Value);
        if (categorie is UnicodeCategory.UppercaseLetter
            or UnicodeCategory.LowercaseLetter
            or UnicodeCategory.TitlecaseLetter)
        {
            return true;
        }
        foreach (var (debut, fin) in PlagesOtherCased)
        {
            if (caractere.Value >= debut && caractere.Value <= fin) return true;
        }
        return false;
    }

    private static readonly HashSet<UnicodeCategory> CategoriesIndifferentesCasse = new()
    {
        // Mn, Me, Cf, Lm, Sk — mesuré exhaustivement sur les 1 114 112 points
        // de code : chacun y est traversé par la règle Final_Sigma, sans
        // exception.
        UnicodeCategory.NonSpacingMark,
        UnicodeCategory.EnclosingMark,
        UnicodeCategory.Format,
        UnicodeCategory.ModifierLetter,
        UnicodeCategory.ModifierSymbol,
    };

    // Complète les cinq catégories ci-dessus par la ponctuation que le
    // découpage par mots d'Unicode (Word_Break MidLetter / MidNumLet /
    // Single_Quote) range aussi dans Case_Ignorable — mesuré : sans ce jeu,
    // un point, un deux-points ou une apostrophe romprait à tort la règle
    // Final_Sigma (« Α.Σ » abaissé en 'α.σ' plutôt qu'en 'α.ς').
    private static readonly HashSet<int> PonctuationIndifferenteCasse = new()
    {
        0x0027, // APOSTROPHE
        0x002E, // FULL STOP
        0x003A, // COLON
        0x00B7, // MIDDLE DOT
        0x0387, // GREEK ANO TELEIA
        0x055F, // ARMENIAN ABBREVIATION MARK
        0x05F4, // HEBREW PUNCTUATION GERSHAYIM
        0x2018, // LEFT SINGLE QUOTATION MARK
        0x2019, // RIGHT SINGLE QUOTATION MARK
        0x2024, // ONE DOT LEADER
        0x2027, // HYPHENATION POINT
        0xFE13, // PRESENTATION FORM FOR VERTICAL COLON
        0xFE52, // SMALL FULL STOP
        0xFE55, // SMALL COLON
        0xFF07, // FULLWIDTH APOSTROPHE
        0xFF0E, // FULLWIDTH FULL STOP
        0xFF1A, // FULLWIDTH COLON
    };

    private static bool EstIndifferentCasse(Rune caractere) =>
        CategoriesIndifferentesCasse.Contains(CharUnicodeInfo.GetUnicodeCategory(caractere.Value))
        || PonctuationIndifferenteCasse.Contains(caractere.Value);

    /// <summary>
    /// Règle Final_Sigma d'Unicode : vrai si le Σ à l'indice <paramref name="indice"/>
    /// est précédé d'une lettre à casse en traversant zéro ou plusieurs
    /// caractères indifférents à la casse, et n'est pas suivi d'une lettre à
    /// casse dans les mêmes conditions.
    /// </summary>
    private static bool EstSigmaFinal(List<Rune> runes, int indice)
    {
        bool precedeParLettreACasse = false;
        for (int i = indice - 1; i >= 0; i--)
        {
            if (EstIndifferentCasse(runes[i])) continue;
            precedeParLettreACasse = EstLettreACasse(runes[i]);
            break;
        }
        if (!precedeParLettreACasse) return false;

        for (int i = indice + 1; i < runes.Count; i++)
        {
            if (EstIndifferentCasse(runes[i])) continue;
            return !EstLettreACasse(runes[i]);
        }
        return true; // fin de texte atteinte sans lettre à casse : final.
    }

    /// <summary>
    /// Abaissement de casse complet d'Unicode, indépendant de toute culture —
    /// la propriété que ni <see cref="string.ToLowerInvariant"/> ni aucun
    /// <c>ToLower(CultureInfo)</c> ne garantissent : les deux sont un mapping
    /// simple, point de code pour point de code, quand cette méthode
    /// applique le mapping complet (potentiellement multi-points-de-code et
    /// dépendant du contexte) qu'utilise <c>str.lower()</c> de Python.
    /// Deux écarts mesurés face au mapping simple, les deux seuls sur
    /// l'intégralité des points de code assignés :
    ///  - U+0130 (İ) s'abaisse en deux points de code (U+0069 puis U+0307
    ///    combinant), une règle de <c>SpecialCasing.txt</c> indépendante de
    ///    toute locale — jamais en U+0130 inchangé ;
    ///  - Σ (U+03A3) s'abaisse en ς (U+03C2, sigma final) plutôt qu'en σ
    ///    (U+03C3) selon la règle Final_Sigma ci-dessus.
    /// Tout autre point de code suit le mapping simple, identique à
    /// <see cref="Rune.ToLowerInvariant"/> sur ces points de code (vérifié
    /// par comparaison exhaustive face à <c>str.lower()</c>).
    /// </summary>
    public static string AbaisserCasseComplet(string texte)
    {
        var runes = texte.EnumerateRunes().ToList();
        var resultat = new StringBuilder(texte.Length);
        for (int i = 0; i < runes.Count; i++)
        {
            Rune rune = runes[i];
            if (rune.Value == 0x0130) // İ LATIN CAPITAL LETTER I WITH DOT ABOVE
            {
                resultat.Append('i'); // LATIN SMALL LETTER I
                resultat.Append('\u0307'); // COMBINING DOT ABOVE
                continue;
            }
            if (rune.Value == 0x03A3) // Σ GREEK CAPITAL LETTER SIGMA
            {
                resultat.Append(EstSigmaFinal(runes, i) ? 'ς' : 'σ');
                continue;
            }
            resultat.Append(Rune.ToLowerInvariant(rune));
        }
        return resultat.ToString();
    }

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
        var categorie = CharUnicodeInfo.GetUnicodeCategory(v);
        return categorie is UnicodeCategory.SpaceSeparator
            or UnicodeCategory.LineSeparator
            or UnicodeCategory.ParagraphSeparator;
    }

    /// <summary>
    /// Équivalent de <c>str.strip()</c> sans argument : <see cref="string.Trim()"/>
    /// de .NET retiendrait un jeu de blancs plus étroit (sans U+001C–U+001F), ce
    /// qui déplacerait la frontière d'un titre ou d'une clôture de bloc de code
    /// commençant ou finissant par l'un de ces caractères.
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
    /// Convertit un retour chariot isolé (non suivi d'un saut de ligne) en saut
    /// de ligne, seul cas que la lecture texte universelle de Python traduit
    /// silencieusement et qu'aucune lecture .NET brute ne traduit. Une paire
    /// <c>\r\n</c> n'est jamais touchée ici — ce n'est pas la même conversion,
    /// et l'ajouter coûterait une passe sur tout le texte sans changer aucun
    /// résultat que ce vérificateur produit.
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
