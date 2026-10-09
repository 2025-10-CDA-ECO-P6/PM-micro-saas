# User Journey — Accéder à une session en tant que joueur (UC-09)

## Périmètre

Parcours couvrant deux flux distincts : l'accès d'un joueur via lien ponctuel sans compte (Lucas, Émilie), et la génération du lien par le MJ ainsi que l'accès via lien permanent (Thomas). Le flux central est celui de Lucas — joueur type sans compte, profil d'adoption critique. La gestion des membres de l'espace partagé est couverte par UC-11. La création de compte est couverte par UC-10.

---

## Personas concernés

| Persona | Mode de travail | Attente principale | Risque de friction |
|---|---|---|---|
| Lucas | accès via lien ponctuel sans compte ; joueur type sans compte, profil d'adoption critique (flux central) | SOUS-SPÉCIFIÉ | SOUS-SPÉCIFIÉ |
| Émilie | accès via lien ponctuel sans compte | SOUS-SPÉCIFIÉ | SOUS-SPÉCIFIÉ |
| Thomas | génération du lien par le MJ ; accès via lien permanent | SOUS-SPÉCIFIÉ | SOUS-SPÉCIFIÉ |

---

## Carte d'expérience

```mermaid
journey
    title Acces a une session en tant que joueur
    section MJ genere le lien de session
        Ouvrir la vue session: 5: Émilie
        Ouvrir la vue session: 4: Thomas
        Cliquer Generer un lien de session: 5: Émilie, Thomas
        Copier le lien et le partager: 5: Émilie
        Copier le lien et le partager: 4: Thomas
    section Joueur rejoint via lien ponctuel
        Cliquer sur le lien recu: 5: Lucas
        Voir la page d acces sans inscription: 5: Lucas
        Saisir uniquement un nom d affichage: 4: Lucas
        Acceder a la vue joueur: 5: Lucas
    section Joueur consulte les informations partagees
        Voir les documents PUBLIC de l espace partage: 5: Lucas
        Voir les documents epingles de la session: 5: Lucas
        Prendre des notes personnelles - hors MVP: 4: Lucas
    section MJ invite un membre regulier via lien permanent
        Generer un lien permanent pour un membre: 5: Thomas
        Partager le lien au joueur: 5: Thomas
        Joueur cree un compte si necessaire: 3: Lucas
        Joueur acces a l historique et au lore: 5: Lucas
    section Acces expire ou revoque
        Joueur tente d ouvrir un lien expire: 2: Lucas
        Voir le message « Ce lien n'est pas utilisable »: 3: Lucas
        Contacter le MJ pour un nouveau lien: 3: Lucas
```

---

## Flux fonctionnel

```mermaid
flowchart TD
    A[MJ genere un lien] --> B{Type de lien}

    B -->|Ponctuel session| C[Invitation portee SESSION\nliee a la session]
    B -->|Permanent espace partage| D[Invitation portee SPACE\nliee a l espace partage]

    C --> E[MJ partage le lien\nDiscord, WhatsApp, email]
    D --> E

    E --> F[Joueur clique sur le lien]
    F --> G[Identity and Access valide le token]

    G --> H{Token valide ?}
    H -->|Expire, revoque, invalide, mal forme\nou espace archive| I[Message lien non utilisable\nContacter le MJ\nOpacite sur l existence du lien et de l espace partage]

    H -->|Valide mais limite FREE atteinte\n5e joueur| I2[Acces refuse - message sobre\nMJ invite a passer PRO]
    H -->|Valide — ponctuel| K{Joueur deja connecte ?}
    K -->|Non| L[Saisit uniquement un nom d affichage]
    K -->|Oui| M[Acces direct avec historique de compte]

    L --> N[GuestAccess actif\nVue joueur]
    M --> N

    H -->|Valide — permanent| O{Joueur a un compte ?}
    O -->|Oui| P[Acces espace partage permanent\nHistorique sessions et lore partage]
    O -->|Non| Q[Invite a creer un compte\nUC-10]
    Q --> P

    N --> R[Documents PUBLIC\nDocuments epingles de la session\nNotes personnelles PLAYER_PRIVATE - hors MVP]

    N --> S{Joueur veut creer un compte ?}
    S -->|Oui| T["Migration GuestAccess vers compte\nNotes preservees (hors MVP) — US-09-04"]
    T --> V{MJ valide l adhesion ?\nRB-09-16}
    V -->|Oui| P
    V -->|Pas encore| W[Acces invite conserve\nGuestAccess CONVERTED]
    S -->|Non| U[Acces expire apres session + 24h de grace]
```

---

## Points de friction

