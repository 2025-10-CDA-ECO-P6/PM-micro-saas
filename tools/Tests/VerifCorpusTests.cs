// Portage C# du script Python d'origine, `tools/tests/test_verif_corpus.py` — caractérise le
// comportement observable de `tools/VerifCorpus` sur son binaire compilé,
// exactement comme le script Python l'était sur son propre exécutable
// (`helpers.executer_script` ↔ `Helpers.ExecuterOutil`, même stratégie
// boîte noire : sous-processus, sortie standard lue, jamais l'API interne).
//
// Deux différences assumées avec l'original, arbitrées par l'opérateur :
//
// 1. Le contrat de code de sortie n'est plus hors oracle. Le script Python
//    ne contenait aucun `sys.exit` et rendait toujours 0 — `ContratCodeDeSortie`
//    ci-dessous documentait ce défaut via `@expectedFailure`. Le portage C#
//    implémente le contrat arbitré (0 rien à signaler, 1 défaut dans les
//    données, 2 panne d'environnement) : ce sont ici des tests ordinaires,
//    qui passent.
// 2. Le code de sortie 2 (panne d'environnement) est un comportement neuf du
//    portage, absent du Python — `PanneRacineNonDepotGit` et
//    `PanneUtf8Invalide` le caractérisent, faute d'équivalent Python à porter.
using System.Text.RegularExpressions;
using Xunit;

namespace CaracterisationOutils;

/// <summary>
/// Golden master n°1 — comportement nominal mesuré sur le corpus réel du
/// dépôt, en lecture seule (aucune écriture : le binaire n'appelle que
/// `git ls-files` et lit des fichiers).
/// </summary>
public class SurCorpusReel
{
    [Fact]
    public void DevraitNeRapporterAucunLienCasseAucuneAncreCasseeAucunOrphelinEtAucuneFenceDesequilibreeSurLeCorpusReel()
    {
        var resultat = Helpers.ExecuterOutil("VerifCorpus", [Helpers.RacineDepot]);
        Assert.Contains("liens à cible inexistante     : 0", resultat.Stdout);
        Assert.Contains("liens à ancre inexistante     : 0", resultat.Stdout);
        Assert.Contains("fichiers orphelins            : 0", resultat.Stdout);
        Assert.Contains("blocs de code déséquilibrés   : 0", resultat.Stdout);
    }

    [Fact]
    public void DevraitIndexerUnNombreNonNulDeFichiersMarkdownSousDocsSurLeCorpusReel()
    {
        // Le décompte exact de fichiers/liens est volatil (le corpus évolue) —
        // exclu du golden master. Seule la non-nullité est vérifiée : un
        // décompte à zéro trahirait un balayage pointant sur le mauvais
        // répertoire, régression bien plus grave qu'une dérive de décompte.
        var resultat = Helpers.ExecuterOutil("VerifCorpus", [Helpers.RacineDepot]);
        var correspondance = Regex.Match(resultat.Stdout, @"sous docs/ : (\d+)");
        Assert.True(correspondance.Success, resultat.Stdout);
        Assert.True(int.Parse(correspondance.Groups[1].Value) > 0);
    }

    [Fact]
    public void DevraitTerminerEnCodeDeSortieZeroSurLeCorpusReel()
    {
        // Contrat neuf (absent du Python) : le corpus réel ne porte aujourd'hui
        // aucun des quatre défauts arbitrés — le code de sortie doit le refléter.
        var resultat = Helpers.ExecuterOutil("VerifCorpus", [Helpers.RacineDepot]);
        Assert.Equal(0, resultat.CodeSortie);
    }
}

/// <summary>
/// Comportement n°12 — l'indexation ne recense que les <c>*.md</c>, via
/// <c>git ls-files</c> ET <c>git ls-files --others --exclude-standard</c> :
/// un <c>.md</c> neuf non commité est vu, un <c>.md</c> d'un répertoire
/// ignoré ne l'est pas.
/// </summary>
public class DecouverteDesFichiers : IDisposable
{
    private readonly RepertoireTemporaire _tmp = new();
    private readonly string _depot;

