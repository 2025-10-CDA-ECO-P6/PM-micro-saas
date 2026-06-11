# NFR-CONF-01 — Séparation stricte entre notes privées du MJ et vue des joueurs

Famille **Confidentialité** — [Index de conception](../README.md)

---

## Énoncé normatif

Un joueur — qu'il dispose d'un compte ou accède via un lien de session — ne voit jamais les notes que le MJ a gardées privées, quels que soient les documents qu'il explore dans sa vue. Cette séparation est permanente et ne dépend d'aucune action supplémentaire du MJ.

---

## Raison d'être

Haversack repose sur une asymétrie d'information fondamentale : le MJ prépare des informations secrètes — PNJ cachés, intrigues, fins alternatives, révélations à venir — et décide seul de ce qui est révélé aux joueurs, quand et comment. Ce contrôle est une règle métier centrale, non une préférence de confort.

Le différenciant « partage fluide » de la vision (vision §5) est formulé ainsi : *« le MJ contrôle ce qui est visible aux joueurs, document par document, et garde ses notes de préparation privées. »* Ce contrôle n'a de sens que si la frontière entre privé et visible est garantie par la conception du produit — pas simplement configurée par l'utilisateur et exposée à une erreur de manipulation.

**[Thomas](../persona/persona-01-thomas.md)** incarne le cas le plus exigeant : MJ expérimenté préparant une campagne narrative sur plusieurs mois, avec des révélations soigneusement construites, il n'accepterait pas d'utiliser un outil dont la frontière privé/visible dépend d'une attention constante de sa part. Sa question — *« quelle est la valeur si on n'y met que 10 % de ses notes ? »* — révèle que la confiance dans la séparation conditionne la quantité de contenu qu'il confie à l'outil.

**[Lucas](../persona/persona-03-lucas.md)** représente le profil joueur de référence : il accède sans compte, via un lien de session, sans avoir demandé à rejoindre l'outil. La garantie qu'il ne voit que ce que le MJ a explicitement choisi de partager est la condition de confiance de la table entière — pas seulement du MJ.

La vue joueur, telle que définie dans UC-06, affiche uniquement les documents dont la visibilité a été basculée à « visible par les joueurs ». Tous les autres documents — notes de préparation, fiches de PNJ non révélés, intrigues, informations secrètes — restent invisibles depuis la vue joueur, sans que le MJ ait à prendre d'action complémentaire pour les masquer. La règle s'applique également aux notes de session à visibilité « privé MJ » : elles ne sont jamais accessibles depuis la vue joueur, même lorsqu'elles référencent un document partagé.

---

## Portée et hors-portée

**Ce que cette exigence couvre :**

- L'ensemble des documents dont la visibilité est « privé MJ » ou non définie : ils ne sont pas accessibles depuis la vue joueur, quelle que soit la façon dont le joueur tente d'y accéder.
- Les notes de session créées par le MJ avec une visibilité « privé MJ » : inaccessibles depuis la vue joueur, même si la session est active.
- L'accès ponctuel via lien de session (joueur sans compte, `GuestAccess`) : la séparation s'applique de façon identique à un joueur avec compte.
- La tentative d'accès direct à un document non partagé, sans passer par l'interface de la vue joueur : cette tentative doit échouer, sans révéler l'existence du document.

**Ce que cette exigence ne couvre pas :**

- Les notes personnelles des joueurs (visibilité « personnelle joueur ») : ces notes sont inaccessibles au MJ selon une règle distincte (UC-06, règles métier). Cette exigence ne concerne que le sens MJ → joueur.
- La granularité du partage par joueur individuel : le MVP livre un partage par document pour l'ensemble des membres et des accès invités. La granularité par joueur ou personnage est un horizon produit (vision §5bis). Cette exigence couvre le niveau de granularité livré.
- La gestion des membres de campagne et la révocation des accès : couvertes par UC-11.

---

## Critères d'acceptation produit

**Situation nominale — vue joueur pendant une session active :**

Un joueur accédant à sa vue pendant une session en cours voit uniquement les documents que le MJ a basculés en « visible par les joueurs ». Les documents de préparation du MJ — PNJ cachés, intrigues, notes privées de session — n'apparaissent pas dans sa vue, sans que le MJ ait eu à prendre d'action supplémentaire pour les masquer.

**Situation nominale — joueur sans compte via lien ponctuel :**

Un joueur accédant via un lien de session ponctuel (sans compte, en tant qu'invité) voit exactement les mêmes documents qu'un joueur avec compte dans le périmètre de la session. Aucun document à visibilité « privé MJ » n'est accessible, quelle que soit la méthode d'accès utilisée.

**Situation limite — document épinglé non partagé :**

Un document épinglé dans les « documents épinglés de la session » mais dont la visibilité n'est pas « visible par les joueurs » n'apparaît pas dans la vue joueur. L'épinglage ne constitue pas un partage implicite.

**Situation limite — tentative d'accès à un document non partagé :**

Un joueur qui tenterait d'accéder directement à un document non partagé par le MJ — y compris en tentant d'accéder directement à des adresses de documents non partagés — se voit opposer un refus. Cette tentative ne révèle pas l'existence du document.

---

## Traçabilité montante

| Artefact | Nature du lien |
|---|---|
| [UC-06 — Vue session](../usecases/UC-06-vue-session.md) | Règles métier : « les joueurs disposent d'une vue distincte : ils ne voient que les informations partagées et leurs propres notes personnelles » ; « un document sans visibilité `visible par les joueurs` n'est pas visible dans la vue joueur, même s'il est épinglé dans la session ». |
| [UC-09 — Accès session joueur](../usecases/UC-09-acces-session-joueur.md) | Le joueur invité via lien ponctuel (GuestAccess) dispose des mêmes droits fonctionnels qu'un joueur avec compte dans le périmètre de son accès — et des mêmes restrictions de visibilité. |
| [US-UC-09 — Accéder à une session en tant que joueur](../user-stories/US-UC-09-acces-session-joueur.md) | RB-09-04 : le joueur invité voit les documents `PUBLIC` et les `documents épinglés` de la session (uniquement ceux `PUBLIC`) ; les documents `PLAYER_PRIVATE` des autres joueurs et ceux du MJ ne lui sont jamais visibles. |
| [Vision produit §5 — Partage fluide](../vision/vision-produit.md) | Différenciant nommé : « le MJ contrôle ce qui est visible aux joueurs, document par document, et garde ses notes de préparation privées ». |
