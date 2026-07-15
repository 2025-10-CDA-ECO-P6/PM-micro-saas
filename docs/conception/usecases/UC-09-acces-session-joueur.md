# UC-09 — Accès joueur via lien (session ou campagne)

## Acteur principal

Joueur

## Acteurs secondaires

MJ

## Objectif

Permettre à un joueur d'obtenir l'accès à une campagne ou à une session via un lien partagé par le MJ, avec ou sans compte : soit comme invité ponctuel pour une session (sans compte, friction minimale), soit comme membre permanent de la campagne (avec compte, accès persistant).

## Contexte

Le MJ choisit Haversack — pas le joueur. Si rejoindre la session impose au joueur de créer un compte, de confirmer un email et de configurer un profil, une partie du groupe ne passera pas cette étape. Lucas (22 ans, étudiant) représente ce profil : il a déjà D&D Beyond, Discord et un carnet. Un outil de plus n'est pas bienvenu. Si le lien d'invitation mène à une page d'inscription, il ferme l'onglet.

L'accès joueur sans compte est la condition d'adoption du groupe entier, ce qui détermine la rétention du MJ.

## Déclencheur

Le MJ partage un lien d'accès à une session (lien ponctuel) ou à sa campagne (lien permanent) avec ses joueurs.

## Préconditions

- Le MJ a un compte (au minimum gratuit) — le partage nécessite un backend.
- Pour le chemin session ponctuelle : le MJ a généré un lien de session ponctuel depuis la vue session ou le panneau membres (UC-11).
- Pour le chemin campagne permanent : le MJ a généré un lien de campagne depuis le panneau membres (UC-11).

## Scénario nominal A — Accès session ponctuel (sans compte)

1. Le MJ génère un lien de session depuis la vue session ou le panneau membres (UC-11).
2. Il partage le lien (Discord, WhatsApp, email, peu importe).
3. Le joueur clique sur le lien.
4. L'application affiche immédiatement les informations partagées par le MJ pour cette session.
5. Le joueur saisit uniquement un **nom d'affichage** (pas d'email, pas de mot de passe).
6. Il peut consulter les documents partagés, les informations révélées en temps réel pendant la session.
7. À la fin de la session, son accès expire.

## Scénario nominal B — Accès campagne permanent (joueur authentifié)

1. Le MJ génère un lien de campagne depuis le panneau membres (UC-11) et le partage avec le joueur.
2. Le joueur clique sur le lien.
3. S'il n'est pas encore connecté, l'application lui propose de se connecter ou de créer un compte (UC-10 pour la création de compte ; la jonction campagne se complète dans UC-09 une fois authentifié).
4. Le joueur est connecté à son compte Haversack.
5. L'application crée un `SpaceMembership` en `PENDING` puis l'active immédiatement via `Activate()` : le lien d'invitation campagne généré par le MJ (UC-11) tient lieu de validation au sens de RB-09-16 — c'est l'invitation préalable du MJ, pas un second clic, qui autorise l'activation. Le cycle de domaine `PENDING → ACTIVE` est respecté sans exception.
6. Le joueur obtient un accès persistant à la campagne : il peut consulter les sessions passées, les documents partagés, et son historique de personnage selon le périmètre du lien.
7. La vue obtenue une fois l'accès actif (fiche de campagne, documents `PUBLIC`, historique selon périmètre) relève d'**UC-12**.
8. L'accès persiste jusqu'à révocation par le MJ (UC-11).

## Scénarios alternatifs

### A1 — Joueur déjà connecté suivant un lien de session ponctuel

1. Le joueur clique sur le lien de session.
2. Il est déjà connecté à son compte Haversack.
3. Il accède directement aux informations partagées, avec son historique de session.

### A2 — Joueur qui veut créer un compte depuis l'accès invité

