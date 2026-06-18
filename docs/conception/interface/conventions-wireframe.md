# Conventions de description basse-fidélité — Haversack MVP

> Couche réflexive de la sous-couche interface.
> Ce fichier définit les conventions de notation et de nommage utilisées dans toutes les fiches de description d'écran (wireframes basse-fidélité, vague 3).
> Il ne contient pas de contenu d'écran et ne dicte pas le besoin — il documente la forme que prend la description de ce besoin.
> Source d'autorité pour la cohérence inter-fiches : toute fiche de description d'écran suit ces conventions sans les redéfinir.

---

## C1 — Discipline et test d'autoportance

### Rôle de ce fichier

Ces conventions permettent de décrire les futures surfaces de l'interface Haversack en langage de besoin et de comportement observable, indépendamment de tout choix d'implémentation. Leur respect garantit que les fiches restent lisibles par toute personne du domaine sans connaissance des technologies retenues, et qu'elles ne deviennent pas obsolètes au premier changement d'implémentation.

### Contrainte structurelle — test d'autoportance

**Toute zone, affordance ou état se décrit par comportement observable.** Le test est le suivant : masquer mentalement un terme technique dans la description. Si la description reste complète et compréhensible, le terme est informatif. Si la description devient vide ou orpheline, elle est à réécrire en langage besoin.

Ce test s'applique à chaque section de chaque fiche d'écran, avant toute intégration. Il n'est pas optionnel.

**Exemples :**

| Formulation à éviter | Formulation correcte |
|---|---|
| « bouton de type primary avec état disabled » | « action principale — inactive si aucun titre n'est saisi » |
| « modal de confirmation » | « surface de confirmation qui bloque toute autre interaction » |
| « toast de succès » | « notification non bloquante signalant la réussite de l'action » |
| « breakpoint tablette » | « forme tablette — largeur réduite, contenu réorganisé » |
| « stocké en localStorage » | « données conservées localement sur l'appareil » |

---

## C2 — Les deux registres de nommage (garde-fou central)

Les descriptions d'écran mobilisent deux registres de nommage distincts. Les confondre, les mélanger ou en créer un troisième est interdit.

### Règle de séparation

Un terme de domaine ne se renomme jamais en surface hors de l'arbitrage AR-05. Un terme de région d'interface reste stable d'une fiche à l'autre et n'entre pas au glossaire. Tout nouveau terme de région s'ajoute dans ce fichier, dans la liste ci-dessous, avant d'être employé dans une fiche.

---

### Registre 1 — Termes de domaine

**Autorité : glossaire** (`docs/conception/glossaire.md`). Ces termes sont figés ; leur forme et leur sémantique sont celles du glossaire.

| Terme de surface | Identifiant de domaine | Remarque |
|---|---|---|
| campagne | `CAMPAIGN` | Label de surface pour un espace de type CAMPAIGN — AR-05 |
| espace personnel | `PERSONAL` / `SpaceType.PERSONAL` | Nommé « Espace personnel » en surface — AR-05 |
| « espace de jeu » | terme de conception | N'apparaît jamais en surface — AR-05 |
| document | `Document` | Unité fondamentale de contenu |
| dossier | `Folder` | Conteneur organisationnel |
| « Non classés » | dossier virtuel (`isVirtual = true`) | Dossier de repli, non supprimable |
| note de session | `LIVE_NOTE` | Document de type LIVE_NOTE |
| scénario | `Document` de type `SCENARIO` | Cas spécialisé de document |
| visible par les joueurs | `PUBLIC` | Niveau de visibilité — cartographie dans glossaire §LIVE_NOTE |
| privé MJ | `GM_ONLY` | Niveau de visibilité — idem |
| note de session personnelle joueur | `PLAYER_PRIVATE` | Niveau de visibilité — idem |
| session | `Session` | Séance de jeu |
| en cours | `LIVE` | Statut de session |
| terminée | `CLOSED` | Statut de session |
| archivée | `ARCHIVED` | Statut de session, lecture seule |
| MJ | `OWNER` ou `GM` | Maître du Jeu — rôle dans un espace partagé |
| joueur | `PLAYER` / `GuestAccess` | Membre joueur — authentifié ou invité |
| membre | `SpaceMembership` actif | Voir glossaire §Membre |
| invité | `GuestAccess` | Joueur sans compte |
| épinglé | `pinnedDocumentIds` | Document dans le panneau des épinglés |
| partagé | `visibility = PUBLIC` | Document rendu visible par les joueurs |
| quota FREE | `AccountTier.FREE` — 3 espaces partagés | Voir AR-17 ; l'espace personnel est hors quota |

