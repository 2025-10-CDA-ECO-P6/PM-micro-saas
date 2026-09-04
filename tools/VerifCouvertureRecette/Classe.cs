namespace VerifCouvertureRecette;

/// <summary>
/// Classement d'un cas de recette face au plan de travail. Seul
/// <see cref="AbsentTrou"/> est le défaut que ce contrôle existe pour trouver ;
/// les trois autres classes prouvent qu'une absence n'a pas été noyée dans ce
/// compteur sans justification.
/// </summary>
internal enum Classe
{
    /// <summary>
    /// Le cahier marque lui-même ce cas comme retiré, dans sa propre colonne
    /// <c>Cas</c> — un cas mort n'a structurellement rien à prouver, donc rien
    /// à citer. Priorité sur toute autre classe, y compris <see cref="Synthese"/>
    /// et <see cref="Cite"/> : un cas retiré l'est quoi qu'il en soit par
    /// ailleurs (mandat opérateur).
    /// </summary>
    Retire,

    /// <summary>Le plan cite l'identifiant, n'importe où dans son texte.</summary>
    Cite,

    /// <summary>
    /// Son use case est au jalon J2 ou J3 (§4) : le plan s'arrête au niveau
    /// épique pour ces jalons (§7, §8) — absence légitime.
    /// </summary>
    AbsentJalonUlterieur,

    /// <summary>
    /// Son use case n'apparaît pas dans « §4. Vue des épiques » : hors du
    /// périmètre MoSCoW du MVP (UC-13, UC-15) — absence légitime.
    /// </summary>
    AbsentHorsMvp,

    /// <summary>
    /// Son use case est au jalon J0 ou J1 et rien ne le mentionne : le plan
    /// aurait dû le citer et ne l'a pas fait — le défaut recherché.
    /// </summary>
    AbsentTrou,

    /// <summary>
    /// Sa colonne Source ne cite que d'autres cas de recette, jamais une story
    /// (cas transverses, §10) : couche de second niveau, jamais comptée comme trou.
    /// </summary>
    Synthese,
}
