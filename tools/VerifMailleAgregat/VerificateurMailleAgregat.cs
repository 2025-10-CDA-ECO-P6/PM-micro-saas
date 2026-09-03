namespace VerifMailleAgregat;

/// <summary>
/// Contrôle la maille de désignation « agrégat » proposée pour les tranches
/// verticales : chaque item de « Périmètre d'écriture » du plan doit se
/// résoudre dans l'une des natures admises, `En conflit avec` doit coïncider
/// avec l'intersection recalculée des périmètres, et aucune fiche vivante ne
/// doit renvoyer vers un identifiant retiré. Ne lit et n'écrit rien au-delà
/// de la racine reçue et du <see cref="TextWriter"/> fourni.
/// </summary>
internal static class VerificateurMailleAgregat
{
    public static Verdict Executer(string racine, string cheminPlanRelatif, TextWriter sortie)
    {
        var ag = ModeleDomaine.Extraire(racine);
        var fi = AutresNatures.FichesEcran(racine);
        var st = AutresNatures.StoresLocaux(racine);

        string textePlan = PlanDeTravail.LireTexte(racine, cheminPlanRelatif);
        var tb = PlanDeTravail.ExtraireTaches(textePlan);
        var retires = PlanDeTravail.ExtraireIdentifiantsRetires(textePlan);

        var idsTries = tb.Keys.ToList();
        idsTries.Sort(ComparateurCodePoints.Instance);

        var nonResolus = new List<(string Id, List<string> Items)>();
        var enAttenteJ0 = new List<(string Id, List<string> Items)>();
        var horsMailles = new List<string>();
        var trous = new List<string>();
        int ok = 0;

        foreach (var id in idsTries)
        {
            var tache = tb[id];
            if (tache.HorsMaille) { horsMailles.Add(id); continue; }
            if (tache.Trou) { trous.Add(id); continue; }

            var mauvais = new List<string>();
            var ouverts = new List<string>();
            foreach (var item in tache.Items)
            {
                string? nature = MailleDesignation.Resout(item, id, ag, fi, st);
                if (nature is null) mauvais.Add(item);
                else if (string.Equals(nature, MailleDesignation.OuvertJ0, StringComparison.Ordinal)) ouverts.Add(item);
            }

            // « résolu » doit rester une affirmation soutenable : un item non clôturable
            // avant J0 n'est pas une erreur (mauvais), mais ce n'est pas davantage un
            // module vérifié — la tranche ne peut pas non plus compter dans « entièrement
            // résolu ».
            if (mauvais.Count > 0) nonResolus.Add((id, mauvais));
            else if (ouverts.Count > 0) enAttenteJ0.Add((id, ouverts));
            else ok++;
        }

        var ecarts = new List<(string A, string B, List<string> Partage, bool Declare)>();
        foreach (var a in idsTries)
        {
            foreach (var b in idsTries)
            {
                if (ComparateurCodePoints.Instance.Compare(a, b) >= 0) continue;
                var ta = tb[a];
                var tbb = tb[b];
                if (ta.Trou || tbb.Trou || ta.HorsMaille || tbb.HorsMaille) continue;

                var partageSet = new HashSet<string>(ta.Items, StringComparer.Ordinal);
                partageSet.IntersectWith(tbb.Items);
                var partage = partageSet.ToList();
                partage.Sort(ComparateurCodePoints.Instance);

                bool declare = ta.Conflits.Contains(b) || tbb.Conflits.Contains(a);
                if ((partage.Count > 0) != declare)
                {
                    ecarts.Add((a, b, partage, declare));
                }
            }
        }

        // --- Partie B : disjonction fiches vivantes / identifiants retirés --------------
        // Propriété tenue au commit gelé (52 fiches vivantes, 43 identifiants retirés,
        // intersection vide, zéro renvoi mort) : instrumentée ici, pas réparée.
        var fichesVivantes = new HashSet<string>(tb.Keys, StringComparer.Ordinal);
        var ambigus = new HashSet<string>(fichesVivantes, StringComparer.Ordinal);
        ambigus.IntersectWith(retires);
        int identifiantsAmbigus = ambigus.Count;

        var renvoisMorts = new List<(string Tache, string Champ, List<string> Ids)>();
        foreach (var id in idsTries)
        {
            var tache = tb[id];

            var dependMorts = tache.DependDe.Where(retires.Contains).ToList();
            dependMorts.Sort(ComparateurCodePoints.Instance);
            if (dependMorts.Count > 0) renvoisMorts.Add((id, "Dépend de", dependMorts));

            var conflitsMorts = tache.Conflits.Where(retires.Contains).ToList();
            conflitsMorts.Sort(ComparateurCodePoints.Instance);
            if (conflitsMorts.Count > 0) renvoisMorts.Add((id, "En conflit avec", conflitsMorts));
        }
        int totalRenvoisMorts = renvoisMorts.Sum(r => r.Ids.Count);

        EcrireRapport(sortie, ag, fi, st, tb, nonResolus, enAttenteJ0, ok, horsMailles, trous,
            ecarts, identifiantsAmbigus, renvoisMorts, totalRenvoisMorts);

        return new Verdict(nonResolus.Count, ecarts.Count, identifiantsAmbigus, totalRenvoisMorts);
    }

