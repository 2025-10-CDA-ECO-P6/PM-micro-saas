# DPA — Data Processing Agreement (Art. 28 RGPD) — squelette

> **CADRE À VALIDER JURISTE — non contractuel, squelette de conception.**
> Ce document n'est **pas** un DPA rédigé ni un texte contractuel opposable. C'est un squelette de sections destiné à guider la rédaction juriste : chaque section indique **quoi y mettre** et renvoie aux ADR de conception d'où provient la matière technique correspondante. Aucune clause n'est tranchée ici. Aucune formulation de ce document ne doit être copiée telle quelle dans un contrat sans revue juriste.

---

## Pourquoi ce document existe

ADR-013 (§5) qualifie Haversack de **sous-traitant (Art. 28 RGPD)** vis-à-vis des utilisateurs Maîtres du Jeu (MJ) qui saisissent, dans l'application, des contenus décrivant des tiers identifiables (joueurs réels projetés sur des personnages). ADR-013 pose explicitement que *« le DPA est un document contractuel, non un ADR technique »* et que son contenu (obligations du sous-traitant, mesures de sécurité, sous-traitants ultérieurs, droits d'audit) est **à définir par l'équipe juridique**.

ADR-018 (section *Conformité conçue, non certifiée*) étend la question : la posture sous-traitant doit être vérifiée **pour les deux périmètres d'espace** — espace partagé (`CAMPAIGN`, `ONE_SHOT`) et espace `PERSONAL` — un contenu personnel décrivant un tiers identifiable (PNJ inspiré d'une personne réelle, note sur un joueur) pouvant ou non entrer dans le périmètre du DPA tel que formulé.

Ce squelette existe pour que la rédaction juriste dispose, section par section, des points de conception déjà actés et des points encore ouverts — sans que la conception ne préjuge d'une qualification ou d'une clause qui relève du juriste.

**Renvoi roadmap** : ce document est le livrable central de [`roadmap-juridique.md`](./roadmap-juridique.md), jalon « DPA finalisé ».

---

## 1. Objet et durée du traitement

**À mettre dans cette section (juriste)** :
- Désignation des parties : Haversack (sous-traitant) / l'utilisateur MJ ou l'entité qu'il représente (responsable de traitement).
- Objet du traitement sous-traité : hébergement et traitement technique des contenus de campagne/session saisis par le MJ (personnages, notes, scénarios) dans la mesure où ils décrivent des tiers identifiables.
- Durée du DPA : alignement proposé sur la durée de vie du compte MJ et/ou de l'espace partagé concerné — **à trancher juriste** (durée fixe, durée du contrat principal, tacite reconduction).
- Articulation avec les CGU/CGV de la plateforme (le DPA est-il un avenant, une annexe, un document distinct accepté séparément ?).

**À valider juriste** : le point d'entrée contractuel — le DPA est-il accepté au moment de la création d'un espace partagé, à la première invitation d'un joueur, ou via une acceptation CGU globale ? Non tranché en conception.

**Renvoi conception** : ADR-013 §5 (qualification sous-traitant, contexte MJ) ; ADR-018 *Conformité conçue, non certifiée* point 2 (extension au périmètre personnel).

---

## 2. Nature et finalités du traitement

**À mettre dans cette section (juriste)** :
- Nature des opérations : collecte, stockage, structuration (blocs de document), sauvegarde/purge, synchronisation local↔cloud (ADR-016), affichage aux membres de l'espace.
- Finalités énoncées par le responsable de traitement (le MJ) : organisation de la campagne/session de jeu de rôle, coordination entre participants.
- Précision sur ce que Haversack ne fait **pas** : pas de profilage des tiers décrits dans les contenus, pas de réutilisation des contenus à des fins autres que la fourniture du service (à faire confirmer contre la politique de confidentialité).

**À valider juriste** : la formulation exacte des finalités doit être cohérente avec la politique de confidentialité (jalon distinct, voir roadmap) et avec le registre des traitements Art. 30 s'il est requis.

**Renvoi conception** : ADR-013 §5 (contexte narratif MJ / tiers identifiables) ; ADR-016 (format de sérialisation et frontière de confiance des contenus transportés, pertinent pour décrire techniquement « ce qui est traité »).

---

## 3. Catégories de données et de personnes concernées

**À mettre dans cette section (juriste)** :
- Catégories de personnes concernées : (i) le MJ lui-même (hors périmètre DPA — il est responsable de traitement, pas la personne concernée du DPA) ; (ii) les joueurs identifiables décrits dans les contenus narratifs saisis par le MJ ; (iii) les joueurs invités (`GuestAccess`) — **à noter : leur traitement direct est couvert par ADR-013 §1-3, hors qualification sous-traitant** ; distinction à clarifier juriste entre les deux régimes.
- Catégories de données : contenus narratifs libres (texte des blocs de document), `display_name` des invités, métadonnées d'attribution (qui a créé/modifié un contenu).
- Catégorie de données sensibles éventuelles : **non évaluée en conception** — un contenu narratif libre peut, selon le texte saisi par un MJ, révéler des catégories particulières de données (Art. 9) de façon incidente ; point à examiner juriste, aucune détection automatique n'est prévue côté conception.

