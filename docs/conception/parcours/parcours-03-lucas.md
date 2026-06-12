# Parcours bout-en-bout — Lucas, joueur réfractaire qui rejoint sans compte

> Objet : vérifier les coutures inter-UC sur le parcours de Lucas, du clic sur le lien de session jusqu'à la fin de vie de son accès invité et, éventuellement, son adhésion durable à la campagne.
> Vocabulaire : glossaire Haversack 2026-06-10 (strict). Termes retenus : `GuestAccess`, `PLAYER_PRIVATE`, `LIVE_NOTE`, `CampaignMembership`, `Session`, `PUBLIC`, `GM_ONLY`, `displayName`, `GuestAccessScope`.
> → [Index des parcours](README.md)
> → [Fiche persona](../persona/persona-03-lucas.md)

---

## Présentation du parcours

Lucas, 22 ans, étudiant en informatique. Joueur depuis 3 ans, jamais MJ. Il a D&D Beyond pour sa fiche, Discord pour le groupe, un carnet pour ses notes. Son MJ lui envoie un lien "j'utilise un nouvel outil". Sa réaction par défaut est la résistance passive. Si la page lui demande de créer un compte, il ferme l'onglet. Si la page lui montre directement du contenu, il reste.

Le parcours de Lucas est celui du seul persona non-MJ du corpus. Il commence à la réception d'un lien et couvre :

1. L'accès sans compte via lien ponctuel — le moment d'adoption critique.
2. La consultation de ce que le MJ partage pendant la session.
3. La prise de notes personnelles pendant la session.
4. La fin de vie de l'accès invité — expiration, fenêtre de grâce, sort de ses notes.
5. La décision éventuelle de créer un compte pour rejoindre la campagne durablement.

Le parcours s'arrête à la fin de vie du `GuestAccess` ou à la création d'un compte et l'adhésion en tant que membre permanent.

---

## Étape 1 — Clic sur le lien, accès sans compte

**UC porteur :** [UC-09](../usecases/UC-09-acces-session-joueur.md) | **UJ porteur :** [UJ-UC-09](../user-journeys/UJ-UC-09-acces-session-joueur.md) | **US porteur :** [US-UC-09](../user-stories/US-UC-09-acces-session-joueur.md) — US-09-01

### Ce que Lucas cherche à faire

Cliquer sur le lien que son MJ lui a envoyé (Discord, WhatsApp, peu importe) et accéder directement à ce que son MJ a préparé. Il ne veut pas créer un compte.

### Précondition vérifiée

UC-09 précondition : "Le MJ a un compte (au minimum gratuit)" et "Le MJ a généré un lien de session ponctuel depuis la vue session ou le panneau membres (UC-06)." Ces préconditions portent sur le MJ, pas sur Lucas. Lucas n'a aucune précondition à satisfaire de son côté.

### Comportements observables

- Lucas clique sur le lien. L'application affiche immédiatement les informations partagées par le MJ pour cette session — pas une page de marketing, pas un formulaire d'inscription.
- Avant ou à l'entrée en session, Lucas reçoit une information simple sur les données conservées : son `displayName`, ses notes privées éventuelles, la durée de conservation (durée de l'accès + 24 heures de grâce), et leur suppression à la fin de l'accès. Cette information est donnée au moment de la saisie du nom (RB-09-20).
- Lucas saisit uniquement un `displayName` — pas d'email, pas de mot de passe. C'est le seul prérequis à l'accès (RB-09-02, US-09-01).
- Un `GuestAccess` de portée `SESSION` est créé : `status = ACTIVE`. Lucas accède à la vue session joueur.
- Son `displayName` est visible par le MJ dans la vue session.

### Ce que Lucas voit

