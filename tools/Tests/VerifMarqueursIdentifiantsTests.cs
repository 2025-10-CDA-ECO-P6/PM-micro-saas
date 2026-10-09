// Caractérise le comportement observable de `tools/VerifMarqueursIdentifiants` sur son
// binaire compilé : sous-processus, sortie standard lue, jamais l'API interne
// (même stratégie boîte noire que `VerifCorpusTests`).
//
// Chaque contrôle a trois épreuves, parce que « 0 problème » peut venir d'une
// recherche trop étroite comme d'une acceptation trop large :
// 1. un défaut injecté qu'il doit trouver ;
// 2. un cas légitime voisin qu'il ne doit pas signaler (faux positif) ;
// 3. la liste de référence : un écart qu'elle accepte ne fait pas échouer, un
//    écart qu'elle ne couvre plus ou qu'elle dépasse fait échouer, une entrée
//    devenue sans objet est imprimée sans faire échouer.
using Xunit;

namespace CaracterisationOutils;

/// <summary>
/// Dépôt de fixture commun aux deux contrôles : un <c>docs/</c> à remplir, des
/// listes de référence écrites hors de <c>docs/</c>.
/// </summary>
public abstract class DepotDeControle : IDisposable
{
    private readonly RepertoireTemporaire _tmp = new();

    protected string Depot => _tmp.Chemin;

    protected DepotDeControle()
    {
        Helpers.InitialiserDepotGit(Depot);
    }

    public void Dispose() => _tmp.Dispose();

    protected void EcrireDoc(string cheminDansDocs, string contenu) =>
        Helpers.Ecrire(Path.Combine(Depot, "docs", cheminDansDocs), contenu);

    protected string EcrireReference(string nom, string contenu)
    {
        string chemin = Path.Combine(Depot, "references", nom);
        Helpers.Ecrire(chemin, contenu);
        return chemin;
    }

    internal ResultatExecution Lancer(string? referenceMarqueurs = null, string? referenceIdentifiants = null)
    {
        var args = new List<string> { Depot };
        if (referenceMarqueurs is not null) args.AddRange(["--reference-marqueurs", referenceMarqueurs]);
        if (referenceIdentifiants is not null) args.AddRange(["--reference-identifiants", referenceIdentifiants]);
        return Helpers.ExecuterOutil("VerifMarqueursIdentifiants", args);
    }
}

/// <summary>
/// Vérité 2 — un document qui porte « RÉSOLU » comme marqueur de statut est
/// signalé avec fichier et ligne ; le même mot en prose ordinaire ne l'est pas.
/// </summary>
public class ControleDesMarqueurs : DepotDeControle
{
    [Fact]
    public void DevraitSignalerResoluCommeMarqueurDeStatutAvecFichierEtLigne()
    {
        EcrireDoc("projet/points.md", "# Points\n\n- **RÉSOLU** — le nom du store est fixé.\n");

        var resultat = Lancer();

        Assert.Equal(1, resultat.CodeSortie);
        Assert.Contains("docs/projet/points.md:3", resultat.Stdout);
        Assert.Contains("RÉSOLU", resultat.Stdout);
    }

    [Theory]
    [InlineData("**Clos (Q#1)** — le nom suffit.", "Clos")]
    [InlineData("Seuil de pondération — **FERMÉE** : décision prise.", "FERMÉE")]
    [InlineData("5. **Espace personnel** — TRANCHÉ (décision produit).", "TRANCHÉ")]
    [InlineData("| Interface — RÉSOLU | décision prise |", "RÉSOLU")]
    [InlineData("Le point reste ouvert [RÉSOLU — ADR-018].", "RÉSOLU")]
    [InlineData("Le point est [NOUVEAU STATUT — motif].", "NOUVEAU STATUT")]
    [InlineData("- **FLAG JURISTE** : quel instant fait foi ?", "FLAG JURISTE")]
    [InlineData("- [REPORTÉ — J2] le chiffrage détaillé.", "REPORTÉ")]
    [InlineData("- **REPORTÉ** au jalon J2.", "REPORTÉ")]
    public void DevraitSignalerChaqueFormeConvertieOuHorsListeEmployeeCommeStatut(string ligne, string marqueur)
    {
        EcrireDoc("projet/points.md", $"# Points\n\n{ligne}\n");

        var resultat = Lancer();

        Assert.Equal(1, resultat.CodeSortie);
        Assert.Contains("docs/projet/points.md:3", resultat.Stdout);
        Assert.Contains($"« {marqueur} »", resultat.Stdout);
    }

