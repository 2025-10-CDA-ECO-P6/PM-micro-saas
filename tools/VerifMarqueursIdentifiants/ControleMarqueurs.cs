using System.Text;
using System.Text.RegularExpressions;

namespace VerifMarqueursIdentifiants;

/// <summary>
/// Signale tout marqueur de statut hors de la liste fermée de l'ADR-019 § 5,
/// dans les <c>*.md</c> de <c>docs/</c>.
///
/// Un marqueur de statut s'écrit sous trois formes, relevées dans le corpus :
/// <list type="bullet">
/// <item>entre crochets, nom en capitales suivi de <c>]</c> ou de « — » :
/// <c>[À TRANCHER — ticket]</c>, <c>[HORS-MVP]</c> (un nom de moins de cinq
/// lettres, ou suivi de <c>(</c> ou de <c>[</c>, est un lien ou un gabarit, pas
/// un statut) ;</item>
/// <item>en gras en tête de point (puce, numéro, cellule de tableau, citation) :
/// <c>**DÉCIDÉ.**</c>, <c>**FLAG JURISTE**</c> ;</item>
/// <item>pour les quatre formes que l'ADR-019 § 5 convertit, le mot lui-même :
/// <c>RÉSOLU</c>, <c>FERMÉE</c> et <c>TRANCHÉ</c> en capitales, où qu'ils
/// soient (la prose ordinaire écrit « tranché » en minuscules) ; <c>Clos</c>
/// seulement en gras ou entre crochets (« Clos par » d'un en-tête de colonne
/// n'est pas un statut).</item>
/// </list>
/// Ne sont pas des emplois : les blocs de code, et un mot nu cité entre
/// accents graves (<c>`RÉSOLU`</c>) : c'est ainsi que <c>docs/glossaire.md</c> et l'ADR-019
/// nomment les quatre formes pour les rejeter. Aucun fichier n'est exempté — un
/// marqueur employé comme statut y serait signalé comme ailleurs.
/// </summary>
internal static class ControleMarqueurs
{
    private static readonly string[] FormesConverties = ["RÉSOLU", "FERMÉE", "TRANCHÉ", "Clos"];

    // La liste fermée : ADR-019 § 5, ajustement du 2026-10-09. La dette reportée à un
    // jalon nommé s'écrit « dette nommée, <jalon> (<renvoi>) » : une tournure de prose,
    // que le contrôle ne lit pas ; `REPORTÉ`, forme du registre généré de
    // docs/conventions.md, n'est pas dans l'ADR et le corpus ne l'emploie pas.
    private static readonly string[] ListeFermee =
    [
        "À TRANCHER",
        "SOUS-SPÉCIFIÉ",
        "DÉCIDÉ",
        "À VALIDER",
        "Retiré",
        "Remplacé par",
    ];

    // Marqueurs d'autres listes que le corpus déclare lui-même : la source est
    // le document qui les déclare. Ce ne sont pas des écarts à la liste fermée.
    private static readonly string[] AutresListesDeclarees =
    [
        // docs/conventions.md, registre des marqueurs.
        "À DÉCIDER",
        "HORS PÉRIMÈTRE",
        "ABSENT PAR NATURE",
        "DÉSACTIVÉ",
        "À COMPLÉTER",
        "TROU",
        "HORS MAILLE",
        // docs/interface/conventions.md, légende des marqueurs des fiches d'écran.
        "HORS-MVP",
        // docs/projet/registre-des-risques.md, tag distinct du champ Confiance.
        "NON-VÉRIFIABLE-EN-BUILD",
        // docs/assurance/strategie-de-test.md, points de décision non vérifiables en CI.
        "DÉCISION MARCHÉ",
        "VALIDATION JURIDIQUE EU",
        // docs/interface/gabarit-ecran.md, préfixe de critère d'ancrage.
        "SOURCE",
        // docs/besoin/decouverte/guide-d-entretien.md, gabarit de date à remplir : un champ, pas un statut.
        "YYYY-MM-DD",
    ];

    private const int LettresMinimales = 5;

    private static readonly HashSet<string> Autorises = new(
        ListeFermee.Concat(AutresListesDeclarees), StringComparer.Ordinal);