- Les documents `PUBLIC` de la campagne.
- Les documents épinglés de la session (uniquement ceux avec `visibility = PUBLIC` — un document épinglé mais `GM_ONLY` n'est pas visible en vue joueur, même épinglé, RB-09-04 / US-09-01 notes de conception).
- Une zone de création de notes de session personnelles.
- Sa fiche de personnage si le MJ lui a associé un personnage (`PLAYER_CHARACTER`).

### Ce que Lucas ne voit pas

- Les notes de session `GM_ONLY` du MJ.
- Les notes de session `PLAYER_PRIVATE` des autres joueurs.
- Tout contenu que le MJ n'a pas explicitement partagé.

### État laissé par l'étape 1

- Un `GuestAccess` (`status = ACTIVE`, portée `SESSION`) existe, rattaché à Lucas et à cette session.
- Lucas est dans la vue session joueur pendant une session `LIVE`.

### Couture vers l'étape 2

La vue session joueur est accessible dès que le `GuestAccess` est `ACTIVE`. La vue est définie dans UC-06 scénario nominal (vue joueur) et UC-12 / UJ-UC-12. Précondition UC-12 : "Le joueur dispose d'un `GuestAccess` ou d'un `CampaignMembership` actif." Satisfaite via le `GuestAccess` créé à l'étape 1. **Couture continue.**

---

## Étape 2 — Consulter ce que le MJ partage pendant la session

**UC porteurs :** [UC-06](../usecases/UC-06-vue-session.md) (vue joueur), [UC-08](../usecases/UC-08-partager-information.md) (réception) | **UJ porteur :** [UJ-UC-12](../user-journeys/UJ-UC-12-rejoindre-campagne.md)

### Ce que Lucas cherche à faire

Consulter passivement les informations que son MJ partage en temps réel — indices, aides de jeu, révélations. Il n'a pas à faire quoi que ce soit d'actif pour les recevoir.

### Comportements observables

- Les documents `PUBLIC` de la campagne sont visibles dans la vue joueur. Ils y restent entre les sessions jusqu'à retrait explicite par le MJ (partage durable — UC-08 règle métier).
- Quand le MJ partage un document pendant la session `LIVE` (UC-08), le document passe à `visibility = PUBLIC`. Les joueurs ayant un `GuestAccess` actif voient immédiatement le document dans leur vue joueur (UC-06 scénario nominal vue joueur, UC-08 scénario nominal step 6).
- Si un personnage a été associé à Lucas par le MJ, sa fiche est visible directement dans la vue joueur (UJ-UC-12 flux fonctionnel).
- Lucas peut naviguer entre les documents partagés sans quitter la vue session.

### Limite du corpus (UC-08)

Le partage est par document, visible par l'ensemble des joueurs autorisés — pas de partage sélectif par joueur ou personnage dans le MVP (UC-08 règles métier, arbitrage 2026-06-10). Ce que Lucas voit est identique à ce que tous les autres joueurs voient.

### État laissé par l'étape 2

- Lucas consulte les documents `PUBLIC`. Aucune modification de son `GuestAccess`.
- Des documents sont potentiellement dans son espace de vue.

### Couture vers l'étape 3

Lucas peut prendre des notes personnelles à tout moment pendant une session `LIVE` (UC-06 règle métier : "Joueur : peut créer des notes de session uniquement pendant une session `LIVE`"). La précondition est que la session soit `LIVE` et que Lucas ait un `GuestAccess` actif. Les deux conditions sont satisfaites. **Couture continue.**

---

## Étape 3 — Prendre des notes personnelles pendant la session

**UC porteur :** [UC-06](../usecases/UC-06-vue-session.md) | **UJ porteur :** [UJ-UC-12](../user-journeys/UJ-UC-12-rejoindre-campagne.md), [UJ-UC-09](../user-journeys/UJ-UC-09-acces-session-joueur.md)

### Ce que Lucas cherche à faire

Consigner ses propres observations pendant la partie — décisions de son personnage, informations importantes, questions à poser au MJ plus tard. Ces notes ne doivent pas être visibles par le MJ ni par les autres joueurs.

### Comportements observables

- Dans la vue session joueur, Lucas dispose d'une zone de création de notes de session.
- Les notes créées par Lucas ont `visibility = PLAYER_PRIVATE` par défaut. Elles sont accessibles uniquement à leur auteur, identifié par `guestAccessId` (glossaire : `LIVE_NOTE`, champ `guestAccessId`).
- La confidentialité `PLAYER_PRIVATE` est absolue : ni le MJ (`OWNER`, `GM`), ni les autres joueurs ne peuvent lire ces notes — pas de lecture, pas d'énumération, pas d'accès aux métadonnées (glossaire `Visibility`).
- Les notes peuvent être liées à un personnage associé (`characterId`) si un personnage lui a été attribué par le MJ.
- Un joueur invité peut créer des notes de session personnelles si son `GuestAccess` est associé à un personnage (UC-06 règle métier : "Un joueur invité sans compte peut créer une note de session personnelle si son accès invité est associé à un personnage associé.")

### Vérification sur pièces — la note reste récupérable la séance suivante

UC-06 règle métier : "La note reste récupérable lors d'une séance suivante via un nouveau lien sécurisé associé au même personnage." Cela suppose que le `GuestAccess` suivant soit associé au même personnage. La mécanique de récupération inter-sessions n'est pas détaillée dans UC-06 ni dans UC-09 : comment le nouveau `GuestAccess` est-il rattaché aux notes de l'ancien ? Ce point est une zone muette (voir table des coutures, C5).

### État laissé par l'étape 3

- Des `LIVE_NOTE` avec `visibility = PLAYER_PRIVATE` et `guestAccessId = [identifiant du GuestAccess de Lucas]` ont été créées.
- La session est toujours `LIVE` ou passe en `CLOSED` quand le MJ la termine.

### Couture vers l'étape 4

La session passe en `CLOSED` quand le MJ clique "Terminer la session" (UC-06 phase 4). Le `GuestAccess` reste `ACTIVE` pendant la durée de la session + 24 heures de grâce (RB-09-01, UC-09 règle métier). La précondition de l'étape 4 (fin de vie du `GuestAccess`) est donc que la session soit `CLOSED` et que le délai de grâce s'écoule. **Couture continue.**

---

## Étape 4 — Fin de vie de l'accès invité : expiration, sort des notes

**UC porteur :** [UC-09](../usecases/UC-09-acces-session-joueur.md) | **US porteur :** [US-UC-09](../user-stories/US-UC-09-acces-session-joueur.md) — US-09-01 (RB-09-18, RB-09-19, RB-09-20), US-09-02

### Ce que Lucas vit à ce moment

Lucas ne fait rien d'actif. Son accès expire. La fenêtre de 24 heures après la fin de session s'écoule. Ce moment du parcours concerne ce qui se passe à ses données et à son accès sans qu'il le demande.

### Comportements observables — expiration normale

- Le `GuestAccess` passe de `ACTIVE` à `EXPIRED` à l'issue de la session + 24 heures de grâce (RB-09-01, RB-09-07).
- Si Lucas tente de rouvrir le lien après expiration, il voit un message sobre : "Ce lien n'est plus actif", avec une invitation à contacter le MJ (RB-09-09, US-09-02). Aucune information sur l'existence de la campagne n'est révélée.

### Sort des données à la fin définitive du GuestAccess (non converti en compte)

- **`displayName` et données d'accès** : à la fin définitive du `GuestAccess` (expiration après grâce ou révocation sans réactivation), le `displayName` de Lucas cesse d'être affiché et utilisé. Il est effacé au plus tard 90 jours après la fin d'accès — cette fenêtre bornée a pour seule finalité de permettre à Lucas d'exercer ses droits, jamais un usage produit (RB-09-18).
- **Notes `PLAYER_PRIVATE`** : les notes créées par Lucas sont supprimées physiquement à la fin définitive de son `GuestAccess` non converti. Seules ses notes sont concernées — jamais celles d'autres participants (RB-09-19).

### Information donnée à Lucas

Lucas a été informé de ces règles au moment de saisir son `displayName` (étape 1, RB-09-20) : ce qui est conservé (son nom d'affichage, ses notes privées éventuelles), pour combien de temps (durée de l'accès + 24 heures de grâce), et ce qui lui arrivera à la fin (ses notes privées sont supprimées s'il n'a pas créé de compte, son nom d'affichage n'est plus utilisé et effacé dans le délai borné).

### État laissé par l'étape 4

- `GuestAccess` = `EXPIRED`.
- Notes `PLAYER_PRIVATE` de Lucas supprimées physiquement.
- `displayName` en attente d'effacement dans la fenêtre de 90 jours.
- Le lien de session est inactif pour Lucas.

### Couture vers l'étape 5

L'étape 5 (rejoindre la campagne durablement) est conditionnée par Lucas ayant décidé de créer un compte **avant** l'expiration définitive de son `GuestAccess`. Si Lucas attend l'expiration, ses notes sont perdues (RB-09-19). La création de compte depuis l'accès invité (US-09-04) doit être déclenchée pendant que le `GuestAccess` est encore `ACTIVE`. Si Lucas décide de créer un compte après expiration, il peut toujours le faire mais sans récupérer ses notes. **Couture continue si Lucas agit pendant la fenêtre d'accès actif. Zone de tension temporelle documentée dans UJ-UC-09 (friction : "Friction lors de la création de compte depuis l'accès invité... si la procédure d'inscription est longue, il abandonne").**

---

## Étape 5 (optionnelle) — Créer un compte, rejoindre la campagne durablement

**UC porteurs :** [UC-09](../usecases/UC-09-acces-session-joueur.md) A2 (migration invité→compte) — octroi d'accès, [UC-12](../usecases/UC-12-rejoindre-campagne.md) — vue campagne joueur post-accès | **US porteur :** [US-UC-09](../user-stories/US-UC-09-acces-session-joueur.md) — US-09-04

### Ce que Lucas cherche à faire

Conserver ses notes d'une session à l'autre, accéder à l'historique de la campagne, ne plus avoir à saisir un nom à chaque session. Ce qui peut motiver Lucas à faire cet effort : voir la valeur des notes personnelles sur plusieurs sessions, ou constater que l'outil lui est utile plus que D&D Beyond pour ce qu'il fait avec son groupe.

### Précondition de l'accès à la vue joueur (UC-12) vérifiée

La vue joueur (UC-12) présuppose que Lucas dispose d'un `CampaignMembership` actif ou d'un `GuestAccess` actif — l'octroi lui-même (création du `CampaignMembership`) est opéré via UC-09 (côté joueur, migration invité→compte) et UC-11 (côté MJ, validation du membre). Lucas n'a pas encore de compte ici. La création de compte est possible directement depuis la vue invité (US-09-04). La précondition de la vue joueur est satisfaite après cette création et validation MJ.

### Comportements observables

- Depuis la vue invité, Lucas clique sur "Créer un compte" (US-09-04 scénario nominal).
- Il crée son compte via UC-10. Son `GuestAccess` existant est rattaché au nouveau compte — l'historique d'accès est préservé.
- Ses notes `PLAYER_PRIVATE` prises en mode invité sont migrées vers son compte (RB-09-14, US-09-04 règle métier). Aucune note n'est perdue.
- La migration est irréversible — Lucas ne peut pas repasser en mode sans compte sur ce compte (RB-09-17).
- Le MJ est notifié qu'un joueur invité a créé un compte (US-09-04 critères d'acceptation). Il peut proposer à Lucas de le promouvoir en membre permanent de la campagne (`CampaignMembership`). Cette promotion nécessite la validation du MJ (RB-09-16).