    private static void EcrireRapport(
        TextWriter sortie,
        Dictionary<string, string> ag,
        HashSet<string> fi,
        HashSet<string> st,
        Dictionary<string, Tache> tb,
        List<(string Id, List<string> Items)> nonResolus,
        List<(string Id, List<string> Items)> enAttenteJ0,
        int ok,
        List<string> horsMailles,
        List<string> trous,
        List<(string A, string B, List<string> Partage, bool Declare)> ecarts,
        int identifiantsAmbigus,
        List<(string Tache, string Champ, List<string> Ids)> renvoisMorts,
        int totalRenvoisMorts)
    {
        sortie.WriteLine($"agrégats extraits du modèle de domaine ({ag.Count}) :");
        var agTries = ag.Keys.ToList();
        agTries.Sort(ComparateurCodePoints.Instance);
        foreach (var n in agTries)
        {
            sortie.WriteLine($"    {TexteUnicode.RemplirCodePoints(n, 20)} ← {ag[n]}");
        }
        sortie.WriteLine($"fiches d'écran indexées      : {fi.Count}");
        sortie.WriteLine($"object stores indexés        : {st.Count}");
        sortie.WriteLine();

        sortie.WriteLine($"tâches du plan                        : {tb.Count}");
        sortie.WriteLine($"  périmètre entièrement résolu        : {ok}");
        sortie.WriteLine($"  périmètre HORS MAILLE (exclu)       : {horsMailles.Count}");
        sortie.WriteLine($"  périmètre TROU (non nommable)       : {trous.Count}");
        sortie.WriteLine($"  périmètre en attente de J0 (rôle)   : {enAttenteJ0.Count}");
        sortie.WriteLine($"  item ne se résolvant pas la maille  : {nonResolus.Count}");
        foreach (var (id, items) in nonResolus.Take(40))
        {
            sortie.WriteLine($"    [NON RÉSOLU] {id} -> {ReprPython.Liste(items)}");
        }
        foreach (var (id, items) in enAttenteJ0.Take(40))
        {
            sortie.WriteLine($"    [EN ATTENTE J0] {id} -> {ReprPython.Liste(items)}");
        }

        sortie.WriteLine();
        sortie.WriteLine($"écarts entre `En conflit avec` déclaré et l'intersection recalculée : {ecarts.Count}");
        foreach (var (a, b, partage, declare) in ecarts.Take(40))
        {
            string partageAffiche = partage.Count == 0 ? "∅" : ReprPython.Liste(partage);
            // bool.ToString() de .NET rend "True"/"False" — identique au rendu qu'une
            // f-string Python produit ici pour un booléen, par coïncidence de
            // représentation, pas par garantie de contrat entre les deux plateformes.
            sortie.WriteLine($"    [ÉCART] {a} ↔ {b} : partagé={partageAffiche} ; déclaré={declare}");
        }

        // --- Partie B : instrumentation, ajoutée en fin de rapport ---------------------
        // Placée après les quatre sections héritées du script Python plutôt qu'intercalée
        // entre elles : aucune ligne existante ne change de contenu ni d'ordre, seules des
        // lignes neuves s'ajoutent à la suite — c'est ce qui rend le différentiel de la
        // partie A trivial (l'oracle plus des lignes ajoutées, jamais des lignes déplacées).
        sortie.WriteLine();
        sortie.WriteLine($"identifiants à la fois fiche vivante et retirés (intersection)     : {identifiantsAmbigus}");
        sortie.WriteLine($"renvois `Dépend de` / `En conflit avec` vers un identifiant retiré : {totalRenvoisMorts}");
        foreach (var (tacheId, champ, ids) in renvoisMorts.Take(40))
        {
            sortie.WriteLine($"    [RENVOI MORT] {tacheId} ({champ}) -> {ReprPython.Liste(ids)}");
        }
    }
}