| Étape | Persona(s) | Friction | Opportunité produit |
|---|---|---|---|
| SOUS-SPÉCIFIÉ | Lucas | **Saisie du nom d'affichage superflue si le joueur est déjà connecté** : Lucas peut avoir un compte Haversack sans le savoir ou sans être connecté. L'interface doit détecter silencieusement une session existante et proposer "Continuer en tant que Lucas (connecté)" plutôt que redemander un nom. | SOUS-SPÉCIFIÉ |
| SOUS-SPÉCIFIÉ | Lucas | **Manque de retour visuel sur l'expiration imminente** : Lucas ne sait pas que son accès expire 24 heures après la fin de session. Si le MJ clôture la session tard dans la nuit, Lucas peut perdre l'accès à ses notes avant de les relire *(hors MVP — notes de session du joueur, Could Have ; voir la [matrice de priorités](../vision/perimetre/matrice-de-priorites.md#notes-personnelles-joueur))* — sans avoir été averti. | SOUS-SPÉCIFIÉ |
| SOUS-SPÉCIFIÉ | Lucas | **Friction lors de la création de compte depuis l'accès invité** : le moment où Lucas veut créer un compte pour garder ses notes *(hors MVP — notes de session du joueur, Could Have ; voir la [matrice de priorités](../vision/perimetre/matrice-de-priorites.md#notes-personnelles-joueur))* coïncide souvent avec la fin de session (fatigue, rush). Si la procédure d'inscription est longue ou exige une validation d'email immédiate, il abandonne. | SOUS-SPÉCIFIÉ |
| SOUS-SPÉCIFIÉ | Lucas | **Opacité du message d'erreur pour un lien inutilisable (expiré, révoqué, invalide ou mal formé)** : le principe d'opacité est correct d'un point de vue sécurité, mais Lucas ne comprend pas pourquoi le lien ne fonctionne pas. Le message doit être sobre mais suffisamment actionnable ("contacter le MJ") pour éviter la confusion. | SOUS-SPÉCIFIÉ |
| SOUS-SPÉCIFIÉ | Thomas | **Génération du lien — Thomas ne sait pas si le lien permanent est déjà actif** : si Thomas génère plusieurs liens pour le même espace partagé ou modifie les membres, l'interface doit clairement indiquer l'état actif/révoqué de chaque lien dans le panneau membres. | SOUS-SPÉCIFIÉ |

---

## Opportunités UX

- **Accès en un clic avec nom pré-rempli depuis Discord / WhatsApp** : si le lien est accompagné d'un paramètre de nom (optionnel, fourni par le MJ), la page d'accès peut pré-remplir le champ de nom d'affichage. Lucas n'a qu'à confirmer — friction réduite à zéro.

- **Détection silencieuse de session existante** : à l'arrivée sur la page de lien, l'interface vérifie si un compte est déjà connecté et propose directement l'accès avec ce compte, sans redemander le nom. Un lien discret "Continuer en tant qu'invité" reste disponible.

- **Bandeau de migration non intrusif** : après quelques sessions passées en invité, un bandeau discret en bas de la vue joueur propose "Créer un compte pour garder tes notes" *(hors MVP — notes de session du joueur, Could Have ; voir la [matrice de priorités](../vision/perimetre/matrice-de-priorites.md#notes-personnelles-joueur))*. Il ne bloque pas l'accès et ne s'affiche qu'une fois par session.

- **Indicateur d'expiration visible dans la vue joueur** : une mention discrète ("Accès valide jusqu'au [date+24h]") dans la vue joueur permet à Lucas d'anticiper l'expiration et de créer un compte ou de demander un nouveau lien à temps.

- **Copie du lien en un clic depuis la vue session** : Émilie génère et partage le lien sans quitter la vue session. Un bouton "Copier le lien" avec retour visuel (toast) suffit — pas de popup, pas de navigation.

- **Panneau membres avec état des liens pour Thomas** : chaque membre ou `GuestAccess` dans le panneau affiche son état (actif, expiré, révoqué) avec une action rapide "Révoquer" ou "Générer un nouveau lien".

---

## Liens

- Use case source : [`docs/besoin/cas-d-utilisation/UC-09-acces-session-joueur.md`](../cas-d-utilisation/UC-09-acces-session-joueur.md)
- User stories associées : [`US-UC-09-acces-session-joueur.md`](../stories/US-UC-09-acces-session-joueur.md)
- UC-10 Compte cloud : [`docs/besoin/cas-d-utilisation/UC-10-compte-cloud.md`](../cas-d-utilisation/UC-10-compte-cloud.md)
- UC-11 Gérer les membres d'un espace partagé : [`docs/besoin/cas-d-utilisation/UC-11-gerer-membres-espace-partage.md`](../cas-d-utilisation/UC-11-gerer-membres-espace-partage.md)
- UC-08 Partager information : [`docs/besoin/parcours-utilisateur/UJ-UC-08-partager-information.md`](UJ-UC-08-partager-information.md)
- Conception Identity and Access : [`docs/domaine/identity-access/context.md`](../../domaine/identity-access/context.md)
- Conception Space Management : [`docs/domaine/space-management/context.md`](../../domaine/space-management/context.md)
- Conception Session Conduct : [`docs/domaine/session-conduct/context.md`](../../domaine/session-conduct/context.md)
