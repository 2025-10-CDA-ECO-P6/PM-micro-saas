# Analyse d'impact relative à la protection des données

## Description des traitements

L'analyse porte sur les traitements PR-01 à PR-10 du [registre des traitements](registre-des-traitements.md) et n'en reprend aucun. Aucune source ne qualifie un traitement de « à risque élevé » : l'obligation de conduire cette analyse n'est pas tranchée [À VALIDER — JURISTE]. Le [registre des risques](../../projet/registre-des-risques.md) dit lui-même qu'il ne se substitue pas à une éventuelle analyse d'impact, qui examinerait chaque traitement en détail.

Chaque traitement est couvert parce qu'un événement redouté pour les personnes s'y rattache dans les sources :

- PR-01 : prise de contrôle d'un compte, énumération d'adresses, mineur sans vérification d'âge (IR-06, IR-08, IR-10).
- PR-02 : ré-identification d'une personne après son effacement (IR-02).
- PR-03 : exposition de notes privées, effacement non respecté (IR-01, IR-03).
- PR-04 : conservation de données dont l'effacement est demandé (IR-03).
- PR-05 : adresse d'origine conservée au-delà de la notification (IR-11).
- PR-06 : données d'invités gardées sans finalité, usurpation auprès du support, fuite d'un contenu privé, mineur sans vérification d'âge (IR-04, IR-05, IR-07, IR-10).
- PR-07 : journaux techniques d'invités gardés trop longtemps (IR-04).
- PR-08 et PR-09 : sortie de contenu narratif par la télémétrie (IR-09).
- PR-10 : fuite d'un contenu privé de tiers identifiables (IR-07).

## Nécessité et proportionnalité

- PR-01 : aucune source n'énonce pourquoi chaque donnée du compte est nécessaire ni quelle voie plus légère a été écartée [À VALIDER — JURISTE].
- PR-02 : la suppression physique de la ligne `users` est impossible tant que des espaces référencent l'`UserId`, d'où l'anonymisation par réécriture (ADR-007 § 1 ; ADR-012, « Alternatives considérées », hard-delete du compte). Une pseudonymisation sans purge des journaux de corrélation a été rejetée comme « factice » (ADR-012, « Alternatives considérées ») [À VALIDER — JURISTE].
- PR-03 : ces contenus sont de nature strictement personnelle et aucun intérêt légitime tiers ne justifie leur conservation (ADR-012 § 3(a)) ; la conservation de tous les documents a été rejetée (ADR-012, « Alternatives considérées ») [À VALIDER — JURISTE].
- PR-04 : la suppression des contenus partagés détruirait l'expérience des tiers (personnages orphelins, scénarios incomplets) ; supprimer tous les documents a été rejeté (ADR-012 § 3(c), « Alternatives considérées ») ; la mise en balance n'a pas été conduite (ADR-012, « Conformité conçue, non certifiée », point 3) [À VALIDER — JURISTE].
- PR-05 : l'adresse réécrite ne permet plus de joindre la personne, et les canaux « affichage en session » et « journal d'audit » ont été écartés ; l'enregistrement est limité à la cible, à l'identifiant de la demande et à un drapeau (ADR-012 § 8) ; sa compatibilité avec la minimisation n'est pas tranchée (ADR-012 § 8, point (b)) [À VALIDER — JURISTE].
- PR-06 : le `display_name` est le seul identifiant collecté, fourni volontairement, sans profilage (ADR-013 § 1, test de mise en balance résumé) ; le consentement et l'exécution du contrat ont été écartés (ADR-013, « Alternatives considérées ») ; des délais de 7 ou 30 jours ont été écartés au profit de 90 jours (ADR-013, « Alternatives considérées ») [À VALIDER — JURISTE].
- PR-07 : ces journaux servent à détecter les abus, déboguer et sécuriser le service (ADR-013 § 1) ; aucune source ne dit pourquoi 30 jours suffisent ni quelle voie plus légère a été écartée [À VALIDER — JURISTE].
- PR-08 : cette mesure compense la perte de signal due au MVP unique (spécification de télémétrie, § 1) ; elle mesure l'occurrence sans capter le contenu narratif (spécification de télémétrie, § 5) et rien ne quitte le navigateur avant la création d'un compte (spécification de télémétrie, § 3.4) [À VALIDER — JURISTE].
- PR-09 : la sollicitation est unique, après l'activation, et un refus vaut refus définitif (spécification de télémétrie, § 4.2) ; aucune source n'énonce pourquoi l'adresse est nécessaire [À VALIDER — JURISTE].
- PR-10 : les finalités sont celles du meneur de jeu, responsable de traitement (squelette de DPA, § 2) ; aucune source n'énonce la nécessité ni la proportionnalité de l'hébergement de ces contenus, ni la qualification de Haversack (ADR-013 § 5) [À VALIDER — JURISTE].