    [Fact]
    public void DevraitLaisserPasserLesMarqueursDeLaListeFermeeEtDesAutresListesDeclarees()
    {
        EcrireDoc("projet/points.md",
            "# Points\n\n"
            + "- **À TRANCHER** : le seuil.\n"
            + "- [À TRANCHER — ticket] et [SOUS-SPÉCIFIÉ — motif].\n"
            + "- **DÉCIDÉ** (2026-10-07, ADR-019) : le registre.\n"
            + "- Retiré. Remplacé par ADR-020.\n"
            + "- [À VALIDER — JURISTE] [HORS PÉRIMÈTRE — ADR-018] [ABSENT PAR NATURE — motif] [DÉSACTIVÉ — condition]\n"
            + "- [HORS-MVP — S8] TROU, HORS MAILLE.\n"
            + "- Migration ascendante du payload : dette nommée, post-MVP (ADR-016, § 1.4).\n");

        var resultat = Lancer();

        Assert.Equal(0, resultat.CodeSortie);
        Assert.Contains("écarts nouveaux                           : 0", resultat.Stdout);
    }

    [Fact]
    public void DevraitLaisserPasserLesMotsDeStatutEnProseOrdinaireEnCodeEnLienEnEnteteDeColonne()
    {
        EcrireDoc("projet/points.md",
            "# Points\n\n"
            + "Le point est tranché ; la question est résolue et la liste fermée.\n"
            + "Les formes `RÉSOLU` et `Clos` sont converties.\n"
            + "| Marqueur | Clos par |\n"
            + "|---|---|\n"
            + "Voir [README](README.md) et [UC-HORS-MVP — Gel](exclusions.md) ou [MCD][ref].\n"
            + "Date [YYYY-MM-DD], motif `[TPJD]-[0-9]{2}`, acronyme [MJ].\n"
            + "```\n"
            + "RÉSOLU — dans un bloc de code\n"
            + "[NOUVEAU STATUT — motif]\n"
            + "```\n");

        var resultat = Lancer();

        Assert.Equal(0, resultat.CodeSortie);
        Assert.Contains("écarts nouveaux                           : 0", resultat.Stdout);
    }

    [Fact]
    public void DevraitLaisserPasserLesFormesConvertiesNommeesEntreAccentsGravesDansLeGlossaireEtLADR019()
    {
        EcrireDoc("glossaire.md", "# Glossaire\n\nLes formes `RÉSOLU` et `Clos` sont converties.\n");
        EcrireDoc(
            "decisions/ADR-019-conventions-de-tenue-du-corpus.md",
            "# ADR-019 — Conventions\n\nLes formes `RÉSOLU`, `FERMÉE`, `Clos` et `TRANCHÉ` sont converties.\n");

        var resultat = Lancer();

        Assert.Equal(0, resultat.CodeSortie);
    }

    [Theory]
    [InlineData("glossaire.md")]
    [InlineData("decisions/ADR-019-conventions-de-tenue-du-corpus.md")]
    public void DevraitSignalerUnMarqueurResoluEmployeCommeStatutDansLeGlossaireEtLADR019(string document)
    {
        EcrireDoc(document, "# Titre\n\n- [RÉSOLU — x]\n");

        var resultat = Lancer();

        Assert.Equal(1, resultat.CodeSortie);
        Assert.Contains($"docs/{document}:3", resultat.Stdout);
    }

    [Fact]
    public void DevraitAccepterUnEcartDeLaListeDeReferenceSansEchouer()
    {
        EcrireDoc("projet/points.md", "# Points\n\n- **RÉSOLU** — le nom du store est fixé.\n");
        string reference = EcrireReference("marqueurs.txt", "# commentaire\n\ndocs/projet/points.md|RÉSOLU|1\n");

        var resultat = Lancer(referenceMarqueurs: reference);

        Assert.Equal(0, resultat.CodeSortie);
        Assert.Contains("écarts acceptés par la liste de référence : 1", resultat.Stdout);
        Assert.DoesNotContain("ÉCART NOUVEAU", resultat.Stdout);
    }

