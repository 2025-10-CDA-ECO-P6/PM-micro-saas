# Persona 07 — Sonia, la MJ one-shot


## Profil

31 ans, animatrice en centre de loisirs. MJ depuis 7 ans, exclusivement sur des one-shots et
courts métrages. Systèmes joués : Alien RPG, Call of Cthulhu, Ironsworn. Elle anime 3 à 5 parties
par mois dans son association locale, dans des festivals de jeux, ou pour des groupes qui veulent
découvrir le JDR sans s'engager sur le long terme.

## Qui elle est

Sonia ne fait pas de campagnes. Chaque partie qu'elle mène est une histoire complète en 3 à 4
heures, avec des joueurs qui changent à chaque fois. Elle a une quinzaine de scénarios dans son
catalogue, qu'elle rejoue et ajuste selon le groupe et l'occasion. Pas de continuité narrative,
pas d'historique commun, pas de groupe stable.

Sa logique de travail est différente d'un MJ de campagne. Elle prépare un scénario comme un
script : des indices, des atmosphères, des PNJ qui n'apparaissent qu'une seule fois. Ce qui
l'intéresse dans un outil, c'est de pouvoir retrouver ses scénarios, les adapter rapidement et
partager des documents avec des joueurs qu'elle ne reverra peut-être jamais.

Pour Sonia, le contenu lui appartient ; la campagne est une organisation optionnelle d'une
partie de ce contenu, pas un prérequis. Ses scénarios vivent au niveau de son compte,
indépendamment de tout groupe. Elle les instancie pour un groupe quand elle joue, et les range
quand la soirée est terminée.

Si elle ouvre Haversack, elle verra "créer une campagne" et aura l'impression que l'outil ne
parle pas d'elle. Créer une campagne vide pour chaque one-shot serait absurde de son point de vue.

## Ce qu'elle utiliserait

- Un espace personnel qui contient tous ses scénarios, accessible sans créer de campagne
- Un espace par scénario instancié, sans notion de campagne comme conteneur obligatoire
- Partager un document ponctuel (une lettre, une carte, un journal in-game) à des joueurs sans
  compte, juste pour la durée de la soirée
- Une bibliothèque de ses scénarios qu'elle peut parcourir et réutiliser

## Ce qu'elle demande à l'app

- Peut-on créer un espace de jeu sans l'appeler "campagne" ?
- Peut-on rejouer le même scénario avec un groupe différent sans dupliquer tout le contenu ?
- Comment gérer des joueurs qui changent à chaque session ? Le modèle de membres suppose une
  liste stable.
- Peut-on partager un document ponctuel à des joueurs sans compte ?

## Ce que ce persona révèle

L'architecture du produit est construite autour de l'espace comme conteneur central, déclinable
en campagne, one-shot ou usage personnel (ADR-018). Ce choix ouvre partiellement la porte à
Sonia en MVP : via l'espace personnel, elle peut stocker et préparer ses scénarios sans avoir à
créer de campagne, ce qui couvre ses besoins de préparation solo. Elle reste **non servie sur
UC-13** (bibliothèque de scénarios réutilisables et instanciation inter-espaces, post-MVP) : la
réutilisation d'un scénario d'un groupe à l'autre, sans dupliquer tout le contenu, n'est pas
disponible en première livraison. La vue session et le partage de documents conservent une valeur
indépendante de la notion de campagne longue. Le one-shot complet reste une porte d'entrée
naturelle vers le JDR pour de nouveaux MJ, et son absence comme parcours dédié ferme un canal
d'acquisition. Sonia est **partiellement servie en MVP** sur ses besoins de préparation, mais
reste hors cible sur le contexte one-shot structuré. Ses questions sur la réutilisabilité
méritent d'être posées maintenant.
