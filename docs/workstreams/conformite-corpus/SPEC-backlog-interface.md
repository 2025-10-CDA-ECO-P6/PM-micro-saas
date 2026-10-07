# SPEC — backlog-interface

## Contrat

### Pourquoi

Les réponses de l'opérateur et les alignements de l'onboarding sont reportés dans les sources, à leur emplacement définitif (ADR-021). C'est le seul commit de la couche qui change la substance (ADR-022, condition 3).

### État actuel

Les 13 entrées de la couche sont listées dans l'index `docs/project/backlog/README.md:1`. Chaque entrée cite ses lignes d'origine aux anciens chemins (antérieurs à F1).

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

- les 13 entrées de la couche
- ADR-019 (ordre d'autorité, registre)
- `docs/workstreams/conformite-corpus/reference.md`

## Périmètre

- `docs/interface/**`
- `docs/project/backlog/**`
- `docs/workstreams/conformite-corpus/reference.md`
- hors couche, pour la seule source que nomme l'entrée 0062 (attributs de l'agrégat Space et du MLD `spaces`) : `docs/domaine/space-management/**` (contrat : « appliquée en entier ici »)
- hors couche, pour les seules sources et consommateurs des entrées 0045, 0047 et 0062 (élargissement par l'orchestrateur, sur délégation de l'opérateur du 2026-10-07) : `docs/besoin/cas-d-utilisation/UC-09-*`, `docs/besoin/parcours-utilisateur/bout-en-bout/parcours-03-*`, `docs/glossaire.md`, `docs/reference/conception-detaillee.md`

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

- Les 13 entrées sont « fait », dans leur fichier et dans l'index. Trois lots parallèles : zonage, conventions et domaine ; raisonnement UX et gabarit d'écran ; fiches et README des écrans. Fusion à trois voies sans conflit.
- Périmètre élargi aux seules sources et consommateurs que nomment les entrées : `docs/domaine/space-management/**` (0062), UC-09, parcours-03, le glossaire et la conception détaillée (0045, 0047, 0062).
- Décisions de l'orchestrateur, prises par délégation de l'opérateur (« si il y a un choix à faire, je te laisse le prendre, mais il doit être justifié ») :
  - **UC-09 scénario A § 4 est amendé selon 0045**, puis parcours-03, le zonage et la fiche d'accès par lien. **Justification** : 0045 est une réponse explicite de l'opérateur ; l'amont porte la décision, et l'aval suit ;
  - **`Space.Create(ownerId, name, type, description?, gameSystem?)`**. **Justification** : UC-02 renseigne ces champs dès la création. Les colonnes restent en `text` NULL, car aucune source ne fixe de longueur ;
  - **0065 item 8, option (b)** : « règle de zoning », « action » et « lien » sont ajoutés à la Famille 1 et au gabarit. **Justification** : retyper dans la liste existante perdait de l'information, et l'option (b) est celle de l'entrée elle-même ;
  - **alignements de 0002**, appliqués seulement quand une fiche suivait déjà une source : graphe S3 du zonage (UC-10), connexion fédérée (UC-10), interface de l'espace personnel (zonage § 9) ;
  - **marqueurs clos à la forme de l'ADR-019**, « DÉCIDÉ <date> (<renvoi>) », chaque date étant vérifiée par `git log -S` : 2026-06-25 (`d18ba7f`), 2026-07-09 (`ba8683f`), 2026-07-15 (`6ce3e66`). Les libellés de l'outillage (`marker.resolved`, « [RÉSOLU — source] ») contredisent l'ADR-019 : c'est un défaut de l'outillage, à reporter.
- Hash : `reference.md` porte la ligne `backlog-interface`, que la tâche suivante complétera.

## Notes de revue

Revue « chaque modification correspond-elle à une entrée, et une seule ? », 2026-10-07 : douze constats.

- `fix` : la date « DÉCIDÉ 2026-06-10 » des fournisseurs d'identité est corrigée en 2026-07-15 (`6ce3e66`).
- `fix` : la fiche d'accès par lien et les conventions citent UC-09 § 4, amendé, comme source première, et la phrase « à amender » est retirée.
- `fix` : la définition de « zone d'information du lien » contredisait la fiche sur l'information RGPD, qui a sa propre zone.
- `fix` : la ligne « Vue campagne » de S4 copiait l'ancien en-tête de la fiche.
- `fix` : RB-02-22 était cité pour la lecture seule ; la citation devient RB-02-21 et la règle métier 7.
- `fix` : les notes périmées de 0002, 0045, 0047 et 0089 sont mises à jour.
- `fix` (optionnel pris) : les affordances de `parametres-campagne` reçoivent un libellé propre.
- Optionnels gardés : la spec porte l'élargissement de périmètre ; 0065 cite `vue-session-mj.md` ; l'étiquette du graphe S3.
- `deferred` :
  - les maquettes `acces-lien-saisie-nom.dc.html` et `parametres-campagne.dc.html` divergent de leurs fiches. La recette fait prévaloir la fiche ; l'alignement d'une maquette est une refonte ;
  - AR-20 dit « Non classés » renommable, alors qu'UC-02, qui fait plus autorité, le dit non renommable. L'écart est préexistant et doit être remonté à l'opérateur.
- Autres restes, différés : UC-06 (§ 4 du scénario nominal et § A, épinglage sans distinguer la note de session) et parcours-02 ; les six contradictions entre fiches listées dans 0002 ; ce que voit un joueur d'un espace ARCHIVED (0018) ; « Vue de l'espace personnel » dans `cahier-des-charges.md` ; les anciens noms de fichiers en prose.

Répartition : 7 `fix`, 3 optionnels gardés, 2 `deferred`.
