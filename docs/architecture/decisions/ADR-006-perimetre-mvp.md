# ADR-006 — Périmètre MVP

- **Statut** : Accepté
- **Date** : 2026-06-09
- **Décideur** : opérateur (validation explicite, session d'audit/remédiation)
- **Findings liés** : CR-7, D-01, D-02, D-04, D-05, D-06, H-04

> **Nature : décision produit — fusionnée** — la substance de cette décision a été fusionnée dans la couche vision (`docs/conception/besoin/vision/moscow.md` et `vision-produit.md`) le 2026-06-10. Ce document est une trace historique ; le raisonnement et les alternatives écartées restent lisibles ici. *(Annotation du 2026-06-10 — arbitrage T-06, audit conception pure 2026-06.)*

---

## Contexte

L'audit a mis en évidence deux problèmes dans la définition du MVP.

**Deux hypothèses produit testées simultanément.** Le MVP couvrait à la fois la préparation (valeur pour le MJ seul) et la vue session (valeur en condition de jeu avec joueurs). Ces deux hypothèses sont indépendantes : l'une peut réussir sans l'autre, mais si l'adoption est faible, on ne peut pas distinguer lequel des deux piliers a échoué.

**Dépendance de priorité incohérente.** UC-08 et UC-09 (partage temps réel avec les joueurs — Must) dépendaient de UC-10 (compte cloud et synchronisation — qui était classé Should). Un Must ne peut pas dépendre d'un Should sans que cette dépendance soit explicitée et résolue.

---

## Décision

**MVP unique — pas de découpage en deux temps de livraison A/B.**

UC-10 (compte cloud, authentification, migration locale→cloud) est **promu Must**. Ce reclassement résout l'incohérence de priorisation : UC-08/09 peuvent désormais s'appuyer sur une dépendance de même niveau.

Le MVP livre dans un seul bloc : préparation, vue session, cloud, partage.

---

## Alternatives considérées

**MVP en deux temps de livraison — une première livraison (vue session solo, local-only) puis une seconde (cloud + partage).**
Recommandée par l'audit comme moyen d'isoler les hypothèses et de réduire le risque technique front-loadé. Écartée par l'opérateur, qui préfère livrer un produit complet d'emblée.

**Statu quo (priorités non cohérentes, UC-10 en Should).**
Non retenu : l'incohérence logique de priorisation rend la planification non fiable.

---

## Conséquences

- L'apprentissage produit n'est pas isolé : en cas d'adoption faible du MVP, il ne sera pas possible de distinguer l'échec de la préparation de celui de la vue session. Ce risque est assumé par l'opérateur.
- Le trio le plus risqué techniquement (synchronisation local↔cloud, migration, temps réel SignalR) est front-loadé dans le MVP. La migration n'est plus différable — elle devient un livrable du jalon J2.
- Les jalons de build se compressent : J1 (local-only), J2 (cloud + migration), J3 (partage + temps réel) doivent être enchaînés sans découplage produit entre eux.
- UC-13 (scénario réutilisable, one-shot) reste hors périmètre MVP au sens « first release » — la vision doit être corrigée pour ne pas le présenter comme un « contexte de premier ordre » dès le lancement.
- Une hypothèse explicite est à ajouter à la vision : « l'expérience joueur consultative (vue session invité) apporte suffisamment de valeur pour justifier l'adoption du groupe ». Cette hypothèse n'est pas formalisée dans la documentation actuelle (finding D-05).
- UC-10 étant promu Must, son estimation de complexité (finding H-04 : sous-estimé) doit être revisitée en J2.

---

## Compléments post-revue (2026-06-09)

Suite à la revue adversariale (revue de la phase de conception, artefact purgé du corpus — historique git), cette décision est complétée comme suit, sans changer sa direction.

- **Instrumentation compensatoire.** Puisque le MVP unique ne sépare pas les hypothèses d'apprentissage, une télémétrie séparée par pilier est ajoutée dans le périmètre MVP :
  - Activation préparation : campagne créée + N documents créés.
  - Activation vue session : session ouverte + usage réel constaté.
  - Activation partage : document partagé + au moins 1 joueur l'ayant ouvert.
  Cette instrumentation récupère l'essentiel de l'apprentissage que la décision de MVP unique assume de perdre.

- **Capture email non bloquante + analytics anonyme RGPD** dès le mode local (finding E-06, aggravé par le MVP unique local-first) — dans le périmètre MVP.

- **CR-1 (exécution du domaine en local, ADR-001) est un préalable bloquant** à la partie cloud/migration du MVP. Le jalon J2 ne peut pas commencer sans que J1 (local-only) soit stabilisé.

- **Gate de validation marché.** L'hypothèse marché (E-04 taille/solvabilité, E-05 GTM) n'est pas testée avant le build. Un gate de validation marché doit être posé avant d'engager le build lourd cloud + temps réel.

- **Contrepoids UC-10 promu Must.** Pour ne pas gonfler le périmètre Must, UC-05 « riche » (dossiers et types élaborés) est rétrogradé en Should (finding D-06).
