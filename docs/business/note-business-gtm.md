# Note business — coût, marché & go-to-market (Haversack)

> **CADRE MÉTHODOLOGIQUE** — les valeurs chiffrées de coût et de marché sont différées (Vague 2 / étude marché) ; ce document pose la méthode et les hypothèses, pas des chiffres définitifs.

---

## Lecteur cible et usage de ce document

Ce document s'adresse à l'**opérateur / pilote produit** de Haversack — celui qui arbitre la trajectoire business (Vague 2, levée éventuelle, priorisation du plan d'étude marché). Il ne s'adresse pas à un exécutant technique : aucun artefact de code n'y figure, conformément à la phase de conception du repo.

**Ce que ce document fait** : décomposer la méthode de calcul du coût infra, du dimensionnement de marché et de la stratégie go-to-market (GTM), en s'appuyant strictement sur le corpus de conception existant (ADR-005, cahier des charges §12.4/§2.4, vision produit, MoSCoW).

**Ce que ce document ne fait pas** : produire des chiffres de coût d'infrastructure, de taille de marché ou d'ARPU. Le corpus les diffère explicitement à la Vague 2 (ADR-005, § Conséquences : *« Une note de coût d'infrastructure est à produire […] Vague 2 »* ; *« Le dimensionnement du marché […] sont à instruire en Vague 2 »*). Toute valeur non présente dans le corpus est marquée `[À TRANCHER — donnée Vague 2 / étude marché]` plutôt qu'estimée.

**Entrées connues** (seules valeurs chiffrées disponibles à ce stade, consolidées et datées dans `docs/cahier-des-charges.md` §12.4, arrêtées au 2026-07-02) :

| Paramètre | Valeur connue | Source |
|---|---|---|
| Quota d'espaces `CAMPAIGN`/`ONE_SHOT` (palier gratuit) | 3 espaces | CdC §12.4 |
| Espace personnel décompté du quota | Non | CdC §12.4 |
| Joueurs par session (palier gratuit) | 4 joueurs distincts, MJ non compté | CdC §12.4 |
| Stockage — palier local | Aucun quota produit (limite = appareil) | CdC §12.4 |
| Stockage — palier gratuit | 500 Mo | CdC §12.4 |
| Stockage — palier Pro | 5 Go et plus | CdC §12.4 |
| Tarif — palier Pro | Environ 7 €/mois | CdC §12.4 |

Ces valeurs sont elles-mêmes qualifiées de **volatiles** par le corpus (CdC §12.4) — elles servent ici d'entrées de méthode, pas de constantes figées.

---

## Volet 1 — Coût d'infrastructure & break-even

### 1.1 Contexte et hypothèse de méthode

ADR-005 diffère explicitement le chiffrage du coût d'infrastructure à la Vague 2, *« après évaluation du coût d'infrastructure par utilisateur »* (ADR-005, Décision). Le finding **E-02** (ADR-005, § Conséquences) charge cette note de poser : le coût d'un utilisateur gratuit, le revenu net d'un utilisateur Pro, et le ratio de break-even.

Ce volet pose la **formule** de chacun de ces trois éléments. Aucune valeur unitaire de coût n'est inscrite — `[À TRANCHER — mesure coût infra Vague 2]`.

### 1.2 Coût par utilisateur gratuit

Le palier Gratuit (CdC §2.4) donne accès à : synchronisation cloud, partage aux joueurs, accès multi-appareil, dans la limite de 3 espaces `CAMPAIGN`/`ONE_SHOT`, 4 joueurs par session, 500 Mo de stockage.

Les **drivers de coût** identifiables à partir de cette description fonctionnelle :

| Driver | Origine fonctionnelle | Nature du coût |
|---|---|---|
| Stockage cloud | Quota 500 Mo par utilisateur gratuit (CdC §12.4) | Coût de stockage à la volumétrie effectivement occupée (pas au quota alloué) |
| Synchronisation cloud | UC-10 — migration locale→cloud, disponibilité multi-device | Coût de calcul/écriture à la fréquence de synchronisation |
| Trafic de partage joueurs | UC-08/UC-09 — diffusion temps réel vers des joueurs sans compte | Coût de bande passante et de connexions concurrentes, fonction du nombre de joueurs actifs simultanés (borné à 4 par session, CdC §12.4) |
| Session temps réel | UC-06/UC-07 — vue de session, création à la volée | Coût de calcul/connexion pendant la durée de la session active |

**Formule (méthode, pas de valeur)** :

```
Coût_utilisateur_gratuit = Coût_stockage(volumétrie réelle ≤ 500 Mo)
                          + Coût_synchro(fréquence de synchro × nombre d'espaces actifs)
                          + Coût_trafic_partage(nombre de sessions partagées × joueurs concurrents ≤ 4)
                          + Coût_session_active(durée cumulée des sessions ouvertes)
```

Chaque terme de droite est `[À TRANCHER — mesure coût infra Vague 2]` : la mesure réelle suppose une instrumentation de coût par service cloud (stockage, temps réel, bande passante), absente du corpus de conception actuel.

### 1.3 Revenu net par utilisateur Pro

Le palier Pro (CdC §2.4/§12.4) : tarif ≈ 7 €/mois, tout le gratuit en illimité, stockage 5 Go et plus, espaces illimités.

**Formule (méthode)** :

```
Revenu_net_utilisateur_Pro = Tarif_Pro (≈ 7 €/mois, CdC §12.4)
                            − Coût_infra_Pro(stockage 5 Go+, synchro, trafic de partage, sans plafond d'espaces)
```

`Coût_infra_Pro` suit la même décomposition que le Volet 1.2, avec deux différences structurelles à noter pour le chiffrage Vague 2 :
- le stockage n'est plus plafonné à 500 Mo mais à 5 Go et plus — le coût marginal de stockage par palier Go doit être établi ;
- le nombre d'espaces actifs n'est plus plafonné à 3 — le coût de synchronisation devient fonction d'un usage non borné, ce qui change la nature du calcul (coût variable pur, non un forfait par quota).

`Tarif_Pro` est connu (≈ 7 €/mois) ; `Coût_infra_Pro` est `[À TRANCHER — mesure coût infra Vague 2]`.

### 1.4 Ratio de break-even

Le finding **E-02** (ADR-005) demande le ratio de conversion gratuit→Pro nécessaire pour couvrir le coût des utilisateurs gratuits.

**Formule (méthode)** :

```
Ratio_unitaire_break-even = Coût_utilisateur_gratuit / Revenu_net_utilisateur_Pro

Ce ratio exprime le nombre de gratuits qu'un utilisateur Pro peut financer.
La condition de point mort est atteinte quand :
  N_Pro × Revenu_net_utilisateur_Pro ≥ N_gratuits × Coût_utilisateur_gratuit

Ou, en termes de ratio unitaire :
  N_Pro / N_gratuits ≥ Ratio_unitaire_break-even
```

Interprétation attendue (à confirmer une fois les coûts unitaires connus) : ce ratio unitaire exprime, indépendamment du volume total d'utilisateurs, combien d'utilisateurs gratuits un utilisateur Pro peut financer. Le déclencheur d'upgrade Gratuit→Pro identifié par le corpus (CdC §2.4 : *« déclenché lorsque le MJ dépasse le quota de trois espaces actifs »*) est le seul signal de conversion actuellement instrumenté (H5, CdC §12.4) — il n'existe pas aujourd'hui de mesure de taux de conversion réel à insérer dans ce ratio.

**Valeurs manquantes pour appliquer ce ratio** : `Coût_utilisateur_gratuit`, `Coût_infra_Pro`, et le taux de conversion réel gratuit→Pro observé — `[À TRANCHER — mesure coût infra Vague 2]`.

---

## Volet 2 — Dimensionnement de marché (TAM/SAM/ARPU)

### 2.1 Contexte et hypothèse de méthode

ADR-005 (§ Conséquences) diffère à la Vague 2 le dimensionnement du marché (TAM/SAM) et l'ARPU cible (finding **E-04**), ainsi que la stratégie d'acquisition GTM (finding **E-05**). Ce volet pose la méthode de calcul et les segments identifiables dans le corpus — aucune valeur de population ou de taille de marché n'est inscrite.

### 2.2 TAM — marché total adressable

**Définition méthodologique** : population totale des Maîtres du Jeu de jeu de rôle sur table (JDR), tous supports et toutes géographies confondus — c'est la borne haute théorique, indépendante de la capacité réelle de Haversack à l'atteindre.

**Ce que le corpus fournit** : le positionnement produit (vision-produit.md §1) situe Haversack face à des outils génériques (Notion, Obsidian, Google Docs) et des tables virtuelles (Roll20, Foundry) — un marché de MJ qui utilisent déjà, ou pourraient utiliser, un outil numérique pour préparer et piloter leurs parties. Le corpus ne fournit aucune estimation de population.

**Donnée manquante** : taille de la population MJ (par géographie, par système de jeu, par fréquence de jeu) — `[À TRANCHER — étude marché]`.

### 2.3 SAM — marché atteignable

**Définition méthodologique** : sous-ensemble du TAM que Haversack peut réalistement adresser compte tenu de son positionnement (agnostique au système de jeu, MJ comme utilisateur principal, sans moteur de règles) et de ses contraintes de lancement (langue, canaux de distribution).

**Segments dérivables du corpus** (vision-produit.md §4.1, personas de référence) :

| Segment | Caractérisation | Source |
|---|---|---|
| MJ numériquement outillé, campagne longue | Personas Thomas (Obsidian, 400 notes, exigeant sur la portabilité), Émilie (systèmes narratifs, création à la volée) | vision-produit.md §4.1 |
| MJ one-shot / convention | Persona Sonia (one-shots exclusivement, 15 scénarios en catalogue) — segment non servi en première livraison (UC-13 hors MVP) | vision-produit.md §4.1, moscow.md UC-13 |
| MJ résistant au numérique | Persona Rémi — n'adopte que si la friction est nulle et la valeur immédiate | vision-produit.md §4.1 |

**Hypothèse de primo-ciblage géographique** : le corpus ne tranche pas explicitement un périmètre géographique de lancement. Une hypothèse plausible — francophone puis élargissement UE — s'aligne avec une première version dont l'interface utilisateur n'est pas multi-langue, mais reste une hypothèse, pas une décision du corpus — `[À TRANCHER — étude marché / décision de lancement]`.

**Donnée manquante** : taille du SAM (nombre de MJ numériquement outillés atteignables) — `[À TRANCHER — étude marché]`.

### 2.4 ARPU cible

**Définition méthodologique** : revenu moyen par utilisateur, dérivé du tarif Pro et du taux de conversion gratuit→Pro observé.

**Formule (méthode)** :

```
ARPU = (N_Pro × Tarif_Pro) / (N_Pro + N_gratuits)
```

`Tarif_Pro` est connu (≈ 7 €/mois, CdC §12.4). `N_Pro`, `N_gratuits` et donc l'ARPU ne peuvent être posés qu'après observation réelle d'une cohorte (les seuils H1-H5, CdC §12.4, mesurent l'activation et la conversion locale→gratuit, pas encore gratuit→Pro à l'échelle) — `[À TRANCHER — étude marché / observation post-lancement]`.

---

## Volet 3 — Stratégie go-to-market

### 3.1 Moteur d'adoption virale (dérivable du corpus)

Le corpus inscrit un mécanisme d'adoption explicite, indépendant de toute donnée de marché :

- **Le partage est inclus au palier gratuit.** ADR-005 (Décision) maintient explicitement cette accroche : *« L'accroche "le partage est gratuit" est maintenue, mais l'échelle de l'usage et du partage est ce qui est monétisé »*. Vision-produit.md §3 formule la même règle : *« Haversack intègre le partage joueurs dans le tier gratuit, ce qui aligne l'adoption MJ et joueurs »*.
- **Mécanique virale attendue** `[À TRANCHER — validation post-lancement]` : chaque MJ au palier gratuit qui partage une session (UC-08) expose l'outil à ses joueurs sans qu'ils créent de compte (UC-09) — un joueur qui devient à son tour MJ d'une table est un canal d'acquisition organique, non payant. Ce mécanisme est cohérent avec la référence explicite du corpus à Obsidian (*« modèle proche d'Obsidian (local gratuit, sync payant) »*, vision-produit.md §3) où le partage, lui, est la variable différenciante ajoutée par Haversack.
- **Ce que le corpus ne monétise pas** : la vision est explicite — le partage lui-même n'est jamais le levier payant (ADR-005, alternative *« Pro = le partage complet »* explicitement écartée : *« sacrifie le différenciateur […] réduit le vecteur d'adoption virale »*). La monétisation porte sur l'**échelle** (nombre d'espaces, stockage), pas sur l'acte de partager.

### 3.2 Friction d'entrée nulle (dérivable du corpus)

- **Aucun compte requis pour commencer** (UC-01, vision-produit.md §2.2) : un MJ crée un espace et prépare du contenu sans inscription, en mode local.
- **Aucun compte requis pour le joueur invité** (UC-09) : un lien, un nom d'affichage, un accès immédiat.
- Conséquence GTM directe : le tunnel d'acquisition n'a pas de barrière d'inscription à l'entrée — la friction n'apparaît qu'au moment où la valeur (partage, sauvegarde cloud) est déjà perçue, ce qui est la logique déclencheur documentée en CdC §2.4 (*« Le passage du palier local au palier gratuit est déclenché naturellement lorsque le MJ souhaite partager une information avec ses joueurs, ou lorsqu'il souhaite sécuriser ses données locales »*).

### 3.3 Monétisation sur l'échelle, pas sur le partage (dérivable du corpus)

Rappel de la règle de palier (CdC §2.4, ADR-005) : le déclencheur Gratuit→Pro est le dépassement du quota de 3 espaces actifs — pas un plafond de partage, de joueurs, ou de fonctionnalité de collaboration. Le levier Pro retenu par ADR-005 (§ Compléments post-revue) est explicitement *« campagnes illimitées + multi-device + stockage étendu »* — le levier « table plus large » (plus de joueurs par session) a été retiré *« aucune persona ne demande davantage de joueurs »*. La stratégie GTM peut donc s'appuyer sur un message stable : *le partage ne coûte jamais plus cher, l'échelle de préparation oui.*

### 3.4 Canaux et tactiques d'acquisition

Le corpus ne fixe pas de plan de canaux — finding **E-05** (ADR-005) est explicitement différé à la Vague 2. Sur la base des segments de personas dérivés en 2.3, les hypothèses de canaux suivantes sont **plausibles au vu du corpus**, sans être une décision :

| Canal hypothétique | Rationale (persona associée) | Statut |
|---|---|---|
| Communautés en ligne de JDR (forums, Discord de jeu, Reddit) | Émilie, Antoine — MJ déjà actifs dans des communautés de systèmes narratifs / non-D&D | `[À TRANCHER — plan GTM Vague 2]` |
| Conventions et événements one-shot | Sonia — profil convention/one-shot, actuellement non servi par la première livraison (UC-13 hors MVP) ; canal pertinent seulement après re-priorisation de UC-13 | `[À TRANCHER — plan GTM Vague 2]` |
| Bouche-à-oreille joueur→MJ | Mécanique virale du Volet 3.1 — un joueur invité sans compte peut devenir MJ d'une future table | `[À TRANCHER — plan GTM Vague 2]` |
| Créateurs de contenu JDR (streaming, chaînes dédiées) | Non dérivable directement des personas actuels du corpus | `[À TRANCHER — plan GTM Vague 2]` |

**Point de vigilance GTM** : le segment Sonia (one-shot/convention) n'est structurellement pas servi par la première livraison (moscow.md, UC-13 *« hors première livraison »*). Un canal GTM construit autour des conventions arriverait donc en avance sur le produit livrable, sauf si la condition de retour de UC-13 (*« si les entretiens ou l'usage révèlent que le profil one-shot est une part significative […], UC-13 est réexaminé »*, vision-produit.md §5bis) est déclenchée.

---

## Synthèse — ce qui reste à trancher en Vague 2

| Volet | Donnée manquante | Origine du différé |
|---|---|---|
| 1 — Coût & break-even | Coût unitaire stockage/synchro/trafic, coût infra Pro, taux de conversion réel gratuit→Pro | ADR-005, finding E-02 |
| 2 — Marché | Taille TAM, taille SAM, ARPU observé | ADR-005, finding E-04 |
| 3 — GTM | Plan de canaux et tactiques d'acquisition, périmètre géographique de lancement | ADR-005, finding E-05 |

Aucune de ces données n'est estimée dans ce document : le corpus de conception ne les porte pas, et ADR-005 les qualifie explicitement de travail de Vague 2. Ce document fournit la méthode et les entrées connues (CdC §12.4) nécessaires pour les instruire dès qu'une mesure de coût infra et une étude marché seront disponibles.
