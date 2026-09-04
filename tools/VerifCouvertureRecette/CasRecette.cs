namespace VerifCouvertureRecette;

/// <summary>
/// Une ligne de tableau du cahier de recette dont la première cellule ouvre
/// un identifiant <c>CR-…</c> : son identifiant tel quel, le contenu brut de
/// la colonne jouant le rôle de <c>Source</c> pour cette ligne — nommée
/// <c>Source</c> dans les tableaux par UC, <c>Renvoi</c> ou <c>Renvoi / Verdict</c>
/// dans les tableaux de cas transverses (§10, sous-sections « Cas transverses ») —
/// et le contenu brut de sa colonne <c>Cas</c>, seule colonne où le cahier
/// marque un cas retiré. Vide quand le tableau ne porte pas de colonne <c>Cas</c>
/// (cas transverses).
/// </summary>
internal sealed record CasRecette(string Id, string Source, string Cas);
