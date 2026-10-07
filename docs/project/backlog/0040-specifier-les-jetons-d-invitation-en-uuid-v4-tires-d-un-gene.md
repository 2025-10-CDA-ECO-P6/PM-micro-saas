# 0040 — Spécifier les jetons d'invitation en UUID v4 tirés d'un générateur cryptographique

- **Id** : 0040
- **Date** : 2026-10-07
- **Statut** : fait — commit de la tâche backlog-domaine (chantier conformite-corpus)
- **Appliqué dans** : `docs/domaine/space-management/context.md` (champs `token` d'`Invitation` et de `GuestAccess`, value objects `InvitationToken` et `GuestAccessToken`), `docs/domaine/space-management/logical-data.md` (colonnes `token` de `invitations` et `guest_accesses`)

Spécifier les jetons d'invitation en UUID v4 tirés d'un générateur cryptographique. 122 bits aléatoires ; l'UUID v7, dont le préfixe temporel est prévisible, est exclu. À écrire dans le domaine et le MLD (space-management.md:114, :146).

**Raison.** Réponse de l'opérateur, onboarding corpus du 2026-10-07

## Alternatives écartées

- Jeton opaque dédié de 256 bits en base64url
