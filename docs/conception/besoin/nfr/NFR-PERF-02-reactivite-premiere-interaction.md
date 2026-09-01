# NFR-PERF-02 — Réactivité à la première interaction

Famille **Performance perçue** · [Index de conception](../../README.md)

---

## Énoncé normatif

L'application s'affiche et devient utilisable dès que le MJ ou le joueur l'ouvre, sans écran
de chargement prolongé qui retarderait le début d'une session.

---

## Raison d'être

Le différenciant central de Haversack est la « friction d'entrée nulle » (vision §5) : un MJ
peut ouvrir l'application et commencer à travailler sans inscription, sans configuration
initiale, sans attente. Cette promesse s'effondre dès l'ouverture si l'utilisateur est
accueilli par un écran de chargement prolongé avant de pouvoir interagir avec son contenu.

**[Nadia](../persona/persona-04-nadia.md)** prépare tard le soir, en 30 à 45 minutes, souvent
sous contrainte de temps. Sa question décisive avant d'adopter un outil est : « combien de temps
pour que ça soit utilisable ? » Un démarrage lent consomme une fraction précieuse de son temps
de préparation et signale que l'outil n'est pas fait pour elle. Le retour après six semaines
d'absence (persona-04 : « Que voit-on quand on revient après 6 semaines d'absence ? ») est
également une situation d'ouverture à froid — l'application doit restituer l'état où elle en
était sans délai perceptible.

**[Sonia](../persona/persona-07-sonia.md)** lance un one-shot en trente secondes chrono
(vision §2.2 : « lancer en moins de 30 secondes depuis une bibliothèque de scénarios »). Dans
ce contexte, le temps d'ouverture de l'application représente une fraction non négligeable du
parcours complet. Si l'application tarde à devenir utilisable, le compte à rebours n'est plus
tenable.

L'hypothèse produit H1 (vision §2.3) mesure l'activation préparation dans les quatorze jours
suivant le premier usage. Un démarrage lent dès la première ouverture peut empêcher un MJ
d'atteindre cette activation — il abandonne avant même d'avoir pu évaluer la valeur du produit.

Cette exigence couvre deux moments distincts : l'ouverture initiale pour un nouvel utilisateur,
et la réouverture par un utilisateur déjà actif qui retrouve son contenu existant.

---

## Portée et hors-portée

**Ce que cette exigence couvre :**

- L'ouverture de l'application par le MJ depuis un appareil où le contenu est déjà présent
  (mode local : campagnes créées localement ; mode cloud : compte actif avec campagnes
  synchronisées).
- L'ouverture par un joueur accédant via un lien de session — il doit pouvoir consulter les
  informations partagées sans traverser une phase d'attente prolongée.
- La première ouverture sur un appareil pour un nouvel utilisateur — l'interface doit être
  opérationnelle immédiatement, même si le contenu est vide.

**Ce que cette exigence ne couvre pas :**

- La navigation entre les écrans une fois l'application ouverte — couverte par NFR-PERF-01
  pour la vue session et NFR-PERF-03 pour les actions en cours de partie.
- Les actions réalisées pendant une session LIVE — couvertes par NFR-PERF-03.
- La synchronisation en arrière-plan d'un compte cloud après ouverture — ce processus peut
  se poursuivre sans bloquer l'interaction de l'utilisateur.
- Le délai de connexion réseau (perte ou lenteur du réseau) — ces situations relèvent des
  exigences hors connexion (NFR-OFF).

---

## Critères d'acceptation produit

**Situation nominale — MJ avec campagnes existantes :**

Le MJ peut commencer à interagir avec son contenu immédiatement après avoir ouvert
l'application, sans attendre une phase de chargement distincte. Ses campagnes et ses documents
sont accessibles dès le premier écran opérationnel.

**Situation nominale — MJ ouvrant l'application pour la première fois :**

L'interface est opérationnelle dès l'ouverture. Le MJ peut créer sa première campagne sans
avoir traversé de phase d'attente prolongée entre l'ouverture et la possibilité d'agir.

**Situation nominale — joueur accédant via un lien de session :**

Le joueur voit les informations partagées par le MJ sans attendre une phase de chargement
distincte entre le clic sur le lien et l'affichage du contenu.

**Situation limite — retour après une longue absence :**

Nadia rouvre l'application après six semaines sans l'avoir utilisée. Ses campagnes s'affichent
dans l'état où elle les a laissées, sans phase d'initialisation perceptible. Elle peut reprendre
là où elle s'était arrêtée immédiatement.

**Situation limite — ouverture depuis un réseau lent :**

Lorsque la connexion réseau est lente, le MJ en mode local peut accéder à son contenu sans
attendre la fin d'une synchronisation. Le contenu local est disponible indépendamment de
l'état du réseau.

---

## Traçabilité montante

| Artefact | Lien |
|---|---|
| UC-06 — Vue session (contexte, déclencheur) | [../usecases/UC-06-vue-session.md](../usecases/UC-06-vue-session.md) |
| US-UC-06 — Epic vue session (US-06-01 : lancement de session) | [../user-stories/US-UC-06-vue-session.md](../user-stories/US-UC-06-vue-session.md) |
| UC-09 — Accès session joueur (accès via lien ponctuel) | [../usecases/UC-09-acces-session-joueur.md](../usecases/UC-09-acces-session-joueur.md) |
| RB-06-02 — Session créée directement en LIVE (pas d'état intermédiaire) | [../user-stories/US-UC-06-vue-session.md](../user-stories/US-UC-06-vue-session.md) |
