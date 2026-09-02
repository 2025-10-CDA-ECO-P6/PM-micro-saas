# Use Cases hors MVP — Vision produit long terme

Les use cases suivants ne sont pas prévus dans le MVP. Ils sont exclus par choix stratégique, pas par oubli.

---

## Fonctionnalités déprioritisées (anciennement envisagées dans le MVP)

### Inventaire personnage

Le joueur gère ses ressources, équipements et monnaie depuis sa fiche personnage.

**Pourquoi Won't Have** : compète avec D&D Beyond sans pouvoir l'égaler. Émilie joue des systèmes sans inventaire. Antoine a trois systèmes aux logiques incompatibles. Une fiche générique serait trop pauvre pour être utile.

**Condition de retour** : si les entretiens révèlent que les joueurs gèrent activement leurs ressources dans l'outil et que D&D Beyond n'est pas utilisé dans le groupe.

---

### Clôture formelle de session

Le MJ clôture une session, écrit un résumé structuré et prépare les éléments pour la suite.

**Pourquoi Won't Have** : absorbé par UC-04 (notes post-session) et UC-07 (capture à la volée). La clôture formelle est un workflow, pas un use case atomique. Sonia invalide le concept (pas de continuité entre one-shots).

**Condition de retour** : si l'historique de campagne devient un différenciant après les premières validations.

---

### Documents de lore (use case distinct)

Le MJ crée et organise des fiches de lieux, factions, organisations et objets importants comme catégorie dédiée.

**Pourquoi Won't Have** : un document de lore est un document dans un dossier "Lore". Le pilier 1 (document générique + dossiers libres + types optionnels) couvre entièrement ce besoin. Créer un use case distinct n'apporterait aucune valeur différenciante.

**Condition de retour** : jamais en tant que use case distinct.

---

### UC-F07 — Créer un scénario directement dans la bibliothèque (sans campagne)

> **Note ADR-018** : ce use case, qualifié de palliatif dans le corpus initial, est largement subsumé par la décision `Space + SpaceType.PERSONAL`. Créer du contenu dans l'espace personnel est désormais le cas nominal — il ne requiert ni campagne intermédiaire ni campagne-atelier. La reformulation ci-dessous en prend acte sans supprimer l'entrée, qui reste utile pour tracer l'historique de la décision. Voir ADR-018 §ScenarioLibrary/UC-F07.

Le MJ crée un nouveau scénario template directement depuis son espace personnel (`SpaceType.PERSONAL`), sans passer par une campagne existante.

**Persona concerné** : Sonia, qui veut préparer un nouveau one-shot pour son catalogue sans avoir à créer une campagne intermédiaire.

**Pourquoi largement subsumé (Could Have résiduel)** : sous ADR-018, l'espace personnel est créé par défaut à l'ouverture du compte et constitue la zone d'atterrissage naturelle pour tout contenu créé sans espace explicite. Sonia crée son scénario dans son espace personnel — c'est le flux de base, pas un palliatif. Le Could Have résiduel concerne uniquement l'interface de bibliothèque dédiée (navigation, recherche, instanciation en un geste), restée post-MVP conformément aux recommandations MoSCoW d'ADR-018.

**Condition de retour** : l'interface de bibliothèque de réutilisation inter-espaces (catalogue, instanciation depuis l'espace personnel) — déclenchée par la condition de retour UC-13 (adoption du flux one-shot).

---

### Gel de campagnes au downgrade de tier

**Statut : spécifié dans [UC-15](UC-15-gel-espaces-downgrade-tier.md).**

Lorsqu'un MJ perd son tier premium (fin d'abonnement, non-renouvellement) et se retrouve alors au-delà de la limite de campagnes du tier gratuit, que se passe-t-il ?

**Questions initialement ouvertes** (désormais tranchées dans UC-15) : quelles campagnes deviennent inaccessibles, selon quel mécanisme, et la situation est-elle réversible ?

**Réponses ratifiées dans UC-15** : les espaces excédentaires sont gelés automatiquement (les plus récents en premier) lors de l'événement `AccountTierChanged` ; la réversibilité est garantie par un dégel automatique au ré-upgrade, sans perte de données (accès en lecture seule pendant le gel, jamais de suppression).

Ce use case délimite le périmètre de la valeur `FROZEN` dans `SpaceStatus` et du comportement `Unfreeze()` mentionnés dans le domaine.

- Voir : `SpaceStatus` (`FROZEN`) dans le glossaire.
- Voir aussi : [UC-15](UC-15-gel-espaces-downgrade-tier.md) — décision complète et ratifiée.
- Concerne : `AccountTier`, la limite d'espaces `CAMPAIGN`/`ONE_SHOT` `FREE`, `Space Management`.

---

## Vision long terme

### UC-F01 — Utiliser des templates de système de jeu

