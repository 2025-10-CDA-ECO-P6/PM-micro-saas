namespace ComparateurJournaux;

/// <summary>
/// Résultat structuré d'une comparaison de deux journaux. Le point d'entrée
/// traduit ce verdict en code de sortie ; la logique elle-même n'analyse
/// jamais sa propre sortie texte pour savoir si une divergence a été
/// rapportée.
/// </summary>
internal sealed record Verdict(int Divergences)
{
    /// <summary>
    /// Vrai si au moins une divergence a été rapportée — c'est cette
    /// propriété, et elle seule, qui distingue le code de sortie 0 du code
    /// de sortie 1.
    /// </summary>
    public bool ADesDefauts => Divergences > 0;
}