---

### Registre 2 — Termes de région d'interface

**Autorité : ce fichier.** Ces termes désignent des zones et éléments de l'interface ; ils sont stables inter-fiches et n'entrent pas au glossaire.

| Terme de région | Description sommaire |
|---|---|
| tableau de bord | Surface listant tous les espaces dont l'utilisateur est propriétaire ou membre |
| vue campagne / espace de travail | Hub d'un espace partagé — point d'entrée vers les dossiers, les documents, la vue session |
| vue session MJ | Surface de pilotage de session — configurable, multi-modes (AR-09) |
| vue joueur | Surface dédiée au joueur — séparée de la surface MJ (AR-03) |
| panneau de dossiers | Zone affichant le contenu d'un dossier sélectionné dans la vue session |
| panneau des documents épinglés | Zone permanente dans la vue session listant les documents épinglés (`pinnedDocumentIds`) |
| panneau latéral | Zone secondaire apparaissant sans interrompre le contexte principal (ex. résultats de recherche en session — AR-11) |
| zone de notes | Zone de saisie des notes de session — co-présente, jamais masquée par défilement (AR-04) |
| barre de recherche | Zone de saisie de la recherche, omniprésente dans la surface concernée (UC-14 ; AR-11) |
| bandeau de durabilité | Bandeau non bloquant signalant un risque de perte des données locales (châssis S7) |
| bandeau de confidentialité | Bandeau non bloquant signalant l'absence de protection par identifiants en mode local (châssis S7) |
| invite contextuelle | Sollicitation non bloquante proposant une action au franchissement d'une frontière (AR-13) |
| indicateur de statut de session | Élément omniprésent dans la barre de la vue session matérialisant le mode courant (AR-09 ; châssis S7) |
| indicateur de partage | Récapitulatif permanent de ce que les joueurs voient en ce moment (AR-12 ; châssis S7) |
| rail de bascule | Mécanisme permettant de basculer d'un panneau focus à un autre quand la largeur d'écran est réduite (AR-07) |
| gate de migration local→cloud | Surface de confirmation explicite lors de la transition mode local → compte cloud — matérialisation de la *Gate de reconnaissance* (glossaire §Gate de reconnaissance). Le terme de région « gate de migration local→cloud » et le terme de domaine « Gate de reconnaissance » désignent la même surface ; seul le terme de région figure dans les fiches d'écran. |
| formulaire de création rapide | Zone de saisie minimale du panneau de création rapide à la volée — seul le titre est obligatoire ; tous les autres champs sont optionnels ou complétables après création. Source : S4 §Vue session MJ ; UC-07. |
| zone d'accès via fournisseur externe | Zone proposant la connexion ou l'inscription via un fournisseur d'identité externe en un seul geste — aucun mot de passe à définir ou à saisir. Source : UC-10 §scénario nominal Connexion via fournisseur externe ; AR-13. |
| zone de contenu partagé | Zone principale de la surface joueur affichant les documents PUBLIC partagés par le MJ pour la session — visible dès l'ouverture du lien, avant saisie du nom. Source : UC-09 scénario A §4 ; AR-03. |
| formulaire de saisie du nom d'affichage | Zone de saisie recueillant uniquement le nom d'affichage du joueur invité — seul champ requis pour participer à une session ponctuelle. Source : UC-09 scénario A §5 ; RB-09-20. |
| zone d'information RGPD | Bandeau non bloquant sur la surface joueur informant l'invité de la nature, de la durée de conservation et du sort de ses données à la fin de l'accès. Source : RB-09-20 (RGPD Art. 13). |
| zone de documents partagés | Zone principale de la vue joueur post-accès listant les documents PUBLIC visibles selon le périmètre d'accès (SESSION ou CAMPAIGN). Source : UC-12 scénario nominal §4 ; RB-12-01 ; AR-06. |
| zone de notes personnelles | Zone de création et de consultation des notes PLAYER_PRIVATE du joueur, liées au personnage actif. Source : UC-12 scénario nominal §5 ; RB-12-02. |
| zone de fiche de personnage | Zone latérale de la vue joueur affichant la fiche du personnage associé au joueur par le MJ. Source : UC-12 scénario nominal §3 ; RB-12-06 ; RB-12-08. |
| zone de message d'erreur | Zone principale de la page d'erreur d'accès — message sobre sans révélation de la campagne, même registre pour tous les motifs (lien expiré, révoqué, invalide, quota atteint). Source : UC-09 A3 ; UC-09 E1 ; UC-09 E2 ; RB-09-21 ; AR-03. |
| zone de navigation par dossiers | Zone affichant l'arborescence des dossiers d'un espace et permettant au MJ d'y parcourir son contenu ; épine dorsale de la navigation (AR-11 — unicité d'appartenance d'un document à un dossier). Source : UC-05 ; AR-11 ; AR-16 (espace personnel : seul « Non classés » à la création, pas de dossiers système). |
| zone de contenu / liste des documents | Zone listant les documents du dossier courant ou de l'espace en vue condensée (titre et repère de type) ; accès à la création et à l'édition de tout document de la liste. Source : UC-04 ; UC-05 A3 ; AR-11. |
| zone d'accès à l'éditeur de document | Point d'entrée depuis la zone de navigation ou la liste des documents vers l'éditeur de document — création (titre seul obligatoire) ou modification d'un document existant. Source : UC-04 ; AR-15 (capture-first). |
| zone d'accès à la vue session | Point d'entrée depuis la vue campagne (espace de travail) vers la vue session MJ — permet de lancer la session (mode LIVE) ou d'y accéder en mode configuration ; affiche un repère « session en cours » si une session est déjà au statut en cours dans l'espace. Source : UC-06 §Déclencheur ; AR-09. |
| zone d'accès aux paramètres | Accès à la configuration de la vue session (dossiers mis en avant, ordre), à l'archivage de l'espace et à la génération du lien d'invitation depuis la vue campagne (espace de travail). Source : UC-06 Phase 2 ; UC-11 A4 ; AR-09 ; AR-10. |

