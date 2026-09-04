using System.Text.RegularExpressions;

namespace VerifCouvertureRecette;

/// <summary>
/// Extraction des identifiants de cas de recette cités par le plan de travail,
/// n'importe où dans le document — critère d'acceptation, ligne « Hors périmètre
/// engagé », ligne « Cas dérivé(s) » : le plan les nomme tous de la même façon
/// syntaxique (§1 — « Un cas de recette est cité par un identifiant `CR-UCNN-KK`,
/// seul ou en plage »), et une citation dans une ligne d'exclusion rend le cas
/// non silencieux au même titre qu'une citation en critère d'acceptation. Rien
/// ici ne restreint donc la lecture à un champ de fiche particulier.
/// </summary>
internal static class CitationsPlan
{
    public static string LireTexte(string racine, string cheminRelatif)
    {
        string chemin = Path.Combine(racine, cheminRelatif);
        if (!File.Exists(chemin))
        {
            throw new PanneExecutionException($"plan de travail introuvable : {chemin}");
        }
        return LecteurTexte.LireTexteIntegral(chemin);
    }

    private static readonly Regex MotifIdentifiantIsole = new(
        @"CR-([A-Z0-9]+)-(\d+)", RegexOptions.CultureInvariant);

    // Même préfixe des deux côtés de la flèche (groupe de rappel \1) — une plage
    // ne mélange jamais deux UC (§3 : `CR-UC07-01 → CR-UC07-09`).
    private static readonly Regex MotifPlage = new(
        @"CR-([A-Z0-9]+)-(\d+)\s*→\s*CR-\1-(\d+)", RegexOptions.CultureInvariant);

    /// <summary>
    /// L'ensemble des identifiants que le plan mentionne, plages développées sur
    /// toute leur étendue. Une citation isolée qui se trouve être la borne d'une
    /// plage par ailleurs présente n'est comptée qu'une fois (ensemble).
    /// </summary>
    public static HashSet<string> ExtraireIdentifiantsCites(string texte)
    {
        var resultat = new HashSet<string>(StringComparer.Ordinal);

        foreach (Match m in MotifPlage.Matches(texte))
        {
            string prefixe = m.Groups[1].Value;
            string debutTexte = m.Groups[2].Value;
            string finTexte = m.Groups[3].Value;
            int debut = int.Parse(debutTexte);
            int fin = int.Parse(finTexte);
            int largeur = Math.Max(debutTexte.Length, finTexte.Length);

            // Une plage mal orientée (borne de fin avant la borne de début) ne
            // développe aucun identifiant plutôt que de compter à rebours — le
            // corpus réel n'en porte aucune, mais l'ensemble vide reste la
            // réponse la plus sûre à une forme qu'aucune règle ne définit.
            for (int n = debut; n <= fin; n++)
            {
                resultat.Add($"CR-{prefixe}-{n.ToString().PadLeft(largeur, '0')}");
            }
        }

        foreach (Match m in MotifIdentifiantIsole.Matches(texte))
        {
            resultat.Add(m.Value);
        }

        return resultat;
    }
}
