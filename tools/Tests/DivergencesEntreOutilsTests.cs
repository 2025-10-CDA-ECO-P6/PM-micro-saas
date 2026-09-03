// Tests d'INVARIANT — distincts des golden masters et des caractérisations
// comportementales du reste de ce projet — portage du script Python
// d'origine, `tools/tests/test_divergences_entre_outils.py`. Ils exposent quatre
// désaccords RÉELS et NON ARBITRÉS entre `tools/VerifMailleAgregat` (le
// vérificateur) et `tools/CreerIssues` (le générateur), sur des entrées que
// le plan de travail réel ne porte pas encore. Un différentiel de sortie
// est aveugle par construction à ces divergences : le plan actuel ne
// déclenche aucune des quatre, donc aucun golden master construit dessus ne
// les verrait jamais. Ces tests les rendent visibles sur des entrées
// CONSTRUITES, portant chacune le déclencheur.
//
// Portée : ce fichier EXPOSE, il ne CORRIGE rien. L'arbitrage de laquelle des
// deux implémentations est juste — ou, pour la divergence D, du choix de
// modélisation à retenir — appartient au donneur d'ordre.
//
// Différence de mécanique assumée avec l'original Python : chaque test
// Python affirmait l'invariant SOUHAITÉ (les deux outils s'accordent) et
// était marqué `@unittest.expectedFailure`, donc rouge tant que la
// divergence existe. Ce marqueur a un défaut : un échec-attendu ne dit rien
// quand la divergence DISPARAÎT — il devient un succès inattendu, que la
// plupart des exécuteurs (dont celui-ci, xUnit, qui n'a pas d'équivalent
// direct de `expectedFailure`) signalent mal ou pas du tout. Chaque test
// ci-dessous affirme au contraire la divergence ELLE-MÊME (les deux outils
// se comportent DIFFÉREMMENT sur l'entrée déclenchante) : il est VERT tant
// que la divergence existe — état actuel, voulu, documenté — et ROUGIT le
// jour où quelqu'un l'arbitre, forçant à venir lire ce qui a été décidé et à
// mettre le test à jour. L'arbitrage reste ouvert dans les quatre cas.
using Xunit;

namespace CaracterisationOutils;

/// <summary>Fixtures partagées par les quatre divergences de ce fichier.</summary>
internal static class FixturesDivergences
{
    public const string DomaineMd =
        "# Personnage (domaine)\n\n## Agrégats\n\n### Personnage\n\nRacine d'agrégat, décrit un personnage joueur.\n";

    // Identique à FixturesCreerIssues.PlanPreambule (CreerIssuesTests.cs) —
    // dupliqué plutôt que partagé : ce fichier documente des divergences
    // entre deux outils, une dépendance croisée vers les fixtures de l'un
    // d'eux irait à l'encontre de cette neutralité.
    public const string PlanPreambule =
        "# Plan de travail (fixture de test)\n\n" +
        "## §2 Maille de désignation\n\n" +
        "**Les six modules de la couche cliente et leur ancrage.**\n\n" +
        "| `service d'accès au store` | Accès IndexedDB |\n" +
        "| `châssis` | Disposition générale |\n" +
        "| `bandeaux transversaux` | Bandeaux d'état |\n" +
        "| `sanitisation` | Nettoyage HTML |\n" +
        "| `politique de sécurité de contenu` | CSP |\n" +
        "| `projection d'export` | Rendu d'export |\n\n" +
        "**L'agrégat couvre ses deux moitiés.**\n\n" +
        "Le domaine et son module d'accès forment la même unité d'écriture.\n\n" +
        "- un **module d'outillage** : `la configuration de la solution`, `la configuration d'intégration continue`\n" +
        "- un **module transverse nommé** : `SharedKernel`, `Haversack.Infrastructure.Notifications`\n\n" +
        "### Tâches\n\n";

