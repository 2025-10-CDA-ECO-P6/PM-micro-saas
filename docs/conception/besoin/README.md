# Besoin — Haversack

> Ce dossier rassemble tout ce qui exprime le besoin produit, dans l'ordre d'autorité que le
> corpus de conception applique partout : personas → vision → use cases → user journeys /
> user stories / NFR → parcours. Chaque sous-dossier est propriétaire de son propre index ;
> celui-ci pointe, il ne recopie pas.

---

## Ordre de lecture

| Sous-dossier | Rôle |
|---|---|
| [`persona/`](persona/README.md) | Profils utilisateurs de référence, utilisés pour valider les choix fonctionnels |
| [`vision/`](vision/README.md) | Positionnement produit, acteurs, choix assumés, priorisation MoSCoW du périmètre MVP |
| [`usecases/`](usecases/README.md) | Use cases MVP — source de vérité du besoin fonctionnel |
| [`user-stories/`](user-stories/README.md) | User stories dérivées des use cases, avec critères d'acceptation et règles métier |
| [`user-journeys/`](user-journeys/README.md) | User journeys dérivées des use cases, perspective émotionnelle et temporelle par persona |
| [`nfr/`](nfr/README.md) | Exigences non fonctionnelles, exprimées en langage besoin (indépendantes de l'implémentation) |
| [`parcours/`](parcours/README.md) | Couture transverse des use cases par persona — vérification des transitions inter-UC |

---

## Ce que chaque artefact est, et d'où il dérive

- **Personas** et **vision** sont premiers dans l'ordre d'autorité : ils cadrent qui est l'utilisateur et ce que le produit vise, avant toute description de fonctionnalité.
- Les **use cases** sont la source de vérité du besoin fonctionnel. Tout le reste de ce dossier en dérive.
- Les **user stories** et les **user journeys** dérivent chacune des use cases et s'y conforment — les premières en critères d'acceptation et règles métier, les secondes en parcours vécu par persona.
- Les **NFR** expriment les exigences non fonctionnelles (performance perçue, hors connexion, confidentialité, accessibilité, internationalisation) en langage besoin, indépendamment de tout choix d'implémentation.
- Les **parcours** ne sont pas une source supplémentaire : ils cousent les use cases bout en bout du point de vue d'un persona, en s'appuyant sur les user journeys qui restent la source de vérité par use case.

---

## Ce que ce dossier ne fait pas

- Il ne recompte ni ne reliste ce que chaque index de sous-dossier possède déjà (nombre de use cases, de personas, etc.) — chaque index propriétaire est l'unique source de ce décompte.
- Il n'arbitre aucun besoin — il oriente vers l'artefact qui le porte.

---

## Voir aussi

- [Glossaire](../glossaire.md) — outil de nommage dérivé, sans autorité de fond sur le besoin
- [Guide d'entretiens utilisateurs](../INTERVIEW_GUIDE.md)
- [Domaine DDD](../domain/README.md) — modélise la résolution du besoin, en aval
- [Interface](../interface/README.md) — traduit le besoin en surfaces décrites, en aval