    [Fact]
    public void DevraitEchouerSurUneOccurrenceAuDelaDeCellesQueLaListeDeReferenceAccepte()
    {
        EcrireDoc("projet/points.md", "# Points\n\n- **RÉSOLU** — le nom du store.\n- **RÉSOLU** — le nom du jalon.\n");
        string reference = EcrireReference("marqueurs.txt", "docs/projet/points.md|RÉSOLU|1\n");

        var resultat = Lancer(referenceMarqueurs: reference);

        Assert.Equal(1, resultat.CodeSortie);
        Assert.Contains("ÉCART NOUVEAU docs/projet/points.md|RÉSOLU", resultat.Stdout);
        Assert.Contains("docs/projet/points.md:4", resultat.Stdout);
    }

    [Fact]
    public void DevraitImprimerUneEntreeDeLaListeDevenueSansObjetSansFaireEchouer()
    {
        EcrireDoc("projet/points.md", "# Points\n\nRien à signaler.\n");
        string reference = EcrireReference("marqueurs.txt", "docs/projet/points.md|RÉSOLU|1\n");

        var resultat = Lancer(referenceMarqueurs: reference);

        Assert.Equal(0, resultat.CodeSortie);
        Assert.Contains("DISPARU docs/projet/points.md|RÉSOLU", resultat.Stdout);
    }

    [Fact]
    public void DevraitRendreLeCodeDeSortieDeuxSurUneListeDeReferenceMalFormee()
    {
        EcrireDoc("projet/points.md", "# Points\n");
        string reference = EcrireReference("marqueurs.txt", "docs/projet/points.md RÉSOLU\n");

        var resultat = Lancer(referenceMarqueurs: reference);

        Assert.Equal(2, resultat.CodeSortie);
        Assert.Contains("panne d'exécution", resultat.Stderr);
    }
}

/// <summary>
/// Vérité 3 — deux documents qui définissent le même RB-NN-NN sont signalés avec
/// leurs deux emplacements ; une citation n'est pas une définition.
/// </summary>
public class ControleDesIdentifiants : DepotDeControle
{
    [Fact]
    public void DevraitSignalerUneRegleMetierDefinieDansDeuxDocumentsAvecLesDeuxEmplacements()
    {
        EcrireDoc("besoin/stories/US-UC-06-vue-session.md", "# Epic\n\n- RB-06-25 : La vue session se rafraîchit.\n");
        EcrireDoc("besoin/cas-d-utilisation/UC-06-vue-session.md", "# UC-06 — Vue\n\n- **RB-06-25** : La vue session se rafraîchit.\n");

        var resultat = Lancer();

        Assert.Equal(1, resultat.CodeSortie);
        Assert.Contains("ÉCART NOUVEAU RB-06-25", resultat.Stdout);
        Assert.Contains("docs/besoin/stories/US-UC-06-vue-session.md:3", resultat.Stdout);
        Assert.Contains("docs/besoin/cas-d-utilisation/UC-06-vue-session.md:3", resultat.Stdout);
    }

    [Fact]
    public void DevraitSignalerUneStoryDefinieDeuxFoisDansLeMemeFichier()
    {
        EcrireDoc(
            "besoin/stories/US-UC-06-vue-session.md",
            "# Epic\n\n### US-06-04 — Voir la session\n\nTexte.\n\n### US-06-04 — Voir la session (bis)\n");

        var resultat = Lancer();

        Assert.Equal(1, resultat.CodeSortie);
        Assert.Contains("ÉCART NOUVEAU US-06-04", resultat.Stdout);
        Assert.Contains("US-UC-06-vue-session.md:3", resultat.Stdout);
        Assert.Contains("US-UC-06-vue-session.md:7", resultat.Stdout);
    }

    [Fact]
    public void DevraitSignalerUnUseCaseDontLeNomDeFichierPoseLeMemeIdentifiantDansDeuxDossiers()
    {
        EcrireDoc("besoin/cas-d-utilisation/UC-05-organiser-dossiers.md", "# UC-05 — Organiser\n");
        EcrireDoc("archive/UC-05-organiser-autrement.md", "# UC-05 — Autrement\n");

        var resultat = Lancer();

        Assert.Equal(1, resultat.CodeSortie);
        Assert.Contains("ÉCART NOUVEAU UC-05", resultat.Stdout);
        Assert.Contains("docs/archive/UC-05-organiser-autrement.md", resultat.Stdout);
    }

