# Priorisation MoSCoW — Haversack MVP

> MoSCoW = **M**ust Have · **S**hould Have · **C**ould Have · **W**on't Have (this release)
>
> Cette matrice couvre les use cases du MVP et les fonctionnalités hors périmètre.
> Chaque décision est justifiée. Source de vérité pour les arbitrages produit.

---

## Résumé

| Catégorie | Nb UCs | UCs |
|---|---|---|
| Must Have | 5 | UC-00, UC-01, UC-02, UC-04, UC-06 |
| Should Have | 10 | UC-03, UC-05, UC-07, UC-09, UC-10, UC-11, UC-13, UC-14, UC-15, UC-17 |
| Could Have | 4 | UC-08, UC-12, UC-16, UC-18 |
| Won't Have | 4 | UC-F01, UC-F02, UC-F03, UC-F04 |

---

## Must Have — Bloquant pour le MVP

> Sans ces use cases, le produit ne peut pas être testé ni validé.
> Ce sont les hypothèses fondamentales à confirmer en premier.

### UC-00 — S'inscrire et gérer son compte

**Pourquoi Must Have** : sans authentification, aucun autre use case ne fonctionne.
Le compte est le point d'entrée de tout le produit.

**Critère de sortie** : un MJ peut créer un compte, se connecter, et accéder à son tableau de bord.

**Risque si absent** : aucun utilisateur ne peut utiliser l'application.

---

### UC-01 — Créer et configurer une campagne

**Pourquoi Must Have** : la campagne est le conteneur de toutes les données.
Sans campagne, aucun contenu (scénario, PNJ, session) ne peut exister.

**Critère de sortie** : un MJ peut créer une campagne nommée, accéder à son tableau de bord et y naviguer.

**Risque si absent** : aucune structuration de contenu possible.

---

### UC-02 — Structurer un scénario

**Pourquoi Must Have** : la préparation de scénario est le cas d'usage central déclaré
par les MJ comme douleur principale. C'est l'hypothèse produit numéro un à valider.

**Critère de sortie** : un MJ peut créer un scénario avec des scènes, lier des PNJ, sauvegarder.

**Risque si absent** : impossible de valider la proposition de valeur principale.

---

### UC-04 — Gérer les notes MJ

**Pourquoi Must Have** : les notes sont le moyen le plus basique et le plus fréquent
de capturer de l'information. C'est le filet de sécurité du MJ. Un MJ sans notes
n'utilisera pas l'outil.

**Critère de sortie** : un MJ peut créer une note privée ou partagée, la retrouver, la modifier.

**Risque si absent** : l'outil ne remplace aucun des supports actuels du MJ.

---

### UC-06 — Utiliser la vue session

**Pourquoi Must Have** : c'est le use case différenciant principal d'Haversack
par rapport aux outils génériques. Si la vue session n'apporte pas de valeur,
la proposition produit est invalidée.

**Critère de sortie** : un MJ peut piloter une session depuis une vue dédiée,
accéder aux PNJ/scènes/notes en quelques secondes, prendre des notes rapides.

**Risque si absent** : impossible de valider l'hypothèse centrale du produit.

---

## Should Have — MVP incomplet sans eux

> Ces use cases apportent une valeur forte. Un MVP sans eux peut être livré
> pour une première validation, mais il serait significativement moins
> convaincant pour les utilisateurs cibles.

### UC-03 — Créer et gérer des PNJ

**Pourquoi Should Have** : les PNJ sont directement liés aux scénarios et à la vue session.
Sans fiches PNJ, la préparation est incomplète et la vue session perd une partie de sa valeur.
Très élevée car bloquant pour l'usage réel, mais pas pour la démonstration du concept.

---

### UC-05 — Préparer une session de jeu

**Pourquoi Should Have** : la préparation de session (sélection de scènes, PNJ, notes)
est la passerelle entre la préparation et la vue session. Nécessaire pour que UC-06
soit exploitable avec du contenu préparé.

