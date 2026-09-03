namespace CreerIssues;

/// <summary>
/// Signale que l'outil n'a pas pu s'exécuter parce que l'ENVIRONNEMENT est en
/// cause — <c>git</c> introuvable ou en échec, <c>gh</c> absent ou non
/// authentifié sous <c>--appliquer</c> — jamais un défaut du corpus ou de sa
/// configuration. C'est cette distinction, portée par le type de l'exception
/// et non par son message, qui sépare le code de sortie 2 du code de sortie 1
/// dans le contrat arbitré (voir <c>Program.cs</c>). Fait partie du socle
/// « lecture du plan » : <see cref="RacineDepot"/> et <see cref="OrigineGit"/>
/// la lèvent sur un échec littéral de <c>git</c>, ce que la vague suivante
/// (report vers un projet GitHub) doit pouvoir distinguer de la même façon.
/// </summary>
internal sealed class PanneEnvironnementException : Exception
{
    public PanneEnvironnementException(string message) : base(message)
    {
    }

    public PanneEnvironnementException(string message, Exception cause) : base(message, cause)
    {
    }
}
