namespace CreerIssues;

/// <summary>
/// Construction du corps d'issue conforme au modèle de `methode-de-ticket.md
/// §1` : une zone engendrée (renvoi vers la tâche de plan, renvois dérivés
/// vers les tickets de `Dépend de` et `En conflit avec`, estampille de
/// provenance), le marqueur de séparation, puis la zone écrite à la main —
/// préservée telle quelle si un corps existant en porte une. Hors socle
/// « lecture du plan ».
/// </summary>
internal static class CorpsIssue
{
    public const string MarqueurSeparation =
        "── engendré — régénérable, écrasé à chaque exécution — "
        + "sous cette ligne : écrit à la main ──";

    public const string ZoneManuelleParDefaut = "\n\n## Pris par\n\n## Notes d'exécution\n\n## Écart constaté\n";

    /// <summary>
    /// Isole ce qui suit le marqueur de séparation dans un corps existant :
    /// cette zone n'est jamais régénérée (`methode-de-ticket.md §1`). Si le
    /// marqueur est absent (corps antérieur à ce modèle), le corps existant
    /// est reporté intégralement plutôt qu'écrasé — mieux vaut le préserver
    /// en entier que risquer d'effacer des notes déjà écrites à la main
    /// faute de savoir où elles commencent.
    /// </summary>
    public static string ExtraireZoneManuelle(string? corpsExistant)
    {
        if (string.IsNullOrEmpty(corpsExistant))
        {
            return ZoneManuelleParDefaut;
        }
        int index = corpsExistant.IndexOf(MarqueurSeparation, StringComparison.Ordinal);
        if (index == -1)
        {
            return "\n\n" + StripperCaractere(corpsExistant, '\n') + "\n";
        }
        return corpsExistant[(index + MarqueurSeparation.Length)..];
    }

    /// <summary>
    /// Équivalent de <c>str.strip(chars)</c> avec un jeu de caractères
    /// explicite (ici, le seul saut de ligne) : ne retire QUE ce caractère,
    /// à la différence de <see cref="TexteUnicode.StripPython"/> qui retire
    /// tout blanc Python.
    /// </summary>
    private static string StripperCaractere(string texte, char caractere)
    {
        int debut = 0, fin = texte.Length;
        while (debut < fin && texte[debut] == caractere) debut++;
        while (fin > debut && texte[fin - 1] == caractere) fin--;
        return texte[debut..fin];
    }

    public static string Construire(
        string tbId, string urlBlobPlan, string revisionPlan, IReadOnlyDictionary<string, string> champs,
        IReadOnlyDictionary<string, int> numeroIssueParTb, string? corpsExistant)
    {
        string renvoisDependDe = TranchesReferences.ConstruireRenvois(
            TranchesReferences.DecouperIdentifiants(champs.GetValueOrDefault("Dépend de", "—"), ','),
            numeroIssueParTb);
        string renvoisEnConflit = TranchesReferences.ConstruireRenvois(
            TranchesReferences.DecouperIdentifiants(champs.GetValueOrDefault("En conflit avec", "—"), ';'),
            numeroIssueParTb);

        string zoneEngendree =
            $"**Tâche de plan** : [`{Chemins.PlanRelatif}`]({urlBlobPlan}) — {tbId}\n"
            + $"**Dépend de** : {renvoisDependDe}\n"
            + $"**En conflit avec** : {renvoisEnConflit}\n"
            + $"**Estampille de provenance** : {Chemins.PlanRelatif}@{revisionPlan} — {tbId}";

        return $"{zoneEngendree}\n\n{MarqueurSeparation}{ExtraireZoneManuelle(corpsExistant)}";
    }
}
