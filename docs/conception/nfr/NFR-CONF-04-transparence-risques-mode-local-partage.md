# NFR-CONF-04 — Transparence sur les risques de confidentialité en mode local partagé

Famille **Confidentialité** — [Index de conception](../README.md)

---

## Énoncé normatif

Lorsque l'application détecte que les données locales d'un MJ pourraient être consultées par un tiers sur le même appareil, elle l'en informe de manière visible et non bloquante, et lui propose un moyen de sécuriser ses données.

---

## Raison d'être

Le mode local sans compte repose sur un choix de conception délibéré : les données de campagne sont stockées dans l'espace de stockage de l'appareil utilisé, sans protection par mot de passe ni compte utilisateur. Ce choix permet une friction d'entrée nulle (vision §5) et une utilisation immédiate, mais il implique une limite assumée : sur un appareil partagé, toute personne ayant accès à ce même appareil et navigateur peut potentiellement lire ces données.

Cette limite ne peut pas être effacée par le mode local — elle en est une contrainte structurelle, documentée dans UC-01 et assumée pour le MVP (US-UC-01, RB-01-14). L'information honnête de l'utilisateur sur cette limite est préférable à une promesse de sécurité que le mode local ne peut pas tenir.

Un MJ qui prépare ses campagnes sur un ordinateur familial ou un poste partagé doit pouvoir prendre une décision éclairée : continuer en mode local en connaissance des limites, ou créer un compte pour bénéficier d'une protection associée à des identifiants. C'est lui qui décide — le produit l'informe et lui propose une alternative, sans bloquer son usage.

La confiance dans un produit qui gère des données de préparation narrative — parfois riches en informations que le MJ souhaite garder secrètes de ses joueurs — passe par cette honnêteté. Un bandeau de confidentialité non bloquant, systématique en mode local, est le vecteur choisi pour cette transparence.

---

## Portée et hors-portée

**Ce que cette exigence couvre :**

- L'affichage d'un bandeau d'information non bloquant, systématiquement présent en mode local, signalant le risque d'accès aux données par une autre personne utilisant le même appareil et navigateur.
- La proposition, depuis ce bandeau, d'un moyen de sécuriser les données : la création d'un compte, qui associe les données à des identifiants personnels.
- La distinction de ce bandeau d'avec le bandeau de durabilité (NFR-OFF-03) : les deux bandeaux ont des causes, des risques et des audiences cibles distincts — ils ne doivent pas être fusionnés (US-UC-01, RB-01-14).

**Ce que cette exigence ne couvre pas :**

- La protection technique des données stockées localement contre la lecture par un tiers : cette protection n'est pas livrée en MVP, c'est une limite assumée (US-UC-01, RB-01-14 — « limitation MVP assumée »). Cette exigence ne demande pas que le risque soit éliminé, mais qu'il soit communiqué.
- Le mode cloud (compte créé) : une fois le MJ authentifié avec un compte, ses données sont associées à ses identifiants. Cette exigence ne s'applique qu'au mode local sans compte.
- La protection contre l'accès aux données par d'autres applications sur le même appareil : ce périmètre dépasse la responsabilité produit et n'est pas couvert.
- La détection automatique de l'identité d'un autre utilisateur sur le poste : le produit ne sait pas si le poste est effectivement partagé. Le bandeau est affiché systématiquement en mode local, sans tentative de détection.

---

## Critères d'acceptation produit

**Situation nominale — premier accès en mode local :**

Un MJ qui choisit de commencer sans compte voit, dès les premières interactions avec l'application en mode local, un bandeau d'information non bloquant lui signalant que les données stockées localement sont lisibles par toute personne ayant accès à ce navigateur sur cet appareil. Ce bandeau lui propose de créer un compte comme alternative plus sécurisée.

**Situation nominale — usage continu en mode local :**

Le bandeau de confidentialité reste présent pendant toute la durée des sessions en mode local. Il ne disparaît pas après un premier affichage, puisque le risque qu'il signale est permanent tant que le MJ reste en mode local. Il est non bloquant : le MJ peut ignorer l'information et continuer à utiliser l'application normalement.

**Situation limite — distinction avec le bandeau de durabilité :**

Un MJ en mode local qui voit à la fois le bandeau de confidentialité et le bandeau de durabilité (déclenché si le navigateur n'a pas garanti la conservation permanente des données — NFR-OFF-03) distingue clairement deux messages séparés portant sur deux risques distincts. Les deux bandeaux ne sont pas fusionnés en un seul message générique.

**Situation limite — action depuis le bandeau :**

Le MJ qui clique sur l'invitation du bandeau de confidentialité est dirigé vers le parcours de création de compte (UC-10). Cette action est proposée, jamais imposée : le bandeau ne bloque pas l'accès à l'application si le MJ choisit de ne pas créer de compte.

---

## Traçabilité montante

| Artefact | Nature du lien |
|---|---|
| [UC-01 — Mode local sans compte](../usecases/UC-01-mode-local-sans-compte.md) | Règle métier : « Bandeau de confidentialité : affiché systématiquement en mode local — risque qu'une personne ayant accès à ce navigateur sur ce poste puisse lire les données. Les données ne sont pas chiffrées au repos, ce qui est une limitation du MVP assumée. » |
| [US-UC-01 — Mode local sans compte](../user-stories/US-UC-01-mode-local-sans-compte.md) | RB-01-14 : deux bandeaux distincts et non bloquants — bandeau de durabilité (conditionnel) et bandeau de confidentialité (systématique en mode local). Les deux bandeaux ne doivent pas être fusionnés. |
| [UC-10 — Compte cloud](../usecases/UC-10-compte-cloud.md) | L'invite du bandeau de confidentialité pointe vers la création de compte comme alternative sécurisée. |
| [Vision produit §5 — Différenciants](../vision/vision-produit.md) | « Friction d'entrée nulle » comme premier différenciant : la transparence sur les limites du mode local est la condition pour que cette promesse soit honnête. |