> Pour ajouter un terme de région : ouvrir ce fichier, ajouter la ligne dans le tableau ci-dessus, documenter la source ou l'arbitrage associé. Ne pas employer un terme de région absent de ce tableau dans une fiche d'écran.

---

## C3 — Légende de notation basse-fidélité

Cette légende couvre les familles de marqueurs utilisées dans les fiches d'écran. Chaque marqueur est exprimé en notation textuelle, sans référence à un outil graphique.

### Famille 1 — Zone

Délimite une région fonctionnelle de l'écran.

```
[ NOM DE ZONE ]
  type     : châssis | principal | latéral | bandeau | formulaire | favoris
  rôle     : <description en une phrase du rôle fonctionnel de la zone>
```

Les types correspondent aux régions suivantes :
- **châssis** : chrome applicatif transversal (indicateurs, bandeaux, barre de navigation).
- **principal** : zone de contenu primaire de l'écran.
- **latéral** : zone secondaire apparaissant à côté du contenu principal sans le remplacer.
- **bandeau** : bande horizontale non bloquante portant un message d'état.
- **formulaire** : zone de saisie structurée.
- **favoris** : zone listant les éléments épinglés ou mis en avant.

---

### Famille 2 — Priorité

Indique la hiérarchie de présence sur l'écran.

```
priorité : co-présent-jamais-masqué | principal | secondaire-configurable
```

- **co-présent-jamais-masqué** : la zone ou l'élément est toujours visible, quels que soient la configuration et le défilement (ex. zone de notes — AR-04, indicateur de statut — AR-09).
- **principal** : présence par défaut, non configurable.
- **secondaire-configurable** : présence conditionnée par la configuration du MJ (ex. panneaux de dossiers — AR-04).

---

### Famille 3 — État / mode

Décrit les modes ou états successifs de l'écran ou d'un élément.

```
état/mode : <liste des états possibles, séparés par " | ">
  - <état A> : <description du comportement observable dans cet état>
  - <état B> : <description du comportement observable dans cet état>
```

