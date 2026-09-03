namespace CreerIssues;

/// <summary>
/// Slugs de fiches d'écran indexés sur disque sous
/// `docs/conception/interface/wireframes/&lt;groupe&gt;/&lt;slug&gt;/` — hors
/// socle « lecture du plan » : `CreerProjet` ne les consomme pas.
/// </summary>
internal static class WireframeSlugs
{
    public static HashSet<string> Lister(string racine)
    {
        var slugs = new HashSet<string>(StringComparer.Ordinal);
        string racineWireframes = Path.Combine(racine, Chemins.WireframesRelatif);
        if (!Directory.Exists(racineWireframes)) return slugs;

        // Path.iterdir() de Python liste toute entrée, dotfiles compris —
        // contrairement à glob.glob(), qui les exclut par défaut ; ici aucun
        // filtre de tête pointée n'est donc appliqué, à la différence des
        // lectures qui passent par glob ailleurs dans cet outillage.
        foreach (var groupe in Directory.EnumerateFileSystemEntries(racineWireframes))
        {
            if (!Directory.Exists(groupe)) continue;
            foreach (var fiche in Directory.EnumerateFileSystemEntries(groupe))
            {
                if (Directory.Exists(fiche))
                {
                    slugs.Add(Path.GetFileName(fiche));
                }
            }
        }
        return slugs;
    }
}
