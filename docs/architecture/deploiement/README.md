# Déploiement — Haversack

> **Nature du document.** Ce dossier accueillera les environnements, scripts, pipelines CI/CD et
> procédures de déploiement de Haversack. **Aucun document de déploiement n'est produit à ce
> jour** — ce dossier est créé par anticipation et se remplira pendant le développement.

---

## Ancres déjà posées dans le corpus

Le sujet n'est pas vierge : plusieurs points relevant du déploiement sont déjà posés ailleurs
dans le corpus, sans y être consolidés :

- **Hébergeur `[À TRANCHER — hébergement]`.** La vue déploiement de [`../../reference/dossier-architecture.md`](../../reference/dossier-architecture.md#7-déploiement) marque explicitement l'hébergeur `[À TRANCHER — hébergement]`, jusque dans son diagramme.
- **Options d'hébergement recensées, non arbitrées.** [`../stack.md`](../stack.md) évoque des options d'hébergement sans trancher.
- **Plateforme d'intégration continue tranchée** (décision d'entrée en build du 2026-09-03) : **GitHub Actions**, la plateforme du dépôt où vit le code, déclenchée à chaque commit ([`../../projet/conventions-dod.md §6`](../../projet/conventions-dod.md)). Cette décision ne porte que sur l'intégration continue — **l'hébergement de l'application reste `[À TRANCHER — hébergement]`**, comme dit ci-dessus.
- **Gates d'intégration continue déjà nommés.** Le test d'architecture en CI (frontières de bounded context) est décrit comme gate obligatoire dans [`../../projet/conventions-dod.md`](../../projet/conventions-dod.md#6-domaine-de-convention--gates-dintégration-continue) et rattaché au jalon J0 dans [`../../projet/roadmap-de-build.md`](../../projet/roadmap-de-build.md).

## Contenu à venir

Ce dossier accueillera, une fois ces points arbitrés et le développement engagé : la
description des environnements, les scripts et la configuration de pipeline CI/CD, et les
procédures de déploiement.
