# SPEC — backlog-domaine

## Contrat

### Pourquoi

Les réponses de l'opérateur et les alignements de l'onboarding sont reportés dans les sources, à leur emplacement définitif (ADR-021). C'est le seul commit de la couche qui change la substance (ADR-022, condition 3).

### État actuel

Les 15 entrées de la couche sont listées dans l'index `docs/project/backlog/README.md:1`. Chaque entrée cite ses lignes d'origine aux anciens chemins (antérieurs à F1).

### Ce qui doit être vrai à la fin

- Chaque entrée appliquée a modifié exactement ce qu'elle décrit, dans les documents qui font foi selon l'ordre d'autorité.
- Chaque entrée appliquée passe au statut « fait », avec le hash du commit.
- Une entrée qui ne peut pas être appliquée telle quelle reste « ouvert » avec la raison, et devient une question pour l'opérateur.
- Les documents aval qui citaient l'ancien texte sont alignés (`write.sh consumers`).
- `reference.md` porte le nouveau commit de départ.

### Contraintes et pièges

- Un seul commit pour la tâche (ADR-022).
- Ne jamais renuméroter ni réutiliser un identifiant (ADR-019, § Identifiants).
- Ne rien supprimer dont le contenu ne se retrouve pas ailleurs : `conform-verify` le détecte.
- Ouvrir la cible au moment d'écrire un renvoi (guide de conventions, § 8.4).
- Appliquer la règle de l'ADR-019 : la correction se fait d'abord à la source (ordre d'autorité), puis dans les consommateurs.
- Après chaque réécriture, chercher la chaîne citée dans tout le corpus (guide de conventions, § 8.1).
- Une entrée qui touche une autre couche est appliquée en entier ici, et le rapport le dit.

### Hors contrat

- Les gabarits, déjà faits.
- Les entrées des autres couches.
- Toute décision nouvelle : une contradiction nouvelle est remontée à l'opérateur, pas tranchée.

### À lire avant de commencer

- les 15 entrées de la couche
- ADR-019 (ordre d'autorité, registre)
- `docs/workstreams/conformite-corpus/reference.md`

## Périmètre

- `docs/domaine/**`
- `docs/project/backlog/**`
- `docs/workstreams/conformite-corpus/reference.md`
- hors couche, pour les seules sources et consommateurs des entrées (contrat : « appliquée en entier ici » ; élargissement décidé par l'opérateur le 2026-10-07) : `docs/architecture/**`, `docs/besoin/**`, `docs/decisions/ADR-014-modele-autorisation-api.md`, `docs/assurance/strategie-de-test.md`, `docs/interface/zonage.md`, `docs/glossaire.md`, `docs/reference/cahier-des-charges.md`

## Revue

Le contrat et les entrées : chaque modification correspond-elle à une entrée, et une seule ?

- general-purpose

## Vérités

1. Quand on ouvre chaque entrée de la couche, son statut est « fait » avec un hash, ou « ouvert » avec une raison.
2. Quand on cherche le texte qu'une entrée « fait » demandait de retirer, on ne le trouve plus dans la couche ni dans ses consommateurs.
3. Quand on lance les quatre contrôles, ils donnent les compteurs de la référence, à l'exception des écarts que les entrées appliquées expliquent et que le rapport liste.
4. Quand on lit `reference.md`, le commit de départ est celui de cette tâche.

## Vérification

- `dotnet test tools/Tests/CaracterisationOutils.csproj` : tous les tests passent.
- `dotnet run --project tools/VerifCorpus` : 0 lien mort, 0 ancre morte, 0 orphelin.
- `dotnet run --project tools/VerifMailleAgregat` : code 0.
- `dotnet run --project tools/VerifCouvertureRecette` : la même liste de TROU et de SOURCE RETIRÉE que la référence de départ.
- pour chaque entrée « fait » : `git grep` de la chaîne retirée, sans résultat

## Notes de dev

- Les 15 entrées sont « fait », dans leur fichier et dans l'index. Deux lots parallèles (Space Management, Identity & Access, Core et README ; Content Library et Session Conduct), fusionnés à trois voies sans conflit. 0060 et 0061 ont été réparties par item entre les deux lots, et l'orchestrateur a posé leur statut.
- Périmètre élargi par l'opérateur (2026-10-07) aux sources et consommateurs hors couche : le contrat disait « appliquée en entier ici », le périmètre ne le permettait pas.
- Décisions de l'opérateur (2026-10-07) :
  - 0060 item 6 : le canal est un contrat applicatif. Conséquence : Space Management ne consomme pas `SessionClosed`, dont le destinataire devient l'application ;
  - l'exception de RB-12-01 (0024) est limitée aux fiches `GM_ONLY` ou `PUBLIC`, et `PLAYER_PRIVATE` reste auteur seul, sans exception. Amendée à la source (UC-12, US-UC-12, UC-11), puis dans le cahier des charges, ADR-014, le glossaire et le domaine ;
  - 0042 : `Folder` porte un rôle système `NOTES`.
- Décisions de l'orchestrateur, prises par délégation de l'opérateur (« si il y a un choix à faire, je te laisse le prendre, mais il doit être justifié ») :
  - `CanBeReadBy` connaît le personnage associé par le lecteur, qui porte les identifiants des personnages associés, résolus par la couche application auprès de Space Management. **Justification** : l'association appartient à Space Management (`Space.AssociateCharacter()`, table `membership_characters`), et Content Library ne le connaît pas. C'est le même patron que le contrat applicatif retenu par l'opérateur pour l'invariant 16. Cela complète la signature de 0044, sans la changer ;
  - 0027 est propagée à la règle 2, au tableau des rôles et à `AddMember`. **Justification** : c'est la conséquence de 0026 et de 0027, et de la décision « au MVP, seul l'OWNER gère les membres » ;
  - le MLD reste l'exception à la règle de pureté (0048), pour son schéma. **Justification** : l'arbitrage T-08, inscrit dans l'en-tête de chaque `logical-data.md`, fait des types SQL la fonction du MLD ;
  - 0087 est appliquée d'abord au besoin (UC-05 A4, US-UC-05, UJ-UC-05). **Justification** : c'est une réponse explicite de l'opérateur, et l'amont doit la porter avant l'aval.
- Hash : `reference.md` inscrit la ligne `backlog-domaine`, et la tâche `gabarits-interface` y portera le hash, selon la règle du commit qui ne peut pas porter son propre hash.
- `conform-verify` ne s'applique pas comme critère à une tâche backlog : chaque ligne réécrite y compte comme perdue, et c'est pourquoi le départ est réépinglé après la tâche (même traitement qu'à la tâche 6).

## Notes de revue

Revue « chaque modification correspond-elle à une entrée, et une seule ? », 2026-10-07 : dix constats, plus trois constats préexistants.

- `fix` : US-UC-05 posait « il n'est pas réordonnable », une règle sans source ; elle est retirée.
- `fix` : consommateurs de l'exception RB-12-01 non alignés : le glossaire (`GM_ONLY`) et la règle 3 de Session Conduct.
- `fix` : zonage.md (« aucune sémantique fixe » sur les dossiers par défaut) contredisait le rôle `NOTES` ; l'exception y est réservée.
- `fix` : la note de 0042 citait un SOUS-SPÉCIFIÉ qui n'existait pas à HEAD.
- `fix` : le passage de `SessionClosed` à l'application est rattaché à 0060 item 6, comme sa conséquence.
- `intent`, tranché par délégation : le personnage associé dans `CanBeReadBy` (voir les notes de dev).
- `fix` (optionnel pris) : les lignes `SpaceDeleted` de Content Library et de Session Conduct affirmaient un invariant de lecture hors de l'entrée et nommaient des tables. Elles sont réécrites au grain du domaine, avec un renvoi à ADR-011.
- Optionnels gardés : l'exception MLD (T-08), la règle de filtrage du flux 8 (ADR-014), le hash différé.
- `deferred` (préexistants, à reprendre au solde du backlog) :
  - space-management/context.md, consommateur de `CharacterAssociated` : « Session Conduct (accès aux notes PLAYER_PRIVATE du personnage) » contredit l'invariant 6 de Content Library ;
  - UC-05 : la « vue "Non classés" dédiée » est close par zonage.md ;
  - « non renommable » (US-UC-05) contredit « peut être renommé » (zonage.md, navigation-dossiers.md) pour « Non classés ».

Répartition : 6 `fix`, 1 `intent` tranché, 3 optionnels gardés, 3 `deferred`.


