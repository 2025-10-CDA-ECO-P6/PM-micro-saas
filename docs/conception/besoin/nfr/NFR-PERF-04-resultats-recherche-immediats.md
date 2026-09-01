# NFR-PERF-04 — Résultats de recherche immédiats

Famille **Performance perçue** · [Index de conception](../../README.md)

---

## Énoncé normatif

La recherche dans le contenu d'une campagne produit des résultats visibles avant que le MJ
n'ait eu le temps de détourner le regard de la table.

---

## Raison d'être

UC-14 énonce sa raison d'être en une phrase : l'accès rapide à l'information. Il couvre deux
situations distinctes que le persona central, **[Nadia](../persona/persona-04-nadia.md)**,
incarne chacune différemment : la retrouvabilité post-absence (elle ne se souvient plus où
est rangée une information après plusieurs semaines) et l'accès rapide en session (retrouver
un PNJ ou une note en quelques secondes pendant la partie).

Dans le contexte de la vue session (UC-06 A3 : « le MJ cherche un document absent de ses
panneaux configurés en pleine session »), la recherche est un mécanisme de secours en
temps réel. Le MJ l'utilise parce que le document dont il a besoin n'est pas dans ses
panneaux. Si la recherche prend du temps, il perd le fil de la scène narrative en cours.
Le délai de la recherche se traduit directement par une interruption perçue à la table.

**[Thomas](../persona/persona-01-thomas.md)** est moins dépendant de la recherche car son
organisation lui suffit généralement — UC-14 personas le mentionne explicitement. Mais
lorsqu'il l'utilise pendant la préparation, il cherche par titre et s'attend à retrouver
immédiatement ce qu'il a nommé. Émilie — citée dans UC-14 — représente le cas de session
intensive : retrouver un PNJ en quelques secondes, sans couper le rythme de la partie.

UC-14 est également le point d'entrée dans le contenu pour les MJ qui ne se souviennent
plus de leur arborescence. Si la recherche tarde à répondre, la résistance à l'utilisation
augmente, et le MJ revient à ses anciennes habitudes (parcourir des fichiers) — ce qui
est exactement la douleur que le produit adresse.

---

## Portée et hors-portée

**Ce que cette exigence couvre :**

- La recherche déclenchée par le MJ depuis la barre de recherche globale de la campagne,
  depuis la bibliothèque de contenu (en préparation) et depuis la vue session (en cours
  de partie) — UC-14 scénario nominal et US-14-01, US-14-03.
- La recherche filtrée par type de document (US-14-02) : le filtre s'applique avec la
  même réactivité que la recherche initiale.
- La recherche menée par le joueur sur le périmètre de documents qui lui sont visibles
  (UC-14 A4).
- Les deux contextes temporels : préparation (hors session) et session LIVE.

**Ce que cette exigence ne couvre pas :**

- La recherche dans le contenu textuel des blocs d'un document — cette capacité est
  explicitement hors périmètre MVP (US-UC-14, capacités exclues : la recherche porte sur
  le titre uniquement dans le MVP). La présente exigence porte sur la recherche par titre.
- La recherche cross-campagne — hors périmètre MVP (RB-14-01 : résultats limités à la
  campagne active).
- La navigation manuelle dans les panneaux de dossiers configurés — couverte par NFR-PERF-01.
- La continuité de l'interface pendant les actions de création ou de partage — couverte
  par NFR-PERF-03.

---

## Critères d'acceptation produit

**Situation nominale — recherche depuis la bibliothèque de contenu :**

Après la frappe, des résultats pertinents apparaissent avant toute relance manuelle par
l'utilisateur. Le MJ n'a pas à valider sa saisie pour obtenir des résultats : ceux-ci
se mettent à jour au fil de la frappe.

**Situation nominale — recherche depuis la vue session :**

Le MJ saisit un terme depuis la barre de recherche de la vue session. Des résultats
apparaissent avant que le MJ n'ait eu le temps de détourner le regard de la table. La
vue session reste active — les résultats s'affichent dans un panneau latéral sans
interrompre le contexte de session (RB-14-07).

**Situation nominale — résultats pondérés en session :**

Depuis la vue session, les documents liés à la session active (épinglés, notes de la
session en cours, documents du scénario associé) apparaissent en tête des résultats
(RB-14-08), sans délai supplémentaire par rapport aux autres résultats.

**Situation nominale — application d'un filtre par type :**

L'application du filtre par type de document sur des résultats déjà affichés est
immédiate — les résultats se recalculent sans phase d'attente visible.

**Situation limite — campagne dense en contenu :**

Même lorsque la campagne contient un grand nombre de documents de natures variées,
les premiers résultats pertinents apparaissent avec la même réactivité que pour une
campagne peu alimentée.

**Situation limite — aucun résultat :**

Lorsque la recherche ne retourne aucun résultat, l'état vide s'affiche immédiatement,
sans délai supplémentaire par rapport à une recherche qui retourne des résultats.

---

## Traçabilité montante

| Artefact | Lien |
|---|---|
| UC-14 — Rechercher rapidement une information | [../usecases/UC-14-recherche.md](../usecases/UC-14-recherche.md) |
| UC-06 — Vue session (A3 : recherche d'un élément non prévu en session) | [../usecases/UC-06-vue-session.md](../usecases/UC-06-vue-session.md) |
| US-UC-14 — Epic recherche (US-14-01, US-14-02, US-14-03) | [../user-stories/US-UC-14-recherche.md](../user-stories/US-UC-14-recherche.md) |
| RB-14-01 — Recherche limitée à la campagne active | [../user-stories/US-UC-14-recherche.md](../user-stories/US-UC-14-recherche.md) |
| RB-14-02, RB-14-03 — Règles de visibilité dans les résultats | [../user-stories/US-UC-14-recherche.md](../user-stories/US-UC-14-recherche.md) |
| RB-14-07 — Résultats dans un panneau latéral depuis la vue session | [../user-stories/US-UC-14-recherche.md](../user-stories/US-UC-14-recherche.md) |
| RB-14-08 — Pondération des éléments de la session active | [../user-stories/US-UC-14-recherche.md](../user-stories/US-UC-14-recherche.md) |
