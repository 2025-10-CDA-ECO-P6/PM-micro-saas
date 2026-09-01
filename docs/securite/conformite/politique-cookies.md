# Politique de cookies et traceurs

> ## ⚠ BROUILLON — À VALIDER JURISTE, NON OPPOSABLE
>
> Ce document est un **projet de politique de cookies et traceurs**, rédigé à partir des décisions de conception déjà actées pour Haversack (ADR, spécifications techniques — en particulier la spécification de télémétrie). Il **n'a pas été validé par un juriste** et **n'est opposable à personne** en l'état. Il ne doit pas être publié ni présenté aux utilisateurs avant relecture et validation par un professionnel du droit.
>
> Ce document ne qualifie aucune exemption de consentement : chaque point qui relève d'une qualification légale est balisé `[À VALIDER — JURISTE]`. Chaque point que la conception technique ne fixe pas encore est balisé `[À COMPLÉTER]`. Ce document n'affirme l'existence d'aucun mécanisme technique (cookie, identifiant local, etc.) au-delà de ce que le corpus de conception acte explicitement.

---

## 1. Objet de ce document

Cette politique décrit les traceurs — cookies ou mécanismes équivalents (stockage local du navigateur, jetons techniques) — utilisés par Haversack, leur finalité, les données qu'ils portent et leur durée, dans la mesure où la conception technique du produit les fixe à ce jour.

Elle doit être lue **en complément** de la [politique de confidentialité](./politique-confidentialite.md), qui couvre l'ensemble des données personnelles traitées par Haversack au-delà du seul périmètre des traceurs. Les deux documents se renvoient l'un à l'autre chaque fois qu'un même sujet est concerné.

