# NFR-I18N-02 — Évolutivité vers d'autres langues sans refonte de l'expérience

Famille **Internationalisation** · [Index de conception](../../README.md)

---

## Énoncé normatif

Le produit est conçu de sorte qu'ajouter une langue d'interface supplémentaire soit un
travail de traduction, non un travail de reconception de l'expérience utilisateur.
La disposition des écrans et le déroulement des parcours restent identiques quelle que soit
la longueur des libellés.

---

## Raison d'être

La communauté du jeu de rôle est internationale. Les univers de référence, les systèmes de jeu
et les groupes de joueurs ne s'arrêtent pas aux frontières linguistiques. Haversack est
agnostique au système de jeu (vision §2.2) — cette même logique d'ouverture s'applique à
la langue : un produit dont l'expérience utilisateur peut accueillir d'autres langues
sans refonte élargit son audience potentielle sans rien sacrifier de son expérience actuelle.

L'enjeu de cette exigence n'est pas d'ouvrir d'autres marchés linguistiques au MVP —
ce n'est pas prévu et n'est pas l'objet de cet énoncé. L'enjeu est de ne pas construire,
dès maintenant, une expérience qui rendrait cette extension structurellement difficile.
Une mise en page qui casse avec des libellés plus longs, un parcours dont l'enchaînement
dépend de la longueur du texte, une disposition d'écrans figée autour du français :
chacun de ces défauts oblige, le jour où l'extension est souhaitée, à reconcevoir
l'expérience plutôt qu'à simplement traduire les textes. C'est un coût que cette
exigence a pour objet d'éviter.

Cette exigence protège l'investissement de conception initial. Les parcours utilisateur
documentés — UC-01 à UC-14, les user journeys, les critères d'acceptation des user stories —
représentent un corpus de conception stabilisé. Si chaque nouvelle langue d'interface
imposait de remettre en question ce corpus, la valeur de cet investissement serait
partiellement détruite. L'exigence garantit que la conception actuelle est réutilisable
telle quelle pour une future extension linguistique.

Les langues présentent des variations de longueur significatives pour les mêmes notions.
Un libellé concis en français peut être sensiblement plus long en allemand ou en portugais,
et plus court en mandarin. Une mise en page qui ne tolère pas cette variation oblige,
à chaque extension linguistique, à retravailler la disposition des éléments d'interface.
Cette exigence garantit que la conception d'interface intègre dès maintenant cette
variabilité comme une contrainte normale.

---

## Portée et hors-portée

**Ce que cette exigence couvre :**

- La conception de la mise en page et de la disposition des éléments d'interface : elle doit
  rester fonctionnelle et cohérente avec des libellés sensiblement plus longs ou plus courts
  qu'en français, sans que des éléments se chevauchent, se tronquent ou débordent de leur
  zone d'affichage.
- L'enchaînement et la logique des parcours utilisateur : ils ne doivent pas dépendre de la
  longueur ou de la forme grammaticale des textes d'interface.
- Les messages d'état et de notification : leur mise en page doit accommoder des formulations
  de longueur variable sans casser la lisibilité.

**Ce que cette exigence ne couvre pas :**

- Le lancement d'une interface dans une autre langue que le français : ce n'est ni planifié
  ni engagé dans le MVP.
- La traduction effective du contenu de l'interface : le travail de traduction lui-même
  est hors périmètre de cette exigence, qui couvre uniquement la capacité à l'accueillir.
- Les formats de date, d'heure, de nombre ou de monnaie : leur adaptation pour d'autres
  marchés est une question d'implémentation, hors de la couche besoin.
- Le contenu créé par le MJ ou les joueurs : entièrement libre de langue, couvert par
  NFR-I18N-03.

**Périmètre MVP assumé :** l'interface est entièrement en français au MVP. Aucun autre marché
linguistique n'est engagé pour cette version. Cette exigence exprime que l'extension doit
rester possible sans travaux structurels, pas qu'elle est planifiée.

---

## Critères d'acceptation produit

- Une nouvelle langue peut être proposée dans l'interface sans que les parcours utilisateur
  ni la disposition des écrans changent, et sans mise en page brisée — y compris avec des
  libellés sensiblement plus longs ou plus courts qu'en français.
- En remplaçant les libellés de l'interface par des équivalents dont la longueur varie de
  façon significative par rapport au français (par exemple, libellés deux fois plus longs),
  aucun élément d'interface ne se chevauche avec un autre, ne se tronque sans indication
  visible, ni ne déborde de sa zone d'affichage.
- En remplaçant les libellés de l'interface par des équivalents significativement plus courts
  qu'en français, la mise en page reste lisible et aucune zone d'interface ne présente un
  déséquilibre visuel qui nuirait à la navigation.
- Les parcours utilisateur documentés — de la création de campagne à la vue session en passant
  par la recherche et le partage d'informations — restent intégralement exécutables sans
  adaptation de leur logique d'enchaînement, indépendamment de la longueur des libellés.
- Les messages de confirmation, d'erreur et de notification restent lisibles et compréhensibles
  avec des formulations de longueur variable, sans troncature ou débordement.

---

## Traçabilité montante

**Use cases servis :**

- [UC-01 — Mode local sans compte](../usecases/UC-01-mode-local-sans-compte.md) — parcours
  d'entrée, dont la fluidité ne doit pas dépendre de la longueur des libellés.
- [UC-02 — Créer un espace de jeu](../usecases/UC-02-creer-espace-jeu.md) — parcours de
  création de campagne, mise en page des formulaires et confirmations.
- [UC-06 — Vue session](../usecases/UC-06-vue-session.md) — interface à la plus forte densité
  d'éléments actifs, dont la disposition doit tolérer la variabilité des libellés.
- [UC-09 — Accès session joueur](../usecases/UC-09-acces-session-joueur.md) — interface
  visible par les joueurs, y compris dans des groupes multilingues.
- [UC-14 — Recherche](../usecases/UC-14-recherche.md) — libellés des filtres, états vides
  et suggestions dont la longueur peut varier selon la langue.

**User stories servies :**

- [US-UC-01 — Mode local sans compte](../user-stories/US-UC-01-mode-local-sans-compte.md)
- [US-UC-02 — Créer un espace de jeu](../user-stories/US-UC-02-creer-espace-jeu.md)
- [US-UC-06 — Vue session](../user-stories/US-UC-06-vue-session.md)
- [US-UC-14 — Recherche](../user-stories/US-UC-14-recherche.md)
