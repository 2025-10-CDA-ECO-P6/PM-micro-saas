# SPEC — identifiants

## Contrat

### Pourquoi

Un identifiant n'a qu'une origine (ADR-019, § Identifiants). Le plan calculé relève RB-05-01, défini deux fois.

### État actuel

RB-05-01 est défini à `docs/besoin/stories/US-UC-05-organiser-contenu-dossiers.md:122` et à `:162`. Le plan propose RB-05-13 pour le second.

### Ce qui doit être vrai à la fin

- Le premier porteur garde RB-05-01.
- Le second reçoit le prochain identifiant libre, donné par `corpus.sh next-id`.
- Chaque citation du second est trouvée par son texte, non par son identifiant, puis redirigée.
- Aucun autre identifiant n'est défini deux fois.

### Contraintes et pièges

- Un seul commit pour la tâche (ADR-022).
- Ne jamais renuméroter ni réutiliser un identifiant (ADR-019, § Identifiants).
- Ne rien supprimer dont le contenu ne se retrouve pas ailleurs : `conform-verify` le détecte.
- Ouvrir la cible au moment d'écrire un renvoi (guide de conventions, § 8.4).

### Hors contrat

- Les autres familles (gabarits, contenu).

### À lire avant de commencer

- ADR-019, § Identifiants
- la ligne F4 du plan calculé

## Lots

- Lot 1 — fichiers: docs/besoin/**, docs/assurance/** — vérités: 1, 2, 3
- Lot 2 — fichiers: docs/projet/** — vérités: 4

## Périmètre

- `docs/besoin/**`
- `docs/assurance/**`
- `docs/projet/**`

## Revue

Le contrat : un seul porteur par identifiant ?

- general-purpose

## Vérités

1. Quand on cherche les définitions d'identifiants dupliquées (même identifiant en tête de deux règles), on n'en trouve aucune.
2. Quand on cherche RB-05-01, on ne trouve que sa définition d'origine et les citations qui la visent.
3. Quand on cherche le nouvel identifiant, on trouve la définition déplacée et toutes les citations qui visaient le second porteur.
4. Quand on lance VerifMailleAgregat et VerifCouvertureRecette, leurs compteurs sont ceux de la référence.

## Vérification

- `bash .claude/skills/onboard/conform.sh verify <commit de départ>` : code 0. Les seules lignes déclarées (`--declared`) sont des titres renommés, des chemins réécrits, des mots-clés Gherkin traduits ou des replis avec renvoi.
- `dotnet test tools/Tests/CaracterisationOutils.csproj` : tous les tests passent.
- `dotnet run --project tools/VerifCorpus` : 0 lien mort, 0 ancre morte, 0 orphelin.
- `dotnet run --project tools/VerifMailleAgregat` : code 0.
- `dotnet run --project tools/VerifCouvertureRecette` : la même liste de TROU et de SOURCE RETIRÉE que la référence de départ.

## Notes de dev

- RB-05-01 : le second porteur (`US-UC-05:162`) devient RB-05-13, valeur donnée par `corpus.sh next-id . user-stories business-rule US-UC-05`. La plage de TB-063 passe à RB-05-01 → RB-05-13.
- Doublons trouvés en plus : RB-09-18 à RB-09-22, définies à la fois dans US-UC-09 et dans UC-09, avec des textes qui divergent. Décision de l'opérateur : US-UC-09 garde les définitions, avec le texte de l'UC (plus riche) et les apports propres à l'US ; UC-09 n'y renvoie plus que par une ligne. EP-04, répété d'un jalon à l'autre dans le plan de travail, est un faux positif.
- Contrôle rejouable : `python3 -I docs/workstreams/conformite-corpus/controle-doublons-identifiants.py` (0 doublon). Familles non couvertes : voir l'en-tête du script.
- `conform.sh verify 282e2b6 --declared docs/workstreams/conformite-corpus/declares-identifiants.txt` : code 0. Ce fichier cumule les lignes déclarées depuis le commit de départ.

## Notes de revue

Revue « un seul porteur par identifiant ? », 2026-10-07 : quatre constats.

- Script et lignes déclarées dans `docs/workstreams/` : rejeté. C'est l'outillage du chantier, nécessaire pour rejouer la tâche.
- RB-09 rapatriées au lieu d'être renumérotées : `intent`, tranché par l'opérateur.
- Ancre du renvoi UC-09 → US-09-01 : rejeté, VerifCorpus la valide (0 ancre morte).

Répartition : 1 `intent`, 3 rejetés.

