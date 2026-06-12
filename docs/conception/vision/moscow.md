# Priorisation MoSCoW — Haversack MVP

> **MoSCoW** = Must Have · Should Have · Could Have · Won't Have (this release)
>
> Source de vérité pour les arbitrages produit. Révisée sur la base de l'analyse des 7 personas
> (Thomas, Émilie, Lucas, Nadia, Antoine, Rémi, Sonia).

---

## Vue d'ensemble

| Catégorie | Nb | Éléments |
|---|---|---|
| **Must Have** | 10 UC + espace personnel + instrumentation | UC-01 à UC-10 (dont UC-05 base — dossiers libres) + espace personnel (capture-first, conteneur par défaut) + instrumentation de validation du MVP |
| **Should Have** | 6 | UC-05 riche (dossiers et types élaborés), UC-11, UC-12, UC-14, UC-13 *(hors première livraison)*, export d'espace |
| **Could Have** | 3 | Types personnalisés, réimport de fichier de sauvegarde *(post-MVP ; distinct de l'export Should Have)*, notes joueur |
| **Won't Have** | — | Voir détail ci-dessous |

---

## Must Have — Le produit ne peut pas être validé sans eux

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

La structure par défaut doit être neutre — des labels comme "Personnages", "Lieux", "Notes",
"Sessions" plutôt que "PNJ", "Monstres", "Sorts". Antoine (Blades in the Dark) et Émilie
(systèmes narratifs) doivent pouvoir renommer ou ignorer la structure.

Chaque espace — y compris l'espace `PERSONAL` — dispose de sa propre arborescence de dossiers
créée à l'initialisation de l'espace.

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
et une synchronisation cloud pour fonctionner. Sans UC-10, le second pilier produit (partage
et collaboration joueur) est inatteignable, alors qu'il est Must Have. Inversement, une
capacité Must Have (partage) ne peut pas dépendre d'une capacité Should Have — cette incohérence
de priorisation rendrait le périmètre MVP non fiable.

Le MVP est livré en un seul bloc (pas de découpage en deux vagues produit) : le cloud n'est
pas différable. Le compte est l'upgrade naturel déclenché par l'intention de partager ou la
peur de perdre ses données locales. La migration des données locales vers le cloud doit être
automatique et transparente.

**Critère de sortie** : un MJ en mode local crée un compte, ses données migrent vers le cloud,
le partage joueurs s'active.

Le raisonnement d'origine et les alternatives écartées (MVP en deux vagues, statu quo du
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
Must Have au-delà des dix use cases initiaux. L'opérateur a tranché : comportement opérationnel
en MVP. Ce point a été arbitré explicitement (ADR-018 — recommandation « capture-first »,
validation session 2026-06-12).

**Critère de sortie** : un MJ en mode local peut créer et retrouver un document sans avoir
créé d'espace `CAMPAIGN` ou `ONE_SHOT`. L'espace personnel est provisonné automatiquement.

**Risque si absent** : le contenu pré-campagne est orphelin. Le modèle reste campagne-centré
et résiste au besoin de capture spontanée identifié comme central (ADR-018 §Contexte).

---

### Quota FREE — espaces `CAMPAIGN`/`ONE_SHOT`

Le tier FREE inclut un quota de **3 espaces de type `CAMPAIGN` ou `ONE_SHOT`**. L'espace
personnel (`SpaceType.PERSONAL`) n'est **pas décompté** dans ce quota — il est provisionné
inconditionnellement à la création du compte, indépendamment du tier.

Cette règle est cohérente avec W1 (vision-produit.md) et UC-02 : l'espace personnel est une
propriété permanente du compte, non une fonctionnalité soumise à limite.

---

### Instrumentation de validation du MVP

**Pourquoi Must Have** : le MVP unique teste simultanément trois piliers (préparation, vue session, partage). Sans mesure granulaire par pilier, un échec d'adoption serait indiagnosticable — impossible de savoir quel pilier n'a pas résonné avec les utilisateurs. L'instrumentation doit permettre une analyse rétrospective claire de chaque pilier.

L'application permet de constater, de façon anonyme et sans capter le contenu narratif :

1. **Activation préparation** : le MJ a créé un espace (ou utilisé son espace personnel) ET y
   a créé ses premiers documents. Ce constat mesure si le pilier 1 (organisation et préparation)
   a engagé l'utilisateur au-delà de la création d'un conteneur.

2. **Activation vue session** : une session a été ouverte ET réellement utilisée pendant une partie — interaction avec du contenu, création de notes, navigation dans les panneaux. Ce constat mesure si le pilier 2 (pilotage en direct) crée une valeur immédiate.

3. **Activation partage** : un document a été partagé aux joueurs ET au moins un joueur l'a consulté. Ce constat mesure si le pilier 3 (collaboration joueur) fonctionne comme canal d'engagement collectif.

**Mesure anonyme dès le mode local** : la capture fonctionne sans compte utilisateur, ne porte aucun contenu narratif ni donnée nominative (aucune lecture de titre, notes ou propriété structurée), et respecte les obligations de protection des données personnelles — elle se limite à l'existence d'une action et à sa date.

**Capture de contact non bloquante en mode local** : l'application peut proposer au MJ en mode local de laisser une adresse de contact (pour un suivi de validation produit ultérieur). Cette proposition est toujours refusable sans conséquence — elle ne bloque ni l'accès ni le fonctionnement.

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

Les types de document optionnels préparent l'architecture pour les relations entre documents
et, à terme, l'émulation partielle de systèmes de jeu — sans que cela soit visible ou
contraignant pour l'utilisateur de base. Antoine (Blades in the Dark) et Émilie (systèmes
narratifs) bénéficient de cette couche, mais peuvent travailler sans elle.

