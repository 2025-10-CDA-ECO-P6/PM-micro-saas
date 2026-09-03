namespace CreerIssues;

/// <summary>
/// Chemins relatifs à la racine du dépôt que la lecture du plan désigne.
/// Fait partie du socle « lecture du plan » : la vague suivante (report vers
/// un projet GitHub) réutilise déjà <see cref="PlanRelatif"/> tel quel — voir
/// <c>CreerProjet.Orchestrateur.Executer</c>, qui le lit via
/// <c>using CreerIssues;</c> plutôt que d'en recopier une forme locale.
/// </summary>
internal static class Chemins
{
    public const string PlanRelatif = "docs/gestion-projet/plan-de-travail.md";
    public const string MethodeRelatif = "docs/gestion-projet/methode-de-ticket.md";
    public const string WireframesRelatif = "docs/conception/interface/wireframes";
}
