# Site de documentation

## À quoi sert le site

Il publie le corpus de `docs/` dans un navigateur : naviguer de document en document, chercher en français (accents et pluriels compris), voir les diagrammes Mermaid et les maquettes HTML du corpus. Il ne vérifie rien du corpus, c'est le rôle de `tools/` ; il le met en forme.

## Pourquoi DocFX

- **Contrainte du dépôt** : pas de Python, une chaîne en C# comme `tools/`. DocFX est une bibliothèque .NET, appelée par le projet de ce dossier ; il n'y a aucun outil à installer.
- **Mesuré lors des essais du 2026-10-09** : sans aucune configuration, DocFX publie le corpus avec 0 ancre cassée et 0 liste abîmée. Mermaid est embarqué, sans dépendance externe. La construction prend quelques secondes (3 s à l'essai).
- **MkDocs, écarté** : Python, et un hook de 165 lignes pour corriger 822 ancres et 461 listes.
- **Docusaurus, écarté** : 1,2 Go de dépendances, 49 vulnérabilités signalées, construction de 70 s à 2 min 40.

## Comment ça marche

`dotnet run --project docs-site` enchaîne :

1. **Copie de travail** de `docs/` dans `docs-site/_work/docs` (`docs/` n'est jamais modifié, `docs/workstreams/` est ignoré). Elle réécrit les liens que GitHub accepte et que DocFX signale : un lien vers un dossier pointe son `README.md` ; un lien qui sort de `docs/` (ou vise un dossier sans `README.md`) pointe le dépôt sur GitHub ; dans les maquettes HTML, un lien vers un `.md` pointe le `.html` publié.
2. **Navigation** : un `toc.yml` à la racine (barre du haut) et un par dossier de premier niveau (sommaire latéral et fil d'Ariane), générés depuis l'arborescence.
3. **Construction DocFX** vers `docs-site/_site` ; le moindre avertissement fait sortir en code 1.
4. **Contrôle** des liens et des ancres du HTML construit, que DocFX ne vérifie pas ; chaque `README.html` est copié en `index.html`.

L'URL du dépôt n'est écrite qu'à un endroit : `_repoUrl` dans `docfx.json`. Le modèle français (libellés, recherche lunr en français, barre de navigation défilante) est dans `modele-fr/`.

Le [workflow](../.github/workflows/docs-site.yml) construit le site sur chaque pull request qui touche `docs/` ou `docs-site/`, et le publie sur GitHub Pages à chaque push sur `main`.

## Construire et prévisualiser en local

```bash
dotnet run --project docs-site -- --servir
```

Le site est servi sur <http://localhost:8080/>. Sans `--servir`, la commande construit et contrôle seulement.

## Entretien

- **Ajouter ou modifier un document** : rien à faire, il entre seul dans la navigation, nommé d'après son premier titre.
- **Ajouter un dossier** : rien à faire s'il a un `README.md`, dont le premier titre (sans la mention « Haversack ») nomme le dossier. Sans `README.md`, le dossier prend son nom ; ajoute une ligne dans `titres-sections.txt` seulement si ce nom a besoin d'accents ou d'une casse. Une ligne pour un dossier doté d'un `README.md`, ou disparu, fait échouer la construction.
- **Monter la version de DocFX** : change `Docfx.App` dans `SiteDocumentation.csproj`, puis construis. Le résultat attendu est 0 avertissement. Vérifie à l'œil sur une page profonde le fil d'Ariane, le sommaire latéral et la recherche : `modele-fr/token.json` (libellés) et `modele-fr/public/main.css` (barre de navigation) dépendent du modèle « modern ».
- **Changer d'organisation ou de nom du dépôt** : modifie `_repoUrl` dans `docfx.json`.
