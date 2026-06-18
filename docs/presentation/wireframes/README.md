# Wireframes basse-fidélité — MVP Haversack

> Index complet des 20 fiches de description d'écran produites pour le MVP.
> Périmètre : ensemble des surfaces MVP (zoning complet — décision opérateur du 2026-06-12).
> Statut : production terminée — 20 fiches déposées.

---

## Séquence de lecture recommandée

Avant de lire les fiches, lire les sources de vérité dans cet ordre :

1. `docs/conception/interface/zoning.md` — arbitrages figés (AR-01..17), inventaire des écrans (S4), châssis (S7), exclusions (S8), trous de corpus (S9).
2. `docs/conception/interface/conventions-wireframe.md` — notation basse-fidélité (familles de marqueurs C3), deux registres de nommage (Registre 1 : termes de domaine ; Registre 2 : termes de région d'interface).
3. `docs/conception/interface/gabarit-ecran.md` — structure du gabarit que chaque fiche instancie.
4. Les fiches de ce répertoire.

---

## Index des fiches par surface

### Surface MJ — Session (vague 3)

| Fiche | Écran | Rôle |
|---|---|---|
| [`vue-session-mj.md`](vue-session-mj.md) | Vue session MJ | Hub central de pilotage de session — tableau de bord configurable à trois modes (configuration / LIVE / consultation CLOSED) ; écran prioritaire du MVP (hypothèse H2) |
| [`panneau-creation-rapide.md`](panneau-creation-rapide.md) | Panneau de création rapide à la volée | Sur-couche sans navigation propre, invoquée depuis la vue session en mode LIVE et en mode consultation CLOSED (UC-07) |

### Écrans transversaux (SV1)

| Fiche | Écran | Rôle |
|---|---|---|
| [`accueil.md`](accueil.md) | Accueil non authentifié | Point d'entrée — présente les deux chemins (sans compte / avec compte) |
| [`inscription.md`](inscription.md) | Inscription | Création de compte (email + nom d'affichage, ou fournisseur externe) |
| [`connexion.md`](connexion.md) | Connexion | Authentification (email/mot de passe ou fournisseur externe) |
| [`profil.md`](profil.md) | Profil utilisateur | Modification du nom d'affichage, du mot de passe ; affichage du niveau de compte |
| [`suppression-compte-rgpd.md`](suppression-compte-rgpd.md) | Suppression de compte (RGPD) | Confirmation de suppression avec présentation des conséquences ; bloquée si campagnes actives avec membres |
| [`gate-migration.md`](gate-migration.md) | Gate de migration local→cloud | Présentation des espaces locaux détectés ; confirmation explicite avant migration ; signale que la configuration de vue session devra être reconfigurée |

### Surface MJ — Entrée espace (SV2)

| Fiche | Écran | Rôle |
|---|---|---|
| [`tableau-de-bord.md`](tableau-de-bord.md) | Tableau de bord des espaces de jeu | Liste de tous les espaces dont le MJ est propriétaire ou membre ; espace personnel hors quota, visuellement distinct ; repère visuel « session en cours » si une session LIVE existe |
| [`creation-espace.md`](creation-espace.md) | Création d'un espace de jeu | Formulaire de création (nom obligatoire, description et système optionnels) pour les espaces partagés (`CAMPAIGN` ou `ONE_SHOT`) |

### Surface MJ — Préparation (SV3)

| Fiche | Écran | Rôle |
|---|---|---|
| [`vue-campagne.md`](vue-campagne.md) | Vue campagne (espace de travail) | Hub de l'espace partagé — point d'entrée vers les dossiers, les documents, la vue session, les paramètres et l'invitation de joueurs |
| [`navigation-dossiers.md`](navigation-dossiers.md) | Navigation par dossiers | Arborescence des dossiers de l'espace ; affichage des documents en vue condensée ; accès à la création d'un document dans le dossier courant |
| [`recherche-preparation.md`](recherche-preparation.md) | Recherche en préparation | Barre de recherche globale dans le contenu de l'espace ; titre seul au MVP ; résultats regroupés par type |
| [`editeur-document.md`](editeur-document.md) | Éditeur de document | Création et modification d'un document (titre, type optionnel, propriétés structurées, blocs libres, visibilité, liens) |
| [`editeur-scenario.md`](editeur-scenario.md) | Éditeur de scénario | Cas spécialisé de l'éditeur pour les documents de type scénario — structure narrative (scènes liées, documents associés) |
| [`parametres-campagne.md`](parametres-campagne.md) | Paramètres de campagne | Configuration de la vue session (dossiers mis en avant, ordre) ; génération du lien d'invitation ; archivage de l'espace |

### Surface MJ — Espace personnel (SV4)

| Fiche | Écran | Rôle |
|---|---|---|
| [`vue-espace-personnel.md`](vue-espace-personnel.md) | Vue de l'espace personnel | Hub de l'espace personnel — accès à la navigation dossiers, à l'éditeur et à la recherche ; sans vue session, sans partage, sans gestion de membres |

### Surface joueur (SV5)

| Fiche | Écran | Rôle |
|---|---|---|
| [`acces-lien-saisie-nom.md`](acces-lien-saisie-nom.md) | Accès par lien + saisie du nom d'affichage | Première page visible par le joueur à l'ouverture du lien ; demande uniquement un nom d'affichage ; information RGPD sur la durée de conservation |
| [`vue-joueur-base.md`](vue-joueur-base.md) | Vue joueur post-accès | Consultation des documents partagés par le MJ ; prise de notes personnelles pendant une session LIVE |
| [`erreur-acces.md`](erreur-acces.md) | Page d'erreur d'accès | Message sobre en cas de lien expiré, révoqué ou invalide ; ne révèle pas l'existence de la campagne |

---

## Table de couverture UC → fiche(s)

| Use Case | Fiche(s) porteuse(s) |
|---|---|
| **UC-01** — Mode local sans compte | `accueil.md` ; `inscription.md` (invite contextuelle A1) ; châssis mode local porté par toutes les fiches surface MJ (bandeaux transversaux) ; `vue-espace-personnel.md` (capture immédiate) |
| **UC-02** — Créer un espace de jeu | `tableau-de-bord.md` ; `creation-espace.md` |
| **UC-03** — Structurer un scénario | `editeur-scenario.md` |
| **UC-04** — Gérer les documents d'un espace | `editeur-document.md` ; `navigation-dossiers.md` |
| **UC-05** — Organiser par dossiers | `navigation-dossiers.md` ; `vue-campagne.md` ; `parametres-campagne.md` |
| **UC-06** — Utiliser la vue session | `vue-session-mj.md` (tous modes) ; `panneau-creation-rapide.md` |
| **UC-07** — Créer un élément à la volée | `panneau-creation-rapide.md` |
| **UC-08** — Partager une information aux joueurs | `vue-session-mj.md` (mode LIVE) ; affordance de génération de lien logée dans `vue-session-mj.md` et `parametres-campagne.md` |
| **UC-09** — Accès joueur via lien | `acces-lien-saisie-nom.md` ; `vue-joueur-base.md` ; `erreur-acces.md` |
| **UC-10** — Créer un compte et synchroniser dans le cloud | `inscription.md` ; `connexion.md` ; `profil.md` ; `suppression-compte-rgpd.md` ; `gate-migration.md` |
| **UC-11** — Gérer les membres d'une campagne | Fraction Must wireframée : affordance de génération de lien logée dans `vue-session-mj.md` et `parametres-campagne.md`. *Écran Membres complet = différé (Should Have — AR-10).* |
| **UC-12** — Consulter sa campagne en tant que joueur (vue post-accès) | `vue-joueur-base.md` (fraction de base). *Enrichissement vue joueur au niveau campagne = différé (Should Have — AR-10).* |
| **UC-13** — Utiliser un scénario réutilisable | **Post-MVP — pas de fiche.** L'interface de bibliothèque « Mes scénarios » se logera dans l'espace personnel (AR-08 révisé) ; UC-13 est hors périmètre wireframe MVP. |
| **UC-14** — Rechercher rapidement une information | `recherche-preparation.md` ; `vue-session-mj.md` (recherche omniprésente en session) |

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

- Arbitrages figés appliqués : `docs/conception/interface/zoning.md §S6 AR-01..17`
- Châssis applicatif (indicateurs, bandeaux, accessibilité transversale) : `zoning.md §S7`
- Exclusions nommées (éléments hors périmètre wireframe MVP) : `zoning.md §S8`
- Trous de corpus (points d'interview, éléments sous-spécifiés) : `zoning.md §S9`
- Termes de région d'interface (Registre 2) : `conventions-wireframe.md §C2`
