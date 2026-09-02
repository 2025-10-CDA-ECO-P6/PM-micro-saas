# Cadrage de validation juridique pré-lancement EU

> **CADRE À VALIDER JURISTE — checklist de consolidation, aucune qualification légale tranchée.**
> Ce document ne tranche **aucun** des points qu'il liste. Il consolide, en un point d'entrée unique, des points de conformité RGPD dispersés dans plusieurs ADR de conception (ADR-012, ADR-013, ADR-018, ADR-015), afin qu'une revue juriste puisse les traiter comme un lot cohérent avant tout lancement EU — plutôt que de les découvrir dispersément au fil des ADR. Chaque axe ci-dessous est un **énoncé de question**, assorti de sa source de corpus et d'un statut unique : **à valider juriste**. Aucune formulation de ce document ne préjuge de l'issue d'une qualification légale.

---

## Pourquoi ce document existe

Trois ADR de conception (ADR-012, ADR-013, ADR-018) portent chacun, indépendamment, une section *Conformité conçue, non certifiée* qui flague des points de validation juridique avant lancement EU. Ces trois sections se recoupent partiellement (le point Art. 8 apparaît dans ADR-012 et ADR-013 ; le point Art. 28/DPA apparaît dans les trois) et se complètent partiellement (ADR-018 étend le périmètre DPA à l'espace `PERSONAL`, un point que ADR-012 et ADR-013 ne couvrent pas). Un quatrième point, sans lien de filiation directe avec ces trois ADR, a été routé séparément depuis les travaux d'identité et d'accès joueur (ADR-015 §2.3, volet sécurité/authentification) vers le cadrage juridique.

Ce cadrage consolide ces points épars en **une liste unique**, énumérée une fois, sourcée vers chaque ADR d'origine. Il ne remplace aucun ADR : en cas de désaccord de lecture entre ce document et un ADR source, l'ADR source fait foi (ce document est un index de consolidation, pas une nouvelle autorité de corpus).

**Portée** : ce document liste les axes à valider. Il ne contient ni délai, ni procédure de saisine du juriste, ni budget — ces éléments relèvent de la roadmap juridique (jalon distinct, cf. renvoi en fin de document).

---

## Vue d'ensemble — les 5 axes

| # | Axe | Corpus source | Statut |
|---|-----|----------------|--------|
| 1 | Art. 8 — qualification « service destiné aux mineurs » | ADR-012 (Conformité conçue, non certifiée, pt 1) · ADR-013 (Conformité conçue, non certifiée, pt 1) | à valider juriste |
| 2 | Art. 28 + périmètre DPA (contenu `PERSONAL` décrivant des tiers inclus) | ADR-012 (pt 2) · ADR-013 §5 + (pt 2) · ADR-018 (Conformité conçue, non certifiée, pt 2, l.237) | à valider juriste |
| 3 | Mise en balance de l'intérêt légitime (§3(c) + `display_name` invité) | ADR-012 (pt 3, §3(c)) · ADR-013 (pt 3) | à valider juriste |
| 4 | Art. 17 — hard-delete inconditionnel de l'espace `PERSONAL` | ADR-012 (pt 4) · ADR-018 (pt 1, l.233-237 et complément l.252) | à valider juriste |
| 5 | Facette RGPD — Option A « reclaim-in-place » (coquille non vérifiée évincée) | ADR-015 §2.3 — routé depuis les travaux d'identité et d'accès joueur | à valider juriste |

Les 5 axes sont chacun **non vérifiables en CI** : aucun ne peut être vérifié par un test automatisé, un linter ou une build Haversack — la dépendance est externe (avis juriste), au même titre que les dépendances **NON VÉRIFIABLE IN BUILD** déjà nommées dans ADR-015/016/017 pour la confiance aux fournisseurs d'identité. Voir section *Non vérifiable en CI* en fin de document.

---

## Axe 1 — Art. 8 (mineurs)

**Énoncé** : le service Haversack est-il, au sens du RGPD et des droits nationaux applicables, un service « destiné aux enfants » ou « accessible aux mineurs » ? Si la réponse est positive, la case d'attestation d'âge (16+) actuellement retenue au formulaire d'inscription (ADR-012) et au formulaire d'accès invité (ADR-013) est-elle suffisante, ou un mécanisme de vérification de l'âge / de consentement parental est-il requis ?

**Corpus source** :
- ADR-012, section *Conformité conçue, non certifiée*, point 1 : qualification Art. 8 pour le compte utilisateur (formulaire d'inscription).
- ADR-013, section *Conformité conçue, non certifiée*, point 1 : qualification Art. 8 pour l'accès invité (formulaire d'accès invité).

**Ce qui est déjà acté en conception (non tranché juridiquement)** : une case d'attestation d'âge 16+ est retenue comme mesure MVP, dans les deux parcours (compte utilisateur et accès invité). Aucun mécanisme de vérification d'âge ni de recueil de consentement parental n'est conçu à ce stade.

**Statut** : à valider juriste.

---

## Axe 2 — Art. 28 + périmètre DPA

**Énoncé** : Haversack agit-il, pour les contenus créés par les Maîtres du Jeu (MJ) décrivant des tiers identifiables (personnages inspirés de joueurs réels, notes sur un joueur), en qualité de sous-traitant (Art. 28 RGPD) ou de responsable de traitement ? Si la qualification sous-traitant est confirmée, le périmètre du DPA (Data Processing Agreement) couvre-t-il uniquement le contenu des espaces partagés (`CAMPAIGN`, `ONE_SHOT`), ou s'étend-il également au **contenu de l'espace `PERSONAL`** lorsque celui-ci décrit un tiers identifiable (PNJ inspiré d'une personne réelle, note personnelle sur un joueur) ?

**Corpus source** :
- ADR-013 §5 (*Contenu créé par le MJ — posture sous-traitant, Art. 28 RGPD*) : pose la posture retenue en conception (sous-traitant), explicitement non tranchée juridiquement.
- ADR-013, section *Conformité conçue, non certifiée*, point 2.
- ADR-012, section *Conformité conçue, non certifiée*, point 2.
- ADR-018, section *Conformité conçue, non certifiée*, point 2 (l.237) : étend explicitement la question au périmètre `PERSONAL` — c'est l'apport propre d'ADR-018 par rapport à ADR-012/013, qui ne couvrent que le périmètre partagé.

**Ce qui est déjà acté en conception (non tranché juridiquement)** : la posture sous-traitant est retenue par défaut pour le contenu MJ en espace partagé décrivant des tiers identifiables. ADR-013 pose explicitement que « le DPA est un document contractuel, non un ADR technique » et que son contenu relève de l'équipe juridique.

**Renvoi** : le squelette de sections destiné à guider la rédaction juriste existe déjà — [`dpa-skeleton.md`](./dpa-skeleton.md). Il ne tranche aucune clause ; il signale lui-même, section 1 et section 3, le même point ouvert (extension au périmètre `PERSONAL`) que le présent axe consolide.

**Statut** : à valider juriste.

---

## Axe 3 — Mise en balance de l'intérêt légitime

**Énoncé** : deux mises en balance formelles de l'intérêt légitime (nécessité, proportionnalité, attentes raisonnables de la personne concernée), requises par l'Art. 6§1(f) et l'Art. 17§3(e), n'ont pas été conduites par un juriste :
1. la conservation, après suppression du compte de son auteur, des documents partagés d'un espace vivant au titre de l'intérêt légitime de continuité pour les membres tiers (§3(c) de la politique de sélection ADR-012) ;
2. la base légale retenue (intérêt légitime, Art. 6§1(f)) pour le traitement du `display_name` du joueur invité (`GuestAccess`).

**Corpus source** :
- ADR-012, section *Conformité conçue, non certifiée*, point 3 (renvoi à §3(c) du même ADR — politique de sélection hard-delete/conservation).
- ADR-013, section *Conformité conçue, non certifiée*, point 3 (base légale `display_name` invité).

**Ce qui est déjà acté en conception (non tranché juridiquement)** : la conservation sous intérêt légitime (§3(c)) est présentée comme reposant sur un motif démontrable (préserver l'expérience de jeu des tiers d'un espace vivant — personnages orphelins, scénarios incomplets si les contributions étaient supprimées). La base légale intérêt légitime du `display_name` invité est retenue par défaut, sans test de mise en balance formalisé.

**Statut** : à valider juriste.

---

## Axe 4 — Art. 17 : hard-delete inconditionnel de l'espace `PERSONAL`

**Énoncé** : la thèse retenue en conception — hard-delete inconditionnel du contenu de l'espace `PERSONAL` à `UserDeleted`, sans possibilité de conservation sous intérêt légitime tiers (puisqu'un espace `PERSONAL` n'a par construction aucun tiers) — est-elle une qualification Art. 17 suffisante ? Deux points subsidiaires restent ouverts :
- (a) l'instant de référence `deletion_requested_at` (retenu pour le contenu partagé, §3(b) d'ADR-012) s'applique-t-il au contenu personnel avec la même rigueur juridique ?
- (b) un document déplacé d'un espace personnel vers un espace partagé (ou réciproquement) après `deletion_requested_at` — la sélection est-elle figée à la date de la demande, ou suivie dynamiquement jusqu'à l'exécution de la saga ? Le mécanisme technique retenu (jeu de sélection matérialisé + step de relocation obligatoire, ADR-012 §7) fige la sélection dans les deux sens à `deletion_requested_at` — sa **suffisance** au regard de l'Art. 17§3 n'est pas tranchée.

**Corpus source** :
- ADR-012, section *Conformité conçue, non certifiée*, point 4.
- ADR-018, section *Conformité conçue, non certifiée*, point 1 (l.233-237) : qualification du contenu « personnel de premier ordre » au regard de l'Art. 17, formulée du point de vue de la généralisation `Space`.
- ADR-018, *Compléments post-revue* (l.252) : la purge inconditionnelle de l'espace `PERSONAL` à `UserDeleted` est actée comme décision de conception (réversible, à confirmer à l'entrée en build) — « sous le même invariant de claim/reprise que la purge J+30 (garantie Art. 17, ADR-011) ». Cette décision de conception ne se substitue pas à la validation juridique de l'axe 4 : elle en est l'objet.

