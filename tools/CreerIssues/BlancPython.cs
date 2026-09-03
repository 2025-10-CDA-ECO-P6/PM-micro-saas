namespace CreerIssues;

/// <summary>
/// Classe de caractères à insérer dans un motif d'expression régulière .NET
/// pour reproduire <c>\s</c> de Python : celui-ci inclut les quatre
/// séparateurs de contrôle U+001C–U+001F et le retour à la ligne suivante
/// U+0085 (NEL) — aucun des cinq n'est reconnu par <c>\s</c> de .NET — ainsi
/// que les catégories Unicode Zs/Zl/Zp ; utiliser <c>\s</c> tel quel
/// déplacerait la coupure du découpage du périmètre d'écriture
/// (<see cref="ResolutionMaille.DecouperPerimetre"/>) ou du motif de fiche
/// d'écran (<see cref="ResolutionMaille.MapperNature"/>) sur une valeur de
/// champ portant l'un de ces cinq caractères.
///
/// Arbitrage ouvert, non corrigé : divergence propre à ce fichier, jamais
/// signalée comme un défaut du script Python d'origine — reproduite ici
/// délibérément, à l'identique.
/// </summary>
internal static class BlancPython
{
    /// <summary>Classe de caractères nue, à composer avec un quantificateur (<c>*</c>, <c>+</c>).</summary>
    public const string Classe = "[\\t\\n\\v\\f\\r\\x1c\\x1d\\x1e\\x1f \\x85\\p{Zs}\\p{Zl}\\p{Zp}]";
}