1. Le joueur a accédé comme invité à plusieurs sessions.
2. Il veut conserver ses notes personnelles entre sessions.
3. Il crée un compte depuis la page invité (UC-10) : ce point d'entrée mène à un **état contextualisé**, pas à un formulaire d'inscription générique — le contexte invité (session en cours, notes déjà prises) est préservé et présenté comme le motif de la création de compte, pour une migration sans perte perçue par le joueur. *(Réalisation d'écran : HAND-OFF présentation — `docs/presentation/**`.)*
4. Ses notes et accès existants sont migrés vers son compte : accès immédiat, sans attendre la validation de l'adresse de messagerie (cf. Règles métier).
5. Il peut ensuite rejoindre la campagne via un lien de campagne (scénario nominal B) ; son admission comme `Member` reste soumise au consentement du MJ (RB-09-16), indépendamment de la validation de son email.

### A3 — Lien expiré ou révoqué

1. Le joueur clique sur un lien de session expiré ou que le MJ a révoqué.
2. L'application affiche un message clair : "Ce lien n'est plus actif."
3. Elle invite le joueur à contacter le MJ pour un nouveau lien.

## Exceptions

### E1 — Lien invalide

Le lien est mal formé ou ne correspond à aucune session. L'application affiche une page d'erreur sobre, sans révéler si la campagne existe.

### E2 — Limite de joueurs du compte gratuit atteinte

Sur un compte `FREE`, une session est limitée à 4 joueurs distincts disposant d'un accès. Lorsqu'un 5e joueur tente d'accéder à la séance, son accès est refusé au moment de l'octroi. Le MJ est informé que la limite est atteinte et reçoit une invitation à passer `PRO` pour la lever. Le joueur surnuméraire voit un message sobre, sans information sur la campagne (même registre que le message de lien expiré).

## Postconditions

**Chemin A — session ponctuelle (sans compte)**
- Le joueur accède aux informations partagées sans avoir créé de compte.
- Son nom d'affichage est visible dans la session pour le MJ.
- Son accès expire à la fin de la session (pour les liens ponctuels).

**Chemin B — campagne permanent (avec compte)**
- Le `SpaceMembership` est actif (créé `PENDING` puis activé via `Activate()` à l'utilisation du lien d'invitation généré par le MJ).
- Le joueur dispose d'un accès persistant à la campagne jusqu'à révocation par le MJ.

## Règles métier

- Un lien de session ponctuel est valable le temps de la session + une fenêtre de grâce de **24 h ferme** (RB-09-01).
- Un lien de campagne permanent est valable jusqu'à révocation par le MJ.
- Le joueur invité (sans compte) a les mêmes droits fonctionnels qu'un joueur authentifié
  dans le périmètre de son lien : il peut voir les contenus publics ou partagés avec lui,
  consulter sa fiche si un personnage lui est associé, et créer des notes personnelles.
  La seule différence est technique : il n'a pas de compte persistant.
- La création d'un compte depuis l'accès invité migre l'accès sans perdre les notes déjà prises. Cette migration **est** une inscription (UC-10) : elle en hérite la règle d'accès sans en ajouter — **accès immédiat**, **validation de l'adresse de messagerie non bloquante** (RB-10-05, ADR-015 §2.1, invariant 7 I&A). Aucun gate spécifique à la migration ne conditionne cet accès à `emailVerified`. En revanche, l'accès **Member** permanent à la campagne reste distinct et gouverné par le **consentement du MJ** (RB-09-16) — la validation de l'email ne s'y substitue pas.
- Le MJ avec un compte gratuit peut inviter jusqu'à 4 joueurs par session. Le compte Pro lève cette limite.
- **Distinction de consentement** : suivre un lien d'invitation **campagne** généré par le MJ (UC-11) vaut validation — le `SpaceMembership` est créé `PENDING` puis activé (`Activate()`), l'invitation préalable du MJ tenant lieu de validation au sens de RB-09-16. En revanche, un joueur invité ne peut pas **s'auto-promouvoir** membre sans lien d'invitation campagne du MJ — une telle demande crée un `SpaceMembership` `PENDING` en attente de validation explicite du MJ (RB-09-16, parcours-03). La validation se fait côté MJ (UC-11).
- **Ownership `GuestAccess`** : UC-09 est le propriétaire des données et du cycle de vie des enregistrements `GuestAccess`. Leur création, expiration, révocation et conversion sont pilotées par les règles de cet UC ; les règles RGPD qui s'y appliquent (RB-09-18, RB-09-19) vivent ici par cohérence de responsabilité.
- RB-09-18 : À la fin définitive d'un `GuestAccess` (expiration après grâce ou révocation sans réactivation), les données personnelles qu'il porte (`displayName`, élément d'accès) cessent immédiatement d'être utilisées et affichées — plus aucune finalité produit. Leur effacement effectif intervient **au plus tard 90 jours** après la fin d'accès ; cette fenêtre bornée a pour seule finalité de permettre à l'invité d'exercer ses droits et de traiter une contestation, jamais un usage produit. ⚠️ Cette fenêtre de rétention de 90 jours porte **uniquement sur l'identifiant d'accès** (`displayName`, élément d'accès) — **les notes `PLAYER_PRIVATE` n'ont pas de fenêtre de rétention** : elles sont supprimées sans délai à la fin définitive de l'accès (RB-09-19). Si l'invité a été converti en compte, ses données suivent les règles du compte (RGPD Art. 5(1)(e) — limitation de la conservation).
- RB-09-19 : À la fin définitive d'un `GuestAccess` non converti, les notes personnelles **écrites par cet invité** (`PLAYER_PRIVATE`) sont supprimées physiquement — la clé de suppression est l'auteur, pas le personnage. Seules les notes dont cet invité est l'auteur sont concernées, jamais celles d'autres participants. La **fiche de personnage** associée survit intacte (elle appartient à la campagne, est ré-associable, et ne contient aucune note d'un autre joueur) (RGPD Art. 17 — droit à l'oubli, fondement identique à la suppression des notes à la suppression d'un compte).
- RB-09-20 : Au moment où l'invité saisit son nom d'affichage (avant ou à l'entrée en session), il est informé de manière simple de ce qui est conservé (son nom d'affichage, ses notes privées éventuelles), pour combien de temps (durée de l'accès + grâce), et du sort de ses données à la fin de l'accès : ses notes privées sont supprimées s'il n'a pas créé de compte, son nom d'affichage cesse d'être utilisé immédiatement et est effacé au plus tard 90 jours après la fin d'accès (RGPD Art. 13 — information de la personne concernée).
- RB-09-21 : Sur un compte `FREE`, une session est limitée à **4 joueurs distincts disposant d'un accès à la séance**, quel que soit le type d'accès (accès invité ponctuel, membre de campagne, ou autre type d'accès futur). Le MJ n'est jamais compté. La limite porte sur le nombre d'**accès distincts accordés** au moment de leur octroi — c'est le refus du **5e accès** qui bloque, pas une mesure de présence en temps réel. Compter un seul type d'accès rendrait la limite contournable et viderait le levier de l'offre supérieure. Le joueur dont l'accès est refusé voit un message sobre d'erreur, sans révélation sur la campagne (même registre que le message de lien expiré) ; le MJ reçoit le signalement de la limite atteinte avec une invitation à passer `PRO` pour la lever.
- RB-09-22 : Au-delà de l'information donnée à la saisie du nom (RB-09-20), l'invité est averti **en temps utile** — à un moment qui lui laisse la possibilité d'agir avant la fin de son accès — que ses notes personnelles ne seront conservées que s'il crée un compte avant cette fin. Cet avertissement traite le risque que l'information initiale ne soit plus présente à l'esprit lorsque l'accès prend fin, et laisse à l'invité le temps de décider de créer un compte pour conserver ses notes (UC-09 A2, RB-09-14).

## Critères d'acceptation

**Chemin A — session ponctuelle (sans compte)**
- Un joueur peut rejoindre une session en cliquant sur un lien, en saisissant uniquement un nom, sans inscription.
- Le joueur voit en temps réel les informations que le MJ partage pendant la session.
- Le lien expire correctement après la session.
- Le joueur peut créer un compte depuis la page invité sans perdre ses données de session.
- Un lien révoqué affiche un message clair sans révéler d'information sur la campagne.

**Chemin B — campagne permanent (avec compte)**
- Un joueur authentifié suivant un lien de campagne obtient un `SpaceMembership` actif dans la campagne (cycle `PENDING → Activate()` respecté ; l'invitation du MJ tient lieu de validation).
- L'accès est persistant et visible dans la gestion des membres (UC-11).
- Un joueur non connecté est invité à se connecter ou créer un compte avant que la jonction soit complétée.

## Questions à valider en interview

- Le nom d'affichage seul est-il suffisant, ou faut-il un identifiant léger (pseudo ou code) pour éviter les collisions ?
- Quelle est la durée de grâce acceptable après la fin de session pour un lien ponctuel ?
- Les joueurs veulent-ils recevoir un récapitulatif post-session sans avoir de compte ?
