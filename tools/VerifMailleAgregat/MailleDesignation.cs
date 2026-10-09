using System.Text.RegularExpressions;

namespace VerifMailleAgregat;

/// <summary>
/// Les natures admises d'un item de « Périmètre d'écriture », au-delà des
/// agrégats de domaine, des fiches d'écran et des object stores — et la
/// résolution d'un item vers sa nature.
/// </summary>
internal static class MailleDesignation
{
    // Les modules transverses nommés, ceux de la couche cliente, les modules d'outillage, les
    // namespaces de bounded context et la tranche qui les crée ne figurent pas ici : ils se lisent
    // dans le § 2 du plan (ModulesOutillage), jamais recopiés en dur.

    // Projet .NET désigné par son rôle tant que son nom n'est pas tranché (§2, convention de
    // désignation de structure-projets.md § Convention de désignation) : Domaine et Application
    // sont des projets uniques (ADR-008), seules ces deux formes canoniques exactes se résolvent
    // ici. Infrastructure et Présentation restent multi-projets : « combien, lesquels » relève
    // de J0, donc aucune forme canonique n'existe encore pour elles — signalées à part plutôt
    // qu'absorbées dans « résolu ».
    public static readonly HashSet<string> ProjetRoleClos = new(StringComparer.Ordinal)
    {
        "le projet Domaine unique", "le projet Application unique",
    };

    private static readonly Regex MotifProjetRoleOuvert = new(
        @"^(le|les)\s+(autres\s+)?projets?\b.*(Infrastructure|Présentation)",
        RegexOptions.CultureInvariant);

    public const string OuvertJ0 = "désignation de rôle non clôturable avant J0 (Infrastructure/Présentation, multi-projets)";

    private static readonly Regex MotifFicheEcran = new(
        @"^fiche\s+`?([a-z0-9-]+)`?$", RegexOptions.CultureInvariant);

    private static readonly Regex MotifStoreLocal = new(
        @"^store local\s+`?([a-z_]+)`?$", RegexOptions.CultureInvariant);

    /// <summary>
    /// Nature de l'item, ou <see langword="null"/> s'il ne se résout pas dans la maille.
    /// </summary>
    public static string? Resout(
        string item,
        string tacheId,
        IReadOnlyDictionary<string, string> agregats,
        IReadOnlySet<string> fiches,
        IReadOnlySet<string> stores,
        ModulesDuPlan modules)
    {
        if (agregats.ContainsKey(item)) return "agrégat";
        if (modules.Transverses.Contains(item)) return "transverse nommé";
        if (modules.Namespaces.Noms.Contains(item))
        {
            return modules.Namespaces.ResoutSur(item, tacheId) ? "namespace de bounded context" : null;
        }
        if (modules.Client.Contains(item)) return "module de la couche cliente";
        if (modules.Outillage.Contains(item)) return "module d'outillage";
        if (ProjetRoleClos.Contains(item)) return "projet .NET (rôle, unicité tranchée)";
        if (MotifProjetRoleOuvert.IsMatch(item)) return OuvertJ0;

        var mFiche = MotifFicheEcran.Match(item);
        if (mFiche.Success)
        {
            return fiches.Contains(mFiche.Groups[1].Value) ? "fiche d'écran" : null;
        }

        var mStore = MotifStoreLocal.Match(item);
        if (mStore.Success)
        {
            return stores.Contains(mStore.Groups[1].Value) ? "object store" : null;
        }

        return null;
    }
}
