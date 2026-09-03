namespace VerifMailleAgregat;

/// <summary>
/// Résultat structuré d'une passe de vérification de la maille de désignation.
/// Le point d'entrée traduit ce verdict en code de sortie ; la logique
/// elle-même n'analyse jamais sa propre sortie texte pour savoir si un défaut
/// a été rapporté.
/// </summary>
internal sealed record Verdict(
    int ItemsNeSeResolvantPas,
    int EcartsConflits,
    int IdentifiantsAmbigus,
    int RenvoisMorts)
{
    /// <summary>
    /// Vrai si au moins un des quatre compteurs arbitrés comme « défaut rapporté »
    /// est non nul. Le périmètre « en attente de J0 » n'entre délibérément pas
    /// dans cette liste (arbitrage explicite) : un item non clôturable avant J0
    /// n'est ni une erreur ni un module vérifié, et ne doit pas rendre un code
    /// de sortie non nul sur un plan par ailleurs sain.
    /// </summary>
    public bool ADesDefauts =>
        ItemsNeSeResolvantPas > 0
        || EcartsConflits > 0
        || IdentifiantsAmbigus > 0
        || RenvoisMorts > 0;
}
