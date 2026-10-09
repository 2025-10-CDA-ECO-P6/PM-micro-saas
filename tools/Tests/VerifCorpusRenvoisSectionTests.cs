// Caractérise le contrôle des renvois « § section » de `tools/VerifCorpus` sur son
// binaire compilé, comme le reste de VerifCorpusTests.cs : sous-processus, sortie
// standard lue, jamais l'API interne.
//
// Un renvoi « § » contrôlable est un lien Markdown vers un .md du dépôt dont le texte
// porte « § <section> », ou que « § <section> » suit immédiatement dans la ligne. La
// section est appariée aux titres ATX et aux libellés en gras de la cible ; un numéro
// seul désigne le titre ou le libellé qui commence par ce numéro.
using Xunit;

namespace CaracterisationOutils;

/// <summary>
/// Défaut injecté : un renvoi « § » vers une section que la cible ne porte pas.
/// </summary>
public class DetectionRenvoiASectionInexistante : IDisposable
{
    private readonly RepertoireTemporaire _tmp = new();
    private readonly string _depot;

    public DetectionRenvoiASectionInexistante()
    {
        _depot = _tmp.Chemin;
        Helpers.InitialiserDepotGit(_depot);
        Helpers.Ecrire(Path.Combine(_depot, "README.md"), "# Dépôt de fixture\n\n[page A](docs/page-a.md)\n");
        Helpers.Ecrire(
            Path.Combine(_depot, "docs/page-a.md"),
            "# Page A\n\nVoir [page B, § Section absente](page-b.md).\n");
        Helpers.Ecrire(
            Path.Combine(_depot, "docs/page-b.md"),
            "# Page B\n\n## Une autre section\n\nContenu de la page B.\n");
        Helpers.CommiterTout(_depot, "renvoi vers une section inexistante");
    }

    public void Dispose() => _tmp.Dispose();

    [Fact]
    public void DevraitSignalerUnRenvoiDontLaSectionViseeNEstPasDansLaCibleAvecFichierLigneEtSection()
    {
        var resultat = Helpers.ExecuterOutil("VerifCorpus", [_depot]);
        Assert.Contains("renvois § à section inexistante : 1", resultat.Stdout);
        Assert.Contains("[SECTION INEXISTANTE] docs/page-a.md:3 -> page-b.md § Section absente", resultat.Stdout);
    }

    [Fact]
    public void DevraitTerminerEnCodeDeSortieUnQuandUnRenvoiASectionInexistanteEstRapporte()
    {
        var resultat = Helpers.ExecuterOutil("VerifCorpus", [_depot]);
        Assert.Equal(1, resultat.CodeSortie);
    }

    [Fact]
    public void DevraitNeChangerAucunCompteurExistant()
    {
        var resultat = Helpers.ExecuterOutil("VerifCorpus", [_depot]);
        Assert.Contains("liens à cible inexistante     : 0", resultat.Stdout);
        Assert.Contains("liens à ancre inexistante     : 0", resultat.Stdout);
        Assert.Contains("fichiers orphelins            : 0", resultat.Stdout);
        Assert.Contains("blocs de code déséquilibrés   : 0", resultat.Stdout);
    }
}

/// <summary>
/// Défaut injecté sous la forme où le « § » suit le lien au lieu d'être dans son texte.
/// </summary>
public class DetectionRenvoiASectionInexistanteApresLeLien : IDisposable
{
    private readonly RepertoireTemporaire _tmp = new();
    private readonly string _depot;

    public DetectionRenvoiASectionInexistanteApresLeLien()
    {
        _depot = _tmp.Chemin;
        Helpers.InitialiserDepotGit(_depot);
        Helpers.Ecrire(Path.Combine(_depot, "README.md"), "# Dépôt de fixture\n\n[page A](docs/page-a.md)\n");
        Helpers.Ecrire(
            Path.Combine(_depot, "docs/page-a.md"),
            "# Page A\n\nLe détail est dans [page B](page-b.md) §9 de ce document.\n");
        Helpers.Ecrire(
            Path.Combine(_depot, "docs/page-b.md"),
            "# Page B\n\n## 1. Une section\n\nContenu de la page B.\n");
        Helpers.CommiterTout(_depot, "renvoi § après le lien");
    }

    public void Dispose() => _tmp.Dispose();