    [Fact]
    public void DevraitSignalerUneLigneDeTableauDefinieDeuxFoisDansLeFichierQuiPorteSaFamille()
    {
        EcrireDoc(
            "assurance/securite/registre-des-traitements.md",
            "# Registre\n\n| Id | Libellé |\n|---|---|\n| PR-01 | Compte |\n| PR-01 | Compte (doublon) |\n");

        var resultat = Lancer();

        Assert.Equal(1, resultat.CodeSortie);
        Assert.Contains("ÉCART NOUVEAU PR-01", resultat.Stdout);
        Assert.Contains("registre-des-traitements.md:5", resultat.Stdout);
        Assert.Contains("registre-des-traitements.md:6", resultat.Stdout);
    }

    [Fact]
    public void DevraitLaisserPasserLesCitationsQuiNeSontPasDesDefinitions()
    {
        EcrireDoc(
            "besoin/stories/US-UC-06-vue-session.md",
            "# Epic\n\n### US-06-04 — Voir la session\n\n- RB-06-25 : La vue se rafraîchit (voir US-06-04).\n\n"
            + "1. **US-06-04** — récapitulatif de l'ordre de livraison\n"
            + "| RB-06-25 : récapitulatif | US-06-04 |\n");
        EcrireDoc(
            "besoin/cas-d-utilisation/UC-06-vue-session.md",
            "# UC-06 — Vue\n\nLa règle RB-06-25 est portée par l'épique.\n- RB-06-25 est citée ici, sans deux-points initial.\n");
        EcrireDoc(
            "tracabilite.md",
            "# Traçabilité\n\n| UC-06 | US-06-04 | RB-06-25 |\n| RB-06-25 | US-06-04 |\n");
        EcrireDoc(
            "assurance/securite/analyse-d-impact.md",
            "# Analyse\n\n- PR-01 : prise de contrôle d'un compte (IR-06).\n- PR-01 : nécessité du traitement.\n");
        EcrireDoc(
            "projet/registre-des-risques.md",
            "# Risques\n\n**T-04 — Reclaim**\n\n- **T-04 (reclaim)** porte trois faces.\n");

        var resultat = Lancer();

        Assert.Equal(0, resultat.CodeSortie);
        Assert.Contains("identifiants définis deux fois", resultat.Stdout);
        Assert.DoesNotContain("ÉCART NOUVEAU", resultat.Stdout);
    }

    [Fact]
    public void DevraitIgnorerUneDefinitionQuiNeFigureQueDansUnBlocDeCode()
    {
        EcrireDoc("besoin/stories/US-UC-06-vue-session.md", "# Epic\n\n- RB-06-25 : La vue se rafraîchit.\n");
        EcrireDoc("conventions.md", "# Conventions\n\n```\n- RB-06-25 : exemple de gabarit\n```\n");

        var resultat = Lancer();

        Assert.Equal(0, resultat.CodeSortie);
    }

    [Fact]
    public void DevraitAccepterUnEcartDeLaListeDeReferenceSansEchouer()
    {
        EcrireDoc("besoin/stories/US-UC-06-vue-session.md", "# Epic\n\n- RB-06-25 : La vue se rafraîchit.\n");
        EcrireDoc("besoin/cas-d-utilisation/UC-06-vue-session.md", "# UC-06 — Vue\n\n- **RB-06-25** : La vue se rafraîchit.\n");
        string reference = EcrireReference(
            "identifiants.txt",
            "# commentaire\nRB-06-25|docs/besoin/stories/US-UC-06-vue-session.md|1\n"
            + "RB-06-25|docs/besoin/cas-d-utilisation/UC-06-vue-session.md|1\n");

        var resultat = Lancer(referenceIdentifiants: reference);

        Assert.Equal(0, resultat.CodeSortie);
        Assert.Contains("écarts acceptés par la liste de référence : 2", resultat.Stdout);
        Assert.DoesNotContain("ÉCART NOUVEAU", resultat.Stdout);
    }

