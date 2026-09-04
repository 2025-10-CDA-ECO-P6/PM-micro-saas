namespace VerifCouvertureRecette;

/// <summary>
/// Contrôle la couverture des cas de recette du cahier (§10) par le plan de
/// travail : tout cas du périmètre engagé (jalon J0/J1) qu'aucune tranche ne
/// cite — ni en critère d'acceptation, ni en exclusion, ni en trou balisé — est
/// un trou silencieux. Contrôle distinct, rendu dans le même rapport : toute
/// colonne Source du cahier qui renvoie vers une règle métier retirée. Ne lit
/// et n'écrit rien au-delà de la racine reçue et du <see cref="TextWriter"/>
/// fourni ; ne modifie jamais le corpus qu'il lit.
/// </summary>
internal static class VerificateurCouvertureRecette
{
    public static Verdict Executer(
        string racine, string cheminCahierRelatif, string cheminPlanRelatif, TextWriter sortie)
    {
        string texteCahier = CahierRecette.LireTexte(racine, cheminCahierRelatif);
        var casReels = CahierRecette.ExtraireCasReels(texteCahier);

        string textePlan = CitationsPlan.LireTexte(racine, cheminPlanRelatif);
        var cites = CitationsPlan.ExtraireIdentifiantsCites(textePlan);
        var jalonParEpique = VueDesEpiques.ExtraireJalonParEpique(textePlan);

        var classes = Classificateur.Classer(casReels, cites, jalonParEpique);
        classes.Sort((a, b) => string.CompareOrdinal(a.Cas.Id, b.Cas.Id));

        var trous = classes.Where(c => c.Classe == Classe.AbsentTrou).ToList();
        var retires = classes.Where(c => c.Classe == Classe.Retire).ToList();
        var sourcesRetirees = casReels
            .Where(c => c.Source.Contains("retirée", StringComparison.Ordinal))
            .OrderBy(c => c.Id, StringComparer.Ordinal)
            .ToList();

        EcrireRapport(sortie, casReels.Count, classes, trous, retires, sourcesRetirees);

        return new Verdict(trous.Count, sourcesRetirees.Count);
    }

    private static void EcrireRapport(
        TextWriter sortie,
        int totalCasReels,
        List<CasClasse> classes,
        List<CasClasse> trous,
        List<CasClasse> retires,
        List<CasRecette> sourcesRetirees)
    {
        int Compte(Classe c) => classes.Count(x => x.Classe == c);

        sortie.WriteLine($"cahier de recette (§10) — cas de recette réels extraits : {totalCasReels}");
        sortie.WriteLine();
        sortie.WriteLine("classement des absences face au plan de travail :");
        sortie.WriteLine($"  RETIRÉ (le cahier le marque mort, ne pèse pas)  : {retires.Count}");
        foreach (var r in retires.Take(40))
        {
            sortie.WriteLine($"    [RETIRÉ] {r.Cas.Id} -> {r.Cas.Cas}");
        }
        sortie.WriteLine($"  CITÉ                                           : {Compte(Classe.Cite)}");
        sortie.WriteLine($"  ABSENT — JALON ULTÉRIEUR (J2/J3, légitime)     : {Compte(Classe.AbsentJalonUlterieur)}");
        sortie.WriteLine($"  ABSENT — HORS MVP (use case hors §4, légitime) : {Compte(Classe.AbsentHorsMvp)}");
        sortie.WriteLine($"  SYNTHÈSE (second niveau, cite d'autres CR-)    : {Compte(Classe.Synthese)}");
        sortie.WriteLine($"  ABSENT — TROU (ni cité, ni légitime)           : {trous.Count}");
        foreach (var t in trous.Take(40))
        {
            sortie.WriteLine($"    [TROU] {t.Cas.Id} ({t.UseCase}, jalon {t.Jalon}) -> cité par aucune tranche du plan");
            sortie.WriteLine($"        Source (cahier) : {t.Cas.Source}");
        }
        if (trous.Count > 0)
        {
            // Limite assumée, pas devinée : §4 ne porte le jalon qu'au grain du
            // use case. Une story qui dépend elle-même d'un use case de jalon
            // ultérieur (ex. US-01-05 → migration → UC-10, J2) n'a aucun champ
            // pour l'exprimer dans le plan — son report n'y est visible que par
            // l'absence de la colonne `Tranches`, que cet outil ne peut pas
            // distinguer d'un oubli sans lire les dépendances de chaque story
            // (donnée que le corpus ne porte pas à ce grain). La Source
            // ci-dessus est affichée précisément pour que cette adjudication se
            // fasse en une passe, sans rouvrir le cahier une ligne à la fois.
            sortie.WriteLine();
            sortie.WriteLine("limite assumée : le jalon ci-dessus est dérivé au grain du use case (§4), faute d'un");
            sortie.WriteLine("champ de jalon au grain de la story dans le plan — un trou dont la Source nomme une");
            sortie.WriteLine("story dépendant elle-même d'un use case de jalon ultérieur demande une adjudication");
            sortie.WriteLine("humaine, pas un classement automatique supplémentaire.");
        }

        sortie.WriteLine();
        sortie.WriteLine($"cahier §13 — contrôle d'intégrité des citations : sources déclarées retirées : {sourcesRetirees.Count}");
        foreach (var s in sourcesRetirees.Take(40))
        {
            sortie.WriteLine($"    [SOURCE RETIRÉE] {s.Id} -> {s.Source}");
        }
    }
}