    private static readonly Regex MotifCodeNu = new(
        "`(RÉSOLU|FERMÉE|TRANCHÉ|Clos)`",
        RegexOptions.CultureInvariant);

    private static readonly Regex MotifFormeEnCapitales = new(
        @"(?<![\p{L}\p{N}_])(RÉSOLU|FERMÉE|TRANCHÉ)(?![\p{L}\p{N}_])",
        RegexOptions.CultureInvariant);

    private static readonly Regex MotifGras = new(
        @"\*\*([^*]+)\*\*",
        RegexOptions.CultureInvariant);

    private static readonly Regex MotifClos = new(
        @"(?<![\p{L}\p{N}_])Clos(?![\p{L}\p{N}_])",
        RegexOptions.CultureInvariant);

    private static readonly Regex MotifClosEntreCrochets = new(
        @"\[Clos(?![\p{L}\p{N}_])",
        RegexOptions.CultureInvariant);

    private static readonly Regex MotifCrochets = new(
        @"\[(?<nom>\p{Lu}[\p{Lu}'’ -]*\p{Lu})(?=\]| —)",
        RegexOptions.CultureInvariant);

    private static readonly Regex MotifGrasEnTete = new(
        @"^\s*(?:>\s*)*(?:(?:[-*+]|\d+[.)])\s+|\|\s*)?\*\*(?<nom>\p{Lu}[\p{Lu}'’ -]*\p{Lu})(?:\s*\([^)]*\))?\s*[.:]?\*\*",
        RegexOptions.CultureInvariant);

    public static List<Constat> Controler(string racine, IReadOnlyList<string> fichiers)
    {
        var constats = new List<Constat>();
        foreach (var rel in fichiers)
        {
            foreach (var (numero, brut) in CorpusMarkdown.LignesHorsCode(racine, rel))
            {
                string ligne = brut.Normalize(NormalizationForm.FormC);
                foreach (var forme in FormesConverteesDe(ligne))
                {
                    constats.Add(new Constat(
                        $"{rel}|{forme}", rel, numero,
                        $"forme « {forme} » à convertir (ADR-019 § 5)"));
                }
                foreach (var nom in NomsHorsListe(ligne))
                {
                    constats.Add(new Constat(
                        $"{rel}|{nom}", rel, numero,
                        $"marqueur « {nom} » hors de la liste fermée (ADR-019 § 5)"));
                }
            }
        }
        return constats;
    }

    private static IEnumerable<string> FormesConverteesDe(string ligne)
    {
        string nette = MotifCodeNu.Replace(ligne, " ");
        foreach (Match m in MotifFormeEnCapitales.Matches(nette)) yield return m.Groups[1].Value;
        foreach (Match gras in MotifGras.Matches(nette))
        {
            if (MotifClos.IsMatch(gras.Groups[1].Value)) yield return "Clos";
        }
        foreach (Match _ in MotifClosEntreCrochets.Matches(nette)) yield return "Clos";
    }

    private static IEnumerable<string> NomsHorsListe(string ligne)
    {
        foreach (Match m in MotifCrochets.Matches(ligne))
        {
            string nom = m.Groups["nom"].Value;
            // `[NOM](cible)` et `[NOM][ref]` sont des liens, pas des statuts — y compris
            // quand le texte du lien porte un « — » (`[UC-HORS-MVP — titre](cible)`).
            int fermeture = ligne.IndexOf(']', m.Index);
            if (fermeture >= 0 && fermeture + 1 < ligne.Length && ligne[fermeture + 1] is '(' or '[') continue;
            if (EstHorsListe(nom)) yield return nom;
        }

        var enTete = MotifGrasEnTete.Match(ligne);
        if (enTete.Success && EstHorsListe(enTete.Groups["nom"].Value)) yield return enTete.Groups["nom"].Value;
    }

    private static bool EstHorsListe(string nom)
    {
        if (Autorises.Contains(nom)) return false;
        if (FormesConverties.Contains(nom, StringComparer.Ordinal)) return false;
        return nom.Count(char.IsLetter) >= LettresMinimales;
    }
}
