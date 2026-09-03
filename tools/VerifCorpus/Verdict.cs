namespace VerifCorpus;

/// <summary>
/// Résultat structuré d'une passe de vérification du corpus. Le point d'entrée
/// traduit ce verdict en code de sortie ; la logique elle-même n'analyse jamais
/// sa propre sortie texte pour savoir si un défaut a été rapporté.
/// </summary>
internal sealed record Verdict(
    int LiensCibleInexistante,
    int LiensAncreInexistante,
    int FichiersOrphelins,
    int BlocsDesequilibres)
{
    /// <summary>
    /// Vrai si au moins un des quatre compteurs arbitrés comme « défaut rapporté »
    /// est non nul — c'est cette propriété, et elle seule, qui distingue le code
    /// de sortie 0 du code de sortie 1.
    /// </summary>
    public bool ADesDefauts =>
        LiensCibleInexistante > 0
        || LiensAncreInexistante > 0
        || FichiersOrphelins > 0
        || BlocsDesequilibres > 0;
}
