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
    /// Rendu d'un champ `Dépend de` ou `En conflit avec` entièrement
    /// qualificatif : au moins un segment de texte, mais aucun n'y nomme de
    /// `TB-nnn`. Ne le confonds jamais avec le rendu `—` d'un champ
    /// réellement vide (`ConstruireRenvois`) : ce dernier affirme une
    /// absence, celui-ci renvoie une qualification non résolue vers la seule
    /// source qui la porte, sans la recopier
    /// (`methode-de-ticket.md §1 § Ce qu'un ticket fait du marqueur TROU`,
    /// même principe appliqué ici à un champ indécidable).
    /// </summary>
    private const string MarqueurQualificatifSansIdentifiant = "texte sans identifiant `TB-nnn` — cf. `Tâche de plan`";

    private static bool EstChampVide(string valeurChamp)
    {
        string valeur = TexteUnicode.StripPython(valeurChamp);
        return valeur.Length == 0 || valeur == "—";
    }

    /// <summary>
    /// Découpe un champ `Dépend de` ou `En conflit avec` (`methode-de-ticket.md §1`)
    /// en identifiants `TB-nnn` référencés, sans retenir le texte explicatif
    /// qui les accompagne. Un segment qui ne commence pas par un identifiant
    /// n'ajoute aucune cible : traite-le comme la qualification d'une
    /// tranche déjà nommée par un segment précédent du même champ, jamais
    /// comme une tranche supplémentaire. Ne conclus jamais, à partir d'une
    /// liste vide renvoyée ici, qu'un champ ne référence aucune tranche : un
    /// champ non vide entièrement qualificatif (aucun segment ne nomme de
    /// `TB-nnn`) et un champ réellement vide (`—`) renvoient tous deux une
    /// liste vide. Distingue-les avec
    /// <see cref="EstQualificatifSansIdentifiant"/> avant de rendre le champ.
    /// </summary>
    public static List<string> DecouperIdentifiants(string valeurChamp, char separateur)
    {
        if (EstChampVide(valeurChamp))
        {
            return new List<string>();
        }
        string valeur = TexteUnicode.StripPython(valeurChamp);
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
    /// Distingue les deux seules causes d'une liste vide renvoyée par
    /// <see cref="DecouperIdentifiants"/> : appelle-la avant
    /// <see cref="ConstruireRenvois"/> pour lui donner de quoi rendre un
    /// champ qualificatif sans identifiant autrement qu'un champ vide.
    /// Renvoie vrai seulement quand le champ porte du texte (donc n'est ni
    /// vide ni `—`) et qu'aucun de ses segments ne nomme de `TB-nnn`.
    /// </summary>
    public static bool EstQualificatifSansIdentifiant(string valeurChamp, char separateur)
        => !EstChampVide(valeurChamp) && DecouperIdentifiants(valeurChamp, separateur).Count == 0;

    /// <summary>
    /// Projette une liste d'identifiants `TB-nnn` en renvois `#&lt;numéro&gt;` —
    /// cliquables, et dont GitHub affiche l'état ouvert/fermé au premier coup
    /// d'œil (`methode-de-ticket.md §1`, « Ce que la zone engendrée porte »).
    /// La table TB-nnn -&gt; numéro d'issue n'est jamais stockée (§6) :
    /// <paramref name="numeroIssueParTb"/> est reconstruite à chaque exécution
    /// depuis les titres d'issue existants. Une tranche référencée sans issue
    /// reste lisible, jamais un lien mort ni un numéro inventé. Rends
    /// <paramref name="estQualificatifSansIdentifiant"/> (calculé par
    /// <see cref="EstQualificatifSansIdentifiant"/> sur le même champ) plutôt
    /// que `—` quand la liste est vide pour cette raison : un champ qui
    /// qualifie sans nommer de tranche n'affirme jamais l'absence que `—`
    /// affirme (`methode-de-ticket.md §1 § Ce qu'un ticket fait du marqueur
    /// TROU`).
    /// </summary>
    public static string ConstruireRenvois(
        IReadOnlyList<string> idsTranches,
        IReadOnlyDictionary<string, int> numeroIssueParTb,
        bool estQualificatifSansIdentifiant = false)
    {
        if (idsTranches.Count == 0)
        {
            return estQualificatifSansIdentifiant ? MarqueurQualificatifSansIdentifiant : "—";
        }

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
