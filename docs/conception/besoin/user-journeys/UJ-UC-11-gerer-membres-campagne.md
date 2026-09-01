# User Journey — Gérer les membres d'une campagne (UC-11)

## Périmètre

Parcours couvrant deux flux distincts du point de vue du MJ : la gestion rapide des invitations (Émilie — table occasionnelle, friction minimale) et la gestion complète des membres (Thomas — table stable, association joueur-personnage, surveillance des accès). Le flux du joueur qui rejoint suite à une invitation est couvert par UC-09 (scénario nominal campagne, avec compte) et UC-09 (sans compte) ; UC-12 couvre la vue obtenue par le joueur après l'accès. L'invitation par email est hors MVP.

---

## Carte d'expérience

```mermaid
journey
    title Gerer les membres d une campagne
    section Emilie genere un lien en 30 secondes
        Ouvrir la section Membres de la campagne: 5: Emilie
        Cliquer sur Inviter un joueur: 5: Emilie
        Choisir le perimetre CAMPAIGN ou SESSION: 4: Emilie
        Cliquer sur Generer le lien: 5: Emilie
        Copier le lien et le coller dans Discord: 5: Emilie
    section Thomas configure une invitation avec options
        Ouvrir la section Membres: 5: Thomas
        Choisir le perimetre et la session cible: 4: Thomas
        Configurer la date d expiration: 4: Thomas
        Configurer le nombre max d utilisations: 3: Thomas
        Generer et copier le lien: 5: Thomas
    section Thomas associe un joueur a son personnage
        Joueur a rejoint la campagne via le lien: 5: Thomas
        Ouvrir la fiche du membre dans la liste: 4: Thomas
        Selectionner le personnage a associer: 4: Thomas
        Confirmer l association: 5: Thomas
    section Thomas revoque une invitation
        Reperer une invitation active non souhaitee: 4: Thomas
        Cliquer sur Revoquer: 5: Thomas
        Confirmer la revocation: 4: Thomas
        Invitation passe a l etat REVOKED: 5: Thomas
    section Thomas retire un membre
        Reperer un membre inactif dans la liste: 4: Thomas
        Cliquer sur Retirer: 4: Thomas
        Confirmer le retrait: 3: Thomas
        Membre passe a l etat REMOVED - donnees preservees: 4: Thomas
```

---

## Flux fonctionnel

```mermaid
flowchart TD
    Start[MJ ouvre la section Membres] --> Action{Quelle action ?}

    Action -->|Inviter un joueur| Inv[US-11-01\nGenerer un lien d invitation]
    Inv --> Perimetre{Perimetre du lien}
    Perimetre -->|CAMPAIGN| LienCampagne[Lien campagne durable\nMember apres utilisation]
    Perimetre -->|SESSION| LienSession[Lien session temporaire\nGuestAccess apres utilisation]

    LienCampagne --> Options[Options : date d expiration\nnombre d utilisations - facultatif]
    LienSession --> Options

    Options --> Generate[Space Management\ncree l Invitation\nIdentity and Access\ngenerele token]
    Generate --> Copy[Bouton Copier\nretour visuel toast\nMJ partage via Discord etc]

    Copy --> Wait{Joueur utilise le lien ?}
    Wait -->|Oui - avec compte| MemberActive[Member ACTIVE → vue UC-12]
    Wait -->|Oui - sans compte| GuestOK[GuestAccess actif\nUC-09]
    Wait -->|Joueur deja membre| E1[E1 - Doublon detecte\nMJ informe - pas de creation]
    Wait -->|Lien expire ou quota atteint| AutoRevoke[Invitation REVOKED\nautomatiquement]

    Action -->|Revoquer une invitation| Rev[US-11-02\nRevoquer invitation]
    Rev --> RevConfirm[Confirmation MJ]
    RevConfirm --> Revoked[Invitation REVOKED\nIdentity and Access invalide le token]
    Revoked --> JoueurLien{Joueur tente le lien apres revocation}
    JoueurLien --> LienMort[Message Ce lien n est plus actif\nOpacite campagne - UC-09 US-09-02]

    Action -->|Retirer un membre| Rem[US-11-03\nRetirer un membre]
    Rem --> RemConfirm[Confirmation MJ]
    RemConfirm --> Removed[Member REMOVED\nAcces campagne retire\nDonnees preservees]
    Removed --> Reinvite{MJ veut reinviter ?}
    Reinvite -->|Oui| Inv
    Reinvite -->|Non| End1[Fin - membre hors campagne]

    Action -->|Associer un personnage| Assoc[US-11-04\nAssocier joueur et personnage]
    Assoc --> SelectMembre[MJ selectionne le membre]
    SelectMembre --> SelectPerso[MJ selectionne le personnage\npersonnage joueur de la campagne]
    SelectPerso --> UniqueCheck{Personnage deja associe ?}
    UniqueCheck -->|Oui| UniqueErr[Erreur - personnage\ndeja associe a un autre membre]
    UniqueCheck -->|Non| AssocOK[Association creee\nJoueur accede a la fiche\net aux notes PLAYER_PRIVATE]
```