    public DecouverteDesFichiers()
    {
        _depot = _tmp.Chemin;
        Helpers.InitialiserDepotGit(_depot);
        Helpers.Ecrire(Path.Combine(_depot, "README.md"), "# Dépôt de fixture\n");
        Helpers.Ecrire(Path.Combine(_depot, "docs/guide.md"), "# Guide\n\nContenu du guide.\n");
        Helpers.Ecrire(Path.Combine(_depot, ".gitignore"), "/ignore-audit/\n");
        Helpers.CommiterTout(_depot, "corpus initial");
        // Après le commit : un .md neuf non suivi (doit être vu), et un .md
        // dans un répertoire ignoré par .gitignore (ne doit pas être vu).
        Helpers.Ecrire(Path.Combine(_depot, "docs/nouveau.md"), "# Note fraîche\n\nPas encore commitée.\n");
        Helpers.Ecrire(Path.Combine(_depot, "ignore-audit/notes.md"), "# Notes d'audit\n\nIgnorées par construction.\n");
    }

    public void Dispose() => _tmp.Dispose();

    [Fact]
    public void DevraitVoirUnMdNeufNonCommiteEtIgnorerUnMdDansUnRepertoireExcluParGitignore()
    {
        var resultat = Helpers.ExecuterOutil("VerifCorpus", [_depot]);
        // Décompte exact : README.md + docs/guide.md (suivis) + docs/nouveau.md
        // (neuf, non ignoré) = 3, dont 2 sous docs/. ignore-audit/notes.md
        // n'entre dans aucun des deux décomptes.
        Assert.Contains("fichiers markdown             : 3  (sous docs/ : 2)", resultat.Stdout);
        Assert.Contains("docs/nouveau.md", resultat.Stdout);
        Assert.DoesNotContain("notes.md", resultat.Stdout);
    }
}

public class DetectionLienACibleInexistante : IDisposable
{
    private readonly RepertoireTemporaire _tmp = new();
    private readonly string _depot;

    public DetectionLienACibleInexistante()
    {
        _depot = _tmp.Chemin;
        Helpers.InitialiserDepotGit(_depot);
        Helpers.Ecrire(Path.Combine(_depot, "README.md"), "# Dépôt de fixture\n");
        Helpers.Ecrire(
            Path.Combine(_depot, "docs/page-a.md"),
            "# Page A\n\nVoir [la page absente](page-inexistante.md) pour la suite.\n");
        Helpers.CommiterTout(_depot, "lien vers cible inexistante");
    }

    public void Dispose() => _tmp.Dispose();

    [Fact]
    public void DevraitSignalerUnLienDontLaCibleRelativeNExistePasSurLeDisque()
    {
        var resultat = Helpers.ExecuterOutil("VerifCorpus", [_depot]);
        Assert.Contains("liens à cible inexistante     : 1", resultat.Stdout);
        Assert.Contains("[CIBLE CASSÉE] docs/page-a.md -> page-inexistante.md", resultat.Stdout);
    }
}

public class DetectionLienAAncreInexistante : IDisposable
{
    private readonly RepertoireTemporaire _tmp = new();
    private readonly string _depot;

    public DetectionLienAAncreInexistante()
    {
        _depot = _tmp.Chemin;
        Helpers.InitialiserDepotGit(_depot);
        Helpers.Ecrire(Path.Combine(_depot, "README.md"), "# Dépôt de fixture\n");
        Helpers.Ecrire(
            Path.Combine(_depot, "docs/page-a.md"),
            "# Page A\n\nVoir [la section](page-b.md#section-absente).\n");
        Helpers.Ecrire(
            Path.Combine(_depot, "docs/page-b.md"),
            "# Page B\n\n## Une autre section\n\nContenu de la page B.\n");
        Helpers.CommiterTout(_depot, "lien vers ancre inexistante");
    }

    public void Dispose() => _tmp.Dispose();

