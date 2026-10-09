# Matrice de priorités

> **MoSCoW** = Must Have · Should Have · Could Have · Won't Have (this release)
>
> Source de vérité pour les arbitrages produit. Révisée sur la base de l'analyse des 7 personas
> (Thomas, Émilie, Lucas, Nadia, Antoine, Rémi, Sonia).

**Sommaire**

- [Vue d'ensemble](#vue-densemble)
- [Must Have](#must-have)
  - [UC-01 — Mode local sans compte](#uc-01--mode-local-sans-compte)
  - [UC-02 — Créer et configurer un espace (campagne ou one-shot)](#uc-02--créer-et-configurer-un-espace-campagne-ou-one-shot)
  - [UC-03 — Structurer un scénario](#uc-03--structurer-un-scénario)
  - [UC-04 — Gérer les documents d'un espace](#uc-04--gérer-les-documents-dun-espace)
  - [UC-05 base — Organiser le contenu en dossiers (Must Have)](#uc-05-base--organiser-le-contenu-en-dossiers-must-have)
  - [UC-06 — Utiliser la vue session](#uc-06--utiliser-la-vue-session)
  - [UC-07 — Créer un élément à la volée en session](#uc-07--créer-un-élément-à-la-volée-en-session)
  - [UC-08 — Partager une information aux joueurs](#uc-08--partager-une-information-aux-joueurs)
  - [UC-09 — Accès joueur sans compte](#uc-09--accès-joueur-sans-compte)
  - [UC-10 — Créer un compte et synchroniser dans le cloud](#uc-10--créer-un-compte-et-synchroniser-dans-le-cloud)
  - [Espace personnel — conteneur par défaut et capture-first (Must Have — ADR-018)](#espace-personnel--conteneur-par-défaut-et-capture-first-must-have--adr-018)
  - [Export d'espace (Must Have — version minimale)](#export-despace-must-have--version-minimale)
  - [Instrumentation de validation du MVP](#instrumentation-de-validation-du-mvp)
- [Should Have — MVP significativement plus faible sans eux](#should-have--mvp-significativement-plus-faible-sans-eux)
  - [UC-05 riche — Dossiers et types de document élaborés (Should Have)](#uc-05-riche--dossiers-et-types-de-document-élaborés-should-have)
  - [UC-11 — Gérer les membres d'un espace](#uc-11--gérer-les-membres-dun-espace)
  - [UC-12 — Consulter son espace en tant que joueur (vue post-accès)](#uc-12--consulter-son-espace-en-tant-que-joueur-vue-post-accès)
  - [UC-13 — Utiliser un scénario réutilisable](#uc-13--utiliser-un-scénario-réutilisable)
  - [UC-14 — Rechercher et filtrer l'information](#uc-14--rechercher-et-filtrer-linformation)
- [UC-15 — Classement arbitré : Post-MVP (spécifiés)](#uc-15--classement-arbitré--post-mvp-spécifiés)
- [Variante one-shot express (UC-02 §A1) — Post-MVP (spécifiés)](#variante-one-shot-express-uc-02-a1--post-mvp-spécifiés)
- [Could Have — Valeur réelle, non prioritaire pour la validation initiale](#could-have--valeur-réelle-non-prioritaire-pour-la-validation-initiale)
  - [Types de document personnalisés](#types-de-document-personnalisés)
  - [Réimport de fichier de sauvegarde *(post-MVP — distinct de l'export Must Have)*](#réimport-de-fichier-de-sauvegarde-post-mvp--distinct-de-lexport-must-have)
  - [Notes personnelles joueur](#notes-personnelles-joueur)
  - [Persistance du personnage actif entre sessions (UC-12)](#persistance-du-personnage-actif-entre-sessions-uc-12)
  - [Notes libres du joueur, sans personnage associé (UC-12)](#notes-libres-du-joueur-sans-personnage-associé-uc-12)
  - [Synchronisation source → instance d'un scénario réutilisable (UC-13)](#synchronisation-source--instance-dun-scénario-réutilisable-uc-13)
  - [Journal d'activité des membres (UC-11)](#journal-dactivité-des-membres-uc-11)
  - [Notification du joueur au partage d'un document (UC-12)](#notification-du-joueur-au-partage-dun-document-uc-12)
  - [Persistance des notes d'un invité d'une session à l'autre (UC-12)](#persistance-des-notes-dun-invité-dune-session-à-lautre-uc-12)
  - [Notification au MJ à l'utilisation d'une invitation (UC-11)](#notification-au-mj-à-lutilisation-dune-invitation-uc-11)
  - [Récapitulatif post-session par email à l'invité (UC-09)](#récapitulatif-post-session-par-email-à-linvité-uc-09)
  - [Template de vue session (UC-06)](#template-de-vue-session-uc-06)
  - [Historique des partages (UC-08)](#historique-des-partages-uc-08)
  - [Historique des recherches récentes (UC-14)](#historique-des-recherches-récentes-uc-14)
  - [Filtrage par tag en recherche (UC-14, A3)](#filtrage-par-tag-en-recherche-uc-14-a3)
- [Won't Have — Exclus par choix stratégique](#wont-have--exclus-par-choix-stratégique)
  - [Inventaire personnage](#inventaire-personnage)
  - [Clôture formelle de session](#clôture-formelle-de-session)
  - [Documents de lore en use case distinct](#documents-de-lore-en-use-case-distinct)
  - [Rôle GM — co-MJ (UC-F08)](#rôle-gm--co-mj-uc-f08)
  - [Relations entre documents typés (UC-F06 — phase 1)](#relations-entre-documents-typés-uc-f06--phase-1)
  - [Moteur de règles / émulation système (UC-F06 — phase 2)](#moteur-de-règles--émulation-système-uc-f06--phase-2)
  - [Assistant IA](#assistant-ia)
  - [Table visuelle](#table-visuelle)
  - [Application desktop avec synchronisation CRDT](#application-desktop-avec-synchronisation-crdt)
  - [Templates communautaires](#templates-communautaires)
- [Dépendances](#dépendances)

---

## Vue d'ensemble

| Catégorie | Nb | Éléments |
|---|---|---|
| **Must Have** | 10 UC + espace personnel + instrumentation + export d'espace | UC-01 à UC-10 (dont UC-05 base — dossiers libres) + espace personnel (capture-first, conteneur par défaut) + instrumentation de validation du MVP + export d'espace (version minimale — promotion du 2026-06-25) |
| **Should Have** | 5 | UC-05 riche (types de document élaborés et templates par défaut des dossiers), UC-11, UC-12, UC-14, UC-13 *(hors première livraison)* |
| **Could Have** | 15 | Types personnalisés, réimport de fichier de sauvegarde *(post-MVP ; distinct de l'export Must Have)*, notes de session du joueur, persistance du personnage actif entre sessions (UC-12), notes libres du joueur sans personnage (UC-12), synchronisation source → instance d'un scénario réutilisable (UC-13), journal d'activité des membres (UC-11), notification du joueur au partage d'un document (UC-12), persistance des notes d'un invité d'une session à l'autre (UC-12), notification au MJ à l'utilisation d'une invitation (UC-11), récapitulatif post-session par email à l'invité (UC-09), template de vue session (UC-06), historique des partages (UC-08), historique des recherches récentes (UC-14), filtrage par tag en recherche (UC-14, A3) |
| **Won't Have** | — | Voir détail ci-dessous |
| **Post-MVP (spécifiés)** | 2 | UC-15 *(gel d'espaces au downgrade de tier — hors catégorisation MoSCoW du MVP, arbitrage du 2026-09-01, voir §UC-15 ci-dessous)*, variante one-shot express d'UC-02 *(UC-02 §A1, voir §Variante one-shot express ci-dessous)* |

---

## Must Have

Le produit ne peut pas être validé sans eux.

Ces dix use cases, complétés par l'espace personnel, forment le périmètre minimal cohérent :
sans l'un d'eux, soit le produit ne peut pas être utilisé, soit l'hypothèse centrale ne peut
pas être testée.

### UC-01 — Mode local sans compte

**Pourquoi Must Have** : forcer la création de compte avant toute valeur perçue est une friction
rédhibitoire. Nadia a abandonné Notion pour cette raison. Rémi ne créera pas de compte sans raison
concrète. Thomas ne migrera pas d'Obsidian — mais il peut évaluer localement.

Le mode local est aussi la fondation du modèle de monétisation non-agressif :
local gratuit → compte gratuit (cloud + partage) → Pro (illimité).
La valeur est perçue avant l'engagement financier.

**Critère de sortie** : un MJ crée un espace et prépare du contenu sans s'inscrire.
Ses données persistent entre sessions navigateur.

**Risque si absent** : friction d'onboarding maximale. Perte d'utilisateurs avant toute
perception de valeur.

---

### UC-02 — Créer et configurer un espace (campagne ou one-shot)

**Pourquoi Must Have** : un espace de type `CAMPAIGN` ou `ONE_SHOT` est le conteneur structurant
pour une table et ses membres.
La définition doit rester large : conteneur léger (Thomas), one-shot isolé (Sonia via UC-13),
campagne narrative longue (Antoine). Configuration opérationnelle en moins de deux minutes.

L'espace personnel (`PERSONAL`) est créé automatiquement à l'initialisation du compte — il
n'est pas décompté du quota d'espaces `CAMPAIGN`/`ONE_SHOT` (voir « Quota FREE » ci-dessous).
UC-02 couvre la création d'espaces partagés ; l'espace personnel est provisonné par le système.

**Critère de sortie** : un MJ crée un espace nommé (type `CAMPAIGN` ou `ONE_SHOT`) et accède à
son espace de travail. Fonctionne en mode local.

**Risque si absent** : aucune structuration de contenu collaboratif possible.

---

### UC-03 — Structurer un scénario

**Pourquoi Must Have** : l'hypothèse produit principale à valider en premier.
Le système de blocs génériques couvre les deux extrêmes : cinq bullet points (Émilie)
et scénario détaillé avec scènes liées (Antoine). Aucune structure D&D-centrique imposée.

**Critère de sortie** : un MJ crée un scénario avec des scènes et sauvegarde.
Fonctionne en mode local.

**Risque si absent** : impossible de valider la proposition de valeur principale.

---

### UC-04 — Gérer les documents d'un espace

**Pourquoi Must Have** : les documents sont le filet de sécurité du MJ — préparation légère,
capture d'urgence en session, consolidation post-session. Un MJ sans possibilité de capturer
et retrouver ses informations n'utilisera pas l'outil. La capture en session doit être
quasi-immédiate. UC-04 couvre l'ensemble des documents libres, des entrées rapides et du
lore, sans restreindre le concept à la prise de notes.

Depuis l'introduction de l'espace personnel (ADR-018), un document peut naître dans l'espace
`PERSONAL` sans qu'un espace `CAMPAIGN` ou `ONE_SHOT` soit nécessaire — UC-04 s'applique à
tout type d'espace.

**Critère de sortie** : un MJ crée un document, le retrouve et le modifie.
Fonctionne en mode local.

**Risque si absent** : l'outil ne remplace aucun des supports actuels du MJ.

---

### UC-05 base — Organiser le contenu en dossiers (Must Have)

**Pourquoi Must Have** : colonne vertébrale du pilier 1 (document générique).
Sans dossiers libres, tout le contenu est à plat et l'agnosticisme système n'existe pas.

La structure par défaut doit être neutre — des labels comme "Personnages", "Joueurs", "Scénarios",
"Notes" plutôt que "PNJ", "Monstres", "Sorts". Antoine (Blades in the Dark) et Émilie
(systèmes narratifs) doivent pouvoir renommer ou ignorer la structure.

Chaque espace — y compris l'espace `PERSONAL` — dispose de sa propre arborescence de dossiers
créée à l'initialisation de l'espace. Pour un espace `CAMPAIGN` ou `ONE_SHOT`, ce sont les quatre
dossiers système ci-dessus ; pour l'espace `PERSONAL`, l'arborescence initiale se réduit au dossier
virtuel « Non classés » : le MJ y ajoute ses propres dossiers.

**Critère de sortie** : un MJ crée des dossiers nommés librement. La structure par défaut
est système-agnostique.

**Risque si absent** : le pilier 1 n'existe pas.

---

### UC-06 — Utiliser la vue session

**Pourquoi Must Have** : use case différenciant principal. Raison pour laquelle Thomas
utiliserait Haversack sans migrer d'Obsidian. Interface centrale de chaque one-shot de Sonia.
Si la vue session n'apporte pas de valeur immédiate, la proposition produit est invalidée.

**Critère de sortie** : un MJ pilote une session depuis une vue dédiée — accès aux scènes,
notes et contenu épinglé, prise de note rapide. Fonctionne en mode local (sans partage joueurs).

**Risque si absent** : impossible de valider l'hypothèse centrale du produit.

---

### UC-07 — Créer un élément à la volée en session

**Pourquoi Must Have** : Émilie invente des PNJ, lieux et factions *pendant* la partie —
c'est sa douleur principale, pas la préparation. Antoine confirme pour Blades in the Dark
(factions et détails qui émergent en jeu).

Sans UC-07, la vue session est un outil de consultation. Avec UC-07, c'est un outil de
pilotage actif. La différence est la valeur réelle du second pilier.

**Critère de sortie** : depuis la vue session, le MJ crée un document en moins de dix secondes
sans quitter le contexte de session. Fonctionne en mode local.

**Risque si absent** : Émilie n'est pas servie. Le second pilier est incomplet.

---

### UC-08 — Partager une information aux joueurs

**Pourquoi Must Have** : justifie l'adoption sans migration. Thomas utiliserait Haversack
par-dessus Obsidian uniquement pour le partage combiné à la vue session. Rémi — résistant
au numérique par philosophie — ne considère Haversack que si le partage est simple.
Sonia en a besoin à chaque one-shot (partage d'intro, cartes, révélations).

Nécessite un compte (minimum gratuit) — le partage requiert un backend pour la diffusion
en temps réel vers les joueurs.

**Critère de sortie** : depuis la vue session, le MJ partage un document ou une note.
Un joueur avec le lien voit l'information sans créer de compte (→ UC-09).

**Risque si absent** : la proposition de valeur pour les profils non-migrants disparaît.

---

### UC-09 — Accès joueur sans compte

**Pourquoi Must Have** : Lucas (22 ans, joueur type) ferme l'onglet si le lien d'invitation
mène à une page d'inscription. Il a déjà D&D Beyond, Discord et un carnet. Un outil de plus
n'est pas bienvenu — si la friction est trop haute, il n'entre pas.

L'adoption joueur détermine la rétention du MJ : un MJ dont les joueurs ne rejoignent pas
arrête d'utiliser l'outil.

**Critère de sortie** : un joueur clique sur un lien, saisit un nom d'affichage, accède aux
informations partagées en temps réel — sans inscription.

**Risque si absent** : l'adoption du groupe entier est compromise. Le MJ perd sa raison
principale d'utiliser UC-08.

---

### UC-10 — Créer un compte et synchroniser dans le cloud

**Pourquoi Must Have** : UC-08 (partage joueurs) et UC-09 (accès joueur) exigent un compte
et une synchronisation cloud pour fonctionner. Inversement, une
capacité Must Have (partage) ne peut pas dépendre d'une capacité Should Have — cette incohérence
de priorisation rendrait le périmètre MVP non fiable.

Le MVP est livré en un seul bloc (pas de découpage en deux temps de livraison) : le cloud n'est
pas différable. Le compte est l'upgrade naturel déclenché par l'intention de partager ou la
peur de perdre ses données locales. La migration des données locales vers le cloud doit être
automatique et transparente.

**Critère de sortie** : un MJ en mode local crée un compte, ses données migrent vers le cloud,
le partage joueurs s'active.

**Risque si absent** : sans UC-10, le second pilier produit (partage
et collaboration joueur) est inatteignable, alors qu'il est Must Have.

Le raisonnement d'origine et les alternatives écartées (MVP en deux temps de livraison, statu quo du
reclassement) sont conservés à titre de trace historique (ADR-006).

---

### Espace personnel — conteneur par défaut et capture-first (Must Have — ADR-018)

**Pourquoi Must Have** : le MJ produit du contenu — idée de donjon, esquisse de PNJ, note de
convention — **sans lien avec une campagne en cours et sans intention d'en créer une**. Sans
zone d'atterrissage naturelle, ce contenu est soit perdu, soit contraint à une création de
campagne artificielle. L'espace personnel (`SpaceType.PERSONAL`) est cette zone.

L'espace personnel est créé automatiquement par le système à l'initialisation de l'environnement
du MJ (local ou compte). Il n'est pas décompté du quota d'espaces `CAMPAIGN`/`ONE_SHOT`
du tier FREE (voir ci-dessous). Tout document créé sans espace explicite y atterrit par défaut.

**Propriété vs activation vs bibliothèque — distinction MVP / post-MVP** :

- **Must Have / MVP — propriété et capture** : l'espace personnel existe, appartient au MJ,
  reçoit du contenu, dispose de sa propre arborescence de dossiers. Un document peut naître
  dans l'espace personnel sans qu'une campagne soit nécessaire (UC-01, UC-04). C'est le
  comportement acté dans cet item.
- **Post-MVP — bibliothèque de réutilisation inter-espaces** : la promotion de contenu vers
  un catalogue partageable, la navigation entre espaces, et l'instanciation en un geste
  (`Document.Instantiate(spaceId, folderId)`) depuis l'espace personnel vers un espace
  `CAMPAIGN`/`ONE_SHOT` constituent l'interface de bibliothèque. Ce comportement est aligné
  sur UC-13 et reste hors première livraison.

**Signal d'élargissement Must Have** : l'introduction de l'espace personnel étend le périmètre
Must Have au-delà des dix use cases initiaux. Ce point est tranché : comportement opérationnel
en MVP. Il a été arbitré explicitement (ADR-018 — recommandation « capture-first »,
décision du 2026-06-12).

**Critère de sortie** : un MJ en mode local peut créer et retrouver un document sans avoir
créé d'espace `CAMPAIGN` ou `ONE_SHOT`. L'espace personnel est provisonné automatiquement.

**Risque si absent** : le contenu pré-campagne est orphelin. Le modèle reste campagne-centré
et résiste au besoin de capture spontanée identifié comme central (ADR-018 §Contexte).

---

#### Quota FREE — espaces `CAMPAIGN`/`ONE_SHOT`

Le tier FREE inclut un quota de **3 espaces de type `CAMPAIGN` ou `ONE_SHOT`**. L'espace
personnel (`SpaceType.PERSONAL`) n'est **pas décompté** dans ce quota — il est provisionné
inconditionnellement à la création du compte, indépendamment du tier.

Cette règle est cohérente avec la vision produit (`vision.md`) et UC-02 : l'espace
personnel est une propriété permanente du compte, non une fonctionnalité soumise à limite.

---

### Export d'espace (Must Have — version minimale)

**Pourquoi Must Have** : (promu de Should Have par décision produit du 2026-06-25)
l'export matérialise le différenciant n°1 de la vision — la possession des données, qui doit être
actionnable et non simplement affirmée. L'export répond aussi à la douleur de confiance de Thomas (crainte
de l'enfermement propriétaire) et de Rémi (réassurance face au numérique), et constitue un filet de
sécurité critique pour le mode local.

**Motivation de la promotion** : la portabilité est un critère d'adoption pour le segment MJ-organisé
(persona Thomas — *"la portabilité, ce n'est pas un confort, c'est une question de confiance"*). Inclure
l'export en version minimale dès le MVP est nécessaire pour que la promesse de possession des données soit
crédible au premier contact.

**Périmètre MVP — version minimale** : export d'un espace (ou de l'espace personnel) en format ouvert,
lisible et réutilisable hors de l'application. Disponible en mode local comme avec un compte. L'UX
d'export avancée (granularité, formats multiples, export sélectif) est post-MVP.

**Distinct du réimport** : le réimport d'un fichier de sauvegarde reste Could Have / post-MVP (voir
section correspondante).

**Critère de sortie** : un MJ exporte l'ensemble de son espace — documents, notes, structure — dans un format ouvert, lisible et réutilisable hors de l'application. Disponible en mode local comme avec un compte.

**Risque si absent** : sans export accessible, la promesse de possession des données
reste une déclaration sans preuve.

> **Note de révision — 2026-06-25** : promu de Should Have (arbitrage 2026-06-10) à Must Have / version
> minimale. Motivation : adoption segment MJ-organisé (Thomas) — la portabilité est un critère de confiance,
> pas un confort. L'export doit être présent dès le MVP pour rendre la promesse de possession des données
> actionnable. Seul l'EXPORT est promu ; le RÉIMPORT reste Could Have / post-MVP.

---

### Instrumentation de validation du MVP

**Pourquoi Must Have** : le MVP unique teste simultanément trois piliers (préparation, vue session, partage). L'instrumentation doit permettre une analyse rétrospective claire de chaque pilier.

**Critère de sortie** : l'application permet de constater, de façon anonyme et sans capter le contenu narratif :

1. **Activation préparation** : le MJ a créé un espace ET y a créé **au moins trois documents** ;
   ou, en l'absence de campagne, a produit dans son espace personnel un **geste structurant** —
   **un dossier créé, ou un document déplacé hors de « Non classés »** (décision produit du 2026-09-03). Ce constat mesure si
   le pilier 1 (organisation et préparation) a engagé l'utilisateur au-delà de la création d'un
   conteneur. Les deux signaux sont observables par le seul modèle, sans lire aucun contenu.
   *Trois plutôt qu'un : un document est un essai, trois sont un usage — un seuil qui ne distingue
   pas prive H1 de la possibilité d'être invalidée, que `vision.md §2.3` exige.*

2. **Activation vue session** : une session a été ouverte ET **au moins une action du MJ y a eu lieu** — une note de session créée, un document épinglé, ou une scène naviguée (décision produit du 2026-09-03). Ce constat mesure si le pilier 2 (pilotage en direct) crée une valeur immédiate.
   *Une action plutôt qu'une durée : une session ouverte puis oubliée mesurerait la présence, non l'usage, alors que H2 porte sur la valeur apportée pendant la partie.*

3. **Activation partage** : un document a été partagé aux joueurs ET au moins un joueur l'a consulté. Ce constat mesure si le pilier 3 (collaboration joueur) fonctionne comme canal d'engagement collectif.

**Risque si absent** : sans mesure granulaire par pilier, un échec d'adoption serait indiagnosticable — impossible de savoir quel pilier n'a pas résonné avec les utilisateurs.

**Mesure anonyme dès le mode local** : la capture fonctionne sans compte utilisateur, ne porte aucun contenu narratif ni donnée nominative (aucune lecture de titre, notes ou propriété structurée), et respecte les obligations de protection des données personnelles — elle se limite à l'existence d'une action et à sa date.

**Comment cette mesure sort du navigateur — tranché** (décision produit du 2026-09-03). Les compteurs sont **écrits dans le store local et ne quittent le navigateur qu'à la création d'un compte**, avec la migration. Aucun appel sortant n'a lieu en mode local : la directive `connect-src 'self'` reste intacte, et avec elle la garantie observable « aucune donnée envoyée au serveur en mode local ».

**Limite de l'instrument, à ne pas taire.** Ce choix implique que les MJ qui ne créent jamais de compte ne transmettent jamais leurs compteurs. L'activation préparation, qui porte précisément sur un usage **sans compte**, n'est donc observée que chez ceux qui convertissent — un biais en faveur des convertis, qui affecte H1 et rend H5 partiellement circulaire. Cette limite est portée par `vision.md §2.3` et doit être rappelée à toute lecture des résultats.

**Capture de contact non bloquante en mode local** : l'application peut proposer au MJ en mode local de laisser une adresse de contact (pour un suivi de validation produit ultérieur). Cette proposition est toujours refusable sans conséquence — elle ne bloque ni l'accès ni le fonctionnement.

**Mécanisme — tranché** (décision produit du 2026-09-03) : la proposition est affichée **une seule fois**, **après** que le MJ a atteint l'activation préparation — donc à quelqu'un qui a déjà trouvé de la valeur, et non à l'entrée, ce qui contredirait le différenciant « friction d'entrée nulle ». **Un refus, ou une absence de réponse, vaut refus définitif** : la proposition ne revient jamais. L'adresse, si elle est donnée, part avec les compteurs à la création de compte, ne sert qu'au suivi de validation, et reste effaçable sur demande.

Le raisonnement d'origine et les alternatives écartées de cet arbitrage sont conservés à titre de trace historique (ADR-006).

---

## Should Have — MVP significativement plus faible sans eux

Ces use cases apportent une valeur forte mais le concept central peut être validé sans eux
lors d'une première version fermée ou d'un test utilisateur.

### UC-05 riche — Dossiers et types de document élaborés (Should Have)

**Pourquoi Should Have** : la base UC-05 (dossiers libres, structure agnostique) est Must Have.
La couche « riche » — types de document built-in (PNJ, Lieu, Objet…) et leur configuration
optionnelle — apporte une valeur forte mais le pilier 1 peut être validé avec des dossiers
libres sans types élaborés.

Contrepoids du reclassement de UC-10 en Must Have : pour ne pas gonfler le périmètre Must
au-delà du minimum cohérent, la couche riche est différée. Le pilier 1 se valide avec les dossiers
libres ; les types de document enrichissent l'expérience mais ne la conditionnent pas.

Les **templates par défaut des dossiers** (un dossier peut porter un modèle réutilisable dont
tout nouveau document du dossier est la copie) relèvent de la même couche riche, avec la même
priorité que l'association type ↔ dossier. Le socle Must de UC-05 reste les dossiers nommés
librement.

Les types de document optionnels préparent l'architecture pour les relations entre documents
et, à terme, l'émulation partielle de systèmes de jeu — sans que cela soit visible ou
contraignant pour l'utilisateur de base. Antoine (Blades in the Dark) et Émilie (systèmes
narratifs) bénéficient de cette couche, mais peuvent travailler sans elle.

**Critère de sortie** : les types de document built-in sont proposés sans être imposés.
Le MJ peut associer un type (PNJ, Lieu, Objet…) à un dossier ou un document pour enrichir
ses entrées sans contraindre la structure. Il peut aussi associer un template par défaut à un
dossier.

**Risque si absent** : le pilier 1 reste fonctionnel mais moins différenciant.

---

### UC-11 — Gérer les membres d'un espace

**Pourquoi Should Have** : couvre l'administration des accès durables — membres permanents
(Thomas, groupes stables, invitation durable) et lien d'espace permanent. Le lien de session
ponctuel (Sonia, joueurs changeants) n'en dépend pas : le MJ le génère depuis la vue session et
il relève d'UC-09. Sans UC-11, le MJ ne peut pas inviter ses joueurs réguliers.

**Critère de sortie** : le MJ peut inviter un membre permanent et générer un lien d'espace
permanent. Il peut révoquer un accès.

---

### UC-12 — Consulter son espace en tant que joueur (vue post-accès)

**Pourquoi Should Have** : distinct de UC-09 (octroi d'accès ponctuel) et UC-11 (gestion des
membres côté MJ). Couvre la valeur obtenue par le joueur une fois entré dans l'espace : une
vue cohérente regroupant sa fiche de personnage, les documents `PUBLIC` et l'historique partagé
selon le périmètre de son accès. Sans cette vue, le joueur membre dispose d'un accès sans
surface d'entrée unifiée — l'adhésion (octroi via lien, création de compte, création du
`SpaceMembership`) relève d'UC-09 (côté joueur) et d'UC-11 (côté MJ).

**Critère de sortie** : un joueur disposant d'un `SpaceMembership` actif accède à une vue
cohérente de son espace — fiche de personnage, documents `PUBLIC`, historique selon le
périmètre de son accès.

---

### UC-13 — Utiliser un scénario réutilisable

**Pourquoi Should Have** : Sonia (one-shots exclusivement, 15 scénarios en catalogue)
est structurellement exclue du produit sans ce use case — le modèle campagne avec membres
fixes ne correspond pas à son fonctionnement. Antoine bénéficie aussi des instances
cross-espaces. Sans UC-13, le produit ignore tout un profil de MJ actif.

**Critère de sortie** : le MJ marque un scénario comme réutilisable, crée une instance
pour un nouveau groupe, l'instance est indépendante du source.

**Arbitrage du 2026-06-10 — hors première livraison** : UC-13 est explicitement hors de la première
livraison. La validation du cœur du produit (préparation, vue session, partage) passe par le contexte
campagne, qui porte les personas principaux et l'hypothèse de monétisation ; le one-shot complet dépend
du catalogue de scénarios réutilisables, une capacité entière qui ne conditionne pas cette validation.
Sonia (one-shots exclusivement, 15 scénarios en catalogue) reste structurellement non servie par la
première livraison — c'est précisément ce que surveille la condition de retour : si les entretiens ou
l'usage révèlent que le profil one-shot exclusif (Sonia — conventions, groupes changeants, catalogue de
scénarios) est une part significative des utilisateurs réels ou un levier d'adoption, UC-13 est réexaminé en priorité lors d'une version ultérieure. Trace complète dans la vision produit,
section « Traces d'arbitrage vision ↔ périmètre MVP ».

**Note ADR-018** : la bibliothèque de réutilisation inter-espaces (UC-13 scénario nominal) s'appuiera
sur l'espace personnel comme infrastructure. L'interface de catalogue, de promotion et d'instanciation
reste post-MVP ; l'infrastructure (l'espace `PERSONAL` lui-même) est Must Have.

---

### UC-14 — Rechercher et filtrer l'information

**Pourquoi Should Have** : fonctionnalité de survie en session pour les MJ avec du volume.
Thomas (400 notes dans son vault) et Émilie (retrouver un PNJ inventé il y a trois séances)
en font une nécessité dès que l'espace a quelques semaines d'existence. La recherche par mot-clé est Should Have MVP ; le filtrage par tags (A3) est Could Have / hors MVP — les tags comme mécanisme au service de la recherche seront évalués en phase suivante selon les usages réels.

**Critère de sortie** : un MJ retrouve n'importe quel document de son espace par mot-clé
depuis la vue session en moins de cinq secondes.

---

## UC-15 — Classement arbitré : Post-MVP (spécifiés)

**Décision (2026-09-01)** : `UC-15` (gel de campagnes/espaces au downgrade de tier) est classé
« Post-MVP (spécifiés) » — hors catégorisation MoSCoW du MVP, catégorie déjà utilisée par
l'index des use cases (`cas-d-utilisation/README.md:44`).

**Motif** : ce classement aligne ce document — seule autorité du corpus pour attribuer une
priorité MoSCoW — sur `cas-d-utilisation/README.md`, qui range déjà `UC-15` dans « Post-MVP (spécifiés) ».
Il évite aussi d'attribuer à `UC-15` un vocabulaire MoSCoW (« Should Have ») réservé aux use cases
effectivement priorisés pour une livraison : le mécanisme de gel suppose un tier payant et un
downgrade, hors périmètre de la première livraison par construction, et non par arbitrage de
priorité relative comme le sont les use cases Should Have.

Avant cette décision, `UC-15` n'avait jamais été classé dans ce document — absent du tableau
« Vue d'ensemble » et du diagramme de dépendances (voir § Dépendances) — ce qui avait laissé le
fichier `UC-15-gel-espaces-downgrade-tier.md` s'auto-attribuer « Should Have — hors première
livraison (post-MVP) », un vocabulaire MoSCoW que seul ce document a l'autorité d'attribuer.

**Résumé du use case** : gel automatique (lecture seule) des espaces `CAMPAIGN`/`ONE_SHOT`
excédentaires lors d'un downgrade de tier, et dégel automatique et réversible au ré-upgrade,
sans perte de données. L'espace `PERSONAL` n'est jamais concerné (hors quota, jamais gelé).
Le mécanisme n'est pas déclenché en MVP — le downgrade suppose un tier payant, hors périmètre
première livraison.

---

## Variante one-shot express (UC-02 §A1) — Post-MVP (spécifiés)

**Décision** : la variante « parcours express » de UC-02 — lancer un one-shot depuis le tableau
de bord, avec un scénario existant ou un nouveau titre, en une action et sans configuration de
campagne — est classée « Post-MVP (spécifiés) », comme UC-15 : hors catégorisation MoSCoW du MVP,
et reportée après la première livraison, comme l'est UC-02 §A1.

**Motif** : le parcours express dépend du catalogue de scénarios réutilisables (UC-13, Should Have,
hors première livraison — arbitrage du 2026-06-10). UC-02 reste Must Have pour son scénario
nominal : en première livraison, un one-shot se crée par le parcours nominal de création d'espace,
avec `type = ONE_SHOT`.

---

## Could Have — Valeur réelle, non prioritaire pour la validation initiale

Ces fonctionnalités répondent à des besoins identifiés mais ne conditionnent pas la validation
du concept. Elles peuvent arriver dans une version ultérieure sans compromettre l'adoption initiale.

### Types de document personnalisés

Le MJ définit ses propres types de document au-delà des types built-in (PNJ, Lieu, Objet…),
avec des propriétés sur mesure adaptées à son système de jeu.

**Valeur** : Antoine (trois campagnes aux logiques différentes) peut créer un type "Faction"
avec les propriétés propres à Blades in the Dark. Thomas peut modéliser ses propres structures
de contenu. Prépare directement l'évolution vers les relations entre documents (UC-F06).

**Déclencheur de montée** : si les types built-in se révèlent insuffisants après quelques
semaines d'usage réel.

---

### Réimport de fichier de sauvegarde *(post-MVP — distinct de l'export Must Have)*

Le MJ réimporte un fichier de sauvegarde exporté par l'application pour récupérer un espace sans ressaisie. Distinct de l'**export d'espace** (Must Have / version minimale, disponible dès le MVP — promu 2026-06-25).

**Valeur** : Thomas a un vault Obsidian de 400 notes qu'il ne migrera pas manuellement.
Un import partiel (même imparfait) réduit le coût de transition. Argument commercial fort
pour les MJ déjà outillés.

**Déclencheur de montée** : si Thomas représente une part significative des early adopters
et que la migration manuelle est citée comme frein principal.

---

### Notes personnelles joueur

Un joueur prend des notes de session privées, pendant une session `LIVE` comme entre les
parties. Aucune prise de notes par le joueur n'existe au MVP : ni en `LIVE`, ni persistante
d'une session à l'autre. Le joueur n'écrit une note que s'il a un personnage associé
(RB-12-03).

**Valeur** : utile pour les groupes stables et les campagnes longues. Complète l'expérience
joueur au-delà de la simple consultation.

**Pourquoi Could Have et non Should Have** : Lucas (joueur type) préfère Discord et son carnet.
Les joueurs one-shot (Sonia) n'ont aucun besoin de persistance. La valeur n'apparaît que pour
les groupes réguliers après plusieurs semaines d'usage.

**Option écartée** (arbitrage du 2026-10-07) : la prise de notes en `LIVE` en Must Have, avec
seule la persistance d'une session à l'autre en Could Have. Toutes les notes de session du
joueur sont sorties du MVP.

---

### Persistance du personnage actif entre sessions (UC-12)

Le personnage actif d'un joueur est mémorisé d'une session à l'autre. UC-12 le décide
session-local : une préférence client, non persistée.

**Pourquoi Could Have** : la sélection du personnage actif ne gouverne que le focus de
présentation, jamais un droit d'accès ; la persister n'est pas nécessaire à la vue post-accès.

**Déclencheur de montée** : les retours des utilisateurs.

---

### Notes libres du joueur, sans personnage associé (UC-12)

Un joueur prend une note `PLAYER_PRIVATE` qui n'est rattachée à aucun personnage.

**Pourquoi Could Have** : toute note `PLAYER_PRIVATE` requiert un personnage associé
(RB-12-02, RB-12-03, RB-12-10) ; UC-12 n'offre aucun emplacement de note libre.

---

### Synchronisation source → instance d'un scénario réutilisable (UC-13)

Un diff ou une synchronisation entre le scénario source et ses instances.

**Pourquoi Could Have** : une instance est une copie indépendante du scénario source
(`Document.Instantiate`) ; UC-13 ne prévoit ni diff ni synchronisation. Le MJ décide seul de
promouvoir à nouveau un source amélioré.

---

### Journal d'activité des membres (UC-11)

Un historique des actions d'invitation et de gestion des membres : qui a rejoint l'espace, quand,
et quel accès a été retiré.

**Pourquoi Could Have** : le MJ gère ses membres sans journal ; la liste des membres et des
invitations suffit à la validation du concept. US-UC-11 l'exclut du MVP (audit log non
prioritaire).

---

### Notification du joueur au partage d'un document (UC-12)

Le joueur est averti activement quand le MJ lui partage un nouveau document.

**Pourquoi Could Have** : le joueur découvre le nouveau document en consultant sa vue ; la
notification active n'est pas nécessaire à la vue post-accès. US-UC-12 l'exclut du MVP
(fonctionnalité de notification non prioritaire).

---

### Persistance des notes d'un invité d'une session à l'autre (UC-12)

Les notes `PLAYER_PRIVATE` d'un invité sans compte survivent à la fin de son `GuestAccess` et
sont retrouvées par un nouveau lien vers le même personnage.

**Pourquoi Could Have** : au MVP, ces notes sont supprimées à la fin définitive du `GuestAccess`
(RGPD, art. 17) ; aucune note de session du joueur n'existe de toute façon au MVP (voir *Notes
personnelles joueur*). US-UC-12 classe cette persistance Could Have, post-MVP.

---

### Notification au MJ à l'utilisation d'une invitation (UC-11)

Le MJ est averti activement quand un joueur utilise le lien d'invitation qu'il a généré.

**Pourquoi Could Have** : le MJ voit l'état de ses invitations et de ses membres dans le panneau
membres ; la notification active n'est pas nécessaire à la validation du concept. US-UC-11
l'exclut du MVP (fonctionnalité de notification non prioritaire).

---

### Récapitulatif post-session par email à l'invité (UC-09)

Un récapitulatif de la session est envoyé par email au joueur invité sans compte.

**Pourquoi Could Have** : l'envoi suppose une adresse email, que l'invité sans compte ne
renseigne pas au MVP. US-UC-09 l'exclut du MVP.

---

### Template de vue session (UC-06)

Le MJ réutilise la configuration de sa vue session d'une campagne ou d'un one-shot à l'autre.

**Pourquoi Could Have** : l'auto-save de la vue session et son mode édition hors session
suffisent au MVP ; le template n'est pas nécessaire à la validation du concept. US-UC-06
l'exclut du MVP.

---

### Historique des partages (UC-08)

Le MJ consulte la date à laquelle chaque document a été partagé aux joueurs.

**Pourquoi Could Have** : la date de partage est tracée dans `AuditInfo` mais n'est pas
affichée au MVP. US-UC-08 l'exclut du MVP.

---

### Historique des recherches récentes (UC-14)

La recherche propose les requêtes récentes du MJ.

**Pourquoi Could Have** : apport d'ergonomie pour les MJ fréquents, non nécessaire à la
recherche par mot-clé du MVP. US-UC-14 l'exclut du MVP.

---

### Filtrage par tag en recherche (UC-14, A3)

Le MJ restreint les résultats de la recherche aux documents portant un tag donné (UC-14 A3).

**Pourquoi Could Have** : la recherche par mot-clé suffit au MVP ; le filtrage par tags est un
usage complémentaire et différé des tags, à évaluer en phase suivante selon les usages réels
(voir § UC-14). Les tags eux-mêmes restent disponibles au MVP pour l'organisation du MJ (UC-04).

---

## Won't Have — Exclus par choix stratégique

Ces fonctionnalités sont explicitement hors périmètre MVP. Leur absence est assumée.

### Inventaire personnage

**Pourquoi non** : compète avec D&D Beyond sur son terrain et sans pouvoir l'égaler.
Émilie joue des systèmes sans inventaire. Antoine a trois systèmes aux logiques incompatibles.
Une implémentation générique serait trop pauvre ; une implémentation correcte, trop coûteuse.

---

### Clôture formelle de session

**Pourquoi non** : absorbé par UC-04 (notes post-session) et UC-07 (capture à la volée).
La "clôture formelle" est un workflow, pas un use case atomique distinct. Sonia invalide
le concept (pas de continuité narrative entre one-shots).

---

### Documents de lore en use case distinct

**Pourquoi non** : un document de lore est un document dans un dossier "Lore". Le pilier 1
(document générique + dossiers libres) le couvre entièrement. Créer un use case distinct
n'ajoute aucune valeur différenciante.

---

### Rôle GM — co-MJ (UC-F08)

**Pourquoi non** : aucun use case du MVP ne décrit comment un membre devient GM ; le MVP n'attribue que les rôles `OWNER` et `PLAYER` (entrée de backlog 0026). Le modèle de domaine garde le rôle. Détail et condition de retour : [exclusions, UC-F08](exclusions.md#uc-f08--rôle-gm-co-mj).

---

### Relations entre documents typés (UC-F06 — phase 1)

**Pourquoi non** : nécessite que les types de document built-in soient stables et adoptés.
La complexité UX des relations bidirectionnelles est élevée. À construire après validation
de l'adoption des types de document.

Les **backlinks** (liste des documents qui pointent vers un document donné) font partie de ce
périmètre : ils sont hors MVP, avec les relations structurées qui les remplacent.

**Prérequis technique à ne pas rater** : le schéma `Document` doit prévoir
`documentTypeId` et `properties` (structure de données structurée) dès le MVP pour éviter une migration
majeure lors de l'activation des relations.

---

### Moteur de règles / émulation système (UC-F06 — phase 2)

**Pourquoi non** : dépend des relations entre documents (phase 1). Vision long terme.
Risque de dérive vers un moteur de règles complet incompatible avec le positionnement
d'outil de narration agnostique.

---

### Assistant IA

**Pourquoi non** : risque de brouiller le positionnement MVP. Haversack doit d'abord
prouver sa valeur comme outil d'organisation. L'IA est une extension, pas le cœur.

---

### Table visuelle

**Pourquoi non** : concurrence directe avec Roll20 et Foundry VTT sur leur territoire.
Hors positionnement du produit.

---

### Application desktop avec synchronisation CRDT

**Pourquoi non** : l'architecture CRDT (résolution de conflits multi-device, sync offline)
est disproportionnée pour le MVP. UC-01 (mode local navigateur) couvre le besoin de
"démarrer sans compte" sans cette complexité.

---

### Templates communautaires

**Pourquoi non** : modération, qualité variable, complexité de gestion d'un catalogue.
Vision long terme uniquement.

---

## Dépendances

```
UC-01 (Mode local)
  ├── [espace PERSONAL — créé automatiquement, conteneur par défaut]
  │     └── UC-04 (Documents — naissance sans campagne)
  └── UC-02 (Espace CAMPAIGN / ONE_SHOT)
        ├── UC-03 (Scénario)
        ├── UC-04 (Documents)
        ├── UC-05 base (Dossiers libres — Must)
        │     └── UC-05 riche (Types élaborés — Should)
        └── UC-06 (Vue session)
              ├── UC-07 (Création à la volée)
              └── UC-08 (Partage) ──── nécessite UC-10 (compte)
                    └── UC-09 (Accès joueur)

UC-10 (Compte — Must)  ← Must car UC-08/09 (Must) en dépendent
  ├── UC-11 (Membres permanents — Should)
  ├── UC-12 (Vue joueur — Should)
  └── UC-13 (One-shot — Should, hors première livraison) ── dépend aussi de UC-02, UC-03
            ↑ infrastructure fournie par [espace PERSONAL] ; interface bibliothèque post-MVP
```

**Point clé** : UC-01 est le nouveau point d'entrée. L'espace personnel est provisionné dès
UC-01 — un document peut naître sans espace `CAMPAIGN` ou `ONE_SHOT`. L'inscription (UC-10)
est Must Have car elle conditionne le partage joueurs (UC-08) — déclenchée par l'intention de
partager, pas par le démarrage. UC-11 à UC-14 restent Should Have. `UC-15`, classé
Post-MVP (spécifiés) (voir §UC-15 ci-dessus), n'apparaît pas dans ce diagramme.

**Convention actée** : ce diagramme couvre les use cases classés MoSCoW (Must Have, Should Have,
Could Have — voir § Vue d'ensemble) et leurs dépendances propres. Il exclut par construction les
use cases classés « Post-MVP (spécifiés) », catégorie distincte de la classification MoSCoW
(voir §UC-15) : ceux-ci n'y figurent pas, avec leurs dépendances. `UC-15` est à ce jour le seul
use case dans ce cas. Un futur use case classé « Post-MVP (spécifiés) » n'a donc pas à être ajouté
à ce diagramme — son absence n'est pas un oubli.
