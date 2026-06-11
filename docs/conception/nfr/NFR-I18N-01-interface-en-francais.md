# NFR-I18N-01 — Interface entièrement utilisable en français au MVP

Famille **Internationalisation** · [Index de conception](../README.md)

---

## Énoncé normatif

L'ensemble de l'interface — libellés, messages d'erreur, notifications, textes d'aide,
confirmations — est en français correct et cohérent. Aucun texte en langue étrangère
n'apparaît à l'utilisateur dans l'expérience normale du produit.

---

## Raison d'être

Le produit est développé et lancé à destination du marché francophone. Ce choix est explicite
et assumé : l'ensemble des personas de référence, des parcours utilisateur et des critères
d'acceptation du MVP ont été conçus dans ce contexte. Une interface partiellement traduite,
mêlant le français à des fragments de langue étrangère, n'est pas une imperfection cosmétique —
c'est un signal de manque de finition qui érode la confiance dès le premier usage.

**[Nadia](../persona/persona-04-nadia.md)** en est le cas test le plus révélateur. Elle a
déjà abandonné un outil — Notion — parce que l'investissement d'adoption était disproportionné
à sa fréquence d'usage. Un outil dont l'interface n'est pas entièrement en français lui renvoie
le signal qu'il n'a pas été conçu pour elle. Sa question fondamentale — *« combien de temps pour
que ça soit utilisable ? »* — suppose une interface immédiatement compréhensible, sans
apprentissage d'une terminologie étrangère.

**[Rémi](../persona/persona-06-remi.md)** représente un profil différent, résistant au
numérique non par incompétence mais par conviction. Sa question *« en quoi c'est mieux qu'une
page vierge ? »* ne peut recevoir de réponse satisfaisante si l'interface elle-même présente
un obstacle de compréhension. Pour un utilisateur qui doute déjà de la valeur de l'outil
numérique, un libellé en langue étrangère confirme que l'outil n'est pas fait pour lui.

L'agnosticisme au système de jeu est un différenciant central de Haversack (vision §2.2) :
le produit fonctionne pour D&D, Call of Cthulhu, Fate, Blades in the Dark, les systèmes
narratifs et les systèmes maison. Cet agnosticisme suppose une interface neutre, accessible
à tout MJ francophone quel que soit son univers de prédilection — et donc entièrement
compréhensible sans connaissance d'une langue étrangère.

Enfin, la promesse de « friction d'entrée nulle » (vision §5) n'est tenue que si l'interface
est immédiatement lisible. Une friction linguistique est une friction d'adoption.

---

## Portée et hors-portée

**Ce que cette exigence couvre :**

- L'ensemble des textes produits par l'application et visibles à l'utilisateur dans les
  parcours normaux : libellés de boutons et d'actions, titres de sections, messages de
  confirmation, messages d'erreur et d'avertissement, textes d'aide, notifications,
  invites, textes des états vides.
- Les textes visibles aux joueurs accédant via un lien de session, dans les mêmes conditions.
- La cohérence et la correction du français employé — pas seulement la présence de texte
  en français, mais sa qualité (absence de fautes manifestes, cohérence terminologique
  sur l'ensemble des parcours).

**Ce que cette exigence ne couvre pas :**

- Le contenu créé par le MJ ou les joueurs (titres de documents, notes, descriptions de
  PNJ, noms de lieux) — ce contenu est libre de langue, comme l'exprime NFR-I18N-03.
- La langue de l'interface dans les versions futures du produit : cette exigence est scoped
  au MVP. L'extension vers d'autres langues d'interface est couverte par NFR-I18N-02.
- Les métadonnées techniques non visibles à l'utilisateur dans l'expérience normale du produit.

**Périmètre MVP assumé :** l'interface est entièrement en français au MVP. Aucun autre marché
linguistique n'est engagé pour cette version.

---

## Critères d'acceptation produit

- En parcourant l'ensemble des écrans accessibles au MJ — création de campagne, préparation
  de scénario, vue session, recherche, partage d'informations, gestion du compte — aucun
  texte en langue étrangère n'apparaît dans les libellés, titres, boutons, messages ou
  notifications produits par l'application.
- En parcourant l'ensemble des écrans accessibles au joueur — accès via lien de session,
  consultation des informations partagées — aucun texte en langue étrangère n'apparaît
  dans les éléments d'interface produits par l'application.
- Dans les états exceptionnels visibles à l'utilisateur (aucun résultat de recherche,
  perte de connexion, espace de stockage sous pression, confirmation de suppression de
  compte), les messages affichés sont en français et compréhensibles sans connaissance
  préalable de l'outil.
- Un utilisateur dont le français est la seule langue maîtrisée peut accomplir l'ensemble
  des parcours utilisateur documentés — UC-01 à UC-14 — sans rencontrer de texte
  incompréhensible produit par l'interface.

---

## Traçabilité montante

**Use cases servis :**

- [UC-01 — Mode local sans compte](../usecases/UC-01-mode-local-sans-compte.md) — parcours
  d'entrée sans inscription, premier contact avec l'interface.
- [UC-02 — Créer un espace de jeu](../usecases/UC-02-creer-espace-jeu.md) — création de
  campagne, libellés et confirmations du parcours de démarrage.
- [UC-06 — Vue session](../usecases/UC-06-vue-session.md) — interface à la plus forte
  densité de libellés actifs pendant une partie.
- [UC-09 — Accès session joueur](../usecases/UC-09-acces-session-joueur.md) — interface
  visible par les joueurs accédant via lien de session.
- [UC-10 — Compte cloud](../usecases/UC-10-compte-cloud.md) — messages d'inscription,
  confirmations et notifications liées au compte.
- [UC-14 — Recherche](../usecases/UC-14-recherche.md) — textes des états vides, suggestions,
  libellés de filtres.

**User stories servies :**

- [US-UC-01 — Mode local sans compte](../user-stories/US-UC-01-mode-local-sans-compte.md)
- [US-UC-06 — Vue session](../user-stories/US-UC-06-vue-session.md)
- [US-UC-09 — Accès session joueur](../user-stories/US-UC-09-acces-session-joueur.md)
- [US-UC-10 — Compte cloud](../user-stories/US-UC-10-compte-cloud.md)
- [US-UC-14 — Recherche](../user-stories/US-UC-14-recherche.md)
