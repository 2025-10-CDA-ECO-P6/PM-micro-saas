namespace CreerProjet;

/// <summary>La seule option de la ligne de commande, une fois analysée.</summary>
internal sealed record Arguments(bool Appliquer);

/// <summary>
/// Analyse de la ligne de commande — le script Python d'origine
/// (<c>creer-projet.py</c>) ne définissait, à la différence de son jumeau
/// (<c>creer-issues.py</c>), aucun alias <c>--aide</c> : seul
/// <c>-h</c>/<c>--help</c> (la forme native d'<c>argparse</c>) affichait
/// l'aide, <c>--aide</c> étant un argument inconnu comme un autre — reproduit
/// ici à l'identique. Reproduction non éprouvée au-delà de l'absence
/// d'argument et d'une option inconnue isolée (voir le rapport de portage) —
/// pas, par exemple, d'une valeur explicite sur <c>--appliquer</c>
/// (<c>--appliquer=x</c>), qu'argparse rejetait d'un message distinct que ce
/// portage ne reproduit pas.
/// </summary>
internal static class AnalyseurArguments
{
    // Divergence assumée, pas une fidélité vérifiée, à l'identique du constat
    // déjà posé dans CreerIssues/Arguments.cs pour son propre script Python
    // d'origine : argparse.HelpFormatter remettait en forme la section
    // « options: » selon la largeur de terminal au moment de l'appel. Ce
    // texte fige donc UNE forme, choisie une fois pour toutes (capturée à la
    // création du portage via `python3 creer-projet.py -h`, sortie non-tty,
    // COLUMNS non définie), et la sert toujours à l'identique, quelle que
    // soit la largeur du terminal appelant.
    public const string Usage = "usage: CreerProjet [-h] [--appliquer]\n";

    public const string TexteAide =
        Usage
        + "\n"
        + "Reporte le plan de travail vers un projet GitHub (Projects v2).\n"
        + "\n"
        + "Crée le projet, ses champs, y ajoute les issues déjà ouvertes par\n"
        + "CreerIssues, et renseigne les deux seuls champs que la méthode de ticket\n"
        + "autorise à vivre hors du plan :\n"
        + "\n"
        + "  - « Statut »  : le cycle de vie de methode-de-ticket.md §5, six états.\n"
        + "                  L'état « Prêt » est CALCULÉ depuis les cinq conditions du §2,\n"
        + "                  jamais estimé.\n"
        + "  - « Épique »  : classification transverse, déduite du plan.\n"
        + "\n"
        + "Ce qu'il ne fait pas, et pourquoi :\n"
        + "  - il ne porte ni taille, ni dépendances, ni périmètre, ni critères : le §1 de\n"
        + "    la méthode les nomme comme champs qu'un ticket ne recopie pas ;\n"
        + "  - il ne crée aucune vue : `gh project` n'a pas de sous-commande pour cela.\n"
        + "    Les vues se créent dans l'interface, la recette est en fin de ce fichier.\n"
        + "\n"
        + "Sans --appliquer : mode à blanc, rien n'est écrit sur GitHub.\n"
        + "\n"
        + "options:\n"
        + "  -h, --help   show this help message and exit\n"
        + "  --appliquer  crée réellement le projet et ses champs (sans : mode à blanc)\n";

    public sealed record Resultat(Arguments? Valeurs, bool DemandeAide, string? ErreurUsage);

    public static Resultat Analyser(string[] args)
    {
        bool appliquer = false;

        foreach (var a in args)
        {
            if (a is "-h" or "--help")
            {
                return new Resultat(null, true, null);
            }
            if (a == "--appliquer")
            {
                appliquer = true;
                continue;
            }
            return new Resultat(null, false, $"unrecognized arguments: {a}");
        }
        return new Resultat(new Arguments(appliquer), false, null);
    }
}
