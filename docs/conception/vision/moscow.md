# Priorisation MoSCoW — Haversack MVP

> **MoSCoW** = Must Have · Should Have · Could Have · Won't Have (this release)
>
> Source de vérité pour les arbitrages produit. Révisée sur la base de l'analyse des 7 personas
> (Thomas, Émilie, Lucas, Nadia, Antoine, Rémi, Sonia).

---

## Vue d'ensemble

| Catégorie | Nb | Use cases |
|---|---|---|
| **Must Have** | 9 | UC-01 à UC-09 |
| **Should Have** | 5 | UC-10 à UC-14 |
| **Could Have** | 4 | Export, types personnalisés, import, notes joueur |
| **Won't Have** | — | Voir détail ci-dessous |

---

## Must Have — Le produit ne peut pas être validé sans eux

Ces neuf use cases forment le périmètre minimal cohérent : sans l'un d'eux, soit le produit
ne peut pas être utilisé, soit l'hypothèse centrale ne peut pas être testée.

### UC-01 — Mode local sans compte

**Pourquoi Must Have** : forcer la création de compte avant toute valeur perçue est une friction
rédhibitoire. Nadia a abandonné Notion pour cette raison. Rémi ne créera pas de compte sans raison
concrète. Thomas ne migrera pas d'Obsidian — mais il peut évaluer localement.

Le mode local est aussi la fondation du modèle de monétisation non-agressif :
local gratuit → compte gratuit (cloud + partage) → Pro (illimité).
La valeur est perçue avant l'engagement financier.

**Critère de sortie** : un MJ crée une campagne et prépare du contenu sans s'inscrire.
Ses données persistent entre sessions navigateur.

**Risque si absent** : friction d'onboarding maximale. Perte d'utilisateurs avant toute
perception de valeur.

---

### UC-02 — Créer et configurer une campagne

**Pourquoi Must Have** : la campagne est le conteneur de toutes les données.
La définition doit rester large : conteneur léger (Thomas), one-shot isolé (Sonia via UC-13),
campagne narrative longue (Antoine). Configuration opérationnelle en moins de deux minutes.

**Critère de sortie** : un MJ crée une campagne nommée et accède à son espace de travail.
Fonctionne en mode local.

**Risque si absent** : aucune structuration de contenu possible.

---

### UC-03 — Structurer un scénario

**Pourquoi Must Have** : l'hypothèse produit principale à valider en premier.
Le système de blocs génériques couvre les deux extrêmes : cinq bullet points (Émilie)
et scénario détaillé avec scènes liées (Antoine). Aucune structure D&D-centrique imposée.

**Critère de sortie** : un MJ crée un scénario avec des scènes et sauvegarde.
Fonctionne en mode local.

**Risque si absent** : impossible de valider la proposition de valeur principale.

---

### UC-04 — Gérer les notes MJ

**Pourquoi Must Have** : les notes sont le filet de sécurité du MJ — préparation légère,
capture d'urgence en session, consolidation post-session. Un MJ sans possibilité de noter
n'utilisera pas l'outil. La capture en session doit être quasi-immédiate.

**Critère de sortie** : un MJ crée une note, la retrouve et la modifie.
Fonctionne en mode local.

**Risque si absent** : l'outil ne remplace aucun des supports actuels du MJ.

---

### UC-05 — Organiser le contenu en dossiers

**Pourquoi Must Have** : colonne vertébrale du pilier 1 (document générique).
Sans dossiers libres, tout le contenu est à plat et l'agnosticisme système n'existe pas.

La structure par défaut doit être neutre — des labels comme "Personnages", "Lieux", "Notes",
"Sessions" plutôt que "PNJ", "Monstres", "Sorts". Antoine (Blades in the Dark) et Émilie
(systèmes narratifs) doivent pouvoir renommer ou ignorer la structure.

Les types de document optionnels (PNJ, Lieu, Objet…) préparent l'architecture pour les
relations entre documents et, à terme, l'émulation partielle de systèmes de jeu — sans que
cela soit visible ou contraignant pour l'utilisateur de base.

**Critère de sortie** : un MJ crée des dossiers nommés librement. La structure par défaut
est système-agnostique. Les types de document built-in sont proposés sans être imposés.

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

## Should Have — MVP significativement plus faible sans eux

Ces use cases apportent une valeur forte mais le concept central peut être validé sans eux
lors d'une première version fermée ou d'un test utilisateur.

### UC-10 — Créer un compte et synchroniser dans le cloud

**Pourquoi Should Have** : le compte n'est plus le prérequis universel — UC-01 couvre
le mode local. Il devient l'upgrade naturel déclenché par l'intention de partager (UC-08)
ou la peur de perdre ses données locales. La migration des données locales vers le cloud
doit être automatique et transparente à la création du compte.

