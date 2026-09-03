namespace FauxGh;

/// Constantes de la garde d'identité, partagées en forme (pas en référence —
/// dupliquées dans `GardeIdentiteGh`, comme partout ailleurs dans cet
/// outillage, ce dépôt ne faisant jamais référencer un projet d'outillage par
/// un autre). Vérifié : sur une sous-commande qu'il ne reconnaît pas, le vrai
/// gh n'écrit rien sur sa sortie standard et sort en erreur — il ne peut donc
/// pas produire la réponse fixée ici.
internal static class IdentiteSentinelle
{
    public const string Argument = "identite-faux-gh--ne-jamais-utiliser-ailleurs";
    public const string Reponse = "FAUX-GH-IDENTITE-2f6a19c4-verifie";
}
