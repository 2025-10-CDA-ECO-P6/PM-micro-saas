using System.Reflection;
using System.Text;
using Xunit;
using Xunit.Sdk;

namespace CaracterisationOutils;

/// <summary>
/// Découverte des tests de l'assembly par réflexion : les méthodes publiques
/// <c>[Fact]</c> et <c>[Theory]</c> + <c>[InlineData]</c> des classes publiques.
/// Tout ce que la suite n'emploie pas et que ce lanceur n'exécuterait pas comme
/// xUnit (fixture, collection, <c>Skip</c>, test asynchrone, autre source de
/// données) est une erreur de découverte, jamais un test passé sous silence.
/// </summary>
internal static class DecouverteDeTests
{
    private const int LongueurMaxChaine = 50;

    public static IReadOnlyList<CasDeTest> Decouvrir(Assembly assembly)
    {
        var cas = new List<CasDeTest>();
        foreach (var classe in assembly.GetTypes().Where(t => t.IsClass && t.IsVisible && !t.IsAbstract))
        {
            var methodes = classe.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(m => m.GetCustomAttribute<FactAttribute>(inherit: true) is not null)
                .ToList();
            if (methodes.Count == 0) continue;

            RefuserCeQueLeLanceurNeSaitPasFaire(classe);
            foreach (var methode in methodes) cas.AddRange(CasDe(classe, methode));
        }
        return [.. cas.OrderBy(c => c.Nom, StringComparer.Ordinal)];
    }

    private static void RefuserCeQueLeLanceurNeSaitPasFaire(Type classe)
    {
        if (classe.GetCustomAttributes<CollectionAttribute>(inherit: true).Any()
            || classe.GetInterfaces().Any(i => i.IsGenericType
                && (i.GetGenericTypeDefinition() == typeof(IClassFixture<>)
                    || i.GetGenericTypeDefinition() == typeof(ICollectionFixture<>))))
        {
            throw new NotSupportedException($"{classe.FullName} : fixture ou collection non prise en charge par le lanceur");
        }
    }

    private static IEnumerable<CasDeTest> CasDe(Type classe, MethodInfo methode)
    {
        string nom = $"{classe.FullName}.{methode.Name}";
        var fact = methode.GetCustomAttribute<FactAttribute>(inherit: true)!;
        if (fact.Skip is not null || fact.DisplayName is not null || methode.ReturnType != typeof(void))
        {
            throw new NotSupportedException($"{nom} : Skip, DisplayName ou test non void non pris en charge par le lanceur");
        }

        if (fact is not TheoryAttribute)
        {
            if (methode.GetParameters().Length != 0)
            {
                throw new NotSupportedException($"{nom} : un [Fact] ne prend pas de paramètre");
            }
            yield return new CasDeTest(nom, classe, methode, []);
            yield break;
        }

        var sources = methode.GetCustomAttributes<DataAttribute>(inherit: true).ToList();
        if (sources.Count == 0 || sources.Any(s => s is not InlineDataAttribute))
        {
            throw new NotSupportedException($"{nom} : un [Theory] n'accepte ici que des [InlineData]");
        }
        var parametres = methode.GetParameters();
        foreach (var source in sources)
        {
            foreach (var ligne in source.GetData(methode))
            {
                if (ligne.Length != parametres.Length)
                {
                    throw new NotSupportedException($"{nom} : {ligne.Length} valeur(s) pour {parametres.Length} paramètre(s)");
                }
                var valeurs = parametres.Select((p, i) => $"{p.Name}: {Formater(ligne[i])}");
                yield return new CasDeTest($"{nom}({string.Join(", ", valeurs)})", classe, methode, ligne);
            }
        }
    }

    // Même rendu que le nom d'un cas de théorie dans `dotnet test --list-tests` :
    // chaîne entre guillemets, tronquée à 50 caractères puis suivie de « ··· ».
    private static string Formater(object? valeur) => valeur switch
    {
        null => "null",
        string s when s.Length > LongueurMaxChaine => $"\"{s[..LongueurMaxChaine]}\"···",
        string s => $"\"{s}\"",
        char c => $"'{c}'",
        IFormattable f => f.ToString(null, System.Globalization.CultureInfo.InvariantCulture),
        _ => valeur.ToString() ?? "null",
    };
}
