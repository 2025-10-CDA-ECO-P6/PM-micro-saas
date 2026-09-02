# Gabarit de description d'écran — Haversack MVP

> Couche réflexive de la sous-couche interface.
> Ce gabarit définit la structure de toute fiche de description d'écran basse-fidélité.
> La notation utilise les familles de marqueurs de `conventions-wireframe.md` — à lire avant d'utiliser ce gabarit.
> L'exemple-pilote (§ Exemple) instancie le gabarit sur la vue session MJ.

---

## Discipline rappelée

**Test d'autoportance** : toute zone, affordance ou état se décrit par comportement observable. Masquer un terme technique ne doit pas rendre la description incomplète. Si c'est le cas, la reformulation est obligatoire.

Les **intitulés de section** de ce gabarit sont en langage de besoin (« Ce que l'utilisateur peut faire », pas « Composants »). Ne pas les renommer dans les fiches d'écran.

---

## Structure du gabarit

---

### En-tête de fiche

```
# [Nom de l'écran]
> Aligné sur l'inventaire S4 de zoning.md.

Nom          : <nom exact tel qu'il figure dans l'inventaire S4>
Surface      : MJ | joueur | transversal
Contexte     : préparation | session | transversal
Type d'espace: CAMPAIGN | ONE_SHOT | PERSONAL | n.a. (si l'écran est indépendant du type)
Forme cible  : grand écran + tablette (mobile : pensé dans la structure, non implémenté au MVP — AR-07)
Traçabilité  : <UC / US / NFR couverts, à la maille écran — source : colonne « UC / US / UJ couverts » de S4>
```

**Règle** : les cinq axes reproduisent exactement les colonnes de S4 (`zoning.md §S4`). Ils ne créent pas de système parallèle. Si un axe est absent du tableau S4, il est noté `non spécifié en S4`.

---

### Noyau obligatoire

#### Intention

> *Une phrase de besoin décrivant pourquoi cet écran existe et ce que l'utilisateur accomplit en l'utilisant.*

```
Intention : <une phrase — ce que l'utilisateur accomplit sur cet écran>
```

Critère de complétude : la phrase nomme l'utilisateur (MJ / joueur / utilisateur non authentifié) et le résultat attendu de son passage sur cet écran. Elle ne mentionne aucune technologie.

---

#### Zones et hiérarchie

> *Inventaire des zones de l'écran, avec leur rôle, leur type et leur priorité de présence. Utilise la Famille 1 et la Famille 2 de `conventions-wireframe.md`.*

```
[ NOM DE ZONE ]
  type     : châssis | principal | latéral | bandeau | formulaire | favoris
  rôle     : <rôle fonctionnel en une phrase>
  priorité : co-présent-jamais-masqué | principal | secondaire-configurable
  visibilité : <qui voit cette zone — Famille 4>
```

Critère de complétude : toutes les zones de l'écran sont listées. Chaque zone a un type, un rôle et une priorité. Les zones ayant une visibilité restreinte (MJ seul, joueur seul) le déclarent explicitement.

---

#### Ce que l'utilisateur peut faire

> *Liste des affordances de l'écran. Utilise la Famille 5 de `conventions-wireframe.md` — verbe de besoin + source AR/UC + résultat observable.*

```
- [SOURCE] <verbe de besoin + conditions> → <résultat observable>
```

Critère de complétude : chaque affordance est ancrée sur un UC ou un AR. Aucun nom de widget ne figure dans cette section.

---

#### États

> *Décrit les états pertinents de l'écran (vide, chargé, erreur). N'invente pas d'état absent du corpus.*

```
état vide   : <comportement quand l'écran s'affiche sans contenu>
état chargé : <comportement nominal>
état erreur : <comportement en cas d'erreur pertinente pour cet écran — uniquement si sourcé>
```

Critère de complétude : seuls les états pertinents pour cet écran sont décrits. Un état sans source corpus est marqué `[SOUS-SPÉCIFIÉ]`.

---

#### Accessibilité (delta)

> *Deux réponses obligatoires. Ne re-liste pas les garanties transversales du châssis S7 (focus visible, ordre clavier — acquises). Décrit uniquement ce qui est spécifique à cet écran.*

Les garanties transversales (focus clavier visible, ordre de navigation cohérent) sont portées par le châssis applicatif (`zoning.md §S7 §Accessibilité transversale`). Cette section ne les répète pas.

```
annonce sans action :
  <quels changements d'état de CET écran sont annoncés assistivement, sans action de l'utilisateur — NFR-ACC-02>

hiérarchie de lecture à distance :
  <ordre et priorité de lecture des zones pour un utilisateur à distance de l'écran — NFR-ACC-04>
```

Critère de complétude : les deux réponses sont présentes. Si un changement d'état n'est pas annoncé assistivement, cela se note explicitement (pas de silence).

**Mise en garde — portée de NFR-ACC-04** : NFR-ACC-04 borne explicitement son périmètre (§Portée et hors-portée) à la vue session, aux notes de session `LIVE`, aux résultats de recherche depuis la vue session et à la vue joueur pendant la session — elle **exclut nommément la phase de préparation**. L'exemple-pilote ci-dessous (vue session MJ) est un écran de session : y citer NFR-ACC-04 est légitime. Ce n'est **pas** un patron à recopier tel quel sur un écran hors session (transversal, préparation, tableau de bord) : sur ces écrans, la hiérarchie de lecture à distance reste utile à décrire, mais sa source doit être marquée `[SOUS-SPÉCIFIÉ]` plutôt que renvoyée à NFR-ACC-04. NFR-ACC-01 (ordre de tabulation clavier) est un objet distinct et ne comble pas ce manque.

---

#### Sources

> *Traçabilité à la maille écran — UC, US, NFR et AR couverts.*

```
Sources : <UC-XX> ; <US-XX> ; <NFR-XX> ; <AR-XX>
```

Critère de complétude : au minimum, les UC porteurs de l'écran (colonne S4) et les AR qui en conditionnent le comportement sont listés.

---

### Sections conditionnelles

Les sections ci-dessous n'apparaissent que si leur déclencheur est avéré. **L'absence d'une section conditionnelle signifie que la fonction est absente par nature** (voir AR-14 et `conventions-wireframe.md §C3 Famille 7`). Ne jamais noter « S.O. » — si la section n'est pas déclenchée, elle n'apparaît pas.

---

#### Modes (conditionnel)

**Déclencheur : l'écran a plus d'un mode.** Source de référence : AR-09 (pour la vue session MJ) ou tout autre arbitrage nommant des modes.

Les modes sont des **sections d'une même surface**, pas des écrans distincts.

```
#### Mode : <nom du mode>

Déclencheur : <condition observable qui active ce mode>

Zones actives     : <quelles zones sont visibles et accessibles dans ce mode>
Zones désactivées : [DÉSACTIVÉ] <zone> — condition de réactivation : <condition>
Zones absentes    : [ABSENT PAR NATURE] <zone> — raison : <raison en langage besoin>
Affordances spécifiques :
  - [SOURCE] <verbe de besoin> → <résultat>
```

---

#### Partage / visibilité (conditionnel)

**Déclencheur : l'écran touche la frontière joueur, c'est-à-dire qu'une action sur cet écran peut modifier ce que les joueurs voient.** Sources : AR-03, AR-12.

```
Règle de séparation : <rappel en une phrase du principe AR-03 pour cet écran>

Indicateur de partage :
  <ce que l'indicateur de partage (châssis S7) affiche sur cet écran>

Affordances de partage et d'épinglage :
  - [AR-12 ; UC-08] partager → <résultat>
  - [AR-12 ; UC-08] épingler → <résultat>
  - [AR-12] distinction partager ≠ épingler : <comment l'écran les distingue visuellement>
```

---

#### Mode local (conditionnel)

**Déclencheur : l'écran est une surface MJ.** Source : châssis S7 ; AR-13.

Les surfaces MJ au sens de l'ossature sont : le tableau de bord, la vue campagne / espace de travail, la vue session MJ, la vue de préparation (navigation dossiers, éditeur, recherche), et l'espace personnel (préparation uniquement). Les écrans transversaux (Accueil, Inscription, Connexion, Gate de migration) et la surface joueur ne sont **pas** des surfaces MJ — ce déclencheur ne s'applique pas à eux. Source : S4 §Écrans transversaux ; AR-01 ; S4 §Surface MJ.

Toutes les surfaces MJ affichent le châssis mode local si l'utilisateur n'a pas de compte.

```
Bandeaux présents :
  - bandeau de durabilité : <comportement spécifique à cet écran, si différent du standard châssis>
  - bandeau de confidentialité : <idem>

Fonctions cloud désactivées sur cet écran :
  - [AR-13] <nom de la fonction> → invite contextuelle : <texte de l'invite en langage besoin>
```

Si aucune fonction cloud n'est désactivée sur cet écran spécifiquement, noter : « Châssis standard — voir S7 §Châssis mode local. »

---

#### Hors-périmètre / différé / sous-spécifié (conditionnel)

**Déclencheur : l'écran contient des éléments listés en S8 ou en S9.**

```
Éléments différés (S8) :
  [HORS-MVP — <source S8>] <description>

Éléments sous-spécifiés (S9) :
  [SOUS-SPÉCIFIÉ — <source S9>] <description>
```

---

### Traçabilité

La traçabilité est à la **maille écran** par défaut (conforme à S4 et S5 de `zoning.md`).

La traçabilité descend à la maille **zone** uniquement lorsqu'une zone matérialise un arbitrage nommé, c'est-à-dire quand le fait de porter cet arbitrage à ce niveau est la raison d'existence de la zone. Exemples légitimes :

- Indicateur de partage ↔ AR-12 (la zone matérialise l'exigence de distinguer partager ≠ épingler).
- Indicateur de statut de session ↔ AR-09 (la zone matérialise l'exigence de rendre le mode courant omniprésent).

**Règle anti-remplissage** : ne pas tracer chaque zone systématiquement. La traçabilité par-zone est réservée aux cas où la source est la raison d'être de la zone. Dans les autres cas, la traçabilité à la maille écran suffit.

---

---

## Exemple — Vue session MJ

> Cet exemple instancie le gabarit sur l'écran le plus riche du MVP.
> Il démontre que le gabarit tient sur un écran à trois modes, avec partage, avec châssis mode local.
> Il ne constitue pas un wireframe (fiche dédiée) — aucune décision nouvelle par rapport au corpus.
> Sources : UC-06 ; UC-07 ; UC-08 ; UC-14 ; US-06-01 à US-06-10 ; UJ-UC-06 ; AR-04 ; AR-09 ; AR-11 ; AR-12 ; NFR-ACC-02 ; NFR-ACC-04 ; châssis S7.

---

### En-tête de fiche

```
Nom          : Vue session MJ
Surface      : MJ
Contexte     : session
Type d'espace: CAMPAIGN | ONE_SHOT (réservé aux espaces partagés — AR-01 ; AR-14)
Forme cible  : grand écran + tablette (mobile : pensé dans la structure, non implémenté au MVP — AR-07)
Traçabilité  : UC-06, UC-07, UC-08, UC-14 ; US-06-01 à US-06-10 ; UJ-UC-06
```

---

### Noyau obligatoire

#### Intention

```
Intention : le MJ pilote la session depuis un tableau de bord configurable qui lui donne accès à ses
            dossiers, à ses notes de session et au partage de documents vers les joueurs — sans quitter
            la surface et sans interrompre le rythme de la table.
```

---

#### Zones et hiérarchie

```
[ INDICATEUR DE STATUT DE SESSION ]
  type     : châssis
  rôle     : matérialise le mode courant de la vue session (configuration / LIVE / consultation CLOSED)
             de façon omniprésente, quelle que soit la position de défilement
  priorité : co-présent-jamais-masqué
  visibilité : MJ seul
  ancrage  : AR-09 ; UJ-UC-06 §friction (« Statut CLOSED peu visible »)

[ INDICATEUR DE PARTAGE ]
  type     : châssis
  rôle     : récapitule en permanence ce que les joueurs voient en ce moment —
             distinct du panneau des documents épinglés (un document épinglé peut ne pas être partagé)
  priorité : co-présent-jamais-masqué
  visibilité : MJ seul
  ancrage  : AR-12 ; châssis S7 §Indicateur de partage

[ ZONE DE NOTES ]
  type     : principal
  rôle     : zone de saisie des notes de session — co-présente et jamais masquée par défilement,
             quelle que soit la configuration des panneaux
  priorité : co-présent-jamais-masqué
  visibilité : MJ seul (notes créées en GM_ONLY par défaut)
  ancrage  : AR-04 ; UJ-UC-06 §friction (« Zone de saisie non visible sans scroll »)

[ PANNEAUX DE DOSSIERS ]
  type     : principal
  rôle     : affiche le contenu des dossiers mis en avant par le MJ dans sa configuration ;
             chaque panneau présente titre et repère de type pour permettre l'accès rapide
             même dans un dossier dense (AR-04 — défaut épuré mais informatif)
  priorité : secondaire-configurable
  visibilité : MJ seul
  ancrage  : AR-04 ; UC-06 ; US-06-01

[ PANNEAU DES DOCUMENTS ÉPINGLÉS ]
  type     : favoris
  rôle     : liste les documents explicitement épinglés par le MJ pendant la session ;
             l'épinglage est local à la session et n'affecte pas la visibilité côté joueur
  priorité : principal
  visibilité : MJ seul
  ancrage  : AR-12 ; UC-06 ; UC-08 ; glossaire §Documents épinglés

[ BARRE DE RECHERCHE ]
  type     : châssis
  rôle     : accélérateur de navigation omniprésent — titre seul au MVP ;
             les résultats s'ouvrent dans un panneau latéral sans interrompre le contexte (AR-11)
  priorité : co-présent-jamais-masqué
  visibilité : MJ seul
  ancrage  : UC-14 ; AR-11 ; UJ-UC-06 §Opportunités UX

[ PANNEAU LATÉRAL DE RÉSULTATS DE RECHERCHE ]
  type     : latéral
  rôle     : affiche les résultats de recherche sans remplacer le contexte courant de la vue session
  priorité : secondaire-configurable (apparaît à la saisie, disparaît à la fermeture)
  visibilité : MJ seul
  ancrage  : AR-11 ; NFR-PERF-04

[ AFFORDANCE DE GÉNÉRATION DE LIEN D'INVITATION ]
  [SOUS-SPÉCIFIÉ — S4 §Surface MJ — Accès ; AR-10] placement non figé :
    l'affordance est logée dans la vue session OU la vue campagne — non tranché.
    Cette zone ne figure ici qu'à titre de traçabilité ; son appartenance à cet écran
    n'est pas une décision (S4 §Surface MJ — Accès, AR-10).
  rôle     : permet au MJ de générer le lien de session ponctuel à partager aux joueurs
  visibilité : MJ seul
  ancrage  : UC-06 (lien ponctuel) ; UC-08 ; UC-11 A4 (fraction Must — AR-10)
```

---

#### Ce que l'utilisateur peut faire

```
- [AR-09 ; UC-06 §Déclencheur] lancer la session → la vue bascule en mode LIVE ;
  la session passe au statut LIVE ; les joueurs peuvent rejoindre via lien

- [AR-09 ; UC-06 A5] reprendre une session en cours → la vue s'ouvre directement en mode LIVE
  si une session est déjà au statut LIVE ; pas de reconfiguration nécessaire

- [AR-04 ; US-06-02] configurer les panneaux de dossiers → le MJ sélectionne et ordonne
  les dossiers mis en avant, sans lancer de session ; la configuration persiste entre les modes

- [AR-12 ; UC-08] partager un document en session LIVE → le document devient visible par les
  joueurs (PUBLIC) et s'auto-épingle dans le panneau des documents épinglés ;
  l'indicateur de partage se met à jour immédiatement

- [AR-12 ; UC-08] retirer le partage d'un document → le document disparaît de la vue joueur
  en temps réel (symétrique) ; il reste épinglé dans le panneau du MJ

- [AR-12 ; UC-06] épingler un document sans le partager → le document est accessible
  rapidement dans le panneau des épinglés ; il reste invisible côté joueur

- [UC-07] créer un document ou une note de session à la volée → panneau de création rapide
  s'ouvre (titre seul obligatoire) ; le document créé est automatiquement épinglé

- [UC-14 ; AR-11] rechercher dans le contenu de l'espace → titre seul au MVP ;
  résultats affichés dans un panneau latéral sans interrompre le contexte de la vue session

- [UC-06 ; RB-06-21] terminer la session → la session passe au statut CLOSED ;
  la machine d'états est unidirectionnelle (pas de retour en LIVE)

- [UC-06 ; UC-11 A4] générer un lien d'invitation → lien de session ponctuel disponible
  à partager aux joueurs (fraction Must — AR-10)
```

---

#### États

```
état vide (mode configuration, aucune session active, aucune configuration enregistrée) :
  la vue s'affiche avec le défaut épuré — zone de notes visible, panneaux de dossiers absents
  ou réduits au minimum ; invite à configurer ou à lancer directement (AR-04)

état chargé (configuration enregistrée, mode quelconque) :
  les panneaux de dossiers sélectionnés s'affichent selon la configuration persistée (AR-09) ;
  la zone de notes est co-présente ; l'indicateur de statut reflète le mode courant

état erreur (perte de connexion en mode cloud) :
  la notification d'état de synchronisation apparaît sans bloquer la saisie (châssis S7) ;
  le MJ peut continuer à prendre des notes et à naviguer pendant la reconnexion
```

---

#### Accessibilité (delta)

Les garanties transversales (focus clavier visible, ordre de navigation cohérent) sont portées par le châssis (`zoning.md §S7 §Accessibilité transversale`). Cette section couvre uniquement ce qui est spécifique à la vue session MJ.

```
annonce sans action :
  - changement de mode (configuration → LIVE → consultation CLOSED) : annoncé assistivement
    sans que le MJ déplace son focus — NFR-ACC-02 (changements d'état dynamiques)
  - apparition d'un résultat de recherche dans le panneau latéral : annoncé assistivement — NFR-ACC-02
  - basculement de la visibilité d'un document (partagé / non partagé) : annoncé assistivement — NFR-ACC-02 ; AR-06
  - changement d'état de synchronisation (reconnexion, stockage sous pression) : annoncé assistivement — châssis S7 ; NFR-ACC-02

hiérarchie de lecture à distance :
  priorité 1 — indicateur de statut de session (mode courant — lecture à distance de la table)
  priorité 2 — zone de notes (co-présente, accès immédiat)
  priorité 3 — panneaux de dossiers (contenu mis en avant par le MJ)
  priorité 4 — panneau des documents épinglés
  priorité 5 — barre de recherche
  source : NFR-ACC-04 (usage à distance normale de l'écran pendant une session à table)
```

---

#### Sources

```
Sources : UC-06 ; UC-07 ; UC-08 ; UC-14 ;
          US-06-01 à US-06-10 ; UJ-UC-06 ;
          AR-04 ; AR-09 ; AR-11 ; AR-12 ;
          NFR-ACC-02 ; NFR-ACC-04 ; NFR-PERF-01 ; NFR-PERF-04 ;
          châssis S7 (zoning.md §S7)
```

---

### Modes (déclenché — AR-09 : l'écran a 3 modes)

---

#### Mode : configuration

```
Déclencheur : aucune session LIVE n'est active dans cet espace ;
              le MJ arrive sur la vue session depuis la vue campagne

Zones actives :
  - indicateur de statut de session (affiche : « aucune session active »)
  - zone de notes (accessible pour prise de notes préparatoires)
  - panneaux de dossiers (configurables — AR-04 ; US-06-02 : configuration accessible
    sans lancer de session)
  - barre de recherche

Zones désactivées :
  [DÉSACTIVÉ] indicateur de partage — condition de réactivation : passage en mode LIVE
  [DÉSACTIVÉ] affordance de partage de document — condition : passage en mode LIVE

Zones absentes par nature :
  [ABSENT PAR NATURE] affordance « Terminer la session » — il n'y a pas de session en cours

Affordances spécifiques :
  - [AR-09 ; UC-06 §Déclencheur] lancer la session → basculement en mode LIVE
  - [AR-04 ; US-06-02] configurer les panneaux → sélection et ordonnancement des dossiers
    mis en avant ; la configuration est persistée (AR-09 : disposition persistée entre modes)
```

---

#### Mode : LIVE

```
Déclencheur : le MJ a lancé la session depuis ce mode configuration ou depuis la vue campagne ;
              la session est au statut LIVE

Zones actives : toutes les zones listées dans « Zones et hiérarchie »

Affordances spécifiques :
  - [AR-12 ; UC-08] partager un document → visible par les joueurs + auto-épinglage (AR-12 §Raison d'être)
  - [AR-12 ; UC-08] retirer le partage → disparition symétrique côté joueur en temps réel (AR-06)
  - [UC-07] créer à la volée → panneau de création rapide (titre seul obligatoire) ;
    document créé automatiquement épinglé
  - [UC-06 ; RB-06-21] terminer la session → basculement en mode consultation CLOSED ;
    unidirectionnel (pas de retour en LIVE — machine d'états RB-06-21)

Note sur la frontière de confidentialité :
  L'annonce de l'apparition d'un document partagé côté vue joueur est distincte ;
  cette fiche ne rend pas la vue joueur — NFR-CONF-01 (AR-06 ; NFR-ACC-02).
```

---

#### Mode : consultation CLOSED

```
Déclencheur : le MJ a terminé la session ; la session est au statut CLOSED ;
              annotations rétroactives encore possibles (résumé, notes MJ)

Zones actives :
  - indicateur de statut de session (affiche : « session terminée »)
  - zone de notes (annotations rétroactives MJ possibles en CLOSED)
  - panneaux de dossiers (lecture seule — navigation possible, pas de modification)
  - panneau des documents épinglés (lecture seule)
  - barre de recherche

Zones désactivées :
  [DÉSACTIVÉ] affordance de partage — condition de réactivation : aucune (CLOSED ne repasse pas en LIVE)

Zones absentes par nature :
  [ABSENT PAR NATURE] affordance « Lancer la session » — la machine d'états est unidirectionnelle ;
    une session CLOSED ne retourne pas à LIVE (RB-06-21)
  [ABSENT PAR NATURE] auto-épinglage au partage — l'auto-épinglage (UC-08 A3) est réservé au
    mode LIVE ; un partage rétroactif depuis CLOSED ne déclenche pas d'épinglage automatique

Affordances spécifiques :
  - [UC-07 ; S4 §Vue session MJ] créer un document ou une note à la volée de façon rétroactive
    → le panneau de création rapide s'ouvre (titre seul obligatoire) — UC-07 A5
  - [UC-08 A3 ; AR-12] partager un document rétroactivement → le document devient visible par
    les joueurs (PUBLIC) SANS auto-épinglage (l'auto-épinglage est réservé au mode LIVE)
  - [UC-06 ; glossaire §Session] archiver la session → la session passe au statut ARCHIVED,
    lecture seule complète (aucune modification possible)

Placement de l'affordance de génération de lien d'invitation :
  [SOUS-SPÉCIFIÉ — S4 §Surface MJ — Accès ; AR-10] l'affordance est logée dans la vue session
    OU dans la vue campagne — non tranché. Son appartenance à cet écran (et donc à ce mode)
    n'est pas décidée ; elle ne peut donc être ni désactivée ni absente par nature dans ce mode.

Éléments différés (S8) :
  [HORS-MVP — moscow.md §Won't Have] clôture formelle de session (procédure de clôture guidée)
```

---

### Partage / visibilité (déclenché — AR-03 et AR-12 : la vue session MJ touche la frontière joueur)

```
Règle de séparation : la vue session MJ n'expose aucune structure MJ à la vue joueur ;
  les documents non partagés (GM_ONLY, PLAYER_PRIVATE) sont invisibles depuis la vue joueur,
  quelle que soit la tentative d'accès — NFR-CONF-01 ; AR-03.

Indicateur de partage :
  Récapitule en permanence la liste des documents visibles par les joueurs en ce moment.
  Distinct du panneau des documents épinglés (un document peut être épinglé sans être partagé).
  Source : AR-12 ; châssis S7 §Indicateur de partage.

Affordances de partage et d'épinglage :
  - [AR-12 ; UC-08] partager → le document devient PUBLIC (visible par les joueurs) ;
    en mode LIVE, l'auto-épinglage est déclenché (sens unique : partage → épingle, jamais l'inverse)
  - [AR-12 ; UC-06] épingler → le document est ajouté au panneau des épinglés ;
    l'épinglage seul ne rend pas le document visible côté joueur
  - [AR-12] distinction partager ≠ épingler : deux affordances visuellement distinctes sur chaque document ;
    l'indicateur de partage reflète uniquement ce qui est PUBLIC ; le panneau des épinglés
    reflète uniquement ce qui est épinglé — les deux états sont indépendants et co-visibles
```

---

### Mode local (déclenché — surface MJ)

```
Bandeaux présents (châssis standard — voir zoning.md §S7 §Châssis mode local) :
  - bandeau de durabilité : non bloquant ; signale que les données locales ne bénéficient pas
    d'une garantie de conservation permanente ; propose la création d'un compte
  - bandeau de confidentialité : non bloquant ; signale l'absence de protection par identifiants ;
    propose la création d'un compte

Fonctions cloud désactivées sur la vue session MJ en mode local :
  - [AR-13 ; UC-01 A1] partager des documents avec les joueurs → invite contextuelle :
    « Partager avec des joueurs nécessite un compte — créer un compte en un geste »
  - [AR-13] génération du lien d'invitation → même invite contextuelle

Fonctions disponibles localement (non désactivées) :
  La prise de notes (zone de notes), la navigation dans les dossiers locaux,
  la recherche, l'épinglage et la création à la volée restent disponibles
  sans compte — elles ne nécessitent pas de connexion au service cloud.
  Seuls le partage et la génération du lien d'invitation sont cloud-dépendants.
  Source : AR-13 ; UC-01 §Règles métier.
```

---

### Hors-périmètre / différé / sous-spécifié (déclenché — éléments S8 et S9)

```
Éléments différés (S8) :
  [HORS-MVP — UC-11 scénario nominal] écran Membres complet (révocations, associations joueur-personnage)
  [HORS-MVP — UC-08 §Règles métier ; vision §5bis] partage sélectif par joueur ou personnage
  [HORS-MVP — moscow.md §Won't Have] clôture formelle de session
  [HORS-MVP — moscow.md §Won't Have] table visuelle

Éléments sous-spécifiés (S9) :
  [SOUS-SPÉCIFIÉ — S9 §Notification active côté joueur] la notification active (signalement
    sonore ou visuel du partage côté joueur) n'est pas décidée pour le MVP — angle d'interview
    (US-06 §Questions ouvertes)
  [SOUS-SPÉCIFIÉ — S9 §Persistance des notes invité inter-sessions] la mécanique de récupération
    des notes PLAYER_PRIVATE d'un invité via un nouveau lien est évoquée dans UC-06 §Règles métier
    mais non entièrement spécifiée — relève de remédiation corpus
```

*Contre-exemple d'absence au niveau section — pour un écran d'espace personnel : les sections conditionnelles « Modes » et « Partage / visibilité » ne se déclenchent pas (ni modes de session, ni frontière joueur) ; elles n'apparaissent donc pas dans la fiche — on ne les note jamais « S.O. » (AR-14).*
