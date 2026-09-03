# Profil utilisateur

> Fiche de description d'écran basse-fidélité — écrans transversaux.
> Instancie le gabarit `docs/conception/interface/gabarit-ecran.md`.
> Notation et nommage : `docs/conception/interface/conventions-wireframe.md`.
> Arbitrages figés : `docs/conception/interface/zoning.md §S6`.

---

## En-tête de fiche

```
Nom          : Profil utilisateur
Surface      : transversal
Contexte     : transversal
Type d'espace: n.a. (l'écran est indépendant du type d'espace)
Forme cible  : grand écran + tablette (mobile : pensé dans la structure, non implémenté au MVP — AR-07)
Traçabilité  : UC-10 A2
```

---

## Noyau obligatoire

### Intention

```
Intention : l'utilisateur authentifié consulte et modifie les informations de son compte
            — nom d'affichage, mot de passe — et prend connaissance du niveau de compte
            actif, depuis un écran dédié accessible depuis le tableau de bord.
```

---

### Zones et hiérarchie

```
[ ZONE D'INFORMATIONS DE COMPTE ]
  type     : principal
  rôle     : affiche le niveau de compte actif (compte gratuit ou Pro) en lecture seule ;
             donne à l'utilisateur une vision synthétique de ce dont son compte dispose
  priorité : principal
  visibilité : utilisateur authentifié

[ FORMULAIRE DE MODIFICATION DU NOM D'AFFICHAGE ]
  type     : formulaire
  rôle     : permet à l'utilisateur de lire et de modifier le nom d'affichage associé
             à son compte ; le nom modifié est répercuté dans tous les espaces où
             l'utilisateur est membre ; borné à 100 caractères maximum, ne peut pas
             être vide (glossaire §Nom d'affichage)
  priorité : principal
  visibilité : utilisateur authentifié

[ FORMULAIRE DE MODIFICATION DU MOT DE PASSE ]
  type     : formulaire
  rôle     : permet à l'utilisateur de définir un nouveau mot de passe après avoir
             confirmé l'actuel ; uniquement accessible si l'adresse de messagerie
             du compte est validée (UC-10 §Règles métier) ; sans objet pour les
             comptes créés exclusivement via connexion fédérée (aucun mot de passe
             défini — UC-10 §Scénario nominal Connexion via fournisseur externe)
  priorité : principal
  visibilité : utilisateur authentifié

[ ZONE D'ACCÈS AUX ACTIONS SENSIBLES ]
  type     : principal
  rôle     : regroupe les actions irréversibles liées au compte — accès à l'écran de
             suppression de compte (RGPD) ; ces actions sont présentées séparément du
             reste du formulaire pour éviter toute activation involontaire
  priorité : secondaire-configurable
  visibilité : utilisateur authentifié
```

---

### Ce que l'utilisateur peut faire

```
- [UC-10 A2] modifier son nom d'affichage → le nouveau nom d'affichage est enregistré ;
  il est immédiatement visible dans l'interface dans tous les espaces où l'utilisateur
  est présent

- [UC-10 A2] modifier son mot de passe → après confirmation du mot de passe actuel
  et saisie du nouveau, le nouveau mot de passe est actif ; action soumise à la
  validation préalable de l'adresse de messagerie (UC-10 §Règles métier)

- [UC-10 A4] accéder à la procédure de suppression de compte → l'utilisateur est
  conduit vers l'écran de suppression de compte (RGPD) ; aucune suppression n'est
  déclenchée depuis cet écran
```

---

### États

```
état chargé (utilisateur authentifié, profil accessible) :
  les informations du compte s'affichent — nom d'affichage courant, niveau de compte ;
  le formulaire de modification du mot de passe est accessible si l'adresse de
  messagerie est validée

état erreur (opération sensible refusée — adresse de messagerie non validée) :
  le formulaire de modification du mot de passe indique que l'action requiert une
  adresse de messagerie validée ; la zone d'accès aux actions sensibles est visible
  mais l'action de suppression de compte est également soumise à cette précondition
  (UC-10 §Règles métier — RB-10-05)
```

---

### Accessibilité (delta)

Les garanties transversales (focus clavier visible en permanence, ordre de navigation cohérent) sont portées par le châssis applicatif (`zoning.md §S7 §Accessibilité transversale`). Cette section couvre uniquement ce qui est spécifique à l'écran Profil utilisateur.

```
annonce sans action :
  - confirmation d'enregistrement du nom d'affichage : annoncée assistivement sans que
    l'utilisateur déplace son focus — NFR-ACC-02
  - confirmation de modification du mot de passe : annoncée assistivement — NFR-ACC-02
  - message de refus (adresse de messagerie non validée) : annoncé assistivement
    sans action — NFR-ACC-02

hiérarchie de lecture à distance :
  priorité 1 — zone d'informations de compte (niveau de compte — repère rapide)
  priorité 2 — formulaire de modification du nom d'affichage
  priorité 3 — formulaire de modification du mot de passe
  priorité 4 — zone d'accès aux actions sensibles
  source : [SOUS-SPÉCIFIÉ] Aucune exigence non fonctionnelle du corpus ne couvre la
           hiérarchie de lecture visuelle hors session (NFR-ACC-04 exclut explicitement
           la phase de préparation ; NFR-ACC-01 couvre l'ordre de tabulation clavier,
           objet distinct). La hiérarchie décrite ici relève de la bonne pratique et
           attend une source.
```

---

### Sources

```
Sources : UC-10 A2 ; UC-10 A4 (accès) ; UC-10 §Règles métier (RB-10-05) ;
          NFR-ACC-02 ;
          châssis S7 (zoning.md §S7)
```

---

## Sections conditionnelles

### Hors-périmètre / différé / sous-spécifié (déclenché — éléments S8 et S9)

```
Éléments différés (S8) :
  [HORS-MVP — UC-10 A4 §Note MVP] transfert de propriété de campagne avant suppression
    de compte : hors MVP — la suppression est bloquée si l'utilisateur est propriétaire
    de campagnes avec des membres actifs ; le transfert est un mécanisme post-MVP

Éléments différés (S8) ci-dessus — aucun élément sous-spécifié résiduel.
```
