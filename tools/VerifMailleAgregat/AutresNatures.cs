using System.Text.RegularExpressions;

namespace VerifMailleAgregat;

/// <summary>
/// Les deux natures admises hors agrégat de domaine : une fiche d'écran indexée
/// sous les wireframes, un object store nommé par l'ADR du modèle IndexedDB local.
/// </summary>
internal static class AutresNatures
{
    private static readonly Regex MotifStoreCode = new(
        @"`([a-z][a-z_]+)`", RegexOptions.CultureInvariant);

    public static HashSet<string> FichesEcran(string racine)
    {
        var resultat = new HashSet<string>(StringComparer.Ordinal);
        string basePath = Path.Combine(racine, "docs/conception/interface/wireframes");
        if (!Directory.Exists(basePath)) return resultat;

        foreach (var svDir in Directory.EnumerateDirectories(basePath, "sv*"))
        {
            foreach (var entree in Directory.EnumerateFileSystemEntries(svDir))
            {
                string nom = Path.GetFileName(entree);
                // glob.glob ignore par défaut les entrées de tête pointée sur le
                // segment générique final « * » — une énumération .NET brute ne
                // les exclut pas.
                if (nom.StartsWith('.')) continue;
                if (Directory.Exists(entree)) resultat.Add(nom);
            }
        }
        return resultat;
    }

    public static HashSet<string> StoresLocaux(string racine)
    {
        var resultat = new HashSet<string>(StringComparer.Ordinal);
        string chemin = Path.Combine(racine, "docs/architecture/decisions/ADR-017-modele-indexeddb-local.md");
        if (!File.Exists(chemin)) return resultat;

        string texte = LecteurTexte.LireTexteIntegral(chemin);
        foreach (Match m in MotifStoreCode.Matches(texte))
        {
            resultat.Add(m.Groups[1].Value);
        }
        return resultat;
    }
}