    [Fact]
    public void DevraitSignalerUnLienDontLAncreNeCorrespondAAucunTitreNiAncreNommeeDeLaCible()
    {
        var resultat = Helpers.ExecuterOutil("VerifCorpus", [_depot]);
        Assert.Contains("liens à ancre inexistante     : 1", resultat.Stdout);
        Assert.Contains("[ANCRE CASSÉE] docs/page-a.md -> page-b.md#section-absente", resultat.Stdout);
    }
}

public class DetectionFichierOrphelin : IDisposable
{
    private readonly RepertoireTemporaire _tmp = new();
    private readonly string _depot;

    public DetectionFichierOrphelin()
    {
        _depot = _tmp.Chemin;
        Helpers.InitialiserDepotGit(_depot);
        Helpers.Ecrire(Path.Combine(_depot, "README.md"), "# Dépôt de fixture\n\nAucun lien vers docs/.\n");
        Helpers.Ecrire(Path.Combine(_depot, "docs/isole.md"), "# Page isolée\n\nAucun fichier n'y renvoie.\n");
        Helpers.CommiterTout(_depot, "fichier sans lien entrant");
    }

    public void Dispose() => _tmp.Dispose();

    [Fact]
    public void DevraitSignalerUnFichierMdVersLequelAucunAutreFichierNeRenvoie()
    {
        var resultat = Helpers.ExecuterOutil("VerifCorpus", [_depot]);
        Assert.Contains("fichiers orphelins            : 1", resultat.Stdout);
        Assert.Contains("[ORPHELIN] docs/isole.md", resultat.Stdout);
    }
}

public class DetectionBlocDeCodeDesequilibre : IDisposable
{
    private readonly RepertoireTemporaire _tmp = new();
    private readonly string _depot;

    public DetectionBlocDeCodeDesequilibre()
    {
        _depot = _tmp.Chemin;
        Helpers.InitialiserDepotGit(_depot);
        Helpers.Ecrire(Path.Combine(_depot, "README.md"), "# Dépôt de fixture\n");
        Helpers.Ecrire(
            Path.Combine(_depot, "docs/code.md"),
            "# Extrait de code\n\n```python\ndef f():\n    return 1\n");
        Helpers.CommiterTout(_depot, "fence jamais refermée");
    }

    public void Dispose() => _tmp.Dispose();

    [Fact]
    public void DevraitSignalerUnFichierDontLeBlocDeCodeOuvertNEstJamaisReferme()
    {
        var resultat = Helpers.ExecuterOutil("VerifCorpus", [_depot]);
        Assert.Contains("blocs de code déséquilibrés   : 1", resultat.Stdout);
        Assert.Contains("[FENCE] docs/code.md", resultat.Stdout);
    }
}

/// <summary>
/// Contrat de code de sortie — anciennement <c>ContratFuturCodeDeSortie</c>
/// côté Python, marqué <c>@expectedFailure</c> (le script n'avait aucun
/// <c>sys.exit</c>). Le portage C# implémente le contrat arbitré : ce test
/// est désormais un test ordinaire, vert.
/// </summary>
public class ContratCodeDeSortieVerifCorpus : IDisposable
{
    private readonly RepertoireTemporaire _tmp = new();
    private readonly string _depot;

    public ContratCodeDeSortieVerifCorpus()
    {
        _depot = _tmp.Chemin;
        Helpers.InitialiserDepotGit(_depot);
        Helpers.Ecrire(Path.Combine(_depot, "README.md"), "# Dépôt de fixture\n");
        Helpers.Ecrire(
            Path.Combine(_depot, "docs/page-a.md"),
            "# Page A\n\nVoir [la page absente](page-inexistante.md) pour la suite.\n");
        Helpers.CommiterTout(_depot, "lien vers cible inexistante");
    }

    public void Dispose() => _tmp.Dispose();

    [Fact]
    public void DevraitTerminerEnCodeDeSortieNonNulQuandUnDefautEstRapporte()
    {
        var resultat = Helpers.ExecuterOutil("VerifCorpus", [_depot]);
        // Le défaut est bien rapporté en sortie standard — et, désormais, le
        // code de sortie le reflète.
        Assert.Contains("liens à cible inexistante     : 1", resultat.Stdout);
        Assert.NotEqual(0, resultat.CodeSortie);
    }
}

