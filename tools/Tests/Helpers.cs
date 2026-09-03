// Utilitaires partagés par les tests de caractérisation des outils de `tools/`.
// Portage direct du script Python d'origine, `tools/tests/helpers.py` : mêmes garanties, même prudence —
// aucun de ces utilitaires n'écrit jamais dans le dépôt réel. Tout dépôt git de
// fixture est construit dans le répertoire temporaire système (jamais sous ce
// dépôt suivi par git), propre à chaque instance de test.

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace CaracterisationOutils;

/// <summary>
/// Résultat d'une invocation d'un binaire de `tools/` en sous-processus —
/// jamais d'exception sur un code de sortie non nul, c'est aux tests d'en
/// adjuger le sens (contrat arbitré : 0 rien à signaler, 1 défaut dans les
/// données, 2 panne d'environnement).
/// </summary>
internal sealed record ResultatExecution(string Stdout, string Stderr, int CodeSortie);

internal static class Helpers
{
    private static string CheminSource([CallerFilePath] string chemin = "") => chemin;

    // Même construction que ICI / RACINE_OUTILS / RACINE_DEPOT côté Python :
    // dérivée du chemin physique de CE fichier, jamais du répertoire courant
    // du processus de test.
    public static readonly string Ici = Path.GetDirectoryName(CheminSource())!;
    public static readonly string RacineOutils = Path.GetDirectoryName(Ici)!;
    public static readonly string RacineDepot = Path.GetDirectoryName(RacineOutils)!;

    private static string NomExecutable(string nomOutil) =>
        OperatingSystem.IsWindows() ? $"{nomOutil}.exe" : nomOutil;

    /// <summary>
    /// Chemin du binaire natif d'un outil porté — copié dans le répertoire de
    /// sortie de CE projet par la seule vertu du <c>ProjectReference</c> vers
    /// son projet exécutable (mesuré : aucun fichier de solution requis).
    /// </summary>
    public static string CheminBinaire(string nomOutil) =>
        Path.Combine(AppContext.BaseDirectory, NomExecutable(nomOutil));

    /// <summary>
    /// Exécute un binaire de `tools/` en sous-processus contre les arguments
    /// donnés et renvoie stdout/stderr/code de sortie. Lecture asynchrone des
    /// deux flux avant l'attente de fin de processus — un <c>ReadToEnd</c>
    /// synchrone sur un seul flux peut interbloquer si l'autre remplit son
    /// tampon en parallèle (même précaution que <c>VerificateurCorpus.InvoquerGit</c>).
    /// </summary>
    public static ResultatExecution ExecuterOutil(
        string nomOutil, IEnumerable<string> args, string? cwd = null, int timeoutMs = 30_000)
    {
        var depart = new ProcessStartInfo(CheminBinaire(nomOutil))
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            WorkingDirectory = cwd ?? AppContext.BaseDirectory,
        };
        foreach (var a in args) depart.ArgumentList.Add(a);

        using var processus = Process.Start(depart)
            ?? throw new InvalidOperationException($"{nomOutil} n'a pas pu être démarré");

