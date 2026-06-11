# User Journeys — Haversack

Ce dossier contient les user journeys associés aux use cases du MVP. Ils complètent les user stories en ajoutant la perspective émotionnelle et temporelle de chaque persona. Là où les user stories définissent ce que le système doit faire, les journeys cartographient ce que l'utilisateur ressent à chaque étape. Ils servent de pont entre la vision produit et l'implémentation : identifier les frictions avant le développement, pas après.

---

## Comment lire un user journey

Chaque fichier contient quatre sections :

- **Carte d'expérience** (`journey`) : score de ressenti par étape pour chaque persona (1 = frustrant, 5 = fluide). Permet d'identifier les creux émotionnels à risque d'abandon.
- **Flux fonctionnel** (`flowchart TD`) : décisions techniques et bounded contexts traversés. Permet de vérifier la cohérence avec l'architecture.
- **Points de friction** : risques d'abandon ou de mauvaise expérience identifiés sur ce parcours.
- **Opportunités UX** : leviers de différenciation spécifiques à ce use case.

---

## Personas de référence

| Persona | Profil | Score de référence attendu |
|---|---|---|
| Emilie | MJ débutante, improvise | Score bas sur les étapes de configuration longues |
| Thomas | MJ expérimenté, préparateur | Score bas sur les étapes non configurables |
| Nadia | MJ casual | Score bas sur tout ce qui demande une mémoire de l'outil |
| Sonia | MJ convention, one-shots | Score bas sur les flux de création de campagne |
| Antoine | MJ avancé, multi-campagnes | Score bas sur l'absence de structure typée |
| Lucas | Joueur, pas d'outil supplémentaire | Score bas sur toute demande de compte ou d'inscription |

---

## Tableau de correspondance

| UC | Titre | MoSCoW | Fichier Journey | Fichier Stories |
|---|---|---|---|---|
| UC-01 | Mode local sans compte | Must Have | [UJ-UC-01](UJ-UC-01-mode-local-sans-compte.md) | [US-UC-01](../user-stories/US-UC-01-mode-local-sans-compte.md) |
| UC-02 | Créer un espace de jeu | Must Have | [UJ-UC-02](UJ-UC-02-creer-espace-jeu.md) | [US-UC-02](../user-stories/US-UC-02-creer-espace-jeu.md) |
| UC-03 | Structurer un scénario | Must Have | [UJ-UC-03](UJ-UC-03-structurer-scenario.md) | [US-UC-03](../user-stories/US-UC-03-structurer-scenario.md) |
| UC-04 | Gérer les documents | Must Have | [UJ-UC-04](UJ-UC-04-gerer-documents-campagne.md) | [US-UC-04](../user-stories/US-UC-04-gerer-documents-campagne.md) |
| UC-05 | Organiser les dossiers | Must Have | [UJ-UC-05](UJ-UC-05-organiser-contenu-dossiers.md) | [US-UC-05](../user-stories/US-UC-05-organiser-contenu-dossiers.md) |
| UC-06 | Vue session | Must Have | [UJ-UC-06](UJ-UC-06-vue-session.md) | [US-UC-06](../user-stories/US-UC-06-vue-session.md) |
| UC-07 | Création à la volée | Must Have | [UJ-UC-07](UJ-UC-07-creation-volee-session.md) | [US-UC-07](../user-stories/US-UC-07-creation-volee-session.md) |
| UC-08 | Partager une information | Must Have | [UJ-UC-08](UJ-UC-08-partager-information.md) | [US-UC-08](../user-stories/US-UC-08-partager-information.md) |
| UC-09 | Accès session joueur | Must Have | [UJ-UC-09](UJ-UC-09-acces-session-joueur.md) | [US-UC-09](../user-stories/US-UC-09-acces-session-joueur.md) |
| UC-10 | Compte cloud | Must Have | [UJ-UC-10](UJ-UC-10-compte-cloud.md) | [US-UC-10](../user-stories/US-UC-10-compte-cloud.md) |
| UC-11 | Gérer les membres | Should Have | [UJ-UC-11](UJ-UC-11-gerer-membres-campagne.md) | [US-UC-11](../user-stories/US-UC-11-gerer-membres-campagne.md) |
| UC-12 | Rejoindre une campagne | Should Have | [UJ-UC-12](UJ-UC-12-rejoindre-campagne.md) | [US-UC-12](../user-stories/US-UC-12-rejoindre-campagne.md) |
| UC-13 | Scénario réutilisable | Should Have | [UJ-UC-13](UJ-UC-13-scenario-reutilisable.md) | [US-UC-13](../user-stories/US-UC-13-scenario-reutilisable.md) |
| UC-14 | Recherche | Should Have | [UJ-UC-14](UJ-UC-14-recherche.md) | [US-UC-14](../user-stories/US-UC-14-recherche.md) |

---

## Synthèse des points de friction par persona

| Persona | Frictions majeures identifiées (tous journeys) |
|---|---|
| Emilie | Confirmations bloquantes en session, formulaires trop chargés, bouton "Créer" non accessible en un clic |
| Thomas | Absence d'indicateur d'état de visibilité, panneau membres peu synthétique, configuration de vue session peu visible |
| Nadia | Découvrabilité des fonctionnalités (création en CLOSED, search de notes passées), reprise après longue absence |
| Sonia | Création d'une campagne obligatoire pour lancer un one-shot, catalogue vide au premier lancement, confusion source/instance |
| Antoine | Document dans "Non classés" si dossier non résolu, types personnalisés absents du panneau de création rapide |
| Lucas | Toute demande de compte ou d'email, expiration silencieuse de l'accès invité, friction de migration en fin de soirée |

---

## Synthèse des opportunités UX majeures

Patterns récurrents identifiés à travers l'ensemble des journeys :

- **Accès immédiat sans friction** (Lucas, Emilie) : priorité au lien, au nom seul, à l'absence de confirmation.
- **Indicateurs d'état visibles en permanence** (Thomas) : visibilité des documents, état des liens membres.
- **Auto-save systématique** (Emilie, Nadia) : jamais de perte de données sans action volontaire.
- **Panneau de création réduit au minimum vital** (Emilie) : un champ, le titre, rien d'autre visible.
- **Accès à la fonctionnalité depuis son contexte naturel** (tous) : pas de navigation vers un écran dédié.
- **Mode édition sans engagement** (Thomas, Sonia) : tester la config sans créer une session, sans archiver.

---

## Liens

- User stories associées : [`../user-stories/`](../user-stories/)
- Use cases source : [`../usecases/`](../usecases/)
- Vision produit : [`../vision/vision-produit.md`](../vision/vision-produit.md)
