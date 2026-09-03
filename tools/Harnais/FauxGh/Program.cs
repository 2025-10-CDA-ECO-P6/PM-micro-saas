// Cet outil n'est pas un vérificateur du corpus documentaire : c'est un
// harnais de test qui remplace `gh` pour capturer, sans jamais contacter
// GitHub, ce que le portage Python → C# de `tools/creer-issues.py` et
// `tools/creer-projet.py` lui demande. Il ne relève donc pas de la maille de
// désignation du plan de travail (`plan-de-travail.md §2`) — ce n'est pas
// lui-même une tâche du plan, c'est l'outillage qui prépare la vérification
// d'une tâche à venir. Des conventions de code C# du corpus
// (guide-conventions-et-dod.md §2), n'applique que les clauses dont l'objet
// nommé existe ici — le style et la version du SDK, jamais le modèle de
// domaine.

using System.Globalization;
using System.Text;
using FauxGh;

// Garde d'identité (voir `GardeIdentiteGh`) : sur cet unique argument
// sentinelle, rend une réponse fixe que le vrai gh ne peut pas produire, et
// ne l'écrit jamais dans le journal — ce n'est pas une invocation de l'outil
// sous harnais, c'est la vérification qui doit précéder toute exécution
// réelle sous ce harnais. Traité avant tout le reste : cette réponse ne doit
// dépendre d'aucun état (variable d'environnement, entrée standard) que le
// reste du programme pourrait faire varier.
if (args.Length == 1 && args[0] == IdentiteSentinelle.Argument)
{
    Console.Out.Write(IdentiteSentinelle.Reponse);
    return 0;
}

// Passe la culture invariante au fil d'exécution entier plutôt que de la
// répéter à chaque appel individuel.
CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

// UTF-8 sans marque d'ordre d'octets, séparateur de ligne forcé à '\n' — un
// outil appelant peut recevoir sur cette sortie une charge JSON portant des
// caractères non-ASCII (ex. les futurs appels GraphQL de `creer-projet.py`).
using var sortie = new StreamWriter(Console.OpenStandardOutput(), new UTF8Encoding(encoderShouldEmitUTF8Identifier: false))
{
    NewLine = "\n",
};

string entreeStandard = LecteurEntreeStandard.Lire();
string? cheminJournal = Environment.GetEnvironmentVariable("JOURNAL_GH");

// Rang lu AVANT l'ajout de la ligne de cette invocation : le rang d'un appel
// est le nombre d'entrées déjà présentes quand il arrive, jamais compté
// après coup — sinon le premier appel se compterait lui-même.
int rang = RangAppel.Calculer(cheminJournal);

try
{
    JournalInvocations.Enregistrer(cheminJournal, args, entreeStandard);
}
catch (Exception erreur)
{
    Console.Error.WriteLine($"faux gh : échec d'écriture du journal : {erreur.Message}");
    return 3;
}

string reponse;
try
{
    reponse = ReponseFauxGh.Resoudre(rang);
}
catch (Exception erreur)
{
    Console.Error.WriteLine($"faux gh : réponse mal configurée : {erreur.Message}");
    return 4;
}

sortie.Write(reponse);
sortie.Flush();

// Code de sortie contrôlable, pour exercer plus tard les branches d'échec de
// l'outil appelant sans changer une ligne de ce faux gh.
string codeBrut = Environment.GetEnvironmentVariable("FAUX_GH_CODE") ?? "0";
return int.TryParse(codeBrut, NumberStyles.Integer, CultureInfo.InvariantCulture, out int code) ? code : 0;
