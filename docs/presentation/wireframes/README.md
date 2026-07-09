# Wireframes basse-fidélité — MVP Haversack

> Index complet des 20 écrans du MVP. Chaque écran a son **dossier** (`<slug>/`) regroupant
> sa fiche de spécification `.md` et son wireframe HTML basse-fidélité (`<slug>.dc.html`).
> Périmètre : ensemble des surfaces MVP (zoning complet — décision opérateur du 2026-06-12).
> Statut : 20 fiches déposées ; **20 wireframes HTML co-localisés (20/20)**.
> Passe de corrections UI/UX **D1..D13** appliquée le 2026-07-02 (épinglage création vs partage,
> aperçu non modifiable des paramètres, lien de session ponctuel en vue session, règle de densité AR-19,
> export MVP, accès compte = composant de châssis sur toute surface MJ, sélection par espace au gate,
> micro-copy espace personnel, liens/backlinks repliés, règle « Non classés » unifiée) — détail : [`AUDIT.md`](AUDIT.md) §0bis.
> Navigation cliquable entre écrans : [`index.html`](index.html) — audit de conformité : [`AUDIT.md`](AUDIT.md).

---

## Organisation du répertoire

Les écrans sont **regroupés par surface** (codes SV du zoning S4) ; un dossier par écran (slug canonique kebab-case, aligné sur `conventions-wireframe.md`) :

```
<surface>/                ex. sv1-transversaux, sv2-entree-espace, sv3-preparation,
  <slug>/                     sv4-espace-personnel, sv5-joueur, sv-session
    <slug>.md         fiche de spécification (source de vérité écran)
    <slug>.dc.html    wireframe HTML basse-fidélité (présent pour les 20 écrans)
support.js            runtime partagé des exports (chaque HTML le référence en ../../support.js)
index.html            hub de navigation cliquable entre écrans
AUDIT.md              audit de conformité écran par écran + écrans manquants
_quarantine/          exports corrompus conservés pour traçabilité (non exploitables)
```

> ⚠️ **Ce répertoire est curé à la main.** Les `.dc.html` ont été renommés d'après leur **contenu réel** :
> l'export initial présentait un décalage nom↔contenu systématique (chaque fichier portait le nom de l'écran voisin).
> **Ne jamais ré-exporter l'outil de design directement par-dessus ce répertoire.** Déposer les nouveaux exports
> dans un dossier d'import séparé (`_inbox/`, hors arbre rangé), les vérifier par leur contenu, puis les intégrer
> manuellement (renommage par contenu + réécriture du chemin `support.js` en `../../support.js`). Un ré-export direct
> reproduirait le décalage et écraserait le rangement comme le câblage de navigation.

---

## Séquence de lecture recommandée

Avant de lire les fiches, lire les sources de vérité dans cet ordre :

