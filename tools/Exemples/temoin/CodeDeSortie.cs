using System.Globalization;

namespace Temoin;

internal static class CodeDeSortie
{
    // Premier argument lu comme entier en culture invariante ; 0 sans
    // argument, 2 si l'argument n'est pas un entier.
    internal static int Depuis(string[] arguments)
    {
        if (arguments.Length == 0)
        {
            return 0;
        }

        return int.TryParse(arguments[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out var code) ? code : 2;
    }
}
