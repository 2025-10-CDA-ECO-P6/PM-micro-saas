# NFR-CONF-02 — Isolation des données en mode local

Famille **Confidentialité** — [Index de conception](../README.md)

---

## Énoncé normatif

En mode local sans compte, aucune donnée saisie par le MJ n'est transmise hors de son appareil sans qu'il ait effectué une action explicite (création de compte, export manuel). L'application ne contacte aucun service extérieur pour les données de campagne tant que le MJ reste en mode local.

---

## Raison d'être

Haversack repose sur une promesse centrale, posée dès la vision comme premier différenciant (vision §5) : « données toujours possédées par l'utilisateur ». Cette possession n'est pas une métaphore — elle est matérielle. Elle signifie que le MJ qui choisit de ne pas créer de compte garde la maîtrise complète de ses données : elles ne quittent pas son appareil sans qu'il en décide.

Cette exigence est la contrepartie technique de la promesse de mode local. Si des données de campagne étaient transmises à un service extérieur à l'insu de l'utilisateur, même pour des raisons légitimes (télémétrie, synchronisation implicite), la promesse de possession serait vidée de son contenu.

**[Thomas](../persona/persona-01-thomas.md)** formule cette attente avec précision : *« Peut-on exporter ses données si on arrête l'outil ? »* et *« La portabilité des données n'est pas un confort, c'est une question de confiance. »* Thomas est développeur backend — il sait ce que signifie « données stockées localement » et ce que cela implique. Une application qui contacte un service extérieur pour des données de campagne, même de façon anodine, romprait immédiatement cette confiance.

Le modèle de monétisation non-agressif du produit (vision §2.1, §3) repose sur la conversion naturelle : la valeur est perçue avant tout engagement, l'upgrade vers le compte cloud se produit quand le besoin de partage ou de sauvegarde apparaît. Ce modèle suppose que le mode local est complet et autonome — pas un aperçu limité d'un service cloud.

UC-01 formalise cette règle métier : *« En mode local, aucune donnée n'est envoyée au serveur. »* Cette règle est qualifiée de « garantie de confiance envers l'utilisateur, pas uniquement une contrainte technique » (US-UC-01, RB-01-01). NFR-CONF-02 en est l'expression côté exigence de besoin.

---

## Portée et hors-portée

**Ce que cette exigence couvre :**

- L'ensemble des données de campagne saisies en mode local : documents, notes, dossiers, scénarios, contenu des sessions.
- L'absence de transmission de ces données vers tout service extérieur, tant que le MJ reste en mode local et n'a pas déclenché d'action explicite (création de compte ou export manuel).
- Le comportement observable lors d'une session hors connexion : l'application fonctionne de façon identique avec ou sans connexion réseau disponible, pour toutes les fonctionnalités de préparation et de session.

**Ce que cette exigence ne couvre pas :**

- Les données transmises après une action explicite du MJ : la création d'un compte déclenche une migration des données locales vers le cloud, soumise à un gate de confirmation (UC-01 A1, US-UC-01 RB-01-09, RB-10-04). Ces transmissions sont consenties.
- L'export manuel depuis les paramètres : l'utilisateur agit délibérément pour produire un fichier de sauvegarde. Cette action sort du périmètre de l'exigence d'isolation.
- Les mesures d'usage anonymes éventuelles : si de telles mesures sont mises en place, elles font l'objet d'un traitement distinct, sans lien avec les données de campagne. Ce périmètre est hors du champ de cette exigence, qui porte uniquement sur les données de campagne.
- La confidentialité des données sur un poste partagé : cette limite est couverte par NFR-CONF-04.

---

## Critères d'acceptation produit

**Situation nominale — utilisation en mode local avec connexion disponible :**

Un MJ qui utilise l'application en mode local, avec une connexion réseau disponible, observe que ses actions de préparation (création de campagne, ajout de documents, prise de notes) ne génèrent aucune communication vers un service extérieur pour les données de campagne. L'application se comporte de façon identique, avec ou sans connexion disponible, pour toutes les fonctionnalités de préparation et de session.

**Situation nominale — utilisation hors connexion :**

Un MJ qui coupe sa connexion réseau alors qu'il utilise l'application en mode local ne voit aucun avertissement réseau et ne constate aucune dégradation du comportement de l'application pour les fonctionnalités de préparation et de session. Cela confirme qu'aucune dépendance réseau n'existe pour ces fonctionnalités.

**Situation limite — tentative d'action cloud depuis le mode local :**

Un MJ qui tente d'utiliser une fonctionnalité nécessitant un compte (partage avec les joueurs, accès multi-appareil) se voit proposer la création d'un compte via une invite claire et non bloquante. Aucune donnée de campagne n'est transmise avant qu'il ait créé un compte et confirmé la migration.

**Situation limite — création de compte depuis le mode local :**

Quand le MJ choisit de créer un compte, l'application lui présente les données détectées (campagnes, volume, date) et lui demande une confirmation explicite avant de démarrer la migration. La migration ne commence qu'après cette confirmation. Aucune donnée n'est transmise sans ce consentement.

---

## Traçabilité montante

| Artefact | Nature du lien |
|---|---|
| [UC-01 — Mode local sans compte](../usecases/UC-01-mode-local-sans-compte.md) | Règle métier fondatrice : « En mode local, aucune donnée n'est envoyée au serveur. » Scénario alternatif A1 : migration uniquement après confirmation explicite. |
| [US-UC-01 — Mode local sans compte](../user-stories/US-UC-01-mode-local-sans-compte.md) | RB-01-01 (aucune donnée envoyée au serveur — garantie de confiance) ; RB-01-09 (gate de confirmation avant migration) ; RB-01-15 (aucun élément protégé conservé dans le navigateur en mode local). |
| [US-UC-10 — Compte cloud](../user-stories/US-UC-10-compte-cloud.md) | RB-10-04 (gate de reconnaissance avant migration : confirmation explicite requise, exigence du système, aucune migration sans elle). |
| [Vision produit §5 — Différenciants](../vision/vision-produit.md) | « Friction d'entrée nulle — aucun compte pour commencer, données locales immédiates et toujours possédées par l'utilisateur. » |
| [Persona Thomas](../persona/persona-01-thomas.md) | Question explicite sur la portabilité et la confiance ; la possession des données est une exigence de confiance, pas un confort. |
