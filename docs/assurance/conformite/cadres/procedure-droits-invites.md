# Procédure d'exercice des droits des invités & procédure support Art. 12§3

> ## ⚠ CADRE À VALIDER JURISTE
>
> Ce document décrit un **cadre procédural** destiné à l'équipe support pour instruire une demande d'exercice de droits RGPD émanant d'un joueur invité (sans compte), ainsi que le traitement générique du délai Art. 12§3. Il **ne tranche aucune qualification légale** : les cas d'invocation de l'extension de délai, la méthode exacte de vérification d'identité, et toute base légale sous-jacente restent marqués « à valider juriste » ou « à valider juriste/sécurité » dans le corps du document. Aucune section de ce cadre ne doit être opérée en production avant validation juridique explicite.

- **Statut** : Cadre proposé — non validé juridiquement
- **Date** : 2026-07-15
- **Portée** : Invités (`GuestAccess`, sans compte) exerçant un droit RGPD auprès du support ; procédure support Art. 12§3 applicable transversalement (y compris le cas d'un compte déjà anonymisé, §5)
- **Renvois normatifs** : ADR-013 (droits invités), ADR-012 (effacement de compte / Art. 12§3), ADR-007 (base du modèle RGPD)

---

## Périmètre de cette procédure

**Couvert par ce document** :
- Le canal de saisine d'une demande de droits par un invité sans compte, et les données minimales que l'invité doit fournir pour que le support puisse instruire sa demande.
- Le principe de vérification d'identité proportionnée avant toute action support (divulgation, rectification, effacement).
- Le déroulé procédural (réception → vérification → traitement → réponse → clôture) et le délai Art. 12§3 associé.
- Le cas où le sujet de la demande est un compte déjà anonymisé (canal de notification alternatif, renvoi ADR-012 §8).

**Hors périmètre de ce document** (voir aussi *Hors-scope* en fin de document) :
- Toute qualification de la base légale sous-jacente (base légale du `display_name`, qualification sous-traitant/responsable, qualification Art. 8 mineurs) — déjà traitées, sous réserve de validation juridique, dans ADR-013 et ADR-012.
- Les cas précis d'invocation de l'extension du délai Art. 12§3 (voir §4).
- La méthode exacte de vérification d'identité (voir §2).
- Toute clause contractuelle (CGU, DPA) — matière contractuelle distincte d'une procédure opérationnelle.
- Tout autre fichier que le présent document.

---

## 1. Droits couverts (Art. 15 à 21 RGPD)

Un invité identifié uniquement par son `display_name` et l'espace auquel il a participé peut exercer les droits suivants, par email à l'adresse support dédiée :

**Canal de saisine** : `support@haversack.io` (placeholder ops — adresse à confirmer au provisionnement, cf. ADR-013 §2). L'invité mentionne dans sa demande :
- son `display_name` tel qu'utilisé en session ;
- l'espace concerné (nom de la campagne / du one-shot, ou tout identifiant permettant de le retrouver côté support) ;
- le droit qu'il souhaite exercer et, le cas échéant, la donnée visée.

| Droit (article RGPD) | Contenu | Application au invité `GuestAccess` |
|---|---|---|
| Accès (Art. 15) | Obtenir confirmation du traitement et copie des données | Le support communique les données associées au `GuestAccess` retrouvé (display_name, character_id le cas échéant, métadonnées de rétention) après vérification d'identité (§2) |
| Rectification (Art. 16) | Corriger une donnée inexacte | Applicable au `display_name` et aux données associées au `GuestAccess` encore actif |
| Effacement (Art. 17) | Suppression sur demande | Applicable au `GuestAccess` et aux données qui lui sont propres ; le sort des contenus produits en espace partagé (notes, contributions) suit les règles de partage déjà actées (ADR-013 §2/§3) — ce document ne rouvre pas cette politique |
| Opposition (Art. 21) | S'opposer à un traitement fondé sur l'intérêt légitime | Applicable dès lors que le traitement du `display_name` et des métadonnées techniques repose sur l'intérêt légitime (Art. 6§1(f), ADR-013 §1) |

**Hors périmètre de ce tableau** : la portabilité (Art. 20) et la limitation du traitement (Art. 18) ne sont pas instruites par cette procédure — ADR-012 situe la portabilité en post-MVP ; leur applicabilité au invité sans compte n'a pas été qualifiée et reste **à valider juriste** si une demande de ce type survient.

> **À valider juriste** : la portée exacte de l'effacement (Art. 17) et de l'opposition (Art. 21) lorsque la donnée de l'invité est imbriquée dans un contenu partagé conservé sous intérêt légitime tiers — ce document renvoie à la politique déjà actée (ADR-013) sans la retrancher.

---

## 2. Vérification d'identité proportionnée avant toute action

**Constat de sécurité** : un invité n'est identifié que par un `display_name` déclaratif et l'espace auquel il a participé — aucun compte, aucun identifiant stable, aucun secret partagé n'est associé à cette identité au moment de la saisine. Un tiers connaissant le `display_name` et le nom de la campagne (information parfois visible d'autres membres, ou devinable) pourrait se faire passer pour l'invité auprès du support.