    public static void ConstruireDepot(string depot, string planMd)
    {
        Helpers.InitialiserDepotGit(depot);
        Helpers.Ecrire(Path.Combine(depot, "docs/gestion-projet/methode-de-ticket.md"), FixturesCreerIssues.MethodeMd);
        Helpers.Ecrire(Path.Combine(depot, "docs/gestion-projet/plan-de-travail.md"), planMd);
        Helpers.Ecrire(Path.Combine(depot, "docs/conception/domain/personnage.md"), DomaineMd);
        Helpers.CommiterTout(depot, "fixture de divergence");
        // Révision simulée déjà poussée uniquement pour que l'AVERTISSEMENT
        // sur la révision, hors sujet ici, n'ajoute pas de bruit à la
        // démonstration (cf. RegimesDeLaRevisionEnModeABlanc, CreerIssuesTests.cs).
        Helpers.ConfigurerOrigineFictive(depot, FixturesCreerIssues.UrlOrigineFictive, pousser: true);
    }

    public static ResultatExecution ExecuterGenerateur(string depot, string racineBin)
    {
        var environnement = new Dictionary<string, string> { ["PATH"] = Helpers.RepertoireBinSansGh(racineBin) };
        return Helpers.ExecuterOutilAvecEnvironnement("CreerIssues", [], depot, environnement);
    }
}

/// <summary>
/// Divergence A — les deux outils analysent le titre de tranche avec des
/// règles différentes :
///   CreerIssues (PlanDeTravail.cs)         ^##### (TB-\d+) — (.+)$   (5 dièses
///                                           EXACTS, tiret cadratin obligatoire)
///   VerifMailleAgregat (PlanDeTravail.cs)  \n#{4,6}\s+(?=TB-)         (4 à 6
///                                           dièses, aucune contrainte de tiret)
///
/// EN PRATIQUE : une tranche titrée avec 4 dièses et un tiret court à la
/// place du cadratin est VUE et contrôlée par le vérificateur — mais
/// INVISIBLE, sans aucun signalement, pour le générateur : elle ne recevrait
/// jamais d'issue GitHub, silencieusement, alors même que le plan la déclare
/// conforme.
/// </summary>
public class DivergenceA_FormatDuTitreDeTranche : IDisposable
{
    private readonly RepertoireTemporaire _tmpDepot = new();
    private readonly RepertoireTemporaire _tmpBin = new();

    public DivergenceA_FormatDuTitreDeTranche()
    {
        string plan = FixturesDivergences.PlanPreambule +
            "#### TB-050 – Tâche à quatre dièses et tiret court\n\n" +
            "| Champ | Valeur |\n|---|---|\n" +
            "| Jalon | J1 — traversée verticale |\n" +
            "| Périmètre d'écriture | Personnage |\n" +
            "| Dépend de | — |\n" +
            "| En conflit avec | — |\n";
        FixturesDivergences.ConstruireDepot(_tmpDepot.Chemin, plan);
    }

    public void Dispose()
    {
        _tmpDepot.Dispose();
        _tmpBin.Dispose();
    }

    [Fact]
    public void ArbitrageOuvert_LeVerificateurVoitUneTacheQueLeGenerateurNeVoitPas()
    {
        var resultatVerificateur = Helpers.ExecuterOutil(
            "VerifMailleAgregat", [_tmpDepot.Chemin, "docs/gestion-projet/plan-de-travail.md"]);
        Assert.Contains("tâches du plan                        : 1", resultatVerificateur.Stdout);

        var resultatGenerateur = FixturesDivergences.ExecuterGenerateur(_tmpDepot.Chemin, _tmpBin.Chemin);
        // Le générateur en lit 0 : ce test rougira le jour où l'un des deux
        // motifs de titre sera aligné sur l'autre — c'est le signal recherché,
        // pas un défaut de ce test.
        Assert.Contains("Tâches lues dans le plan : 0", resultatGenerateur.Stdout);
    }
}

/// <summary>
/// Divergence B — le nombre de chiffres admis dans l'identifiant :
///   VerifMailleAgregat (PlanDeTravail.cs)   TB-\d{3}   exactement trois chiffres,
///                                            non ancré en fin de motif
///   CreerIssues (PlanDeTravail.cs)          TB-\d+     n'importe quel nombre
///
/// EN PRATIQUE : `VerifMailleAgregat` extrait l'identifiant via un motif non
/// ancré en fin de chaîne — il capture seulement les trois premiers chiffres.
/// `TB-1000` est donc lu comme `TB-100` : si une tâche `TB-100` existe par
/// ailleurs dans le même plan, la seconde écrase silencieusement la première
/// dans le dictionnaire des tâches — une tâche entière disparaît du
/// décompte, remplacée par les données de l'autre. `CreerIssues`, qui
/// construit une LISTE et non un dictionnaire indexé par identifiant
/// tronqué, ne connaît pas cette collision et voit les deux tâches.
/// </summary>
public class DivergenceB_NombreDeChiffresDansLIdentifiant : IDisposable
{
    private readonly RepertoireTemporaire _tmpDepot = new();
    private readonly RepertoireTemporaire _tmpBin = new();

