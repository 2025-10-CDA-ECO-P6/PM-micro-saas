# Use Cases hors MVP — Vision produit long terme

Les use cases suivants ne sont pas prévus dans le MVP, mais peuvent guider l'architecture et la vision long terme.

## UC-F01 — Utiliser des templates de système de jeu

Le MJ choisit un système de jeu ou un template. Le produit adapte certains champs : fiche personnage, fiche PNJ, ressources, structure de scénario.

### Intérêt

- Améliore l'adoption.
- Réduit la configuration manuelle.
- Rend l'outil plus adapté aux usages réels.

### Risque

- Forte complexité car chaque système de jeu a ses propres règles.
- Risque de transformer le projet en moteur de règles.

### Position recommandée

Prévoir une fiche générique dans le MVP, puis ajouter des templates configurables plus tard.

---

## UC-F02 — Assistant IA pour le MJ

Le MJ utilise une aide IA pour générer des idées, structurer un scénario, créer un PNJ, résumer une session ou proposer des pistes d'improvisation.

### Intérêt

- Forte valeur perçue.
- Cohérent avec le besoin d'aide à la préparation.
- Peut différencier le produit à moyen terme.

### Risque

- Peut brouiller le positionnement du MVP.
- Risque de construire un produit "IA" au lieu d'un outil d'organisation.

### Position recommandée

L'IA doit rester une extension de l'assistant MJ, pas le cœur du MVP.

---

## UC-F03 — Table visuelle légère

Le produit propose une table visuelle pour représenter des cartes, positions ou scènes.

### Intérêt

- Utile pour les sessions en ligne.
- Peut compléter l'expérience de session.

### Risque

- Concurrence directe avec Roll20 et Foundry.
- Complexité technique élevée.
- Peut détourner le produit de son cœur : l'aide au MJ.

### Position recommandée

À considérer uniquement après validation du socle MJ.

---

## UC-F04 — Version desktop ou local-first

Le MJ utilise l'application en local, potentiellement via une version desktop, avec synchronisation optionnelle.

### Intérêt

- Répond au besoin d'usage hors ligne.
- Pertinent pour les sessions physiques.
- Peut rassurer les utilisateurs sur la possession de leurs données.

### Risque

- Complexifie l'architecture.
- Nécessite une stratégie de synchronisation.

### Position recommandée

Ne pas développer dans le MVP, mais éviter une architecture trop dépendante du cloud si cette piste est importante à long terme.

---

## UC-F05 — Templates communautaires

Les utilisateurs peuvent créer, partager ou installer des templates de campagne, fiches ou aides de jeu.

### Intérêt

- Crée un effet communautaire.
- Peut enrichir le produit sans tout développer en interne.
- Peut soutenir une stratégie freemium ou premium.

### Risque

- Modération.
- Qualité variable.
- Complexité de gestion d'un catalogue.

### Position recommandée

Vision long terme uniquement.
