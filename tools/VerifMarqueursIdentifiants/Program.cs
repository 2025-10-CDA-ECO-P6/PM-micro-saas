// N'écris dans ce projet que la vérification d'une propriété du corpus documentaire.
// À ce titre il ne reçoit pas de tranche (methode-de-ticket.md §7). Des conventions de
// code C# du corpus (guide-conventions-et-dod.md §2), n'applique que les clauses dont
// l'objet nommé existe ici — le style et la version du SDK, jamais le modèle de domaine.

using System.Globalization;
using System.Text;
using VerifMarqueursIdentifiants;

CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

// Usage : VerifMarqueursIdentifiants [racine] [--reference-marqueurs chemin] [--reference-identifiants chemin]
// Sans option, les listes de référence sont celles versionnées à côté de l'outil.
string racine = ".";
string? referenceMarqueurs = null;
string? referenceIdentifiants = null;
for (int i = 0; i < args.Length; i++)
{
    switch (args[i])
    {
        case "--reference-marqueurs" when i + 1 < args.Length:
            referenceMarqueurs = args[++i];
            break;
        case "--reference-identifiants" when i + 1 < args.Length:
            referenceIdentifiants = args[++i];
            break;
        default:
            racine = args[i];
            break;
    }
}
racine = Path.GetFullPath(racine);
string dossierOutil = Path.Combine(racine, "tools", "VerifMarqueursIdentifiants");
referenceMarqueurs ??= Path.Combine(dossierOutil, "reference-marqueurs.txt");
referenceIdentifiants ??= Path.Combine(dossierOutil, "reference-identifiants.txt");

// UTF-8 sans marque d'ordre d'octets, séparateur de ligne forcé à '\n'.
using var sortie = new StreamWriter(Console.OpenStandardOutput(), new UTF8Encoding(encoderShouldEmitUTF8Identifier: false))
{
    NewLine = "\n",
};

try
{
    var fichiers = CorpusMarkdown.ListerFichiers(racine);

    bool marqueursEnEchec = Comparaison.Rapporter(
        "marqueurs hors de la liste fermée",
        ControleMarqueurs.Controler(racine, fichiers),
        ListeReference.Lire(referenceMarqueurs),
        sortie);
    bool identifiantsEnEchec = Comparaison.Rapporter(
        "identifiants définis deux fois",
        ControleIdentifiants.Controler(racine, fichiers),
        ListeReference.Lire(referenceIdentifiants),
        sortie);

    sortie.Flush();
    return marqueursEnEchec || identifiantsEnEchec ? 1 : 0;
}
catch (PanneExecutionException panne)
{
    sortie.Flush();
    Console.Error.WriteLine($"panne d'exécution : {panne.Message}");
    return 2;
}
catch (Exception imprevue)
{
    // Filet de sécurité : toute panne non anticipée rend tout de même un code de
    // sortie du contrat arbitré plutôt qu'une trace non gérée.
    sortie.Flush();
    Console.Error.WriteLine($"panne d'exécution imprévue : {imprevue.Message}");
    return 2;
}