1. `docs/conception/interface/zoning.md` — arbitrages figés (AR-01..20), inventaire des écrans (S4), châssis (S7), exclusions (S8), trous de corpus (S9).
2. `docs/conception/interface/conventions-wireframe.md` — notation basse-fidélité (familles de marqueurs C3), deux registres de nommage (Registre 1 : termes de domaine ; Registre 2 : termes de région d'interface).
3. `docs/conception/interface/gabarit-ecran.md` — structure du gabarit que chaque fiche instancie.
4. Les fiches de ce répertoire.

---

## Index des fiches par surface

### Surface MJ — Session (SV-session, vague 3)

| Fiche | Écran | Rôle |
|---|---|---|
| [`vue-session-mj/vue-session-mj.md`](sv-session/vue-session-mj/vue-session-mj.md) | Vue session MJ | Hub central de pilotage de session — tableau de bord configurable à trois modes (configuration / LIVE / consultation CLOSED) ; écran prioritaire du MVP (hypothèse H2) |
| [`panneau-creation-rapide/panneau-creation-rapide.md`](sv-session/panneau-creation-rapide/panneau-creation-rapide.md) | Panneau de création rapide à la volée | Sur-couche sans navigation propre, invoquée depuis la vue session en mode LIVE et en mode consultation CLOSED (UC-07) |

### Écrans transversaux (SV1)

| Fiche | Écran | Rôle |
|---|---|---|
| [`accueil/accueil.md`](sv1-transversaux/accueil/accueil.md) | Accueil non authentifié | Point d'entrée — présente les deux chemins (sans compte / avec compte) |
| [`inscription/inscription.md`](sv1-transversaux/inscription/inscription.md) | Inscription | Création de compte (email + nom d'affichage, ou fournisseur externe) |
| [`connexion/connexion.md`](sv1-transversaux/connexion/connexion.md) | Connexion | Authentification (email/mot de passe ou fournisseur externe) |
| [`profil/profil.md`](sv1-transversaux/profil/profil.md) | Profil utilisateur | Modification du nom d'affichage, du mot de passe ; affichage du niveau de compte |
| [`suppression-compte-rgpd/suppression-compte-rgpd.md`](sv1-transversaux/suppression-compte-rgpd/suppression-compte-rgpd.md) | Suppression de compte (RGPD) | Confirmation de suppression avec présentation des conséquences ; bloquée si campagnes actives avec membres |
| [`gate-migration/gate-migration.md`](sv1-transversaux/gate-migration/gate-migration.md) | Gate de migration local→cloud | Présentation des espaces locaux détectés ; confirmation explicite avant migration ; signale que la configuration de vue session devra être reconfigurée |

### Surface MJ — Entrée espace (SV2)

| Fiche | Écran | Rôle |
|---|---|---|
| [`tableau-de-bord/tableau-de-bord.md`](sv2-entree-espace/tableau-de-bord/tableau-de-bord.md) | Tableau de bord des espaces de jeu | Liste de tous les espaces dont le MJ est propriétaire ou membre ; espace personnel hors quota, visuellement distinct ; repère visuel « session en cours » si une session LIVE existe |
| [`creation-espace/creation-espace.md`](sv2-entree-espace/creation-espace/creation-espace.md) | Création d'un espace de jeu | Formulaire de création (nom obligatoire, description et système optionnels) pour les espaces partagés (`CAMPAIGN` ou `ONE_SHOT`) |

### Surface MJ — Préparation (SV3)

| Fiche | Écran | Rôle |
|---|---|---|
| [`vue-campagne/vue-campagne.md`](sv3-preparation/vue-campagne/vue-campagne.md) | Vue campagne (espace de travail) | Hub de l'espace partagé — point d'entrée vers les dossiers, les documents, la vue session, les paramètres et l'invitation de joueurs |
| [`navigation-dossiers/navigation-dossiers.md`](sv3-preparation/navigation-dossiers/navigation-dossiers.md) | Navigation par dossiers | Arborescence des dossiers de l'espace ; affichage des documents en vue condensée ; accès à la création d'un document dans le dossier courant |
| [`recherche-preparation/recherche-preparation.md`](sv3-preparation/recherche-preparation/recherche-preparation.md) | Recherche en préparation | Barre de recherche globale dans le contenu de l'espace ; titre seul au MVP ; résultats regroupés par type |
| [`editeur-document/editeur-document.md`](sv3-preparation/editeur-document/editeur-document.md) | Éditeur de document | Création et modification d'un document (titre, type optionnel, propriétés structurées, blocs libres, visibilité, liens) |
| [`editeur-scenario/editeur-scenario.md`](sv3-preparation/editeur-scenario/editeur-scenario.md) | Éditeur de scénario | Cas spécialisé de l'éditeur pour les documents de type scénario — structure narrative (scènes liées, documents associés) |
| [`parametres-campagne/parametres-campagne.md`](sv3-preparation/parametres-campagne/parametres-campagne.md) | Paramètres de campagne | Configuration de la vue session (dossiers mis en avant, ordre) ; génération du lien d'invitation ; archivage de l'espace |

### Surface MJ — Espace personnel (SV4)

| Fiche | Écran | Rôle |
|---|---|---|
| [`vue-espace-personnel/vue-espace-personnel.md`](sv4-espace-personnel/vue-espace-personnel/vue-espace-personnel.md) | Vue de l'espace personnel | Hub de l'espace personnel — accès à la navigation dossiers, à l'éditeur et à la recherche ; sans vue session, sans partage, sans gestion de membres |

### Surface joueur (SV5)

| Fiche | Écran | Rôle |
|---|---|---|
| [`acces-lien-saisie-nom/acces-lien-saisie-nom.md`](sv5-joueur/acces-lien-saisie-nom/acces-lien-saisie-nom.md) | Accès par lien + saisie du nom d'affichage | Première page visible par le joueur à l'ouverture du lien ; demande uniquement un nom d'affichage ; information RGPD sur la durée de conservation |
| [`vue-joueur-base/vue-joueur-base.md`](sv5-joueur/vue-joueur-base/vue-joueur-base.md) | Vue joueur post-accès | Consultation des documents partagés par le MJ ; prise de notes personnelles pendant une session LIVE |
| [`erreur-acces/erreur-acces.md`](sv5-joueur/erreur-acces/erreur-acces.md) | Page d'erreur d'accès | Message sobre en cas de lien expiré, révoqué ou invalide ; ne révèle pas l'existence de la campagne |

---

## Table de couverture UC → fiche(s)

| Use Case | Fiche(s) porteuse(s) |
|---|---|
| **UC-01** — Mode local sans compte | `accueil/accueil.md` ; `inscription/inscription.md` (invite contextuelle A1) ; châssis mode local porté par toutes les fiches surface MJ (bandeaux transversaux) ; `vue-espace-personnel/vue-espace-personnel.md` (capture immédiate) |
| **UC-02** — Créer un espace de jeu | `tableau-de-bord/tableau-de-bord.md` ; `creation-espace/creation-espace.md` |
| **UC-03** — Structurer un scénario | `editeur-scenario/editeur-scenario.md` |
| **UC-04** — Gérer les documents d'un espace | `editeur-document/editeur-document.md` ; `navigation-dossiers/navigation-dossiers.md` |
| **UC-05** — Organiser par dossiers | `navigation-dossiers/navigation-dossiers.md` ; `vue-campagne/vue-campagne.md` ; `parametres-campagne/parametres-campagne.md` |
| **UC-06** — Utiliser la vue session | `vue-session-mj/vue-session-mj.md` (tous modes) ; `panneau-creation-rapide/panneau-creation-rapide.md` |
| **UC-07** — Créer un élément à la volée | `panneau-creation-rapide/panneau-creation-rapide.md` |
| **UC-08** — Partager une information aux joueurs | `vue-session-mj/vue-session-mj.md` (mode LIVE) ; affordance de génération de lien logée dans `vue-session-mj/vue-session-mj.md` et `parametres-campagne/parametres-campagne.md` |
| **UC-09** — Accès joueur via lien | `acces-lien-saisie-nom/acces-lien-saisie-nom.md` ; `vue-joueur-base/vue-joueur-base.md` ; `erreur-acces/erreur-acces.md` |
| **UC-10** — Créer un compte et synchroniser dans le cloud | `inscription/inscription.md` ; `connexion/connexion.md` ; `profil/profil.md` ; `suppression-compte-rgpd/suppression-compte-rgpd.md` ; `gate-migration/gate-migration.md` |
| **UC-11** — Gérer les membres d'une campagne | Fraction Must wireframée : affordance de génération de lien logée dans `vue-session-mj/vue-session-mj.md` et `parametres-campagne/parametres-campagne.md`. *Écran Membres complet = différé (Should Have — AR-10).* |
| **UC-12** — Consulter sa campagne en tant que joueur (vue post-accès) | `vue-joueur-base/vue-joueur-base.md` (fraction de base). *Enrichissement vue joueur au niveau campagne = différé (Should Have — AR-10).* |
| **UC-13** — Utiliser un scénario réutilisable | **Post-MVP — pas de fiche.** L'interface de bibliothèque « Mes scénarios » se logera dans l'espace personnel (AR-08 révisé) ; UC-13 est hors périmètre wireframe MVP. |
| **UC-14** — Rechercher rapidement une information | `recherche-preparation/recherche-preparation.md` ; `vue-session-mj/vue-session-mj.md` (recherche omniprésente en session) |

---

## Éléments différés (non wireframés — S8)

Les éléments suivants sont inventoriés dans le zoning (S8) mais hors périmètre wireframe MVP :

| Élément | Source |
|---|---|
| Écran Membres complet (UC-11 — révocations, associations joueur-personnage, gestion invitations permanentes) | Should Have — AR-10 |
| Enrichissement vue joueur au niveau campagne (UC-12 complet — historique de campagne, sélection de personnage) | Should Have — AR-10 |
| Interface de bibliothèque « Mes scénarios » (UC-13) | Post-MVP — AR-08 révisé ; UC-13 §Statut |

---

## Renvois

- Arbitrages figés appliqués : `docs/conception/interface/zoning.md §S6 AR-01..20`
- Châssis applicatif (indicateurs, bandeaux, accessibilité transversale) : `zoning.md §S7`
- Exclusions nommées (éléments hors périmètre wireframe MVP) : `zoning.md §S8`
- Trous de corpus (points d'interview, éléments sous-spécifiés) : `zoning.md §S9`
- Termes de région d'interface (Registre 2) : `conventions-wireframe.md §C2`
