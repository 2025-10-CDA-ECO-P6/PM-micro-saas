# Identity & Access — Modèle Conceptuel de Données (MCD)

Le bounded context Identity & Access a un seul concept central : le compte utilisateur.
Les détails d'authentification (mot de passe, tokens) sont délégués à l'infrastructure d'identité
et ne font pas partie du modèle conceptuel de domaine.

```mermaid
erDiagram
    USER {
        uuid id PK "Identifiant unique du compte"
        string email "Unique, normalisé en minuscules"
        boolean email_verified "Adresse de messagerie vérifiée"
        string display_name "Nom d'affichage public"
        string status "ACTIVE | SUSPENDED | DELETED"
        string tier "FREE | PRO"
        datetime created_at
        datetime updated_at
    }
```

> **Note** : Le mode local (sans compte) n'est pas représenté ici —
> il n'y a pas de `User` en mode local. Les données locales vivent dans le stockage local du navigateur.

