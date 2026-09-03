namespace CreerIssues;

/// <summary>Les trois axes de libellés (§3 de `methode-de-ticket.md`).</summary>
internal sealed record TaxonomieLibelles(List<string> Jalon, List<string> NatureTranche, List<string> Nature);

/// <summary>
/// Lecture de la taxonomie de libellés — hors socle « lecture du plan » :
/// `CreerProjet` ne la consomme pas (il lit ses propres états via
/// `methode-de-ticket.md §5`, une structure distincte).
/// </summary>
internal static class Taxonomie
{
    /// <summary>
    /// Lit les trois axes de libellés (§3 de `methode-de-ticket.md`) : leurs
    /// valeurs, jamais codées en dur ici, sont celles lues dans le document.
    /// </summary>
    public static TaxonomieLibelles Extraire(string contenuMethode)
    {
        int iJalon = contenuMethode.IndexOf("**Par jalon**", StringComparison.Ordinal);
        int iCouche = contenuMethode.IndexOf("**Par nature de tranche**", StringComparison.Ordinal);
        int iNature = contenuMethode.IndexOf("**Par nature de vérification**", StringComparison.Ordinal);
        int iFin = contenuMethode.IndexOf("**Interdiction nommée", StringComparison.Ordinal);

        bool positionsManquantes = iJalon == -1 || iCouche == -1 || iNature == -1 || iFin == -1;
        bool ordreRespecte = iJalon < iCouche && iCouche < iNature && iNature < iFin;
        if (positionsManquantes || !ordreRespecte)
        {
            throw new ErreurCorpusException(
                "la structure attendue de la taxonomie de libellés (§3 de "
                + $"{Chemins.MethodeRelatif}) est introuvable — le document a changé de forme, "
                + "ce script doit être corrigé avant de continuer");
        }

        return new TaxonomieLibelles(
            ExtraireValeurs(contenuMethode[iJalon..iCouche]),
            ExtraireValeurs(contenuMethode[iCouche..iNature]),
            ExtraireValeurs(contenuMethode[iNature..iFin]));
    }

    private static List<string> ExtraireValeurs(string bloc) =>
        TableauMarkdown.AnalyserLignes(bloc).Select(ligne => ligne.Premiere).ToList();
}
