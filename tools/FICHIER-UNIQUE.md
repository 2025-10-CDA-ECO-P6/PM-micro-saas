# Outil en fichier unique

Convention de forme d'un outil de `tools/` sans `.csproj`. Elle garantit qu'un outil en fichier unique se compile avec les mêmes exigences que son ancien projet. Le comportement du programme lui-même ne dépend que de son code.

## SDK

`global.json`, à la racine du dépôt, exige le SDK 10.0.300 ou plus récent de la bande 10.0 (`rollForward: latestFeature`, sans préversion). Les deux workflows lisent ce fichier (`global-json-file: global.json`) : le SDK de la CI ne peut donc pas diverger du SDK local.

La version minimale est celle où `#:include` existe. Sur une machine qui n'a que 10.0.112, `dotnet` échoue net (« A compatible .NET SDK was not found ») au lieu de compiler de travers.

## Où est le fichier d'entrée

- Le fichier d'entrée est `tools/<Outil>/<Outil>.cs`. Il porte les instructions de premier niveau et rien d'autre que la déclaration de l'outil : directives, puis programme.
- Les autres sources de l'outil restent dans `tools/<Outil>/`, un fichier par type, comme avant. Aucun fichier d'une autre racine n'est inclus sans que le chemin le dise.
- Exemple de référence : `tools/VerifCorpus/VerifCorpus.cs`.
- Le fichier d'entrée ne se trouve jamais dans un dossier qui garde un `.csproj`. Ce projet compilerait le fichier avec ses instructions de premier niveau, en double.

## Comment il inclut ses autres sources

Une directive `#:include` par fichier, chemin relatif au fichier d'entrée, écrit en entier :

```csharp
#:include VerificateurCorpus.cs
```

- **Jamais de motif** (`*`, `**`) : un `#:include` par motif désactive le cache de compilation.
- **Un fichier inclus n'a pas d'instructions de premier niveau.** Seules les déclarations de types y sont permises.
- Une directive retirée fait échouer la compilation (type introuvable) : rien n'est compilé en silence sans le fichier.

## Premier caractère du fichier : le shebang

La première ligne du fichier d'entrée est `#!/usr/bin/env dotnet`. Dès qu'un fichier d'entrée a des `#:include`, l'analyseur CA2266 le demande, et la compilation échoue avec `TreatWarningsAsErrors` (mesuré sur le SDK 10.0.300). Les directives `#:` viennent après, avant le premier `using` ou la première instruction.

## Propriétés à déclarer

Un fichier unique n'hérite pas du `.csproj`. Chaque propriété qu'il portait se redéclare par `#:property`, sinon le comportement s'écarte :

| Propriété | Valeur | Pourquoi |
|---|---|---|
| `TargetFramework` | `net10.0` | même membre que les `.csproj` |
| `ImplicitUsings` | `enable` | mêmes `using` implicites |
| `Nullable` | `enable` | mêmes avertissements de nullabilité |
| `EnforceCodeStyleInBuild` | `true` | le style est contrôlé à la compilation, comme avant |
| `TreatWarningsAsErrors` | `true` | un avertissement fait échouer la compilation |
| `PublishAot` | `false` | actif par défaut dans un fichier unique ; sans cela le comportement s'écarte du `.csproj` |
| `RootNamespace` | celui du `.csproj` | |
| `AssemblyName` | celui du `.csproj` (par exemple `gh` pour FauxGh) | sinon le nom de l'exécutable est celui du fichier |

`tools/.editorconfig` s'applique tel quel aux fichiers uniques : ils se trouvent sous `tools/`.

## Lancer et compiler

- Lancer : `dotnet run --file tools/<Outil>/<Outil>.cs -- <arguments>`. Toujours `--file` : si un `.csproj` se trouve dans le répertoire courant, `dotnet run <fichier>.cs` lance le projet à la place.
- Compiler une fois : `dotnet build tools/<Outil>/<Outil>.cs -o <dossier>` produit un exécutable natif. Plusieurs lancements parallèles de `dotnet run` sur le même fichier se disputent les sorties de compilation ; compiler d'abord, puis lancer l'exécutable.
- Le code de sortie de `dotnet run --file` est celui que renvoie le programme.

## Ce que la convention ne couvre pas

Elle fixe la forme, pas le partage de code entre outils. Deux fichiers de même nom dans deux outils ne sont pas forcément identiques : avant de les fusionner, on compare leur comportement, car une fusion peut changer ce que fait l'un des outils.