## Risques pour les personnes

| ID | Événement redouté | Gravité | Vraisemblance | Traitements |
| --- | --- | --- | --- | --- |
| IR-01 | un invité réassocié au même `character_id` lit les notes `PLAYER_PRIVATE` d'un compte supprimé (ADR-012 § 4, règle F-08) | [SOUS-SPÉCIFIÉ — échelle de gravité non sourcée] | [SOUS-SPÉCIFIÉ — échelle de vraisemblance non sourcée] | PR-03 |
| IR-02 | l'identité réelle d'une personne dont le compte est effacé est retrouvée par les journaux corrélant `UserId` et email, ou par les sauvegardes (ADR-012 § 5, « Alternatives considérées ») | [SOUS-SPÉCIFIÉ — échelle de gravité non sourcée] | [SOUS-SPÉCIFIÉ — échelle de vraisemblance non sourcée] | PR-02 |
| IR-03 | des données dont la personne a demandé l'effacement sont conservées, ou des données à conserver sont détruites, parce que leur appartenance d'espace a changé entre la demande et l'exécution (ADR-012 § 3(b), § 7) | [SOUS-SPÉCIFIÉ — échelle de gravité non sourcée] | [SOUS-SPÉCIFIÉ — échelle de vraisemblance non sourcée] | PR-03, PR-04 |
| IR-04 | le `display_name` ou l'adresse IP d'un invité est gardé sans finalité après l'expiration de son accès (ADR-013 § 3 : accès expirés depuis 18 mois) | [SOUS-SPÉCIFIÉ — échelle de gravité non sourcée] | [SOUS-SPÉCIFIÉ — échelle de vraisemblance non sourcée] | PR-06, PR-07 |
| IR-05 | un tiers se fait passer pour un invité auprès du support et obtient la divulgation ou l'effacement de ses données (procédure d'exercice des droits des invités, § 2) | [SOUS-SPÉCIFIÉ — échelle de gravité non sourcée] | [SOUS-SPÉCIFIÉ — échelle de vraisemblance non sourcée] | PR-06 |
| IR-06 | un tiers prend le contrôle du compte d'une personne, par préemption de compte, vol ou réutilisation de jeton (dossier de sécurité, surfaces S1 et S2) | [SOUS-SPÉCIFIÉ — échelle de gravité non sourcée] | [SOUS-SPÉCIFIÉ — échelle de vraisemblance non sourcée] | PR-01 |
| IR-07 | un contenu `GM_ONLY` ou `PLAYER_PRIVATE`, ou la description d'un tiers, est lu par une personne non autorisée par l'API ou le canal temps réel (ADR-007, « Contexte » ; dossier de sécurité, surfaces S3 et S4) | [SOUS-SPÉCIFIÉ — échelle de gravité non sourcée] | [SOUS-SPÉCIFIÉ — échelle de vraisemblance non sourcée] | PR-06, PR-10 |
| IR-08 | un observateur confirme qu'une adresse email possède un compte, par le message « adresse déjà associée » (ADR-015, « Résidu CWE-204 ») | [SOUS-SPÉCIFIÉ — échelle de gravité non sourcée] | [SOUS-SPÉCIFIÉ — échelle de vraisemblance non sourcée] | PR-01 |
| IR-09 | du contenu narratif d'un meneur de jeu, ou une donnée identifiante, sort du navigateur par la télémétrie (spécification de télémétrie, § 5) | [SOUS-SPÉCIFIÉ — échelle de gravité non sourcée] | [SOUS-SPÉCIFIÉ — échelle de vraisemblance non sourcée] | PR-08, PR-09 |
| IR-10 | les données d'une personne mineure sont traitées sans autre garde que sa déclaration d'avoir 16 ans (ADR-007 § 2 ; ADR-013 § 4) | [SOUS-SPÉCIFIÉ — échelle de gravité non sourcée] | [SOUS-SPÉCIFIÉ — échelle de vraisemblance non sourcée] | PR-01, PR-06 |
| IR-11 | l'adresse d'origine d'une personne dont le compte est anonymisé est conservée au-delà de la fin du traitement de sa demande (ADR-012 § 8, `[À VALIDER — JURISTE]` (b)) | [SOUS-SPÉCIFIÉ — échelle de gravité non sourcée] | [SOUS-SPÉCIFIÉ — échelle de vraisemblance non sourcée] | PR-05 |