Pour la vue session MJ, les modes sont nommés `configuration | LIVE | consultation CLOSED` conformément à AR-09.

---

### Famille 4 — Visibilité

Décrit qui peut voir une zone ou un élément, en termes de besoin et de domaine.

```
visibilité : PUBLIC (visible par les joueurs)
           | GM_ONLY (privé MJ)
           | PLAYER_PRIVATE (note de session personnelle joueur)
           | MJ seul
           | joueur seul
           | tous membres
```

---

### Famille 5 — Affordance

Décrit ce que l'utilisateur peut faire depuis la zone — en verbe de besoin, jamais en nom de widget.

```
affordance :
  - [SOURCE AR-XX | UC-XX] <verbe de besoin + conditions> → <résultat observable>
```

Exemples corrects :
```
affordance :
  - [AR-12 ; UC-08] partager le document → le document devient visible par les joueurs et s'auto-épingle (session LIVE uniquement)
  - [AR-09 ; UC-06] lancer la session → la vue bascule en mode LIVE
```

Exemples incorrects (nom de widget, pas de verbe de besoin) :
```
affordance :
  - bouton « Partager » (disabled si CLOSED)   ← INTERDIT
```

---

### Famille 6 — Différé / hors-MVP

> **Départage Famille 6 / Famille 8** : une exclusion *décidée* (Won't Have ou Could Have, source S8 ou moscow) → `[HORS-MVP]` (Famille 6) ; une conception *non tranchée* (trou de corpus, point d'interview non résolu, S9) → `[SOUS-SPÉCIFIÉ]` (Famille 8).

Signale un élément présent dans le corpus mais explicitement hors périmètre wireframe MVP (S8).

```
[HORS-MVP — <source S8>] <description de l'élément différé>
```

Exemples :
```
[HORS-MVP — UC-11 scénario nominal] invitation par email
[HORS-MVP — moscow.md §Could Have] notes personnelles joueur persistantes inter-sessions sans compte
```

---

### Famille 7 — Absent par nature vs désactivé / grisé

Cette distinction est fondamentale et s'appuie sur AR-14.

```
[ABSENT PAR NATURE] <nom de la fonction>
  → La fonction n'existe pas pour ce type d'espace ou ce contexte.
  → Ne pas noter « S.O. » ni « désactivé ».

[DÉSACTIVÉ] <nom de la fonction>
  → La fonction existe mais n'est pas accessible dans l'état courant.
  → Condition de réactivation : <condition observable>
```

**Règle d'application :** pour l'espace personnel (`PERSONAL`), la vue session et la surface joueur sont **absentes par nature** (AR-14 ; AR-01). Elles ne sont ni grisées ni désactivées — elles n'ont pas de sens pour un espace mono-membre. Cette distinction n'est jamais contournée par « S.O. » ou un tiret silencieux.

---

### Famille 8 — Sous-spécifié

Signale un élément dont la conception n'est pas encore tranchée dans le corpus (S9).

```
[SOUS-SPÉCIFIÉ — <source S9>] <description de ce qui manque>
```

Exemples :
```
[SOUS-SPÉCIFIÉ — S9 §Éditeur de document] disposition des blocs et affordances de type en détail
[SOUS-SPÉCIFIÉ — S9 §Vue « Non classés »] interface de la vue dédiée aux documents non classés
```

---

### Famille 9 — Accessibilité

Décrit les points d'accessibilité **spécifiques à cet écran** (delta par rapport aux garanties transversales du châssis S7 — voir `gabarit-ecran.md §Accessibilité delta`).

```
accessibilité (delta) :
  - annonce sans action : <quels changements d'état de CET écran sont annoncés assistivement — NFR-ACC-02>
  - hiérarchie de lecture à distance : <ordre et priorité des zones pour un usage à distance — NFR-ACC-04>
```

---

### Famille 10 — Ancrage besoin

Trace la source de besoin d'une zone, d'une affordance ou d'un état.

```
ancrage : UC-XX | US-XX | NFR-XX | AR-XX
```

La traçabilité est à la maille **écran** par défaut. Elle descend à la maille **zone** uniquement quand une zone matérialise un arbitrage particulier (ex. indicateur de partage ↔ AR-12 — voir `gabarit-ecran.md §Traçabilité`).
