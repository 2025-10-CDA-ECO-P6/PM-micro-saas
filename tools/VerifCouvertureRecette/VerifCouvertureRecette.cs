#!/usr/bin/env dotnet
// N'écris dans ce projet que la vérification d'une propriété du corpus documentaire.
// À ce titre il ne reçoit pas de tranche (methode-de-ticket.md §7). Des conventions de
// code C# du corpus (guide-conventions-et-dod.md §2), n'applique que les clauses dont
// l'objet nommé existe ici — le style et la version du SDK, jamais le modèle de domaine.
// Outil de lecture seule : aucun mode d'application n'existe ni ne doit être ajouté ici.

// Propriétés de l'ancien .csproj (tools/FICHIER-UNIQUE.md). Version en support à
// long terme en cours à l'ouverture du build : règle et formulation posées par
// guide-conventions-et-dod.md §2 — « ce guide énonce la règle, jamais un numéro —
// un numéro vieillit, la règle non » — qui renvoie lui-même au §8 pour la pratique
// de fond dont elle découle. Style porté uniquement par les analyzers livrés avec
// le SDK — aucune dépendance d'analyse tierce.
#:property TargetFramework=net10.0
#:property ImplicitUsings=enable
#:property Nullable=enable
#:property EnforceCodeStyleInBuild=true
#:property TreatWarningsAsErrors=true
#:property PublishAot=false
#:property RootNamespace=VerifCouvertureRecette
#:property AssemblyName=VerifCouvertureRecette

#:include CahierRecette.cs
#:include CasClasse.cs
#:include CasRecette.cs
#:include CitationsPlan.cs
#:include Classe.cs
#:include Classificateur.cs
#:include LecteurTexte.cs
#:include PanneExecutionException.cs
#:include TableauMarkdown.cs
#:include Verdict.cs
#:include VerificateurCouvertureRecette.cs
#:include VueDesEpiques.cs

using System.Globalization;
using System.Text;
using VerifCouvertureRecette;

// Passe la culture invariante au fil d'exécution entier plutôt que de la
// répéter à chaque appel individuel.
CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

string racine = Path.GetFullPath(args.Length > 0 ? args[0] : ".");
string cheminCahier = args.Length > 1 ? args[1] : "docs/assurance/strategie-de-test.md";
string cheminPlan = args.Length > 2 ? args[2] : "docs/projet/plan-de-travail.md";

// UTF-8 sans marque d'ordre d'octets, séparateur de ligne forcé à '\n' — le
// séparateur par défaut de l'environnement vaut \r\n sur Windows et suffirait
// à lui seul à faire échouer une comparaison octet à octet avec l'oracle.
using var sortie = new StreamWriter(Console.OpenStandardOutput(), new UTF8Encoding(encoderShouldEmitUTF8Identifier: false))
{
    NewLine = "\n",
};

try
{
    var verdict = VerificateurCouvertureRecette.Executer(racine, cheminCahier, cheminPlan, sortie);
    sortie.Flush();
    return verdict.ADesDefauts ? 1 : 0;
}
catch (PanneExecutionException panne)
{
    sortie.Flush();
    Console.Error.WriteLine($"panne d'exécution : {panne.Message}");
    return 2;
}
catch (Exception imprevue)
{
    // Filet de sécurité : toute panne non anticipée (chemin introuvable,
    // UTF-8 invalide propagé depuis la lecture, etc.) rend tout de même un
    // code de sortie du contrat arbitré plutôt qu'une trace non gérée.
    sortie.Flush();
    Console.Error.WriteLine($"panne d'exécution imprévue : {imprevue.Message}");
    return 2;
}
