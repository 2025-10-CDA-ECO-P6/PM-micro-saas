using System.Text.RegularExpressions;

namespace VerifCouvertureRecette;

/// <summary>
/// Dérive, pour chaque cas de recette réel, la classe qui explique son absence
/// du plan — ou son caractère non applicable (§10 « Cas transverses »). Aucune
/// classe n'est codée en dur par identifiant : tout se déduit de la colonne
/// Source du cas et de la lecture du plan (citations, §4).
/// </summary>
internal static class Classificateur
{
    private static readonly Regex MotifUseCase = new(
        @"^CR-UC(\d+)-\d+$", RegexOptions.CultureInvariant);

    // Forme mesurée sur les deux seules occurrences du cahier (CR-UC01-04,
    // CR-UC02-11) : la colonne `Cas` ouvre sur cette marque italique exacte.
    // `Contains`, pas `StartsWith`, pour ne pas casser sur un futur préfixe de
    // catégorie (`[erreur] *(retiré)*`) qu'aucun des deux cas actuels ne porte.
    private const string MarqueRetire = "*(retiré)*";

    public static List<CasClasse> Classer(
        IReadOnlyList<CasRecette> casReels,
        IReadOnlySet<string> citesParLePlan,
        IReadOnlyDictionary<string, string> jalonParEpique)
    {
        return casReels.Select(cas => ClasserUn(cas, citesParLePlan, jalonParEpique)).ToList();
    }

    private static CasClasse ClasserUn(
        CasRecette cas,
        IReadOnlySet<string> citesParLePlan,
        IReadOnlyDictionary<string, string> jalonParEpique)
    {
        // Priorité absolue, mandatée par l'opérateur : le cahier marque lui-même
        // ce cas comme retiré dans sa propre colonne Cas (jamais Source, qui
        // porte un défaut distinct — §13). Un cas mort n'a rien à prouver, donc
        // rien à citer : ni sa Source (synthèse) ni sa présence au plan
        // n'entrent en jeu une fois ce marqueur trouvé.
        if (cas.Cas.Contains(MarqueRetire, StringComparison.Ordinal))
        {
            return new CasClasse(cas, Classe.Retire, UseCase: null, Jalon: null);
        }

        // Couche de second niveau (§10 « Cas transverses ») : une colonne Source
        // qui ne cite que d'autres cas de recette, jamais une story, n'est pas un
        // trou de traçabilité vers le plan — elle se recette via les cas qu'elle
        // recoupe, pas directement vers une US. Vérifié en premier : ce classement
        // prime sur toute citation ou tout jalon.
        bool citeUnCas = cas.Source.Contains("CR-", StringComparison.Ordinal);
        bool citeUneStory = cas.Source.Contains("US-", StringComparison.Ordinal);
        if (citeUnCas && !citeUneStory)
        {
            return new CasClasse(cas, Classe.Synthese, UseCase: null, Jalon: null);
        }

        if (citesParLePlan.Contains(cas.Id))
        {
            return new CasClasse(cas, Classe.Cite, UseCase: null, Jalon: null);
        }

        var m = MotifUseCase.Match(cas.Id);
        if (!m.Success)
        {
            // Forme non rattachable à un use case (ni `CR-UCnn-kk`, ni synthèse) :
            // aucune donnée du corpus ne permet d'en dériver un jalon — absence
            // traitée comme légitime plutôt qu'en trou fabriqué sans preuve.
            return new CasClasse(cas, Classe.AbsentHorsMvp, UseCase: null, Jalon: null);
        }

        string useCase = $"US-UC-{m.Groups[1].Value}";
        if (!jalonParEpique.TryGetValue(useCase, out string? jalon))
        {
            return new CasClasse(cas, Classe.AbsentHorsMvp, useCase, Jalon: null);
        }

        bool ulterieur = jalon.Contains("J2", StringComparison.Ordinal) || jalon.Contains("J3", StringComparison.Ordinal);
        return new CasClasse(cas, ulterieur ? Classe.AbsentJalonUlterieur : Classe.AbsentTrou, useCase, jalon);
    }
}
