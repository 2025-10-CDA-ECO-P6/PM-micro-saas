namespace CreerIssues;

/// <summary>
/// Le garde-fou qui refuse d'écrire si la révision courante n'est pas
/// atteignable depuis le distant — sans quoi le corps d'issue produirait des
/// renvois pointant une page introuvable. Le constat se fait sans contacter
/// le réseau (uniquement des refs locales). Hors socle « lecture du plan » :
/// propre à la création d'issues, `CreerProjet` ne le consomme pas.
/// </summary>
internal static class RevisionGuard
{
    public static string LireBrancheCourante()
    {
        var resultat = Processus.Executer("git", "rev-parse", "--abbrev-ref", "HEAD");
        string branche = TexteUnicode.StripPython(resultat.Stdout);
        if (resultat.Code != 0 || branche.Length == 0 || branche == "HEAD")
        {
            return "main";
        }
        return branche;
    }

    public static string LireCommitCourant() => TexteUnicode.StripPython(Processus.ExecuterGit("rev-parse", "HEAD"));

    /// <summary>
    /// Révision du plan de travail au sens de `methode-de-ticket.md §1` — pas
    /// HEAD, mais le dernier commit qui a modifié le fichier lui-même. C'est
    /// cette valeur, gelée au moment de la génération (§6, clause du gel
    /// assumé), que l'estampille de provenance porte.
    /// </summary>
    public static string LireRevisionPlan()
    {
        string revision = TexteUnicode.StripPython(
            Processus.ExecuterGit("log", "-1", "--format=%H", "--", Chemins.PlanRelatif));
        if (revision.Length == 0)
        {
            throw new ErreurCorpusException(
                $"aucun historique git pour {Chemins.PlanRelatif} : impossible de dater "
                + "l'estampille de provenance");
        }
        return revision;
    }

    /// <summary>
    /// Constate, sans contacter le distant, si <paramref name="commit"/> est
    /// déjà atteignable depuis `origin/&lt;branche&gt;` ou, à défaut,
    /// `origin/main`, tel que connu localement.
    /// </summary>
    public static bool CommitPresentSurDistant(string commit, string branche)
    {
        foreach (var reference in new[] { $"origin/{branche}", "origin/main" })
        {
            var existe = Processus.Executer("git", "rev-parse", "--verify", "-q", reference);
            if (existe.Code != 0) continue;

            var ancetre = Processus.Executer("git", "merge-base", "--is-ancestor", commit, reference);
            return ancetre.Code == 0;
        }
        return false;
    }
}