## Mesures

| Mesure | Risques réduits | Ancre |
| --- | --- | --- |
| suppression physique définitive des documents `PLAYER_PRIVATE` et des notes liées aux personnages | IR-01 | contre-mesure # 39 du [dossier de sécurité](dossier-securite.md) ; [ADR-012](../../decisions/ADR-012-rgpd-effacement-compte.md) § 3(a) et § 4 |
| purge des journaux de corrélation identifiant technique et email, et rétention des sauvegardes limitée à 30 jours (« valeur retenue ; à confirmer selon la stratégie de backup P7 multi-instance », ADR-012 § 5) | IR-02 | contre-mesure # 41 ; ADR-012 § 5 |
| figement de la sélection à `deletion_requested_at`, jeu matérialisé consommé sans recalcul, relocation obligatoire | IR-03 | contre-mesure # 43 ; ADR-012 § 7 |
| purge autonome des accès invités expirés à 90 jours, journaux techniques conservés 30 jours | IR-04 | contre-mesure # 44 ; [ADR-013](../../decisions/ADR-013-rgpd-donnees-invites.md) § 1 et § 3 |
| mot de passe de 12 caractères au moins, hachage Argon2id, jetons d'accès ≤ 15 minutes, signature asymétrique, rotation des jetons de rafraîchissement par famille | IR-06 | contre-mesures # 1 à # 6 ; [ADR-015](../../decisions/ADR-015-securite-authentification-mvp.md) § 1 et § 3 |
| verrou anti-préemption : liaison OAuth seulement si l'email préexistant est prouvé vérifié, confiance conditionnée à la déclaration du fournisseur | IR-06 | contre-mesures # 12 et # 13 ; ADR-015 § 2.2 et § 2.3 |
| limitation de débit hybride par adresse IP et par compte, qui mitige aussi le résidu d'énumération accepté | IR-06, IR-08 | contre-mesure # 9 ; ADR-015 § 4.1 et « Résidu CWE-204 » |
| point d'autorisation unique composant appartenance et visibilité, appliqué à toute requête portée par un espace, `PLAYER_PRIVATE` réservé à l'auteur seul | IR-07 | contre-mesures # 17, # 18 et # 20 ; [ADR-014](../../decisions/ADR-014-modele-autorisation-api.md) § 3 et § 7 |
| filtrage de chaque diffusion SignalR par la même politique, jeton invité hors chaîne de requête, déconnexion forcée à la révocation de l'accès | IR-07 | contre-mesures # 19, # 25 et # 26 ; ADR-014 § 6 ; ADR-004, « Compléments post-revue » |
| case d'attestation « au moins 16 ans » à l'inscription et au formulaire invité | IR-10 | ADR-007 § 2 ; ADR-013 § 4 |
| mesure d'occurrence sans captation du contenu narratif, compteurs écrits dans le store local, sollicitation d'email unique avec refus définitif | IR-09 | contre-mesure # 45 ; [spécification de télémétrie](../../architecture/specs/telemetrie.md), § 3.4, § 4.2 et § 5 |
| enregistrement de notification à finalité limitée, purgé à l'envoi réussi ou en fin de fenêtre Art. 12§3 | IR-11 | ADR-012 § 8 |

## Points ouverts

- IR-05 n'a aucune mesure : la méthode de vérification d'identité n'est retenue par aucune source (procédure d'exercice des droits des invités, § 2) [À VALIDER — JURISTE].
- IR-06 : le mécanisme de reprise en place (contre-mesure # 16) n'est pas ratifié et n'est pas compté comme une mesure (dossier de sécurité, section 5.2) [À VALIDER — JURISTE].
- IR-08 : le résidu est un arbitrage produit accepté (dossier de sécurité, section 5.2), dont l'acceptation au regard des personnes n'est pas qualifiée [À VALIDER — JURISTE].
- IR-10 : la suffisance de l'attestation de 16 ans n'est pas établie (ADR-007 § 2 ; ADR-013, « Conformité conçue, non certifiée », point 1) [À VALIDER — JURISTE].
- IR-03 : l'instant qui fait foi juridiquement, demande ou exécution, n'est pas tranché ; la suffisance du mécanisme de figement n'est pas validée (ADR-012 § 7) [À VALIDER — JURISTE].
- IR-11 : la compatibilité de la rétention de l'adresse sous base « obligation légale » avec la minimisation n'est pas tranchée (ADR-012 § 8, point (b)) [À VALIDER — JURISTE].
- Les onze risques portent une gravité et une vraisemblance sans valeur : aucune source ne fixe d'échelle de cotation pour les personnes.
