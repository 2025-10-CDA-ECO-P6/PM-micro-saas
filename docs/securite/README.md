# Sécurité et conformité — Haversack

> **Nature du document.** Ce dossier porte deux familles de documents distinctes, l'une
> technique, l'autre juridique. Aucun de ces documents ne tranche de décision propre : chacun
> consolide un contenu décidé ailleurs et renvoie à sa source de conception.

---

## Sécurité technique

- [dossier-securite.md](dossier-securite.md) — modèle de menaces, surfaces d'attaque et contre-mesures, consolidés depuis le corpus de conception (ADR, spécifications fonctionnelles et non fonctionnelles, contrats d'API).

## Conformité légale

Les quatre documents suivants sont des **cadres internes**, tous en attente de validation par
un juriste — chacun le déclare explicitement en tête de document :

| Document | Contenu |
|---|---|
| [conformite/cadrage-validation-pre-lancement-eu.md](conformite/cadrage-validation-pre-lancement-eu.md) | Checklist de consolidation des points de conformité RGPD à valider avant tout lancement EU |
| [conformite/dpa-skeleton.md](conformite/dpa-skeleton.md) | Squelette de sections pour la rédaction juriste d'un Data Processing Agreement (Art. 28 RGPD) |
| [conformite/procedure-droits-invites.md](conformite/procedure-droits-invites.md) | Cadre procédural d'instruction d'une demande d'exercice de droits RGPD par un joueur invité, et procédure support Art. 12§3 |
| [conformite/roadmap-juridique.md](conformite/roadmap-juridique.md) | Séquencement des livrables juridiques préalables au lancement EU |

## Documents destinés aux utilisateurs

Les documents suivants sont des **brouillons destinés à être publiés aux utilisateurs**, distincts
des cadres internes ci-dessus. Chacun porte en tête un bandeau explicite : brouillon non validé
juridiquement, non opposable en l'état.

| Document | Contenu |
|---|---|
| [conformite/cgu.md](conformite/cgu.md) | Conditions générales d'utilisation — accès au service (mode local, compte cloud, invité), compte utilisateur, contenu MJ, paliers et limites, gel des espaces au changement de palier |
| [conformite/mentions-legales.md](conformite/mentions-legales.md) | Mentions légales — éditeur, contact, hébergeur, propriété intellectuelle (essentiellement `[À COMPLÉTER]`, l'entité éditrice n'étant pas encore constituée) |
| [conformite/politique-confidentialite.md](conformite/politique-confidentialite.md) | Politique de confidentialité — traitement des données personnelles, trois populations (MJ avec compte, MJ en mode local, joueur invité), droits et procédure d'effacement |
| [conformite/politique-cookies.md](conformite/politique-cookies.md) | Politique de cookies et traceurs — traceurs strictement nécessaires (compte cloud, canal invité), mesure d'audience anonyme, stockage local du mode sans compte |