        var tacheSortie = processus.StandardOutput.ReadToEndAsync();
        var tacheErreur = processus.StandardError.ReadToEndAsync();
        if (!processus.WaitForExit(timeoutMs))
        {
            processus.Kill(entireProcessTree: true);
            throw new TimeoutException($"{nomOutil} n'a pas rendu la main sous {timeoutMs} ms");
        }
        string stdout = tacheSortie.GetAwaiter().GetResult();
        string stderr = tacheErreur.GetAwaiter().GetResult();
        return new ResultatExecution(stdout, stderr, processus.ExitCode);
    }

    private static void ExecuterGit(string depot, params string[] args)
    {
        var depart = new ProcessStartInfo("git") { RedirectStandardOutput = true, RedirectStandardError = true, UseShellExecute = false };
        depart.ArgumentList.Add("-C");
        depart.ArgumentList.Add(depot);
        foreach (var a in args) depart.ArgumentList.Add(a);

        using var processus = Process.Start(depart)
            ?? throw new InvalidOperationException("git n'a pas pu être démarré");
        var tacheErreur = processus.StandardError.ReadToEndAsync();
        processus.StandardOutput.ReadToEnd();
        processus.WaitForExit();
        string erreur = tacheErreur.GetAwaiter().GetResult();
        if (processus.ExitCode != 0)
        {
            throw new InvalidOperationException($"git {string.Join(' ', args)} a échoué dans {depot} : {erreur.Trim()}");
        }
    }

    /// <summary>
    /// Crée un dépôt git minimal, identité locale fixée (aucune dépendance à
    /// la config git globale de la machine), sans aucun remote.
    /// <c>core.quotepath=false</c> posé par construction — piège d'environnement
    /// mesuré : un nom de fichier non-ASCII s'échapperait sinon en notation
    /// octale dans la sortie de `git ls-files`.
    /// </summary>
    public static void InitialiserDepotGit(string depot)
    {
        Directory.CreateDirectory(depot);
        ExecuterGit(depot, "-c", "init.defaultBranch=main", "init", "-q");
        ExecuterGit(depot, "config", "user.email", "test-writer@haversack.invalid");
        ExecuterGit(depot, "config", "user.name", "Test Writer (fixture)");
        ExecuterGit(depot, "config", "core.quotepath", "false");
    }

    public static void Ecrire(string chemin, string contenu)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(chemin)!);
        File.WriteAllText(chemin, contenu, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));
    }

    /// <summary>
    /// Écrit des octets bruts — utilisé pour injecter une séquence UTF-8
    /// invalide dans une fixture, hors d'atteinte de <see cref="Ecrire"/>
    /// qui ne prend qu'un <see cref="string"/> déjà valide.
    /// </summary>
    public static void EcrireOctets(string chemin, byte[] octets)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(chemin)!);
        File.WriteAllBytes(chemin, octets);
    }

    public static void CommiterTout(string depot, string message = "fixture")
    {
        ExecuterGit(depot, "add", "-A");
        ExecuterGit(depot, "commit", "-q", "-m", message);
    }

    private static string ExecuterGitEtLire(string depot, params string[] args)
    {
        var depart = new ProcessStartInfo("git") { RedirectStandardOutput = true, RedirectStandardError = true, UseShellExecute = false };
        depart.ArgumentList.Add("-C");
        depart.ArgumentList.Add(depot);
        foreach (var a in args) depart.ArgumentList.Add(a);

        using var processus = Process.Start(depart)
            ?? throw new InvalidOperationException("git n'a pas pu être démarré");
        string sortie = processus.StandardOutput.ReadToEndAsync().GetAwaiter().GetResult();
        string erreur = processus.StandardError.ReadToEndAsync().GetAwaiter().GetResult();
        processus.WaitForExit();
        if (processus.ExitCode != 0)
        {
            throw new InvalidOperationException($"git {string.Join(' ', args)} a échoué dans {depot} : {erreur.Trim()}");
        }
        return sortie.Trim();
    }

    /// <summary>
    /// Ajoute un remote <c>origin</c> fictif (jamais contacté — aucun test de
    /// ce dépôt n'invoque un vrai réseau) et, si <paramref name="pousser"/>,
    /// simule localement que la révision courante est déjà présente côté
    /// distant en créant la référence de suivi <c>refs/remotes/origin/&lt;branche&gt;</c> —
    /// exactement ce que lirait <c>RevisionGuard.CommitPresentSurDistant</c>
    /// après un vrai <c>git fetch</c> d'une révision déjà poussée. Équivalent
    /// du geste fait à la main dans <c>_construire_depot</c> côté Python.
    /// </summary>
    public static void ConfigurerOrigineFictive(string depot, string urlOrigin, bool pousser)
    {
        ExecuterGitEtLire(depot, "remote", "add", "origin", urlOrigin);
        if (pousser)
        {
            string branche = ExecuterGitEtLire(depot, "rev-parse", "--abbrev-ref", "HEAD");
            string commit = ExecuterGitEtLire(depot, "rev-parse", "HEAD");
            ExecuterGitEtLire(depot, "update-ref", $"refs/remotes/origin/{branche}", commit);
        }
    }

    private static string CheminReel(string nomOutil)
    {
        var depart = new ProcessStartInfo("which") { RedirectStandardOutput = true, UseShellExecute = false };
        depart.ArgumentList.Add(nomOutil);
        using var processus = Process.Start(depart)
            ?? throw new InvalidOperationException("« which » n'a pas pu être démarré");
        string sortie = processus.StandardOutput.ReadToEndAsync().GetAwaiter().GetResult();
        processus.WaitForExit();
        if (processus.ExitCode != 0 || sortie.Trim().Length == 0)
        {
            throw new InvalidOperationException($"outil requis introuvable sur cette machine : {nomOutil}");
        }
        return sortie.Trim();
    }

    private static void SymlinkSiAbsent(string lien, string cible)
    {
        if (!File.Exists(lien) && !Directory.Exists(lien))
        {
            File.CreateSymbolicLink(lien, cible);
        }
    }

    /// <summary>
    /// Répertoire ne portant qu'un lien vers <c>git</c> réel — jamais vers
    /// <c>gh</c>. Utilisé comme PATH (intégral, pas complété) d'un
    /// sous-processus pour garantir qu'aucun appel réseau vers l'API GitHub
    /// n'est possible, quel que soit l'état d'authentification <c>gh</c> de la
    /// machine hôte. Équivalent de <c>helpers.repertoire_bin_sans_gh</c> côté
    /// Python.
    /// </summary>
    public static string RepertoireBinSansGh(string racineTemp)
    {
        string binDir = Path.Combine(racineTemp, "bin-sans-gh");
        Directory.CreateDirectory(binDir);
        SymlinkSiAbsent(Path.Combine(binDir, "git"), CheminReel("git"));
        return binDir;
    }

    /// <summary>
    /// Répertoire portant <c>git</c> réel ET le faux <c>gh</c> de ce harnais
    /// (<c>tools/Harnais/FauxGh</c>, copié dans le répertoire de sortie de ce
    /// projet de test par la seule vertu de son <c>ProjectReference</c>) —
    /// jamais le vrai <c>gh</c>. À utiliser comme PATH (intégral) d'un
    /// sous-processus dont on veut exercer une branche « gh disponible »
    /// sans jamais risquer un appel réseau réel.
    /// </summary>
    public static string RepertoireBinAvecFauxGh(string racineTemp)
    {
        string binDir = Path.Combine(racineTemp, "bin-avec-faux-gh");
        Directory.CreateDirectory(binDir);
        SymlinkSiAbsent(Path.Combine(binDir, "git"), CheminReel("git"));
        SymlinkSiAbsent(Path.Combine(binDir, "gh"), CheminBinaire("gh"));
        return binDir;
    }

    /// <summary>
    /// Variante de <see cref="ExecuterOutil"/> substituant tout ou partie de
    /// l'environnement du sous-processus (PATH restreint, variables du faux
    /// <c>gh</c> — <c>JOURNAL_GH</c>, <c>FAUX_GH_SORTIE</c>, <c>FAUX_GH_CODE</c>) —
    /// ajout pur : aucun appelant existant ne passe par ici, le comportement
    /// de <see cref="ExecuterOutil"/> lui-même est inchangé. L'entrée standard
    /// du sous-processus est explicitement fermée aussitôt après son
    /// démarrage — un outil invoqué sous ce harnais qui la lirait sans
    /// condition (le faux <c>gh</c>, si son entrée est redirigée mais jamais
    /// close) bloquerait sinon indéfiniment (piège d'environnement mesuré).
    /// </summary>
    public static ResultatExecution ExecuterOutilAvecEnvironnement(
        string nomOutil, IEnumerable<string> args, string cwd,
        IReadOnlyDictionary<string, string> environnement, int timeoutMs = 30_000)
    {
        var depart = new ProcessStartInfo(CheminBinaire(nomOutil))
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            RedirectStandardInput = true,
            UseShellExecute = false,
            WorkingDirectory = cwd,
        };
        foreach (var a in args) depart.ArgumentList.Add(a);
        foreach (var (cle, valeur) in environnement) depart.EnvironmentVariables[cle] = valeur;

        using var processus = Process.Start(depart)
            ?? throw new InvalidOperationException($"{nomOutil} n'a pas pu être démarré");
        processus.StandardInput.Close();

        var tacheSortie = processus.StandardOutput.ReadToEndAsync();
        var tacheErreur = processus.StandardError.ReadToEndAsync();
        if (!processus.WaitForExit(timeoutMs))
        {
            processus.Kill(entireProcessTree: true);
            throw new TimeoutException($"{nomOutil} n'a pas rendu la main sous {timeoutMs} ms");
        }
        string stdout = tacheSortie.GetAwaiter().GetResult();
        string stderr = tacheErreur.GetAwaiter().GetResult();
        return new ResultatExecution(stdout, stderr, processus.ExitCode);
    }
}

/// <summary>
/// Répertoire temporaire système (jamais sous le dépôt suivi par git),
/// détruit à la fin du test qui le possède — équivalent de
/// <c>tempfile.TemporaryDirectory()</c> utilisé en <c>setUp</c>/<c>tearDown</c>
/// côté Python, porté ici sur <see cref="IDisposable"/> (xUnit instancie une
/// fixture de test par méthode, puis la dispose).
/// </summary>
internal sealed class RepertoireTemporaire : IDisposable
{
    public string Chemin { get; }

    public RepertoireTemporaire()
    {
        Chemin = Path.Combine(Path.GetTempPath(), "haversack-caracterisation-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(Chemin);
    }

    public void Dispose()
    {
        try { Directory.Delete(Chemin, recursive: true); }
        catch (IOException) { /* best-effort — ne fait jamais échouer un test sur le nettoyage. */ }
    }
}
