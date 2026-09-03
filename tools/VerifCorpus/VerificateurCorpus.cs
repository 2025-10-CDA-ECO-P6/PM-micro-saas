using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace VerifCorpus;

/// <summary>
/// Vérifie l'état du corpus Markdown : liens relatifs, ancres, fichiers
/// orphelins, blocs de code déséquilibrés. Ne lit et n'écrit rien au-delà de
/// la racine reçue et du <see cref="TextWriter"/> fourni — jamais directement
/// sur la console.
/// </summary>
internal static class VerificateurCorpus
{
    // Encodage UTF-8 strict et non consommateur de marque d'ordre d'octets :
    // - throwOnInvalidBytes lève sur toute séquence invalide, là où le lecteur
    //   .NET par défaut la remplacerait silencieusement par U+FFFD ;
    // - construit un GetString/StreamReader qui ne retire jamais un U+FEFF de
    //   tête, à l'inverse de la détection d'encodage par défaut de .NET. Cette
    //   fidélité inclut le faux positif mesuré côté Python (un titre précédé
    //   d'une marque n'est pas indexé, faute d'apparier le motif de titre) —
    //   vague 1 : on la reproduit, on ne la corrige pas.
    private static readonly UTF8Encoding Utf8Strict = new(
        encoderShouldEmitUTF8Identifier: false,
        throwOnInvalidBytes: true);

    private static readonly Regex MotifLien = new(
        @"\[(?:[^\[\]]|\[[^\[\]]*\])*\]\(([^()\s]+(?:\([^()\s]*\)[^()\s]*)*)\)",
        RegexOptions.CultureInvariant);

    private static readonly Regex MotifAtx = new(
        @"^(#{1,6})\s+(.*?)\s*#*\s*$",
        RegexOptions.CultureInvariant);

    private static readonly Regex MotifFence = new(
        @"^\s*(```+|~~~+)",
        RegexOptions.CultureInvariant);

    private static readonly Regex MotifAncreHtml = new(
        "<a\\s+(?:id|name)=\"([^\"]+)\"",
        RegexOptions.CultureInvariant);

    private static readonly Regex MotifBlocsCode = new(
        @"^(```+|~~~+).*?^\1\s*$",
        RegexOptions.Singleline | RegexOptions.Multiline | RegexOptions.CultureInvariant);

