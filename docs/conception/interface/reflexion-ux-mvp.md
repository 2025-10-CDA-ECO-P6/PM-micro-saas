# Réflexion UX/UI — MVP Haversack

> Document de rationale de conception d'interface.
> Couche : interface (en aval de zoning.md, en aval des fiches wireframe).
> Méthode : 4 lentilles d'analyse appliquées aux 20 écrans du MVP.
> Les résolutions sont portées dans le zoning et les fiches concernées — ce document en est le rationale, pas la source des arbitrages.
> Hiérarchie d'autorité respectée : vision > personas/parcours > UC > NFR > interface (zoning.md) > fiches wireframe.

---

## 1. Objet & méthode

Ce document restitue et structure une réflexion UI/UX globale menée sur l'ensemble des 20 écrans du MVP Haversack. Son rôle est d'exposer le raisonnement qui a conduit aux décisions d'interface consignées dans `zoning.md` et dans les fiches wireframe — pas de poser des règles nouvelles.

### Les 4 lentilles appliquées

La réflexion a été conduite en 4 lentilles successives, appliquées à chaque surface :

1. **Architecture de l'information & navigation** — cohérence de l'ossature, des transitions, des retours ; détection des tensions entre surfaces.
2. **Questions de design** — questions ouvertes non résolues par le corpus, points de présentation (pas de structure) à trancher avant de wireframer.
3. **Charge informationnelle** — densité réelle de chaque écran ; risques de surcharge ; identification des zones qui s'accumulent parce qu'on les a, pas parce qu'elles servent.
4. **Résolution des zones sous-spécifiées & adéquation personas** — inventaire des marqueurs `<!-- TODO -->`, répartition par famille, priorisation, adéquation de l'ensemble aux 7 personas.

### Grille de lecture appliquée partout

Pour chaque zone de chaque écran : *est-ce utile ? pertinent au moment ? remplit-il son but sans excès d'information ?* Cette grille a produit les constats et décisions des sections §3 à §7.

### Ce que ce document est et n'est pas

Ce document est un **rationale** : il explique pourquoi les décisions ont été prises. Les résolutions elles-mêmes (arbitrages transversaux, modifications de fiches, nouvelles règles de châssis) sont portées dans `zoning.md` et dans les fiches concernées. Les renvois précis sont en §8.

Il ne contredit pas le corpus amont. Toute décision mentionnée ici qui touche la structure est déjà tracée en tant qu'arbitrage AR dans `zoning.md` ou signalée comme devant y être portée.

---

## 2. L'espace personnel (question phare) — décision : le mix assumé

### Frontière fonctionnelle verrouillée

La frontière fonctionnelle de l'espace personnel n'est pas une question ouverte : elle est déjà résolue par le corpus. L'espace personnel est de type `PERSONAL`, hors quota, mono-membre, mène uniquement à Préparation (pas de vue session), et constitue le conteneur par défaut de tout contenu créé hors d'un espace partagé explicite. Ces contraintes sont posées par AR-14, AR-16, AR-17 et confirmées par UC-02 §interview.

La question ouverte n'est donc pas structurelle — c'est une question de **présentation** : comment l'espace personnel se présente-t-il à l'utilisateur ? Ce point est explicitement ouvert dans `zoning.md §S9` et dans UC-02 §interview.

### Décision retenue : le mix

L'option retenue est le **mix** — déjà ancré dans le corpus avant cette réflexion sous trois formes :

- une entrée distincte hors-quota au tableau de bord (AR-17),
- un écran d'atterrissage capture-first (AR-15),
- un rôle de conteneur de transit pour le contenu hors-campagne.

Il n'y a pas de quatrième emplacement à inventer. La décision est de rendre ce mix **lisible**, pas de le remplacer.

### Libellé confirmé

Le libellé retenu est « **Espace personnel** » (AR-05). Ce libellé est confirmé à l'issue de cette réflexion comme le plus neutre vis-à-vis de la double nature décrite ci-dessous.

### Double nature d'un même conteneur

L'espace personnel porte deux usages distincts, coexistants dès le MVP :

**(i) Bac de capture et de brouillon** — usage Nadia (MJ occasionnelle) : un endroit où déposer une idée, une note, sans avoir à créer une campagne. Premier contact avec l'outil, friction quasi nulle.

**(ii) Foyer de contenus réutilisables** — usage Antoine (MJ multi-campagnes) : un réservoir de documents, notes et scénarios « que les campagnes consomment ». Ce second usage anticipe la bibliothèque « Mes scénarios » post-MVP (AR-08).

