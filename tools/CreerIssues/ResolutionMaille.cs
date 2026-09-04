using System.Text.RegularExpressions;

namespace CreerIssues;

/// <summary>
/// Résolution jalon / nature de tranche d'une tâche, depuis les valeurs de
/// champ « Jalon » et « Périmètre d'écriture » lues dans le plan — hors
/// socle « lecture du plan » : cette résolution ne relève pas de la lecture
/// elle-même, et `CreerProjet` ne la consomme pas.
/// </summary>
internal static class ResolutionMaille
{
    // \b : frontière de mot Python (LimiteMotPython), pas celle du moteur
    // .NET — voir la documentation de LimiteMotPython.
    private static readonly Regex MotifJalon = new(@"^(J[0-3])", RegexOptions.CultureInvariant);

    private static readonly Regex MotifSeparateurSegments = new(
        $"{BlancPython.Classe}*;{BlancPython.Classe}*", RegexOptions.CultureInvariant);

    private static readonly Regex MotifFicheEcran = new(
        $"^fiche{BlancPython.Classe}+`([^`]+)`$", RegexOptions.CultureInvariant);

    // Ordre fixe et explicite plutôt que l'ordre d'énumération d'un
    // Dictionary<> — non garanti par le contrat .NET, à la différence de
    // l'ordre d'insertion d'un dict Python (3.7+), dont dépend le choix de
    // nature retenu par le premier segment appartenant à plusieurs natures.
    private static readonly string[] OrdreNatures = { "Comportement", "Couche cliente", "Outillage", "Socle" };

    /// <summary>
    /// Clé de jalon (« J0 » à « J3 ») en tête d'une valeur — celle d'un champ
    /// de fiche (`| Jalon | J0 |`) comme celle d'une valeur de taxonomie
    /// (`J0 — Socle`) : les deux ne coïncident pas forcément au caractère
    /// près, seule cette clé les relie. <see langword="null"/> si la valeur
    /// ne commence pas par une clé de jalon suivie d'une frontière de mot.
    /// </summary>
    public static string? ClePrefixeJalon(string valeur)
    {
        string strippe = TexteUnicode.StripPython(valeur);
        var m = MotifJalon.Match(strippe);
        return m.Success && LimiteMotPython.FrontiereApres(strippe, m.Length) ? m.Groups[1].Value : null;
    }

    /// <summary>
    /// Résout la valeur de taxonomie (potentiellement porteuse d'un intitulé,
    /// ex. « J0 — Socle ») dont la clé de jalon correspond à celle du champ de
    /// fiche — jamais la clé elle-même, qui ne coïncide plus nécessairement
    /// avec le nom posé sur l'objet GitHub depuis que la taxonomie porte
    /// l'intitulé du jalon en plus de sa clé.
    /// </summary>
    public static (string? Jalon, string? Anomalie) ResoudreJalon(string valeurChamp, IReadOnlyCollection<string> jalonsValides)
    {
        string? cle = ClePrefixeJalon(valeurChamp);
        string? valeur = cle is not null ? jalonsValides.FirstOrDefault(v => ClePrefixeJalon(v) == cle) : null;
        if (valeur is not null)
        {
            return (valeur, null);
        }
        return (null, $"valeur de jalon non résolue dans la taxonomie : {ReprPython.Chaine(valeurChamp)}");
    }

    /// <summary>
    /// Un champ dont la valeur commence par le marqueur d'absence « TROU »
    /// est indivisible : son texte d'explication peut lui-même contenir des
    /// points-virgules, qui ne sont alors pas des séparateurs de module.
    /// </summary>
    public static (List<string> Segments, string? Marqueur) DecouperPerimetre(string valeurChamp)
    {
        string valeur = TexteUnicode.StripPython(valeurChamp);
        if (valeur.StartsWith("HORS MAILLE", StringComparison.Ordinal))
        {
            return (new List<string>(), "hors_maille");
        }
        if (valeur.StartsWith("TROU", StringComparison.Ordinal))
        {
            return (new List<string> { valeur }, "trou");
        }
        var segments = MotifSeparateurSegments.Split(valeur)
            .Select(TexteUnicode.StripPython)
            .Where(s => s.Length > 0)
            .ToList();
        return (segments, null);
    }

    /// <summary>
    /// Déduit le libellé de nature de tranche depuis le périmètre d'écriture.
    /// Renvoie (libellés déduits, segments non résolus).
    /// </summary>
    public static (HashSet<string> Libelles, List<string> Residuels) MapperNature(
        List<string> segments, string? marqueur, IReadOnlyDictionary<string, HashSet<string>> maille,
        IReadOnlySet<string> slugsWireframes, IReadOnlySet<string> naturesValides)
    {
        if (marqueur == "hors_maille")
        {
            bool valide = naturesValides.Contains("Hors maille");
            var libellesHm = valide ? new HashSet<string>(StringComparer.Ordinal) { "Hors maille" } : new HashSet<string>(StringComparer.Ordinal);
            var residuelsHm = valide
                ? new List<string>()
                : new List<string> { "correspondance déduite vers « Hors maille », absente de la taxonomie lue" };
            return (libellesHm, residuelsHm);
        }
        if (marqueur == "trou")
        {
            return (new HashSet<string>(StringComparer.Ordinal), new List<string>());
        }

        var libelles = new HashSet<string>(StringComparer.Ordinal);
        var residuels = new List<string>();

        foreach (var segment in segments)
        {
            var mFiche = MotifFicheEcran.Match(segment);
            if (mFiche.Success)
            {
                if (slugsWireframes.Contains(mFiche.Groups[1].Value))
                {
                    libelles.Add("Surface");
                }
                else
                {
                    residuels.Add($"{segment} — slug de fiche d'écran non reconnu sur disque");
                }
                continue;
            }

            string? natureTrouvee = null;
            foreach (var nature in OrdreNatures)
            {
                if (maille.TryGetValue(nature, out var noms) && noms.Contains(segment))
                {
                    natureTrouvee = nature;
                    break;
                }
            }

            if (natureTrouvee is not null)
            {
                libelles.Add(natureTrouvee);
            }
            // Un projet .NET désigné par son rôle, non énuméré par la puce du §2.
            else if (segment.StartsWith("le projet ", StringComparison.Ordinal)
                || segment.StartsWith("les autres projets", StringComparison.Ordinal))
            {
                libelles.Add("Socle");
            }
            else
            {
                residuels.Add(segment);
            }
        }

        var libellesValides = new HashSet<string>(libelles.Where(naturesValides.Contains), StringComparer.Ordinal);
        foreach (var l in libelles.Where(l => !libellesValides.Contains(l)))
        {
            residuels.Add($"correspondance déduite vers « {l} », absente de la taxonomie lue");
        }
        return (libellesValides, residuels);
    }
}