    [Fact]
    public void DevraitEchouerSurUneTroisiemeDefinitionQuandLaListeDeReferenceEnAccepteDeux()
    {
        EcrireDoc("besoin/stories/US-UC-06-vue-session.md", "# Epic\n\n- RB-06-25 : La vue se rafraîchit.\n");
        EcrireDoc("besoin/cas-d-utilisation/UC-06-vue-session.md", "# UC-06 — Vue\n\n- **RB-06-25** : La vue se rafraîchit.\n");
        EcrireDoc("projet/notes.md", "# Notes\n\n- RB-06-25 : encore une définition.\n");
        string reference = EcrireReference(
            "identifiants.txt",
            "RB-06-25|docs/besoin/stories/US-UC-06-vue-session.md|1\n"
            + "RB-06-25|docs/besoin/cas-d-utilisation/UC-06-vue-session.md|1\n");

        var resultat = Lancer(referenceIdentifiants: reference);

        Assert.Equal(1, resultat.CodeSortie);
        Assert.Contains("ÉCART NOUVEAU RB-06-25", resultat.Stdout);
        Assert.Contains("docs/projet/notes.md:3", resultat.Stdout);
    }

    [Fact]
    public void DevraitSignalerUneDefinitionAccepteeQuiChangeDeFichier()
    {
        // D21 : la liste accepte RB-06-25 dans deux fichiers. La seconde définition
        // quitte cas-d-utilisation/ pour projet/ : le nombre de définitions reste
        // deux, mais l'une d'elles n'est plus celle que la liste accepte.
        EcrireDoc("besoin/stories/US-UC-06-vue-session.md", "# Epic\n\n- RB-06-25 : La vue session se rafraîchit.\n");
        EcrireDoc("projet/notes.md", "# Notes\n\n- **RB-06-25** : La vue session se rafraîchit.\n");
        string reference = EcrireReference(
            "identifiants.txt",
            "RB-06-25|docs/besoin/stories/US-UC-06-vue-session.md|1\n"
            + "RB-06-25|docs/besoin/cas-d-utilisation/UC-06-vue-session.md|1\n");

        var resultat = Lancer(referenceIdentifiants: reference);

        Assert.Equal(1, resultat.CodeSortie);
        Assert.Contains("ÉCART NOUVEAU RB-06-25|docs/projet/notes.md : 1 constat(s)", resultat.Stdout);
        Assert.Contains("docs/projet/notes.md:3", resultat.Stdout);
        // La définition restée en place est toujours acceptée ; seule l'ancienne place est à retirer.
        Assert.DoesNotContain("DISPARU RB-06-25|docs/besoin/stories/US-UC-06-vue-session.md", resultat.Stdout);
        Assert.Contains("DISPARU RB-06-25|docs/besoin/cas-d-utilisation/UC-06-vue-session.md", resultat.Stdout);
    }

    [Fact]
    public void DevraitEchouerSurUnAutreIdentifiantQueCeluiQueLaListeDeReferenceAccepte()
    {
        EcrireDoc("besoin/stories/US-UC-06-vue-session.md", "# Epic\n\n- RB-06-26 : La vue se rafraîchit.\n- RB-06-26 : Doublon.\n");
        string reference = EcrireReference(
            "identifiants.txt",
            "RB-06-25|docs/besoin/stories/US-UC-06-vue-session.md|2\n");

        var resultat = Lancer(referenceIdentifiants: reference);

        Assert.Equal(1, resultat.CodeSortie);
        Assert.Contains("ÉCART NOUVEAU RB-06-26", resultat.Stdout);
        Assert.Contains("DISPARU RB-06-25", resultat.Stdout);
    }
}

/// <summary>
/// Golden master — le corpus réel, mesuré en lecture seule, avec les listes de
/// référence versionnées à côté de l'outil.
/// </summary>
public class SurLeCorpusReelDesMarqueursEtDesIdentifiants
{
    [Fact]
    public void DevraitNeSignalerAucunEcartNouveauSurLeCorpusReel()
    {
        var resultat = Helpers.ExecuterOutil("VerifMarqueursIdentifiants", [Helpers.RacineDepot]);

        Assert.Equal(0, resultat.CodeSortie);
        Assert.Equal(2, System.Text.RegularExpressions.Regex.Matches(resultat.Stdout, "écarts nouveaux +: 0").Count);
    }
}