/// <summary>
/// Corpus sain, construit indépendamment du dépôt réel (jamais affecté par
/// son évolution) : zéro défaut sur les quatre familles, code de sortie 0.
/// Complète le sens « détecte » des fixtures ci-dessus par le sens « se tait
/// à bon escient » — un vérificateur qui rapporterait toujours au moins un
/// défaut passerait toutes les fixtures de détection sans jamais être pris
/// en défaut sur celle-ci.
/// </summary>
public class CorpusSain : IDisposable
{
    private readonly RepertoireTemporaire _tmp = new();
    private readonly string _depot;

    public CorpusSain()
    {
        _depot = _tmp.Chemin;
        Helpers.InitialiserDepotGit(_depot);
        Helpers.Ecrire(Path.Combine(_depot, "README.md"), "# Dépôt de fixture\n\nVoir [le guide](docs/guide.md).\n");
        Helpers.Ecrire(Path.Combine(_depot, "docs/guide.md"), "# Guide\n\nContenu du guide.\n");
        Helpers.CommiterTout(_depot, "corpus sain");
    }

    public void Dispose() => _tmp.Dispose();

    [Fact]
    public void DevraitTerminerEnCodeDeSortieZeroQuandAucunDefautNEstRapporte()
    {
        var resultat = Helpers.ExecuterOutil("VerifCorpus", [_depot]);
        Assert.Contains("liens à cible inexistante     : 0", resultat.Stdout);
        Assert.Contains("liens à ancre inexistante     : 0", resultat.Stdout);
        Assert.Contains("fichiers orphelins            : 0", resultat.Stdout);
        Assert.Contains("blocs de code déséquilibrés   : 0", resultat.Stdout);
        Assert.Equal(0, resultat.CodeSortie);
    }
}

/// <summary>
/// Volet « panne d'environnement » (code 2) du contrat arbitré — comportement
/// neuf du portage, sans équivalent Python (le script d'origine ne distinguait
/// pas panne et défaut : les deux terminaient en 0).
/// </summary>
public class PanneRacineNonDepotGit : IDisposable
{
    private readonly RepertoireTemporaire _tmp = new();

    public void Dispose() => _tmp.Dispose();

    [Fact]
    public void DevraitTerminerEnCodeDeSortieDeuxQuandLaRacineNEstPasUnDepotGit()
    {
        // Aucun `git init` : la racine indexée n'est pas un dépôt, le
        // `git ls-files` interne échoue — panne d'environnement, pas défaut
        // de données.
        var resultat = Helpers.ExecuterOutil("VerifCorpus", [_tmp.Chemin]);
        Assert.Equal(2, resultat.CodeSortie);
        Assert.Contains("panne d'exécution", resultat.Stderr);
    }
}

public class PanneUtf8Invalide : IDisposable
{
    private readonly RepertoireTemporaire _tmp = new();
    private readonly string _depot;

    public PanneUtf8Invalide()
    {
        _depot = _tmp.Chemin;
        Helpers.InitialiserDepotGit(_depot);
        Helpers.Ecrire(Path.Combine(_depot, "README.md"), "# Dépôt de fixture\n");
        // Séquence UTF-8 invalide (0xFF n'est un octet de tête d'aucune
        // séquence UTF-8 valide) — décodage strict, throwOnInvalidBytes.
        Helpers.EcrireOctets(Path.Combine(_depot, "docs/mauvais.md"), [0x23, 0x20, 0xFF, 0x0A]);
        Helpers.CommiterTout(_depot, "octet invalide");
    }

    public void Dispose() => _tmp.Dispose();

    [Fact]
    public void DevraitTerminerEnCodeDeSortieDeuxQuandUnFichierMdContientUneSequenceUtf8Invalide()
    {
        var resultat = Helpers.ExecuterOutil("VerifCorpus", [_depot]);
        Assert.Equal(2, resultat.CodeSortie);
        Assert.Contains("panne d'exécution", resultat.Stderr);
    }
}
