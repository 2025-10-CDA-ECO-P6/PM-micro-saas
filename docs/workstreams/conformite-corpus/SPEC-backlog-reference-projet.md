# SPEC — backlog-reference-projet

## Contrat

### Pourquoi

Les réponses de l'opérateur et les alignements de l'onboarding sont reportés dans les sources, à leur emplacement définitif (ADR-021). C'est le seul commit de la couche qui change la substance (ADR-022, condition 3).

### État actuel

Les 18 entrées de la couche sont listées dans l'index `docs/project/backlog/README.md:1`. Chaque entrée cite ses lignes d'origine aux anciens chemins (antérieurs à F1).

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

- les 18 entrées de la couche
- ADR-019 (ordre d'autorité, registre)
- `docs/workstreams/conformite-corpus/reference.md`

## Lots

- Lot 1 — fichiers: docs/reference/**, docs/projet/** — vérités: 2, 3
- Lot 2 — fichiers: docs/project/backlog/**, docs/README.md, README.md, docs/workstreams/conformite-corpus/reference.md — vérités: 1, 4

## Périmètre

- `docs/reference/**`
- `docs/projet/**`
- `docs/README.md`
- `README.md`
- `docs/project/backlog/**`
- `docs/workstreams/conformite-corpus/reference.md`
- hors couche, pour la seule source que nomme l'entrée 0075 : `docs/decisions/ADR-018-espace-personnel-generalisation-space.md` (contrat : « appliquée en entier ici »)
- hors couche, pour les seuls consommateurs de 0070 et 0073 (décision de l'orchestrateur, par délégation de l'opérateur) : `docs/besoin/stories/US-UC-01-mode-local-sans-compte.md`, `docs/architecture/specs/document-properties-schemas.md`, `docs/assurance/strategie-de-test.md` (consommateur de 0090), `docs/architecture/specs/telemetrie.md` (source du décompte « cinq des six »)

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

- Les 18 entrées sont « fait », dans leur fichier et dans l'index. Deux lots parallèles : la couche référence (0005, 0006, 0012, 0070, 0073, 0074, 0075, 0079) et la couche projet (0004, 0051, 0071, 0072, 0076, 0077, 0078, 0090, 0091, 0092). Fusion à trois voies sans conflit. 0079 était déjà appliquée par la tâche 12 : elle est marquée « fait » avec sa preuve.
- Vérifications : 0005, l'annexe « Points à arbitrer » était périmée (`6ce3e66`) ; 0006, le décompte est de 17 FK inter-modules (`f4214d1`) ; 0012, « 06 » est l'ancien `06-structure-projets.md` (`dbb181e`) ; 0004, un seul écart (UC-02 §A1).
- Périmètre élargi aux seules sources et consommateurs des entrées : ADR-018 (0075), US-UC-01 (0070), `document-properties-schemas.md` (0073), `strategie-de-test.md` (0090), `telemetrie.md` (source du décompte).
- Décisions de l'orchestrateur, prises par délégation de l'opérateur :
  - **ADR-018 reçoit un complément post-revue** (geste capture-first, 2026-06-12). **Justification** : la matrice de priorités, plus haute dans l'ordre d'autorité, porte la décision ; patron d'ADR-014, 016 et 017 ;
  - **P-08 est recoté Modérée**. **Justification** : le registre a un précédent, T-03, « principe acté, paramètres non fixés », et c'est l'état de la télémétrie ;
  - **la décision de marché est croisée avec H1 et H2**, et non plus avec H1 à H4. **Justification** : H3 et H4 portent sur le partage (J3) et H5 sur la conversion cloud (J2) ; selon la réponse de l'opérateur en 0090, la décision ne mesure que ce que J1 livre ;
  - **T-14 et T-15 du registre sont clos**. **Justification** : leur objet est tranché le 2026-09-03 (schéma vide au MVP, écriture refusée sans type), et T-06 est le précédent du registre ;
  - **le marqueur `[À TRANCHER — J0]` du § 7 de la méthode de ticket est gardé**. **Justification** : il porte sur le dépôt du produit au J0, que l'outillage du corpus ne tranche pas.
- Différés : « campagne par campagne » (parcours-01, parcours-02, RB-01-11), terme rejeté qui revient à la tâche 15 ; le `TRANCHÉ` préexistant de US-UC-01 et le « (résolu) » de D-01, à convertir avec les autres marqueurs.
- Hash : `reference.md` porte la ligne de cette tâche, que la tâche 15 complétera.

## Notes de revue

Revue « chaque modification correspond-elle à une entrée, et une seule ? », 2026-10-07 : neuf constats.

- `fix` : « six points tranchés » contredisait le § 6 de la spec de télémétrie (cinq sur six). La source (bandeau de `telemetrie.md`) est corrigée d'abord, puis le guide de lecture, le registre, les décisions en attente, le cahier des spécifications et 0072.
- `intent`, tranché par délégation : T-14 et T-15 sont clos (voir les notes de dev).
- `fix` : la description de CreerIssues dit maintenant la zone engendrée et l'estampille de provenance, et celle de VerifMailleAgregat son contrôle des identifiants ambigus. La phrase qui ouvrait une question hors de l'index unique est réécrite en constat.
- `fix` : le compte de renvois de 0012 est corrigé (sept renvois, dix numéros de section).
- `fix` : le périmètre et le § 5 de `document-properties-schemas.md` sont alignés.
- `fix` : T-06, la date de résolution est distinguée de celle du retrait de la matrice.
- `fix` : « câblé » devient « intégré aux contrats » dans EP-04, comme le veut 0078.
- Gardés : 0051 dans la DoD (l'entrée le demande, et la règle nomme les UC, ADR et specs) ; le marqueur `[À TRANCHER — J0]`.

Répartition : 1 `intent` tranché, 6 `fix`, 2 gardés.
