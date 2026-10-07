# 0010 — Vérifier la définition de GuestAccess, Non classés et LIVE_NOTE

- **Id** : 0010
- **Date** : 2026-10-07
- **Statut** : fait — commit de la tâche backlog-domaine (chantier conformite-corpus)
- **Appliqué dans** : aucun fichier modifié (vérification)
- **Conclusion** : les trois termes sont définis, aucun correctif. `GuestAccess` : `docs/glossaire.md`, entrée `GuestAccess` (statuts, portée, propriétaire Space Management), et `docs/domaine/space-management/context.md`, section « GuestAccess (agrégat séparé) ». « Non classés » : `docs/glossaire.md`, entrée `Folder` (dossier virtuel, `isVirtual = true`), et `docs/domaine/content-library/context.md`, champ `isVirtual` et invariant 4. `LIVE_NOTE` : `docs/glossaire.md`, entrée `LIVE_NOTE` (note de session), `docs/domaine/content-library/context.md`, section DocumentType (types système, propriétés du type `live_note`), et `docs/domaine/session-conduct/context.md`, section « Notes de session ». Preuve : `git grep -n "^### .GuestAccess\|^### .LIVE_NOTE\|isVirtual = true" docs/glossaire.md` rend l'entrée `GuestAccess`, l'entrée `LIVE_NOTE` et la définition du dossier virtuel.

Vérifier la définition de GuestAccess, Non classés et LIVE_NOTE. Ces termes sont utilisés dans les US sans y être définis ; vérifier qu'ils le sont dans le glossaire ou le domaine.

**Raison.** Trou de lecture de l'analyse d'onboarding du 2026-10-07, classé B (à vérifier) par l'opérateur
