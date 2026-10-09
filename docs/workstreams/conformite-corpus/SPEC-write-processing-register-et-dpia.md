# SPEC — write-processing-register-et-dpia

## Contrat

### Pourquoi

Le module est sélectionné comme attendu (`docs/corpus.md`), et l'opérateur a demandé qu'il soit écrit dans ce chantier.

### État actuel

Aucune instance n'existe. Source disponible : les ADR RGPD (ADR-007, ADR-012, ADR-013), le dossier de sécurité et les cadrages de conformité (`docs/decisions/ADR-012-rgpd-effacement-compte.md:147`).

### Ce qui doit être vrai à la fin

- Chaque module a au moins une instance écrite par `/write <module>`, qui ne reprend que ce que ses sources établissent.
- L'index du module existe, et la carte du README le liste.
- Une section sans source n'est pas écrite (aucun gabarit vide).

### Contraintes et pièges

- Un seul commit pour la tâche (ADR-022).
- Ne jamais renuméroter ni réutiliser un identifiant (ADR-019, § Identifiants).
- Ne rien supprimer dont le contenu ne se retrouve pas ailleurs : `conform-verify` le détecte.
- Ouvrir la cible au moment d'écrire un renvoi (guide de conventions, § 8.4).
- Aucune valeur inventée : chaque nombre, seuil ou délai est copié avec sa référence de source.

### Hors contrat

- Toute décision nouvelle, sauf réponse de l'opérateur pendant `/write`.

### À lire avant de commencer

- la recette du module sous `.claude/corpus/modules/`
- les ADR RGPD (ADR-007, ADR-012, ADR-013), le dossier de sécurité et les cadrages de conformité

## Périmètre

- `docs/**`

## Revue

Le contrat et les sources : aucune valeur inventée ?

- general-purpose

## Vérités

1. Quand on lance `write.sh check <module>` sur chaque nouvelle instance, il sort `CLEAN`.
2. Quand on cherche chaque nombre de l'instance, on trouve à côté sa référence de source.
3. Quand on lance VerifCorpus, on obtient 0 orphelin et 0 lien mort.

## Vérification

- `dotnet test tools/Tests/CaracterisationOutils.csproj` : tous les tests passent.
- `dotnet run --project tools/VerifCorpus` : 0 lien mort, 0 ancre morte, 0 orphelin.
- `dotnet run --project tools/VerifMailleAgregat` : code 0.
- `dotnet run --project tools/VerifCouvertureRecette` : la même liste de TROU et de SOURCE RETIRÉE que la référence de départ.
- `bash .claude/skills/write/write.sh check <module> <fichier>`

## Notes de dev

- Deux instances écrites : `docs/assurance/securite/registre-des-traitements.md` (10 traitements, PR-01 à PR-10) et `docs/assurance/securite/analyse-d-impact.md` (11 risques, IR-01 à IR-11, et 12 mesures). Elles sont indexées dans `docs/assurance/securite/README.md` et inscrites au journal du corpus.
- Chemins : les libellés par défaut disent `docs/securite/`, que l'opérateur a rangé sous `docs/assurance/` ; la sélection porte les chemins réels.
- Aucune valeur sans source. Chaque durée, délai ou base légale porte sa référence (ADR-012, ADR-013, ADR-015, dossier de sécurité, télémétrie, cadrages de conformité). Une qualification juridique non donnée par les sources porte `[À VALIDER — JURISTE]`. Ce que le besoin ne tranche pas porte `[SOUS-SPÉCIFIÉ — …]` : la gravité et la vraisemblance des risques (aucune échelle sourcée), le schéma des événements de télémétrie.
- Contrôle `template` sur une racine minimale : 0 écart, en quelques secondes. Contrôle `markers` : 0 marqueur hors liste. `conform-verify-complet.sh a4a2bf7 --declared docs/workstreams/conformite-corpus/declares-processing-register-et-dpia.txt` : code 0.

## Notes de revue

Revue « aucune valeur inventée ? », 2026-10-09 : huit constats, tous appliqués.

- `fix` : la durée de PR-10 citait la durée du contrat de sous-traitance ; la conservation cite maintenant le § 8 du squelette de DPA.
- `fix` : PR-03 restreint aux `session_live_notes` `PLAYER_PRIVATE` (ADR-012 : les notes partagées sont conservées sous auteur anonymisé).
- `fix` : PR-06, la base légale d'ADR-013 § 1 ne couvre que le nom d'affichage et les journaux ; le reste porte le marqueur juriste.
- `fix` : les sous-traitants ultérieurs ont une liste indicative, non contractuelle (squelette de DPA § 5, ADR-013), citée avant le marqueur.
- `fix` (optionnels pris) : la réserve « à confirmer selon la stratégie de backup » sur les 30 jours de sauvegardes ; PR-08, l'identifiant de session invité et la durée en mode local prennent `SOUS-SPÉCIFIÉ`.
- `fix` (décision de l'orchestrateur, par délégation) : PR-02 et PR-07 portent la base légale que donnent leurs sources (intérêt légitime, ADR-012 § 1 et ADR-013 § 1), sans marqueur juriste. **Justification** : ce marqueur couvre une qualification qu'aucune source ne donne.

Répartition : 8 `fix`.
