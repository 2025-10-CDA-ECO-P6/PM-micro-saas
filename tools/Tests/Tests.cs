#!/usr/bin/env dotnet
// Suite de caractérisation des outils de `tools/` : lanceur maison (découverte,
// exécution en série, compte, code de sortie) qui garde les attributs et les
// Assert de xUnit, chargés par #:package — sans le moteur d'exécution de xUnit.
// Les tests lancent les outils comme des processus : le lanceur les compile
// d'abord, une fois, dans tools/Tests/bin/outils/ (CompilationDesOutils.cs).
//
//   dotnet run --file tools/Tests/Tests.cs              lance toute la suite
//   dotnet run --file tools/Tests/Tests.cs -- --lister  liste les noms, sans rien lancer

// Propriétés de l'ancien .csproj (tools/FICHIER-UNIQUE.md).
#:property TargetFramework=net10.0
#:property ImplicitUsings=enable
#:property Nullable=enable
#:property EnforceCodeStyleInBuild=true
#:property TreatWarningsAsErrors=true
#:property PublishAot=false
#:property RootNamespace=CaracterisationOutils
#:property AssemblyName=CaracterisationOutils

#:package xunit@2.9.3

#:include CasDeTest.cs
#:include CompilationDesOutils.cs
#:include DecouverteDeTests.cs
#:include ExecutionDeTests.cs
#:include Helpers.cs
#:include LanceurDeTests.cs
#:include CreerIssuesTests.cs
#:include CreerProjetTests.cs
#:include DivergencesEntreOutilsTests.cs
#:include VerifCorpusRenvoisSectionTests.cs
#:include VerifCorpusTests.cs
#:include VerifCouvertureRecetteTests.cs
#:include VerifMailleAgregatTests.cs
#:include VerifMarqueursIdentifiantsTests.cs

using CaracterisationOutils;

return LanceurDeTests.Executer(args);
