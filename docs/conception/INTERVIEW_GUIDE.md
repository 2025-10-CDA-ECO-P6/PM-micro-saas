# Guide d'entretien utilisateur — Haversack

> Document de référence pour la conduite des interviews utilisateurs pré-développement et pré-lancement MVP.
>
> Use cases source : [usecases/README.md](usecases/README.md) — Vision produit : [vision/vision-produit.md](vision/vision-produit.md)

---

## Table des matières

1. [Objectif des interviews](#1-objectif-des-interviews)
2. [Profils à interviewer](#2-profils-à-interviewer)
3. [Questions par thème](#3-questions-par-thème)
4. [Piliers critiques — Hypothèses et questions structurantes](#4-piliers-critiques--hypothèses-et-questions-structurantes)
5. [Questions prioritaires (top 10)](#5-questions-prioritaires-top-10)
6. [Anti-patterns à éviter](#6-anti-patterns-à-éviter)
7. [Template de compte-rendu](#7-template-de-compte-rendu)
8. [Résultats de la campagne d'entretiens](#8-résultats-de-la-campagne-dentretiens)

---

## 1. Objectif des interviews

Les interviews cherchent à comprendre les **comportements actuels** des maîtres de jeu et des joueurs : comment ils préparent une campagne aujourd'hui, comment ils pilotent une session, comment ils partagent des informations avec leur groupe, et quels outils ils utilisent réellement. L'objectif est de valider ou d'invalider des hypothèses de conception déjà formulées — pas de découvrir le marché depuis zéro.

Ce que ces interviews ne cherchent pas : recueillir des listes de fonctionnalités souhaitées, valider l'intérêt pour un produit inexistant, ou tester des maquettes. Les questions sur les préférences futures ("est-ce que vous utiliseriez...") sont à éviter. On s'intéresse aux habitudes réelles, aux frictions vécues et aux contournements inventés.

---

## 2. Profils à interviewer

### MJ débutant (moins de 2 ans d'expérience)

- **Nombre recommandé** : 3 interviews
- **Critères** : a maîtrisé au moins une campagne ou un one-shot complet, utilise un outil numérique pour ses notes (même basique)
- **Où les trouver** : forums Reddit r/jdrfr, Discord JDR francophones, groupes Facebook JDR débutants, boutiques de jeux de société

### MJ expérimenté (5 ans et plus, campagnes longues)

- **Nombre recommandé** : 4 interviews
- **Critères** : gère au minimum une campagne active avec un groupe stable, a un système de prise de notes établi, a testé au moins deux outils différents (Notion, Obsidian, Google Docs, Roll20...)
- **Où les trouver** : communautés spécialisées (Casus Belli, forum Donjon, Discord campagnes longues), conventions JDR

### MJ convention / one-shot (animateur régulier)

- **Nombre recommandé** : 3 interviews
- **Critères** : anime des one-shots en convention ou soirées découverte, rejouent les mêmes scénarios avec des groupes différents, catalogue de scénarios existant
- **Où les trouver** : associations JDR locales, conventions (Toulouse Game Show, Paris Est Ludique, Octogônes), réseaux d'animateurs bénévoles

### Joueur régulier (avec compte ou habitudes numériques)

- **Nombre recommandé** : 3 interviews
- **Critères** : participe à une campagne longue, reçoit actuellement des informations de son MJ par un canal numérique (Discord, Google Docs, partage de fichiers)
- **Où les trouver** : via les MJ déjà interviewés (demander à recruter un joueur de leur groupe), forums de joueurs

### Joueur occasionnel / one-shot

- **Nombre recommandé** : 2 interviews
- **Critères** : joue moins de 6 fois par an, majoritairement en one-shot ou conventions, peu ou pas d'outils dédiés
- **Où les trouver** : entourage des MJ convention déjà recrutés, groupes Facebook initiations JDR

---

## 3. Questions par thème

### Organisation du contenu

- Décrivez-moi comment vous organisez vos notes de campagne en ce moment : quels outils, quels dossiers, quelle logique ? [MJ expérimenté, MJ débutant]
- Montrez-moi à quoi ressemble votre espace de notes pour votre campagne la plus active, si vous l'avez sous la main. [MJ expérimenté]
- Quels types de contenus créez-vous en dehors des scénarios — PNJ, lieux, factions, objets ? Comment les rangez-vous ? [MJ expérimenté, MJ débutant]
- Est-ce que vous avez des catégories qui reviennent dans chaque campagne, ou est-ce que ça change complètement selon le système de jeu ? [MJ expérimenté]
- Vous êtes-vous déjà retrouvé à chercher une information que vous saviez avoir notée quelque part et ne plus la retrouver ? Racontez. [MJ expérimenté, MJ débutant]
- Quand vous utilisez Notion, Obsidian ou un équivalent, est-ce que vous avez l'impression de passer du temps à construire votre organisation plutôt qu'à préparer du contenu ? [MJ expérimenté]
- Les MJ qui jouent à des systèmes différents de D&D — comment organisez-vous un jeu comme Blades in the Dark ou Call of Cthulhu ? Ça change quelque chose à votre façon de classer l'information ? [MJ expérimenté]

### Conduite de session

- Racontez-moi comment se passe concrètement le début d'une session pour vous : qu'est-ce que vous avez ouvert devant vous, sur quoi vous appuyez-vous ? [MJ expérimenté, MJ débutant]
- Pendant une session, combien de fois vous interrompez-vous pour chercher quelque chose ? Qu'est-ce que vous cherchez le plus souvent ? [MJ expérimenté]
- Qu'est-ce qui se passe quand les joueurs font quelque chose que vous n'aviez pas prévu — vous créez un PNJ improvisé, un lieu, un indice ? Comment vous le notez à ce moment-là ? [MJ expérimenté, MJ débutant]
- En pleine session, si vous aviez besoin d'accéder à une fiche PNJ ou une description de lieu préparée, combien de temps vous mettez à la retrouver dans votre organisation actuelle ? [MJ expérimenté, MJ débutant]
- Pendant une partie, est-ce qu'il vous arrive de devoir quitter votre système de notes principal pour chercher un détail ailleurs — papier, autre onglet, mémoire ? Racontez un exemple. [MJ expérimenté, MJ débutant]
- Après une session, que faites-vous de vos notes de session — les relisez-vous avant la session suivante ? Où les rangez-vous ? [MJ expérimenté, MJ débutant]
- Avez-vous déjà cherché dans des notes de sessions passées une information précise — du type "dans quelle session est-ce qu'on a mentionné ce PNJ" ? Comment vous faites ça aujourd'hui ? [MJ expérimenté, Nadia - profil casual]
- Sur quoi consultez-vous vos notes pendant une partie : ordinateur, tablette, téléphone, papier ? [tous MJ]

### Partage avec les joueurs

- Comment partagez-vous des informations avec vos joueurs entre les sessions et pendant les sessions ? [MJ expérimenté, MJ débutant]
- Avez-vous déjà partagé une information par accident — un document que les joueurs n'auraient pas dû voir ? Comment ça s'est passé ? [MJ expérimenté]
- Est-ce qu'il vous arrive de vouloir partager une information à un joueur précis mais pas aux autres ? Comment vous gérez ça aujourd'hui ? [MJ expérimenté]
- Quels types d'informations partagez-vous le plus souvent — des fiches de lieux, des résumés de session, des fiches de personnages joueurs, des images ? [MJ expérimenté, MJ débutant]
- Est-ce que vos joueurs reviennent souvent consulter ce que vous avez partagé, ou c'est surtout "in the moment" ? [MJ expérimenté]

### Accès joueur

- Quand vous partagez quelque chose avec vos joueurs via Discord ou un lien, est-ce qu'ils cliquent dessus ? Qu'est-ce qui les freine ? [MJ expérimenté]
- Est-ce que vous avez déjà essayé de partager un outil ou un espace numérique avec votre groupe et rencontré des résistances ? Racontez. [MJ expérimenté, MJ débutant]
- En tant que joueur, quand votre MJ partage un document ou un lien, qu'est-ce qui vous donne envie de l'ouvrir ou non ? [Joueur régulier, Joueur occasionnel]
- Est-ce que vous accédez aux informations de campagne depuis votre téléphone ou plutôt depuis un ordinateur ? [Joueur régulier, Joueur occasionnel]
- Avez-vous déjà eu besoin d'accéder à une fiche de personnage ou à des notes de campagne entre deux sessions ? Comment vous faites ça ? [Joueur régulier]
- Dans un one-shot en convention, est-ce que vous auriez envie de recevoir des informations en direct sur votre téléphone pendant la partie ? Qu'est-ce que ça changerait ? [Joueur occasionnel, MJ convention]

### Réutilisation de contenu

- Est-ce que vous rejouez des scénarios que vous avez déjà maîtrisés ? Si oui, comment vous préparez la deuxième fois ? [MJ convention, MJ expérimenté]
- Avez-vous un "catalogue" de vos scénarios — même informel — ou chaque one-shot existe de façon indépendante ? [MJ convention]
- Quand vous relancez un scénario avec un nouveau groupe, qu'est-ce que vous voulez garder exactement identique, et qu'est-ce que vous adaptez ? [MJ convention]
- Avez-vous déjà voulu utiliser un PNJ ou un lieu créé pour une campagne dans une autre ? Comment vous avez géré ça ? [MJ expérimenté, Antoine - multi-campagnes]
- Est-ce que vous utilisez des templates ou des structures de documents récurrentes — une fiche PNJ standard, un plan de scène type ? [MJ expérimenté, MJ débutant]

### Mode hors-ligne vs cloud

- Avez-vous peur de perdre vos notes de campagne ? Est-ce que ça vous est déjà arrivé ? [tous MJ]
- Est-ce que vous préparez vos sessions depuis plusieurs appareils ou depuis un seul ? [MJ expérimenté, MJ débutant]
- Si un outil conservait vos données uniquement dans votre navigateur — sans compte, sans serveur — et vous proposait une sauvegarde manuelle, est-ce que ce serait suffisant pour commencer à l'utiliser ? Qu'est-ce qui vous ferait douter ? [MJ débutant, Nadia - profil casual]
- À quel moment vous dites-vous "il faut que je crée un compte" pour un outil — qu'est-ce qui déclenche ce moment ? [tous MJ]
- Si vous utilisiez un outil depuis plusieurs semaines et que quelqu'un vous proposait de synchroniser vos données dans le cloud, quel serait votre premier réflexe ? [MJ expérimenté, MJ débutant]

### Onboarding et premier lancement

- Quand vous essayez un nouvel outil, qu'est-ce qui fait que vous l'abandonnez dans les premières heures ? [tous MJ]
- Avez-vous déjà adopté un outil pour votre préparation JDR et ensuite l'avez abandonné ? Pourquoi ? [MJ expérimenté]
- Combien de temps seriez-vous prêt à passer à configurer un outil avant de pouvoir créer votre première note ? [tous MJ]
- Si un outil vous proposait 4 dossiers par défaut à la création d'une campagne — "Personnages", "Scénarios", "Joueurs", "Notes" — est-ce que ça vous aiderait ou est-ce que vous préféreriez partir d'une page vide ? [MJ expérimenté, MJ débutant]
- Avez-vous déjà été convaincu d'essayer un outil par votre MJ ou par un joueur de votre groupe ? Qu'est-ce qui vous a décidé ? [Joueur régulier, Joueur occasionnel]

---

## 4. Piliers critiques — Hypothèses et questions structurantes

Les deux hypothèses centrales du MVP (vision §2.3) doivent être validées en priorité par cette campagne d'entretiens. Cette section regroupe, pour chacun des deux piliers, l'hypothèse exacte, les questions à poser, et le signal attendu en réponse.

### Pilier 1 — Préparation sans friction

**Hypothèse à valider** : Un MJ peut créer une campagne structurée et y retrouver ses informations sans friction d'onboarding — spécifiquement, sans compte obligatoire au démarrage, en utilisant les données locales du navigateur comme point d'entrée naturel.

**Questions structurantes** :
- Quand vous essayez un nouvel outil, qu'est-ce qui fait que vous l'abandonnez dans les premières heures ? [§3.7 Onboarding et premier lancement — cible MJ débutant, MJ casual]
- Combien de temps seriez-vous prêt à passer à configurer un outil avant de pouvoir créer votre première note ? [§3.7 Onboarding et premier lancement]
- À quel moment vous dites-vous "il faut que je crée un compte" pour un outil — qu'est-ce qui déclenche ce moment ? [§3.6 Mode hors-ligne vs cloud — top 10 #1]
- Si un outil conservait vos données uniquement dans votre navigateur sans compte, est-ce que ce serait suffisant pour commencer à l'utiliser ? Qu'est-ce qui vous ferait douter ? [§3.6 Mode hors-ligne vs cloud — top 10 #10]
- Si un outil vous proposait 4 dossiers par défaut à la création d'une campagne — "Personnages", "Scénarios", "Joueurs", "Notes" — est-ce que ça vous aiderait ou est-ce que vous préféreriez partir d'une page vide ? [§3.7 Onboarding et premier lancement]

**Signaux attendus en réponse** :
- ✅ **Pilier validé si** : le MJ accepte de commencer sans compte — soit immédiatement, soit après 10-15 minutes d'utilisation ; le temps de configuration initial (structure des dossiers) ne dépasse pas 5 minutes ; les dossiers par défaut aident plus qu'ils ne bloquent.
- ❌ **Pilier invalidé si** : le MJ exige un compte obligatoire pour se sentir en sécurité ; la friction d'onboarding est présente au-delà de 15 minutes ; la peur de perdre les données locales est un frein majeur non résolvable par un simple rappel visuel.
- ⚠️ **Non tranchée si** : les réponses sont mitigées ou contradictoires entre MJ débutant et MJ expérimenté — dans ce cas, le profil le plus exigeant dicte l'ajustement.

---

### Pilier 2 — Vue session avec valeur réelle en partie

**Hypothèse à valider** : La vue session apporte une valeur réelle pendant une partie — réduction mesurable du temps de recherche, accès fluide au contenu préparé, capacité de création à la volée sans rupture.

**Questions structurantes** :
- Racontez-moi comment se passe concrètement le début d'une session pour vous : qu'est-ce que vous avez ouvert devant vous, sur quoi vous appuyez-vous ? [§3.2 Conduite de session]
- Pendant une session, combien de fois vous interrompez-vous pour chercher quelque chose ? Qu'est-ce que vous cherchez le plus souvent ? [§3.2 Conduite de session — top 10 #7]
- Qu'est-ce qui se passe quand les joueurs font quelque chose que vous n'aviez pas prévu — vous créez un PNJ improvisé, un lieu, un indice ? Comment vous le notez à ce moment-là ? [§3.2 Conduite de session — top 10 #7]
- En pleine session, si vous aviez besoin d'accéder à une fiche PNJ ou une description de lieu préparée, combien de temps vous mettez à la retrouver dans votre organisation actuelle ? [MJ expérimenté, MJ débutant]
- Pendant une partie, est-ce qu'il vous arrive de devoir quitter votre système de notes principal pour chercher un détail ailleurs — papier, autre onglet, mémoire ? Racontez un exemple. [MJ expérimenté, MJ débutant]
- Sur quoi consultez-vous vos notes pendant une partie : ordinateur, tablette, téléphone, papier ? [§3.2 Conduite de session]

**Signaux attendus en réponse** :
- ✅ **Pilier validé si** : le MJ interrompt la session **plus de 2 fois** pour chercher une information ; le temps de recherche moyen dépasse **30 secondes** ; il existe actuellement des tâches de création à la volée (PNJ, lieux) qui **cassent le rythme** de jeu ; l'accès au contenu préparé est fragmenté (plusieurs outils/emplacements) ; le MJ reconnaît que centraliser cet accès serait une amélioration directe.
- ❌ **Pilier invalidé si** : le MJ ne rencontre jamais d'interruption pour chercher (organisation déjà très efficace) ; la création à la volée est rapide et n'interfère pas avec le rythme ; tous les contenus sont déjà centralisés et accessibles d'un seul clic.
- ⚠️ **Non tranchée si** : le problème existe mais est marginal (< 2 interruptions par session) ; le MJ n'est pas convaincu qu'une vue dédiée changerait son flux actuel.

---

## 5. Questions prioritaires (top 10)

Les questions suivantes couvrent les incertitudes les plus critiques identifiées lors de la revue des use cases.

| # | Question | Profil cible | Hypothèse à valider ou invalider |
|---|---|---|---|
| 1 | À quel moment vous dites-vous "il faut que je crée un compte" pour un outil — qu'est-ce qui déclenche ce moment ? | MJ débutant, MJ casual (Nadia) | H : le déclencheur naturel est le partage avec les joueurs, pas la peur de perdre des données. Si faux, le message "risque de perte" doit être plus présent en mode local (UC-01). |
| 2 | Quand vous relancez un scénario avec un nouveau groupe, qu'est-ce que vous voulez garder exactement identique, et qu'est-ce que vous adaptez ? | MJ convention (Sonia) | H : Sonia veut un "instantané" du scénario source, pas un diff. Si faux, un mécanisme de comparaison source/instance est nécessaire (UC-13). |
| 3 | Avez-vous un "catalogue" de vos scénarios — même informel — ou chaque one-shot existe de façon indépendante ? | MJ convention (Sonia) | H : Sonia a une organisation mentale par scénario, pas par campagne. Si vrai, le point d'entrée "Bibliothèque" du tableau de bord est prioritaire sur la vue "Campagnes". |
| 4 | Les MJ font-ils une séparation mentale claire entre "campagne" et "one-shot" avant de lancer l'application ? | MJ débutant, MJ convention | H : la distinction campagne / one-shot est évidente pour l'utilisateur. Si faux, le choix au démarrage risque de bloquer l'onboarding (UC-02). |
| 5 | Avez-vous cherché dans des notes de sessions passées une information précise ? Comment vous faites ça aujourd'hui ? | MJ casual (Nadia), MJ expérimenté | H : la recherche dans les notes de sessions archivées est un besoin réel non couvert par la navigation (UC-14 périmètre MVP à trancher). |
| 6 | Est-ce qu'il vous arrive de vouloir partager une information à un joueur précis mais pas aux autres ? Comment vous gérez ça aujourd'hui ? | MJ expérimenté (Antoine) | H : le partage par groupe (tous les joueurs) suffit pour le MVP. Si faux, la granularité par joueur ou personnage doit être réévaluée (UC-08, UC-09). |
| 7 | Pendant une session, qu'est-ce qui se passe quand les joueurs font quelque chose que vous n'aviez pas prévu — comment vous le notez à ce moment-là ? | MJ expérimenté, MJ débutant (Émilie) | H : la création à la volée (UC-07) est un besoin fort pendant la session. Valide l'importance du panneau de création rapide. |
| 8 | En tant que joueur, est-ce que vous accédez aux informations de campagne depuis votre téléphone ou plutôt depuis un ordinateur ? | Joueur régulier (Lucas), Joueur occasionnel | H : les joueurs accèdent depuis mobile. Si vrai, la vue joueur mobile est critique pour le MVP (UC-06, UC-09 — angle mort identifié). |
| 9 | Avez-vous déjà essayé de partager un outil numérique avec votre groupe et rencontré des résistances ? Racontez. | MJ expérimenté, MJ débutant | H : l'obligation de création de compte côté joueur est le premier frein d'adoption. Valide ou invalide le choix "accès joueur sans compte" (UC-09, UC-12). |
| 10 | Si un outil conservait vos données uniquement dans votre navigateur sans compte, est-ce que ce serait suffisant pour commencer à l'utiliser ? | MJ débutant, MJ casual (Nadia, Rémi) | H : le mode local sans compte réduit la friction d'onboarding. Si faux, il faut revoir le message de risque de perte de données ou proposer un export plus lisible dès le départ (UC-01). |

---

## 6. Anti-patterns à éviter

**Questions fermées déguisées en questions ouvertes**
A éviter : "Est-ce que vous utiliseriez une fonctionnalité qui vous permet de partager vos notes directement depuis l'outil ?"
Reformuler : "Comment partagez-vous vos notes avec vos joueurs aujourd'hui ?"

**Présenter le produit avant de recueillir les comportements**
Ne pas montrer de maquette ou décrire Haversack avant d'avoir posé les questions sur les habitudes actuelles. La description du produit biaiserait toutes les réponses suivantes.

**Demander ce que l'utilisateur ferait dans une situation hypothétique**
A éviter : "Si l'application avait un mode hors-ligne, est-ce que ça vous conviendrait ?"
Les déclarations d'intention ne prédisent pas les comportements. Ancrer toujours la question dans une expérience vécue.

**Valider ses propres hypothèses en les posant directement**
A éviter : "On pense que les MJ perdent beaucoup de temps à chercher leurs notes pendant une session — c'est votre cas ?"
Formuler : "Racontez-moi ce que vous faites quand vous cherchez une information pendant une partie."

**Couper les silences**
Un silence après une réponse est souvent le signe que l'interviewé n'a pas encore dit l'essentiel. Attendre avant de relancer.

**Se laisser entraîner sur le terrain des fonctionnalités**
Si l'interviewé dit "ce qu'il me faudrait c'est une fonctionnalité X", ne pas rebondir sur X. Revenir au comportement : "Et aujourd'hui, comment vous faites sans X ?"

**Mener l'entretien comme un questionnaire**
Les questions du guide sont des points d'entrée, pas une liste à cocher. Une anecdote riche vaut mieux qu'une réponse à chaque question.

**Recruter uniquement dans son entourage immédiat**
Les premières personnes disponibles (amis, famille) ont tendance à valider pour être agréables. Viser des inconnus ou des contacts distants.

---

## 7. Template de compte-rendu

Utiliser ce template pour chaque interview réalisé. Stocker les fichiers dans `docs/interviews/YYYY-MM-DD-[prénom-profil].md`.

```markdown
# Interview — [Prénom ou pseudonyme] — [Date]

## Profil

- Profil : [MJ débutant / MJ expérimenté / MJ convention / Joueur régulier / Joueur occasionnel]
- Expérience JDR : [durée, systèmes joués]
- Outils actuels : [liste]
- Contexte de jeu : [campagne longue / one-shot / convention / mixte]
- Durée de l'entretien :

## Résumé (3-5 phrases)

[Ce que cette personne fait réellement, ce qui la freine, ce qui la motive.]

## Comportements clés observés

- [Comportement 1 — ancré dans un exemple concret de l'entretien]
- [Comportement 2]
- [Comportement 3]

## Verbatims marquants

> "[Citation exacte]" — contexte : [ce qui a été demandé]

> "[Citation exacte]" — contexte : [ce qui a été demandé]

## Hypothèses validées

| Hypothèse | Statut | Justification |
|---|---|---|
| [Hypothèse issue du top 10] | Validée / Invalidée / Non tranchée | [Ce que l'interviewé a dit] |

## Angles morts révélés

[Besoins ou comportements non anticipés dans les use cases. Référencer les UC concernés si possible.]

## Questions restées sans réponse

[Ce qu'on n'a pas réussi à explorer dans cet entretien.]

## Actions suggérées

- [ ] [Ajustement de conception ou de priorisation à considérer]
- [ ] [UC ou user story à revoir]
```

---

## 8. Résultats de la campagne d'entretiens

Cette section centralise les résultats de la campagne de validation des hypothèses pré-MVP. À remplir à mesure que les entretiens sont menés.

### 8.1 Suivi des entretiens réalisés

*(à remplir en terrain)*

| Date | Profil | Prénom / pseudonyme | Fichier de compte-rendu | Notes rapides |
|---|---|---|---|---|
| [YYYY-MM-DD] | [MJ débutant / MJ expérimenté / MJ convention / Joueur régulier / Joueur occasionnel] | [Prénom] | `docs/interviews/YYYY-MM-DD-[prénom-profil].md` | [Remarque d'orientation initiale, si pertinent] |
| | | | | |
| | | | | |

**Objectif de quota** : 15 entretiens cibles (3 MJ débutant + 4 MJ expérimenté + 3 MJ convention + 3 joueur régulier + 2 joueur occasionnel).

---

### 8.2 Synthèse des hypothèses de la vision

Évaluation du statut des 5 hypothèses du MVP ([vision/vision-produit.md §2.3](vision/vision-produit.md)).

*(à remplir en terrain)*

#### H1 — Préparation sans friction

**Énoncé** : Un MJ peut créer une campagne structurée et y retrouver ses informations sans friction d'onboarding — pas de compte obligatoire au démarrage.

| Statut | Justification croisée | Verbatims clés | Actions |
|---|---|---|---|
| Validée / Invalidée / Non tranchée | [Synthèse des patterns observés + nombre de MJ validant] | > "[Citations des interviewés]" | [Décisions de conception à reconsidérer] |

---

#### H2 — Vue session avec valeur réelle

**Énoncé** : La vue session apporte une valeur réelle pendant une partie — réduction du temps de recherche, accès au contenu préparé, création à la volée.

| Statut | Justification croisée | Verbatims clés | Actions |
|---|---|---|---|
| Validée / Invalidée / Non tranchée | [Synthèse des patterns observés + nombre de MJ validant] | > "[Citations des interviewés]" | [Décisions de conception à reconsidérer] |

---

#### H3 — Partage fluide avec les joueurs

**Énoncé** : Le partage d'informations aux joueurs est plus fluide que les solutions actuelles (Discord, Google Docs, papier).

| Statut | Justification croisée | Verbatims clés | Actions |
|---|---|---|---|
| Validée / Invalidée / Non tranchée | [Synthèse des patterns observés + nombre de MJ/joueurs validant] | > "[Citations des interviewés]" | [Décisions de conception à reconsidérer] |

---

#### H4 — Accès joueur sans compte

**Énoncé** : L'accès joueur sans compte n'est pas un frein à l'adoption du groupe entier.

| Statut | Justification croisée | Verbatims clés | Actions |
|---|---|---|---|
| Validée / Invalidée / Non tranchée | [Synthèse des patterns observés + nombre de joueurs acceptant] | > "[Citations des interviewés]" | [Décisions de conception à reconsidérer] |

---

#### H5 — Conversion naturelle vers le compte payant

**Énoncé** : La conversion du mode local vers un compte payant se produit naturellement quand le besoin de partage ou de cloud apparaît.

| Statut | Justification croisée | Verbatims clés | Actions |
|---|---|---|---|
| Validée / Invalidée / Non tranchée | [Synthèse des patterns observés + déclencheurs identifiés] | > "[Citations des interviewés]" | [Décisions de conception à reconsidérer] |

---

### 8.3 Angles morts révélés

*(à remplir en terrain)*

Besoins ou comportements non anticipés dans les use cases ou la vision, mais qui ont émergé lors des entretiens. Référencer les UC ou sections de vision concernés.

| Angle mort | Fréquence | Profils affectés | Implication potentielle |
|---|---|---|---|
| [Comportement / besoin inattendu] | [N entretiens] | [MJ débutant / expérimenté / joueur…] | [Risque pour le MVP ou opportunité de pivot] |
| | | | |
| | | | |

---

### 8.4 Décisions de conception à reconsidérer

*(à remplir en terrain)*

Sur la base des entretiens, indiquer les points de la conception actuellement documentée (vision, use cases) qui demandent un réajustement — pas un changement complet, mais une inflexion.

| Élément à reconsidérer | Raison (issue de quel entretien) | Recommandation |
|---|---|---|
| [Exemple : "L'obligation de choisir Campagne vs One-shot au démarrage"] | [Profil affecté, pattern observé] | [Option A / Option B / Garder tel quel avec ajustement UX] |
| | | |
| | | |

---

### 8.5 Conclusion de la campagne

*(à remplir en terrain)*

**Date de clôture** : [YYYY-MM-DD]

**Synthèse générale** : [2-4 phrases récapitulant les principaux apprentissages et le signal go/no-go pour le MVP.]

**Signaux pour la priorisation MVP** :
- Hypothèses validées : [liste]
- Hypothèses à affiner : [liste]
- Hypothèses invalides ou en tension : [liste]

**Recommandations pour l'équipe de développement** : [Points clés de priorisation, de design ou d'architecture découlant directement des entretiens.]
