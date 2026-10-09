using System.Text;
using System.Text.RegularExpressions;

namespace VerifCorpus;

/// <summary>
/// Calcule le slug d'ancre d'un titre Markdown, tel que GitHub le calcule.
/// Le <c>slug()</c> Python d'origine a été retiré au portage en C# (commit
/// 81b30eb) : cette classe est désormais la seule référence, et la cible est le
/// comportement de GitHub, pas celui de l'ancien script.
/// </summary>
internal static class Slug
{
    private static readonly Regex MotifCodeInline =
        new(@"`([^`]*)`", RegexOptions.CultureInvariant);
    private static readonly Regex MotifLienOuImage =
        new(@"!?\[([^\]]*)\]\([^)]*\)", RegexOptions.CultureInvariant);
    private static readonly Regex MotifBaliseHtml =
        new(@"<[^>]+>", RegexOptions.CultureInvariant);
    // `*` et `~` sont toujours retirés. Un `_` n'est retiré que s'il n'est pas
    // coincé entre deux caractères alphanumériques : GitHub le garde en
    // intra-mot (LIVE_NOTE donne live_note) et ne retire que les `_`
    // d'emphase (_italique_, __gras__).
    private static readonly Regex MotifEmphase =
        new(@"[*~]|(?<![\p{L}\p{M}\p{N}])_|_(?![\p{L}\p{M}\p{N}])", RegexOptions.CultureInvariant);

    // Remplace, dans le code inline, les `_` que l'emphase ne doit jamais
    // toucher : GitHub n'y voit pas d'emphase. Un point de code d'usage privé,
    // absent de tout titre du corpus, tient la place jusqu'à la restitution.
    private const char SouligneProtege = '';

    public static string Calculer(string texte)
    {
        string t = TexteUnicode.StripPython(texte);
        t = MotifCodeInline.Replace(t, m => m.Groups[1].Value.Replace('_', SouligneProtege));
        t = MotifLienOuImage.Replace(t, "$1");
        t = MotifBaliseHtml.Replace(t, "");
        t = MotifEmphase.Replace(t, "");
        t = t.Replace(SouligneProtege, '_');
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
