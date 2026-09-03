// Cet outil n'est ni une tâche du plan de travail (`plan-de-travail.md §2`)
// ni un vérificateur du corpus documentaire au sens de la clause de
// `methode-de-ticket.md §7` (« l'outillage qui lit et vérifie le corpus
// documentaire ») : il LIT le plan de travail et la méthode de ticket, mais
// ne VÉRIFIE aucune propriété du corpus — il en reporte le contenu vers des
// issues GitHub, en écriture. Cette clause ne le couvre donc pas à la
// lettre ; sa désignation propre par la maille du plan (§2) reste un point
// ouvert, non tranché par le corpus actuel — signalé ici plutôt que réglé en
// silence par une citation qui ne le couvrirait pas. Des conventions de code
// C# du corpus (guide-conventions-et-dod.md §2), n'applique que les clauses
// dont l'objet nommé existe ici — le style et la version du SDK, jamais le
// modèle de domaine.

using System.Globalization;
using System.Text;
using CreerIssues;

// Passe la culture invariante au fil d'exécution entier plutôt que de la
// répéter à chaque appel individuel.
CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

// UTF-8 sans marque d'ordre d'octets, séparateur de ligne forcé à '\n' — le
// séparateur par défaut de l'environnement vaut \r\n sur Windows et suffirait
// à lui seul à faire échouer une comparaison octet à octet avec l'oracle.
var encodageSortie = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
using var sortie = new StreamWriter(Console.OpenStandardOutput(), encodageSortie) { NewLine = "\n" };
using var erreur = new StreamWriter(Console.OpenStandardError(), encodageSortie) { NewLine = "\n" };

var analyse = AnalyseurArguments.Analyser(args);

if (analyse.DemandeAide)
{
    sortie.Write(AnalyseurArguments.TexteAide);
    sortie.Flush();
    return 0;
}

if (analyse.ErreurUsage is not null)
{
    // Mode d'échec d'argparse lui-même (option inconnue, valeur manquante) —
    // reproduction non éprouvée au-delà de ces deux cas (voir Arguments.cs).
    // Code de sortie 2 : c'est celui qu'argparse rend nativement pour toute
    // erreur d'analyse de la ligne de commande, indépendamment du contrat de
    // sortie arbitré pour ce portage — les deux coïncident ici sans tension.
    erreur.Write(AnalyseurArguments.Usage);
    erreur.WriteLine($"CreerIssues: error: {analyse.ErreurUsage}");
    erreur.Flush();
    return 2;
}

try
{
    bool signalOperateur = Orchestrateur.Executer(analyse.Valeurs!, sortie);
    sortie.Flush();
    return signalOperateur ? 1 : 0;
}
catch (PanneEnvironnementException panne)
{
    sortie.Flush();
    erreur.WriteLine($"Erreur : {panne.Message}");
    erreur.Flush();
    return 2;
}
catch (ErreurCorpusException erreurCorpus)
{
    sortie.Flush();
    erreur.WriteLine($"Erreur : {erreurCorpus.Message}");
    erreur.Flush();
    return 1;
}
catch (Exception imprevue)
{
    // Filet de sécurité : toute panne non anticipée rend tout de même un
    // code de sortie du contrat arbitré plutôt qu'une trace non gérée. C'est
    // ici une ENVIRONMENT-classée par défaut faute de mieux : une panne que
    // ni la classification corpus ni la classification environnement
    // n'anticipaient n'est, par construction, pas un signal que l'opérateur
    // peut résoudre en éditant un document.
    sortie.Flush();
    erreur.WriteLine($"panne d'exécution imprévue : {imprevue.Message}");
    erreur.Flush();
    return 2;
}