**Point de vigilance central de ce document** : la conception technique de Haversack ne fixe pas, à ce jour, le mécanisme exact de stockage ou de transport de plusieurs éléments techniques évoqués ci-dessous (jetons d'authentification, canal invité). Ce document ne présume pas qu'un cookie est utilisé lorsque le corpus ne le dit pas explicitement — chaque cas est balisé selon ce que la conception fixe réellement.

---

## 2. Le stockage local du navigateur en mode sans compte — ce n'est pas un traceur

**Point important à comprendre pour un utilisateur de Haversack** : en mode local sans compte, l'application stocke les données du Maître du Jeu directement dans le navigateur, via un mécanisme appelé IndexedDB. Ce stockage **n'est pas un traceur au sens publicitaire ou de mesure d'audience** — c'est le mécanisme de fonctionnement même du produit en mode local.

Ce qu'il faut retenir :

- En mode local, Haversack **n'a aucune authentification, aucun compte, aucun jeton — ni JWT d'accès, ni jeton de rafraîchissement, ni cookie de session** (ADR-017 §4.5, ADR-015 §Périmètre). Il n'existe donc, dans ce mode, **aucun cookie lié à l'authentification**.
- Les données stockées dans IndexedDB (campagnes, documents, notes) ne quittent le navigateur à aucun moment tant que le Maître du Jeu reste en mode local, sauf action explicite de sa part (création de compte, export manuel) — NFR-CONF-02.
- Ce stockage n'est **pas chiffré au repos** : c'est une limitation documentée du produit, distincte de la question des traceurs, décrite en détail dans la [politique de confidentialité §2.2](./politique-confidentialite.md#22-le-maître-du-jeu-en-mode-local-sans-compte).
- Aucun outil de mesure d'audience tiers n'intervient sur ce stockage local : si des mesures d'usage anonymes sont mises en place (voir [§4](#4-mesure-daudience--instrumentation-par-pilier)), elles font l'objet d'un traitement distinct, sans lien avec le contenu local du Maître du Jeu (NFR-CONF-02, portée et hors-portée).

En résumé : IndexedDB en mode local est un mécanisme de fonctionnement du produit, comparable à un fichier enregistré sur le disque de l'utilisateur — pas un traceur soumis à la réglementation cookies au sens où on l'entend pour un outil de mesure d'audience ou de publicité.

---

## 3. Traceurs strictement nécessaires — compte cloud et authentification

Cette section couvre les mécanismes techniques nécessaires au fonctionnement du service pour un utilisateur ayant créé un compte, ou pour un joueur invité rejoignant une session partagée.

### 3.1 Ce que la conception fixe

| Élément | Finalité | Donnée portée | Durée | Source |
|---|---|---|---|---|
| Jeton d'accès (access token) | Maintenir l'utilisateur authentifié sans lui redemander son mot de passe à chaque requête | Jeton signé (JWT), sans contenu narratif | ≤ 15 minutes, sans prolongation | ADR-015 §3.1 |
| Jeton de rafraîchissement (refresh token) | Renouveler le jeton d'accès sans nouvelle saisie du mot de passe | Jeton signé, identifiant de famille de jetons | ≤ 7 jours, borne absolue non glissante | ADR-015 §3.1, §3.3 |

**Ce que la conception ne fixe pas** : le mécanisme exact de stockage de ces jetons côté navigateur — cookie, stockage local du navigateur (`localStorage`), ou mémoire applicative — n'est **pas arrêté** dans le corpus de conception consulté. `[À COMPLÉTER]`. Si le mécanisme retenu à l'implémentation est un cookie, cette section devra être mise à jour pour en préciser le caractère (durée, portée, indicateurs `HttpOnly`/`Secure`/`SameSite`) avant toute publication de cette politique.

### 3.2 Canal temps réel invité — point non arbitré

Pour un joueur invité participant à une session en temps réel, un jeton d'accès invité (`GuestAccess`) doit être transmis au canal de communication temps réel. La conception acte que ce jeton **ne doit pas** être transmis dans l'URL de la requête (chaîne de requête), pour des raisons de sécurité, mais **ne tranche pas** entre les deux mécanismes envisagés pour cette transmission : un cookie de courte durée, ou un échange de jeton préalable à l'établissement de la connexion (ADR-004 §Compléments post-revue). `[À COMPLÉTER]` — ce point devra être précisé, y compris la durée exacte et le caractère du cookie s'il est retenu, avant toute publication de cette politique.

### 3.3 Ce que ces traceurs ne font pas

Aucun des éléments ci-dessus ne sert à suivre l'utilisateur à des fins de mesure d'audience ou de publicité — leur unique fonction est de maintenir une session authentifiée ou un accès de session, strictement nécessaire au fonctionnement du service demandé par l'utilisateur.

---

## 4. Mesure d'audience — instrumentation par pilier

Une instrumentation de mesure d'usage est prévue par la conception, décrite en détail dans la [spécification de télémétrie](../../architecture/specs/telemetrie.md). Cette section en reporte fidèlement le contenu et les points laissés ouverts — elle n'invente ni outil, ni mécanisme technique au-delà de ce que cette spécification fixe.

### 4.1 Ce qui est mesuré

L'instrumentation mesure l'activation de trois piliers d'usage, chacun défini par un critère composite à deux temps :

| Pilier | Critère d'activation |
|---|---|
| Préparation | Une campagne créée, et un nombre de documents créés à atteindre (valeur non fixée, `[À COMPLÉTER]`) |
| Vue de session | Une session ouverte, et un « usage réel constaté » (notion non opérationnalisée par la conception à ce jour, `[À COMPLÉTER]`) |
| Partage | Un document partagé, et au moins un joueur l'ayant ouvert |

### 4.2 Principe non négociable — occurrence, jamais contenu

Chaque événement mesuré est un **fait d'occurrence** (une campagne a été créée, une session a été ouverte, un document a été partagé) — **à aucun moment le contenu narratif** créé ou partagé par le Maître du Jeu (texte des documents, notes, contenu de session) n'est capté par cette instrumentation. Ce principe est posé explicitement au cahier des charges (§7.4) et à l'exigence d'isolation des données en mode local (NFR-CONF-02), et contraint toute résolution future des points encore ouverts ci-dessous.

### 4.3 Ce qui n'est pas encore fixé

| Point ouvert | Statut |
|---|---|
| Outil analytique utilisé pour la collecte | `[À COMPLÉTER]` — non nommé dans la conception |
| Nom exact et propriétés de chaque événement (schéma d'événements formalisé) | `[À COMPLÉTER]` — seuls les trois critères d'activation ci-dessus sont fixés |
| Technique d'anonymisation RGPD appliquée à ces événements | `[À COMPLÉTER]` — seule l'exigence de résultat (« anonyme RGPD ») est actée, pas le moyen technique |
| Mécanisme technique de mesure (cookie, identifiant généré côté client, ou autre) | `[À COMPLÉTER]` — non arrêté ; cette politique devra être mise à jour dès que ce choix sera fait |
| Valeur du seuil de documents créés (pilier préparation) | `[À COMPLÉTER]` |
| Opérationnalisation de « usage réel constaté » (pilier vue de session) | `[À COMPLÉTER]` |

**Conséquence pour cette politique** : tant que l'outil et le mécanisme technique de cette mesure d'audience ne sont pas arrêtés, cette section ne peut pas décrire de durée de conservation ni de caractère (cookie de mesure d'audience classique, identifiant côté serveur, etc.) pour ces traceurs. Elle devra être complétée dès que ces choix seront faits en implémentation.

### 4.4 Capture d'email non bloquante

La conception prévoit, dès le mode local, une capture d'adresse email **non bloquante** — le Maître du Jeu n'est pas empêché de progresser dans l'application s'il ne la fournit pas. Ni le point de sollicitation exact dans le parcours, ni le comportement en cas de refus, ni le traitement ultérieur de l'adresse collectée ne sont fixés par la conception à ce jour. `[À COMPLÉTER]` (spécification télémétrie §4).

---

## 5. Consentement

**Aucune exemption de consentement n'est affirmée par ce document.** La question de savoir quels traceurs, parmi ceux listés ci-dessus, peuvent bénéficier d'une exemption de consentement au titre de leur caractère strictement nécessaire au service demandé par l'utilisateur est une **qualification légale**, non tranchée en conception.

| Point | Statut |
|---|---|
| Exemption de consentement pour les jetons d'authentification ([§3](#3-traceurs-strictement-nécessaires--compte-cloud-et-authentification)) | `[À VALIDER — JURISTE]` |
| Exemption de consentement pour le jeton du canal temps réel invité | `[À VALIDER — JURISTE]` |
| Nécessité d'un recueil de consentement pour la mesure d'audience ([§4](#4-mesure-daudience--instrumentation-par-pilier)) | `[À VALIDER — JURISTE]` — dépend notamment de la technique d'anonymisation retenue, elle-même non fixée |
| Mécanisme de recueil et de gestion du consentement (bandeau, préférences) | `[À COMPLÉTER]` — aucun mécanisme de ce type n'est décrit dans le corpus de conception consulté |

---

## 6. Comment gérer vos préférences

`[À COMPLÉTER]` — aucun mécanisme de gestion des préférences de cookies (bandeau de consentement, panneau de préférences, retrait du consentement) n'est décrit dans la conception technique consultée à ce jour. Cette section devra être rédigée une fois ce mécanisme arrêté, en cohérence avec la qualification juridique du [§5](#5-consentement).

Pour le stockage local du navigateur en mode sans compte ([§2](#2-le-stockage-local-du-navigateur-en-mode-sans-compte--ce-nest-pas-un-traceur)), l'utilisateur garde la maîtrise complète de ses données : il peut les effacer à tout moment via les outils de son navigateur, sans passer par un mécanisme de préférences dédié à Haversack — puisqu'il s'agit de son propre stockage local, non transmis à un tiers.

---

## 7. Articulation avec la politique de confidentialité

Cette politique se limite aux traceurs et à leur mécanisme technique. Pour tout ce qui concerne les catégories de données personnelles traitées, leurs finalités, leur base légale, leurs destinataires, les durées de conservation autres que celles listées ci-dessus, et l'exercice de vos droits, se reporter à la [politique de confidentialité](./politique-confidentialite.md).

---

## Traçabilité — sources de ce document

| Source | Contenu utilisé |
|---|---|
| `docs/architecture/specs/telemetrie.md` | Piliers d'activation, principe de mesure d'occurrence sans contenu narratif, capture email non bloquante, points ouverts (outil, schéma d'événements, anonymisation) |
| `docs/architecture/decisions/ADR-015-securite-authentification-mvp.md` | Cycle de vie des jetons d'accès et de rafraîchissement, périmètre exclusivement cloud de l'authentification |
| `docs/architecture/decisions/ADR-017-modele-indexeddb-local.md` §4.5 | Absence de jeton, de compte et de cookie en mode local |
| `docs/architecture/decisions/ADR-004-transport-temps-reel.md` §Compléments post-revue | Transmission du jeton invité hors chaîne de requête, mécanisme non arbitré entre cookie et échange de jeton |
| `docs/conception/besoin/nfr/NFR-CONF-02-isolation-donnees-mode-local.md` | Portée de l'isolation des données locales, traitement distinct des mesures d'usage anonymes |
| `docs/context/cahier-des-charges.md` §7.4 | Principe de mesure d'occurrence sans contenu narratif |
| `docs/securite/conformite/politique-confidentialite.md` | Renvoi croisé, catégories de données et durées hors périmètre des traceurs |
