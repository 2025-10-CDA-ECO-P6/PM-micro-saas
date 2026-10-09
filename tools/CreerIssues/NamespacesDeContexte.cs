namespace CreerIssues;

/// <summary>
/// Les namespaces de bounded context que le § 2 du plan n'admet comme module
/// que « sur la seule tranche qui les crée », et l'identifiant de cette tranche
/// (vide si le plan ne nomme aucun namespace).
/// </summary>
internal sealed record NamespacesDeContexte(HashSet<string> Noms, string TrancheCreatrice)
{
    /// <summary>
    /// Les namespaces qui se résolvent comme module sur la tranche
    /// <paramref name="idTache"/> : tous sur la tranche qui les crée, aucun ailleurs.
    /// </summary>
    public IReadOnlySet<string> ResolublesSur(string idTache) =>
        string.Equals(idTache, TrancheCreatrice, StringComparison.Ordinal) ? Noms : new HashSet<string>(StringComparer.Ordinal);
}
