using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using CreerIssues;

namespace CreerProjet;

/// <summary>
/// Appel GraphQL via <c>gh api graphql --input -</c> : la charge passe sur
/// l'ENTRÉE STANDARD du sous-processus, jamais en argument — le seul point de
/// cet outil (et de tout l'outillage C# du dépôt) qui écrit sur l'entrée
/// standard de `gh`. Reproduit <c>graphql(requete, variables)</c> du script
/// Python d'origine (<c>creer-projet.py</c>). La forme de l'échappement JSON
/// envoyé (points de
/// code non-ASCII échappés en <c>\uXXXX</c> par <see cref="JsonNode.ToJsonString()"/>
/// plutôt que passés tels quels comme le ferait <c>json.dumps(ensure_ascii=False)</c>)
/// est sans conséquence pour le critère de différentiel de journal : le faux
/// `gh` du harnais capture l'entrée standard littérale, et
/// <c>ComparateurJournaux</c> décode puis compare la charge JSON plutôt que
/// le texte brut — deux échappements distincts d'une même valeur ne s'y
/// distinguent jamais.
/// </summary>
internal static class Graphql
{
    private static readonly UTF8Encoding Utf8SansMarque = new(encoderShouldEmitUTF8Identifier: false);

    public static JsonElement Executer(string requete, JsonObject variables)
    {
        var charge = new JsonObject { ["query"] = requete, ["variables"] = variables };
        string texteCharge = charge.ToJsonString();

        var depart = new ProcessStartInfo("gh")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            RedirectStandardInput = true,
            StandardInputEncoding = Utf8SansMarque,
            UseShellExecute = false,
        };
        foreach (var a in new[] { "api", "graphql", "--input", "-" }) depart.ArgumentList.Add(a);

        Process processus;
        try
        {
            processus = Process.Start(depart)
                ?? throw new PanneEnvironnementException("« gh » n'a pas pu être démarré");
        }
        catch (Win32Exception cause)
        {
            throw new PanneEnvironnementException("« gh » est introuvable sur ce système", cause);
        }

        using (processus)
        {
            var tacheSortie = processus.StandardOutput.ReadToEndAsync();
            var tacheErreur = processus.StandardError.ReadToEndAsync();

            // Écrit la charge puis referme aussitôt l'entrée standard : un
            // `gh` qui attend la fin de flux sur `--input -` resterait bloqué
            // sinon — exactement le piège mesuré sur le faux gh du harnais
            // (fermer l'entrée standard, jamais la laisser ouverte).
            processus.StandardInput.Write(texteCharge);
            processus.StandardInput.Close();

            processus.WaitForExit();
            string sortie = tacheSortie.GetAwaiter().GetResult();
            string erreur = tacheErreur.GetAwaiter().GetResult();

            if (processus.ExitCode != 0)
            {
                throw new ErreurCorpusException($"GraphQL a échoué : {TexteUnicode.StripPython(erreur)}");
            }

            using var document = JsonDocument.Parse(sortie);
            var racine = document.RootElement;
            if (racine.TryGetProperty("errors", out var erreurs))
            {
                throw new ErreurCorpusException(
                    $"GraphQL a renvoyé une erreur : {RenduPythonDeJson.Rendre(erreurs)}");
            }
            return racine.GetProperty("data").Clone();
        }
    }
}
