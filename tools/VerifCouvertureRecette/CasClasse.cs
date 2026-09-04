namespace VerifCouvertureRecette;

/// <summary>
/// Le classement retenu pour un cas de recette, avec le contexte qui l'explique
/// — son use case et le jalon lu pour lui (§4) quand la classe en dépend, <c>null</c>
/// sinon (<see cref="Classe.Cite"/>, <see cref="Classe.Synthese"/>).
/// </summary>
internal sealed record CasClasse(CasRecette Cas, Classe Classe, string? UseCase, string? Jalon);
