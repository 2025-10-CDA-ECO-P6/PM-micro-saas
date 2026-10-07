# SPEC — backlog-besoin

## Contrat

### Pourquoi

Les réponses de l'opérateur et les alignements de l'onboarding sont reportés dans les sources, à leur emplacement définitif (ADR-021). C'est le seul commit de la couche qui change la substance (ADR-022, condition 3).

### État actuel

Les 36 entrées de la couche sont listées dans l'index `docs/project/backlog/README.md:1`. Chaque entrée cite ses lignes d'origine aux anciens chemins (antérieurs à F1).

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

- les 36 entrées de la couche
- ADR-019 (ordre d'autorité, registre)
- `docs/workstreams/conformite-corpus/reference.md`

## Périmètre

- `docs/besoin/**`
- `docs/project/backlog/**`
- `docs/workstreams/conformite-corpus/reference.md`

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

- Les 36 entrées sont « fait », dans leur fichier et dans l'index ; 0036 l'est « sans objet au MVP » (0081). Trois lots en parallèle (cas d'utilisation, matrice, stories et autres), fusionnés à trois voies (6 conflits résolus en combinant les deux apports), puis trois passages d'alignement des consommateurs aval, hors couche compris (référence, assurance, interface, domaine, projet), comme le contrat le prévoit.
- Décisions de l'opérateur (2026-10-07) : au MVP, seul l'OWNER gère les membres (UC-11 ; le droit du GM vaut à l'activation du rôle, hors MVP) ; le message unique d'un lien inutilisable est « Ce lien n'est pas utilisable ».
- Décisions de l'orchestrateur, par conséquence de décisions déjà prises : 0033 étendue à US-03-05 ; 0032 propagée (phrase des 14 epics, sous-capacités Could Have écrites hors matrice rangées dans la matrice) ; 0081 propagée, les règles RGPD d'effacement des notes restant en vigueur ; chemin des entretiens aligné sur la sélection.
- Hash du commit : un commit ne peut pas porter son propre hash. Les entrées désignent le commit de la tâche, et `reference.md` porte une table de réépinglage que la tâche `gabarits-domaine` complète avec le hash.
- Reporté à `backlog-domaine` : `content-library/context.md` (règles 2 et 3, `CanBeReadBy`) contredit l'exception de RB-12-01 (0024).
- Ambigus, non tranchés : les « notes partagées » du membre (RB-11-13, RB-11-21, UC-11).

## Notes de revue

Revue « chaque modification correspond-elle à une entrée, et une seule ? », 2026-10-07 : neuf constats.

- `fix` : UC-F08 attribuait une priorité hors matrice. Le rôle GM est classé Won't Have dans la matrice (0026, 0032), et UC-F08 y renvoie.
- `fix` : notes périmées de 0081 et 0055 mises à jour ; statut de 0036 précisé ; fichiers hors couche ajoutés aux lignes « Appliqué dans » (0024, 0025, 0026, 0029, 0055, 0081, 0084).
- `fix` : le constat de 0011 ne prescrit plus de remède.
- `intent` : le hash différé (paradoxe d'un commit sur lui-même), soumis à l'opérateur.
- Optionnels gardés : chemin des entretiens (sélection), justifications des sections Could Have ajoutées, détail d'interface de l'archivage (UC-06 A7).

Répartition : 3 `fix`, 1 `intent`, 3 optionnels gardés.

