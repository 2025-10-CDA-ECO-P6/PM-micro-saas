namespace GardeIdentiteGh;

/// Constantes de la garde d'identité — forme miroir de `FauxGh.IdentiteSentinelle`,
/// dupliquée plutôt que partagée (aucune référence de projet entre les deux
/// outils de ce harnais). Les deux fichiers doivent rester identiques en
/// valeur : c'est cette égalité, et elle seule, qui rend la preuve valide.
internal static class IdentiteSentinelle
{
    public const string Argument = "identite-faux-gh--ne-jamais-utiliser-ailleurs";
    public const string Reponse = "FAUX-GH-IDENTITE-2f6a19c4-verifie";
}