---

## Points de friction identifiés

- **Choix du périmètre peu intuitif pour Émilie** : la distinction `CAMPAIGN` / `SESSION` est technique. Émilie veut juste "inviter ses joueurs pour ce soir". Un libellé orienté usage ("Accès permanent à la campagne" / "Accès pour cette session") réduirait la friction cognitive.

- **Génération du lien hors de la vue session** : Émilie veut souvent inviter ses joueurs juste avant de démarrer la session. Si le point d'entrée est uniquement dans la section "Membres" de la campagne, elle doit naviguer hors de la vue session en cours. Un raccourci depuis la vue session (UC-06) éviterait cette rupture.

- **Absence de confirmation visuelle que le joueur a rejoint** : Thomas génère un lien et attend. Sans indication dans l'interface que le lien a été utilisé, il ne sait pas si son joueur a bien rejoint. L'état de l'invitation (`PENDING` → `ACTIVE`) doit être visible en temps réel dans le panneau membres.

- **Association joueur-personnage silencieuse** : Thomas associe un joueur à son personnage mais ne sait pas si le joueur l'a vu. Si l'association a lieu alors que le joueur est déjà connecté, une notification in-app côté joueur éviterait les demandes répétées ("tu as ma fiche ?").

- **Retrait d'un membre — confirmation anxiogène** : le MJ craint de supprimer des données. Un message de confirmation explicite ("les données du joueur — personnage, notes — sont conservées dans la campagne") réduit l'hésitation.

- **Quota d'usages — valeur par défaut ambiguë** : si Thomas ne renseigne pas de nombre maximum d'utilisations, le comportement par défaut (illimité ou 1 ?) n'est pas évident. La valeur par défaut doit être visible et documentée dans le formulaire.

---

## Opportunités UX

- **Bouton "Inviter" dans la vue session** : depuis la vue session active (UC-06), un bouton "Inviter" génère directement un lien `SESSION` sans quitter la vue. Émilie invite ses joueurs en une action depuis l'endroit où elle se trouve déjà.

- **Libellés orientés usage plutôt que techniques** : remplacer `CAMPAIGN` / `SESSION` par "Accès durable à la campagne" / "Accès pour une session" dans le formulaire, tout en conservant les codes internes dans les données.

- **Indicateur d'état en temps réel dans le panneau membres** : chaque invitation affiche son état actuel (`PENDING`, `ACTIVE`, `REVOKED`) avec une mise à jour en temps réel quand un joueur l'utilise. Thomas sait immédiatement que son joueur a rejoint.

- **Copie du lien en un clic avec toast** : un bouton "Copier le lien" avec retour visuel discret (toast "Lien copié !") suffit. Pas de popup, pas de modale, pas de navigation.

- **Message de confirmation rassurant au retrait** : lors du retrait d'un membre, un message explicite ("Le personnage et les notes de ce joueur restent dans la campagne. Seul l'accès est retiré.") réduit l'anxiété de suppression.

- **Réinvitation depuis la fiche membre REMOVED** : depuis la fiche d'un membre `REMOVED`, un bouton "Réinviter" déclenche directement la génération d'un nouveau lien (US-11-01) pré-configuré pour ce membre, sans navigation supplémentaire.

- **Panneau membres synthétique** : Thomas dispose d'un panneau unique listant membres actifs, invitations en attente et membres retirés, avec le statut de chaque accès, l'association personnage, et les actions disponibles (Révoquer, Retirer, Associer, Réinviter).

---

## Liens

- Use case source : [`docs/conception/besoin/usecases/UC-11-gerer-membres-campagne.md`](../usecases/UC-11-gerer-membres-campagne.md)
- User stories associées : [`US-UC-11-gerer-membres-campagne.md`](../user-stories/US-UC-11-gerer-membres-campagne.md)
- UC-09 Accès session joueur : [`docs/conception/besoin/usecases/UC-09-acces-session-joueur.md`](../usecases/UC-09-acces-session-joueur.md)
- UC-12 Consulter sa campagne (vue joueur) : [`docs/conception/besoin/usecases/UC-12-rejoindre-campagne.md`](../usecases/UC-12-rejoindre-campagne.md)
- User Journey UC-09 : [`UJ-UC-09-acces-session-joueur.md`](UJ-UC-09-acces-session-joueur.md)
- Conception Space Management : [`docs/conception/domain/space-management.md`](../../domain/space-management.md)
- Conception Identity and Access : [`docs/conception/domain/identity-access.md`](../../domain/identity-access.md)