    // Ancrage explicite : re.match de Python ancre implicitement au début,
    // IsMatch de .NET cherche. Le schéma est la seule des cinq classes captée
    // ici à devoir aussi ignorer la casse — en culture invariante, pour ne pas
    // suivre la casse courante du thread.
    private static readonly Regex MotifSchemeExterne = new(
        @"^(https?:|mailto:|tel:|data:|ftp:)",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    public static Verdict Executer(string racine, TextWriter sortie)
    {
        var fichiers = ListerFichiersMd(racine);

        var index = new Dictionary<string, HashSet<string>>(StringComparer.Ordinal);
        var desequilibres = new List<string>();
        foreach (var rel in fichiers)
        {
            var (ancres, desequilibre) = AnalyserAncresEtFences(Path.Combine(racine, rel));
            index[rel] = ancres;
            if (desequilibre) desequilibres.Add(rel);
        }

        int nRel = 0, nAnc = 0;
        var cibleKo = new List<(string Source, string Cible)>();
        var ancreKo = new List<(string Source, string Cible)>();
        // defaultdict(set) : une entrée n'existe que si au moins un lien y renvoie.
        var entrants = new Dictionary<string, HashSet<string>>(StringComparer.Ordinal);

        foreach (var rel in fichiers)
        {
            string src = CheminsPosix.Dirname(rel);
            string texte = LireTexteIntegral(Path.Combine(racine, rel));
            texte = MotifBlocsCode.Replace(texte, "");

            foreach (Match m in MotifLien.Matches(texte))
            {
                string cible = m.Groups[1].Value;
                if (MotifSchemeExterne.IsMatch(cible)) continue;

                var (chemin, frag) = PartitionnerSurDiese(cible);
                if (chemin.Length == 0)
                {
                    nAnc++;
                    if (frag.Length > 0 && !index[rel].Contains(TexteUnicode.AbaisserCasseComplet(frag)))
                    {
                        ancreKo.Add((rel, cible));
                    }
                    continue;
                }

                nRel++;
                if (frag.Length > 0) nAnc++;

                string tgt = CheminsPosix.NormPath(CheminsPosix.Join(src, Uri.UnescapeDataString(chemin)));
                string absTgt = CheminsPosix.Join(racine, tgt);
                if (!Path.Exists(absTgt))
                {
                    cibleKo.Add((rel, cible));
                    continue;
                }

                if (tgt.EndsWith(".md", StringComparison.Ordinal))
                {
                    if (!entrants.TryGetValue(tgt, out var referents))
                    {
                        referents = new HashSet<string>(StringComparer.Ordinal);
                        entrants[tgt] = referents;
                    }
                    referents.Add(rel);

                    if (frag.Length > 0)
                    {
                        if (!index.TryGetValue(tgt, out var connues))
                        {
                            (connues, _) = AnalyserAncresEtFences(absTgt);
                            index[tgt] = connues;
                        }
                        if (!connues.Contains(TexteUnicode.AbaisserCasseComplet(Uri.UnescapeDataString(frag))))
                        {
                            ancreKo.Add((rel, cible));
                        }
                    }
                }
            }
        }

        var orphelins = fichiers.Where(f => !entrants.ContainsKey(f) && f != "README.md").ToList();

        EcrireRapport(sortie, fichiers, nRel, nAnc, cibleKo, ancreKo, orphelins, desequilibres);

        return new Verdict(cibleKo.Count, ancreKo.Count, orphelins.Count, desequilibres.Count);
    }

    private static void EcrireRapport(
        TextWriter sortie,
        List<string> fichiers,
        int nRel,
        int nAnc,
        List<(string Source, string Cible)> cibleKo,
        List<(string Source, string Cible)> ancreKo,
        List<string> orphelins,
        List<string> desequilibres)
    {
        int sousDocs = fichiers.Count(f => f.StartsWith("docs/", StringComparison.Ordinal));
        sortie.WriteLine($"fichiers markdown             : {fichiers.Count}  (sous docs/ : {sousDocs})");
        sortie.WriteLine($"liens relatifs                : {nRel}");
        sortie.WriteLine($"liens portant une ancre       : {nAnc}");
        sortie.WriteLine($"liens à cible inexistante     : {cibleKo.Count}");
        sortie.WriteLine($"liens à ancre inexistante     : {ancreKo.Count}");
        sortie.WriteLine($"fichiers orphelins            : {orphelins.Count}");
        sortie.WriteLine($"blocs de code déséquilibrés   : {desequilibres.Count}");

        foreach (var (etiquette, items) in new[] { ("CIBLE", cibleKo), ("ANCRE", ancreKo) })
        {
            foreach (var (s, t) in items.Take(30))
            {
                sortie.WriteLine($"  [{etiquette} CASSÉE] {s} -> {t}");
            }
        }
        foreach (var o in orphelins.Take(30))
        {
            sortie.WriteLine($"  [ORPHELIN] {o}");
        }
        foreach (var u in desequilibres.Take(30))
        {
            sortie.WriteLine($"  [FENCE] {u}");
        }
    }

    /// <summary>
    /// Équivalent de <c>cible.partition('#')</c> : (avant, après) le premier
    /// <c>#</c>, ou (cible, "") si aucun n'est présent.
    /// </summary>
    private static (string Chemin, string Fragment) PartitionnerSurDiese(string cible)
    {
        int position = cible.IndexOf('#');
        return position < 0 ? (cible, string.Empty) : (cible[..position], cible[(position + 1)..]);
    }

    private static string LireTexteIntegral(string chemin)
    {
        byte[] octets = File.ReadAllBytes(chemin);
        string texte = Utf8Strict.GetString(octets);
        return TexteUnicode.ConvertirRetourChariotIsole(texte);
    }

    private static (HashSet<string> Ancres, bool Desequilibre) AnalyserAncresEtFences(string chemin)
    {
        var ancres = new HashSet<string>(StringComparer.Ordinal);
        var vues = new Dictionary<string, int>(StringComparer.Ordinal);
        bool dansBloc = false;
        char? caractereFence = null;

        using var flux = File.OpenRead(chemin);
        // detectEncodingFromByteOrderMarks: false — une marque d'ordre d'octets
        // de tête doit rester le premier caractère du texte décodé, jamais être
        // absorbée par le lecteur (propriété 4).
        using var lecteur = new StreamReader(flux, Utf8Strict, detectEncodingFromByteOrderMarks: false);

        // ReadLine découpe déjà sur \r\n, \r et \n : les trois séparateurs de la
        // lecture universelle de Python, sans qu'un découpage manuel sur \n du
        // texte entier soit nécessaire ici.
        string? ligne;
        while ((ligne = lecteur.ReadLine()) is not null)
        {
            var mFence = MotifFence.Match(ligne);
            if (mFence.Success)
            {
                string fenceCourante = mFence.Groups[1].Value;
                if (!dansBloc)
                {
                    dansBloc = true;
                    caractereFence = fenceCourante[0];
                }
                else if (fenceCourante[0] == caractereFence
                    && TexteUnicode.StripPython(ligne) == fenceCourante)
                {
                    dansBloc = false;
                    caractereFence = null;
                }
                continue;
            }
            if (dansBloc) continue;

            var mAtx = MotifAtx.Match(ligne);
            if (mAtx.Success)
            {
                string baseSlug = Slug.Calculer(mAtx.Groups[2].Value);
                if (baseSlug.Length > 0)
                {
                    vues.TryGetValue(baseSlug, out int occurrences);
                    occurrences++;
                    vues[baseSlug] = occurrences;
                    ancres.Add(occurrences == 1 ? baseSlug : $"{baseSlug}-{occurrences - 1}");
                }
            }
            foreach (Match m in MotifAncreHtml.Matches(ligne))
            {
                ancres.Add(TexteUnicode.AbaisserCasseComplet(m.Groups[1].Value));
            }
        }

        return (ancres, dansBloc);
    }

    private static List<string> ListerFichiersMd(string racine)
    {
        var suivis = InvoquerGit(racine, "ls-files", "*.md");
        var neufs = InvoquerGit(racine, "ls-files", "--others", "--exclude-standard", "*.md");

        var ensemble = new HashSet<string>(StringComparer.Ordinal);
        foreach (var p in suivis) if (p.Length > 0) ensemble.Add(p);
        foreach (var p in neufs) if (p.Length > 0) ensemble.Add(p);

        var liste = ensemble.ToList();
        liste.Sort(ComparateurCodePoints.Instance);
        return liste;
    }

    private static List<string> InvoquerGit(string racine, params string[] argumentsGit)
    {
        var depart = new ProcessStartInfo("git")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };
        depart.ArgumentList.Add("-C");
        depart.ArgumentList.Add(racine);
        foreach (var a in argumentsGit) depart.ArgumentList.Add(a);

        Process processus;
        try
        {
            processus = Process.Start(depart)
                ?? throw new PanneExecutionException("git n'a pas pu être démarré");
        }
        catch (Win32Exception cause)
        {
            throw new PanneExecutionException("git est introuvable sur ce système", cause);
        }

        using (processus)
        {
            var tacheSortie = processus.StandardOutput.ReadToEndAsync();
            var tacheErreur = processus.StandardError.ReadToEndAsync();
            processus.WaitForExit();
            string sortieGit = tacheSortie.GetAwaiter().GetResult();
            string erreurGit = tacheErreur.GetAwaiter().GetResult();

            if (processus.ExitCode != 0)
            {
                throw new PanneExecutionException(
                    $"git a échoué (code {processus.ExitCode}) sur '{racine}' : {erreurGit.Trim()}");
            }

            // git quote/échappe tout chemin portant un caractère spécial (dont
            // les séparateurs de ligne autres que \n) : un découpage sur '\n'
            // seul suffit donc à répliquer str.splitlines() sur cette sortie,
            // sans reproduire les onze séparateurs que splitlines() reconnaît
            // en toute généralité.
            if (sortieGit.Length == 0) return new List<string>();
            var lignes = sortieGit.Split('\n').ToList();
            if (lignes.Count > 0 && lignes[^1].Length == 0) lignes.RemoveAt(lignes.Count - 1);
            return lignes;
        }
    }
}
