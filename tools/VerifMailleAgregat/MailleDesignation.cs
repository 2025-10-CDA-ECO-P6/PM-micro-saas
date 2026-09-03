using System.Text.RegularExpressions;

namespace VerifMailleAgregat;

/// <summary>
/// Les natures admises d'un item de « Périmètre d'écriture », au-delà des
/// agrégats de domaine, des fiches d'écran et des object stores — et la
/// résolution d'un item vers sa nature.
/// </summary>
internal static class MailleDesignation
{
    public static readonly HashSet<string> Transverses = new(StringComparer.Ordinal)
    {
        "SharedKernel", "Haversack.Infrastructure.Notifications",
    };

    // Modules de la couche cliente : les six préoccupations nommées par le corpus (§2, tableau).
    public static readonly HashSet<string> Client = new(StringComparer.Ordinal)
    {
        "service d'accès au store", "châssis", "bandeaux transversaux",
        "sanitisation", "politique de sécurité de contenu", "projection d'export",
    };

    // Module d'outillage : liste fermée à quatre libellés (§2, entre guillemets typographiques `…`).
    //
    // Divergence C — cf.
    // tools/Tests/DivergencesEntreOutilsTests.cs::DivergenceC_MailleLueOuCodeeEnDur.
    // Seul le TRAITEMENT de cette divergence est arbitré ici : reproduire telle quelle,
    // sans corriger. Le FOND — laquelle des deux implémentations est juste — n'est pas
    // tranché par ce commentaire. Cette liste est codée en dur, jamais extraite du §2 du
    // plan comme le fait le générateur d'issues.
    public static readonly HashSet<string> Outillage = new(StringComparer.Ordinal)
    {
        "le projet de test d'architecture", "le projet de test de bout en bout",
        "la configuration de la solution", "la configuration d'intégration continue",
    };

    // Namespaces de bounded context : n'apparaissent comme module que sur la seule tranche qui les
    // crée — TB-003, nommée en toutes lettres par le §2 lui-même.
    //
    // Divergence D — cf.
    // tools/Tests/DivergencesEntreOutilsTests.cs::DivergenceD_VerrouDuNamespaceSurLaTrancheDeCreation.
    // Seul le TRAITEMENT de cette divergence est arbitré ici : reproduire telle quelle,
    // sans corriger. Le FOND — NON un défaut, mais quel choix de MODÉLISATION retenir —
    // n'est pas tranché par ce commentaire. Cette clause d'exception, avec son identifiant
    // de tranche en dur, est reproduite telle quelle — le générateur d'issues ne
    // l'implémente pas.
    public static readonly HashSet<string> NamespacesBc = new(StringComparer.Ordinal)
    {
        "IdentityAccess", "SpaceManagement", "ContentLibrary", "SessionConduct",
    };
    public const string TrancheCreationNamespaces = "TB-003";

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
        IReadOnlySet<string> stores)
    {
        if (agregats.ContainsKey(item)) return "agrégat";
        if (Transverses.Contains(item)) return "transverse nommé";
        if (NamespacesBc.Contains(item))
        {
            return string.Equals(tacheId, TrancheCreationNamespaces, StringComparison.Ordinal)
                ? "namespace de bounded context"
                : null;
        }
        if (Client.Contains(item)) return "module de la couche cliente";
        if (Outillage.Contains(item)) return "module d'outillage";
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
