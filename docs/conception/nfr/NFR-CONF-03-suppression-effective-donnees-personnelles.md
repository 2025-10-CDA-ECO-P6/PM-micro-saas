# NFR-CONF-03 — Suppression effective des données personnelles sur demande

Famille **Confidentialité** — [Index de conception](../README.md)

---

## Énoncé normatif

Lorsqu'un utilisateur supprime son compte, ses données personnelles sont effectivement supprimées du produit. L'utilisateur reçoit une confirmation de cette suppression. La suppression n'est pas réversible sans action de sa part (référence : droit à l'effacement — RGPD, article 17).

---

## Raison d'être

La protection des données personnelles des utilisateurs s'inscrit dans le cadre du règlement européen de protection des données personnelles (référence : RGPD), dont les exigences s'appliquent aux données traitées par le produit. Le droit à l'effacement (RGPD, article 17) est un droit reconnu à toute personne physique dont les données font l'objet d'un traitement — il n'est pas facultatif.

Au-delà de l'obligation légale, cette exigence exprime une condition de confiance dans le produit. Un utilisateur — MJ ou joueur — doit pouvoir quitter Haversack sans laisser de traces personnelles dans le système. Cette assurance est particulièrement importante dans un contexte où le produit traite des données liées à des activités récréatives privées : noms, notes personnelles, historiques de sessions.

**[Thomas](../persona/persona-01-thomas.md)** pose explicitement la question de la sortie sans lock-in : *« Peut-on exporter ses données si on arrête l'outil ? »*. La capacité de suppression effective est le pendant de la capacité d'export : l'une garantit la possession, l'autre garantit la maîtrise de la fin du traitement.

**[Lucas](../persona/persona-03-lucas.md)**, en tant que joueur accédant sans compte via un lien de session, représente un cas complémentaire : ses données personnelles (nom d'affichage, notes privées de session) ont une durée de vie bornée, distincte de la suppression de compte — couverte par NFR-CONF-03 uniquement dans sa dimension compte. Les règles d'effacement propres aux accès invités sans compte sont définies dans le domaine (RB-09-18, RB-09-19, UC-09).

Cette exigence est distincte des règles métier détaillées de suppression, qui vivent dans le domaine Identity & Access (règle F-08 : suppression physique des documents privés après suppression de compte) et dans les user stories correspondantes. NFR-CONF-03 exprime ce que l'utilisateur doit pouvoir constater et ressentir — la règle détaillée de ce qui est supprimé, dans quel ordre et avec quelles conséquences pour les contenus de campagne, est définie dans le domaine.

---

## Portée et hors-portée

**Ce que cette exigence couvre :**

- La suppression effective des données personnelles d'un utilisateur titulaire d'un compte (FREE ou PRO) qui en fait la demande.
- La confirmation reçue par l'utilisateur que la suppression a eu lieu.
- L'impossibilité de se reconnecter avec les mêmes identifiants après suppression.
- Le caractère irréversible de la suppression sans action de l'utilisateur (l'utilisateur peut créer un nouveau compte, mais ses données supprimées ne sont pas restaurées automatiquement).

**Ce que cette exigence ne couvre pas :**

- Le détail des catégories de données supprimées ou conservées (contenus de campagne sous intérêt légitime, données anonymisées) : ces règles vivent dans le domaine Identity & Access (règle F-08, invariant 6 de l'agrégat `User`) et dans la modélisation DDD correspondante.
- L'effacement des données des joueurs invités sans compte à la fin de leur accès : ce cas est régi par UC-09 (RB-09-18, RB-09-19) et la règle d'information RB-09-20. Il relève d'une durée de vie d'accès, pas d'une demande de suppression de compte.
- Le délai technique d'exécution de la suppression : ce paramètre n'est pas acté à ce stade.
- La suppression d'un compte suspendu : le comportement dans ce cas est défini dans Identity & Access.

---

## Critères d'acceptation produit

**Situation nominale — suppression du compte :**

Un utilisateur qui demande la suppression de son compte reçoit une confirmation visible que la suppression a été prise en compte. Ses données personnelles — nom d'affichage, adresse de messagerie, éléments d'identification — ne sont plus accessibles dans le produit. Une tentative de reconnexion avec les mêmes identifiants échoue.

**Situation limite — opération sensible requise avant suppression :**

La demande d'effacement des données personnelles est une opération sensible. Elle nécessite que l'adresse de messagerie du compte soit préalablement confirmée (US-UC-10, RB-10-05). Un compte dont l'adresse de messagerie n'a pas été confirmée est informé de cette condition avant que la demande puisse aboutir.

**Situation limite — tentative de reconnexion après suppression :**

Après suppression du compte, toute tentative de reconnexion avec les identifiants de ce compte est refusée. Le message affiché est générique — il ne révèle pas si le compte a été supprimé ou si les identifiants sont incorrects (principe de non-révélation d'existence).

**Situation limite — accès aux données après suppression :**

Un utilisateur dont le compte a été supprimé ne peut plus accéder à ses données personnelles via le produit. Les contenus créés qui sont conservés dans les campagnes (sous intérêt légitime pour la continuité des campagnes actives) apparaissent sous une identité anonymisée — ils ne permettent pas d'identifier leur auteur.

---

## Traçabilité montante

| Artefact | Nature du lien |
|---|---|
| [domain/identity-access.md](../domain/identity-access.md) | Règle F-08 : suppression physique des documents privés à la suppression de compte (RGPD Art. 17) ; invariant de l'agrégat utilisateur : la suppression est irréversible, les données identifiantes sont remplacées par des valeurs neutres. |
| [UC-10 — Compte cloud](../usecases/UC-10-compte-cloud.md) | Précondition : la demande d'effacement nécessite une adresse de messagerie vérifiée (opération sensible). |
| [US-UC-10 — Compte cloud](../user-stories/US-UC-10-compte-cloud.md) | RB-10-05 : la validation de l'adresse de messagerie est requise avant les opérations sensibles, dont la demande d'effacement des données personnelles. |
| [UC-09 — Accès session joueur](../usecases/UC-09-acces-session-joueur.md) | RB-09-18, RB-09-19 : règles d'effacement distinctes pour les accès invités sans compte (durée de vie bornée de l'accès, suppression des notes privées à la fin définitive de l'accès). Ces règles sont complémentaires à NFR-CONF-03, non redondantes. |
| [US-UC-09 — Accéder à une session en tant que joueur](../user-stories/US-UC-09-acces-session-joueur.md) | RB-09-18, RB-09-19, RB-09-20 : cycle de vie et information des invités sans compte. |
| [Vision produit §5 — Différenciants](../vision/vision-produit.md) | La possession des données est actionnable : « le MJ peut exporter l'ensemble de sa campagne dans un format ouvert ». La suppression effective en est le pendant : ce qui peut être conservé peut aussi être effacé sur demande. |
| [Persona Thomas](../persona/persona-01-thomas.md) | Question explicite sur la sortie sans lock-in et la portabilité des données. |
