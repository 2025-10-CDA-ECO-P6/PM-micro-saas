# Registre ADR — Haversack

Ce registre consigne les décisions d'architecture retenues pour le projet Haversack.
Il repart à neuf après plusieurs remises à zéro de la conception (voir note ci-dessous).

Retour vers l'index d'architecture : [architecture/README.md](../README.md)

---

## Note sur la numérotation

Les fichiers ADR précédents ont été supprimés lors des remises à zéro successives de la conception.
Le présent registre repart à **ADR-001**. L'unique décision antérieure qui avait survécu
(FK `ownerId`, anciennement référencée sous « ADR-12 » ou « CRIT-04 ») est refformalisée ici
sous **ADR-009**.

---

## Index

| Numéro | Titre | Nature | Statut | Date |
|--------|-------|--------|--------|------|
| [ADR-001](ADR-001-execution-domaine-mode-local.md) | Exécution du domaine en mode local | conception | Accepté | 2026-06-09 |
| [ADR-002](ADR-002-tout-est-document-gouvernance.md) | « Tout est Document » strict et gouvernance des identifiants et de `properties` | conception | Accepté | 2026-06-09 |
| [ADR-003](ADR-003-stack-front.md) | Stack front : mono-écosystème Angular | pré-implémentation | Accepté | 2026-06-09 |
| [ADR-004](ADR-004-transport-temps-reel.md) | Transport temps réel : SignalR | pré-implémentation | Accepté | 2026-06-09 |
| [ADR-005](ADR-005-modele-monetisation.md) | Modèle de monétisation | produit — fusionné | Accepté | 2026-06-09 |
| [ADR-006](ADR-006-perimetre-mvp.md) | Périmètre MVP | produit — fusionné | Accepté | 2026-06-09 |
| [ADR-007](ADR-007-rgpd-autorisation-api.md) | Conformité RGPD et modèle d'autorisation API | conception | Accepté | 2026-06-09 |
| [ADR-008](ADR-008-structure-solution.md) | Structure physique de la solution | pré-implémentation | Accepté | 2026-06-09 |
| [ADR-009](ADR-009-fk-campaign-owner.md) | FK CAMPAIGN.ownerId → USER | conception | Accepté | 2026-06-09 |
| [ADR-010](ADR-010-suppression-campagne.md) | Suppression de campagne (soft-delete + purge + saga) | conception | Accepté | 2026-06-09 |
| [ADR-011](ADR-011-cascade-integrite-referentielle.md) | Cascade & intégrité référentielle (sagas `SpaceDeleted` et `UserAnonymized`) | pré-implémentation | Accepté | 2026-06-09 |
| [ADR-012](ADR-012-rgpd-effacement-compte.md) | RGPD : effacement de compte (Art. 17 — droit à l'oubli) | conception | Accepté | 2026-06-09 |
| [ADR-013](ADR-013-rgpd-donnees-invites.md) | RGPD : données des joueurs invités (GuestAccess, Art. 6/13) | conception | Accepté | 2026-06-09 |
| [ADR-014](ADR-014-modele-autorisation-api.md) | Modèle d'autorisation API (appartenance ressource↔campagne) | conception | Accepté | 2026-06-09 |
| [ADR-015](ADR-015-securite-authentification-mvp.md) | Sécurité authentification MVP (politique mdp, tokens, OAuth, rate limiting) | pré-implémentation | Accepté | 2026-06-10 |
| [ADR-016](ADR-016-serialisation-locale-migration.md) | Sérialisation locale et contrat de migration local→cloud (format payload, frontière de confiance, parcours d'échec) | mixte (dominante pré-implémentation) | Accepté | 2026-06-10 |
| [ADR-017](ADR-017-modele-indexeddb-local.md) | Modèle IndexedDB local et sécurité du mode local (object stores, posture migration-only, persist(), F-09) | mixte (dominante pré-implémentation) | Accepté | 2026-06-10 |
| [ADR-018](ADR-018-espace-personnel-generalisation-space.md) | Contenu personnel de premier ordre : généralisation de `Campaign` en `Space` (`SpaceType.PERSONAL`) | conception | Accepté | 2026-06-12 |

---

## Légende des statuts

| Statut | Signification |
|--------|---------------|
| **Accepté** | Décision validée et appliquée à la conception courante |
| **Proposé** | Décision soumise, en attente de validation par l'opérateur |
| **Remplacé** | Décision remplacée par un ADR ultérieur (lien vers le successeur) |
| **Déprécié** | Décision obsolète sans successeur direct |

---

## Légende des natures

| Nature | Signification |
|--------|---------------|
| **conception** | Décision qui contraint le modèle métier/données de la conception courante |
| **pré-implémentation** | Décision d'architecture actée en phase conception, à confirmer à l'entrée en build |
| **mixte (dominante pré-implémentation)** | Décision combinant des parts de conception (nommées dans le bandeau de l'ADR) et des parts pré-implémentation à confirmer à l'entrée en build |
| **produit — fusionné** | Substance produit fusionnée dans la couche vision ; l'ADR est une trace historique, le raisonnement reste lisible |