Ces deux usages ne sont pas contradictoires, mais leur coexistence crée un **risque de dette de perception** si l'espace personnel est présenté exclusivement comme un espace de brouillons : les utilisateurs de type Antoine ne s'y reconnaîtraient pas, et la valeur du contenu qu'ils y rangent serait implicitement dépréciée.

### Parade retenue : micro-copy d'intention neutre

La parade n'est pas d'implémenter la couche (ii) au MVP — c'est de ne pas **fermer** le sens de l'espace personnel par une formulation restrictive. La micro-copy d'intention retenue est :

> **« Vos notes et contenus, hors campagne »**

Cette formulation est compatible avec les deux usages sans en privilégier un. Elle ne requiert pas d'implémentation supplémentaire au MVP. Elle est posée dans `zoning.md §S9` (question ouverte de présentation) et portée dans la fiche `vue-espace-personnel`.

### L'espace personnel ne s'explique pas — il se découvre

L'espace personnel ne se présente pas par un texte d'explication. Son modèle est celui du capture-first (AR-15) : l'utilisateur y arrive avec une zone de saisie au premier plan. Il comprend l'espace par l'usage, pas par la lecture d'un paragraphe d'accueil.

---

## 3. Cohérence d'ensemble — architecture de l'information & navigation

### Socle sain : 3 forces identifiées

L'analyse de l'architecture de l'information révèle un socle cohérent, avec trois points de force structurants.

**Châssis mode local appliqué à la lettre** — Le mode local (RB-01 à RB-14) est présent sur toutes les surfaces MJ sans exception. L'utilisateur sans compte accède au même espace de travail qu'un utilisateur connecté, avec des limites progressivement exposées (AR-13, AR-17). Aucune surface ne court-circuite ce modèle.

**Frontière MJ/joueur étanche et tenue bout-en-bout** — La séparation entre la surface MJ et la surface joueur est un déterminant de premier ordre (AR-03, NFR-CONF-01). Cette séparation n'est pas une option d'implémentation — c'est une garantie de conception. Elle est vérifiable sur chaque écran : aucune surface MJ n'expose de structure invisible depuis la surface joueur, et inversement.

**Reprise de session doublement câblée** — La reprise d'une session en cours est accessible depuis deux points d'entrée distincts : le tableau de bord (repère « session en cours » direct vers le mode LIVE) et la vue campagne. AR-09 garantit la continuité d'état. Ce doublement est intentionnel : il sécurise le parcours du MJ qui revient après une interruption.

### 6 surfaces documentaires → 3 surfaces perçues

Le corpus documente 6 surfaces (espace personnel, campagne, vue session, surface joueur, profil/compte, gate de migration). L'utilisateur en perçoit 3 au quotidien : tableau de bord/espace, vue session, surface joueur. La frontière MJ/joueur est la seule frontière forte du point de vue utilisateur — bien placée, car elle correspond à un besoin réel de séparation (NFR-CONF-01).

### 3 faiblesses à corriger

**Faiblesse 1 — Configuration de la vue session logée à deux endroits**

La configuration de la vue session est accessible depuis deux surfaces : `parametres-campagne` (comme section) et `vue-session-mj` en mode configuration. Cette duplication est en tension avec US-06-02 (*« pas de panneau de réglages séparé »*). La résolution retenue : `parametres-campagne` pointe vers le mode configuration ; la configuration effective s'effectue dans la surface session. Portée dans `zoning.md` avec un nouvel arbitrage AR.

**Faiblesse 2 — Retours de navigation non spécifiés**

Les règles de retour entre écrans ne sont pas spécifiées dans le corpus. Le risque est réel : ouvrir un résultat de recherche depuis une session LIVE puis « revenir » peut éjecter le MJ de sa session. La résolution retenue repose sur deux règles :

- *Retour contextuel* : tout écran de détail retourne à sa surface appelante en préservant l'état.
- *Recherche = composant de châssis* : la recherche est un overlay omniprésent, jamais un écran à part entière. Ouvrir un document depuis la recherche ne change pas la surface courante.

Ces règles sont portées dans `zoning.md §S7`.

**Faiblesse 3 — Accès au profil dépendant du seul tableau de bord**

L'accès au profil utilisateur passe actuellement uniquement par le tableau de bord. Or le MJ en mode capture-first (AR-15) peut ne jamais passer par le tableau de bord en début de session. La résolution retenue : promouvoir l'accès compte au rang de composant de châssis, au même titre que la recherche — présent en permanence, quelle que soit la surface active. Porté dans `zoning.md §S7`.

---

## 4. Charge informationnelle

### Verdict global