    [Fact]
    public void DevraitSignalerUnRenvoiDontLeSectionSuitLeLien()
    {
        var resultat = Helpers.ExecuterOutil("VerifCorpus", [_depot]);
        Assert.Contains("renvois § à section inexistante : 1", resultat.Stdout);
        Assert.Contains("[SECTION INEXISTANTE] docs/page-a.md:3 -> page-b.md § 9", resultat.Stdout);
    }
}

/// <summary>
/// Cas légitimes voisins du défaut : aucun ne doit être signalé (faux positifs).
/// </summary>
public class RenvoisASectionExistante : IDisposable
{
    private readonly RepertoireTemporaire _tmp = new();
    private readonly string _depot;

    public RenvoisASectionExistante()
    {
        _depot = _tmp.Chemin;
        Helpers.InitialiserDepotGit(_depot);
        Helpers.Ecrire(Path.Combine(_depot, "README.md"), "# Dépôt de fixture\n\n[page A](docs/page-a.md)\n");
        Helpers.Ecrire(
            Path.Combine(_depot, "docs/page-a.md"),
            "# Page A\n\n"
            + "Titre accentué : [page B, § Périmètre & limites : l'état (MVP)](page-b.md).\n\n"
            + "Libellé en gras : [page B, § A4 — Suppression du compte](page-b.md).\n\n"
            + "Numéro seul : [page B, § 8.4](page-b.md).\n\n"
            + "Numéro collé : [`page-b.md §3`](page-b.md).\n\n"
            + "Après le lien : [page B](page-b.md), § Une autre section ; et [page B](page-b.md) §8.4 aussi.\n\n"
            + "Suite du libellé : [page B, § Une autre section — avec un sous-titre libre](page-b.md).\n\n"
            + "Lien externe : [site, § Inconnu](https://exemple.invalid/x.md).\n\n"
            + "Lien sans section : [page B](page-b.md).\n\n"
            + "```\n[dans un bloc de code, § Absente](page-b.md)\n```\n");
        Helpers.Ecrire(
            Path.Combine(_depot, "docs/page-b.md"),
            "# Page B\n\n"
            + "## Périmètre & limites : l'état (MVP)\n\n"
            + "- **A4 — Suppression du compte** : texte.\n\n"
            + "#### 8.4 Ouvrir la cible\n\n"
            + "### 3. Troisième\n\n"
            + "## Une autre section\n");
        Helpers.CommiterTout(_depot, "renvois à section existante");
    }

    public void Dispose() => _tmp.Dispose();

    [Fact]
    public void DevraitNeRienSignalerPourUnTitreAccentueUnLibelleEnGrasUnNumeroOuUnRenvoiApresLeLien()
    {
        var resultat = Helpers.ExecuterOutil("VerifCorpus", [_depot]);
        Assert.Contains("renvois § à section inexistante : 0", resultat.Stdout);
        Assert.DoesNotContain("[SECTION INEXISTANTE]", resultat.Stdout);
    }

    [Fact]
    public void DevraitTerminerEnCodeDeSortieZero()
    {
        var resultat = Helpers.ExecuterOutil("VerifCorpus", [_depot]);
        Assert.Equal(0, resultat.CodeSortie);
    }
}

/// <summary>
/// Une section citée à la fois comme titre et comme numéro n'autorise pas un numéro qui
/// n'est qu'un préfixe d'un autre (« 5 » ne désigne pas « 5.1 »).
/// </summary>
public class RenvoiParNumeroSansConfusionDePrefixe : IDisposable
{
    private readonly RepertoireTemporaire _tmp = new();
    private readonly string _depot;

    public RenvoiParNumeroSansConfusionDePrefixe()
    {
        _depot = _tmp.Chemin;
        Helpers.InitialiserDepotGit(_depot);
        Helpers.Ecrire(Path.Combine(_depot, "README.md"), "# Dépôt de fixture\n\n[page A](docs/page-a.md)\n");
        Helpers.Ecrire(
            Path.Combine(_depot, "docs/page-a.md"),
            "# Page A\n\nVoir [page B, § 5](page-b.md).\n");
        Helpers.Ecrire(
            Path.Combine(_depot, "docs/page-b.md"),
            "# Page B\n\n### 5.1 Sous-section\n\n### 50 Autre\n");
        Helpers.CommiterTout(_depot, "numéro préfixe");
    }

    public void Dispose() => _tmp.Dispose();

    [Fact]
    public void DevraitSignalerLeNumeroCinqQuandSeulsCinqPointUnEtCinquanteExistent()
    {
        var resultat = Helpers.ExecuterOutil("VerifCorpus", [_depot]);
        Assert.Contains("[SECTION INEXISTANTE] docs/page-a.md:3 -> page-b.md § 5", resultat.Stdout);
    }
}