**Ce qui est déjà acté en conception (non tranché juridiquement)** : le hard-delete inconditionnel est présenté comme une qualification « solide en principe » (absence de tiers dans un espace personnel = absence d'intérêt légitime de continuité Art. 17§3(e)), mais explicitement non confirmée par un juriste, avec deux points subsidiaires ouverts sur l'instant de référence.

**Statut** : à valider juriste.

---

## Axe 5 — Facette RGPD de l'Option A « reclaim-in-place » *(routé depuis les travaux d'identité et d'accès joueur)*

**Énoncé** : lorsqu'une preuve OAuth (email réputé vérifié par le fournisseur d'identité) reprend une **coquille de compte non vérifiée préexistante** portant la même adresse email (Option A « reclaim-in-place » — bascule `emailVerified`, neutralisation du credential préexistant, liaison fédérée), que devient le **contenu éventuel** (notes, documents) déjà rattaché à cette coquille au moment de la reprise ? Ce point est une facette RGPD distincte de la ratification sécurité de l'Option A elle-même (anti-hijacking, CWE-287, invariant 1 email-unique) — celle-ci n'est pas rouverte ici.

**Corpus source** :
- ADR-015 §2.3 (*Résolution — email OAuth face à un compte préexistant NON vérifié*) : pose l'Option A, explicite le découpage du routage (facette RGPD hors périmètre sécurité de l'ADR).
- ADR-015 § Dettes nommées (non silencieuses) : recense la facette RGPD du reclaim-in-place (sort du contenu éventuel de la coquille non vérifiée évincée) comme renvoyée au cadrage juridique interne, distincte de l'opération de domaine dédiée renvoyée à B1.5.
- ADR-015 § Points à trancher (*[À TRANCHER — PRODUIT]*) : résume la résolution proposée et le double routage (facette RGPD → cadrage juridique interne ; opération de domaine dédiée → build B1.5).

