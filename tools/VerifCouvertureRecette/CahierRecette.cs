using System.Text.RegularExpressions;

namespace VerifCouvertureRecette;

/// <summary>
/// Lecture du cahier de stratégie de test et de recette : extraction des cas de
/// recette réels (§10, une ligne de tableau par cas) et de leur colonne « Source ».
/// Ne lit rien au-delà du texte reçu.
/// </summary>
internal static class CahierRecette
{
    private static readonly Regex MotifIdentifiantCas = new(
        @"^CR-[A-Z0-9]+-\d+$", RegexOptions.CultureInvariant);

    public static string LireTexte(string racine, string cheminRelatif)
    {
        string chemin = Path.Combine(racine, cheminRelatif);
        if (!File.Exists(chemin))
        {
            throw new PanneExecutionException($"cahier de recette introuvable : {chemin}");
        }
        return LecteurTexte.LireTexteIntegral(chemin);
    }

    /// <summary>
    /// Découpe chaque ligne de tableau Markdown en cellules, en retenant l'en-tête
    /// de tableau courant — la ligne immédiatement suivie d'un séparateur
    /// <c>|---|---|</c> — pour localiser, à chaque ligne de donnée, la colonne
    /// jouant le rôle de <c>Source</c>. Son intitulé exact varie selon le tableau :
    /// <c>Source</c> dans les tableaux par UC (§10), <c>Renvoi</c> ou
    /// <c>Renvoi / Verdict</c> dans les deux tableaux de « Cas transverses » —
    /// la colonne est donc retrouvée par contenu d'en-tête, jamais par position
    /// fixe, seule lecture qui reste correcte sur les deux formes de tableau.
    ///
    /// Une ligne dont la première cellule n'ouvre pas un identifiant <c>CR-…</c>
    /// n'est pas un cas de recette — c'est le cas de toute autre ligne de tableau
    /// du document (matrice de traçabilité §11, registre des points ouverts §12…),
    /// et de toute ligne où un identifiant <c>CR-…</c> n'apparaît qu'en colonne
    /// <c>Source</c>/<c>Renvoi</c> ou dans de la prose : ce sont des renvois, pas
    /// des cas.
    /// </summary>
    public static List<CasRecette> ExtraireCasReels(string texte)
    {
        string[] lignes = texte.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
        var resultat = new List<CasRecette>();

        int colonneSource = -1;
        int colonneCas = -1;

        for (int i = 0; i < lignes.Length; i++)
        {
            string ligne = lignes[i].Trim();
            if (!ligne.StartsWith('|') || !ligne.EndsWith('|')) continue;
            if (TableauMarkdown.EstLigneSeparatrice(ligne)) continue;

            string[] cellules = TableauMarkdown.DecouperCellules(ligne);
            if (cellules.Length == 0) continue;

            // Une ligne d'en-tête est celle immédiatement suivie d'une ligne
            // séparatrice — c'est cette relance structurelle, pas l'intitulé de
            // sa première cellule, qui distingue un en-tête d'une ligne de donnée.
            bool estEntete = i + 1 < lignes.Length && TableauMarkdown.EstLigneSeparatrice(lignes[i + 1].Trim());
            if (estEntete)
            {
                colonneSource = Array.FindIndex(cellules, c =>
                    c.Contains("Source", StringComparison.Ordinal)
                    || c.Contains("Renvoi", StringComparison.Ordinal));
                // « Cas » n'existe que sur les tableaux par UC — les deux
                // tableaux de cas transverses l'appellent « Invariant » et ne
                // marquent jamais de retrait (mesuré, §10).
                colonneCas = Array.FindIndex(cellules, c => c.Contains("Cas", StringComparison.Ordinal));
                continue;
            }

            if (!MotifIdentifiantCas.IsMatch(cellules[0])) continue;

            string source = colonneSource >= 0 && colonneSource < cellules.Length
                ? cellules[colonneSource]
                : "";
            string cas = colonneCas >= 0 && colonneCas < cellules.Length
                ? cellules[colonneCas]
                : "";
            resultat.Add(new CasRecette(cellules[0], source, cas));
        }

        return resultat;
    }
}
