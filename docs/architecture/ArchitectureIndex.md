# Haversack — Documentation d'architecture

Ce dossier contient la documentation d'architecture du projet Haversack.
Chaque fichier couvre une préoccupation précise — lire dans l'ordre conseillé
pour une première découverte, ou aller directement au fichier pertinent.

---

## Ordre de lecture conseillé

| Fichier | Contenu | Pour qui |
|---|---|---|
| [01-ddd-fondations.md](01-ddd-fondations.md) | Pourquoi DDD, vocabulaire et concepts appliqués au projet | Toute personne qui rejoint le projet |
| [stack.md](stack.md) | Choix technologiques, justifications, points forts/faibles, roadmap clients | Développeur qui veut comprendre la stack retenue |
| [06-structure-projets.md](06-structure-projets.md) | Structure des projets .NET + périmètre du mode local TypeScript | Développeur qui échafaude ou navigue la solution |
| [decisions/README.md](decisions/README.md) | Registre des décisions d'architecture (ADR) | Toute personne qui veut comprendre les arbitrages structurels |

---


## En résumé

Haversack est modélisé avec **Domain-Driven Design (DDD)** organisé en 4 Bounded Contexts :

- **Identity & Access** — utilisateurs authentifiés
- **Campaign Management** — campagnes, membres, invitations, accès aux contenus
- **Content Library** — tout le contenu éditorial (documents, PNJ, personnages, scénarios)
- **Session Conduct** — conduite des sessions en temps réel

Le pattern central est le **modèle Document modulaire** : tout contenu éditorial
est un `Document` composé de `DocumentBlock`. Cela permet de supporter n'importe quel système de jeu sans migration de schéma.