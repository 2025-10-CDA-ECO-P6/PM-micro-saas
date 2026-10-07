---
date: 2026-10-07
auteur: architect
paquet: ADR-020, ADR-021, proposition condor § 5.10 et § 9.9, en-têtes de conform.sh et de conform-verify.sh, docs/corpus.md, index du backlog, quatre faits observés
revérifié: conform-verify.sh:19-22, labels:992 et 1030, corpus.sh:750, sections [À DÉCIDER] des ADR-020 et 021 : relus par l'orchestrateur le 2026-10-07
---

# Revue du chemin de conformité

**Verdict.** Le chemin tient, à trois conditions : séparer, pour chaque couche, les gabarits et les entrées du backlog en deux commits ; faire la réécriture de la sélection et les déplacements dans un même commit ; écrire une table ancien → nouveau explicite avant tout déplacement.

## Risques, du plus grave au moins grave

1. **Contenu invérifiable.** Mêler gabarits et entrées de backlog dans un même commit rend conform-verify inopérant : il n'excuse que les titres renommés, les chemins réécrits et les replis (`.claude/skills/onboard/conform-verify.sh:19`), alors que plusieurs entrées changent la substance (0024, 0036). Le commit de départ est épinglé une seule fois.
2. **Contenu au mauvais endroit.** `conform.sh plan --map` envoie `docs/conception/besoin/user-stories/README.md` vers `docs/securite/conformite/README.md`. Il aplatit aussi les fiches écran et leur fait perdre le niveau surface.
3. **Arborescence cible sous-spécifiée.** Le catalogue range conventions-dod et roadmap-produit dans `docs/pilotage/` (`.claude/corpus/labels:992`, `:1030`), ce que l'ADR-020 ne prévoit pas. Il garde aussi `docs/projet/` à plat. Par ailleurs, `.claude/hooks/lib/corpus.sh:750` code en dur `docs/project/backlog`.
4. **Écart entre la sélection réécrite et les déplacements.** `corpus.sh` résout les dossiers depuis la sélection : entre les deux, les écritures iraient vers des dossiers inexistants.
5. **Vérification auto-certifiée.** Les golden masters portent sur le corpus réel ; il faut exiger que leur différence soit exactement la substitution des chemins.
6. **Travail perdu.** L'entrée 0076 aligne `decisions-en-attente.md`, que F5 transforme ensuite en renvoi.

## Tâches manquantes relevées

- une table ancien → nouveau relue, domaine compris ;
- la correction des défauts de `conform.sh` ;
- une décision sur `docs/project/backlog` face à `docs/projet/` ;
- la régénération des règles `.claude/rules/corpus-*.md` ;
- un commit de référence portant les compteurs ;
- le consentement au renouvellement des issues GitHub ;
- la conservation du plan complet ;
- le remplissage des sections laissées ouvertes des ADR-020 et 021.
