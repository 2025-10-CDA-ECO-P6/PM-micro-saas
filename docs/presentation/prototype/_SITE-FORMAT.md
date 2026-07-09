# Contrat de format — site statique du prototype cliquable Haversack

> Ce document décrit l'architecture réelle du prototype cliquable : un site
> de pages HTML statiques, une par écran, reliées par de vrais `<a href>`,
> avec une feuille de design partagée (`site.css`) et un petit script local
> de sur-couches (`overlay.js`). Sert de référence à quiconque (agent ou
> humain) ajoute, modifie ou audite une page de ce site.
>
> Graphe de navigation amont autoritatif : `docs/conception/interface/zoning.md
> §S3` (nœuds) et `§S7` (châssis) — lecture seule, ce document n'en est
> qu'une matérialisation concrète, il ne le redéfinit pas.
>
> **Mise à jour 2026-07-09 (parcours complet)** : le prototype n'est plus un
> miroir fidèle du graphe `EDGES` (règle « lien réel seulement si arête
> `EDGES` modélisée, span inerte sinon ») — c'est désormais une expérience
> de parcours complète. Toute affordance représentant une transition
> logique du parcours est câblée en `<a href>` réel, y compris au-delà de
> ce que `EDGES` modélise ; les cibles non maquettées pointent vers un
> placeholder `_stub/*.html` « à venir » plutôt que de rester un `<span>`
> inerte (cf. §6 et §8, nouveaux). La puce de compte du châssis est
> désormais un menu déroulant (`.menu`/`data-menu`, cf. §3 et §4). Ce
> changement ne redéfinit pas le graphe amont (`zoning.md §S3` reste
> autoritatif) : il change ce que ce document prescrit pour le câblage du
> site qui le matérialise.

## 1. Organisation des fichiers

Racine (`docs/presentation/prototype/`) :

```
index.html       page d'entrée du site (choix de parcours)
site.css         feuille de design partagée — source unique de styles
overlay.js       mécanisme JS local de sur-couches (modale / panneau
                 latéral / menu déroulant)
_etat/           pages matérialisant les nœuds abstraits du graphe
  mode-local.html
  lien-session.html
_stub/           placeholders « à venir » pour les cibles non maquettées
  aide.html
  espace-archive.html
  export-espace.html
  offre-pro.html
  reinitialisation-mdp.html
<surface>/<slug>/<slug>.html    une page par écran (cf. §7)
```

6 dossiers de surface : `sv1-transversaux`, `sv2-entree-espace`,
`sv3-preparation`, `sv4-espace-personnel`, `sv5-joueur`, `sv-session` — 17
pages d'écran au total, plus les 2 pages `_etat/`, les 5 pages `_stub/` et
`index.html`.

**Deux nœuds du graphe ne sont plus des pages séparées** (réalignement
UI/UX) :
- `PanneauCreation` (panneau de création rapide, UC-07) est une **modale**
  intégrée à `sv-session/vue-session-mj/vue-session-mj.html` (déclencheur
  « + créer à la volée », disponible en mode LIVE et en consultation
  CLOSED) ;
- `Recherche` / `RecherchePerso` (recherche en préparation, UC-14) est une
  **barre de recherche de châssis + panneau latéral overlay**, présente à
  l'identique sur les trois surfaces MJ qui l'exposent :
  `vue-session-mj.html`, `vue-campagne.html`, `vue-espace-personnel.html`.

`NavDossiers` / `NavDossiersPerso` (navigation par dossiers) n'est pas non
plus une page séparée : c'est une **colonne embarquée**
(`.split-2col__aside`, toujours affichée, jamais masquée) dans
`vue-campagne.html` et `vue-espace-personnel.html`.

## 2. `site.css` — feuille de design partagée

Toute page référence la même feuille, à profondeur fixe 2 niveaux sous
`prototype/` :

```html
<link rel="stylesheet" href="../../site.css">
```

`site.css` est la source unique de tokens et de composants — l'en-tête du
fichier documente ses 4 sections :

1. **Tokens** (`:root`) — couleurs neutres gris/blanc, un seul gris
   d'accent, typo système, échelle d'espacement, rayons, ombres.
2. **App-shell** — gabarit de page réutilisable (`.app-shell`,
   `.app-header`, `.app-nav`, `.app-main`, `.app-content`).
3. **Composants** — boutons, cartes, badges, champs (`.field`, jamais de
   contrôle natif), bandeaux, puces de compte, etc.
4. **Sur-couches** — `.modal` / `.side-panel` / `.split-2col` / `.menu`
   (voir §4).

Aucune page ne redéfinit de style local hors ajustement strictement
documenté en commentaire : une classe manquante s'ajoute dans `site.css`
(source unique), jamais localement.

## 3. `overlay.js` — mécanisme de sur-couches