**Principe retenu — proportionnalité** : aucune action support (communication de données, rectification, effacement) n'est exécutée **avant vérification que le demandeur est bien la personne correspondant au `GuestAccess` visé**. Le niveau de rigueur de cette vérification est **proportionné** :
- à la **sensibilité de l'action demandée** — une communication de données personnelles ou un effacement irréversible appelle un niveau de vérification plus élevé qu'une simple confirmation que le traitement existe ;
- au **risque de préjudice en cas d'erreur** — divulguer les données d'un invité à un usurpateur, ou effacer les données d'un invité sur la base d'une demande frauduleuse, sont des scénarios que la vérification doit prévenir en priorité ;
- à ce qui est **raisonnablement disponible** au support pour corroborer l'identité, compte tenu du fait que l'invité n'a ni compte ni mot de passe.

**Ce que ce document ne fait pas** : il ne fixe pas la méthode exacte de vérification (question de sécurité opérationnelle et de qualification juridique du niveau de preuve suffisant au sens de l'Art. 12§6 RGPD). Des pistes de corroboration sont envisageables (ex. confirmation croisée par un membre habilité de l'espace, éléments contextuels connus du seul participant réel, fenêtre temporelle de participation) mais **aucune n'est retenue à ce stade** — le choix de la ou des méthodes doit être arbitré avec la sécurité et validé juridiquement avant mise en œuvre.

> **À valider juriste/sécurité** : la ou les méthodes exactes de vérification d'identité proportionnée, leur niveau de preuve jugé suffisant au regard de l'Art. 12§6 RGPD, et le traitement du cas où l'identité ne peut pas être corroborée (refus motivé de traiter la demande, ou traitement en doute favorable au demandeur — non tranché ici).

**Conséquence procédurale immédiate** : tant que la méthode n'est pas validée, aucune demande d'invité ne doit être traitée sur la seule base du `display_name` et du nom d'espace déclarés dans l'email de saisine — ces deux éléments servent à **retrouver** le `GuestAccess` concerné, pas à **authentifier** le demandeur.

---

## 3. Déroulé procédural

Le traitement d'une demande suit les étapes suivantes. Chaque étape est bloquante pour la suivante.

1. **Réception** — la demande arrive à `support@haversack.io`. Le support horodate la réception (`date de réception`) : c'est le point de départ du délai Art. 12§3 (§4).
2. **Identification du `GuestAccess`** — le support recherche le `GuestAccess` correspondant au `display_name` et à l'espace mentionnés. Si plusieurs `GuestAccess` correspondent (même `display_name` réutilisé sur plusieurs espaces ou sessions), le support demande une précision complémentaire au demandeur avant de poursuivre.
3. **Vérification d'identité proportionnée** — conformément au §2. Cette étape est **bloquante** : aucune action des étapes 4-5 n'est engagée avant son issue positive.
4. **Traitement de la demande** selon le droit exercé (§1) :
   - **Accès** : extraction des données associées au `GuestAccess` identifié.
   - **Rectification** : correction de la donnée visée si le `GuestAccess` est encore actif.
   - **Effacement** : déclenchement de la suppression selon la politique déjà actée (ADR-013 §2/§3) — ce document ne redéfinit pas cette politique, il en déclenche l'exécution sur demande individuelle plutôt que sur l'expiration automatique.
   - **Opposition** : évaluation de l'opposition au regard de la base légale intérêt légitime (ADR-013 §1) ; réponse motivée au demandeur.
5. **Réponse au demandeur** — envoyée à l'adresse email utilisée pour la saisine (cas nominal invité, canal encore joignable — à distinguer du cas §5).
6. **Clôture et journalisation** — la demande est marquée close, avec horodatage de clôture, pour permettre de vérifier a posteriori le respect du délai Art. 12§3.

---

## 4. Délais de traitement (Art. 12§3)

**Délai de principe** : toute demande d'exercice de droit reçue par le canal support est traitée dans un délai d'**1 mois** à compter de la date de réception (étape 1, §3).

**Extension** : ce délai est extensible à **2 mois** si la complexité ou le nombre de demandes le justifie, à condition d'en informer le demandeur **dans le premier mois**, avec indication des motifs du report.

