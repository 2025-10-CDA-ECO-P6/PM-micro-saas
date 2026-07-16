# État de navigabilité du parcours — prototype cliquable MVP Haversack

> Audience : mainteneur du graphe de navigation (graphe amont autoritatif :
> `docs/conception/interface/zoning.md §S3` ; matérialisation concrète : les
> pages HTML statiques de `docs/presentation/prototype/`, une par écran, cf.
> `_SITE-FORMAT.md`) et relecteur cohérence conception.
>
> Historique de consolidation : 2026-07-03 (rendu multi-états, compagnon JS
> runtime) → 2026-07-09 première mise à jour (site de pages HTML statiques
> reliées par de vrais `<a href>`, mais encore audité comme un miroir du
> graphe `EDGES` : une affordance sans arête modélisée restait un signal
> d'écart) → **2026-07-09, présente mise à jour (parcours complet)** : le
> prototype assume désormais des transitions hors-`EDGES` au nom du
> parcours complet — les cibles non maquettées reçoivent un placeholder
> `_stub/*.html` plutôt que de rester inertes, et la puce de compte devient
> un menu déroulant (Profil / Se déconnecter). Ce changement de posture du
> prototype **change l'objet de cet audit** (§1) et requalifie la quasi-
> totalité de ce qui était listé comme « affordance manquante » à la
> consolidation précédente — vérifié ligne par ligne contre le HTML actuel
> des 17 pages d'écran + 2 pages `_etat/` + 5 pages `_stub/`.
>
> Ce document audite l'**état de navigabilité du parcours** matérialisé
> par le prototype — distinct d'un audit de conformité des planches
> (zones, états, RB/AR couverts).

---

## 1. Objet & méthode

**Ce document n'audite plus « telle arête `EDGES` a-t-elle une
affordance ? » comme critère de conformité.** Ce critère avait déjà cessé
d'être pertinent dès lors que le prototype a cessé d'être un miroir fidèle
du graphe `EDGES` : il matérialise désormais une **expérience de parcours
complète** — toute affordance représentant une transition logique du
parcours y est câblée, y compris au-delà de ce que `EDGES` modélise (cf.
`_SITE-FORMAT.md §6`).

L'objet de cet audit est donc reformulé : **l'état de navigabilité du
parcours**. Pour chaque écran, pour chaque affordance visible, ce document
constate l'une de ces trois situations, jamais une conformité à `EDGES` :

- **câblée vers un écran réel** — l'affordance porte un `<a href>` réel
  vers une page maquettée du prototype (§2) ;
- **câblée vers un placeholder** — l'affordance porte un `<a href>` réel
  vers `_stub/<nom>.html`, la cible n'étant pas (encore) maquettée (§3) ;
- **volontairement inerte** — l'affordance reste un `<span>`/`<button>`
  sans `href`, pour l'une des trois raisons admises par
  `_SITE-FORMAT.md §6` : interaction strictement in-page, repère de mode
  de la vue session (AR-09), ou arête `EDGES` de type `"effect"` interdite
  de navigation (AR-03) (§4).

Les écarts de **cohérence de conception** encore pertinents (divergences
de libellé fiche↔graphe, trous du graphe amont `zoning.md §S3`) restent
consignés (§5) — mais **comme observations pour le mainteneur du graphe**,
plus comme des affordances manquantes dans le prototype : elles ont été
comblées côté site, `EDGES` ne les modélise simplement pas encore.

**Méthode** : lecture manuelle du HTML de chaque écran et de son
commentaire d'audit inline (quand présent), classement dans l'une des
trois situations ci-dessus. Aucun mécanisme runtime de détection
d'affordances orphelines n'existe (plus de surbrillance, plus de
`console.warn`, plus de toast « cible hors pilote ») : ces comportements
appartenaient au compagnon runtime d'un rendu antérieur, retiré avec lui.
Cette lecture manuelle a ses propres angles morts, listés en §6.

**Régénération / complément** : ce document est une synthèse figée à la
date ci-dessus. Toute nouvelle page livrée, tout nouveau placeholder
ajouté, ou tout écart corrigé dans `EDGES`, doit être répercuté ici en
relisant le HTML de la page concernée et le tableau `EDGES` à jour — il
n'existe pas de génération automatique de ce fichier.

Sources exploitées : le HTML des 17 pages d'écran (`sv1-transversaux/`,
`sv2-entree-espace/`, `sv3-preparation/`, `sv4-espace-personnel/`,
`sv5-joueur/`, `sv-session/`), `index.html`, les 2 pages `_etat/`, les 5
pages `_stub/`, `overlay.js`, le graphe amont
`docs/conception/interface/zoning.md §S3` (`EDGES`/`NODES`),
`_SITE-FORMAT.md` (contrat de format actuel, §§1/3/4/6/8).

