# Exigences non fonctionnelles produit — Haversack

> **Nature du document.** Ce répertoire rassemble les exigences non fonctionnelles côté besoin :
> ce que l'utilisateur doit pouvoir ressentir, faire ou vérifier, indépendamment des choix
> d'implémentation. Les exigences sont formulées du point de vue de l'utilisateur et restent
> observables par quiconque utilise le produit.
>
> **Filtre de nature.** Chaque exigence a passé le test du README de conception :
> masquer une technologie ou un ADR ne la vide pas de son sens. Le COMMENT appartient aux
> couches architecture et implémentation ; ce document dit uniquement le QUOI.
>
> **Règle de lecture.** Un critère d'observation est exprimé en expérience vécue, jamais en
> métrique d'infrastructure. Un identifiant stable (NFR-XXX-NN) permet de tracer chaque
> exigence depuis les user stories et les critères d'acceptation.

---

## 1. Performance perçue

La douleur principale du MJ pendant une partie est le temps perdu à chercher une information
(vision §1.1 : « le rythme de jeu est cassé »). La vue session est conçue précisément pour
réduire ce délai ressenti (UC-06 contexte). Les exigences ci-dessous expriment ce que le MJ
et le joueur doivent ressentir, non ce qu'une mesure d'infrastructure doit constater.

| Exigence | Intitulé |
|----------|----------|
| [NFR-PERF-01](NFR-PERF-01-fluidite-navigation-session.md) | Fluidité de la navigation en session |
| [NFR-PERF-02](NFR-PERF-02-reactivite-premiere-interaction.md) | Réactivité à la première interaction |
| [NFR-PERF-03](NFR-PERF-03-continuite-sans-interruption-session.md) | Continuité sans interruption lors des actions en cours de partie |
| [NFR-PERF-04](NFR-PERF-04-resultats-recherche-immediats.md) | Résultats de recherche immédiats |

---

## 2. Hors connexion

Le mode local (UC-01) est le point d'entrée sans compte du produit. Il doit fonctionner
intégralement sans réseau, conformément au différenciant « données toujours possédées par
l'utilisateur » (vision §5).

**Périmètre MVP assumé.** Le mode local sans compte fonctionne intégralement sans connexion :
préparation, vue session, création à la volée, recherche locale. Les capacités de partage
avec les joueurs et de sauvegarde dans le cloud nécessitent une connexion — c'est un choix
de périmètre explicite, pas un oubli. La synchronisation hors connexion entre plusieurs
appareils est explicitement exclue du MVP : la vision §2.4 identifie cette capacité comme
une complexité disproportionnée au regard du périmètre initial.

| Exigence | Intitulé |
|----------|----------|
| [NFR-OFF-01](NFR-OFF-01-utilisation-integrale-sans-connexion.md) | Utilisation intégrale sans connexion en mode local |
| [NFR-OFF-02](NFR-OFF-02-durabilite-donnees-locales.md) | Durabilité des données locales entre les sessions |
| [NFR-OFF-03](NFR-OFF-03-aucune-perte-silencieuse-donnees-locales.md) | Aucune perte silencieuse de données locales |
| [NFR-OFF-04](NFR-OFF-04-fonctionnement-partiel-perte-reseau-cloud.md) | Fonctionnement partiel en cas de perte de réseau passagère en mode cloud |
| [NFR-OFF-05](NFR-OFF-05-continuite-edition-preparation-cloud.md) | Continuité d'édition en préparation cloud lors d'une perte de réseau |

---

## 3. Confidentialité

Haversack repose sur une asymétrie d'information fondamentale : le MJ prépare des
informations secrètes (PNJ cachés, intrigues, fins alternatives) et décide de ce qui est
révélé aux joueurs, quand et comment. Ce contrôle est une règle métier centrale, non
une préférence de confort.

La protection des données personnelles des utilisateurs s'inscrit dans le cadre du règlement
européen de protection des données personnelles (référence : RGPD), dont les exigences
s'appliquent aux données traitées par le produit.

| Exigence | Intitulé |
|----------|----------|
| [NFR-CONF-01](NFR-CONF-01-separation-notes-privees-vue-joueurs.md) | Séparation stricte entre notes privées du MJ et vue des joueurs |
| [NFR-CONF-02](NFR-CONF-02-isolation-donnees-mode-local.md) | Isolation des données en mode local |
| [NFR-CONF-03](NFR-CONF-03-suppression-effective-donnees-personnelles.md) | Suppression effective des données personnelles sur demande |
| [NFR-CONF-04](NFR-CONF-04-transparence-risques-mode-local-partage.md) | Transparence sur les risques de confidentialité en mode local partagé |

---

## 4. Accessibilité

Haversack s'utilise dans des conditions particulières : éclairage faible autour d'une table
de jeu, lecture rapide en pleine session, parfois sur un grand écran partagé ou un petit
écran mobile. Ces contextes réels d'usage définissent les exigences ci-dessous.

Le référentiel d'accessibilité des contenus web reconnu internationalement (référence :
WCAG 2.1, niveau AA) fournit des critères mesurables qui guident l'implémentation de ces
exigences, sans s'y substituer.

| Exigence | Intitulé |
|----------|----------|
| [NFR-ACC-01](NFR-ACC-01-utilisabilite-sans-dispositif-de-pointage.md) | Utilisabilité sans dispositif de pointage |
| [NFR-ACC-02](NFR-ACC-02-compatibilite-lecteurs-ecran.md) | Compatibilité avec les outils de lecture d'écran |
| [NFR-ACC-03](NFR-ACC-03-lisibilite-faible-eclairage.md) | Lisibilité en conditions de faible éclairage |
| [NFR-ACC-04](NFR-ACC-04-tailles-texte-lecture-rapide-session.md) | Tailles de texte adaptées à la lecture rapide en session |

---

## 5. Internationalisation

Le produit est développé et lancé en français. Sa conception doit permettre d'accueillir
d'autres langues sans refonte de l'expérience utilisateur — c'est une exigence d'évolutivité
produit, non un engagement de calendrier.

**Périmètre MVP assumé.** L'interface est entièrement en français au MVP. Aucun autre marché
linguistique n'est engagé pour cette version. L'exigence NFR-I18N-02 exprime que cette
extension doit rester possible sans travaux structurels, pas qu'elle est planifiée.

| Exigence | Intitulé |
|----------|----------|
| [NFR-I18N-01](NFR-I18N-01-interface-en-francais.md) | Interface entièrement utilisable en français au MVP |
| [NFR-I18N-02](NFR-I18N-02-evolutivite-multilingue-sans-refonte.md) | Évolutivité vers d'autres langues sans refonte de l'expérience |
| [NFR-I18N-03](NFR-I18N-03-liberte-langue-contenu-mj.md) | Liberté totale de langue pour le contenu créé par le MJ |