Script vanilla, local, zéro dépendance, chargé en `<script defer>` après
`site.css` sur toute page qui expose une modale, un panneau latéral ou un
menu déroulant :

```html
<script src="../../overlay.js" defer></script>
```

Deux mécanismes cohabitent, tous deux pilotés par délégation d'événements
sur `document` (aucun `querySelectorAll` à maintenir par page) :

**A. Modale / panneau latéral** — un seul et même mécanisme sert
indifféremment les deux (ils reposent sur la même classe d'état
`is-open`) :

- **Ouverture** — tout élément portant `data-open="<id>"` ajoute `is-open`
  à l'élément `#<id>`.
- **Fermeture** — trois voies équivalentes : un élément `[data-close]` à
  l'intérieur de la sur-couche ouverte, un clic sur `.modal__backdrop`, ou
  la touche Échap (ferme la sur-couche ouverte la plus récente, dans
  l'ordre du document).

**B. Menu déroulant (popover)** — même classe d'état `is-open`, mécanisme
de positionnement et de fermeture distinct :

- **Ouverture / bascule** — tout élément portant `data-menu="<id>"` bascule
  `is-open` sur l'élément `#<id>` (une `.menu`, cf. §4) ; ouvrir un menu
  ferme tout autre menu déjà ouvert (un seul menu ouvert à la fois). Le
  menu est niché en DOM à l'intérieur de son déclencheur
  (`position:relative` sur le déclencheur, `position:absolute` sur
  `.menu`), pas de `.modal__backdrop`.
- **Fermeture** — trois voies équivalentes : un clic sur un `.menu__item`
  (le menu se ferme, la navigation du lien suit son cours normal, pas de
  `preventDefault`), un clic en dehors du menu et de son déclencheur, ou
  la touche Échap.

Aucune logique métier modélisée (pas de validation de formulaire, pas de
filtrage de résultats) — uniquement la bascule visuelle `is-open`. Ajouter
une sur-couche ou un menu ne demande aucun JS supplémentaire : poser
`data-open`/`data-close`/`data-menu` aux bons endroits suffit, le script
les câble automatiquement.

## 4. Patterns de sur-couche / mise en page

Quatre patterns distincts, à ne pas confondre :

| Pattern | Classe `site.css` | Visibilité par défaut | Exemple |
|---|---|---|---|
| Modale | `.modal` (+ `.modal__backdrop`, `.modal__panel`, `.modal__close`) | masquée, plein écran centré une fois `is-open` | `#quick-create-modal` dans `vue-session-mj.html` |
| Panneau latéral overlay | `.side-panel` (dans `.split-2col__aside`) | masqué, affiché in-page via `is-open` | `#search-results` dans `vue-session-mj.html` / `vue-campagne.html` / `vue-espace-personnel.html` |
| Colonne embarquée | `.split-2col__aside` (sans `.side-panel`) | **toujours affichée**, jamais masquée | arborescence des dossiers dans `vue-campagne.html` / `vue-espace-personnel.html` |
| Menu déroulant (popover) | `.menu` (+ `.menu__item`), déclencheur `[data-menu]` | masqué, popover ancré en `position:absolute` sous le déclencheur une fois `is-open` — pas de backdrop | `#account-menu` dans la puce de compte (`.account-chip[data-menu]`), présent à l'identique sur toutes les surfaces MJ |

Le point de distinction entre panneau latéral overlay et colonne embarquée :
un panneau latéral overlay porte la classe `.side-panel` et un `id` ciblé
par un déclencheur `data-open` ; une colonne embarquée est une simple
`.split-2col__aside` sans cette classe — elle ne se masque jamais.

`.split-2col` peut s'imbriquer (ex. `vue-campagne.html` : un premier niveau
sépare la colonne dossiers du reste de la page, un second niveau, imbriqué
dans la colonne de contenu, sépare le contenu du panneau de résultats de
recherche) — c'est la même primitive réutilisée, jamais de classe inventée
en plus.

**Primitive menu déroulant — puce de compte.** La puce de compte du
châssis (`.account-chip`, présente sur toutes les surfaces MJ) est le
déclencheur du menu ; le menu lui-même est niché en DOM à l'intérieur de
la puce :

```html
<span class="account-chip" data-menu="account-menu">
  <span class="account-chip__avatar"></span>
  Thomas
  <span class="account-chip__caret">▾</span>
  <span class="menu" id="account-menu">
    <a href="../../sv1-transversaux/profil/profil.html" class="menu__item">Profil</a>
    <a href="../../sv1-transversaux/accueil/accueil.html" class="menu__item">Se déconnecter</a>
  </span>
</span>
```

