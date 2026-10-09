# Catalogue des événements

<!-- corpus:generated:events:begin -->
| Événement | Producteur | Consommateurs | Cas d'utilisation |
|---|---|---|---|
| AccountSuspended | User.Suspend() (identity-access) | Application (toutes les sessions actives de l'utilisateur sont immédiatement révoquées — ADR-015) | - |
| AccountTierChanged | User.ChangeTier() (identity-access) | Space Management (quotas espaces/joueurs), Application (notification) | UC-15 |
| CharacterAssociated | Space.AssociateCharacter() (space-management) | Application (résout les personnages associés au joueur pour l'exception de RB-12-01 — règle 2 de Content Library ; PLAYER_PRIVATE reste lisible par son seul auteur) | - |
| DisplayNameUpdated | User.UpdateDisplayName() (identity-access) | Space Management, Session Conduct (affichage du nom dans les vues joueur et membre) | - |
| DocumentCreated | Document.Create() (content-library) | Session Conduct (si créé à la volée en session) | - |
| DocumentDeleted | Document.Delete() (content-library) | Session Conduct (retirer des documents épinglés si actif en session) | - |
| DocumentInstantiated | Document.Instantiate() (content-library) | Space Management si besoin | - |
| DocumentLinked | Document.LinkDocument() (content-library) | — (backlinks calculés en lecture) | - |
| DocumentPinned | Session.PinDocument() (session-conduct) | Application (épinglage automatique UC-07, UC-08) | - |
| DocumentUnlinked | Document.UnlinkDocument() (content-library) | — | - |
| DocumentUnpinned | Session.UnpinDocument() (session-conduct) | — | - |
| DocumentVisibilityChanged | Document.Share() / Unshare() (content-library) | Session Conduct (mise à jour de la vue joueur en temps réel) | - |
| EmailChangeRequested | User.ChangeEmail() (identity-access) | Application (envoi de la confirmation à la nouvelle adresse, préalable à sa prise en compte) | - |
| EmailVerified | User.VerifyEmail() (identity-access) | Interne I&A — aucun autre contexte consommateur ; débloque en interne les opérations sensibles conditionnées par emailVerified (invariant 7) | - |
| FederatedIdentityLinked | User.LinkFederatedIdentity() (identity-access) | Interne I&A — aucun autre contexte consommateur | - |
| FolderCreated | Folder.Create() (content-library) | — | - |
| FolderDeleted | Folder.Delete() (content-library) | Session Conduct (retirer les documents du dossier supprimé si actifs en session) | - |
| GuestAccessConvertedToMember | GuestAccess.Convert(userId) (space-management) | Application (finalise la création de compte), Session Conduct | - |
| GuestAccessCreated | GuestAccess.Create() (space-management) | Session Conduct (accès aux informations partagées) | - |
| GuestAccessExpired | GuestAccess.Expire() (space-management) | Session Conduct | - |
| GuestAccessRevoked | GuestAccess.Revoke() (space-management) | Session Conduct (couper l'accès actif en session si présent) | - |
| InitialPasswordSet | User.SetInitialPassword() (identity-access) | Interne I&A — aucun autre contexte consommateur ; déclenche côté infrastructure d'identité l'enregistrement du mot de passe complémentaire, après vérification de la preuve d'identité alternative (règle métier n°6) | - |
| InvitationCreated | Space.CreateInvitation() (space-management) | Application (fourniture du lien au MJ) | - |
| InvitationRevoked | Space.RevokeInvitation() (space-management) | Aucun contexte externe — l'invalidation du lien d'invitation reste interne à Space Management | - |
| MemberActivated | SpaceMembership.Activate() (space-management) | Session Conduct (autoriser l'accès en session), Application | - |
| MemberJoined | Space.AddMember() (space-management) | Application (notification) | - |
| MemberRemoved | Space.RemoveMember() (space-management) | Session Conduct (retirer l'accès actif si session en cours) | - |
| SessionArchived | Session.Archive() (session-conduct) | — | - |
| SessionClosed | Session.Close() (session-conduct) | Application (déclenche l'expiration des GuestAccess SESSION dans Space Management — règle métier 6) | - |
| SessionStarted | Session.Start() (session-conduct) | Application (notification membres) | - |
| SpaceArchived | Space.Archive() (space-management) | Content Library, Session Conduct | - |
| SpaceCreated | Space.Create() (space-management) | Content Library (créer les dossiers système), Application | UC-02 |
| SpaceDeleted | Space.Delete() (space-management) | Content Library, Session Conduct, Application | - |
| SpaceFrozen | Space.Freeze() (space-management) | Application (notification au MJ) | UC-15 |
| SpaceUnarchived | Space.Unarchive() (space-management) | Content Library, Session Conduct | - |
| SpaceUnfrozen | Space.Unfreeze() (space-management) | Application (notification au MJ) | UC-15 |
| UserAnonymized | User.Anonymize() (identity-access) | Interne I&A — déclenché après confirmation de UserDeleted ; déclenche la révocation de toutes les sessions actives de l'utilisateur (ADR-015) | - |
| UserDeleted | User.Delete() (identity-access) | Space Management (routage cascade par type d'espace, invariant 3 : hard-delete de l'espace PERSONAL via la saga SpaceDeleted, anonymisation ownerId pour CAMPAIGN/ONE_SHOT), Content Library (hard-delete des documents PLAYER_PRIVATE créés par l'utilisateur — F-08/RB-4) | - |
| UserRegistered | User.Register() (identity-access) | Application (email de bienvenue), Space Management (création de l'espace PERSONAL du compte — invariant 14 ; conversion depuis GuestAccess le cas échéant) | - |
<!-- corpus:generated:events:end -->
