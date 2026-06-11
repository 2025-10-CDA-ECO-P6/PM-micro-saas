# NFR-I18N-03 — Liberté totale de langue pour le contenu créé par le MJ

Famille **Internationalisation** · [Index de conception](../README.md)

---

## Énoncé normatif

La langue de l'interface n'impose aucune contrainte sur la langue dans laquelle le MJ rédige
son contenu. Un MJ peut créer des documents, des notes et des titres dans n'importe quelle
langue, y compris des langues fictives inventées pour sa campagne.

---

## Raison d'être

Haversack est agnostique au système de jeu (vision §2.2). Cette neutralité s'étend à la langue
du contenu : le produit ne doit pas imposer, même silencieusement, une langue de travail au MJ.
Un MJ qui joue en espagnol avec une interface en français, qui invente une langue elfique pour
son univers de fantasy, ou qui nomme ses PNJ dans une écriture à idéogrammes ne doit rencontrer
aucune friction de l'outil. La langue de l'interface et la langue du contenu sont deux registres
entièrement indépendants.

Cette séparation est une conséquence directe du positionnement agnostique du produit. Un outil
qui contraindrait la langue du contenu se spécialiserait implicitement, excluant des profils
pour lesquels il aurait par ailleurs de la valeur. La vocation de Haversack est de disparaître
dans le flux créatif du MJ — imposer une contrainte linguistique sur le contenu serait l'opposé
de cette disparition.

**[Nadia](../persona/persona-04-nadia.md)** joue à Call of Cthulhu — un système dont les
univers empruntent fréquemment à des langues étrangères pour nommer les entités, les lieux
et les artefacts. Ses documents, ses PNJ et ses notes de session peuvent mêler le français,
l'anglais et des noms inventés. Si l'outil introduisait une friction à la saisie ou à
l'affichage de ce contenu mélangé, il cesserait d'être utile précisément dans le contexte
où elle en a le plus besoin — la session elle-même.

Le cas de la langue fictive est emblématique de l'agnosticisme du produit. Un MJ créant un
univers de science-fiction peut nommer ses planètes, ses factions et ses personnages dans
une langue construite, avec des caractères ou des séquences inhabituelles. Un outil qui
ne permettrait pas de saisir, retrouver et afficher ce contenu serait un obstacle à la
créativité narrative — l'opposé de ce que Haversack propose.

La recherche dans le contenu de la campagne (UC-14) étend cette exigence à la capacité de
retrouver du contenu quelle que soit sa langue. Un titre rédigé en japonais, en arabe ou dans
une langue fictive doit être aussi cherchable qu'un titre en français.

---

## Portée et hors-portée

**Ce que cette exigence couvre :**

- La saisie de contenu par le MJ dans n'importe quelle langue, y compris des langues utilisant
  des scripts ou des directions d'écriture différents du français, et des langues fictives
  inventées pour la campagne.
- L'enregistrement et la restitution fidèle de ce contenu : ce que le MJ saisit est ce qu'il
  retrouve, sans transformation, normalisation ou perte de caractères.
- La recherche dans les titres des documents créés par le MJ, quelle que soit la langue de
  ces titres (UC-14, périmètre MVP : recherche sur le titre uniquement).
- L'affichage du contenu créé par le MJ dans la vue session et dans la bibliothèque de
  contenu, sans dégradation de l'affichage due à la langue du contenu.
- Le contenu visible par les joueurs via les informations partagées : un document dont le
  titre est en langue étrangère s'affiche sans erreur dans la vue joueur.

**Ce que cette exigence ne couvre pas :**

- La langue de l'interface produite par l'application : couverte par NFR-I18N-01.
- La mise en page adaptée aux scripts à direction d'écriture de droite à gauche ou verticale :
  cette adaptation relève de la conception d'interface pour ces marchés spécifiques, et
  non de la liberté de saisie du contenu. Le contenu peut être saisi dans ces langues
  et restitué fidèlement ; la mise en page optimisée pour ces directions d'écriture est
  hors périmètre MVP.
- La vérification orthographique ou grammaticale du contenu créé par le MJ : l'outil
  n'intervient pas sur le contenu, quelle qu'en soit la langue.
- La recherche en plein texte dans le contenu des documents (hors périmètre MVP, la
  recherche MVP porte sur les titres uniquement — UC-14).

---

## Critères d'acceptation produit

- Un MJ peut saisir un titre de document, une note de session ou une description de PNJ
  dans une langue différente de celle de l'interface — par exemple en anglais, en espagnol,
  en japonais ou dans une langue fictive — et retrouver ce contenu dans l'application
  tel qu'il l'a saisi, sans modification ni message d'erreur.
- Un MJ peut saisir un titre de document dans une langue fictive comportant des caractères
  inhabituels, enregistrer ce document, et le retrouver affiché correctement dans sa
  bibliothèque de contenu et dans sa vue session.
- La recherche par titre (UC-14) retourne le document attendu lorsque le MJ saisit une
  partie du titre en langue étrangère ou fictive, dans les mêmes conditions que pour un
  titre en français.
- Un document dont le titre est rédigé dans une langue différente de l'interface s'affiche
  sans dégradation visuelle dans la vue session, dans la vue joueur et dans les résultats
  de recherche.
- Un MJ dont le groupe joue dans une langue différente de l'interface peut travailler
  intégralement avec un contenu rédigé dans cette langue, sans message d'erreur ni
  fonctionnalité dégradée.

---

## Traçabilité montante

**Use cases servis :**

- [UC-03 — Structurer un scénario](../usecases/UC-03-structurer-scenario.md) — saisie
  de contenu narratif pouvant être rédigé dans n'importe quelle langue.
- [UC-04 — Gérer les documents d'une campagne](../usecases/UC-04-gerer-documents-campagne.md)
  — création, nommage et organisation de documents sans contrainte de langue.
- [UC-06 — Vue session](../usecases/UC-06-vue-session.md) — affichage du contenu du MJ
  en cours de partie, y compris les titres et notes en langue étrangère ou fictive.
- [UC-07 — Création à la volée en session](../usecases/UC-07-creation-volee-session.md)
  — saisie de notes rapides en cours de partie dans la langue de travail du MJ.
- [UC-08 — Partager une information aux joueurs](../usecases/UC-08-partager-information.md)
  — partage de documents dont le contenu peut être dans une langue différente de l'interface.
- [UC-14 — Recherche](../usecases/UC-14-recherche.md) — recherche par titre dans du contenu
  rédigé en toute langue ; le critère de visibilité (MJ voit tout, joueur voit le public)
  s'applique indépendamment de la langue du contenu.

**User stories servies :**

- [US-UC-03 — Structurer un scénario](../user-stories/US-UC-03-structurer-scenario.md)
- [US-UC-04 — Gérer les documents d'une campagne](../user-stories/US-UC-04-gerer-documents-campagne.md)
- [US-UC-06 — Vue session](../user-stories/US-UC-06-vue-session.md)
- [US-UC-07 — Création à la volée en session](../user-stories/US-UC-07-creation-volee-session.md)
- [US-UC-08 — Partager une information aux joueurs](../user-stories/US-UC-08-partager-information.md)
- [US-UC-14 — Recherche](../user-stories/US-UC-14-recherche.md)
