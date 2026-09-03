using System.Text.RegularExpressions;

namespace CreerIssues;

/// <summary>
/// Découpage des identifiants `TB-nnn` référencés par un champ « Dépend de »
/// ou « En conflit avec », et projection en renvois `#&lt;numéro&gt;`. Hors
/// socle « lecture du plan » : propre à la construction du corps d'issue.
/// </summary>
internal static class TranchesReferences
{
    // \s* : classe de blanc Python (BlancPython), pas \s du moteur .NET.
    // \b : frontière de mot Python (LimiteMotPython), pas celle du moteur .NET.
    private static readonly Regex MotifIdentifiantEnTete = new(
        $"^{BlancPython.Classe}*(TB-\\d+)", RegexOptions.CultureInvariant);

    /// <summary>
    /// Découpe un champ `Dépend de` ou `En conflit avec` (`methode-de-ticket.md §1`)
    /// en identifiants `TB-nnn` référencés, sans retenir le texte explicatif
    /// qui les accompagne. Un segment qui ne commence pas par un identifiant
    /// n'ajoute aucune cible : il qualifie toujours une tranche déjà nommée
    /// par un segment précédent du même champ, jamais une tranche
    /// supplémentaire. Renvoie une liste vide si le champ ne référence
    /// aucune tranche (valeur `—`).
    /// </summary>
    public static List<string> DecouperIdentifiants(string valeurChamp, char separateur)
    {
        string valeur = TexteUnicode.StripPython(valeurChamp);
        if (valeur.Length == 0 || valeur == "—")
        {
            return new List<string>();
        }
        var identifiants = new List<string>();
        foreach (var segment in valeur.Split(separateur))
        {
            var m = MotifIdentifiantEnTete.Match(segment);
            if (m.Success && LimiteMotPython.FrontiereApres(segment, m.Length))
            {
                identifiants.Add(m.Groups[1].Value);
            }
        }
        return identifiants;
    }

    /// <summary>
    /// Projette une liste d'identifiants `TB-nnn` en renvois `#&lt;numéro&gt;` —
    /// cliquables, et dont GitHub affiche l'état ouvert/fermé au premier coup
    /// d'œil (`methode-de-ticket.md §1`, « Ce que la zone engendrée porte »).
    /// La table TB-nnn -&gt; numéro d'issue n'est jamais stockée (§6) :
    /// <paramref name="numeroIssueParTb"/> est reconstruite à chaque exécution
    /// depuis les titres d'issue existants. Une tranche référencée sans issue
    /// reste lisible, jamais un lien mort ni un numéro inventé.
    /// </summary>
    public static string ConstruireRenvois(IReadOnlyList<string> idsTranches, IReadOnlyDictionary<string, int> numeroIssueParTb)
    {
        if (idsTranches.Count == 0) return "—";

        var renvois = new List<string>();
        var dejaVus = new HashSet<string>(StringComparer.Ordinal);
        foreach (var tbId in idsTranches)
        {
            if (!dejaVus.Add(tbId)) continue;
            renvois.Add(numeroIssueParTb.TryGetValue(tbId, out int numero)
                ? $"{tbId} (#{numero})"
                : $"{tbId} (pas encore d'issue)");
        }
        return string.Join(", ", renvois);
    }
}
