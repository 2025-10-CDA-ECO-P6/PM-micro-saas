# Connexion

> Fiche de description d'écran basse-fidélité — écrans transversaux.
> Instancie le gabarit `docs/conception/interface/gabarit-ecran.md`.
> Notation et nommage : `docs/conception/interface/conventions-wireframe.md`.
> Arbitrages figés : `docs/conception/interface/zoning.md §S6`.

---

## En-tête de fiche

```
Nom          : Connexion
Surface      : transversal
Contexte     : transversal
Type d'espace: n.a. (l'écran est indépendant du type d'espace)
Forme cible  : grand écran + tablette (mobile : pensé dans la structure, non implémenté au MVP — AR-07)
Traçabilité  : UC-10 (scénario nominal Connexion, scénario nominal Connexion via fournisseur
               externe, A1 — Réinitialisation du mot de passe, E2 — Identifiants invalides)
```

---

## Noyau obligatoire

### Intention

```
Intention : l'utilisateur disposant d'un compte s'authentifie avec son adresse de messagerie
            et son mot de passe ou via un fournisseur d'identité externe, pour accéder à
            son tableau de bord et à ses espaces synchronisés dans le cloud.
```

---

### Zones et hiérarchie

```
[ FORMULAIRE DE CONNEXION ]
  type     : formulaire
  rôle     : recueille l'adresse de messagerie et le mot de passe de l'utilisateur
             (UC-10 §scénario nominal Connexion) ; permet la soumission des identifiants
  priorité : principal
  visibilité : tous (utilisateur non authentifié)

[ ZONE D'ACCÈS VIA FOURNISSEUR EXTERNE ]
  type     : principal
  rôle     : permet de se connecter en un geste via un fournisseur d'identité externe ;
             si un compte existe déjà avec l'adresse de messagerie fournie par le
             fournisseur, l'utilisateur y est connecté directement ; si aucun compte
             n'existe, un nouveau compte est créé automatiquement (tier gratuit)
             (UC-10 §scénario nominal Connexion via fournisseur externe)
             fournisseurs exposés : Google et Discord (cahier des charges §Authentification —
             RÉSOLU ; ADR-015 ; zoning.md §S9 « Fournisseurs d'identité — RÉSOLU »)
             [SOUS-SPÉCIFIÉ — UC-10 §Questions à valider en interview] le wording exact
             des libellés de bouton par fournisseur n'est pas figé
  priorité : principal
  visibilité : tous (utilisateur non authentifié)

[ LIEN DE RÉINITIALISATION DU MOT DE PASSE ]
  type     : châssis
  rôle     : oriente l'utilisateur ayant oublié son mot de passe vers la procédure
             de réinitialisation — demande d'un lien de réinitialisation par adresse
             de messagerie (UC-10 A1)
  priorité : principal
  visibilité : tous (utilisateur non authentifié)

[ LIEN VERS L'INSCRIPTION ]
  type     : châssis
  rôle     : oriente l'utilisateur sans compte vers l'écran d'inscription,
             sans friction supplémentaire
  priorité : principal
  visibilité : tous (utilisateur non authentifié)
```

---

### Ce que l'utilisateur peut faire

```
- [UC-10 §scénario nominal Connexion] saisir adresse de messagerie et mot de passe
  → les identifiants sont validés ; l'utilisateur est redirigé vers son tableau de bord

- [UC-10 §scénario nominal Connexion via fournisseur externe] s'authentifier via
  un fournisseur externe → connexion au compte existant (même adresse de messagerie)
  ou création d'un nouveau compte (tier gratuit, premier accès) ; redirection vers
  le tableau de bord

- [UC-10 A1] demander la réinitialisation du mot de passe → le système envoie
  un lien de réinitialisation à l'adresse de messagerie saisie ; l'utilisateur choisit
  un nouveau mot de passe depuis ce lien (le lien expire — UC-10 E3)
```

---

### États

```
état vide (arrivée sur l'écran, aucune saisie) :
  le formulaire de connexion s'affiche vierge ; la zone via fournisseur externe
  et le lien de réinitialisation sont disponibles ; aucun message d'erreur

état chargé (saisie en cours) :
  les champs reflètent la saisie de l'utilisateur ;
  aucune validation en temps réel définie dans le corpus — comportement non figé

état erreur (identifiants invalides) :
  le système affiche un message d'erreur générique sans préciser si c'est l'adresse
  de messagerie ou le mot de passe qui est incorrect — UC-10 E2 (protection par
  non-divulgation de la donnée en erreur) ; le formulaire reste affiché et éditable
```

---

### Accessibilité (delta)

Les garanties transversales (focus clavier visible en permanence, ordre de navigation cohérent) sont portées par le châssis applicatif (`zoning.md §S7 §Accessibilité transversale`). Cette section couvre uniquement ce qui est spécifique à l'écran de connexion.

```
annonce sans action :
  - erreur de soumission (identifiants invalides — UC-10 E2) : annoncée assistivement
    sans que l'utilisateur ait à déplacer son focus — NFR-ACC-02 (changements d'état
    dynamiques annoncés sans action) ; le message générique (sans désignation du champ
    erroné) est maintenu même dans l'annonce assistive — protection UC-10 E2

hiérarchie de lecture à distance :
  priorité 1 — formulaire de connexion (chemin principal)
  priorité 2 — zone d'accès via fournisseur externe (chemin alternatif en un geste)
  priorité 3 — lien de réinitialisation du mot de passe
  priorité 4 — lien vers l'inscription
  source : [SOUS-SPÉCIFIÉ] Aucune exigence non fonctionnelle du corpus ne couvre la
           hiérarchie de lecture visuelle hors session (NFR-ACC-04 exclut explicitement
           la phase de préparation ; NFR-ACC-01 couvre l'ordre de tabulation clavier,
           objet distinct). La hiérarchie décrite ici relève de la bonne pratique et
           attend une source.
```

---

### Sources

```
Sources : UC-10 (scénario nominal Connexion,
                  scénario nominal Connexion via fournisseur externe,
                  A1 — Réinitialisation du mot de passe,
                  E2 — Identifiants invalides,
                  E3 — Lien de réinitialisation expiré) ;
          NFR-ACC-02 ;
          châssis S7 (zoning.md §S7)
```

---

## Sections conditionnelles

### Hors-périmètre / différé / sous-spécifié (déclenché — éléments S9)

```
Éléments sous-spécifiés (S9) :
  [SOUS-SPÉCIFIÉ — UC-10 §Questions à valider en interview] wording exact des libellés
    de bouton par fournisseur d'identité externe — les fournisseurs eux-mêmes (Google,
    Discord) sont résolus (cahier des charges §Authentification ; ADR-015)
```
