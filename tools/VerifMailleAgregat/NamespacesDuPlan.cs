namespace VerifMailleAgregat;

/// <summary>
/// Les namespaces de bounded context que le § 2 du plan n'admet comme module
/// que « sur la seule tranche qui les crée », et l'identifiant de cette tranche :
/// l'un et l'autre lus dans le plan, jamais recopiés. Même contenu que
/// <c>NamespacesDeContexte</c> de CreerIssues, que cet outil ne peut pas inclure
/// (voir <see cref="ModulesOutillage"/>).
/// </summary>
internal sealed record NamespacesDuPlan(HashSet<string> Noms, string TrancheCreatrice)
{
    /// <summary>
    /// Vrai si <paramref name="item"/> est un namespace de contexte ET que la tâche
    /// <paramref name="idTache"/> est la tranche qui les crée.
    /// </summary>
    public bool ResoutSur(string item, string idTache) =>
        Noms.Contains(item) && string.Equals(idTache, TrancheCreatrice, StringComparison.Ordinal);
}
