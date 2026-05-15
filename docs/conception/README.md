# Haversack — Conception

Index de toute la documentation de conception produit et domaine.

---

## Vision & Produit

| Fichier | Contenu |
|---|---|
| [vision/vision-produit.md](vision/vision-produit.md) | Positionnement produit, acteurs, choix assumés, modèle de monétisation |
| [vision/moscow.md](vision/moscow.md) | Matrice MoSCoW complète (Must / Should / Could / Won't Have) |

---

## Personas

7 profils utilisateurs de référence utilisés pour valider les choix fonctionnels.

| Fichier | Persona |
|---|---|
| [persona/persona-01-thomas.md](persona/persona-01-thomas.md) | Thomas — MJ préparateur |
| [persona/persona-02-emilie.md](persona/persona-02-emilie.md) | Émilie — MJ impro |
| [persona/persona-03-lucas.md](persona/persona-03-lucas.md) | Lucas — joueur sans compte |
| [persona/persona-04-nadia.md](persona/persona-04-nadia.md) | Nadia — MJ casual |
| [persona/persona-05-antoine.md](persona/persona-05-antoine.md) | Antoine — MJ avancé |
| [persona/persona-06-remi.md](persona/persona-06-remi.md) | Rémi — MJ débutant |
| [persona/persona-07-sonia.md](persona/persona-07-sonia.md) | Sonia — MJ one-shot |

→ [Vue d'ensemble des personas](persona/README.md)

---

## Use Cases (UC)

14 use cases MVP couvrant l'ensemble des fonctionnalités de la première version.

| UC | Titre |
|---|---|
| [UC-01](usecases/UC-01-mode-local-sans-compte.md) | Mode local sans compte |
| [UC-02](usecases/UC-02-creer-espace-jeu.md) | Créer un espace de jeu |
| [UC-03](usecases/UC-03-structurer-scenario.md) | Structurer un scénario |
| [UC-04](usecases/UC-04-gerer-documents-campagne.md) | Gérer les documents d'une campagne |
| [UC-05](usecases/UC-05-organiser-dossiers.md) | Organiser par dossiers |
| [UC-06](usecases/UC-06-vue-session.md) | Vue session |
| [UC-07](usecases/UC-07-creation-volee-session.md) | Création à la volée en session |
| [UC-08](usecases/UC-08-partager-information.md) | Partager une information aux joueurs |
| [UC-09](usecases/UC-09-acces-session-joueur.md) | Accès session joueur |
| [UC-10](usecases/UC-10-compte-cloud.md) | Compte cloud |
| [UC-11](usecases/UC-11-gerer-membres-campagne.md) | Gérer les membres d'une campagne |
| [UC-12](usecases/UC-12-rejoindre-campagne.md) | Rejoindre une campagne |
| [UC-13](usecases/UC-13-scenario-reutilisable.md) | Scénario réutilisable |
| [UC-14](usecases/UC-14-recherche.md) | Recherche |
| [UC-HORS-MVP](usecases/UC-HORS-MVP.md) | Fonctionnalités exclues du MVP |

→ [Index use cases](usecases/README.md) · [Diagrammes de cas d'utilisation](usecases/use-cases.md)

---

## User Stories (US)

User stories détaillées avec critères d'acceptation et règles métier (RB-XX) pour chaque UC.

→ [Index user stories](user-stories/README.md)

---

## User Journeys (UJ)

Parcours utilisateur pas à pas pour chaque use case, du point de vue des personas.

→ [Index user journeys](user-journeys/README.md)

---

## Domaine DDD

Modélisation Domain-Driven Design — 4 bounded contexts + shared kernel.

| Fichier | Bounded context | Responsabilité |
|---|---|---|
| [domain/core.md](domain/core.md) | Core (Shared Kernel) | Abstractions DDD, IDs typés, value objects transverses |
| [domain/identity-access.md](domain/identity-access.md) | Identity & Access | Comptes utilisateurs, authentification, tiers, suppression RGPD |
| [domain/campaign-management.md](domain/campaign-management.md) | Campaign Management | Campagnes, one-shots, membres, invitations, accès invités |
| [domain/content-library.md](domain/content-library.md) | Content Library | Documents, dossiers, types de documents, références entre documents |
| [domain/session-conduct.md](domain/session-conduct.md) | Session Conduct | Cycle de vie de session, tableau de bord configurable, notes de session |

→ [Index domaine](domain/README.md)

---

## Stack technique

→ [stack.md](stack.md) — Choix technologiques, justifications, points forts/faibles, roadmap clients

---

## Autres

- [INTERVIEW_GUIDE.md](INTERVIEW_GUIDE.md) — Guide d'entretiens utilisateurs
