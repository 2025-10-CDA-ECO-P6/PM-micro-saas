# NFR-PERF-03 — Continuité sans interruption lors des actions en cours de partie

Famille **Performance perçue** · [Index de conception](../../README.md)

---

## Énoncé normatif

Créer une note à la volée, épingler un document ou partager une information aux joueurs
pendant une session active ne gèle pas l'interface ni n'oblige le MJ à attendre une
confirmation avant de continuer à piloter la session.

---

## Raison d'être

Les actions couvertes par cette exigence — créer un élément à la volée (UC-07) et partager
une information aux joueurs (UC-08) — surviennent au milieu d'une interaction narrative en
cours. Le MJ ne les déclenche pas dans un moment de calme : il les déclenche parce qu'un
joueur vient de faire quelque chose d'inattendu, ou parce qu'il décide à l'instant de
révéler une information.

La douleur prioritaire identifiée en vision §1.1 est précisément le rythme cassé. Toute
interruption de l'interface pendant une action de session — gel visible, indicateur d'attente
bloquant, écran intermédiaire de confirmation — constitue un coupure dans la narration que
les joueurs perçoivent. L'outil, censé réduire la friction, devient lui-même sa source.

UC-07 exprime ce besoin en termes de contexte : « Le MJ doit pouvoir créer un document durable
ou une note de session sans quitter le contexte de session ni bloquer le rythme de jeu. » Le
besoin utilisateur est formulé explicitement : créer un document en quelques secondes depuis
la vue session, avec un minimum d'informations requises, pour maintenir la fluidité de la
partie.

UC-08 expose la même contrainte du côté du partage : le MJ décide de rendre visible un
document aux joueurs pendant la partie. Cette décision n'appelle pas un délai d'attente —
elle appelle une exécution immédiate qui lui permette de poursuivre sans rupture.

L'exigence vaut pour les deux profils qui usent le plus intensément de la vue session en
mode dynamique. **[Thomas](../persona/persona-01-thomas.md)** a une organisation préparée
et bien structurée ; ce qui lui manquait, c'est une vue propre pendant la session sans
avoir à jongler — un gel de l'interface à chaque action ramènerait exactement cette friction.
**[Nadia](../persona/persona-04-nadia.md)** lance et pilote sa session en moins de temps que
Thomas ; tout ralentissement de l'interface en cours de partie lui coûte proportionnellement
plus cher.

---

## Portée et hors-portée

**Ce que cette exigence couvre :**

- La création à la volée d'une note de session, d'un document libre ou d'un document typé
  (PNJ, lieu, etc.) depuis la vue session pendant une session LIVE — UC-07 scénario nominal
  et alternatives A1, A3, A4.
- L'épinglage d'un document en cours de session, qu'il soit déclenché manuellement ou
  automatiquement lors d'une création à la volée ou d'un partage.
- Le partage d'un document aux joueurs depuis la vue session (UC-08 A3) — l'action de
  basculement de visibilité ne doit pas interrompre l'interface du MJ.
- Le basculement de visibilité d'une note de session de privé MJ vers visible par les joueurs
  (UC-06, US-06-04).

**Ce que cette exigence ne couvre pas :**

- La navigation dans les panneaux configurés et la consultation de documents — couverte par
  NFR-PERF-01.
- Le premier affichage de l'application — couvert par NFR-PERF-02.
- La recherche dans le contenu d'une campagne — couverte par NFR-PERF-04.
- La situation de perte de réseau — couverte par les exigences hors connexion (NFR-OFF-04),
  qui traitent la résilience et la conservation du contenu ; NFR-PERF-03 traite la fluidité
  en conditions normales.
- La création à la volée depuis une session CLOSED (ajout rétroactif, UC-07 A5) — le contexte
  temporel y est différent, sans interaction narrative en cours.

---

## Critères d'acceptation produit

**Situation nominale — création d'une note à la volée :**

Après avoir validé la création d'une note de session (titre seul suffit), le MJ peut
immédiatement effectuer une autre action sans observer de gel ni d'indicateur d'attente
bloquant. La note est référencée dans la session active sans que le MJ ait eu à attendre.

**Situation nominale — création d'un document typé (PNJ, lieu) :**

Après avoir créé un PNJ à la volée avec son seul nom, le document est épinglé dans la
session et disponible pour consultation — le MJ peut continuer sa narration sans pause
visible liée à l'outil.

**Situation nominale — partage d'un document aux joueurs :**

Après avoir déclenché l'action de partage sur un document, le MJ peut immédiatement
effectuer une autre action. Le document devient visible dans la vue joueur sans que le
MJ ait observé de gel de l'interface de son côté.

**Situation nominale — épinglage d'un document :**

L'épinglage d'un document, qu'il soit manuel ou automatique, ne crée pas de moment
d'attente. Le document apparaît dans le panneau Documents épinglés sans interruption
perceptible du contexte de session.

**Situation limite — plusieurs actions rapprochées :**

Un MJ qui enchaîne plusieurs actions en peu de temps — créer une note, épingler un
document, partager une information — ne voit pas l'interface ralentir cumulativement.
Chaque action reste aussi fluide que la première.

---

## Traçabilité montante

| Artefact | Lien |
|---|---|
| UC-07 — Créer un élément à la volée pendant la session | [../usecases/UC-07-creation-volee-session.md](../usecases/UC-07-creation-volee-session.md) |
| UC-08 — Partager une information aux joueurs | [../usecases/UC-08-partager-information.md](../usecases/UC-08-partager-information.md) |
| UC-06 — Vue session (A2, A4, phases 3) | [../usecases/UC-06-vue-session.md](../usecases/UC-06-vue-session.md) |
| US-UC-07 — Epic création à la volée | [../user-stories/US-UC-07-creation-volee-session.md](../user-stories/US-UC-07-creation-volee-session.md) |
| US-UC-08 — Epic partage d'information | [../user-stories/US-UC-08-partager-information.md](../user-stories/US-UC-08-partager-information.md) |
| US-UC-06 — Epic vue session (US-06-04 : notes de session MJ, US-06-05 : épinglage) | [../user-stories/US-UC-06-vue-session.md](../user-stories/US-UC-06-vue-session.md) |
| RB-06-13 — Note visible par les joueurs immédiatement | [../user-stories/US-UC-06-vue-session.md](../user-stories/US-UC-06-vue-session.md) |
| RB-06-17 — Épinglage automatique à la création à la volée et au partage | [../user-stories/US-UC-06-vue-session.md](../user-stories/US-UC-06-vue-session.md) |