### Ce que Lucas obtient avec un compte

- Accès à l'historique des sessions et aux documents de lore `PUBLIC` entre les séances (UC-12 postconditions, US-09-03 règle RB-09-13).
- Ses notes `PLAYER_PRIVATE` conservées d'une session à l'autre.
- Accès sans saisir de nom à chaque session (UC-09 A1 : "Le joueur est déjà connecté à son compte Haversack — accès direct avec historique de compte").
- Via UC-12 (vue joueur, périmètre `CAMPAIGN`) : accès au lore complet partagé, pas seulement aux documents de la session en cours.

### Ce que Lucas n'obtient pas avec un simple compte

L'accès à l'historique complet de la campagne (lore, sessions précédentes) nécessite un lien de campagne permanent (`GuestAccessScope = CAMPAIGN` ou `CampaignMembership`) généré par le MJ via UC-11 — pas simplement d'avoir un compte. UC-12 décrit la vue obtenue une fois ce lien ou ce membership actif ; l'entrée dans ce périmètre passe par UC-09 (octroi d'accès) et UC-11 (lien généré par le MJ), pas par la simple création de compte (US-09-03 notes de conception : "La génération du lien permanent est couverte par UC-11").

### État laissé par l'étape 5

- Lucas est `User` authentifié avec `AccountStatus = ACTIVE`.
- Son `GuestAccess` est `CONVERTED`.
- Il peut être proposé comme `CampaignMembership` si le MJ le valide.

---

## Points de friction propres à Lucas

Les points suivants sont fondés sur le corpus lu (UC-09, UC-12, UJ-UC-09, UJ-UC-12, persona).

**1. La première page est le test d'adoption**
La persona est explicite : si la page demande de créer un compte, Lucas ferme l'onglet. UC-09 et US-09-01 répondent directement à cette friction : la saisie du `displayName` seul, sans email ni mot de passe, est la seule étape. Le corpus adresse ce point.

**2. Lucas ne sait pas que son accès expire dans 24 heures**
UJ-UC-09 identifie cette friction : "Lucas ne sait pas que son accès expire 24 heures après la fin de session. Si le MJ clôture la session tard dans la nuit, Lucas peut perdre l'accès à ses notes avant de les relire — sans avoir été averti." RB-09-20 oblige à informer Lucas à l'entrée, mais l'information donnée au moment de la saisie du nom (avant la session) peut ne pas être mémorisée six heures plus tard. UJ-UC-09 propose un indicateur d'expiration visible dans la vue joueur — non prescrit comme règle métier, seulement proposé comme opportunité UX.

**3. La motivation de créer un compte reste faible pour Lucas**
La persona est explicite : "Pourquoi créerait-il un compte plutôt que de rester invité ? Qu'est-ce qu'il y gagne ?" Ce que le corpus documente comme gains (historique entre sessions, notes conservées, accès sans saisir de nom) sont des gains réels mais non visibles sans avoir vécu plusieurs sessions en invité. Le corpus identifie ce problème (persona, UJ-UC-09 friction "bandeau de migration non intrusif" comme opportunité UX) mais ne le résout pas en règle métier prescriptive. La valeur doit être perçue par expérience.

**4. La coexistence avec D&D Beyond reste non résolue**
La persona le nomme explicitement : "La coexistence avec D&D Beyond n'est pas résolue non plus : si les joueurs ont déjà leur fiche ailleurs, le module personnage de Haversack est redondant." Le corpus ne répond pas à ce point. Il n'y a pas de module d'import de fiche depuis D&D Beyond dans le périmètre MVP (vision-produit.md §2.4 : "Inventaire personnage — Compète avec D&D Beyond sans pouvoir l'égaler" est listé comme hors périmètre). Ce n'est pas une rupture de parcours, c'est un positionnement produit assumé.

**5. Récupération des notes inter-sessions via le même personnage : mécanique non spécifiée**
UC-06 règle métier dit que la note d'un invité "reste récupérable lors d'une séance suivante via un nouveau lien sécurisé associé au même personnage." Mais la mécanique concrète de cette récupération (comment le nouveau `GuestAccess` est associé aux notes de l'ancien via le personnage ?) n'est pas spécifiée dans UC-06, UC-09 ni dans les user stories. Voir table des coutures C5.

---

## Table récapitulative des coutures

| # | Couture | Étapes | Statut | Détail |
|---|---|---|---|---|
| C1 | Lien valide → accès immédiat sans compte | 1 | **Continue** | UC-09 scénario nominal : token `GuestAccess` valide → saisie du `displayName` seul → `GuestAccess` actif. RB-09-02 : "La saisie d'un nom d'affichage est obligatoire pour activer le `GuestAccess`. L'email et le mot de passe ne sont pas requis." Précondition satisfaite (MJ a un compte, lien généré depuis UC-06). |
| C2 | GuestAccess actif → vue session joueur avec documents PUBLIC | 1 → 2 | **Continue** | UC-06 scénario nominal vue joueur : "Le joueur accède à la campagne pendant une session `LIVE`. Le système lui présente une vue joueur simplifiée. Cette vue affiche les documents partagés par le MJ (`visibility = visible par les joueurs`) [...]." US-09-01 RB-09-04 : "Le joueur invité dispose des mêmes droits fonctionnels qu'un joueur avec compte dans le périmètre de son `GuestAccess`." |
| C3 | Vue session joueur → création de notes PLAYER_PRIVATE | 2 → 3 | **Continue** | UC-06 règle métier : "Joueur : peut créer des notes de session uniquement pendant une session `LIVE`. Visibilité par défaut : personnelle joueur." RB-09-04 : les notes personnelles sont dans le périmètre fonctionnel d'un invité. La note est rattachée à `guestAccessId`. |
| C4 | Session CLOSED → expiration du GuestAccess après grâce | 3 → 4 | **Continue** | UC-09 règle métier : "Un lien de session ponctuel est valable le temps de la session + une fenêtre de grâce (ex. : 24 h)." RB-09-01 confirme. UC-09 A3 et US-09-02 couvrent le cas du lien expiré côté Lucas. RB-09-18 et RB-09-19 couvrent le sort des données. |
| C5 | Notes PLAYER_PRIVATE d'une session → récupération la séance suivante via personnage | 3 → session suivante | **Continue (documentée)** | La fiche de personnage (`PLAYER_CHARACTER`) de Lucas reste ré-associable lors d'une séance suivante — ce qui lui permet de retrouver le contexte de son personnage dans la campagne (UC-06, UC-11 corrigés, décision issue du modèle de domaine). Ses notes personnelles (`PLAYER_PRIVATE`) créées en mode invité ne survivent à la fin de son accès que s'il crée un compte avant cette fin (US-09-04, RB-09-14, RB-09-19) — la récupération inter-sessions via un nouveau `GuestAccess` pur (sans compte) n'est pas possible, car la confidentialité `PLAYER_PRIVATE` est indexée par `guestAccessId` qui change à chaque accès, et le modèle de domaine ne prévoit pas de chemin de lecture cross-`GuestAccess`. Cette limite de périmètre assumée est documentée dans UC-06 et UC-11, et Lucas en est averti en temps utile pour agir avant la fin de son accès (RB-09-22). La levée de cette limite (identité invité persistante inter-sessions) relève d'un changement de positionnement produit traité au plan, pas du périmètre MVP. |
| C6 | GuestAccess actif → création de compte et migration sans perte | 4 → 5 | **Continue (fenêtre temporelle contrainte)** | US-09-04 couvre la migration invité→compte avec préservation des notes (RB-09-14). Condition : Lucas doit déclencher la migration pendant que son `GuestAccess` est encore `ACTIVE`. Si le `GuestAccess` est expiré, les notes ont déjà été supprimées (RB-09-19) — la migration ne peut plus les récupérer. Cette contrainte temporelle est une friction identifiée dans UJ-UC-09 mais n'est pas une rupture si Lucas agit dans la fenêtre. |
| C7 | Compte créé → membre permanent de la campagne via MJ | 5 | **Continue (validation MJ requise)** | L'octroi du `CampaignMembership` relève d'UC-09 (côté joueur, migration invité→compte) et d'UC-11 (côté MJ, validation). La précondition de la vue joueur (UC-12) — disposer d'un `CampaignMembership` actif — est satisfaite après cette validation. RB-09-16 : "La promotion en `Member` permanent nécessite la validation du MJ." Lucas ne peut pas s'auto-promouvoir. La promotion est à l'initiative du MJ après notification (US-09-04). Cette dépendance est documentée dans le corpus et n'est pas une rupture. |
| C8 | Accès SESSION → accès à l'historique complet de la campagne | 1 ou 5 | **Continue (documentée)** | Avec un `GuestAccess` de portée `SESSION`, Lucas n'a pas accès à l'historique des sessions précédentes ni au lore complet de la campagne (UJ-UC-12 flux : "Documents épinglés + PUBLIC / Pas d'historique campagne / Pas de lore complet"). L'accès à l'historique complet nécessite un `GuestAccess` de portée `CAMPAIGN` ou un `CampaignMembership` — les deux nécessitent un compte et un lien permanent généré par le MJ via UC-11. **Spécifié** : le périmètre `SESSION` vs `CAMPAIGN` est une décision MJ au moment de générer l'invitation (UC-11 scénario nominal, US-11-01). Le comportement résultant côté joueur est tracé (UC-09 Scénario nominal B, accès historique/lore via lien permanent ou compte). **Limite assumée** : la demande par Lucas d'un accès campagne via l'interface est hors-bande (Lucas demande socialement au MJ). Le seul signal in-app joueur→MJ existant est la notification de création de compte (US-09-04). Si le produit voulait un canal in-app de "demande d'accès campagne" initié par le joueur, ce serait une fonctionnalité NOUVELLE, hors de cette couture. **Localisation : UC-09 Scénario nominal B, UC-11 scénario nominal, US-11-01, UJ-UC-09 flux fonctionnel nœud "Valide — permanent."** |
