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
    // Un titre de niveau 4 à 6 qui commence par « TB- » est une tâche déclarée : il porte la
    // forme du plan ou il est signalé, jamais sauté. C'est la règle de CreerIssues
    // (PlanDeTravail.cs), qui lit le même plan.
    private static readonly Regex MotifTitreCandidat = new(
        @"^#{4,6}[ \t]+TB-.*$", RegexOptions.Multiline | RegexOptions.CultureInvariant);

    // Forme du plan : cinq dièses, identifiant entier, tiret cadratin, intitulé. Le motif
    // d'identifiant n'a pas de plafond de chiffres (ADR-019 § 2 : « le plus grand + 1 »).
    private static readonly Regex MotifTitreConforme = new(
        @"^##### (TB-\d+) — (.+)$", RegexOptions.CultureInvariant);

    private static readonly Regex MotifLigneChamp = new(
        @"^\|\s*([^|]+?)\s*\|\s*(.*?)\s*\|\s*$",
        RegexOptions.Multiline | RegexOptions.CultureInvariant);

    // Identifiant entier, sans plafond de chiffres ni troncature : « TB-1000 » n'est pas
    // « TB-100 ». Réutilisé aussi bien pour les renvois « Dépend de » / « En conflit avec »
    // que pour les identifiants retirés (§ 11).
    private static readonly Regex MotifIdentifiantReference = new(
        @"TB-\d+", RegexOptions.CultureInvariant);

    private static readonly Regex MotifSectionIdentifiantsRetires = new(
        @"^##\s+.*Identifiants retirés.*$",
        RegexOptions.Multiline | RegexOptions.CultureInvariant);

    private static readonly Regex MotifHeadingNiveau2 = new(
        @"^##\s+", RegexOptions.Multiline | RegexOptions.CultureInvariant);

    private static readonly Regex MotifLigneIdentifiantRetire = new(
        @"^\|\s*(TB-\d+)\s*\|",
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

    public static LectureDesTaches ExtraireTaches(string texte)
    {
        var resultat = new Dictionary<string, Tache>(StringComparer.Ordinal);
        var titresNonConformes = new List<(int Ligne, string Titre)>();
        var lignesParId = new Dictionary<string, List<int>>(StringComparer.Ordinal);
        var candidats = MotifTitreCandidat.Matches(texte);

        for (int idx = 0; idx < candidats.Count; idx++)
        {
            var candidat = candidats[idx];
            int ligneDuTitre = 1 + texte.AsSpan(0, candidat.Index).Count('\n');
            string titre = candidat.Value.TrimEnd('\r');

            var conforme = MotifTitreConforme.Match(titre);
            if (!conforme.Success)
            {
                titresNonConformes.Add((ligneDuTitre, titre));
                continue;
            }

            string id = conforme.Groups[1].Value;
            if (!lignesParId.TryGetValue(id, out var lignes))
            {
                lignesParId[id] = lignes = [];
            }
            lignes.Add(ligneDuTitre);
            // Un identifiant défini deux fois : la première définition garde la place, la
            // suivante est signalée plus bas, jamais écrasée l'une par l'autre.
            if (lignes.Count > 1) continue;

            int finBloc = idx + 1 < candidats.Count ? candidats[idx + 1].Index : texte.Length;
            string bloc = texte[(candidat.Index + candidat.Length)..finBloc];

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

        var enDouble = lignesParId
            .Where(kv => kv.Value.Count > 1)
            .Select(kv => (kv.Key, kv.Value))
            .OrderBy(t => t.Key, ComparateurCodePoints.Instance)
            .ToList();
        return new LectureDesTaches(resultat, titresNonConformes, enDouble);
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