La sobriété est le principe directeur déclaré (AR-04 — « épuré par défaut »). L'application de ce principe au niveau de chaque écran est globalement bonne. Le risque ne vient pas du bas (les écrans par défaut ne sont pas surchargés) mais **du haut** : certains écrans ont un état de configuration maximale dont la densité n'est pas bornée, et des zones sont présentes parce qu'elles ont été spécifiées, pas parce qu'elles sont utiles à l'instant t.

### Principe directeur retenu

**Plancher garanti, plafond borné, le reste se mérite.**

Pour chaque écran, ce principe se décline en trois niveaux :

- **(a) Plancher garanti** — un petit ensemble de zones jamais masquées, présentes quelle que soit la configuration : l'utilisateur s'y repère toujours.
- **(b) Plafond borné** — au-delà d'un seuil de densité, toute zone supplémentaire se replie ou cède selon une priorité déclarée. Ce plafond doit être posé *avant* de wireframer, pas découvert en le dessinant.
- **(c) Divulgation progressive** — tout le reste (zones hors-MVP, zones sous-spécifiées, configurations avancées) n'apparaît pas au premier plan. Il est accessible à qui le cherche, sans encombrer le cas nominal.

### 3 écrans à alléger en priorité

**Vue session MJ en mode LIVE** — La règle d'éviction des panneaux (quelle zone cède en premier quand l'espace manque ?) est absente du corpus. Elle doit être posée avant de wireframer cet écran. C'est l'écran le plus dense du produit et le support de validation de l'hypothèse H2 — c'est donc le plus urgent. Priorité : H2, manquant.

**Paramètres de campagne** — L'export d'espace et l'archivage sont actuellement au même niveau que les réglages courants. L'export passe au MVP (décision §7) mais doit sortir du premier plan de l'écran (section dédiée, non proéminente). L'archivage se replie : action rare, pas au premier plan.

**Éditeur de document** — Les liens sortants et les backlinks sont des zones utiles mais consultées ponctuellement. Ils se replient par défaut ; l'utilisateur les ouvre à la demande.

---

## 5. Zones sous-spécifiées — 68 marqueurs inventoriés

### Trois familles

Les 68 marqueurs identifiés dans les fiches wireframe se répartissent en trois familles, selon leur nature et le traitement approprié.

**Famille A — À trancher en wireframe (résoluble)** — Points de détail de présentation qui peuvent être résolus par le designer de l'écran, sans arbitrage transversal ni information corpus manquante. Ils ne bloquent pas la production de la fiche.

**Famille B — Arbitrage transversal unique** — Points qui affectent plusieurs fiches simultanément. Les résoudre une fois pour l'ensemble (dans `zoning.md`) élimine d'un coup plusieurs marqueurs dupliqués. Ces arbitrages doivent précéder la production des fiches concernées.

**Famille C — Trou de corpus ou point d'interview (à ne pas forcer)** — Points qui nécessitent soit une information absente du corpus (à compléter hors wireframe), soit un retour utilisateur (interview). Forcer une décision à ce stade créerait un arbitrage non fondé. Ces marqueurs sont nommés, pas résolus.

### 3 résolutions les plus structurantes

**Résolution 1 — Interface de l'éditeur de document** (10 marqueurs, Famille A/B)

Les marqueurs portant sur les types de blocs, la hiérarchie des blocs et les liens sortants dans l'éditeur de document représentent le groupe le plus dense d'un même écran. Ils sont hérités par l'éditeur de scénario (cas spécialisé du même composant). Résoudre la fiche `editeur-document` en débloque deux : `editeur-document` et `editeur-scenario`.

**Résolution 2 — Configuration de la vue session** (Famille B, support H2)

Les marqueurs de configuration de la vue session sont dispersés sur plusieurs fiches (vue session MJ, paramètres de campagne, surface joueur). Un arbitrage transversal unique dans `zoning.md` (cf. §3, faiblesse 1) les résout en bloc. Priorité maximale — support direct de l'hypothèse H2.

**Résolution 3 — Lien d'invitation** (AR-10, Famille B, 5+ marqueurs dupliqués sur 3 fiches)

Le lien d'invitation (AR-10) génère des marqueurs dupliqués sur trois fiches distinctes (vue session MJ, paramètres de campagne, fiche membres). Ces marqueurs se réduisent à un seul arbitrage transversal : séparer le lien par nature d'usage.

- **Lien ponctuel** (valable pour une session donnée) → présent dans la surface `vue-session-mj`.
- **Lien permanent** (valable pour toute la campagne) → présent dans `parametres-campagne` et dans la section membres.

