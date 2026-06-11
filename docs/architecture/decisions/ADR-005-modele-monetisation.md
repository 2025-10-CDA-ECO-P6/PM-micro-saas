# ADR-005 — Modèle de monétisation

- **Statut** : Accepté
- **Date** : 2026-06-09
- **Décideur** : opérateur (validation explicite, session d'audit/remédiation)
- **Findings liés** : CR-6, E-01, E-02, D-03, A-09

> **Nature : décision produit — fusionnée** — la substance de cette décision a été fusionnée dans la couche vision (`docs/conception/vision/moscow.md` et `vision-produit.md`) le 2026-06-10. Ce document est une trace historique ; le raisonnement et les alternatives écartées restent lisibles ici. *(Annotation du 2026-06-10 — arbitrage T-06, audit conception pure 2026-06.)*

---

## Contexte

L'audit a identifié que le tier Gratuit précédent offrait la quasi-totalité de la valeur perçue du produit (cloud sync, partage joueurs, multi-device, jusqu'à 3 campagnes), laissant le tier Pro sans différenciateur réel. La « value metric » retenue (nombre de campagnes) ne croît pas avec la valeur perçue : la majorité des MJ actifs ne dépassent pas 3 campagnes simultanées, rendant la conversion Pro structurellement proche de zéro (finding CR-6, E-01).

---

## Décision

**Pro = l'échelle.** Le tier Pro donne accès à : campagnes illimitées, multi-device, table de joueurs plus large, stockage étendu.

**Gratuit = un avant-goût.** Le tier Gratuit donne accès à : 1 campagne cloud, partage avec un petit groupe. L'accroche « le partage est gratuit » est maintenue, mais l'échelle de l'usage et du partage est ce qui est monétisé.

**Les chiffres précis** (prix, seuils exacts de joueurs, volume de stockage) sont à valider en Vague 2, après évaluation du coût d'infrastructure par utilisateur.

---

## Alternatives considérées

**Pro = le partage complet (Gratuit = sauvegarde solo uniquement).**
Écartée. Cette option sacrifie le différenciateur « le partage est inclus » que la vision produit revendique comme élément de différenciation. Priver le tier Gratuit de toute expérience de partage réduit le vecteur d'adoption virale.

**Pro = fonctionnalités qualitatives (historique, export avancé, recherche full-text).**
Écartée. Le risque est que peu de MJ aient besoin de ces fonctionnalités au point de payer pour elles. Les fonctionnalités qualitatives sont difficiles à présenter comme une nécessité perçue dans un tunnel de conversion.

---

## Conséquences

- La table de monétisation dans la section §3 de la vision produit doit être révisée pour refléter la nouvelle structure (Vague 1/Vague 2).
- Une note de coût d'infrastructure est à produire : coût d'un utilisateur gratuit, revenu net d'un utilisateur Pro, ratio de break-even (Vague 2, finding E-02).
- Le dimensionnement du marché (TAM/SAM, ARPU cible) et la stratégie d'acquisition (GTM) sont à instruire en Vague 2 (finding E-04, E-05).
- La limite du tier Gratuit (1 campagne cloud) implique une révision des invariants de domaine sur `Campaign` (finding A-09 sur la limite « 4 joueurs FREE » également à aligner).

---

## Compléments post-revue (2026-06-09)

Suite à la revue adversariale (revue Vague 0, artefact purgé du corpus — historique git), cette décision est complétée comme suit, sans changer sa direction.

- **Levier Pro révisé (arbitrage opérateur).** Le tier Pro donne accès à : campagnes illimitées + multi-device + stockage étendu. Le levier « table plus large » est retiré : aucune persona ne demande davantage de joueurs, les tables sont fixes entre 3 et 6 participants — ce levier était mort.

- **A-09 à trancher avant la Vague 1.** Décider si la limite « 4 joueurs/session » pour le tier FREE existe. Si oui, l'inscrire dans la table de monétisation de la vision produit. Si non, la retirer des invariants de domaine. Ce point ne peut pas rester non tranché au démarrage du build.

- **Périmètre de conversion testé par le MVP.** La conversion testée par le MVP est local→gratuit (hypothèse 5a). La conversion payante (hypothèse 5b) n'est pas testée par le MVP : le seul déclencheur Pro atteignable est le passage au-delà d'une campagne cloud.
