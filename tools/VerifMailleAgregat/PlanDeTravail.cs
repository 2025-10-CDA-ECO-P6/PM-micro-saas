using System.Text.RegularExpressions;

namespace VerifMailleAgregat;

/// <summary>
/// Lecture du plan de travail : découpage en tâches (§ Tâches) et, séparément,
/// extraction des identifiants de la section « Identifiants retirés » (§11) —
/// deux lectures indépendantes du même texte, chacune scopée à sa propre partie
/// du document.
/// </summary>
internal static class PlanDeTravail
{
    private static readonly Regex MotifSplitBloc = new(
        @"\n#{4,6}\s+(?=TB-)", RegexOptions.CultureInvariant);

    // Ancrage explicite ajouté : re.match de Python ancre implicitement en tête de
    // chaîne, Match de .NET cherche. Le bloc analysé ici commence toujours par
    // « TB-... » (garanti par la coupure ci-dessus, qui coupe juste avant l'identifiant
    // sans le consommer), donc une recherche non ancrée retrouverait aujourd'hui le
    // même résultat qu'un ancrage explicite — la divergence est latente, pas active :
    // c'est justement l'espèce qui se réveille sans bruit si cette garantie de position
    // venait à changer. Ancré ici pour ne pas en dépendre.
    private static readonly Regex MotifIdentifiantBloc = new(
        @"^(TB-\d{3})", RegexOptions.CultureInvariant);

    private static readonly Regex MotifLigneChamp = new(
        @"^\|\s*([^|]+?)\s*\|\s*(.*?)\s*\|\s*$",
        RegexOptions.Multiline | RegexOptions.CultureInvariant);

    // Divergence B — cf.
    // tools/Tests/DivergencesEntreOutilsTests.cs::DivergenceB_NombreDeChiffresDansLIdentifiant.
    // Seul le TRAITEMENT de cette divergence est arbitré ici : reproduire telle quelle,
    // sans corriger. Le FOND — laquelle des deux implémentations est juste — n'est pas
    // tranché par ce commentaire. Exactement trois chiffres, sans ancre de fin — un
    // identifiant à quatre chiffres est tronqué en silence aux trois premiers. Réutilisé
    // aussi bien pour l'identifiant de bloc que pour les renvois « Dépend de » /
    // « En conflit avec ».
    private static readonly Regex MotifIdentifiantReference = new(
        @"TB-\d{3}", RegexOptions.CultureInvariant);

    private static readonly Regex MotifSectionIdentifiantsRetires = new(
        @"^##\s+.*Identifiants retirés.*$",
        RegexOptions.Multiline | RegexOptions.CultureInvariant);

    private static readonly Regex MotifHeadingNiveau2 = new(
        @"^##\s+", RegexOptions.Multiline | RegexOptions.CultureInvariant);

    private static readonly Regex MotifLigneIdentifiantRetire = new(
        @"^\|\s*(TB-\d{3})\s*\|",
        RegexOptions.Multiline | RegexOptions.CultureInvariant);

    public static string LireTexte(string racine, string cheminPlanRelatif)
    {
        string chemin = Path.Combine(racine, cheminPlanRelatif);
        if (!File.Exists(chemin))
        {
            throw new PanneExecutionException($"plan introuvable : {chemin}");
        }
        return LecteurTexte.LireTexteIntegral(chemin);
    }

    public static Dictionary<string, Tache> ExtraireTaches(string texte)
    {
        var resultat = new Dictionary<string, Tache>(StringComparer.Ordinal);
        var blocs = MotifSplitBloc.Split(texte);

        for (int idx = 1; idx < blocs.Length; idx++)
        {
            string bloc = blocs[idx];
            string id = MotifIdentifiantBloc.Match(bloc).Groups[1].Value;

            var champs = new Dictionary<string, string>(StringComparer.Ordinal);
            foreach (Match ligne in MotifLigneChamp.Matches(bloc))
            {
                string cle = TexteUnicode.StripPython(ligne.Groups[1].Value);
                string valeur = TexteUnicode.StripPython(ligne.Groups[2].Value);
                champs[cle] = valeur;
            }

            string perimetre = champs.GetValueOrDefault("Périmètre d'écriture", "");
            // HORS MAILLE : la tranche ne vise aucun module de code — ce n'est pas un
            // item qui se résout, c'est l'absence d'item. Exclue de la liste au même
            // titre que TROU, donc exclue par construction du recalcul des conflits :
            // deux tranches HORS MAILLE ne peuvent plus produire de conflit fantôme,
            // quel que soit leur libellé.
            bool horsMaille = perimetre.Contains("HORS MAILLE", StringComparison.Ordinal);
            bool trou = perimetre.Contains("TROU", StringComparison.Ordinal);

            List<string> items;
            if (horsMaille || trou)
            {
                items = [];
            }
            else
            {
                items = perimetre.Split(';')
                    .Select(TexteUnicode.StripPython)
                    .Where(x => x.Length > 0
                        && !string.Equals(x, "—", StringComparison.Ordinal)
                        && !string.Equals(x, "-", StringComparison.Ordinal))
                    .ToList();
            }

            var conflits = new HashSet<string>(StringComparer.Ordinal);
            foreach (Match m in MotifIdentifiantReference.Matches(champs.GetValueOrDefault("En conflit avec", "")))
            {
                conflits.Add(m.Value);
            }
            var dependDe = new HashSet<string>(StringComparer.Ordinal);
            foreach (Match m in MotifIdentifiantReference.Matches(champs.GetValueOrDefault("Dépend de", "")))
            {
                dependDe.Add(m.Value);
            }

            resultat[id] = new Tache(items, horsMaille, trou, conflits, dependDe);
        }
        return resultat;
    }

    /// <summary>
    /// Identifiants retirés (§11) : le contenu de la seule colonne de gauche des
    /// lignes de tableau de la section, du titre de la section jusqu'au prochain
    /// titre de niveau 2 (ou la fin du document). Scopé à la section pour ignorer
    /// une colonne « Repris par » qui peut elle-même citer un identifiant vivant.
    /// </summary>
    public static HashSet<string> ExtraireIdentifiantsRetires(string texte)
    {
        var resultat = new HashSet<string>(StringComparer.Ordinal);

        var debut = MotifSectionIdentifiantsRetires.Match(texte);
        if (!debut.Success) return resultat;

        int positionApresEntete = debut.Index + debut.Length;
        var finSuivante = MotifHeadingNiveau2.Match(texte, positionApresEntete);
        string section = finSuivante.Success
            ? texte[positionApresEntete..finSuivante.Index]
            : texte[positionApresEntete..];

        foreach (Match m in MotifLigneIdentifiantRetire.Matches(section))
        {
            resultat.Add(m.Groups[1].Value);
        }
        return resultat;
    }
}