Cet arbitrage est porté dans `zoning.md`.

### Famille C — Nommée, non forcée

Les points suivants sont explicitement laissés à l'interview ou au corpus à venir. Les décisions ne seront pas prises sur la base d'une hypothèse non vérifiée.

- **Notification active côté joueur** (AR-06) — le modèle d'apparition ambiante est décidé, mais la question d'une notification active explicite (push, badge, signal sonore) reste ouverte.
- **Persistance des notes invité** — comportement des notes créées par un utilisateur non connecté lors d'un passage au mode compte : merge, abandon, demande explicite ?
- **Sémantique de l'état `ARCHIVED` pour l'espace personnel** — que signifie « archiver » un espace personnel ? L'opération est-elle réversible ? Qui peut la déclencher ?
- **Fournisseurs d'identité externes** — la connexion via fournisseur externe est dans le corpus (écran Connexion) mais les fournisseurs supportés ne sont pas nommés.
- **Libellé définitif de l'espace personnel** — confirmé à « Espace personnel » dans cette réflexion (§2), mais soumis à validation interview UC-02 §interview.

---

## 6. Adéquation aux personas

### Bilan par persona

**Émilie (MJ improvisatrice)** — pleinement servie. L'écran épuré par défaut (AR-04) et le capture-first (AR-15) sont calibrés pour son usage : elle entre, elle joue, elle ne configure pas. Aucun écran ne l'oblige à passer par une configuration pour accéder à sa session.

**Nadia (MJ occasionnelle)** — pleinement servie. Le mode local (RB-01 à RB-14) et l'espace personnel capture-first répondent exactement à son besoin de premier contact sans friction. L'invite contextuelle (AR-13) lui propose le passage au compte au bon moment, sans l'y forcer.

**Lucas (joueur)** — servi sur le périmètre MVP. La surface joueur est étanche (AR-03), accessible par lien direct, sans friction d'inscription. Le périmètre de sa vue est la fraction Must de UC-09/UC-12 (AR-10). L'enrichissement de sa vue au niveau campagne est différé post-MVP, assumé.

**Thomas (MJ organisé)** — partiellement servi, avec décision. Thomas valorise l'organisation et la portabilité. La structure de dossiers libres (décision §7) répond à son besoin d'organisation. En revanche, l'export d'espace était hors-MVP (Should Have), ce qui créait un risque d'adoption : Thomas ne confie pas son travail à un outil dont il ne peut pas extraire les données. L'export passe au MVP en version minimale (décision §7) — ce point est directement motivé par ce persona.

**Antoine (MJ multi-campagnes)** — servi structurellement, avec réserve post-MVP assumée. La structure multi-espaces du tableau de bord (déterminant (d), §S2 du zoning) répond à son besoin de navigation entre campagnes. La liberté de renommer et supprimer les dossiers (décision §7) répond à sa première question de persona. La réutilisation inter-campagnes (UC-13) reste hors-MVP, assumée : l'espace personnel est déjà identifié comme le futur foyer de « Mes scénarios » (AR-08), ce qui préserve la trajectoire sans l'implémenter.

**Sonia (MJ one-shot)** — servie en préparation. La friction de création de campagne est atténuée par le mode capture-first et l'espace personnel, qui lui permettent de préparer sans créer une campagne. Le parcours one-shot via campagne (parcours-07) couvre son besoin de session partagée.

**Rémi (joueur distant, coordinateur)** — hors cible du MVP, assumé. Son profil (coordination de groupe, gestion de plannings, outils tiers) dépasse le périmètre de la première livraison. Son exclusion est documentée dans les personas et non corrigée par une décision d'interface.

### Lecture transversale

Aucun écran n'est surchargé pour un persona donné. La séparation des surfaces (MJ/joueur) et le principe d'épuration par défaut (AR-04) protègent de la surcharge : chaque persona accède à la surface qui lui est destinée, dans un état de densité minimal adapté à son usage courant. Les personas avancés (Thomas, Antoine) trouvent la profondeur dans les zones secondaires, sans l'imposer aux personas simples (Émilie, Nadia, Sonia).

---

## 7. Décisions opérateur actées

Les quatre décisions suivantes ont été prises lors de cette réflexion. Elles font partie du rationale et sont à porter dans les documents amont concernés (voir §8).

### Décision 1 — Espace personnel : mix assumé + micro-copy neutre

L'espace personnel est présenté selon le modèle du mix (§2) : entrée distincte hors-quota au tableau de bord, atterrissage capture-first, conteneur de transit. La micro-copy d'intention retenue est **« Vos notes et contenus, hors campagne »** — neutre vis-à-vis de la double nature (bac de capture / foyer de réutilisation), compatible avec les deux usages sans implémenter le second au MVP.

