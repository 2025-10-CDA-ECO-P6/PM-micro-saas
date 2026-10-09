using System.Reflection;

namespace CaracterisationOutils;

/// <summary>
/// Un test à exécuter : une méthode <c>[Fact]</c>, ou une ligne
/// <c>[InlineData]</c> d'une méthode <c>[Theory]</c>. <paramref name="Nom"/> est
/// le nom complet <c>Espace.Classe.Méthode</c>, suivi de
/// <c>(paramètre: valeur, …)</c> pour un cas de théorie.
/// </summary>
internal sealed record CasDeTest(string Nom, Type Classe, MethodInfo Methode, object?[] Arguments);