/// <summary>
/// Liste de référence : un écart connu, inscrit dans
/// `tools/VerifCorpus/renvois-section-acceptes.txt` à la racine auditée, est compté à part
/// et ne fait pas échouer ; un écart absent de la liste, ou plus fréquent qu'elle ne
/// l'accepte (clé|nombre), échoue.
/// </summary>
public class ListeDeReferenceDesRenvoisASectionInexistante : IDisposable
{
    private readonly RepertoireTemporaire _tmp = new();
    private readonly string _depot;

    public ListeDeReferenceDesRenvoisASectionInexistante()
    {
        _depot = _tmp.Chemin;
        Helpers.InitialiserDepotGit(_depot);
        Helpers.Ecrire(Path.Combine(_depot, "README.md"), "# Dépôt de fixture\n\n[page A](docs/page-a.md)\n");
        Helpers.Ecrire(
            Path.Combine(_depot, "docs/page-a.md"),
            "# Page A\n\nVoir [page B, § Section connue](page-b.md).\n");
        Helpers.Ecrire(
            Path.Combine(_depot, "docs/page-b.md"),
            "# Page B\n\n## Une autre section\n");
        Helpers.Ecrire(
            Path.Combine(_depot, "tools/VerifCorpus/renvois-section-acceptes.txt"),
            "# Écarts connus, un par ligne : source -> cible § section|nombre d'occurrences acceptées\n"
            + "docs/page-a.md -> page-b.md § Section connue|1\n");
        Helpers.CommiterTout(_depot, "écart connu");
    }

    public void Dispose() => _tmp.Dispose();

    [Fact]
    public void DevraitCompterAPartUnEcartInscritDansLaListeEtTerminerEnCodeZero()
    {
        var resultat = Helpers.ExecuterOutil("VerifCorpus", [_depot]);
        Assert.Contains("renvois § à section inexistante : 0", resultat.Stdout);
        Assert.Contains("renvois § écarts acceptés       : 1", resultat.Stdout);
        Assert.DoesNotContain("[SECTION INEXISTANTE]", resultat.Stdout);
        Assert.Equal(0, resultat.CodeSortie);
    }

    [Fact]
    public void DevraitEchouerSurUnEcartAbsentDeLaListe()
    {
        Helpers.Ecrire(
            Path.Combine(_depot, "docs/page-a.md"),
            "# Page A\n\nVoir [page B, § Section connue](page-b.md) et [page B, § Section nouvelle](page-b.md).\n");
        var resultat = Helpers.ExecuterOutil("VerifCorpus", [_depot]);
        Assert.Contains("renvois § à section inexistante : 1", resultat.Stdout);
        Assert.Contains("renvois § écarts acceptés       : 1", resultat.Stdout);
        Assert.Contains("[SECTION INEXISTANTE] docs/page-a.md:3 -> page-b.md § Section nouvelle", resultat.Stdout);
        Assert.Equal(1, resultat.CodeSortie);
    }

    [Fact]
    public void DevraitEchouerSurUnRenvoiDePlusVersLaMemeSectionAbsenteDepuisLeMemeFichier()
    {
        Helpers.Ecrire(
            Path.Combine(_depot, "docs/page-a.md"),
            "# Page A\n\nVoir [page B, § Section connue](page-b.md).\n\nEncore [page B, § Section connue](page-b.md).\n");
        var resultat = Helpers.ExecuterOutil("VerifCorpus", [_depot]);
        Assert.Contains("renvois § à section inexistante : 1", resultat.Stdout);
        Assert.Contains("renvois § écarts acceptés       : 1", resultat.Stdout);
        Assert.Contains("[SECTION INEXISTANTE] docs/page-a.md:5 -> page-b.md § Section connue", resultat.Stdout);
        Assert.Equal(1, resultat.CodeSortie);
    }

    [Fact]
    public void DevraitRendreLeCodeDeSortieDeuxSurUneLigneSansNombre()
    {
        Helpers.Ecrire(
            Path.Combine(_depot, "tools/VerifCorpus/renvois-section-acceptes.txt"),
            "docs/page-a.md -> page-b.md § Section connue\n");
        var resultat = Helpers.ExecuterOutil("VerifCorpus", [_depot]);
        Assert.Equal(2, resultat.CodeSortie);
        Assert.Contains("panne d'exécution", resultat.Stderr);
    }
}
