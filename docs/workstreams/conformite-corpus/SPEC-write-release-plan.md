# SPEC — write-release-plan

## Contrat

### Pourquoi

Le module est sélectionné comme attendu (`docs/corpus.md`), et l'opérateur a demandé qu'il soit écrit dans ce chantier.

### État actuel

Aucune instance n'existe. Source disponible : la roadmap d'entrée en build et le plan de travail (`docs/projet/roadmap-de-build.md:117`).

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
- la roadmap d'entrée en build et le plan de travail

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

- Instance écrite : `docs/projet/plan-de-livraison.md`, indexée dans `docs/projet/README.md` et inscrite au journal du corpus. Le libellé par défaut disait `docs/pilotage/` ; la couche projet vit sous `docs/projet/` (ADR-020).
- Trois livraisons, toutes sourcées : Local-only (J1) à la cohorte pilote ; le socle ; l'approfondissement du cœur. J0 et J2 ne sont pas des livraisons, faute de mise à disposition d'un public dans les sources. Les critères de sortie ne sont pas recopiés : chaque champ renvoie à la roadmap de build, qui en est propriétaire.
- Décisions de l'orchestrateur, prises par délégation de l'opérateur :
  - **la décision de marché est une porte écrite en texte, sans marqueur**. **Justification** : la roadmap la prévoit (§ 3.3) ; ce n'est pas un point de conception non tranché. Le marqueur `À DÉCIDER` exige un modèle de fonctionnement, qui n'est pas écrit (tâche 19 reportée). La validation juridique porte `[À VALIDER — JURISTE]` sur ses 5 axes ;
  - **J1 est une livraison à la cohorte pilote**. **Justification** : la décision de marché mesure un usage réel de ce que J1 livre (roadmap § 3.3), et la vision nomme ce public (§ 2.3, « cohorte pilote restreinte »).
- Contrôles `template` et `markers` sur une racine minimale : 0 écart. `conform-verify-complet.sh 34cb703 --declared docs/workstreams/conformite-corpus/declares-release-plan.txt` : code 0.
- Différé au solde du backlog : le plan de travail place US-UC-11 et US-UC-12 en J3, et la roadmap produit dans l'approfondissement du cœur.

## Notes de revue

Revue « aucune valeur inventée ? », 2026-10-09 : quatre constats, tous appliqués.

- `intent`, tranché par délégation : la livraison J1 à la cohorte pilote est ajoutée (voir les notes de dev).
- `fix` : l'attribution de la décision de marché au responsable produit renvoie à la roadmap § 1, qui la porte.
- `fix` : le marqueur juriste porte sur les 5 axes, et non sur « sans bloquer le build », qu'établit la roadmap.
- `fix` (optionnel pris) : les constats d'état « non franchi » deviennent des renvois aux décisions en attente.

Répartition : 1 `intent` tranché, 3 `fix`.
