// N'écris dans ce projet que la vérification d'une propriété du corpus documentaire.
// À ce titre il ne reçoit pas de tranche (methode-de-ticket.md §7). Des conventions de
// code C# du corpus (guide-conventions-et-dod.md §2), n'applique que les clauses dont
// l'objet nommé existe ici — le style et la version du SDK, jamais le modèle de domaine.
// Outil de lecture seule : aucun mode d'application n'existe ni ne doit être ajouté ici.

using System.Globalization;
using System.Text;
using VerifCouvertureRecette;

// Passe la culture invariante au fil d'exécution entier plutôt que de la
// répéter à chaque appel individuel.
CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

string racine = Path.GetFullPath(args.Length > 0 ? args[0] : ".");
string cheminCahier = args.Length > 1 ? args[1] : "docs/test/cahier-strategie-test-et-recette.md";
string cheminPlan = args.Length > 2 ? args[2] : "docs/gestion-projet/plan-de-travail.md";

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