Le MJ choisit un système de jeu ou un template. Le produit adapte certains champs : fiche personnage, fiche PNJ, ressources, structure de scénario.

**Intérêt** : améliore l'adoption, réduit la configuration manuelle.

**Risque** : forte complexité de contenu. Risque de transformer le projet en moteur de règles.

**Position recommandée** : document générique dans le MVP, templates configurables après validation de l'adoption.

---

### UC-F02 — Assistant IA pour le MJ

Le MJ utilise une aide IA pour générer des idées, structurer un scénario, créer un PNJ, résumer une session ou proposer des pistes d'improvisation.

**Intérêt** : forte valeur perçue. Cohérent avec le besoin d'aide à la préparation.

**Risque** : peut brouiller le positionnement MVP. Risque de construire un produit "IA" au lieu d'un outil d'organisation.

**Position recommandée** : extension de l'assistant MJ après validation du socle organisationnel.

---

### UC-F03 — Table visuelle légère

Le produit propose une table visuelle pour représenter des cartes, positions ou scènes.

**Intérêt** : utile pour les sessions en ligne.

**Risque** : concurrence directe avec Roll20 et Foundry. Complexité technique élevée. Détourne le produit de son cœur.

**Position recommandée** : à considérer uniquement après validation du socle MJ.

---

### UC-F04 — Application desktop avec synchronisation CRDT

Le MJ utilise une application desktop avec synchronisation offline-first complète (résolution de conflits multi-device, usage sans connexion prolongé).

**Intérêt** : répond au besoin d'usage hors ligne. Peut rassurer sur la possession des données.

**Risque** : architecture CRDT ou équivalent — complexité élevée. Nécessite une stratégie de synchronisation de conflits.

**Distinction avec UC-01** : le mode local (UC-01) utilise le stockage navigateur (IndexedDB) pour différer l'obligation de compte sur la même session de navigation. UC-F04 est une architecture offline-first complète, multi-device, avec résolution de conflits — un projet à part entière.

**Position recommandée** : ne pas développer dans le MVP. UC-01 couvre le besoin de "démarrer sans compte". UC-F04 est réservé à une version post-validation avec une base utilisateur active.

---

### UC-F05 — Templates communautaires

Les utilisateurs peuvent créer, partager ou installer des templates de campagne, fiches ou aides de jeu.

**Intérêt** : effet communautaire. Peut enrichir le produit sans tout développer en interne. Soutient une stratégie freemium.

**Risque** : modération, qualité variable, complexité de gestion d'un catalogue.

**Position recommandée** : vision long terme uniquement.

---

### UC-F06 — Relations et règles entre documents typés

#### Contexte

Les types de document (PNJ, Personnage joueur, Objet, Faction — voir [UC-05](UC-05-organiser-dossiers.md))
sont la fondation de cette évolution. Une fois que les documents ont des propriétés structurées,
deux couches peuvent s'y ajouter progressivement.

#### Couche 1 — Relations typées entre documents

Un document typé peut déclarer des relations vers d'autres documents typés :
- Ce PNJ appartient à cette Faction.
- Cet Objet est porté par ce Personnage joueur.
- Cette Scène se déroule dans ce Lieu.

Les relations sont bidirectionnelles et naviguables : depuis la fiche d'une faction, on voit ses membres. Depuis la fiche d'un personnage, on voit ses objets. Les backlinks que Thomas utilise dans Obsidian deviennent ici des relations structurées et interrogeables.

**Intérêt** : navigation contextuelle en vue session, requêtes croisées ("quels PNJ sont membres de cette faction ?"), cohérence narrative.

**Risque** : complexité de modélisation UX. Les relations doivent rester optionnelles pour ne pas décourager les MJ qui n'en ont pas besoin.

#### Couche 2 — Règles sur les propriétés et relations

Une règle est une logique appliquée aux propriétés de documents liés :
- Si un personnage porte un objet avec la propriété "dégâts: 1d6", une règle peut afficher le total de dégâts.
- Si un personnage a la propriété "armure: 3", une règle peut calculer la réduction de dégâts.

C'est une émulation légère de système de jeu — pas un moteur de règles complet, mais un calculateur contextuel sur des propriétés déclarées.

**Intérêt** : un MJ peut modéliser son système de jeu dans l'outil sans en être prisonnier. Le système reste agnostique — les règles sont définies par l'utilisateur, pas par l'application.

**Risque** : frontière floue avec un vrai moteur de règles. Risque de dérive vers la simulation. Doit rester un outil d'aide, pas de substitution au système de jeu.

**Prérequis techniques** : les types de document (UC-05) doivent être implémentés et stables. Le schéma de document doit prévoir type optionnel et propriétés structurées (structure de données structurée) dès le MVP pour ne pas nécessiter de migration majeure.

**Position recommandée** : Couche 1 (relations) post-MVP, après validation de l'adoption des types de document. Couche 2 (règles) vision long terme uniquement.