---

### UC-07 — Créer et gérer une fiche personnage

**Pourquoi Should Have** : la fiche personnage est une attente forte des joueurs
et un élément visible de la valeur pour le groupe. Sa présence ou absence impacte
l'adoption côté joueurs.

---

### UC-09 — Partager une information aux joueurs

**Pourquoi Should Have** : le partage sélectif est un différenciant fort vs Notion/Google Docs.
Les MJ ne peuvent pas contrôler qui voit quoi dans leurs outils actuels.
Ce use case valide une partie importante de la promesse produit.

---

### UC-10 — Rejoindre une campagne ou une session

**Pourquoi Should Have** : sans ce use case, les joueurs ne peuvent pas accéder à l'application.
L'expérience joueur sans friction (GuestAccess) est un argument commercial clé.

---

### UC-11 — Rechercher rapidement une information

**Pourquoi Should Have** : la recherche est le mécanisme de survie en session.
Un MJ qui ne peut pas retrouver une information en 5 secondes abandonnera l'outil.
Fortement liée à la valeur de UC-06.

---

### UC-13 — Gérer ses notes personnelles (joueur)

**Pourquoi Should Have** : les joueurs invités sans compte doivent pouvoir agir comme des joueurs
complets sur le périmètre de leur personnage. Les notes `PLAYER_PRIVATE` liées au `CharacterId`
sont donc importantes pour tenir la promesse de continuité entre deux séances, même sans compte.

---

### UC-14 — Créer un élément à la volée pendant la session

**Pourquoi Should Have** : la réactivité aux imprévus est une douleur directement
exprimée par les MJ. Un PNJ créé en 3 secondes pendant la partie est une fonctionnalité
à fort impact perçu. Directement complémentaire à UC-06.

---

### UC-15 — Gérer les membres d'une campagne

**Pourquoi Should Have** : sans gestion des membres, le MJ ne peut pas inviter ses joueurs.
L'adoption du groupe entier est le vecteur de rétention principal du produit.

---

### UC-17 — Organiser le contenu en dossiers

**Pourquoi Should Have** : les dossiers sont le mécanisme central d'organisation du contenu.
Sans eux, tout le contenu est à plat et la modularité promise (MJ crée ses propres catégories)
n'existe pas. Les dossiers système (PNJ, Personnages joueurs, Scénarios, Notes) sont créés
automatiquement à la création de la campagne — ils sont requis pour que UC-03, UC-07 et UC-02
aient un emplacement logique. La feature dossiers personnalisés peut être livrée en seconde vague,
mais les dossiers système doivent exister dès le Must Have.

---

## Could Have — Utile, non bloquant

> Ces use cases améliorent l'expérience mais ne conditionnent pas la validation
> du concept. Peuvent être reportés à une itération post-MVP sans compromettre
> l'adoption initiale.

### UC-08 — Gérer l'inventaire d'un personnage

**Pourquoi Could Have** : l'inventaire est une feature de confort pour les joueurs.
Sa présence augmente l'adoption joueur, mais son absence ne bloque pas le MJ.
À inclure si le temps le permet.

---

### UC-12 — Clôturer une session et préparer la suite

**Pourquoi Could Have** : la clôture avec résumé est une bonne pratique mais pas
un use case d'urgence. Le MJ peut clôturer sans résumé dans un premier temps.
La valeur se révèle sur le long terme (historique de campagne).

---

### UC-16 — Gérer les documents de lore

**Pourquoi Could Have** : les fiches de lieux et factions enrichissent les campagnes
denses, mais sont moins urgentes pour un premier MVP. Les MJ peuvent temporairement
stocker ces informations comme notes (UC-04) avec un type CUSTOM.

---

### UC-18 — Synchroniser un document avec son template

