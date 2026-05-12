# Ubiquitous Language

Vocabulaire partagé du domaine. Ces termes sont utilisés tels quels dans le code,
la documentation et les conversations.

| Terme | Définition |
|---|---|
| **Campagne** | Espace organisationnel regroupant scénarios, PNJ, personnages, sessions et notes d'une aventure JDR. |
| **MJ** | Maître du Jeu. Rôle contextuel dans une campagne (`MemberRole.OWNER`). Propriétaire de la campagne, droits complets. Tout utilisateur authentifié peut être MJ en créant une campagne. |
| **Joueur** | Rôle contextuel dans une campagne (`MemberRole.PLAYER`). Membre authentifié associé à un ou plusieurs personnages joueurs. Un utilisateur peut être joueur dans certaines campagnes et MJ dans d'autres. |
| **Joueur invité** | Accès temporaire sans compte persistant. Représenté par un GuestAccess, pas un User. Traité comme un joueur ordinaire pour l'accès au contenu dès lors qu'il est associé au bon Personnage joueur. |
| **Membre** | Toute personne ayant accès à une campagne : joueur authentifié (CampaignMembership) ou joueur invité actif (GuestAccess). Un contenu PUBLIC est accessible à tous les membres, y compris les invités. |
| **Scénario** | Structure narrative préparée par le MJ, composée de scènes ordonnées. |
| **Scène** | Unité narrative d'un scénario. Peut être liée à des PNJ. |
| **PNJ** | Personnage Non-Joueur. Entité narrative créée et gérée par le MJ. |
| **Personnage joueur** | Fiche d'un personnage. Porte l'identité fonctionnelle côté joueur dans la campagne : un compte joueur ou un GuestAccess peut être autorisé à accéder à ce personnage. Peut exister sans accès associé. |
| **Document** | Unité de contenu modulaire. Tout contenu éditorial est un Document typé composé de blocs. |
| **Bloc** | Unité atomique de contenu dans un Document. Chaque bloc a un type et une valeur fortement typée. |
| **Template** | Schéma de blocs définissant la structure attendue d'un Document. Snapshot à la création — non lié après. |
| **Tag** | Étiquette créée au niveau de la campagne. Réutilisable sur n'importe quel Document de la campagne. Permet le filtrage et la navigation transversale. |
| **Dossier** | Conteneur organisationnel créé par le MJ pour regrouper ses documents librement. Quatre dossiers système existent dans chaque campagne (PNJ, Personnages joueurs, Scénarios, Notes). Le MJ peut créer des dossiers personnalisés. |
| **Backlink** | Référence inverse — liste des documents qui pointent vers un document donné via un bloc RELATION. Calculé à la lecture depuis l'index, sans table de liaison dédiée. Un backlink pointant vers un document supprimé n'est pas affiché. |
| **Session** | Instance d'une partie jouée. Liée optionnellement à un scénario. |
| **Note live** | Note prise pendant une session. Peut être créée par le MJ ou par un joueur. Liée automatiquement à la session en cours. |
| **Résumé** | Compte-rendu d'une session clôturée. Peut être partagé aux joueurs. |
| **Visibilité** | Niveau d'accès d'une ressource partageable : PRIVATE (MJ uniquement), PLAYER_PRIVATE (personnage joueur propriétaire uniquement — MJ exclu), SHARED (membres ou personnages ciblés via AccessPolicy), PUBLIC (tous les membres, y compris les invités actifs). |
| **RequesterId** | Identité du demandeur d'accès à un contenu. Type union scellé : soit un utilisateur authentifié (`UserId` + `CharacterId?`) soit un invité (`GuestAccessId` + `CharacterId?`). Utilisé par AccessPolicy. |
| **AccessPolicy** | Service domaine gérant les autorisations d'accès aux ressources partageables d'une campagne : Document, LiveNote, SessionSummary. |
| **GuestAccess** | Accès temporaire sécurisé dans une campagne. N'est pas un User — pas d'identité persistante. Il donne accès au périmètre du personnage auquel il est associé. |
| **Invitation** | Token généré par le MJ permettant à un joueur de rejoindre une campagne. |
| **Système de jeu** | Référentiel de règles d'un JDR (D&D 5e, Call of Cthulhu, etc.). Point d'extension futur. |
| **Slug** | Identifiant lisible généré depuis un titre. Utilisé pour l'affichage. Les URLs utilisent l'Id UUID — le slug n'est jamais dans les routes. |
| **Audit** | Traçabilité des créations et modifications : qui, quand. |
| **Soft delete** | Suppression logique — l'entité est marquée supprimée mais reste en base pour l'intégrité référentielle. |
