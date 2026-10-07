# SPEC — backlog-decisions-architecture-assurance

## Contrat

### Pourquoi

Les réponses de l'opérateur et les alignements de l'onboarding sont reportés dans les sources, à leur emplacement définitif (ADR-021). C'est le seul commit de la couche qui change la substance (ADR-022, condition 3).

### État actuel

Les 10 entrées de la couche sont listées dans l'index `docs/project/backlog/README.md:1`. Chaque entrée cite ses lignes d'origine aux anciens chemins (antérieurs à F1).

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

- les 10 entrées de la couche
- ADR-019 (ordre d'autorité, registre)
- `docs/workstreams/conformite-corpus/reference.md`

## Lots

- Lot 1 — fichiers: docs/decisions/**, docs/architecture/** — vérités: 1, 2
- Lot 2 — fichiers: docs/assurance/**, docs/project/backlog/**, docs/workstreams/conformite-corpus/reference.md — vérités: 1, 2, 3, 4

## Périmètre

- `docs/decisions/**`
- `docs/architecture/**`
- `docs/assurance/**`
- `docs/project/backlog/**`
- `docs/workstreams/conformite-corpus/reference.md`
- hors couche, pour les seuls consommateurs des entrées 0015, 0049 et 0067 (contrat : « appliquée en entier ici ») : `docs/projet/conventions-dod.md`, `docs/projet/decisions-en-attente.md`, `docs/reference/dossier-architecture.md`

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

- Les 10 entrées sont « fait », dans leur fichier et dans l'index. Deux lots parallèles : le dossier de sécurité (0003, 0068) ; les ADR, les specs, la stratégie de test et les vérifications d'outillage (0008, 0009, 0014, 0015, 0049, 0066, 0067, 0069). Fusion à trois voies sans conflit.
- 0003 : les 45 contre-mesures sont recoupées avec leurs sources ; 4 sont corrigées, ainsi que deux renvois du dossier.
- Périmètre élargi aux seuls consommateurs des entrées : `conventions-dod.md` (0015), `decisions-en-attente.md` (0049), `dossier-architecture.md` (0067).
- Décisions de l'orchestrateur, prises par délégation de l'opérateur :
  - **classement STRIDE des n° 14 et 42** : la matrice et les tables S2 et S9 sont alignées sur le § 4. **Justification** : elles résument l'analyse du § 4, et aucune source amont ne fixe ce classement ;
  - **§ 4.9 du dossier** : le texte est ramené à ce que porte ADR-015 §Croisements (signature asymétrique), et l'ancre est ajoutée. **Justification** : l'aval se corrige vers sa source ;
  - **ADR-016 et ADR-017 reçoivent un complément post-revue daté du 2026-09-03**, qui renvoie au § 7 du guide de conventions (bibliothèque côté client, directives CSP, liste des balises sans objet). **Justification** : la décision existait déjà, datée (`04f16eb`, `fd9124f`), mais l'amont renvoyait encore ces points à P6 et B1.2. L'ADR-019 veut la source d'abord ; c'est le patron suivi pour ADR-014 à la tâche 8 ;
  - **§ 13 de la stratégie de test** : laissé tel quel. **Justification** : il énonce une exigence par jalon sans l'attribuer à un outil. L'outil manquant est différé à la tâche 22 (contrôles en CI).
- Différés :
  - le marqueur du reclaim-in-place (« à ratifier produit » contre « PRODUIT » dans ADR-015), dont les autres porteurs sont hors couche ;
  - les `RÉSOLU` de PO-04 et PO-06 dans la stratégie de test, à convertir avec les autres marqueurs ;
  - la télémétrie, encore dite ouverte dans `cahier-specifications-techniques.md` et `conception-detaillee.md` (tâche 14) ;
  - « ordre de build C#-first » dans `cadrage-validation-pre-lancement-eu.md` et `cahier-specifications-techniques.md`.
- Hash : `reference.md` porte la ligne de cette tâche, que la tâche 13 complétera.

## Notes de revue

Revue « chaque modification correspond-elle à une entrée, et une seule ? », 2026-10-07 : sept constats.

- `intent`, tranché par délégation : l'aval (dossier, guide de conventions) devançait ADR-016 et ADR-017. Les deux ADR reçoivent un complément post-revue (voir les notes de dev).
- `fix` : l'ancre « ADR-015 §Croisements » manquait au § 4.9.
- `fix` : les numéros de ligne de la conclusion de 0008 sont corrigés (`nRel++` l. 97, `Path.Exists` l. 102).
- `fix` (optionnel pris) : la preuve de 0009 repose sur l'argument chronologique. La comparaison de sorties opposait deux DLL reconstruites, et elle est reformulée.
- Optionnels gardés : les statuts « fait » sans hash (règle du hash différé) ; la ligne de périmètre de la spec ; « ordre de build C#-first » (différé).

Répartition : 1 `intent` tranché, 3 `fix`, 3 gardés.
