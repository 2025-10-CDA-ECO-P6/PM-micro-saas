# Haversack — Documentation

Outil d'assistance au Maître du Jeu pour la préparation et la conduite de parties de jeu de rôle. Pour découvrir le projet et choisir un parcours de lecture, partir du [README du dépôt](../README.md).

---

## Comment ce corpus est organisé

Le corpus se range en un dossier par couche, du besoin vers sa réalisation. Deux dossiers consolident le reste sans jamais faire foi : [`reference/`](reference/README.md) rassemble les documents **de référence** — autoportants, lisibles seuls, exportables hors de ce dépôt — et [`projet/`](projet/README.md) porte le pilotage. Les autres dossiers portent le **corpus de traçabilité détaillé** : c'est là que vivent les décisions, leur justification et leur détail.

Chaque dossier ci-dessous est propriétaire de son propre index ; ce document n'en recopie pas le contenu. L'ordre d'autorité entre les couches est dans [conventions.md](conventions.md).

---

## Les dossiers

| Dossier | Rôle | Index |
|---|---|---|
| `besoin/` | Le besoin produit : personas, vision et priorités, use cases, user stories, user journeys, parcours de bout en bout, exigences non fonctionnelles | [besoin/README.md](besoin/README.md) |
| `domaine/` | Le modèle de domaine DDD : un dossier par contexte métier et un noyau partagé, avec règles, agrégats, événements, diagrammes et modèles de données | [domaine/README.md](domaine/README.md) |
| `interface/` | La conception de l'interface : zonage et navigation, conventions, fiches d'écran et maquettes basse fidélité | [interface/README.md](interface/README.md) |
| `decisions/` | Le registre unique des décisions d'architecture (ADR) | [decisions/README.md](decisions/README.md) |
| `architecture/` | La documentation technique : fondations DDD, stack, structure des projets, préoccupations transverses et spécifications pré-build ; porte aussi le dossier [`deploiement/`](architecture/deploiement/README.md), réservé | [architecture/README.md](architecture/README.md) |
| `assurance/` | La stratégie de test et de recette, la [sécurité technique](assurance/securite/README.md), les cadrages de conformité (RGPD) en attente de validation juridique, et le dossier [`ecoconception/`](assurance/ecoconception/README.md), réservé | [assurance/README.md](assurance/README.md) |
| `reference/` | Les documents de référence autoportants, consolidations du corpus détaillé | [reference/README.md](reference/README.md) |
| `projet/` | Le pilotage : cadrage, roadmaps, plan de travail, méthode de ticket, conventions et définition de fini, risques, décisions en attente, revues | [projet/README.md](projet/README.md) |

---

## Les documents d'entrée

| Document | Rôle |
|---|---|
| [conventions.md](conventions.md) | Les conventions de tenue du corpus : ordre d'autorité, registre des identifiants, registre des marqueurs, renvois |
| [glossaire.md](glossaire.md) | La référence de forme des termes du corpus ; il fixe le nom des termes, pas leur sens |
| [corpus.md](corpus.md) | La sélection des modules du corpus lue par l'outillage d'assistance (`.claude/`) |
| [journal-du-corpus.md](journal-du-corpus.md) | Le journal des changements normatifs du corpus, alimenté par l'outillage |
| [tracabilite.md](tracabilite.md) | La matrice des use cases vers leurs stories, règles, cas de test, tâches, écrans et décisions, générée par l'outillage |

Le catalogue des événements du domaine, lui aussi alimenté par l'outillage, est dans [domaine/evenements.md](domaine/evenements.md).
