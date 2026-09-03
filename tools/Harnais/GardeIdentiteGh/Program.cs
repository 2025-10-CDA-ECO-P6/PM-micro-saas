// Cet outil n'est pas un vérificateur du corpus documentaire : c'est la
// garde d'identité du harnais de test pour le portage Python → C# de
// `tools/creer-issues.py` et `tools/creer-projet.py`. Il ne relève donc pas
// de la maille de désignation du plan de travail (`plan-de-travail.md §2`) —
// ce n'est pas lui-même une tâche du plan. Des conventions de code C# du
// corpus (guide-conventions-et-dod.md §2), n'applique que les clauses dont
// l'objet nommé existe ici — le style et la version du SDK, jamais le
// modèle de domaine.

using System.Globalization;
using System.Text;
using GardeIdentiteGh;

CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

using var sortie = new StreamWriter(Console.OpenStandardOutput(), new UTF8Encoding(encoderShouldEmitUTF8Identifier: false))
{
    NewLine = "\n",
};

// Invoque cette garde avant toute exécution sous harnais qui doit avoir un
// effet réel (--appliquer). Code de sortie non nul = refus : l'exécution qui
// devait suivre ne doit alors jamais avoir lieu, quelle qu'en soit la cause.
bool identiteEtablie = VerificateurIdentite.GhResoluEstLeFaux(sortie);
sortie.Flush();
return identiteEtablie ? 0 : 1;
