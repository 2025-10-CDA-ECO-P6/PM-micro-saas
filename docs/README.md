# Haversack — Documentation

Outil d'assistance au Maître du Jeu pour la préparation et la conduite de parties de jeu de rôle.

---

## Comment ce corpus est organisé

Ce corpus distingue deux paliers. [`context/`](reference/README.md) rassemble les documents
**de référence** — autoportants, lisibles seuls, exportables hors de ce dépôt (on peut en
remettre un à un prestataire ou dans un dossier de due diligence sans joindre le reste). Les
autres dossiers portent le **corpus de traçabilité détaillé** dont ces documents de référence
sont la consolidation : c'est là que vivent les décisions, leur justification et leur détail.

Chaque dossier ci-dessous est propriétaire de son propre index ; ce document n'en recopie pas
le contenu.

---

## Les dossiers

| Dossier | Nature | Index |
|---|---|---|
| `context/` | Documents de référence de premier rang — le quoi fonctionnel, le quoi technique, la carte du système, le comment, le cadrage business | [reference/README.md](reference/README.md) |
| `gestion-projet/` | Pilotage du projet — cadrage, planification, organisation, risques, conventions de travail | [projet/README.md](projet/README.md) |
| `securite/` | Sécurité technique consolidée, distincte des cadrages de conformité légale (RGPD) internes en attente de validation juriste | [assurance/securite/README.md](assurance/securite/README.md) |
| `test/` | Stratégie de test et de recette | [assurance/README.md](assurance/README.md) |
| `deploiement/` | Environnements, pipelines et procédures de déploiement — dossier réservé, à remplir pendant le développement | [architecture/deploiement/README.md](architecture/deploiement/README.md) |
| `ecoconception/` | Démarche d'écoconception — dossier réservé, sujet non engagé à ce jour | [assurance/ecoconception/README.md](assurance/ecoconception/README.md) |
| `architecture/` | Corpus détaillé de traçabilité technique — décisions d'architecture (ADR), structure des projets, spécifications pré-build | [architecture/README.md](architecture/README.md) |
| `besoin/`, `domaine/`, `interface/` | Corpus détaillé de traçabilité produit et domaine, organisé selon l'ordre d'autorité besoin → domaine → interface | [reference/ordre-d-autorite.md](reference/ordre-d-autorite.md) |

La sélection des modules du corpus, lue par l'outillage d'assistance (`.claude/`), est tenue dans [corpus.md](corpus.md).

Le backlog d'alignement de l'onboarding (2026-10-07) a été appliqué puis retiré le 2026-10-09 ; son histoire se lit dans `docs/project/backlog/README.md @ 51a7cdc`, et le reste à faire dans le [plan de travail](projet/plan-de-travail.md).

Les règles de tenue du corpus sont tenues dans [conventions.md](conventions.md). Trois documents sont générés ou alimentés par l'outillage : la matrice des cas d'utilisation dans [tracabilite.md](tracabilite.md), le catalogue des événements dans [domaine/evenements.md](domaine/evenements.md) et le journal des changements normatifs dans [journal-du-corpus.md](journal-du-corpus.md).
