# NFR-OFF-03 — Aucune perte silencieuse de données locales

Famille **Hors connexion** — [Index de conception](../../README.md)

---

## Énoncé normatif

Si les données locales d'un MJ risquent de ne plus être disponibles — que ce soit parce que l'espace de stockage de l'appareil est sous pression ou pour toute autre raison — l'application l'avertit explicitement, de manière non bloquante, et lui propose une action pour sécuriser ses données.

---

## Raison d'être

Le mode local repose sur une limitation inhérente : la conservation permanente des données ne peut pas toujours être garantie par l'appareil. Des situations existent — espace de stockage saturé, contrainte de mémoire, effacement des données de navigation — qui peuvent conduire à la disparition des données d'un MJ. Cette réalité est connue et assumée. Ce qui n'est pas acceptable, en revanche, c'est qu'elle survienne sans que le MJ en ait été informé.

**[Thomas](../persona/persona-01-thomas.md)** exprime cet enjeu de manière directe : « La portabilité des données n'est pas un confort, c'est une question de confiance. » Découvrir après coup que plusieurs heures de préparation ont disparu, sans avoir eu le moindre signal d'alerte, serait une trahison de cette confiance. Thomas sait gérer un risque quand il lui est présenté honnêtement ; il ne pardonnera pas à un outil qui lui a dissimulé ce risque.

**[Nadia](../persona/persona-04-nadia.md)** représente le profil le plus exposé à une perte silencieuse : elle revient après six semaines, dans un navigateur qui a pu gérer sa mémoire de manière agressive entre-temps. Elle n'a pas les ressources techniques pour anticiper ce risque de son propre chef. C'est l'application qui doit l'informer au bon moment.

Le scénario UC-01 A3 décrit le moment où la perte est déjà consommée — les données sont introuvables et le MJ revient. Cette exigence adresse le moment en amont : informer avant la perte, pas après. L'exception E1 d'UC-01 couvre le cas de saturation immédiate du stockage, où la prise en compte est synchrone avec l'action du MJ.

La promesse de « données toujours possédées par l'utilisateur » (vision §5) inclut la transparence sur les risques qui pèsent sur ces données. Posséder, c'est aussi être en capacité de réagir.

---

## Portée et hors-portée

**Ce que cette exigence couvre :**

- L'alerte non bloquante lorsque la conservation permanente des données ne peut pas être garantie par l'appareil (demande de garantie refusée).
- L'alerte non bloquante lorsque l'espace de stockage de l'appareil est saturé et que l'enregistrement d'une nouvelle donnée est refusé.
- La proposition d'une action concrète permettant au MJ de sécuriser ses données : création d'un compte pour migrer vers le cloud, ou export de ses données.
- La garantie qu'aucune disparition de données ne survient sans qu'un message ait été affiché préalablement.

**Ce que cette exigence ne couvre pas :**

- Le cas où le MJ efface délibérément les données de son navigateur : ce cas est hors du périmètre de l'alerte (l'action est intentionnelle de la part du MJ, à l'extérieur de l'application).
- La récupération des données après leur perte : ce cas est couvert par UC-01 (scénario alternatif A3) et US-01-06.
- La protection des données contre la lecture par un tiers sur un appareil partagé : ce risque distinct est adressé par la famille Confidentialité (NFR-CONF-04).
- La synchronisation vers un autre appareil en réponse à l'alerte : cette capacité nécessite un compte et une connexion.
- La valeur numérique du seuil de saturation qui déclenche le refus d'enregistrement : cette valeur est une contrainte d'interface, pas une règle métier durable ([README de la famille](README.md)) — elle est documentée à titre indicatif dans [UC-01](../usecases/UC-01-mode-local-sans-compte.md) et qualifiée comme telle par [US-UC-01](../user-stories/US-UC-01-mode-local-sans-compte.md) (RB-01-05) ; elle n'est pas reproduite ici.

**Distinction entre les deux situations d'alerte :** la saturation du stockage (E1) signale un refus d'enregistrement immédiat — les nouvelles données ne peuvent pas être sauvegardées. La demande de garantie refusée signale un risque futur — les données existantes sont conservées au mieux mais peuvent être supprimées sous pression. Ces deux situations sont distinctes et doivent être communiquées différemment au MJ.

---

## Critères d'acceptation produit

**Situation nominale — conservation garantie non obtenue :**

Lorsque l'application ne parvient pas à obtenir la garantie de conservation permanente de l'appareil, un message non bloquant apparaît dans l'interface. Ce message est visible sans que le MJ ait à naviguer pour le trouver. Il décrit le risque en termes compréhensibles (les données peuvent être supprimées par l'appareil) et propose une action concrète. Il ne bloque pas l'utilisation de l'application.

**Situation nominale — stockage de l'appareil saturé :**

Lorsque l'appareil refuse d'enregistrer de nouvelles données faute d'espace disponible, un message non bloquant s'affiche et informe le MJ que l'enregistrement a échoué. Une action concrète est proposée (créer un compte pour migrer les données). Le MJ comprend que ses données existantes sont intactes mais que rien de nouveau ne peut être sauvegardé.

**Situation limite — aucune disparition sans message préalable :**

Dans tous les cas où une perte de données locales est techniquement possible dans le périmètre de l'application, un message d'alerte a été affiché au MJ avant que cette perte ne survienne. Il n'existe pas de scénario dans lequel des données disparaissent et le MJ n'en a jamais été informé.

**Situation limite — message non bloquant :**

Le message d'alerte ne suspend pas les actions en cours ni ne gèle l'interface. Le MJ peut continuer à utiliser l'application pendant la durée de l'alerte. L'alerte est visible mais non intrusive.

---

## Traçabilité montante

| Artefact | Nature du lien |
|---|---|
| [UC-01 — Mode local sans compte](../usecases/UC-01-mode-local-sans-compte.md) | Scénario alternatif A3 (données introuvables après perte), Exception E1 (stockage plein — refus d'enregistrement immédiat). Les deux bandeaux distincts (durabilité et confidentialité) documentés dans les règles métier de UC-01. |
| [US-UC-01 — Mode local sans compte](../user-stories/US-UC-01-mode-local-sans-compte.md) | US-01-06 (être guidé quand les données locales sont introuvables), RB-01-05 (le seuil de stockage — contrainte d'interface, pas une règle métier durable — est le déclencheur concret de la situation couverte par cette exigence), RB-01-12 (distinction première visite / données perdues), RB-01-13 (migration vers le cloud proposée en cas de stockage plein), RB-01-14 (bandeau de durabilité conditionnel — distinct du bandeau de confidentialité). |