    public DivergenceB_NombreDeChiffresDansLIdentifiant()
    {
        string plan = FixturesDivergences.PlanPreambule +
            "##### TB-100 — Tâche à trois chiffres\n\n" +
            "| Champ | Valeur |\n|---|---|\n" +
            "| Jalon | J1 — traversée verticale |\n" +
            "| Périmètre d'écriture | Personnage |\n" +
            "| Dépend de | — |\n" +
            "| En conflit avec | — |\n\n" +
            "---\n\n" +
            "##### TB-1000 — Tâche à quatre chiffres\n\n" +
            "| Champ | Valeur |\n|---|---|\n" +
            "| Jalon | J1 — traversée verticale |\n" +
            "| Périmètre d'écriture | châssis |\n" +
            "| Dépend de | — |\n" +
            "| En conflit avec | — |\n";
        FixturesDivergences.ConstruireDepot(_tmpDepot.Chemin, plan);
    }

    public void Dispose()
    {
        _tmpDepot.Dispose();
        _tmpBin.Dispose();
    }

    [Fact]
    public void ArbitrageOuvert_LeVerificateurNeCompteQuUneTacheLaOuLeGenerateurEnCompteDeux()
    {
        var resultatGenerateur = FixturesDivergences.ExecuterGenerateur(_tmpDepot.Chemin, _tmpBin.Chemin);
        // Prémisse, vraie aujourd'hui : le générateur (\d+) distingue bien
        // les deux identifiants et voit deux tâches.
        Assert.Contains("Tâches lues dans le plan : 2", resultatGenerateur.Stdout);

        var resultatVerificateur = Helpers.ExecuterOutil(
            "VerifMailleAgregat", [_tmpDepot.Chemin, "docs/gestion-projet/plan-de-travail.md"]);
        // TB-1000 est tronqué en TB-100 et écrase la vraie TB-100 dans le
        // dictionnaire des tâches ; le vérificateur n'en compte plus qu'une.
        // Ce test rougira le jour où l'un des deux motifs sera aligné sur
        // l'autre.
        Assert.Contains("tâches du plan                        : 1", resultatVerificateur.Stdout);
    }
}

/// <summary>
/// Divergence C — la maille de désignation, dans les deux sens :
///   CreerIssues (MailleDuPlan.cs)          LIT le §2 du plan à chaque exécution
///   VerifMailleAgregat (MailleDesignation.cs) porte la liste d'outillage EN DUR
///                                              (quatre libellés figés dans le code)
///
/// EN PRATIQUE : si le §2 gagne un cinquième libellé d'outillage, le
/// générateur le résout (il le lit) tandis que le vérificateur le refuse
/// toujours, quand bien même le corpus l'autorise désormais — une fausse
/// alerte de non-conformité sur un module pourtant légitime.
/// </summary>
public class DivergenceC_MailleLueOuCodeeEnDur : IDisposable
{
    private readonly RepertoireTemporaire _tmpDepot = new();
    private readonly RepertoireTemporaire _tmpBin = new();

    public DivergenceC_MailleLueOuCodeeEnDur()
    {
        string plan = FixturesDivergences.PlanPreambule.Replace(
            "- un **module d'outillage** : `la configuration de la solution`, `la configuration d'intégration continue`",
            "- un **module d'outillage** : `la configuration de la solution`, `la configuration d'intégration continue`, "
            + "`le pipeline de publication npm`") +
            "##### TB-060 — Publication automatisée du paquet npm\n\n" +
            "| Champ | Valeur |\n|---|---|\n" +
            "| Jalon | J1 — traversée verticale |\n" +
            "| Périmètre d'écriture | le pipeline de publication npm |\n" +
            "| Dépend de | — |\n" +
            "| En conflit avec | — |\n";
        FixturesDivergences.ConstruireDepot(_tmpDepot.Chemin, plan);
    }

