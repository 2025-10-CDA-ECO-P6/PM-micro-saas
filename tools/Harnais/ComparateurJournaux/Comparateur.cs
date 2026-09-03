using System.Text.Json;
using System.Text.Json.Nodes;

namespace ComparateurJournaux;

/// <summary>
/// Compare deux journaux d'invocations du faux gh, appel par appel et dans
/// l'ordre. Deux appels sont égaux si leurs vecteurs d'arguments sont égaux
/// terme à terme et si leurs entrées standard sont égales — littéralement,
/// ou, quand les deux se décodent comme du JSON, une fois décodées et
/// normalisées : une même charge JSON diffère par les octets (échappement
/// des caractères non-ASCII, espacement) sans différer par le sens, et ce
/// n'est jamais cet écart de forme que ce comparateur doit rapporter.
/// </summary>
internal static class Comparateur
{
    public static Verdict Executer(IReadOnlyList<EntreeJournal> a, IReadOnlyList<EntreeJournal> b, TextWriter sortie)
    {
        int divergences = 0;

        if (a.Count != b.Count)
        {
            sortie.WriteLine($"Nombre d'appels différent : {a.Count} (A) vs {b.Count} (B)");
            divergences++;
        }

        int nombreCommun = Math.Min(a.Count, b.Count);
        for (int i = 0; i < nombreCommun; i++)
        {
            divergences += ComparerAppel(i, a[i], b[i], sortie);
        }

        for (int i = nombreCommun; i < a.Count; i++)
        {
            sortie.WriteLine($"Appel #{i + 1} : présent côté A, absent côté B — {DecrireAppel(a[i])}");
            divergences++;
        }
        for (int i = nombreCommun; i < b.Count; i++)
        {
            sortie.WriteLine($"Appel #{i + 1} : présent côté B, absent côté A — {DecrireAppel(b[i])}");
            divergences++;
        }

        if (divergences == 0)
        {
            sortie.WriteLine($"Aucune divergence — {a.Count} appel(s) comparé(s), identiques dans l'ordre.");
        }

        return new Verdict(divergences);
    }

    private static int ComparerAppel(int index, EntreeJournal appelA, EntreeJournal appelB, TextWriter sortie)
    {
        int divergences = 0;

        if (!appelA.Args.SequenceEqual(appelB.Args, StringComparer.Ordinal))
        {
            sortie.WriteLine($"Appel #{index + 1} : vecteurs d'arguments différents");
            sortie.WriteLine($"  A : [{string.Join(", ", appelA.Args)}]");
            sortie.WriteLine($"  B : [{string.Join(", ", appelB.Args)}]");
            divergences++;
        }

        if (!EntreesStandardEquivalentes(appelA.Stdin, appelB.Stdin, out string? formeA, out string? formeB))
        {
            string origine = formeA is not null ? " (charge JSON normalisée)" : "";
            sortie.WriteLine($"Appel #{index + 1} : entrée standard différente{origine}");
            sortie.WriteLine($"  A : {formeA ?? appelA.Stdin}");
            sortie.WriteLine($"  B : {formeB ?? appelB.Stdin}");
            divergences++;
        }

        return divergences;
    }

    /// Compare deux entrées standard. Égalité littérale d'abord (couvre le
    /// cas — le plus courant — où les deux valent la chaîne vide). Sinon, si
    /// les deux se décodent comme un document JSON, compare la forme décodée
    /// plutôt que le texte brut : un ordre de clé, un échappement ou un
    /// espacement distincts n'y font plus une divergence. Si l'une des deux
    /// ne se décode pas comme du JSON, la comparaison littérale — déjà
    /// négative à ce point — fait foi : rien à normaliser.
    private static bool EntreesStandardEquivalentes(
        string a, string b, out string? formeNormaliseeA, out string? formeNormaliseeB)
    {
        formeNormaliseeA = null;
        formeNormaliseeB = null;
        if (string.Equals(a, b, StringComparison.Ordinal))
        {
            return true;
        }

        JsonNode? noeudA = TenterDecoder(a);
        JsonNode? noeudB = TenterDecoder(b);
        if (noeudA is null || noeudB is null)
        {
            return false;
        }

        formeNormaliseeA = noeudA.ToJsonString();
        formeNormaliseeB = noeudB.ToJsonString();
        return JsonNode.DeepEquals(noeudA, noeudB);
    }

    private static JsonNode? TenterDecoder(string texte)
    {
        if (texte.Trim().Length == 0)
        {
            return null;
        }
        try
        {
            return JsonNode.Parse(texte);
        }
        catch (JsonException)
        {
            return null;
        }
    }

    private static string DecrireAppel(EntreeJournal appel) => $"[{string.Join(", ", appel.Args)}]";
}
