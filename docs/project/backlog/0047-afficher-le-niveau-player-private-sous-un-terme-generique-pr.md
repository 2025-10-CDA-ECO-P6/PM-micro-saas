# 0047 — Afficher le niveau PLAYER_PRIVATE sous un terme générique, « Privé (joueur) »

- **Id** : 0047
- **Date** : 2026-10-07
- **Statut** : fait — commit de la tâche backlog-interface (chantier conformite-corpus)
- **Appliqué dans** : `docs/interface/conventions.md` (Registre 1 : lignes « privé (joueur) » et « note de session personnelle joueur » ; Famille 4 — Visibilité), `docs/interface/ecrans/sv3-preparation/editeur-document/editeur-document.md` (zone de visibilité), `docs/glossaire.md` (§ LIVE_NOTE, cartographie des niveaux ; table des termes dépréciés : lignes « note privée » et `personnelle joueur`)
- **Note** : le niveau `PLAYER_PRIVATE` s'affiche « Privé (joueur) » ; « note de session personnelle joueur » reste le nom du type de note. La fiche `editeur-document.md` (zone de visibilité) n'emploie plus « note de session personnelle joueur » comme niveau : elle affiche « Privé (joueur) ».

Afficher le niveau PLAYER_PRIVATE sous un terme générique, « Privé (joueur) ». « Note de session personnelle joueur » reste le nom du type note (conventions-wireframe.md:60), puisque le domaine applique PLAYER_PRIVATE à tout type de document.

**Raison.** Réponse de l'opérateur, onboarding corpus du 2026-10-07

## Alternatives écartées

- Restreindre PLAYER_PRIVATE aux notes dans le domaine