**Dépendance de build** : ce point ne peut être qualifié indépendamment de l'opération de domaine dédiée `ReclaimViaFederatedProof()` (ou équivalent), qui reste à spécifier au build **B1.5** — nom, signature, invariants précis non encore fixés en conception. La validation juridique de la facette RGPD peut porter sur le principe (neutralisation d'un contenu rattaché à une coquille non prouvée) indépendamment du détail d'implémentation de B1.5, mais la mise en œuvre technique du traitement retenu (conservation, suppression, ou anonymisation du contenu de la coquille évincée) dépend de cette opération.

**Ce qui est déjà acté en conception (non tranché juridiquement)** : l'Option A elle-même (reprise de la coquille par preuve IdP plutôt que création d'un doublon) est motivée par l'absence de propriétaire prouvé d'une coquille non vérifiée — mais le sort du contenu qui y serait déjà rattaché (RGPD, pas sécurité) n'est pas qualifié.

**Statut** : à valider juriste.

---

## Non vérifiable en CI

Les 5 axes ci-dessus partagent une propriété : aucun n'est vérifiable par la CI Haversack, quelle que soit la maturité du build. Il ne s'agit pas d'un cas de test manquant ou d'une dette technique — c'est une dépendance structurelle à un avis externe (juriste), de même nature que les dépendances déjà nommées **NON VÉRIFIABLE IN BUILD** dans ADR-015 (§2.2, fiabilité du claim d'email vérifié d'un fournisseur OAuth), ADR-016 et ADR-017 (maillons de confiance externes à la migration/sérialisation locale). Dans ces trois ADR, le gate applicatif qui *encadre* la dépendance externe reste testable en CI (via un provider mocké) — mais la dépendance elle-même ne l'est pas. Le parallèle ici est le même : un mécanisme technique peut encadrer chacun des 5 axes (attestation d'âge, DPA signé, test de mise en balance documenté, invariant de figement `deletion_requested_at`, opération `ReclaimViaFederatedProof`), mais la **qualification légale** sous-jacente à chaque axe reste, par nature, non vérifiable en CI.

Ce cadrage constitue, au même titre que les autres préalables non réalisables dans un repo documentaire (confirmation des 8 ADR pré-implémentation, ordre de build C#-first, décision marché, etc.), un jalon de suivi porté hors de ce repo. Ce document en est le contenu détaillé — c'est ici que les 5 axes sont énoncés et sourcés, un renvoi de suivi externe ne ferait que les recopier.

---

## Renvois

- `docs/architecture/decisions/ADR-012-rgpd-effacement-compte.md` — section *Conformité conçue, non certifiée*.
- `docs/architecture/decisions/ADR-013-rgpd-donnees-invites.md` — §5 et section *Conformité conçue, non certifiée*.
- `docs/architecture/decisions/ADR-018-espace-personnel-generalisation-space.md` — section *Conformité conçue, non certifiée* et *Compléments post-revue*.
- `docs/architecture/decisions/ADR-015-securite-authentification-mvp.md` — §2.3.
- [`docs/securite/conformite/dpa-skeleton.md`](./dpa-skeleton.md) — squelette de sections DPA (axe 2).

---

## Ce que ce document ne fait pas

- Il ne tranche aucune qualification légale (Art. 8, Art. 28, Art. 17, intérêt légitime) : chaque axe reste un énoncé de question.
- Il ne fixe ni délai ni procédure de saisine juriste — ces éléments relèvent de la roadmap juridique.
- Il ne rédige aucune clause contractuelle (le DPA reste un document distinct, `dpa-skeleton.md`, lui-même non contractuel).
- Il ne modifie ni ne réinterprète les ADR sources : en cas de désaccord de lecture, l'ADR d'origine fait foi.
