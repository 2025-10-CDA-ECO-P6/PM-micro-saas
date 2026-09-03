namespace CreerIssues;

/// <summary>
/// Reproduit l'ordre de <c>sorted()</c> côté Python : une comparaison point de
/// code par point de code, sans aucune table de collation. Forme miroir de
/// <c>VerifMailleAgregat.ComparateurCodePoints</c>, dupliquée plutôt que
/// partagée. <see cref="StringComparer"/>.Ordinal comparerait des unités
/// UTF-16 — les deux ordres divergent hors du plan multilingue de base.
/// </summary>
internal sealed class ComparateurCodePoints : IComparer<string>
{
    public static readonly ComparateurCodePoints Instance = new();

    private ComparateurCodePoints()
    {
    }

    public int Compare(string? x, string? y)
    {
        if (ReferenceEquals(x, y)) return 0;
        if (x is null) return -1;
        if (y is null) return 1;

        using var runesX = x.EnumerateRunes().GetEnumerator();
        using var runesY = y.EnumerateRunes().GetEnumerator();
        while (true)
        {
            bool aEncoreX = runesX.MoveNext();
            bool aEncoreY = runesY.MoveNext();
            if (!aEncoreX && !aEncoreY) return 0;
            if (!aEncoreX) return -1;
            if (!aEncoreY) return 1;

            int ecart = runesX.Current.Value.CompareTo(runesY.Current.Value);
            if (ecart != 0) return ecart;
        }
    }
}
