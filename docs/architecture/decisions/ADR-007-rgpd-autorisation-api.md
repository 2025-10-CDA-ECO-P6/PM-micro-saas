# ADR-007 — Conformité RGPD et modèle d'autorisation API

- **Statut** : Accepté
- **Date** : 2026-06-09
- **Décideur** : opérateur (validation explicite, session d'audit/remédiation)
- **Findings liés** : F-03, F-04, F-05, F-12, C-10, F-02, F-06, F-10, F-11

---

## Contexte

L'audit a relevé plusieurs lacunes dans deux domaines distincts mais liés.

**RGPD.** Le droit à l'effacement était repoussé post-MVP sans procédure documentée (finding F-03). Les données des joueurs sans compte (GuestAccess) n'étaient pas couvertes par une base légale explicite (finding F-04). La question de l'âge minimal (Art. 8 RGPD) n'était pas adressée (finding F-12).

**Autorisation API.** Aucun modèle d'autorisation au niveau des endpoints API n'était décrit (finding F-05). Cette absence expose le système à des vulnérabilités de type IDOR (Insecure Direct Object Reference) : un utilisateur authentifié pourrait accéder à des ressources appartenant à une campagne à laquelle il n'appartient pas.

---

## Décision

**1. Droit à l'effacement implémenté au MVP.**
Le domaine `User` modélise déjà `User.Delete()` et `User.Anonymize()`. L'effacement est mis en œuvre dès le MVP par anonymisation par réécriture : `email` et `display_name` sont remplacés par des valeurs neutres (`[Compte supprimé]`), l'`id` de l'utilisateur est conservé comme référence morte dans les autres contextes. Le hard delete n'est pas envisageable tant que des campagnes référencent le `UserId` (voir ADR-009).

**2. Âge minimal : 16 ans.**
À l'inscription, l'utilisateur coche une case déclarant avoir au moins 16 ans. Aucun mécanisme de consentement parental n'est implémenté.

**3. Invariant d'autorisation API.**
Toute requête portant sur une ressource vérifie que cette ressource appartient à une campagne accessible à l'appelant. Un appelant est « accessible » s'il est membre actif (`CampaignMembership.status = ACTIVE`) ou s'il dispose d'un `GuestAccess` actif sur la campagne ou la session concernée. Cet invariant est appliqué dans la couche Application, distinct des règles métier du domaine.

---

## Alternatives considérées

**Effacement RGPD post-MVP avec procédure manuelle + lancement restreint géographiquement.**
Écartée. L'opérateur veut un lancement public EU dès le MVP. Un lancement public en Union Européenne sans procédure d'effacement documentée et fonctionnelle expose l'éditeur à des obligations réglementaires non couvertes.

**Âge minimal 13 ans avec consentement parental (Art. 8 RGPD option membre).**
Écartée. Le mécanisme de consentement parental vérifiable est complexe à implémenter de manière conforme et à maintenir. La limite à 16 ans est plus stricte et évite ce mécanisme.

---

## Conséquences

- Le modèle d'anonymisation est à concevoir en cohérence avec la FK `ownerId` (ADR-009) : un `USER` ne peut pas être hard-deleted tant qu'une campagne le référence — d'où l'anonymisation par réécriture comme seule option. Ce couplage est documenté dans ADR-009.
- La base légale et la durée de conservation des données des joueurs invités (GuestAccess, sans compte) sont documentées dans **[ADR-013](ADR-013-rgpd-donnees-invites.md)** (finding F-04 — résolu).
- Les éléments suivants ont été spécifiés dans **[ADR-015](ADR-015-securite-authentification-mvp.md)** (résolu) : politique de mot de passe (finding F-02), validation email asynchrone / opérations sensibles (finding F-10 et F-11), durée et révocation des JWT (finding F-10), liaison OAuth sécurisée (finding F-11). Rate limiting endpoint de validation de token (finding F-06) → **ADR-015** (résolu).
- L'invariant d'autorisation API (point 3) doit être intégré dans les contrats de la couche Application avant le début du jalon J1 (Vague 3). Le modèle d'autorisation concret — principal (membre/invité), composition des quatre prédicats, matrice ressource↔campagne, systématisation pipeline behavior et droits invité — est défini dans **[ADR-014](ADR-014-modele-autorisation-api.md)**.
- Une note sur les notes `PLAYER_PRIVATE` d'un compte supprimé : ces documents sont **supprimés physiquement** à la suppression du compte (ADR-012 §3(a), règle F-08) — ils ne sont pas conservés ni rattachés au personnage. La continuité de campagne concerne les contenus partagés conservés sous intérêt légitime (`PUBLIC`, `GM_ONLY`), pas les notes auteur-seul. **Statué dans [ADR-012](ADR-012-rgpd-effacement-compte.md) §4 (règle F-08).**
- **Périmètre Art. 17 et procédure d'effacement étendu** : la politique de sélection des documents supprimables à la demande d'un utilisateur (notes `PLAYER_PRIVATE`, documents non partagés, conservation sous intérêt légitime), la procédure d'anonymisation complète et les obligations Art. 12§3 sont définis dans **[ADR-012](ADR-012-rgpd-effacement-compte.md)**.
- **Base légale invités F-04** : la base légale retenue pour les données des joueurs invités (`GuestAccess.display_name`), l'obligation d'information Art. 13, la rétention autonome des `guest_accesses` expirés et la posture sur les mineurs sont définis dans **[ADR-013](ADR-013-rgpd-donnees-invites.md)**.

---

## Compléments post-revue (2026-06-09)

Suite à la revue adversariale (revue Vague 0, artefact purgé du corpus — historique git), cette décision est complétée comme suit, sans changer sa direction.

- **Posture RGPD actée : pseudonymisation documentée.** L'`UserId` est conservé pour la continuité de campagne. Base légale = intérêt légitime. Les limites de cette posture sont documentées dans la politique de confidentialité.

- **Email anonymisé = valeur unique.** Le format retenu est `deleted-{id}@haversack.invalid` pour ne pas violer la contrainte `UNIQUE` sur la colonne email. La neutralisation de `asp_net_users` (réécriture du hash de mot de passe et du `security_stamp`) est obligatoire, pas optionnelle. Cette correction doit être faite AVANT d'implémenter `Anonymize()` — un second effacement casserait sinon la contrainte.

- **Invariant d'autorisation étendu au canal SignalR.** Conformément à ADR-004, chaque message poussé sur le canal SignalR est filtré selon la même règle d'appartenance ressource↔campagne définie au point 3 de cette décision.

- **Dépendance vers ADR-002.** L'invariant d'autorisation exige que `characterId` et `guestAccessId` soient des colonnes indexées de premier niveau (ADR-002). Sans cette promotion, l'invariant s'exécute sur du jsonb non indexé.

- **Bloquants MVP reclassés.** Les items suivants étaient en Vague 2 ; ils sont reclassés bloquants MVP et **résolus dans [ADR-015](ADR-015-securite-authentification-mvp.md)** :
  - F-11 + F-02 : validation email avant liaison OAuth + politique mot de passe (longueur ≥ 8 via `ASP.NET Identity PasswordOptions`) → **ADR-015 (résolu)**.
  - F-10 : access token ≤ 15 min, refresh token ≤ 7 jours avec rotation, denylist JTI → **ADR-015 (résolu)**.
  - F-04 : base légale + durée de conservation + information Art. 13 RGPD pour les données des joueurs invités, avant tout lancement EU → **ADR-013 (résolu)**.

- **F-08 — notes PLAYER_PRIVATE post-suppression.** Tranché dans **[ADR-012](ADR-012-rgpd-effacement-compte.md) §3(a) + §4** : suppression physique obligatoire à la suppression du compte. Aucune conservation ni rattachement au personnage. Le cas du nouvel invité réassocié au même `characterId` est couvert — les notes sont supprimées avant toute réassociation possible.
