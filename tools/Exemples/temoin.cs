#!/usr/bin/env dotnet
// Outil témoin de la convention `tools/FICHIER-UNIQUE.md` : un fichier
// d'entrée, une source incluse, les propriétés de ses anciens `.csproj`.
// Il rend en code de sortie le premier argument (0 sans argument). À retirer
// dès qu'un outil réel suit la convention.

#:property TargetFramework=net10.0
#:property ImplicitUsings=enable
#:property Nullable=enable
#:property EnforceCodeStyleInBuild=true
#:property TreatWarningsAsErrors=true
#:property PublishAot=false
#:property RootNamespace=Temoin
#:property AssemblyName=temoin

#:include temoin/CodeDeSortie.cs

return Temoin.CodeDeSortie.Depuis(args);