**Critère de sortie** : les types de document built-in sont proposés sans être imposés.
Le MJ peut associer un type (PNJ, Lieu, Objet…) à un dossier ou un document pour enrichir
ses entrées sans contraindre la structure.

**Risque si absent** : le pilier 1 reste fonctionnel mais moins différenciant.

---

### UC-11 — Gérer les membres d'un espace

**Pourquoi Should Have** : couvre deux besoins distincts — membres permanents (Thomas, groupes
stables, invitation durable) et accès ponctuels (Sonia, joueurs changeants, lien de session
temporaire). Sans ce use case, le MJ ne peut pas inviter ses joueurs réguliers ni créer les
liens de session nécessaires à UC-08.

**Critère de sortie** : le MJ peut inviter un membre permanent et générer un lien de session
temporaire. Il peut révoquer un accès.

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
l'usage révèlent que le profil one-shot est une part significative des utilisateurs réels ou un levier
d'adoption, UC-13 est réexaminé en priorité de la vague suivante. Trace complète dans la vision produit,
section « Traces d'arbitrage vision ↔ périmètre MVP ».

**Note ADR-018** : la bibliothèque de réutilisation inter-espaces (UC-13 scénario nominal) s'appuiera
sur l'espace personnel comme infrastructure. L'interface de catalogue, de promotion et d'instanciation
reste post-MVP ; l'infrastructure (l'espace `PERSONAL` lui-même) est Must Have.

---

### UC-14 — Rechercher et filtrer l'information

**Pourquoi Should Have** : fonctionnalité de survie en session pour les MJ avec du volume.
Thomas (400 notes dans son vault) et Émilie (retrouver un PNJ inventé il y a trois séances)
en font une nécessité dès que l'espace a quelques semaines d'existence. Inclut le filtrage
par tags — les tags sont un mécanisme au service de la recherche, pas un use case distinct.

**Critère de sortie** : un MJ retrouve n'importe quel document de son espace par mot-clé
depuis la vue session en moins de cinq secondes.

---

### Export d'espace

**Pourquoi Should Have** (rehaussé de Could Have par arbitrage du 2026-06-10) :
l'export matérialise le différenciant n°1 de la vision — la possession des données, qui doit être
actionnable et non simplement affirmée. Sans export accessible, la promesse de possession des données
reste une déclaration sans preuve. L'export répond aussi à la douleur de confiance de Thomas (crainte
de l'enfermement propriétaire) et de Rémi (réassurance face au numérique), et constitue un filet de
sécurité critique pour le mode local.

**Critère de sortie** : un MJ exporte l'ensemble de son espace — documents, notes, structure — dans un format ouvert, lisible et réutilisable hors de l'application. Disponible en mode local comme avec un compte.

---

## Could Have — Valeur réelle, non prioritaire pour la validation initiale

Ces fonctionnalités répondent à des besoins identifiés mais ne conditionnent pas la validation
du concept. Elles peuvent arriver dans une deuxième vague sans compromettre l'adoption initiale.

### Types de document personnalisés

Le MJ définit ses propres types de document au-delà des types built-in (PNJ, Lieu, Objet…),
avec des propriétés sur mesure adaptées à son système de jeu.

**Valeur** : Antoine (trois campagnes aux logiques différentes) peut créer un type "Faction"
avec les propriétés propres à Blades in the Dark. Thomas peut modéliser ses propres structures
de contenu. Prépare directement l'évolution vers les relations entre documents (UC-F06).

**Déclencheur de montée** : si les types built-in se révèlent insuffisants après quelques
semaines d'usage réel.

---

### Réimport de fichier de sauvegarde *(post-MVP — distinct de l'export Should Have)*

Le MJ réimporte un fichier de sauvegarde exporté par l'application pour récupérer un espace sans ressaisie. Distinct de l'**export d'espace** (Should Have, disponible dès le MVP).

**Valeur** : Thomas a un vault Obsidian de 400 notes qu'il ne migrera pas manuellement.
Un import partiel (même imparfait) réduit le coût de transition. Argument commercial fort
pour les MJ déjà outillés.

**Déclencheur de montée** : si Thomas représente une part significative des early adopters
et que la migration manuelle est citée comme frein principal.

---

### Notes personnelles joueur

Un joueur avec un compte prend des notes privées liées à une session, persistantes entre
les parties.

**Valeur** : utile pour les groupes stables et les campagnes longues. Complète l'expérience
joueur au-delà de la simple consultation.

**Pourquoi Could Have et non Should Have** : Lucas (joueur type) préfère Discord et son carnet.
Les joueurs one-shot (Sonia) n'ont aucun besoin de persistance. La valeur n'apparaît que pour
les groupes réguliers après plusieurs semaines d'usage.

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

### Relations entre documents typés (UC-F06 — phase 1)

**Pourquoi non** : nécessite que les types de document built-in soient stables et adoptés.
La complexité UX des relations bidirectionnelles est élevée. À construire après validation
de l'adoption des types de document.

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
  ├── UC-11 (Membres — Should)
  │     └── UC-09 (liens de session)
  ├── UC-12 (Vue joueur — Should)
  └── UC-13 (One-shot — Should, hors première livraison) ── dépend aussi de UC-02, UC-03
            ↑ infrastructure fournie par [espace PERSONAL] ; interface bibliothèque post-MVP
```

**Point clé** : UC-01 est le nouveau point d'entrée. L'espace personnel est provisionné dès
UC-01 — un document peut naître sans espace `CAMPAIGN` ou `ONE_SHOT`. L'inscription (UC-10)
est Must Have car elle conditionne le partage joueurs (UC-08) — déclenchée par l'intention de
partager, pas par le démarrage. UC-11 à UC-14 restent Should Have.
