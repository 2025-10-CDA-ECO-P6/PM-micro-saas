# Recherche en préparation

> Fiche de description d'écran basse-fidélité — préparation espace partagé.
> Instancie le gabarit `docs/conception/interface/gabarit-ecran.md`.
> Notation et nommage : `docs/conception/interface/conventions-wireframe.md`.
> Arbitrages figés : `docs/conception/interface/zoning.md §S6`.

---

## En-tête de fiche

```
Nom          : Recherche en préparation
Surface      : MJ
Contexte     : préparation
Type d'espace: CAMPAIGN | ONE_SHOT | PERSONAL (disponible pour tous les types d'espace — UC-14 §Contexte)
Forme cible  : grand écran + tablette (mobile : pensé dans la structure, non implémenté au MVP — AR-07)
Traçabilité  : UC-14 ; US-UC-14
```

---

## Noyau obligatoire

### Intention

```
Intention : le MJ retrouve rapidement un document dans son espace par son titre,
            depuis la barre de recherche omniprésente, sans avoir à naviguer
            dans l'arborescence des dossiers.
```

---

### Zones et hiérarchie

```
[ BARRE DE RECHERCHE ]
  type     : châssis
  rôle     : zone de saisie de la recherche — omniprésente dans la surface de
             préparation, accessible depuis n'importe quel état de navigation ;
             la recherche porte sur le titre des documents de l'espace actif
             uniquement (titre seul au MVP — UC-14 scénario nominal)
  priorité : co-présent-jamais-masqué
  visibilité : MJ seul
  ancrage  : UC-14 ; AR-11 (accélérateur omniprésent)

[ ZONE DE RÉSULTATS ]
  type     : latéral
  rôle     : affiche les résultats correspondant à la saisie, regroupés par type
             de document ; apparaît à la saisie, se ferme à la fermeture explicite
             ou à la sélection d'un résultat ; n'interrompt pas le contexte de
             navigation courant
  priorité : secondaire-configurable
  visibilité : MJ seul
  ancrage  : UC-14 scénario nominal (regroupement par type) ; AR-11 (non disruptif)
```

---

### Ce que l'utilisateur peut faire

```
- [UC-14 scénario nominal] rechercher par titre → le MJ saisit un mot-clé ;
  les résultats correspondant au titre des documents de l'espace actif s'affichent
  dans la zone de résultats, regroupés par type de document

- [UC-14 scénario nominal §5] sélectionner un résultat → le document sélectionné
  s'ouvre dans l'éditeur ; la zone de résultats se ferme

- [UC-14 A2] filtrer par type de document → le MJ réduit les résultats à un type
  donné parmi les types disponibles dans la zone de résultats

- [UC-14 A1] recevoir un état vide → si aucun titre ne correspond à la saisie,
  la zone de résultats indique l'absence de résultat et propose de créer un
  document ou de modifier la saisie
```

---

### États

```
état vide (aucune saisie) :
  la barre de recherche est visible et accessible ; la zone de résultats
  n'est pas affichée

état chargé (saisie en cours, résultats disponibles) :
  la zone de résultats s'affiche avec les documents correspondant au titre saisi,
  regroupés par type ; les types disponibles au MVP sont les types natifs de documents
  définis par le domaine (`docs/conception/domain/content-library.md §DocumentType —
  types système built-in`) plus un groupe « sans type » pour les documents libres
  (UC-14 scénario nominal §4)

état vide de résultats (saisie sans correspondance) :
  la zone de résultats s'affiche avec un état vide explicite ;
  invite à créer un document ou à modifier la saisie (UC-14 A1)
```

---

### Accessibilité (delta)

Les garanties transversales (focus clavier visible en permanence, ordre de navigation cohérent) sont portées par le châssis applicatif (`zoning.md §S7 §Accessibilité transversale`). Cette section couvre uniquement ce qui est spécifique à la recherche en préparation.

```
annonce sans action :
  - apparition de la zone de résultats à la saisie : annoncé assistivement
    (nombre de résultats, regroupement par type) — NFR-ACC-02
  - mise à jour des résultats au fil de la saisie : annoncé assistivement
    sans déplacement de focus — NFR-ACC-02
  - état vide (aucun résultat) : annoncé assistivement — NFR-ACC-02

hiérarchie de lecture à distance :
  priorité 1 — barre de recherche (saisie du mot-clé)
  priorité 2 — zone de résultats (résultats regroupés par type)
  source : [SOUS-SPÉCIFIÉ] Aucune exigence non fonctionnelle du corpus ne couvre la
           hiérarchie de lecture visuelle hors session (NFR-ACC-04 exclut explicitement
           la phase de préparation ; NFR-ACC-01 couvre l'ordre de tabulation clavier,
           objet distinct). La hiérarchie décrite ici relève de la bonne pratique et
           attend une source.
```

---

### Sources

```
Sources : UC-14 scénario nominal, A1, A2 ; US-UC-14 ;
          AR-11 ;
          NFR-ACC-02 ;
          châssis S7 (zoning.md §S7)
```

---

## Sections conditionnelles

### Mode local (déclenché — surface MJ)

```
Bandeaux présents (châssis standard — voir zoning.md §S7 §Châssis mode local) :
  - bandeau de durabilité : non bloquant ; signale que les données locales ne
    bénéficient pas d'une garantie de conservation permanente ;
    propose la création d'un compte
  - bandeau de confidentialité : non bloquant ; signale l'absence de protection
    par identifiants ; propose la création d'un compte

Châssis standard — voir S7 §Châssis mode local.
La recherche est disponible en mode local :
  la recherche par titre dans l'espace local ne nécessite pas de connexion
  au service cloud.
  Source : AR-13 ; UC-01 §Règles métier.
```

---

### Hors-périmètre / différé / sous-spécifié (déclenché — éléments S8 et S9)

```
Éléments différés (S8) :
  [HORS-MVP — UC-14 §Scénarios alternatifs A3] recherche par tag — Could Have ;
    non implémentée au MVP
  [HORS-MVP — UC-14 §Scénarios alternatifs A3] recherche par contenu des blocs —
    Could Have ; la correspondance au MVP porte exclusivement sur le titre
  [HORS-MVP — UC-14 scénario nominal §4] types personnalisés dans les groupes
    de résultats — les documents de types créés par le MJ (« Objets », « Factions »,
    etc.) apparaissent dans le groupe « sans type » au MVP ; les types personnalisés
    sont post-MVP (Could Have)
```