    public void Dispose()
    {
        _tmpDepot.Dispose();
        _tmpBin.Dispose();
    }

    [Fact]
    public void ArbitrageOuvert_LeGenerateurResoutUnCinquiemeLibelleDOutillageQueLeVerificateurRefuse()
    {
        var resultatGenerateur = FixturesDivergences.ExecuterGenerateur(_tmpDepot.Chemin, _tmpBin.Chemin);
        // Prémisse, vraie aujourd'hui : le générateur (lecture dynamique du
        // §2) résout la tâche sans réserve.
        Assert.Contains("Tâches sans libellé de nature déductible (0)", resultatGenerateur.Stdout);

        var resultatVerificateur = Helpers.ExecuterOutil(
            "VerifMailleAgregat", [_tmpDepot.Chemin, "docs/gestion-projet/plan-de-travail.md"]);
        // Le vérificateur (maille codée en dur) refuse toujours le nouveau
        // libellé. Ce test rougira le jour où l'un des deux sera aligné sur
        // l'autre.
        Assert.Contains("item ne se résolvant pas la maille  : 1", resultatVerificateur.Stdout);
    }
}

/// <summary>
/// Divergence D — NON un défaut, une différence de MODÉLISATION — sur le
/// verrou du namespace de bounded context :
///   VerifMailleAgregat (MailleDesignation.cs)  implémente la clause du §2
///                                               « sur la seule tranche qui les
///                                               crée » : un namespace ne
///                                               résout que sur TB-003.
///   CreerIssues (ResolutionMaille.cs)          n'implémente PAS cette clause :
///                                               sa résolution ne prend aucun
///                                               paramètre d'identifiant de
///                                               tâche.
///
/// EN PRATIQUE, mesuré sur TB-003 elle-même : le vérificateur la compte
/// résolue (la clause du §2 appliquée) ; le générateur la classe sans
/// libellé de nature déductible — n'ayant pas la notion de namespace, il ne
/// le résout NULLE PART, pas même sur la tranche qui le crée. Ce n'est pas
/// que le générateur « admette le namespace partout » (il ne le résout nulle
/// part) : il traite TB-003 comme n'importe quelle autre tranche, faute
/// d'implémenter la clause d'exception que le vérificateur, lui, porte.
/// </summary>
public class DivergenceD_VerrouDuNamespaceSurLaTrancheDeCreation : IDisposable
{
    private readonly RepertoireTemporaire _tmpDepot = new();
    private readonly RepertoireTemporaire _tmpBin = new();

    public DivergenceD_VerrouDuNamespaceSurLaTrancheDeCreation()
    {
        string plan = FixturesDivergences.PlanPreambule +
            "##### TB-003 — Création du namespace IdentityAccess\n\n" +
            "| Champ | Valeur |\n|---|---|\n" +
            "| Jalon | J0 — fondations |\n" +
            "| Périmètre d'écriture | IdentityAccess |\n" +
            "| Dépend de | — |\n" +
            "| En conflit avec | — |\n";
        FixturesDivergences.ConstruireDepot(_tmpDepot.Chemin, plan);
    }

    public void Dispose()
    {
        _tmpDepot.Dispose();
        _tmpBin.Dispose();
    }

    [Fact]
    public void ArbitrageOuvert_LeVerificateurResoutTB003AlorsQueLeGenerateurLaClasseSansNatureDeductible()
    {
        var resultatVerificateur = Helpers.ExecuterOutil(
            "VerifMailleAgregat", [_tmpDepot.Chemin, "docs/gestion-projet/plan-de-travail.md"]);
        Assert.Contains("périmètre entièrement résolu        : 1", resultatVerificateur.Stdout);

        var resultatGenerateur = FixturesDivergences.ExecuterGenerateur(_tmpDepot.Chemin, _tmpBin.Chemin);
        // Le générateur classe TB-003 sans libellé de nature déductible. Ce
        // test rougira le jour où l'un des deux sera aligné sur l'autre — ou
        // que le choix de modélisation sera explicitement tranché.
        Assert.Contains("TB-003 : aucun segment reconnu dans le périmètre d'écriture", resultatGenerateur.Stdout);
    }
}
