namespace VerifCouvertureRecette;

/// <summary>
/// Résultat structuré d'une passe de vérification de la couverture de recette.
/// Le point d'entrée traduit ce verdict en code de sortie ; la logique
/// elle-même n'analyse jamais sa propre sortie texte pour savoir si un défaut
/// a été rapporté.
/// </summary>
internal sealed record Verdict(int AbsentTrou, int SourceRetiree)
{
    /// <summary>
    /// Deux défauts de nature distincte, dans deux documents distincts : un cas
    /// de recette du périmètre engagé qu'aucune tranche ne cite (plan de
    /// travail), une colonne Source du cahier qui renvoie vers une règle
    /// retirée (cahier de recette). Ni l'un ni l'autre ne recouvre l'absence
    /// légitime (jalon ultérieur, hors MVP) ni la couche de synthèse.
    /// </summary>
    public bool ADesDefauts => AbsentTrou > 0 || SourceRetiree > 0;
}
