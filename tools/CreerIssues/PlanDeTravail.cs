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
    // Cinq dièses exacts, tiret cadratin obligatoire — c'est la forme du
    // script Python d'origine (creer-issues.py), distincte de celle du
    // vérificateur de maille (4 à 6 dièses, aucun tiret imposé) : Divergence A
    // documentée par tools/Tests/DivergencesEntreOutilsTests.cs::DivergenceA_FormatDuTitreDeTranche.
    // Seul le TRAITEMENT de cette divergence est arbitré ici : reproduire telle quelle,
    // sans corriger. Le FOND — laquelle des deux implémentations est juste — n'est pas
    // tranché par ce commentaire.
    private static readonly Regex MotifTitre = new(
        @"^##### (TB-\d+) — (.+)$", RegexOptions.Multiline | RegexOptions.CultureInvariant);

    private static readonly Regex MotifLimiteBloc = new(
        $"^---{BlancPython.Classe}*$",
        RegexOptions.Multiline | RegexOptions.CultureInvariant);

    public static List<Tache> ExtraireTaches(string contenuPlan)
    {
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