**Critère de sortie** : un MJ en mode local crée un compte, ses données migrent vers le cloud,
le partage joueurs s'active.

---

### UC-11 — Gérer les membres d'une campagne

**Pourquoi Should Have** : couvre deux besoins distincts — membres permanents (Thomas, groupes
stables, invitation durable) et accès ponctuels (Sonia, joueurs changeants, lien de session
temporaire). Sans ce use case, le MJ ne peut pas inviter ses joueurs réguliers ni créer les
liens de session nécessaires à UC-08.

**Critère de sortie** : le MJ peut inviter un membre permanent et générer un lien de session
temporaire. Il peut révoquer un accès.

---

### UC-12 — Rejoindre une campagne (membre permanent)

**Pourquoi Should Have** : distinct de UC-09 (accès ponctuel sans compte). Couvre le joueur
régulier qui veut accéder à l'historique des sessions partagées et aux documents de lore
visibles entre les parties. Nécessite un compte joueur.

**Critère de sortie** : un joueur avec un compte rejoint une campagne via un lien permanent
et accède aux informations historiques partagées par le MJ.

---

### UC-13 — Utiliser un scénario réutilisable

**Pourquoi Should Have** : Sonia (one-shots exclusivement, 15 scénarios en catalogue)
est structurellement exclue du produit sans ce use case — le modèle campagne avec membres
fixes ne correspond pas à son fonctionnement. Antoine bénéficie aussi des instances
cross-campagnes. Sans UC-13, le produit ignore tout un profil de MJ actif.

**Critère de sortie** : le MJ marque un scénario comme réutilisable, crée une instance
pour un nouveau groupe, l'instance est indépendante du source.

---

### UC-14 — Rechercher et filtrer l'information

**Pourquoi Should Have** : fonctionnalité de survie en session pour les MJ avec du volume.
Thomas (400 notes dans son vault) et Émilie (retrouver un PNJ inventé il y a trois séances)
en font une nécessité dès que la campagne a quelques semaines d'existence. Inclut le filtrage
par tags — les tags sont un mécanisme au service de la recherche, pas un use case distinct.

**Critère de sortie** : un MJ retrouve n'importe quel document de sa campagne par mot-clé
depuis la vue session en moins de cinq secondes.

---

## Could Have — Valeur réelle, non prioritaire pour la validation initiale

Ces fonctionnalités répondent à des besoins identifiés mais ne conditionnent pas la validation
du concept. Elles peuvent arriver dans une deuxième vague sans compromettre l'adoption initiale.

### Export de campagne

Le MJ exporte l'ensemble de sa campagne (documents, notes, structure) en Markdown ou PDF.

**Valeur** : portabilité des données pour Thomas (qui veut pouvoir récupérer son travail),
réassurance sur la possession des données pour Rémi, filet de sécurité pour le mode local.
Cohérent avec le positionnement "vos données vous appartiennent".

**Déclencheur de montée** : si les entretiens révèlent une peur forte de l'enfermement
propriétaire (vendor lock-in).

---

### Types de document personnalisés

Le MJ définit ses propres types de document au-delà des types built-in (PNJ, Lieu, Objet…),
avec des propriétés sur mesure adaptées à son système de jeu.

**Valeur** : Antoine (trois campagnes aux logiques différentes) peut créer un type "Faction"
avec les propriétés propres à Blades in the Dark. Thomas peut modéliser ses propres structures
de contenu. Prépare directement l'évolution vers les relations entre documents (UC-F06).

**Déclencheur de montée** : si les types built-in se révèlent insuffisants après quelques
semaines d'usage réel.

---

### Import depuis Obsidian / Notion

Le MJ importe des fichiers Markdown existants pour récupérer son travail passé sans ressaisie.

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
`documentTypeId` et `properties` (JSON structuré) dès le MVP pour éviter une migration
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
  └── UC-02 (Campagne)
        ├── UC-03 (Scénario)
        ├── UC-04 (Notes)
        ├── UC-05 (Dossiers + types)
        └── UC-06 (Vue session)
              ├── UC-07 (Création à la volée)
              └── UC-08 (Partage) ──── nécessite UC-10 (compte)
                    └── UC-09 (Accès joueur)

UC-10 (Compte)
  ├── UC-11 (Membres)
  │     └── UC-09 (liens de session)
  ├── UC-12 (Rejoindre)
  └── UC-13 (One-shot) ── dépend aussi de UC-02, UC-03
```

**Point clé** : UC-01 est le nouveau point d'entrée. L'inscription (UC-10) est le premier
upgrade naturel, déclenché par l'intention de partager — pas par le démarrage.
