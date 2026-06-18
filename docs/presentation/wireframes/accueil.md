# Accueil non authentifié

> Fiche de description d'écran basse-fidélité — Vague 4, sous-vague 1, lot A.
> Instancie le gabarit `docs/conception/interface/gabarit-ecran.md`.
> Notation et nommage : `docs/conception/interface/conventions-wireframe.md`.
> Arbitrages figés : `docs/conception/interface/zoning.md §S6`.

---

## En-tête de fiche

```
Nom          : Accueil non authentifié
Surface      : transversal
Contexte     : transversal
Type d'espace: n.a. (l'écran est indépendant du type d'espace)
Forme cible  : grand écran + tablette (mobile : pensé dans la structure, non implémenté au MVP — AR-07)
Traçabilité  : UC-01, UC-10
```

---

## Noyau obligatoire

### Intention

```
Intention : l'utilisateur non authentifié découvre les deux chemins d'entrée dans l'application
            — commencer sans compte ou créer un compte / se connecter — et choisit celui qui
            correspond à son besoin du moment, sans friction d'inscription imposée.
```

---

### Zones et hiérarchie

```
[ ZONE DE PRÉSENTATION ]
  type     : principal
  rôle     : présente l'application et son usage en quelques mots ; oriente l'utilisateur
             vers l'un des deux chemins d'entrée ; contient un message court sur le
             stockage local (données conservées localement sur l'appareil — UC-01 §scénario
             nominal étape 4) avec un lien vers la FAQ
  priorité : principal
  visibilité : tous (utilisateur non authentifié uniquement — cet écran n'est pas accessible
               après authentification)

[ PORTE D'ENTRÉE SANS COMPTE ]
  type     : principal
  rôle     : point d'accès au mode local — l'utilisateur démarre immédiatement, sans
             fournir d'identifiants ; le contenu créé atterrit dans l'espace personnel
             par défaut (UC-01 ; RB-01-01) ; deux chemins équivalents sur cet écran,
             aucun n'est mis en avant plus que l'autre
  priorité : principal
  visibilité : tous (utilisateur non authentifié)

[ PORTE D'ENTRÉE AVEC COMPTE ]
  type     : principal
  rôle     : point d'accès à l'inscription ou à la connexion — l'utilisateur se dirige vers
             la création de compte ou l'authentification ; le chemin inscription et le chemin
             connexion sont distincts mais logés dans la même porte d'entrée
  priorité : principal
  visibilité : tous (utilisateur non authentifié)
```

---

### Ce que l'utilisateur peut faire

```
- [UC-01 §scénario nominal étape 3] choisir de commencer sans compte → l'utilisateur accède
  à l'espace de travail en mode local ; le contenu qu'il crée atterrit dans l'espace personnel
  par défaut, sans créer de campagne (RB-01-01)

- [UC-10 §scénario nominal Inscription sans données locales] choisir de créer un compte →
  l'utilisateur est conduit vers l'écran d'inscription

- [UC-10 §scénario nominal Connexion] choisir de se connecter → l'utilisateur est conduit
  vers l'écran de connexion
```

---

### États

```
état vide (première visite, aucune donnée locale ni compte détecté) :
  l'écran s'affiche avec les deux portes d'entrée et le message sur le stockage local ;
  état standard de l'écran

état chargé :
  [SOUS-SPÉCIFIÉ — UC-01 §Questions à valider en interview]
  si des données locales existent dans le navigateur, l'écran peut proposer
  de reprendre la session (UC-01 A2) — comportement non figé

état erreur :
  cet écran ne dépend d'aucune connexion au service cloud ; aucun état d'erreur réseau
  pertinent à ce stade — l'écran s'affiche sans connexion (AR-15 ; UC-01)
```

---

### Accessibilité (delta)

Les garanties transversales (focus clavier visible en permanence, ordre de navigation cohérent) sont portées par le châssis applicatif (`zoning.md §S7 §Accessibilité transversale`). Cette section couvre uniquement ce qui est spécifique à l'écran d'accueil.

```
annonce sans action :
  aucun changement d'état dynamique sur cet écran — pas d'annonce assistive spécifique
  au-delà des garanties transversales du châssis ; les deux portes d'entrée sont
  statiques et accessibles dès le chargement — NFR-ACC-02 (aucun delta nécessaire)

hiérarchie de lecture à distance :
  priorité 1 — message de présentation (zone de présentation — contexte et choix)
  priorité 2 — porte d'entrée sans compte
  priorité 3 — porte d'entrée avec compte
  source : NFR-ACC-04 (lecture à distance ; ordre de lecture logique conforme à l'intention
           de l'écran — deux chemins équivalents, lisibles sans ambiguïté)
```

---

### Sources

```
Sources : UC-01 (scénario nominal, A1, A2, RB-01-01, RB-01-03) ;
          UC-10 (scénario nominal Inscription sans données locales,
                 scénario nominal Connexion) ;
          AR-13 (invite contextuelle sans-compte → compte) ;
          AR-15 (onboarding capture-first) ;
          NFR-ACC-02 ; NFR-ACC-04 ;
          châssis S7 (zoning.md §S7)
```

---

## Sections conditionnelles

### Hors-périmètre / différé / sous-spécifié (déclenché — éléments S9)

```
Éléments sous-spécifiés (S9) :
  [SOUS-SPÉCIFIÉ — UC-01 §Questions à valider en interview]
    comportement de l'écran quand des données locales sont détectées au retour
    (UC-01 A2) — le corpus ne décrit pas l'interface de cet état de retour
```