> **À valider juriste** : les cas concrets justifiant l'invocation de l'extension à 2 mois (ex. volume de demandes, difficulté à retrouver le `GuestAccess`, vérification d'identité prolongée) ne sont pas qualifiés ici. Ce document ne fixe qu'un délai de principe et son mécanisme d'extension tels que posés par l'article — pas les critères d'invocation, qui restent à documenter avec le juriste avant mise en opération de la procédure.

**Articulation avec la vérification d'identité (§2)** : le temps consacré à la vérification d'identité s'inscrit dans le délai global — il ne le suspend pas. Une vérification prolongée qui menacerait le respect du délai d'1 mois est un signal potentiel d'invocation de l'extension (à valider juriste, cf. ci-dessus), pas un motif de dépassement silencieux du délai.

---

## 5. Cas particulier — compte utilisateur déjà anonymisé (canal de notification Art. 12§3)

Ce cas ne concerne pas un invité `GuestAccess`, mais relève de la même procédure support Art. 12§3 : un ancien titulaire de compte a demandé l'effacement de son compte (saga `UserAnonymized`, ADR-012), et le support doit notifier l'issue du traitement de sa demande dans le délai Art. 12§3.

**Constat** : une fois l'anonymisation exécutée, l'adresse email d'origine du compte a été réécrite (`deleted-{id}@haversack.invalid`, ADR-012 §1) — le sujet **n'est plus joignable** à cette adresse. Un canal de notification qui reposerait sur un journal consultable (ex. rechercher l'ancienne adresse dans un log d'audit ou d'accès) est **inadapté** : ce n'est pas la fonction d'un tel journal, et la réintroduction d'une corrélation `UserId` ↔ email dans un log persistant est précisément ce que la purge des logs de corrélation (ADR-012 §5) vise à éviter.

**Canal retenu (renvoi ADR-012 §8)** : la destination de notification est capturée, au moment de la demande (`deletion_requested_at`), dans un **enregistrement de notification dédié, à finalité strictement limitée** (*purpose-limited*) — distinct de tout journal consultable :
- il contient uniquement la cible (adresse email au moment de la demande), l'identifiant de la demande d'effacement, et un drapeau d'achèvement du traitement ;
- il repose sur une base légale distincte de la pseudonymisation elle-même — obligation légale (Art. 6§1(c)) au titre du respect de l'Art. 12§3 ;
- il est purgé dès l'envoi réussi de la notification, ou au plus tard à la fin de la fenêtre Art. 12§3 (§4), selon l'événement survenant en premier.

**Ce que le support doit retenir** : si la notification de fin de traitement n'a pas pu être envoyée avant la réécriture de l'email (chemin nominal, ADR-012 §8), le support ne doit **pas** chercher l'adresse d'origine dans un journal d'audit ou d'accès — la destination à utiliser est celle capturée dans l'enregistrement dédié décrit ci-dessus.

**Renvoi** : mécanisme et bases légales détaillés dans ADR-012 §8 (chemin nominal et reprise après incident) ; point encore ouvert au titre des *Points à trancher* d'ADR-012 (qualification juridique de ce canal).

---

## 6. Renvois croisés

| Sujet | Référence |
|---|---|
| Droits invités, base légale du `display_name`, information Art. 13, rétention `GuestAccess` | **ADR-013** |
| Procédure d'effacement de compte, délai Art. 12§3, mécanisme de notification (§8) | **ADR-012** |
| Modèle d'autorisation API et posture RGPD générale | **ADR-007** |

---

## Points à valider (juriste / sécurité) — récapitulatif

| # | Point | Section | Nature de la validation |
|---|---|---|---|
| 1 | Portée de l'effacement/opposition sur un contenu partagé imbriquant la donnée de l'invité | §1 | Juriste |
| 2 | Applicabilité de la portabilité (Art. 20) et de la limitation (Art. 18) à un invité sans compte | §1 | Juriste |
| 3 | Méthode(s) exacte(s) de vérification d'identité proportionnée et niveau de preuve suffisant (Art. 12§6) | §2 | Juriste + Sécurité |
| 4 | Traitement du cas où l'identité ne peut pas être corroborée | §2 | Juriste + Sécurité |
| 5 | Critères concrets d'invocation de l'extension de délai à 2 mois | §4 | Juriste |
| 6 | Qualification juridique du canal de notification pour compte anonymisé | §5 (renvoi ADR-012 §8) | Juriste |

Aucune action opérationnelle sur ces six points ne doit être prise sur la seule base de ce document.

---

## Hors-scope de cette procédure

- Aucune qualification légale n'est tranchée ici (base légale, cas d'invocation de l'extension de délai, niveau de preuve suffisant) — chaque point est marqué « à valider juriste » ou « à valider juriste/sécurité » ci-dessus.
- Ce document ne modifie ni ne redéfinit la politique de sélection des données déjà actée dans ADR-013 (§2/§3) ou ADR-012 (§2/§3) — il en décrit l'exécution sur demande individuelle.
- Aucune clause contractuelle (CGU, DPA) n'est rédigée ici — matière contractuelle distincte, hors périmètre d'une procédure opérationnelle.
- Aucun autre fichier n'est produit ou modifié par cette tâche.
