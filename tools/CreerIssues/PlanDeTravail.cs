using System.Text.RegularExpressions;

namespace CreerIssues;

/// <summary>Une tâche du plan, telle qu'extraite de sa fiche.</summary>
internal sealed record Tache(string Id, string Titre, Dictionary<string, string> Champs);

/// <summary>
/// Extraction des tâches du plan de travail. Fait partie du socle « lecture
/// du plan » : sans dépendance à la résolution de jalon/couche, aux appels
/// `gh`, ni à la construction de corps d'issue — la vague suivante (report
/// vers un projet GitHub) lie ce fichier tel quel, pour la même extraction.
/// </summary>
internal static class PlanDeTravail
{
    // Cinq dièses exacts, identifiant entier sans plafond de chiffres (ADR-019 § 2 :
    // « le plus grand + 1 »), tiret cadratin obligatoire : la forme du plan. Un titre de
    // niveau 4 à 6 qui commence par « TB- » sans cette forme est une tâche déclarée que
    // personne ne lirait : il arrête l'outil (MotifTitreCandidat) au lieu d'être sauté.
    // VerifMailleAgregat (PlanDeTravail.cs) applique la même règle ; l'accord des deux
    // outils est vérifié par tools/Tests/DivergencesEntreOutilsTests.cs.
    private static readonly Regex MotifTitre = new(
        @"^##### (TB-\d+) — (.+)$", RegexOptions.Multiline | RegexOptions.CultureInvariant);

    private static readonly Regex MotifTitreCandidat = new(
        @"^#{4,6}[ \t]+TB-.*$", RegexOptions.Multiline | RegexOptions.CultureInvariant);

    private static readonly Regex MotifLimiteBloc = new(
        $"^---{BlancPython.Classe}*$",
        RegexOptions.Multiline | RegexOptions.CultureInvariant);

    public static List<Tache> ExtraireTaches(string contenuPlan)
    {
        VerifierTitres(contenuPlan);

        var correspondances = MotifTitre.Matches(contenuPlan);
        var taches = new List<Tache>();

        for (int i = 0; i < correspondances.Count; i++)
        {
            var m = correspondances[i];
            int debut = m.Index + m.Length;
            int fin = i + 1 < correspondances.Count ? correspondances[i + 1].Index : contenuPlan.Length;
            string bloc = contenuPlan[debut..fin];

            // Une tâche se termine avant le prochain filet horizontal isolé
            // (« --- » seul sur sa ligne), qui marque une frontière de
            // section — jamais avant une ligne de séparation de tableau
            // (« |---|---| »).
            var limite = MotifLimiteBloc.Match(bloc);
            if (limite.Success)
            {
                bloc = bloc[..limite.Index];
            }

            taches.Add(new Tache(m.Groups[1].Value, TexteUnicode.StripPython(m.Groups[2].Value), ExtraireChamps(bloc)));
        }
        return taches;
    }

    /// <summary>
    /// Refuse un plan dont un titre de tâche est hors de la forme (ligne nommée)
    /// ou dont un identifiant est défini plus d'une fois (lignes nommées) :
    /// <see cref="ErreurCorpusException"/> plutôt qu'une tâche sautée ou écrasée.
    /// </summary>
    private static void VerifierTitres(string contenuPlan)
    {
        var horsForme = new List<string>();
        var lignesParId = new Dictionary<string, List<int>>(StringComparer.Ordinal);

        foreach (Match candidat in MotifTitreCandidat.Matches(contenuPlan))
        {
            int ligne = 1 + contenuPlan.AsSpan(0, candidat.Index).Count('\n');
            string titre = candidat.Value.TrimEnd('\r');
            var conforme = MotifTitre.Match(titre);
            if (!conforme.Success)
            {
                horsForme.Add($"ligne {ligne} : {titre}");
                continue;
            }
            string id = conforme.Groups[1].Value;
            if (!lignesParId.TryGetValue(id, out var lignes))
            {
                lignesParId[id] = lignes = [];
            }
            lignes.Add(ligne);
        }

        if (horsForme.Count > 0)
        {
            throw new ErreurCorpusException(
                $"{Chemins.PlanRelatif} : titre de tâche hors de la forme `##### TB-<chiffres> — <intitulé>` "
                + $"({string.Join(" ; ", horsForme)})");
        }

        var enDouble = lignesParId
            .Where(kv => kv.Value.Count > 1)
            .Select(kv => $"{kv.Key} (lignes {string.Join(", ", kv.Value)})")
            .ToList();
        if (enDouble.Count > 0)
        {
            throw new ErreurCorpusException(
                $"{Chemins.PlanRelatif} : identifiant de tâche défini deux fois ou plus : {string.Join(" ; ", enDouble)}");
        }
    }

    private static Dictionary<string, string> ExtraireChamps(string bloc)
    {
        var champs = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var (cle, valeur) in TableauMarkdown.AnalyserLignes(bloc))
        {
            champs[cle] = valeur;
        }
        return champs;
    }
}
