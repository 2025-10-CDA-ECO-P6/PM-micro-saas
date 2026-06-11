# NFR-PERF-01 — Fluidité de la navigation en session

Famille **Performance perçue** · [Index de conception](../README.md)

---

## Énoncé normatif

En cours de partie, le MJ retrouve le document qu'il cherche avant que la conversation à table
n'ait marqué de pause perceptible. Consulter un PNJ, épingler un document ou lancer une
recherche ne crée pas de moment d'attente visible pour les joueurs.

---

## Raison d'être

La douleur centrale que Haversack adresse est décrite en vision §1.1 : « le rythme de jeu est
cassé » lorsque le MJ perd du temps à chercher une information pendant la partie. La vue session
(UC-06) a été conçue précisément pour réduire ce délai ressenti — sa raison d'être complète
tient en une phrase : réduire le délai entre « le MJ cherche une information » et « le MJ
la trouve », sans jamais casser le rythme de la table.

**[Thomas](../persona/persona-01-thomas.md)** est le profil qui illustre ce que le produit
remplace : jongler entre des onglets pendant la partie. Son organisation documentaire est
maîtrisée, mais la friction de navigation en session le contraint à interrompre le flux narratif.
La valeur que Haversack lui propose — une vue session dédiée — s'évanouit si la navigation
dans cette vue introduit elle-même des temps d'attente visibles.

**[Nadia](../persona/persona-04-nadia.md)** représente le cas où l'outil est la seule porte
d'entrée vers le contenu. Elle cherchait dans cinq fichiers avant une session ; si Haversack
lui impose des délais identiques sous une autre forme, le produit ne résout pas sa douleur,
il la déplace.

La vue session est l'interface à la plus forte contrainte temporelle du produit : chaque action
du MJ se produit pendant qu'une conversation réelle se déroule autour de lui. L'enjeu de
cette exigence est que l'outil disparaisse dans le flux narratif — que le MJ n'ait jamais à
signaler à sa table qu'il est en train d'attendre l'application.

---

## Portée et hors-portée

**Ce que cette exigence couvre :**

- Toutes les actions de navigation réalisées par le MJ dans la vue session pendant une session
  en cours (statut LIVE) : consultation d'un document dans un panneau de dossier, épinglage
  d'un document, ouverture d'un document épinglé, lancement de la recherche globale.
- Le MJ utilisant le produit en mode local sans compte et le MJ utilisant un compte cloud,
  dans leurs conditions d'usage normales en session.
- Les interactions décrites dans UC-06 phases 2 et 3 : navigation dans les panneaux configurés,
  consultation de documents en vue condensée, épinglage.

**Ce que cette exigence ne couvre pas :**

- La phase de préparation (hors session LIVE) — le contexte temporel y est différent.
- Le premier affichage de l'application à l'ouverture — couvert par NFR-PERF-02.
- Les actions de création à la volée et de partage pendant la session — couvertes par
  NFR-PERF-03, dont les contraintes de continuité sont distinctes.
- La recherche dans le contenu d'une campagne — couverte par NFR-PERF-04.
- La vue joueur — les joueurs ne pilotent pas la session et ne subissent pas la même
  contrainte de rapidité de navigation.

---

## Critères d'acceptation produit

**Situation nominale — navigation dans un panneau configuré :**

Un observateur assis à la table ne remarque pas que le MJ attend l'application. Le MJ peut
passer d'un panneau à un autre, ouvrir un document en vue condensée et revenir à la
conversation sans que les joueurs aient perçu de pause liée à l'outil.

**Situation nominale — épinglage d'un document :**

Après avoir cliqué sur « Épingler », le document apparaît dans le panneau Documents épinglés
sans que le MJ ait à attendre une confirmation visible. Il peut immédiatement poursuivre
sa narration.

**Situation nominale — ouverture d'un document épinglé :**

Un clic sur un document épinglé l'ouvre dans un panneau de consultation. Le contenu est
lisible avant que la conversation n'ait marqué de pause perceptible.

**Situation limite — campagne riche en contenu :**

Même lorsque la campagne contient un nombre important de documents et de dossiers, la
navigation dans les panneaux configurés reste aussi fluide que pour une campagne peu
alimentée. La densité de contenu n'est pas perçue par le MJ comme une source de lenteur.

**Situation limite — reprise après interruption :**

Lorsque le MJ reprend une session interrompue (UC-06 A5), les panneaux configurés se
restituent dans un délai qui ne crée pas de moment d'attente visible pour les joueurs
présents à la table.

---

## Traçabilité montante

| Artefact | Lien |
|---|---|
| UC-06 — Vue session (contexte, phases 2-3, A3, A6) | [../usecases/UC-06-vue-session.md](../usecases/UC-06-vue-session.md) |
| US-UC-06 — Epic vue session (US-06-01 à US-06-05) | [../user-stories/US-UC-06-vue-session.md](../user-stories/US-UC-06-vue-session.md) |
| RB-06-05 — Sauvegarde automatique de la configuration de vue | [../user-stories/US-UC-06-vue-session.md](../user-stories/US-UC-06-vue-session.md) |
| RB-06-09 — Filtrage des documents par visibilité | [../user-stories/US-UC-06-vue-session.md](../user-stories/US-UC-06-vue-session.md) |
| RB-06-15, RB-06-16 — Épinglage sans effet sur le document source | [../user-stories/US-UC-06-vue-session.md](../user-stories/US-UC-06-vue-session.md) |
