# User Journey — Partager une information aux joueurs (UC-08)

## Périmètre

Parcours du MJ depuis l'identification d'un document à partager jusqu'à la confirmation que les joueurs y ont accès. Couvre trois profils : Émilie (partage à la volée en session, besoin de rapidité), Thomas (partage préparé au moment précis d'une révélation), Nadia (partage occasionnel d'une aide de jeu simple). La gestion de session est couverte par UC-06.

---

## Carte d'expérience

```mermaid
journey
    title Partager une information avec les joueurs
    section Identifier le contenu a partager
        Ouvrir un document depuis la bibliotheque: 5: Thomas, 4: Nadia
        Trouver le document depuis la vue session: 4: Émilie, 3: Thomas
    section Partager
        Cliquer Partager: 5: Thomas, 5: Émilie, 5: Nadia
        Confirmer si demande: 3: Thomas, 2: Émilie, 4: Nadia
        Constater la confirmation visuelle: 4: Thomas, 4: Émilie, 3: Nadia
    section Verifier la visibilite joueur
        Document visible dans l'espace joueur: 5: Thomas, 4: Nadia
        Document epingle dans la vue session: 5: Émilie, 4: Thomas
    section Retirer le partage
        Trouver le document partage: 4: Thomas, 3: Nadia
        Cliquer Retirer le partage: 5: Thomas, 4: Nadia
        Constater que le document est redevenu prive: 4: Thomas, 4: Nadia
```

---

## Flux fonctionnel

```mermaid
flowchart TD
    A[MJ identifie un document a partager] --> B{Point d entree}

    B -->|Depuis la bibliotheque de contenu| C[Ouvre le document\nClique Partager]
    B -->|Depuis la vue session LIVE| D[Clique Partager\ndepuis le panneau de session]

    C --> E[Document.Share\nGM_ONLY vers PUBLIC\nDocumentVisibilityChanged]
    D --> E

    E --> F{Partage depuis session LIVE ?}
    F -->|Oui| G[Session Conduct consomme DocumentVisibilityChanged\nauto-epinglage dans documents epingles]
    F -->|Non| H[Document PUBLIC\npas d auto-epinglage]

    G --> I[Document epingle dans la session\nJoueurs voient le document en temps reel]
    H --> J[Document visible dans l espace joueur\ndurable entre les sessions]

    I --> K{MJ retire le partage ?}
    J --> K

    K -->|Oui| L[Document.Unshare\nPUBLIC vers GM_ONLY\nDocumentVisibilityChanged]
    L --> M[Document redevenu GM_ONLY\nJoueurs ne voient plus le document\nDocument reste dans documents epingles — visible MJ uniquement]

    K -->|Non| N[Partage durable\npersiste jusqu a retrait explicite]
```

---

## Points de friction identifiés

- **Confirmation superflue pour Émilie** : une boite de dialogue de confirmation interrompt le rythme de la partie. Émilie partage à la volée — chaque seconde compte.
- **Manque de retour visuel sur l'état de partage** : sans indicateur clair sur le document (icône, badge), le MJ ne sait pas d'un coup d'oeil quels documents sont `PUBLIC` ou `GM_ONLY`. Thomas prépare plusieurs révélations — il a besoin de distinguer visuellement l'état de chacune.
- **Auto-épinglage non perceptible** : le document est ajouté aux `documents épinglés` automatiquement lors d'un partage depuis la vue session, mais si aucun indicateur visuel ne le confirme, Émilie ne sait pas que le document est désormais accessible aux joueurs dans la session.
- **Retrait du partage difficile à trouver** : l'action "Retirer le partage" peut être enfouie dans un menu contextuel ou peu visible si l'interface ne distingue pas les documents `PUBLIC` des documents `GM_ONLY`.

---

## Opportunités UX

- **Indicateur d'état de visibilité persistant** : un badge ou une icône sur chaque document (bibliotheque et vue session) indique son état (`GM_ONLY` / `PUBLIC`) sans ouvrir le document.
- **Action directe sans confirmation en session** : pour Émilie, le partage depuis la vue session ne demande pas de confirmation. Une confirmation peut être proposée depuis la bibliothèque (contexte non urgent).
- **Toast de confirmation après partage** : un retour visuel bref (toast) confirme le partage et l'auto-épinglage sans bloquer l'interaction.
- **Accès rapide au partage depuis la vue session** : un bouton ou un raccourci visible sur le document dans la liste des `documents épinglés` ou dans le panneau de documents de la session, sans ouvrir le document.

---

## Liens

- Use case source : [`docs/conception/usecases/UC-08-partager-information.md`](../usecases/UC-08-partager-information.md)
- User stories associées : [`US-UC-08-partager-information.md`](../user-stories/US-UC-08-partager-information.md)
- UC-06 Vue session : [`docs/conception/user-journeys/UJ-UC-06-vue-session.md`](UJ-UC-06-vue-session.md)
- UC-07 Création à la volée : [`docs/conception/user-journeys/UJ-UC-07-creation-volee-session.md`](UJ-UC-07-creation-volee-session.md)
- Conception Content Library : [`docs/conception/domain/content-library.md`](../domain/content-library.md)
- Conception Session Conduct : [`docs/conception/domain/session-conduct.md`](../domain/session-conduct.md)
