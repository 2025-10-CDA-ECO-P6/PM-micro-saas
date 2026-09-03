using System.Text;
using System.Text.RegularExpressions;

namespace VerifCorpus;

/// <summary>
/// Calcule le slug d'ancre d'un titre Markdown, à l'identique du <c>slug()</c>
/// Python de référence.
/// </summary>
internal static class Slug
{
    private static readonly Regex MotifCodeInline =
        new(@"`([^`]*)`", RegexOptions.CultureInvariant);
    private static readonly Regex MotifLienOuImage =
        new(@"!?\[([^\]]*)\]\([^)]*\)", RegexOptions.CultureInvariant);
    private static readonly Regex MotifBaliseHtml =
        new(@"<[^>]+>", RegexOptions.CultureInvariant);
    private static readonly Regex MotifEmphase =
        new(@"[*_~]", RegexOptions.CultureInvariant);

    public static string Calculer(string texte)
    {
        string t = TexteUnicode.StripPython(texte);
        t = MotifCodeInline.Replace(t, "$1");
        t = MotifLienOuImage.Replace(t, "$1");
        t = MotifBaliseHtml.Replace(t, "");
        t = MotifEmphase.Replace(t, "");
        // Abaissement de casse d'abord, normalisation NFC ensuite — l'ordre
        // inverse laisserait passer un cas mesuré : U+0130 (İ) s'abaisse en
        // 'i' + U+0307 combinant, qu'une NFC appliquée après l'abaissement ne
        // recompose plus (le combinant seul échoue ensuite à l'appartenance
        // alphanumérique et disparaît, ce qui est le comportement mesuré côté
        // Python).
        //
        // TexteUnicode.AbaisserCasseComplet, jamais ToLowerInvariant ni
        // ToLower(culture) : les deux sont un mapping simple, point de code
        // pour point de code — İ y resterait un seul point de code inchangé,
        // et un Σ (sigma majuscule grec) s'y abaisserait toujours en σ, sans
        // jamais rendre ς (sigma final) même en position finale de mot.
        t = TexteUnicode.AbaisserCasseComplet(t);
        t = t.Normalize(NormalizationForm.FormC);

        var resultat = new StringBuilder();
        foreach (var rune in t.EnumerateRunes())
        {
            if (rune.Value == '-' || rune.Value == '_' || TexteUnicode.EstAlphanumeriquePython(rune))
            {
                resultat.Append(rune);
            }
            else if (rune.Value == ' ' || rune.Value == '\t')
            {
                resultat.Append('-');
            }
        }
        return resultat.ToString();
    }
}
