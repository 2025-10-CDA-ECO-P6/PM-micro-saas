namespace VerifCorpus;

/// <summary>
/// Rejoue <c>os.path</c> en mode POSIX (séparateur <c>/</c>) sur les chemins
/// relatifs que porte l'index git : ces chemins ne désignent jamais le système
/// de fichiers de la machine d'exécution directement (ils servent de clé de
/// dictionnaire et de comparaison), donc une résolution absolue ou une
/// consultation du disque via <see cref="Path"/> les dénaturerait.
/// </summary>
internal static class CheminsPosix
{
    /// <summary>
    /// Équivalent de <c>os.path.dirname</c> en mode POSIX : tout ce qui précède
    /// le dernier <c>/</c>, ou la chaîne vide s'il n'y en a pas.
    /// </summary>
    public static string Dirname(string chemin)
    {
        int position = chemin.LastIndexOf('/');
        return position < 0 ? string.Empty : chemin[..position];
    }

    /// <summary>
    /// Équivalent à deux arguments d'<c>os.path.join</c> : si le second segment
    /// est absolu (commence par <c>/</c>), il efface le premier — c'est le
    /// comportement réel d'<c>os.path.join</c>, pas une approximation.
    /// </summary>
    public static string Join(string a, string b)
    {
        if (b.StartsWith('/')) return b;
        if (a.Length == 0) return b;
        return a.EndsWith('/') ? a + b : a + "/" + b;
    }

    /// <summary>
    /// Équivalent lexical de <c>posixpath.normpath</c> : ne consulte jamais le
    /// système de fichiers, ne résout aucun lien symbolique, et conserve un
    /// <c>..</c> de tête quand aucun segment réel ne précède pour l'absorber
    /// (c'est le cas d'un chemin relatif — un chemin absolu, lui, absorbe
    /// silencieusement tout <c>..</c> surnuméraire au-delà de la racine).
    /// </summary>
    public static string NormPath(string chemin)
    {
        if (chemin.Length == 0) return ".";

        bool estAbsolu = chemin.StartsWith('/');
        // POSIX distingue un unique "//" de tête (préservé tel quel) d'un
        // groupe de trois "/" ou plus (ramené à un seul) ; historique du
        // standard POSIX, reconduit tel quel par posixpath.normpath.
        int nombreSlashesInitiaux = 0;
        if (estAbsolu)
        {
            nombreSlashesInitiaux = chemin.StartsWith("//") && !chemin.StartsWith("///") ? 2 : 1;
        }

        var segments = new List<string>();
        foreach (var segment in chemin.Split('/'))
        {
            if (segment.Length == 0 || segment == ".") continue;
            if (segment != ".."
                || (!estAbsolu && segments.Count == 0)
                || (segments.Count > 0 && segments[^1] == ".."))
            {
                segments.Add(segment);
            }
            else if (segments.Count > 0)
            {
                segments.RemoveAt(segments.Count - 1);
            }
        }

        string resultat = string.Join('/', segments);
        if (nombreSlashesInitiaux > 0)
        {
            resultat = new string('/', nombreSlashesInitiaux) + resultat;
        }
        return resultat.Length > 0 ? resultat : ".";
    }
}