**À valider juriste** : la portée exacte de « tiers identifiable » (seuil d'identifiabilité applicable à un contenu de fiction inspiré de personnes réelles) n'est pas tranchée en conception — ADR-013 §5 le signale explicitement comme relevant du juriste.

**Renvoi conception** : ADR-013 §1 (données invités, périmètre distinct) ; ADR-013 §5 ; ADR-018 *Conformité conçue, non certifiée* point 2.

---

## 4. Obligations du sous-traitant (Art. 28 §3 a–h)

**À mettre dans cette section (juriste)** — squelette point par point, chaque lettre de l'Art. 28§3 à rédiger séparément, aucune ne l'est ici :

- **(a) Traitement sur instruction documentée du responsable** — à définir juriste (quelles instructions, quel canal, que fait Haversack en l'absence d'instruction explicite au-delà du fonctionnement standard de l'app).
- **(b) Engagement de confidentialité du personnel** — à définir juriste (référence à une politique interne Haversack, hors périmètre de ce squelette).
- **(c) Mesures de sécurité Art. 32** — voir section 6 ci-dessous (renvoi ADR-015/016/017), rédaction finale à définir juriste.
- **(d) Conditions de recours à un sous-traitant ultérieur** — voir section 5 ci-dessous, rédaction finale à définir juriste.
- **(e) Assistance pour les demandes d'exercice des droits** — voir section 7 ci-dessous, rédaction finale à définir juriste.
- **(f) Assistance pour la sécurité, notification de violation, analyses d'impact** — à définir juriste ; **note conception** : aucune procédure de notification de violation n'est actée dans le corpus ADR à ce stade — point ouvert, pas seulement une clause de style.
- **(g) Sort des données en fin de contrat** — voir section 8 ci-dessous (renvoi purge ADR-011/012), rédaction finale à définir juriste.
- **(h) Mise à disposition des informations nécessaires + droit d'audit** — voir section 9 ci-dessous, rédaction finale à définir juriste.

**À valider juriste** : l'intégralité de cette section est un squelette de renvoi ; aucune formulation contractuelle n'est proposée.

---

## 5. Sous-traitants ultérieurs

**À mettre dans cette section (juriste)** :
- Liste des sous-traitants ultérieurs pressentis : hébergeur cloud, CDN, service de messagerie transactionnelle (email support, notifications) — **liste indicative de conception, non contractuelle, à confirmer/compléter juriste selon les prestataires effectivement retenus au provisionnement**.
- Régime d'autorisation : autorisation générale avec information préalable des modifications, ou autorisation spécifique par sous-traitant — **à trancher juriste**.
- Obligations répercutées sur les sous-traitants ultérieurs (mêmes garanties que le DPA principal) — clause standard à rédiger juriste.

**À valider juriste** : ADR-013 §5 mentionne explicitement les « sous-traitants ultérieurs » comme un point non défini en conception, à charge du juriste en lien avec la politique de confidentialité.

**Renvoi conception** : ADR-013 §5 ; ADR-013 Points à trancher (« périmètre exact du DPA : contenu, sous-traitants ultérieurs, hébergeur, CDN, droits d'audit »).

---

## 6. Mesures de sécurité (Art. 32)

**À mettre dans cette section (juriste)** — traduction contractuelle des mesures techniques actées en conception, sans réouverture technique :
- Authentification et gestion des tokens — voir ADR-015 (politique de mot de passe, hachage, cycle de vie JWT, rotation des refresh tokens, rate limiting).
- Format et frontière de confiance des données transportées lors de la migration local→cloud — voir ADR-016 (revalidation serveur, sanitisation, frontière de confiance).
- Sécurité du mode local (stockage IndexedDB, CSP, sanitisation côté client, absence de chiffrement at-rest en mode local) — voir ADR-017 §4.

**Note conception, pas une clause** : ADR-017 §4.4 documente explicitement une absence de chiffrement des données at-rest en mode local (bandeau de confidentialité côté produit, pas de mesure cryptographique). Cette limite technique doit être reflétée fidèlement dans la clause de sécurité du DPA — **le juriste ne doit pas rédiger une clause de sécurité qui suggérerait un chiffrement at-rest non implémenté.**

**À valider juriste** : niveau de granularité attendu par un DPA standard Art. 32 (renvoi à une annexe technique versionnée vs description en corps de contrat) — choix rédactionnel juriste.

**Renvoi conception** : ADR-015 (sécurité authentification) ; ADR-016 (sérialisation et frontière de confiance) ; ADR-017 §4 (sécurité du mode local).

---

## 7. Assistance aux droits des personnes concernées

**À mettre dans cette section (juriste)** :
- Modalités par lesquelles Haversack assiste le MJ (responsable de traitement) lorsqu'une personne décrite dans un contenu narratif exerce un droit (accès, rectification, effacement, opposition) à l'égard du MJ.
- Canal de remontée d'une demande reçue directement par Haversack pour un traitement dont le MJ est responsable (redirection vers le MJ, ou traitement direct selon accord).
- Distinction à documenter juriste vis-à-vis du canal **déjà défini pour les invités eux-mêmes** (traitement direct de leurs propres données par Haversack, hors qualification sous-traitant) — voir ADR-013 §2, canal support déjà retenu pour ce cas distinct.

**À valider juriste** : le régime d'assistance Art. 28§3(e) pour des tiers décrits dans un contenu narratif (qui n'ont eux-mêmes aucun compte ni accès direct à la plateforme) est un cas non standard — aucune procédure n'est actée en conception au-delà du canal support des invités, qui couvre un périmètre différent.

**Renvoi conception** : ADR-013 §2 (procédure d'exercice des droits des invités, périmètre distinct) ; le cadrage juridique interne porte également l'item « définir l'adresse email support + la procédure d'exercice des droits des invités » (non encore réalisé).

---

## 8. Sort des données en fin de contrat

**À mettre dans cette section (juriste)** :
- Choix contractuel standard Art. 28§3(g) : suppression ou restitution des données à la fin de la prestation, au choix du responsable de traitement.
- Articulation avec les mécaniques de purge déjà actées en conception :
  - Purge d'un espace partagé (saga `SpaceDeleted`, J+30 après soft-delete) — voir ADR-011.
  - Anonymisation de compte utilisateur (saga `UserAnonymized`) et sort différencié des documents (supprimables / conservés sous intérêt légitime documenté) — voir ADR-012 §2-3.
  - Purge autonome des `guest_accesses` expirés (90 jours après `expires_at`, indépendamment de la purge d'espace) — voir ADR-013 §3.
- Question à trancher juriste : la fin du DPA (résiliation du contrat sous-traitant) est-elle un événement distinct des mécanismes de purge produit ci-dessus, ou s'appuie-t-elle sur les mêmes sagas ? Le corpus de conception n'a pas modélisé de déclencheur "fin de DPA" indépendant d'une suppression de compte ou d'espace.

**À valider juriste** : aucune clause de restitution des données (export à la fin du contrat, format, délai) n'est actée en conception — à rédiger juriste en cohérence avec les mécanismes de purge existants, sans leur ajouter un chemin de purge supplémentaire sans validation technique.

**Renvoi conception** : ADR-011 (saga `SpaceDeleted`, cascade et purge) ; ADR-012 §2-3 (sort des données à l'effacement de compte) ; ADR-013 §3 (purge autonome des invités).

---

## 9. Droits d'audit

**À mettre dans cette section (juriste)** :
- Modalités d'exercice d'un droit d'audit du responsable de traitement (MJ, ou entité qu'il représente) sur Haversack : fréquence, préavis, périmètre (documentation vs audit technique sur site), coût.
- Alternative usuelle : mise à disposition de certifications/rapports d'audit tiers en lieu et place d'un audit direct — **à trancher juriste**, aucune certification n'est actée en conception à ce stade (MVP).

**À valider juriste** : ADR-013 §5 mentionne les « droits d'audit » comme un point non défini en conception, à charge du juriste. Aucune mesure organisationnelle d'audit n'existe dans le corpus MVP au-delà des mesures de sécurité techniques listées en section 6.

**Renvoi conception** : ADR-013 §5 ; ADR-013 Points à trancher.

---

## Points de qualification préalables — non tranchés ici

Avant même de rédiger un DPA définitif, deux qualifications juridiques amont conditionnent son périmètre et ne sont **pas tranchées par la conception** :

1. **Haversack est-il sous-traitant ou responsable conjoint/autonome** pour les contenus MJ décrivant des tiers identifiables ? — ADR-013 §5, ADR-013 *Conformité conçue, non certifiée* point 2.
2. **Le périmètre s'étend-il aux contenus de l'espace `PERSONAL`** (contenu personnel décrivant un tiers identifiable) ou seulement aux espaces partagés ? — ADR-018 *Conformité conçue, non certifiée* point 2.

Ces deux points sont consolidés dans le cadrage de validation pré-lancement EU (5 axes), voir [`cadrage-validation-pre-lancement-eu.md`](./cadrage-validation-pre-lancement-eu.md).

---

## Renvois croisés

| Sujet | Source conception |
|---|---|
| Qualification sous-traitant Art. 28, contexte MJ | ADR-013 §5 |
| Extension du périmètre à l'espace `PERSONAL` | ADR-018, *Conformité conçue, non certifiée* point 2 |
| Sécurité — authentification, tokens | ADR-015 |
| Sécurité — sérialisation, frontière de confiance | ADR-016 |
| Sécurité — mode local, IndexedDB, CSP | ADR-017 §4 |
| Purge d'espace (fin de contrat / suppression) | ADR-011 |
| Anonymisation de compte, sort des documents | ADR-012 §2-3 |
| Purge autonome des invités | ADR-013 §3 |
| Séquencement du DPA dans la trajectoire juridique globale | [`roadmap-juridique.md`](./roadmap-juridique.md) |
