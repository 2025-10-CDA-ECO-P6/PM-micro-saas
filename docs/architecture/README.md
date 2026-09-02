# Haversack — Documentation d'architecture

Ce dossier contient la documentation d'architecture du projet Haversack.
Chaque fichier couvre une préoccupation précise — lire dans l'ordre conseillé
pour une première découverte, ou aller directement au fichier pertinent.

---

## Ordre de lecture conseillé

| Fichier | Contenu | Pour qui |
|---|---|---|
| [Dossier d'Architecture Technique](../context/dossier-architecture.md) | Carte structurelle du système : vues C4 (contexte, conteneurs, composants, déploiement) + choix de structure macro + justification par renvoi ADR | Qui veut la carte du système avant d'en lire le détail |
| [ddd-fondations.md](ddd-fondations.md) | Pourquoi DDD, vocabulaire et concepts appliqués au projet | Toute personne qui rejoint le projet |
| [stack.md](stack.md) | Choix technologiques, justifications, points forts/faibles, roadmap clients | Développeur qui veut comprendre la stack retenue |
| [structure-projets.md](structure-projets.md) | Structure des projets .NET + périmètre du mode local TypeScript | Développeur qui échafaude ou navigue la solution |
| [specs/](specs/) | Spécifications techniques pré-build dérivées des ADR (contrats API, sanitisation/CSP, mapping EF Core, schémas de propriétés, télémétrie, config, repli temps réel, effacement) | Développeur en entrée de build |
| [architecture-detaillee.md](architecture-detaillee.md) | Vue par préoccupation technique transverse (sécurité, RGPD, persistance, temps réel, mode local), complémentaire de la carte structurelle | Qui explore les décisions d'archi par sujet transverse |
| [decisions/README.md](decisions/README.md) | Registre des décisions d'architecture (ADR) | Toute personne qui veut comprendre les arbitrages structurels |

---

## Planification et phasage

| Document | Contenu |
|---|---|
| [../gestion-projet/roadmap-entree-build.md](../gestion-projet/roadmap-entree-build.md) | Roadmap d'ordonnancement du build (jalons J0→J3, décision marché et validation juridique EU) — **distinct de l'architecture** : explique le *quand* et le *dans quel ordre construire*, pas le *comment c'est construit* |

---

## En résumé

Haversack est modélisé avec **Domain-Driven Design (DDD)** organisé en 4 Bounded Contexts :

- **Identity & Access** — utilisateurs authentifiés
- **Space Management** — espaces (`Space` : campagnes, one-shots, espaces personnels), membres, invitations, accès aux contenus
- **Content Library** — tout le contenu éditorial (documents, PNJ, personnages, scénarios)
- **Session Conduct** — conduite des sessions en temps réel

Le pattern central est le **modèle Document modulaire** : tout contenu éditorial
est un `Document` composé de `DocumentBlock`. Cela permet de supporter n'importe quel système de jeu sans migration de schéma.