Deux items seulement, identiques sur toute page MJ : **Profil** (mène à
`sv1-transversaux/profil/profil.html`) et **Se déconnecter** (mène à
`sv1-transversaux/accueil/accueil.html`, simulant la déconnexion — aucune
session réelle n'est modélisée). Le menu ne mène jamais à la surface
joueur (`sv5-joueur/`) — étanchéité AR-03, cf. §6. Page de référence pour
ce pattern : `sv2-entree-espace/tableau-de-bord/tableau-de-bord.html`
(cf. en-tête `site.css §4`).

## 5. Règle basse-fidélité

- Aucun contrôle de formulaire natif (`<input>`/`<select>`/`<textarea>`) :
  utiliser `.field` / `.field__value` / `.field__placeholder` (span
  stylés).
- Zéro dépendance externe : aucun `http`/CDN/police réseau — pile système
  uniquement.
- Palette sobre gris/neutre — pas de couleur d'application, réutiliser les
  tokens `--accent*` existants plutôt qu'en créer de nouveaux.
- Gabarit app-shell : en-tête plein-écran + contenu centré sur
  `--content-max`, gouttières fluides.

## 6. Comment les liens inter-pages sont câblés

**Cette règle a changé (mise à jour 2026-07-09).** Auparavant, un lien
réel n'existait que pour une transition portée par une arête `EDGES`
retrouvée sur l'écran ; toute affordance sans arête modélisée restait un
`<span>` inerte. Cette règle ne s'applique plus telle quelle : le
prototype matérialise désormais une **expérience de parcours complète**,
au-delà du seul graphe `EDGES`.

Règle actuelle : **toute affordance représentant une transition logique du
parcours est un `<a href="...">` réel** — que la transition corresponde ou
non à une arête modélisée dans `EDGES`. Deux cas :

- la cible est un **écran maquetté** du prototype → le lien pointe
  directement dessus ;
- la cible n'est **pas (encore) maquettée** → le lien pointe vers un
  placeholder `_stub/<nom>.html` « à venir » (cf. §8) — jamais un
  `<span>` inerte pour ce motif.

Le chemin relatif se calcule depuis le dossier de la page source
(profondeur fixe 2 niveaux) :

- vers un écran de la **même surface** :
  `../<slug-cible>/<slug-cible>.html` ;
- vers un écran d'une **autre surface** :
  `../../<surface-cible>/<slug-cible>/<slug-cible>.html` ;
- vers une page `_etat/` : `../_etat/<fichier>.html` (ou `./_etat/...`
  depuis `index.html`, à la racine) ;
- vers une page `_stub/` : `../../_stub/<fichier>.html` (profondeur fixe 2
  niveaux, comme pour une autre surface — cf. §8).

**Ne restent inertes que trois familles d'interaction**, jamais un `<a>` :

1. **Interactions strictement in-page** — édition (ex. « Enregistrer » un
   nom d'affichage), filtrage/sélection sans transition d'écran (ex. noms
   de dossiers dans la colonne embarquée `NavDossiers`/`NavDossiersPerso`,
   cf. §4), bascule d'état locale (ex. « + note », « déplacer › »,
   « consulter en place › »). Ce sont des `<span>`/`<button>` sans `href`.
2. **Repères de mode de la vue session** (AR-09, machine d'états
   unidirectionnelle) — le statut « Configuration → LIVE — session en
   cours → Consultation CLOSED » et le bouton « Terminer » sur
   `vue-session-mj.html` restent des `<span>` non cliquables : la cible
   serait le même fichier physique (transition intra-écran), non
   modélisable par un `<a href>` vers une autre page.
3. **Arêtes `EDGES` de type `"effect"` interdites de navigation** (AR-03)
   — « Copier » (génération du lien d'invitation, `parametres-campagne.html`)
   et « ⧉ Lien de session » (indicateur de partage, `vue-session-mj.html`)
   restent des `<span>` : ce sont des actions de génération/copie, jamais
   des transitions de navigation.

**Affordance visuellement désactivée mais réelle** (ex. « Créer la
campagne » à formulaire incomplet) : reste un `<a>`/`<button>` réel avec une
classe visuelle passive (`.btn--muted`, éventuellement `disabled` pour un
`<button>`) — ne pas poser `aria-disabled="true"` sur un lien qui reste
fonctionnel (signal d'accessibilité trompeur).

**AR-03 (étanchéité surface joueur)** : aucune page MJ ne lie vers
`sv5-joueur/` — la seule porte d'entrée vers la surface joueur est
`_etat/lien-session.html`, elle-même accessible depuis `index.html`. Dans
l'autre sens (joueur → MJ), **seules deux traversées sont autorisées**,
toutes deux vers l'écran transversal `inscription.html` (jamais vers un
écran propre à la structure MJ) :

- `vue-joueur-base.html` → `sv1-transversaux/inscription/inscription.html`
  (« Créer un compte », dans le châssis joueur) ;
- `acces-lien-saisie-nom.html` → `sv1-transversaux/inscription/inscription.html`
  (« Créer un compte pour conserver mes notes », zone information RGPD).

## 7. Page d'entrée

`index.html` (statique, à la racine) propose deux points de départ :
- parcours MJ standard → `sv1-transversaux/accueil/accueil.html` ;
- simulation de réception d'un lien de session (entrée joueur, seule porte
  vers la surface joueur, AR-03) → `_etat/lien-session.html`.

`_etat/mode-local.html` et `_etat/lien-session.html` matérialisent les deux
nœuds abstraits du graphe de navigation (`ModeLocal`, `LienSession` —
`zoning.md §S3`) : ce ne sont pas des écrans du produit, mais des pages de
bifurcation qui exposent, sous forme de choix explicites, les branches que
ces nœuds arbitrent normalement de façon conditionnelle (détection de
données locales, validité du lien reçu).

## 8. Dispositif placeholder — `_stub/`

Toute cible d'une transition logique du parcours qui n'est **pas (encore)
maquettée** dans le prototype reçoit un placeholder dédié dans `_stub/`,
plutôt que de laisser l'affordance source inerte (cf. §6). Cinq
placeholders existent à ce jour :

| Fichier `_stub/` | Origine (affordance source) |
|---|---|
| `aide.html` | « En savoir plus » (Accueil, message stockage local) |
| `offre-pro.html` | « Voir l'offre Pro » (Tableau de bord, invite quota atteint) |
| `espace-archive.html` | « Archiver… » (Paramètres de campagne, zone d'archivage) |
| `export-espace.html` | « Exporter… » (Paramètres de campagne, zone d'export) |
| `reinitialisation-mdp.html` | « Mot de passe oublié ? » (Connexion) |

**Gabarit** — chaque page `_stub/` reprend le même contenu minimal,
uniquement le châssis générique (`app-header` marque + tagline, sans puce
de compte) et une carte `.card--dashed` :

```html
<article class="card card--dashed" style="width:100%;max-width:480px;text-align:center;align-items:center;margin-top:var(--space-6);">
  <span class="card__eyebrow">à venir</span>
  <h1 class="card__title" style="font-size:var(--text-lg);"><em>Titre de l'écran</em> — à venir</h1>
  <p class="card__meta">Cet écran n'est pas encore maquetté dans le prototype.</p>
  <a href="<chemin-retour>" class="card__link">‹ Retour</a>
</article>
```

Le lien retour (`card__link`) pointe vers l'écran d'où provient
l'affordance source — jamais vers `index.html` ni vers un écran
générique : c'est ce qui évite le cul-de-sac (aucune action du parcours ne
doit laisser l'utilisateur sans retour possible).

**Emplacement** — `_stub/` est un dossier plat à la racine (profondeur 1
niveau depuis la racine, comme `_etat/`) : toute page `_stub/` référence
`../site.css` (pas `../../site.css`), et son lien retour recalcule le
chemin relatif depuis `_stub/` vers la page d'origine.

**Ajouter un nouveau placeholder** : créer `_stub/<nom>.html` sur ce
gabarit, `<nom>` reprenant l'intitulé de la cible non maquettée ; câbler
l'affordance source en `<a href="../../_stub/<nom>.html">` (ou
`../_stub/<nom>.html` depuis une page à profondeur 1) ; ne jamais
réutiliser un placeholder existant pour une cible sémantiquement
différente.

## 9. Comment ajouter une page

1. Créer `<surface>/<slug>/<slug>.html` (créer le dossier si nouvelle
   surface/slug).
2. Référencer `../../site.css` et, si la page expose une modale, un
   panneau latéral ou un menu déroulant, `../../overlay.js` (voir §2-§4).
3. Partir du gabarit structurel de
   `sv2-entree-espace/tableau-de-bord/tableau-de-bord.html` (en-tête de
   châssis avec menu compte, section-header, grille de cartes, badges) ou,
   pour une page à sur-couches, de
   `sv-session/vue-session-mj/vue-session-mj.html` (modale + panneau
   latéral) — adapter au contenu réel de l'écran, ne pas recopier le
   contenu.
4. Câbler en `<a href>` réel chaque affordance représentant une transition
   logique du parcours (§6) : vers l'écran réel s'il est maquetté, vers un
   placeholder `_stub/<nom>.html` sinon (§8, en créant le placeholder s'il
   n'existe pas encore). Ne laisser en `<span>` que les trois familles
   d'interaction volontairement inertes énumérées en §6 (in-page, repère
   de mode AR-09, effet AR-03).
5. Ajouter toute classe CSS manquante dans `site.css` (source unique),
   jamais en style local — sauf ajustement strictement documenté en
   commentaire.
