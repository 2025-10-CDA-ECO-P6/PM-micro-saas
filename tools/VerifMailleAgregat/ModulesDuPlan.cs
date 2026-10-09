namespace VerifMailleAgregat;

/// <summary>
/// Les modules que le § 2 du plan nomme lui-même, lus par <see cref="ModulesOutillage"/> :
/// ce que le plan énonce n'est jamais recopié dans le code de cet outil.
/// </summary>
internal sealed record ModulesDuPlan(
    HashSet<string> Outillage,
    HashSet<string> Transverses,
    HashSet<string> Client,
    NamespacesDuPlan Namespaces);
