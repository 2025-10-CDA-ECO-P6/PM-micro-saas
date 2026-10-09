namespace VerifMarqueursIdentifiants;

/// <summary>
/// Confronte les constats d'un contrôle à la liste de référence. Seul un écart
/// absent de la liste, ou plus fréquent qu'elle ne l'accepte, est un échec ; les
/// entrées de la liste que le corpus ne justifie plus sont imprimées pour qu'on
/// la réduise, sans faire échouer.
/// </summary>
internal static class Comparaison
{
    public static bool Rapporter(
        string titre, IReadOnlyList<Constat> constats, IReadOnlyDictionary<string, int> acceptes, TextWriter sortie)
    {
        var parCle = constats
            .GroupBy(c => c.Cle, StringComparer.Ordinal)
            .OrderBy(g => g.Key, StringComparer.Ordinal)
            .ToList();

        sortie.WriteLine($"== {titre} ==");

        int nouveaux = 0;
        int tolerees = 0;
        foreach (var groupe in parCle)
        {
            int trouves = groupe.Count();
            acceptes.TryGetValue(groupe.Key, out int permis);
            if (trouves <= permis)
            {
                tolerees += trouves;
                continue;
            }
            nouveaux++;
            sortie.WriteLine(permis == 0
                ? $"ÉCART NOUVEAU {groupe.Key} : {trouves} constat(s)"
                : $"ÉCART NOUVEAU {groupe.Key} : {trouves} constat(s), {permis} accepté(s) par la liste de référence");
            foreach (var constat in groupe.OrderBy(c => c.Fichier, StringComparer.Ordinal).ThenBy(c => c.Ligne))
            {
                sortie.WriteLine($"  {constat.Emplacement} : {constat.Message}");
            }
        }

        var presentes = parCle.ToDictionary(g => g.Key, g => g.Count(), StringComparer.Ordinal);
        int disparues = 0;
        foreach (var (cle, permis) in acceptes.OrderBy(a => a.Key, StringComparer.Ordinal))
        {
            presentes.TryGetValue(cle, out int trouves);
            if (trouves >= permis) continue;
            disparues++;
            sortie.WriteLine($"DISPARU {cle} : {permis} accepté(s), {trouves} trouvé(s) — à retirer de la liste de référence");
        }

        sortie.WriteLine($"écarts acceptés par la liste de référence : {tolerees}");
        sortie.WriteLine($"écarts nouveaux                           : {nouveaux}");
        sortie.WriteLine($"entrées de la liste à réduire             : {disparues}");
        return nouveaux > 0;
    }
}