**Pourquoi Could Have** : la synchronisation manuelle résout un vrai problème (faire évoluer
la structure de fiches existantes), mais ce problème ne se pose qu'après un usage prolongé.
Un MJ débutant ne le ressentira pas dans les premières semaines. À inclure quand les premiers
utilisateurs commencent à avoir des campagnes matures.

---

## Won't Have — Hors périmètre MVP

> Ces fonctionnalités sont explicitement exclues du MVP. Leur absence est un choix
> stratégique, pas un oubli. Certaines sont modélisées dans le domaine pour
> faciliter leur activation future.

### UC-F01 — Templates de système de jeu

**Pourquoi Won't Have** : chaque système de jeu a ses propres règles et structures.
Implémenter des templates corrects pour D&D, Call of Cthulhu, Fate, etc. représente
un effort de contenu considérable. Le modèle Document générique du MVP est suffisant
pour valider l'usage. À construire après validation de l'adoption de base.

**État dans le domaine** : `DocumentTemplate` est modélisé. `GameSystem` existe.
L'activation ne nécessite pas de refonte.

---

### UC-F02 — Assistant IA pour le MJ

**Pourquoi Won't Have** : l'IA peut apporter de la valeur (génération de PNJ, résumés,
suggestions d'improvisation), mais elle risque de brouiller le positionnement MVP.
Haversack doit d'abord prouver sa valeur comme outil d'organisation, pas comme outil d'IA.

**Risque principal** : construire un produit "IA" au lieu d'un outil d'organisation.

---

### UC-F03 — Table visuelle légère

**Pourquoi Won't Have** : concurrence directe avec Roll20 et Foundry VTT sur leur
territoire. La complexité technique est élevée (canvas, synchronisation temps réel,
assets graphiques). Hors positionnement du produit.

---

### UC-F04 — Version desktop / local-first

**Pourquoi Won't Have** : architecture local-first nécessite une stratégie de
synchronisation complexe (CRDT ou équivalent). Utile pour les sessions physiques
hors connexion, mais la complexité est disproportionnée pour le MVP.

---

## Notes de priorisation

### Dépendances entre Must Have

```
UC-00 (Auth)
  └── UC-01 (Campagne)
        ├── UC-02 (Scénario)
        ├── UC-04 (Notes)
        └── UC-06 (Vue session)
```

Les Must Have ont une dépendance stricte en cascade. Ils doivent être livrés
dans cet ordre.

### Dépendances Should Have → Must Have

```
UC-03 (PNJ)           dépend de UC-01, UC-02, UC-17 (dossier PNJ système)
UC-05 (Prép. session) dépend de UC-01, UC-02, UC-03
UC-09 (Partage)       dépend de UC-04
UC-10 (Rejoindre)     dépend de UC-01, UC-15
UC-11 (Recherche)     dépend de UC-01
UC-13 (Notes joueur)  dépend de UC-07, UC-10
UC-14 (Volée)         dépend de UC-06
UC-15 (Membres)       dépend de UC-01
UC-17 (Dossiers)      dépend de UC-01 — dossiers système créés avec la campagne
```

### Slice minimale UC-06

UC-06 reste Must Have, mais sa première livraison doit être une slice minimale :
session rapide ou préparée, affichage scénario/scènes si présents, LiveNotes MJ,
`pinnedItems` et consultation rapide. Les fonctions avancées appelées depuis UC-06
(partage détaillé, recherche pondérée, création à la volée, vue joueur enrichie)
peuvent arriver via les Should Have sans bloquer le démarrage du développement.

### Dépendances Could Have

```
UC-16 (Lore)          dépend de UC-17 (dossier personnalisé)
UC-18 (Sync template) dépend de UC-17, d'un template existant et de documents créés depuis ce template
```

### Révision périodique

Cette matrice doit être révisée après chaque cycle de feedback utilisateur.
Les priorités Could Have peuvent monter si les interviews révèlent
une douleur non anticipée (ex. : UC-08 Inventaire critique si les joueurs
demandent un suivi de ressources pendant la session).
