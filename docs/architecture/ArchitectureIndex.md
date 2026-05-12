# Haversack — Documentation d'architecture

Ce dossier contient la documentation d'architecture du projet Haversack.
Chaque fichier couvre une préoccupation précise — lire dans l'ordre conseillé
pour une première découverte, ou aller directement au fichier pertinent.

---

## Ordre de lecture conseillé

| Fichier | Contenu | Pour qui |
|---|---|---|
| [01-ddd-fondations.md](01-ddd-fondations.md) | Pourquoi DDD, vocabulaire et concepts appliqués au projet | Toute personne qui rejoint le projet |
| [02-bounded-contexts.md](02-bounded-contexts.md) | Découpage en contextes, Shared Kernel, règles d'isolation | Développeur qui touche à plusieurs contextes |
| [03-patterns-de-modelisation.md](03-patterns-de-modelisation.md) | Agrégats, modèle Document modulaire, références cross-context | Développeur qui modifie ou étend le domaine |
| [04-domaine-et-infrastructure.md](04-domaine-et-infrastructure.md) | Séparation des couches, EF Core, ASP.NET Identity, AuditInfo | Développeur qui implémente la persistance |
| [05-adr-et-anti-patterns.md](05-adr-et-anti-patterns.md) | Décisions d'architecture et pièges à éviter | Toute personne qui se demande "pourquoi" |

---


## En résumé

Haversack est modélisé avec **Domain-Driven Design (DDD)** organisé en 4 Bounded Contexts :

- **Identity & Access** — utilisateurs authentifiés
- **Campaign Management** — campagnes, membres, invitations, accès aux contenus
- **Content Library** — tout le contenu éditorial (documents, PNJ, personnages, scénarios)
- **Session Conduct** — conduite des sessions en temps réel

Le pattern central est le **modèle Document modulaire** : tout contenu éditorial
est un `Document` composé de `DocumentBlock` typés, inspiré du modèle de blocs de Notion.
Cela permet de supporter n'importe quel système de jeu sans migration de schéma.