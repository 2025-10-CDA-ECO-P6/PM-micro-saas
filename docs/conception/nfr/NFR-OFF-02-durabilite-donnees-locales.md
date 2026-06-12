# NFR-OFF-02 — Durabilité des données locales entre les sessions

Famille **Hors connexion** — [Index de conception](../README.md)

---

## Énoncé normatif

Les données qu'un MJ a saisies dans le mode local sont retrouvables intactes la prochaine fois qu'il ouvre l'application sur le même appareil, même après avoir fermé le navigateur ou redémarré l'appareil.

---

## Raison d'être

Le mode local repose sur une promesse implicite : ce que le MJ saisit lui appartient et sera là la prochaine fois qu'il reviendra. Sans cette durabilité, la valeur de la préparation disparaît — préparer une campagne dans un outil qui oublie est moins utile qu'un carnet de papier.

**[Nadia](../persona/persona-04-nadia.md)** incarne le scénario critique : elle joue une fois par mois quand tout va bien, prépare en 30 à 45 minutes le dimanche soir. Ses données locales doivent survivre à ces intervalles longs. Sa question directe — *« que voit-on quand on revient après 6 semaines d'absence ? »* — est précisément ce que cette exigence adresse. Une perte silencieuse de données après une longue absence confirmerait sa crainte initiale : les outils numériques ne survivent pas à son rythme de vie.

**[Thomas](../persona/persona-01-thomas.md)** pose une exigence de confiance différente : la portabilité des données n'est pas un confort, c'est une question de confiance. Un outil qui perd des données silencieusement ne mérite pas cette confiance, quelle que soit sa richesse fonctionnelle.

La vision §5 nomme la « possession des données » comme différenciant de premier ordre. Posséder ses données implique qu'elles restent disponibles sans action de maintenance de la part du MJ. La durabilité n'est pas une feature optionnelle : elle conditionne la réalité de la promesse.

Le scénario alternatif A2 d'UC-01 décrit précisément ce flux : le MJ ferme puis revient, retrouve ses campagnes et documents intacts. L'exigence NFR-OFF-02 est la garantie non fonctionnelle qui rend ce scénario possible.

---

## Portée et hors-portée

**Ce que cette exigence couvre :**

- La persistance de l'ensemble du contenu local du MJ créé en mode local — campagnes, espace personnel, documents, dossiers et configurations — entre deux ouvertures successives de l'application, y compris après fermeture du navigateur ou redémarrage de l'appareil.
- La durabilité sur le même appareil, dans le même navigateur.
- L'information proactive du MJ lorsque cette durabilité ne peut pas être garantie par l'appareil — via un bandeau non bloquant distinct (couvert par NFR-OFF-03 pour le volet alerte).

**Ce que cette exigence ne couvre pas :**

- La persistance entre plusieurs appareils différents : la synchronisation multi-appareils nécessite un compte et une connexion. La synchronisation hors connexion entre plusieurs appareils est explicitement exclue du MVP.
- La durabilité dans le cloud : hors périmètre du mode local, par définition.
- La récupération des données après un effacement délibéré des données de navigation par le MJ lui-même ou par l'appareil sous contrainte de mémoire : ces cas sont couverts par NFR-OFF-03 (alerte) et UC-01 (scénario alternatif A3).
- La durabilité des sessions en cours lors d'une interruption réseau en mode cloud : couverte par NFR-OFF-04.

**Périmètre MVP assumé :** la synchronisation entre plusieurs appareils sans connexion est explicitement exclue du périmètre initial. La durabilité que garantit cette exigence est locale à un appareil et un navigateur.

---

## Critères d'acceptation produit

**Situation nominale — fermeture et réouverture du navigateur :**

Un MJ qui a créé une campagne, ajouté des documents et structuré son contenu, puis fermé le navigateur, retrouve l'intégralité de son contenu intact à la prochaine ouverture de l'application dans le même navigateur sur le même appareil. Aucun contenu n'est manquant, aucun document n'est partiellement chargé.

**Situation nominale — retour après plusieurs semaines :**

Un MJ qui n'a pas utilisé l'application depuis plusieurs semaines retrouve ses campagnes dans l'état exact où il les avait laissées. Le contenu, l'organisation des dossiers et la configuration de la vue session sont préservés.

**Situation nominale — redémarrage de l'appareil :**

Après un redémarrage complet de l'appareil, le MJ retrouve ses données locales dans le même état qu'avant le redémarrage.

**Situation limite — demande de conservation garantie refusée par l'appareil :**

Lorsque l'application demande à l'appareil la garantie de conservation permanente des données et que cette demande est refusée, les données sont conservées au mieux et un bandeau non bloquant informe le MJ que cette garantie n'a pas pu être obtenue. La conservation reste effective jusqu'à une éventuelle contrainte de mémoire — le MJ est informé du risque, pas de la perte effective.

---

## Traçabilité montante

| Artefact | Nature du lien |
|---|---|
| [UC-01 — Mode local sans compte](../usecases/UC-01-mode-local-sans-compte.md) | Scénario alternatif A2 (retour après fermeture du navigateur) : l'application récupère les données locales, le MJ retrouve ses campagnes et documents intacts. Exception E1 (stockage plein) : cas limite couvert par NFR-OFF-03. |
| [US-UC-01 — Mode local sans compte](../user-stories/US-UC-01-mode-local-sans-compte.md) | US-01-02 (retrouver ses données après fermeture du navigateur), RB-01-04 (les données sont durables — elles survivent à la fermeture et à la réouverture), RB-01-14 (bandeau de durabilité conditionnel). |
