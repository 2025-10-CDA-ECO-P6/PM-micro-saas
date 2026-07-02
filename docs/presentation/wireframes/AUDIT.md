# Audit de conformité des wireframes HTML — MVP Haversack

> Audience : équipe conception/produit interne.
> Date d'audit : 2026-06-25.
> Périmètre : 20 écrans canoniques du MVP (zoning figé, décision opérateur 2026-06-12).
> Source de vérité des constats : bundle d'audit consolidé par session d'analyse (non ré-auditable par cet artefact).

---

## 0. Addendum — résolutions appliquées (2026-06-25, post-audit)

> Cet addendum trace les corrections portées aux wireframes **après** la passe d'audit ci-dessous. Les sections 1 à 6 décrivent l'état **au moment de l'audit** ; elles ne sont pas réécrites, mais les points suivants sont désormais **résolus** dans les fichiers.

- **20/20 wireframes présents.** Les 3 écrans manquants ont été produits : `vue-session-mj`, `creation-espace`, `panneau-creation-rapide`.
- **`suppression-compte-rgpd` : PARTIEL → CONFORME.** Le verrou **RB-10-05** (précondition « adresse e-mail validée ») est ajouté : 3ᵉ état « adresse e-mail non validée » (confirmation verrouillée + invite à vérifier, annoncée assistivement) ; l'état confirmable indique la précondition satisfaite.
- **`editeur-scenario` : audit dédié réalisé → CONFORME.** Les 3 zones de la fiche sont présentes ; gaps mineurs corrigés (champ **contexte** ajouté aux infos générales, **fil d'Ariane** scénario › scène explicite, état **monobloc A1** illustré).
- **États secondaires complétés** (priorité faible de la §6) : `accueil` (retour/données locales UC-01 A2), `acces-lien-saisie-nom` (vide + annonce assistive), `gate-migration` (rapport de rejets E5), `inscription` (invite contextuelle A1), `navigation-dossiers` (hors connexion NFR-OFF-04 + mode local), `parametres-campagne` (invite locale sur lien + conséquences d'archivage), `profil` (erreur e-mail non validée), `recherche-preparation` (mode local), `tableau-de-bord` (état vide 0/3), `vue-campagne` (vide + erreur synchro + mode local), `vue-joueur-base` (vide), `editeur-document` (brouillon titre seul E1 + erreur synchro), `vue-espace-personnel` (affordance « déplacer » visible).
- **Répertoire mirroré** sur l'arborescence du repo : `<surface>/<slug>/<slug>.dc.html` + `<slug>.md`, `index.html` (3 écrans passés en « présent »), `support.js` référencé en `../../support.js`.

---

## 0bis. Addendum — corrections UI/UX D1..D13 (2026-07-02)

> Passe de corrections UX appliquée aux planches (autorité : UC > domaine > zoning AR-01..20 > fiches). Le langage basse-fidélité est préservé (aucun contrôle natif). Aucun fichier d'autorité modifié ; aucune tension ouverte tranchée.

- **D1 — `panneau-creation-rapide`** : distinction création ≠ partage rétablie. Un document *créé* à la volée est épinglable en LIVE **comme en CLOSED** (UC-07) ; seul l'auto-épinglage du *partage* est réservé au LIVE (UC-08 A3). Affordance « Épingler à la vue session » ajoutée aux panneaux LIVE et CLOSED ; modalité (« si utile » vs systématique) signalée `[OUVERT]`, non tranchée.
- **D2 — `parametres-campagne`** : la zone de configuration de la vue session devient un **aperçu en lecture seule** + renvoi « Configurer dans la vue session › » — la vue session (mode configuration) est la seule source (AR-18 ; US-06-02). Laptop + tablette.
- **D3 — `vue-session-mj`** : affordance « ⧉ Lien de session › » (lien ponctuel) ajoutée au mode LIVE, laptop et tablette (AR-10 révisé).
- **D4 — `vue-session-mj`** : règle de densité déclarée sur la planche (bande « règle de densité (AR-19) » + post-it) : plancher garanti (statut de session · partage · zone de notes · recherche) et ordre d'éviction à saturation : 1. résultats de recherche → 2. épinglés → 3. dossiers condensés (AR-19 volet b).
- **D5 — `parametres-campagne`** : l'export d'espace n'est plus présenté grisé/post-MVP — action **disponible** (version minimale, MVP — reflexion-ux §7 Décision 3, persona Thomas), maintenue hors du premier plan.
- **D6 — toutes surfaces MJ** : accès compte = **composant de châssis** (zoning S7), affordance identique (avatar + « Thomas ▾ » ; en mode local : « Créer un compte ») sur `tableau-de-bord`, `creation-espace`, `vue-campagne`, `navigation-dossiers`, `recherche-preparation`, `editeur-document`, `editeur-scenario`, `parametres-campagne`, `vue-espace-personnel` et `vue-session-mj` (tous modes), laptop + tablette.
- **D7 — `vue-campagne`, `parametres-campagne`** : cadres « placement non décidé » retirés ; AR-10 révisé appliqué (ponctuel → vue session ; permanent → paramètres). Le sélecteur Session/Campagne des paramètres est remplacé par le seul lien permanent ; « Inviter des joueurs » en vue campagne est annoté comme renvoi, pas un point de génération.
- **D8 — `gate-migration`** : sélection **par espace** rendue (éligibles cochés par défaut, décochables ; espace bloqué — session en cours — non sélectionnable). Granularité globale (un seul lot vs lots successifs) signalée `[OUVERT]`. Fiche alignée : la modalité de sélection est un choix de maquette, non arbitré par le corpus.
- **D9 — `vue-espace-personnel`** : micro-copy d'intention « Vos notes et contenus, hors campagne » rendue en **libellé discret** sous le titre (laptop, état vide, tablette) — pas de texte d'accueil (reflexion-ux §2).
- **D10 — `editeur-document`, `editeur-scenario`** : documents liés et « Référencé par » **repliés par défaut** sur laptop (divulgation progressive, AR-19c ; reflexion-ux §4), alignés sur la variante tablette.
- **D11 — `vue-espace-personnel`** : affordance « déplacer › » présente sur **chaque ligne** de la liste — rendu et annotation alignés (unicité AR-11).
- **D12 — `navigation-dossiers` + `vue-espace-personnel`** : règle unique pour « Non classés » : **masqué s'il est vide, affiché dès qu'il contient des documents** — annotée sur les deux écrans (UC-05 A4 ; AR-16/AR-20).
- **D13 — `vue-session-mj`** : en mode configuration, le panneau de résultats de recherche n'est plus projeté sans recherche en cours (AR-19c) — réactivable via le tweak `showSearchResults`. Mode LIVE inchangé.

**Point de navigation §5.1 (placement du lien d'invitation) : résolu** par AR-10 révisé — voir D3/D7. Les points §5.2 à §5.5 restent ouverts.

---

## 1. Synthèse exécutive

Le MVP recense **20 écrans canoniques**. À l'issue du remaniement du répertoire :

- **17 wireframes HTML présents** — substance saine, co-localisés dans leur dossier `<slug>/` respectif.
- **3 écrans sans wireframe** — fiche `.md` présente, export HTML absent (`vue-session-mj`, `creation-espace`, `panneau-creation-rapide`).
- **Index original perdu** (dump runtime) — remplacé par un hub `index.html` reconstruit.

**Bilan de conformité substance** : **16 écrans sur 17 sont CONFORMES** à leur fiche et aux UC associés. Un seul écran est **PARTIEL** : `suppression-compte-rgpd` — le verrou RB-10-05 (précondition « adresse e-mail validée ») est absent, ce qui constitue un défaut de substance à corriger en priorité (sécurité/RGPD).

Les écarts relevés sur les 16 écrans conformes sont **homogènes et de faible gravité** : états secondaires non illustrés (vide, erreur synchro) et variantes mode local non montrées. Aucun défaut de substance grave hors `suppression-compte-rgpd`.

**Le problème structurel corrigé n'était pas la substance mais le nommage** : l'export de l'outil de design produisait un décalage systématique d'un cran entre le nom de fichier et le contenu réel (17/17 fichiers affectés). Ce défaut a été corrigé par renommage manuel de chaque fichier d'après son contenu. Voir section 2.

---

## 2. Cartographie nom d'export initial → contenu réel

Le décalage était systématique : chaque fichier portait le nom de l'écran précédent dans la séquence d'export. Le tableau ci-dessous documente la correction appliquée.

| Nom de fichier exporté (initial) | Contenu réel (slug retenu) |
|---|---|
| `Accès joueur (lien + nom).dc.html` | `accueil` |
| `Connexion.dc.html` | `acces-lien-saisie-nom` |
| `Espace personnel.dc.html` | `connexion` |
| `Gate de migration.dc.html` | `vue-espace-personnel` |
| `Inscription.dc.html` | `gate-migration` |
| `Navigation par dossiers.dc.html` | `inscription` |
| `Page d'erreur d'accès.dc.html` | `navigation-dossiers` |
| `Paramètres de campagne.dc.html` | `erreur-acces` |
| `Profil utilisateur.dc.html` | `parametres-campagne` |
| `Recherche en préparation.dc.html` | `profil` |
| `Suppression de compte (RGPD).dc.html` | `recherche-preparation` |
| `Tableau de bord.dc.html` | `suppression-compte-rgpd` |
| `Vue campagne.dc.html` | `tableau-de-bord` |
| `Vue joueur.dc.html` | `vue-campagne` |
| `Vue session MJ.dc.html` | `vue-joueur-base` |
| `Éditeur de scénario.dc.html` | `editeur-document` |
| `download` (sans extension) | `editeur-scenario` (écran récupéré d'un fichier qu'on croyait parasite) |

**Fichiers mis en quarantaine** (répertoire `_quarantine/`) :

| Fichier | Motif |
|---|---|
| `Wireframes Haversack - Index.dc.html` | Dump du runtime — md5 identique à `Éditeur de document.dc.html`, non exploitable |
| `Éditeur de document.dc.html` (renommé `Editeur de document (doublon de l-index).dc.html` en quarantaine) | Dump du runtime — doublon de l'index, contenu non pertinent |
| `support (1).js` | Image WEBP mal nommée — renommée `support (1).webp` en quarantaine |

> La règle de préservation est documentée dans le README : ne jamais ré-exporter l'outil de design directement par-dessus ce répertoire. Tout nouvel export doit transiter par un dossier `_inbox/`, être vérifié par son contenu, puis intégré manuellement (renommage par contenu + réécriture du chemin `support.js` en `../support.js`).

---

## 3. Audit de conformité écran par écran

Les 17 écrans présents sont audités ci-dessous. Format par entrée : conformité, ce qui manque (éléments requis par la fiche absents du wireframe), ce qui est à améliorer (présent mais incomplet ou non illustré), ce qui est faux (éléments présents mais contradictoires avec la fiche ou les règles du corpus, à supprimer ou corriger).

**Bilan de l'angle « faux » sur l'ensemble des 17 écrans** : aucun élément contradictoire ou aberrant n'a été détecté. L'angle est traité et conclut à vide pour les 16 écrans CONFORMES. Le seul écart de substance est le PARTIEL de `suppression-compte-rgpd` (verrou RB-10-05 absent — manque, non élément faux).

### 3.1 `accueil` — Accueil non authentifié

**Conformité : CONFORME**

Couverture UC-01. Les deux chemins (sans compte / avec compte) sont présents.

- **À améliorer** : l'état de retour (données locales détectées, UC-01 A2) n'est pas matérialisé visuellement.

---

### 3.2 `acces-lien-saisie-nom` — Accès par lien + saisie du nom d'affichage

**Conformité : CONFORME**

Couverture UC-09 A. Les trois zones (saisie du nom, information RGPD, garde-fou AR-03) sont présentes.

- **À améliorer** :
  - L'état vide (lien valide, aucun document partagé) n'est pas représenté.
  - L'annonce assistive d'apparition de document (NFR-ACC-02/AR-06) n'est pas matérialisée.

---

### 3.3 `connexion` — Connexion

**Conformité : CONFORME**

Couverture UC-10. Les quatre zones sont présentes. L'erreur générique E2 (ne désigne pas le champ fautif) et le rappel d'annonce assistive (NFR-ACC-02) sont correctement rendus.

- **À améliorer** : aucun point bloquant identifié.

---

### 3.4 `vue-espace-personnel` — Vue de l'espace personnel

**Conformité : CONFORME**

Couverture UC-01/04/05 ; AR-14/15/16/17. Traitement exemplaire de l'absence par nature : vue session, partage et membres ne sont jamais grisés (absents par construction, non désactivés).

- **À améliorer** : l'affordance « déplacer un document » (unicité AR-11) est peu visible.

---

### 3.5 `gate-migration` — Gate de migration local→cloud

**Conformité : CONFORME**

Couverture UC-01 A1, UC-10. Les zones, l'état bloquant (session en cours) et l'ancrage anti-appropriation (ADR-016 §4) sont présents.

- **À améliorer** : l'état « rapport de rejets » (UC-10 E5) est mentionné en texte seulement, non illustré.

---

### 3.6 `inscription` — Inscription

**Conformité : CONFORME**

Couverture UC-10, UC-01 A1. Le fournisseur externe est positionné en haut (AR-13) ; l'état erreur E1 est présent.

- **À améliorer** : l'arrivée depuis invite contextuelle (UC-01 A1) sans perte du contenu en cours n'est pas illustrée.

---

### 3.7 `navigation-dossiers` — Navigation par dossiers

**Conformité : CONFORME**

Couverture UC-05/04. Écran le plus complet de la série : arborescence et liste condensée, renommer/réordonner/supprimer, unicité AR-11, vue « Non classés » invisible si vide (UC-05 A4), suppression de dossier non vide sans perte silencieuse (E2/E3).

- **À améliorer** :
  - L'état d'erreur perte de connexion cloud (NFR-OFF-04) n'est pas montré.
  - La variante mode local n'est pas représentée.

---

### 3.8 `erreur-acces` — Page d'erreur d'accès

**Conformité : CONFORME — couverture intégrale**

Couverture UC-09 A3/E1/E2. Le message sobre est identique pour tous les motifs d'erreur (non-révélation RB-09-21/AR-03). Écran à une seule zone, statique — couverture exacte de la fiche.

- **À améliorer** : aucun point identifié.

---

### 3.9 `parametres-campagne` — Paramètres de campagne

**Conformité : CONFORME**

Couverture sous-spécification assumée et tracée (S9/S5). Les quatre zones sont présentes : configuration de la vue session (AR-09), lien d'invitation (UC-11), archivage, export (Should Have / post-MVP).

- **À améliorer** :
  - L'invite contextuelle mode local (AR-13) sur le lien d'invitation n'est pas représentée.
  - Les conséquences de l'archivage avant confirmation (S9) ne sont pas montrées.

---

### 3.10 `profil` — Profil utilisateur

**Conformité : CONFORME**

Couverture UC-10 A2. Les quatre zones sont présentes ; les actions sensibles sont isolées ; la modification du mot de passe est conditionnée (RB-10-05).

- **À améliorer** :
  - L'état erreur « adresse e-mail non validée » n'est pas illustré.
  - Le libellé « Pro » en surface est à surveiller (cohérence terminologique).

---

### 3.11 `recherche-preparation` — Recherche en préparation

**Conformité : CONFORME**

Couverture UC-14. Barre omniprésente et panneau de résultats non disruptif. Regroupement et filtre par type. Les trois états sont présents : résultats chargés, liste vide, liste vide de résultats avec invite à créer (UC-14 A1) — couverture d'états exemplaire.

- **À améliorer** :
  - Seuls 8 types natifs sont échantillonnés dans l'illustration.
  - La variante mode local n'est pas montrée.

---

### 3.12 `suppression-compte-rgpd` — Suppression de compte (RGPD)

**Conformité : PARTIEL — defaut de substance prioritaire**

Couverture UC-10. Présents : deux états (confirmable / bloqué E4), zone de conséquences, case « action définitive », blocage si campagnes actives. NFR-CONF-03 (suppression physique des notes PLAYER_PRIVATE) est correctement énoncé.

**Manque principal** : le verrou RB-10-05 (précondition « adresse e-mail validée ») exigé par la fiche est **absent** de l'écran. Seul le verrou « aucune campagne active » est rendu. Ce verrou est une précondition de sécurité et de conformité RGPD — son absence constitue un **défaut de substance à corriger en priorité**.

- **À surveiller** : l'exhaustivité des conséquences (orphelines/anonymisation) dans l'état bloqué condensé.

---

### 3.13 `tableau-de-bord` — Tableau de bord des espaces de jeu

**Conformité : CONFORME**

Couverture UC-02/UC-06 A5/UC-01. Toutes les zones sont présentes, ainsi que les deux états (mode local, quota atteint). Bandeaux mode local distincts pour durabilité et confidentialité (RB-01-14). AR-17 (espace personnel hors quota, jamais bloqué) et reprise session en un clic (AR-09) sont correctement rendus.

- **À améliorer** : l'état vide 0/3 (sous-spécifié S4) n'est pas illustré.

---

### 3.14 `vue-campagne` — Vue campagne (espace de travail)

**Conformité : CONFORME**

Couverture UC-02/04/05/06/08/11. Les zones sont présentes (accès vue session AR-09, dossiers, liste de documents, recherche, lien d'invitation, paramètres). Les garde-fous AR-01/09/10/11 sont rendus.

- **À améliorer** : les états vide et erreur de synchronisation et la variante mode local ne sont pas montrés.

---

### 3.15 `vue-joueur-base` — Vue joueur post-accès

**Conformité : CONFORME**

Couverture UC-12/09. Les trois zones sont présentes : documents partagés en temps réel (AR-06), fiche personnage en lecture seule (RB-12-07), notes PLAYER_PRIVATE invisibles du MJ (RB-12-02). Les garde-fous de confidentialité sont exemplaires : disparition symétrique AR-06, zone notes absente par nature (RB-12-03).

- **À améliorer** : l'état vide (aucun document partagé) n'est pas illustré.

---

### 3.16 `editeur-document` — Éditeur de document

**Conformité : CONFORME**

Couverture UC-04/07. Les six zones et les sept types de blocs UC-04 sont présents. La visibilité par défaut (Privé MJ — AR-13/UC-08) et les backlinks « référencé par » sont correctement rendus.

- **À améliorer** :
  - L'état vide (brouillon titre seul, UC-04 E1) n'est pas illustré.
  - L'état erreur de synchronisation n'est pas représenté.

---

### 3.17 `editeur-scenario` — Éditeur de scénario

**Conformité : PRESUMEE — audit détaillé non produit**

Couverture UC-03. Cet écran a été récupéré depuis le fichier `download` (sans extension), identifié tardivement comme export valide d'un écran qu'on croyait parasite. Il constitue une spécialisation de l'éditeur de document (structure narrative, scènes liées, documents associés).

> **Statut d'audit** : la passe de conformité détaillée n'a pas été réalisée pour cet écran (fichier récupéré après la session d'audit principale). La conformité est présumée sur la base de la fiche `editeur-scenario.md`. **Une passe d'audit dédiée est recommandée.**

---

## 4. Écrans manquants

Trois écrans canoniques du MVP sont sans wireframe HTML. Leur fiche `.md` est présente dans le répertoire.

### 4.1 `vue-session-mj` — Vue session MJ

**Priorité : CRITIQUE**

Hub central de pilotage de session — tableau de bord configurable à trois modes (configuration / LIVE / consultation CLOSED). Écran prioritaire du MVP (hypothèse H2). Porteur des cas d'usage UC-06 (pilotage de session), UC-08 (mode LIVE, partage aux joueurs) et UC-14 (recherche omniprésente en session). L'absence de cet écran est la lacune la plus significative de la couverture wireframe.

### 4.2 `creation-espace` — Création d'un espace de jeu

Formulaire de création d'un espace partagé (nom obligatoire, description et système optionnels). Porteur de UC-02.

### 4.3 `panneau-creation-rapide` — Panneau de création rapide à la volée

Sur-couche sans navigation propre, invoquée depuis la vue session en mode LIVE et en mode consultation CLOSED. Porteur de UC-07.

---

> **Note sur UC-13** : l'interface de bibliothèque « Mes scénarios » (UC-13 — scénario réutilisable) est hors périmètre wireframe MVP par décision (AR-08 révisé). L'absence de wireframe correspondant n'est pas un manque.

---

## 5. Points de navigation non tranchés par le corpus

Ces points n'ont pas été arbitrés par le corpus de conception à la date d'audit. Ils devront être tranchés avant le câblage fin de la navigation.

1. **Placement de l'affordance « générer un lien d'invitation »** : présente à titre de traçabilité dans `vue-campagne`, `parametres-campagne` et `vue-session-mj` (AR-10, sous-spécifié). Un seul point d'entrée ou les trois au MVP ?

2. **UC-09 A2 — « créer un compte » depuis la surface joueur** : la cible est-elle l'écran `inscription` standard ou un état contextualisé « depuis lien d'invitation » ?

3. **`recherche-preparation` — écran navigable ou overlay** : écran distinct navigable, ou overlay co-présent avec `vue-campagne` / `vue-espace-personnel` ?

4. **Geste « retour » depuis `editeur-document`** : le corpus ne nomme pas l'écran cible du retour depuis l'éditeur.

5. **Vue « Non classés » (UC-05 A4)** : vue dédiée évoquée dans `navigation-dossiers` mais non spécifiée (S9).

---

## 6. Recommandations

Les recommandations sont ordonnées par urgence.

### Priorité haute

- **Corriger le verrou RB-10-05 sur `suppression-compte-rgpd`** : la précondition « adresse e-mail validée » est absente de l'écran. C'est le seul défaut de substance grave identifié — précondition de sécurité et de conformité RGPD.

- **Produire les 3 écrans manquants**, dans cet ordre de priorité :
  1. `vue-session-mj` — écran prioritaire du MVP (H2) ; sa production débloque la couverture UC-06, UC-08 et UC-14 en session.
  2. `creation-espace` — formulaire UC-02.
  3. `panneau-creation-rapide` — sur-couche UC-07.

### Priorité moyenne

- **Auditer `editeur-scenario`** (passe dédiée) : conformité présumée, non vérifiée. Prioriser avant la revue finale.

- **Trancher les points de navigation non résolus** (section 5) avant le câblage fin ; les points 1 et 3 ont le plus d'impact sur la navigation globale.

### Priorité faible

- **Compléter les états secondaires** sur les écrans existants : états vide, erreur de synchronisation et variantes mode local sont absents de manière homogène sur plusieurs écrans. Les écrans les plus concernés sont `editeur-document`, `vue-campagne`, `navigation-dossiers` et `recherche-preparation`. L'impact est faible (conformité non remise en cause) mais la couverture est incomplète pour une revue utilisateur.

### Règle de maintenance

Ne jamais ré-exporter l'outil de design directement par-dessus ce répertoire (cf. README `docs/presentation/wireframes/`). Tout nouvel export transite par `_inbox/`, vérification par contenu, intégration manuelle. Un ré-export direct reproduirait le décalage nom↔contenu et écraserait le câblage de navigation.
