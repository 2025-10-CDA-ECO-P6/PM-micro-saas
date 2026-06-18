# Inscription

> Fiche de description d'écran basse-fidélité — Vague 4, sous-vague 1, lot A.
> Instancie le gabarit `docs/conception/interface/gabarit-ecran.md`.
> Notation et nommage : `docs/conception/interface/conventions-wireframe.md`.
> Arbitrages figés : `docs/conception/interface/zoning.md §S6`.

---

## En-tête de fiche

```
Nom          : Inscription
Surface      : transversal
Contexte     : transversal
Type d'espace: n.a. (l'écran est indépendant du type d'espace)
Forme cible  : grand écran + tablette (mobile : pensé dans la structure, non implémenté au MVP — AR-07)
Traçabilité  : UC-10 (scénario nominal Inscription sans données locales,
               scénario nominal Connexion via fournisseur externe) ; UC-01 A1
```

---

## Noyau obligatoire

### Intention

```
Intention : l'utilisateur crée un compte pour accéder à la synchronisation cloud,
            au partage avec les joueurs et à l'accès multi-appareils — en saisissant
            une adresse de messagerie, un nom d'affichage et un mot de passe,
            ou en s'authentifiant via un fournisseur d'identité externe en un seul geste.
```

---

### Zones et hiérarchie

```
[ FORMULAIRE DE CRÉATION DE COMPTE ]
  type     : formulaire
  rôle     : recueille les informations nécessaires à la création du compte —
             adresse de messagerie, nom d'affichage, mot de passe (UC-10 §scénario
             nominal Inscription) ; les trois champs sont obligatoires pour ce chemin
  priorité : principal
  visibilité : tous (utilisateur non authentifié)

[ ZONE D'ACCÈS VIA FOURNISSEUR EXTERNE ]
  type     : principal
  rôle     : permet de créer un compte en un geste via un fournisseur d'identité
             externe — aucun mot de passe à définir ; si un compte existe déjà avec
             cette adresse de messagerie, l'utilisateur y est connecté directement
             (UC-10 §scénario nominal Connexion via fournisseur externe) ;
             mis en avant comme chemin de moindre coût à l'instant de bascule (AR-13)
             [SOUS-SPÉCIFIÉ — UC-10 §Questions à valider en interview]
             le ou les fournisseurs exposés et leur wording ne sont pas figés
  priorité : principal
  visibilité : tous (utilisateur non authentifié)

[ LIEN VERS LA CONNEXION ]
  type     : châssis
  rôle     : oriente l'utilisateur ayant déjà un compte vers l'écran de connexion,
             sans friction supplémentaire
  priorité : principal
  visibilité : tous (utilisateur non authentifié)
```

---

### Ce que l'utilisateur peut faire

```
- [UC-10 §scénario nominal Inscription sans données locales] saisir adresse de messagerie,
  nom d'affichage et mot de passe → le compte est créé (tier gratuit) ;
  l'utilisateur est redirigé vers l'écran de création de campagne

- [UC-10 §scénario nominal Inscription depuis le mode local] créer un compte depuis le
  mode local → si des données locales existent, le gate de migration local→cloud est présenté
  (titres, volume, date des espaces détectés) avant toute migration ; la migration ne
  démarre qu'après confirmation explicite

- [UC-10 §scénario nominal Connexion via fournisseur externe] s'authentifier via un
  fournisseur externe → le compte est créé automatiquement (premier accès) ou l'utilisateur
  est connecté au compte existant (même adresse de messagerie) ; si des données locales
  existent, le gate de migration local→cloud est présenté avant migration (ADR-016 §4 — Gate de reconnaissance)

- [AR-13 ; UC-01 A1] accéder à cet écran depuis une invite contextuelle (action cloud
  déclenchée en mode local) → l'invite a orienté l'utilisateur vers l'inscription ;
  le formulaire s'affiche en contexte, sans perdre le contenu en cours
```

---

### États

```
état vide (arrivée sur l'écran, aucune saisie) :
  le formulaire s'affiche vierge ; la zone d'accès via fournisseur externe est disponible ;
  aucun message d'erreur

état chargé (saisie en cours) :
  les champs reflètent la saisie de l'utilisateur ; aucune validation en temps réel
  définie dans le corpus — comportement non figé

état erreur (adresse de messagerie déjà utilisée) :
  le système informe l'utilisateur que l'adresse est déjà associée à un compte (UC-10 E1) ;
  le formulaire reste affiché et éditable ; l'utilisateur peut corriger ou basculer
  vers la connexion
```

---

### Accessibilité (delta)

Les garanties transversales (focus clavier visible en permanence, ordre de navigation cohérent) sont portées par le châssis applicatif (`zoning.md §S7 §Accessibilité transversale`). Cette section couvre uniquement ce qui est spécifique à l'écran d'inscription.

```
annonce sans action :
  - erreur de soumission (adresse déjà utilisée — UC-10 E1) : annoncée assistivement
    sans que l'utilisateur ait à déplacer son focus — NFR-ACC-02 (changements d'état
    dynamiques annoncés sans action)

hiérarchie de lecture à distance :
  priorité 1 — formulaire de création de compte (chemin principal)
  priorité 2 — zone d'accès via fournisseur externe (chemin alternatif en un geste)
  priorité 3 — lien vers la connexion
  source : NFR-ACC-04 (ordre de lecture logique conforme à l'intention de l'écran)
```

---

### Sources

```
Sources : UC-10 (scénario nominal Inscription sans données locales,
                  scénario nominal Inscription depuis le mode local,
                  scénario nominal Connexion via fournisseur externe,
                  E1 — Email déjà utilisé) ;
          UC-01 A1 (invite contextuelle déclenchant l'inscription) ;
          AR-13 (fournisseur externe mis en avant, moindre coût à l'instant de bascule) ;
          NFR-ACC-02 ; NFR-ACC-04 ;
          ADR-016 §4 (gate de reconnaissance avant migration) ;
          châssis S7 (zoning.md §S7)
```

---

## Sections conditionnelles

### Hors-périmètre / différé / sous-spécifié (déclenché — éléments S9)

```
Éléments sous-spécifiés (S9) :
  [SOUS-SPÉCIFIÉ — UC-10 §Questions à valider en interview]
    fournisseur(s) d'identité externe exposés sur cet écran et leur wording —
    non figés dans le corpus
```