### Décision 2 — Dossiers système : entièrement libres

Les dossiers proposés par défaut dans l'espace personnel (Personnages, Scénarios, Notes…) sont **entièrement libres** : renommables et supprimables. La structure par défaut est un gabarit de départ, pas une contrainte. L'utilisateur peut l'adopter, la modifier ou la remplacer entièrement.

Un garde-fou est garanti par construction : un réceptacle pour le contenu non classé (« Non classés ») existe toujours. Il peut être renommé, mais il est recréé s'il est supprimé, afin qu'aucun document ne soit orphelin de dossier. Cette décision lève un angle mort du corpus — la liberté des dossiers n'était pas explicitement tranchée — et répond directement à la première question du persona Antoine.

### Décision 3 — Export d'espace : entre au MVP en version minimale

L'export d'un espace passe de Should Have (hors-MVP) à **Must Have, inclus dans la première livraison**, en version minimale. Cette décision est motivée par le critère de confiance et de portabilité du persona Thomas : un outil qui ne permet pas d'extraire ses données ne recueille pas la confiance d'un utilisateur qui y investit du temps. La version minimale de l'export (format et périmètre exacts à définir en fiche) suffit à répondre à ce besoin sans alourdir le scope de manière disproportionnée. Cette décision impacte : le MoSCoW, la fiche `parametres-campagne`, et UC-01 scénario alternatif A4a.

### Décision 4 — Aperçu « vue joueur » côté MJ : post-MVP

La possibilité pour le MJ de prévisualiser ce que voit le joueur (aperçu lecture seule de la surface joueur) est **différée post-MVP**. Au MVP, l'indicateur de partage visible dans la surface session (§S7 du zoning) suffit : le MJ sait ce qui est partagé sans avoir besoin d'une vue miroir. Un aperçu lecture seule intégral est une feature utile mais non bloquante pour la validation de H2.

---

## 8. Renvois — où les résolutions sont portées

Ce document est le rationale. Les résolutions effectives sont consignées dans les documents suivants.

### Arbitrages transversaux → `zoning.md`

Les résolutions ci-dessous sont portées (ou à porter) dans `zoning.md` comme arbitrages AR :

| Sujet | Section cible |
|---|---|
| Configuration vue session : paramètres pointe vers la surface session | S6 (nouvel AR, complémentaire à AR-02 et AR-09) |
| Retour contextuel (tout détail retourne à sa surface appelante, état préservé) | S7 |
| Recherche = composant de châssis (overlay, jamais un écran) | S7 |
| Accès compte = composant de châssis (présent sur toutes les surfaces) | S7 |
| Espace personnel : mix assumé + micro-copy d'intention neutre | S9 (question ouverte résolue) |
| Dossiers entièrement libres + garde-fou « Non classés » | S6 (complémentaire à AR-16) |
| Lien d'invitation : ponctuel (vue session) / permanent (paramètres/membres) | S6 (complémentaire à AR-10) |
| Principe de densité : plancher garanti, plafond borné, divulgation progressive | S7 |

### Fiches wireframe concernées

| Fiche | Motif de mise à jour |
|---|---|
| `editeur-document` | Résolution Famille A/B (types de blocs, liens, backlinks — §5) ; repli des backlinks par défaut (§4) |
| `editeur-scenario` | Hérite de `editeur-document` (même composant de base) |
| `parametres-campagne` | Export passe au MVP (décision §7.3) ; archivage en repli ; config session pointe vers la surface session |
| `tableau-de-bord` | Accès compte promu composant de châssis ; indicateur espace personnel hors-quota |
| `gate-migration` | Cohérence avec le modèle d'espace personnel libre (dossiers libres, §7.2) |
| `vue-espace-personnel` | Micro-copy d'intention neutre (§2) ; modèle capture-first confirmé |
| `vue-session-mj` | Règle d'éviction des panneaux à poser (§4) ; lien d'invitation ponctuel (§5) ; config pointe vers surface session |
| `navigation-dossiers` | Dossiers entièrement libres + garde-fou « Non classés » (§7.2) |
| `panneau-creation-rapide` | Cohérence avec espace personnel capture-first et modèle de dossiers libres |

### Document vision et MoSCoW

| Sujet | Document cible |
|---|---|
| Export d'espace : passage de Should Have à Must Have | `vision/moscow.md` |

Les résolutions de cette passe sont en cours d'application au corpus dans la même vague de travail.
