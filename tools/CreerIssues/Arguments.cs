namespace CreerIssues;

/// <summary>Les trois options de la ligne de commande, une fois analysées.</summary>
internal sealed record Arguments(string? Seulement, bool Appliquer);

/// <summary>
/// Analyse de la ligne de commande — un sous-ensemble volontairement étroit
/// de ce qu'<c>argparse</c> couvre côté Python (les trois options réellement
/// définies : <c>--seulement</c>, <c>-h</c>/<c>--aide</c>, <c>--appliquer</c>) ;
/// la reproduction n'a été éprouvée que sur ces trois formes et sur l'absence
/// d'argument — pas sur les messages d'erreur d'argparse pour une invocation
/// malformée (option inconnue, valeur manquante).
/// </summary>
internal static class AnalyseurArguments
{
    // Divergence assumée, pas une fidélité vérifiée : argparse.HelpFormatter
    // remet en forme la section « options: » selon la largeur de terminal au
    // moment de l'appel — il n'existe donc aucune forme unique à reproduire,
    // une largeur différente produit un texte différent (mesuré : 1715 octets
    // à 80 colonnes, 1810 à 60, 1643 à 200). Réimplémenter ce retour à la
    // ligne dépendant de l'environnement coûterait du code pour préserver une
    // propriété indésirable ici — un message d'aide dont la forme dépend du
    // lecteur. Ce texte fige donc UNE forme, choisie une fois pour toutes
    // (rendue par RawDescriptionHelpFormatter à 80 colonnes, COLUMNS non
    // définie), et la sert toujours à l'identique, quelle que soit la largeur
    // du terminal appelant. RawDescriptionHelpFormatter ne remet en forme ni
    // la description ni l'épilogue dans aucun cas : ce sont les lignes du
    // docstring et de l'epilog Python, verbatim.
    public const string Usage = "usage: CreerIssues [--seulement TB-nnn[,TB-nnn…]] [-h] [--appliquer]\n";

    public const string TexteAide =
        Usage
        + "\n"
        + "Reporte le plan de travail versionné vers les issues GitHub du dépôt : libellés, jalons, puis une issue par tâche.\n"
        + "\n"
        + "options:\n"
        + "  --seulement TB-nnn[,TB-nnn…]\n"
        + "                        restreint la création d'issues aux seules tâches\n"
        + "                        nommées ; les libellés et les jalons, eux, sont créés\n"
        + "                        en entier car ils sont partagés\n"
        + "  -h, --aide            affiche cette aide et quitte\n"
        + "  --appliquer           crée réellement les libellés, les jalons et les issues\n"
        + "                        (sans ce drapeau : mode à blanc)\n"
        + "\n"
        + "Ordre d'exécution : (1) création des libellés lus dans la taxonomie de methode-de-ticket.md §3 (trois axes : jalon, nature de tranche, nature de vérification — aucun axe de priorité, explicitement interdit par la méthode) ; (2) création des jalons GitHub correspondant aux jalons de build lus dans la même taxonomie, via l'API (« gh milestone » n'existe pas dans les versions récentes de gh) ; (3) création d'une issue par tâche du plan, avec un titre verbatim (identifiant puis intitulé), un corps qui renvoie seulement vers le fichier du plan sans recopier son but, ses dépendances ni ses critères d'acceptation, et les libellés de jalon et de couche déduits du périmètre d'écriture de la tâche ; (4) un compte rendu de ce qui a été créé, de ce qui existait déjà, et de ce qui a échoué.\n"
        + "\n"
        + "Sans --appliquer, rien n'est écrit sur GitHub : le script affiche exactement ce qu'il créerait, ce qui existe déjà (si gh est disponible et authentifié), et les tâches pour lesquelles aucun libellé de couche n'a pu être déduit du plan.\n";

    public sealed record Resultat(Arguments? Valeurs, bool DemandeAide, string? ErreurUsage);

    public static Resultat Analyser(string[] args)
    {
        string? seulement = null;
        bool appliquer = false;

        for (int i = 0; i < args.Length; i++)
        {
            string a = args[i];
            if (a is "-h" or "--aide")
            {
                return new Resultat(null, true, null);
            }
            if (a == "--appliquer")
            {
                appliquer = true;
                continue;
            }
            if (a == "--seulement")
            {
                if (i + 1 >= args.Length)
                {
                    return new Resultat(null, false, "argument --seulement: expected one argument");
                }
                seulement = args[++i];
                continue;
            }
            if (a.StartsWith("--seulement=", StringComparison.Ordinal))
            {
                seulement = a["--seulement=".Length..];
                continue;
            }
            return new Resultat(null, false, $"unrecognized arguments: {a}");
        }
        return new Resultat(new Arguments(seulement, appliquer), false, null);
    }
}