---

## 2. Transitions câblées vers un écran réel

Toutes les transitions de navigation portées par une arête `EDGES
type:"nav"` retrouvent aujourd'hui une affordance réelle câblée vers un
écran existant du prototype, **sauf trois** (détaillées en §4 : deux pour
raison structurelle intra-écran, une — `Tableau→SuppressionRGPD` —
accessible uniquement par un chemin indirect, cf. note ci-dessous).

| Arête / transition | Écran source | Affordance | Cible |
|---|---|---|---|
| `Accueil→ModeLocal` | Accueil | « Commencer › » | `_etat/mode-local.html` |
| `Accueil→Inscription` | Accueil | « Créer un compte » | Inscription |
| `Accueil→Connexion` | Accueil | « J'ai déjà un compte — Se connecter » | Connexion |
| `Inscription→Tableau` | Inscription | « Créer mon compte » | Tableau de bord |
| `Connexion→Tableau` | Connexion | « Se connecter » / « Continuer avec un fournisseur » | Tableau de bord |
| `GateMigration→Tableau` | GateMigration | « Migrer 2 espaces › » et « Plus tard » (2 boutons réels, même cible — granularité `EDGES` non distinguée, cf. note) | Tableau de bord |
| `Tableau→NouvelEspace` | Tableau de bord | « + Nouvelle campagne » | Création d'espace |
| `Tableau→EspacePerso` | Tableau de bord | « Ouvrir › » (Espace personnel) | Vue espace personnel |
| `Tableau→Campagne` | Tableau de bord | « Ouvrir l'espace › » | Vue campagne |
| `Tableau→VSLIVE` | Tableau de bord | « Reprendre la session › » | Vue session MJ |
| `Tableau→Profil` | Tableau de bord | menu déroulant compte → « Profil » | Profil |
| `EspacePerso→EditeurDocPerso` | Vue espace personnel | « + Nouveau document » | Éditeur de document |
| `EspacePerso→RecherchePerso` | Vue espace personnel | barre de recherche → panneau latéral `#search-results` | Éditeur de document (par ligne de résultat) |
| `Campagne→EditeurDoc` | Vue campagne | chaque ligne de document (dossier Personnages) | Éditeur de document |
| `Campagne→EditeurScenario` | Vue campagne | ligne « La Forge Maudite » (dossier Scénarios, réceptacle d'accès unique) | Éditeur de scénario |
| `Campagne→Recherche` | Vue campagne | barre de recherche → panneau latéral `#search-results` | Éditeur de document (par ligne de résultat) |
| `Campagne→ParamsCampagne` | Vue campagne | « Inviter des joueurs › » et « Paramètres » (2 boutons réels, même cible) | Paramètres de campagne |
| `Campagne→VSConfig` / `Campagne→VSLIVE` | Vue campagne | « Reprendre la session › » (bouton unique — « Configurer la vue » retiré, une session en cours ne se configure pas, elle se reprend ; les deux arêtes menaient à la même cible) | Vue session MJ |
| `VSLIVE→PanneauCreation` | Vue session MJ | « + créer à la volée » (ouvre `#quick-create-modal`, sur-couche in-page — plus une navigation inter-page, `PanneauCreation` n'est plus une page) | modale in-page |
| `SaisieNom→VueJoueur` | Accès par lien (saisie du nom) | « Rejoindre › » | Vue joueur |
| `VueJoueur→Inscription` | Vue joueur | « Créer un compte » (seule traversée joueur→MJ documentée par `EDGES`, AR-03) | Inscription |
| `ModeLocal→EspacePerso` / `→GateMigration` / `→Tableau` | `_etat/mode-local.html` | « Ouvrir › » par branche | Vue espace personnel / GateMigration / Tableau de bord |
| `LienSession→SaisieNom` / `→ErreurAcces` | `_etat/lien-session.html` | « Ouvrir › » par branche | Accès par lien / Erreur d'accès |
| retour Profil↔Tableau | Profil | « ‹ tableau de bord » | Tableau de bord |
| retour Inscription↔Connexion | Inscription / Connexion | « Se connecter » / « Créer un compte » | Connexion / Inscription |
| Recherche→EditeurDoc / RecherchePerso→EditeurDocPerso | panneau latéral `#search-results` (Vue campagne / Vue session MJ / Vue espace personnel) | chaque ligne de résultat | Éditeur de document |
| Profil→SuppressionRGPD | Profil | « Supprimer… » (zone actions sensibles) | Suppression RGPD |
| retour SuppressionRGPD↔Profil | Suppression RGPD | « ‹ profil » | Profil |
| SuppressionRGPD→Accueil | Suppression RGPD | « Supprimer définitivement mon compte » | Accueil |
| NouvelEspace→Tableau (aller + retour) | Création d'espace | « Créer la campagne » (simule la postcondition UC-02) et « ‹ tableau de bord » | Tableau de bord |
| ParamsCampagne→VSConfig | Paramètres de campagne | « Configurer dans la vue session › » | Vue session MJ |
| retour VSConfig/VSLIVE/VSClosed→Campagne | Vue session MJ | « ‹ vue campagne » | Vue campagne |
| SaisieNom→Inscription | Accès par lien (saisie du nom) | « Créer un compte pour conserver mes notes » (deuxième traversée joueur→MJ, décision opérateur AR-03) | Inscription |

**Note — `Tableau→SuppressionRGPD`** : cette arête n'a pas d'affordance
directe distincte sur le tableau de bord (le menu déroulant compte ne
porte que « Profil » et « Se déconnecter »). Elle reste néanmoins
atteignable par un chemin réel à deux sauts : Tableau de bord → (menu)
Profil → « Supprimer… » → Suppression RGPD. Ce n'est plus une affordance
manquante au sens strict (le parcours complet l'atteint), mais ce n'est
pas non plus un câblage direct de l'arête `EDGES` telle qu'elle est
documentée — signalé au mainteneur du graphe en §5.

**Note — granularité `GateMigration→Tableau` et `Campagne→ParamsCampagne`** :
dans les deux cas, deux affordances réelles distinctes convergent vers la
même cible sans que `EDGES` distingue laquelle des deux actions elle
représente (migrer vs différer ; inviter vs paramétrer). Câblage réel
sans ambiguïté de rendu — seule la granularité du graphe amont reste à
préciser (§5).

---

## 3. Transitions câblées vers un placeholder `_stub/`

Cinq affordances mènent vers un placeholder « à venir » plutôt que vers un
écran maquetté (dispositif documenté en `_SITE-FORMAT.md §8`) :

| Affordance | Écran source | Placeholder |
|---|---|---|
| « En savoir plus » (message stockage local) | Accueil | `_stub/aide.html` |
| « Voir l'offre Pro » (invite quota 3/3 atteint) | Tableau de bord | `_stub/offre-pro.html` |
| « Mot de passe oublié ? » | Connexion | `_stub/reinitialisation-mdp.html` |
| « Archiver… » (zone d'archivage de l'espace) | Paramètres de campagne | `_stub/espace-archive.html` |
| « Exporter… » (zone d'export de l'espace) | Paramètres de campagne | `_stub/export-espace.html` |

Chacune de ces cinq affordances était, à la consolidation précédente
(2026-07-09, première mise à jour), soit un `<span>` inerte sans cible
(« Voir l'offre Pro »), soit une affordance non couverte par l'audit
antérieur (« En savoir plus », « Mot de passe oublié ? », « Archiver… »,
« Exporter… » n'y figuraient pas). Les cinq sont désormais des `<a href>`
réels — aucune ne reste un cul-de-sac ni un signal d'écart : chaque
placeholder porte un lien de retour vers l'écran d'origine
(`_SITE-FORMAT.md §8`).

---

## 4. Interactions volontairement inertes

Ce que le rendu laisse délibérément non cliquable, classé selon les trois
familles admises par `_SITE-FORMAT.md §6` — plus une « affordance
manquante » depuis que le parcours complet est assumé, mais un choix
explicite documenté.

### 4.1 Interactions strictement in-page

| Interaction | Écran | Motif |
|---|---|---|
| Noms de dossiers (arborescence) | Vue campagne, Vue espace personnel | Colonne embarquée `.split-2col__aside` toujours affichée — la sélection d'un dossier filtre en place, ce n'est pas une transition inter-écran (`NavDossiers`/`NavDossiersPerso` ne sont plus des pages, cf. `_SITE-FORMAT.md §1`). |
| « + dossier » | Vue campagne, Vue espace personnel | Action de création in-page, aucune arête de navigation. |
| « Enregistrer » (nom d'affichage) | Profil | Effet d'édition, pas une arête nav. |
| « + note » | Vue joueur | Création de note in-page. |
| « déplacer › » (lignes de document) | Vue espace personnel | Action in-page ; l'ouverture d'un document existant emprunterait la même arête `EspacePerso→EditeurDocPerso` que « + Nouveau document » — non dupliquée pour éviter un double câblage de la même arête. |
| « consulter en place › » (dossiers mis en avant) | Vue session MJ | Consultation en place (AR-04), aucune arête de navigation inter-écran modélisée. |

### 4.2 Repères de mode de la vue session (AR-09)

La vue session MJ matérialise ses trois modes (`VSConfig`/`VSLIVE`/
`VSClosed`) sur une seule page, par un repère textuel non interactif
plutôt que trois rendus séparés :

- le statut « Configuration → **LIVE — session en cours** → Consultation
  CLOSED » est un repère non interactif — machine d'états
  unidirectionnelle (AR-09) ;
- « Terminer » (porterait `VSLIVE→VSClosed`) reste un `<span>` non
  cliquable : la cible serait le même fichier physique (transition
  intra-écran), non modélisable par un `<a href>` vers une autre page.
  Conséquence directe et assumée du choix éditorial « une page = un mode
  interactif affiché » — pas un angle mort d'audit (§6) ;
- symétriquement, aucune affordance ne porte `VSConfig→VSLIVE` (« Lancer
  la session › ») sur cette même page : seul l'état LIVE y est rendu.

Ces deux arêtes (`VSConfig→VSLIVE`, `VSLIVE→VSClosed`) restent donc les
deux seules transitions `EDGES type:"nav"` sans câblage réel ni
placeholder — pour la raison structurelle ci-dessus, pas par omission.

### 4.3 Arêtes `EDGES` de type `"effect"` interdites de navigation (AR-03)

| Interaction | Écran | Arête `EDGES` (effect) |
|---|---|---|
| « Copier » (lien d'invitation) | Paramètres de campagne | `ParamsCampagne→LienSession` — génération de lien, jamais une navigation. |
| « ⧉ Lien de session » (indicateur de partage) | Vue session MJ | `VSLIVE→LienSession` — génération de lien, jamais une navigation. |
| « Archiver » (repère effect distinct de la zone d'archivage de Paramètres de campagne, §3) | Vue session MJ (mode CLOSED) | `VSClosed→Archive` — n'a aucune représentation, même non cliquable, sur cette page ; serait de toute façon exclue d'un câblage `<a>` (AR-03). |

---

## 5. Observations de cohérence de conception préservées (pour le mainteneur du graphe)

Ces constats restent valides et utiles — **ils ne sont plus des
affordances manquantes dans le prototype** (le parcours complet les a
comblés, cf. §2-§3), mais le graphe amont `EDGES` ne les modélise pas
encore tel qu'il est câblé aujourd'hui. Reste à trancher, côté graphe :

- **Arêtes réellement câblées mais absentes d'`EDGES`** : `Recherche →
  EditeurDoc` / `RecherchePerso → EditeurDocPerso` (panneau de résultats,
  câblé de longue date) ; retours Inscription↔Connexion, Profil↔Tableau,
  SuppressionRGPD↔Profil, VSConfig/LIVE/Closed↔Campagne ;
  `SuppressionRGPD→Accueil` ; `NouvelEspace→Tableau` ; `ParamsCampagne→
  VSConfig` ; `SaisieNom→Inscription`. Chacune est aujourd'hui une
  transition réelle du site (§2) — la question pour le mainteneur du
  graphe n'est plus « faut-il l'ajouter au prototype » (fait) mais « faut-
  il l'ajouter à `EDGES` pour que le graphe reflète le site ».
- **Granularité d'arête à préciser** : `GateMigration→Tableau` (recouvre
  « Migrer… » et « Plus tard ») et `Campagne→ParamsCampagne` (recouvre
  « Inviter des joueurs › » et « Paramètres ») — deux actions réelles
  distinctes par arête `EDGES`, câblées sans ambiguïté côté site (§2).
- **`Tableau→SuppressionRGPD`** : câblée dans le parcours complet
  uniquement via un chemin indirect (Tableau → Profil → Suppression RGPD,
  cf. §2 note) — à arbitrer : ajouter une affordance directe sur le
  tableau de bord, ou documenter dans `EDGES` que cette arête est
  atteinte par composition d'arêtes plutôt que directement.
- **`NavDossiers` / `NavDossiersPerso`** : ces nœuds ne sont plus des
  pages séparées dans l'architecture du site (colonne embarquée toujours
  affichée, cf. `_SITE-FORMAT.md §1`) — si `EDGES` modélise encore
  `Campagne→NavDossiers` / `EspacePerso→NavDossiersPerso` comme des
  transitions vers un écran distinct, ces arêtes devraient être retirées
  ou requalifiées en interaction in-page.
- **Tension AR-03 `SaisieNom→Inscription`** : résolue côté site (décision
  opérateur, câblée — §2), documentée comme la seconde des deux seules
  traversées joueur→MJ autorisées (`_SITE-FORMAT.md §6`). À reporter dans
  `EDGES` si le graphe doit refléter cette décision.
- **Divergences de libellé `EDGES` ↔ page rendue** (inchangées depuis la
  consolidation initiale, toujours vérifiées contre le HTML actuel) :

  | Arête `EDGES` | Libellé `EDGES` | Libellé réel (page actuelle) |
  |---|---|---|
  | `Accueil → ModeLocal` | « Démarrer sans compte » | « Commencer › » |
  | `Tableau → VSLIVE` | « Reprendre session en cours » | « Reprendre la session › » |
  | `Campagne → VSConfig` | « Accéder à la vue session » | « Configurer la vue » *(bouton retiré, cf. §2 — divergence caduque si `EDGES` n'est pas mis à jour vers « Reprendre la session › »)* |
  | `Campagne → VSLIVE` | « Lancer la session » | « Reprendre la session › » |
  | `VSLIVE → PanneauCreation` | « Panneau création rapide à la volée (UC-07) » | « + créer à la volée » |
  | `VueJoueur → Inscription` | « Créer un compte (seule traversée joueur→MJ — UC-09 A2) » | « Créer un compte » (la parenthèse explicative de `EDGES` n'apparaît pas dans le DOM rendu) |

  Variantes de libellé intra-wireframe (hors comparaison à `EDGES`, qui ne
  porte pas de label pour ces arêtes), conservées en trace d'audit :

  | Arête | Libellé canonique retenu | Variante(s) non canonique(s) relevée(s) |
  |---|---|---|
  | `Accueil → ModeLocal` | « Commencer › » (première visite) | « Reprendre mon travail › » (état retour — données locales détectées, UC-01 A2) |
  | `Accueil → Connexion` | « J'ai déjà un compte — Se connecter » (première visite) | « Se connecter » (tablette, libellé court) |
  | `Inscription → Tableau` | « Créer mon compte » (état vide) | « Créer mon compte et continuer » (depuis une invite contextuelle, UC-01 A1) |

---

## 6. Angles morts de la méthode d'audit

Ce que la lecture manuelle ne couvre pas mécaniquement, pour ne pas donner
une fausse impression d'exhaustivité :

- **Aucun filet automatique.** Il n'existe plus de mécanisme runtime de
  détection d'affordances orphelines (plus de surbrillance, plus de
  `console.warn`) : la couverture de ce document dépend entièrement de la
  lecture manuelle consolidée ci-dessus.
- **Page unique portant 3 nœuds du graphe.**
  `sv-session/vue-session-mj/vue-session-mj.html` matérialise
  `VSConfig`/`VSLIVE`/`VSClosed` par un repère textuel plutôt que trois
  rendus distincts — conséquence directe du choix éditorial « une page =
  un mode interactif affiché », pas un défaut de la méthode d'audit (§4.2).
- **Correspondance placeholder ↔ arête `EDGES`** : les cinq placeholders
  de §3 répondent à des affordances réelles du rendu, pas nécessairement
  à des arêtes `EDGES` nommément identifiées (certaines, comme « En savoir
  plus » ou « Archiver… »/« Exporter… », n'étaient pas couvertes par les
  consolidations précédentes de cet audit) — ce document constate le
  câblage réel, il n'établit pas une correspondance certaine avec un
  identifiant d'arête du graphe amont pour ces cas.

---

## 7. Récap

| Catégorie | Compte | Détail |
|---|---|---|
| Arêtes `EDGES type:"nav"` câblées vers un écran réel | 32 / 34 | §2 — dont `Tableau→SuppressionRGPD` par chemin indirect (note §2) |
| Arêtes `EDGES type:"nav"` non câblées — raison structurelle intra-écran (AR-09) | 2 / 34 | `VSConfig→VSLIVE`, `VSLIVE→VSClosed` (§4.2) |
| Affordances hors `EDGES` câblées vers un écran réel | 12 | retours et transitions listés en §2 et repris comme observations en §5 |
| Affordances câblées vers un placeholder `_stub/` | 5 | §3 |
| Interactions volontairement inertes — in-page | 6 familles | §4.1 |
| Interactions volontairement inertes — effect AR-03 | 3 | §4.3 |

Ce décompte diffère de celui de la consolidation précédente (27 câblées /
7 non câblées, dont 5 pour absence d'affordance et 2 pour raison
structurelle) : l'écart tient à ce que les 5 arêtes alors « non câblées —
pas de bouton distinct trouvé » et les 14 affordances alors « orphelines
potentielles » sont, dans le parcours complet actuel, toutes câblées pour
de vrai — vers un écran réel ou vers un placeholder. Il ne reste plus que
les deux arêtes structurellement non câblables (§4.2) comme transitions
`EDGES type:"nav"` sans traduction réelle sur le site.

**Ce que le mainteneur du graphe devrait arbitrer** (repris de §5, pour
mémoire) : ajouter à `EDGES` les arêtes réellement câblées qu'il ne
modélise pas encore ; préciser la granularité de `GateMigration→Tableau`
et `Campagne→ParamsCampagne` ; arbitrer le chemin indirect de
`Tableau→SuppressionRGPD` ; retirer ou requalifier `Campagne→NavDossiers`
et `EspacePerso→NavDossiersPerso` ; harmoniser les libellés divergents de
§5 ; documenter la tension AR-03 `SaisieNom→Inscription` comme tranchée.
