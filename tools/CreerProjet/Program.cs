// Cet outil n'est ni une tâche du plan de travail (`plan-de-travail.md §2`)
// ni un vérificateur du corpus documentaire au sens de la clause de
// `methode-de-ticket.md §7` (« l'outillage qui lit et vérifie le corpus
// documentaire ») : il LIT le plan de travail et la méthode de ticket, mais
// ne VÉRIFIE aucune propriété du corpus — il en reporte le contenu vers un
// tableau de projet GitHub (Projects v2), en écriture, exactement comme
// `CreerIssues` le reporte vers des issues. Cette clause ne le couvre
// donc pas à la lettre ; sa désignation propre par la maille du plan (§2)
// reste un point ouvert, non tranché par le corpus actuel — signalé ici
// plutôt que réglé en silence par une citation qui ne le couvrirait pas. Des
// conventions de code C# du corpus (guide-conventions-et-dod.md §2),
// n'applique que les clauses dont l'objet nommé existe ici — le style et la
// version du SDK, jamais le modèle de domaine.

using System.Globalization;
using System.Text;
using CreerIssues;
using CreerProjet;

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
    // Mode d'échec d'argparse lui-même (option inconnue) — reproduction non
    // éprouvée au-delà de ce seul cas (voir Arguments.cs) ; `--appliquer` est
    // un drapeau (store_true) et ne porte donc pas le second cas de
    // `CreerIssues` (valeur manquante sur une option qui en attend une).
    // Code de sortie 2 : c'est celui qu'argparse rend nativement pour toute
    // erreur d'analyse de la ligne de commande, indépendamment du contrat de
    // sortie arbitré pour ce portage — les deux coïncident ici sans tension.
    erreur.Write(AnalyseurArguments.Usage);
    erreur.WriteLine($"CreerProjet: error: {analyse.ErreurUsage}");
    erreur.Flush();
    return 2;
}

try
{
    return Orchestrateur.Executer(analyse.Valeurs!.Appliquer, sortie);
}
catch (PanneEnvironnementException panne)
{
    // Pas de préfixe « Erreur : » : à la différence du script Python
    // d'origine de CreerIssues (creer-issues.py), qui encapsulait ses échecs
    // dans une exception `ErreurScript` explicitement rattrapée et préfixée
    // (`print(f"Erreur : {erreur}", file=sys.stderr)`), le script Python
    // d'origine de cet outil (creer-projet.py) levait `SystemExit(message)`
    // NU à chaque point d'échec — jamais intercepté. Le message rendu ici est
    // donc celui, littéral, que l'interpréteur Python écrivait pour ce
    // SystemExit non rattrapé (sans trace, sans préfixe), là où ce script
    // Python d'origine avait un équivalent (`gh()`/`graphql()`) ; les pannes
    // propres au portage (git introuvable, sans aucun équivalent Python à
    // l'identique) suivent le même style, par cohérence plutôt que par
    // fidélité mesurée.
    sortie.Flush();
    erreur.WriteLine(panne.Message);
    erreur.Flush();
    return 2;
}
catch (ErreurCorpusException erreurCorpus)
{
    // Pas de préfixe non plus — voir la note ci-dessus : ce message reproduit
    // le texte exact du `SystemExit(...)` Python correspondant.
    sortie.Flush();
    erreur.WriteLine(erreurCorpus.Message);
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
    // peut résoudre en éditant un document. Le script Python d'origine
    // (`creer-projet.py`) n'avait, lui, aucune de ces deux classes : une
    // trace Python non interceptée rendait toujours 1 — c'est ici la branche
    // la plus large où ce portage diverge du Python d'origine (voir le
    // rapport de portage, code de sortie §5).
    sortie.Flush();
    erreur.WriteLine($"panne d'exécution imprévue : {imprevue.Message}");
    erreur.Flush();
    return 2;
}
