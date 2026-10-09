namespace VerifMarqueursIdentifiants;

/// <summary>
/// Signale une panne d'exécution (git absent ou en échec, UTF-8 invalide,
/// liste de référence illisible) — distincte d'un écart rapporté sur le corpus
/// lui-même. Le point d'entrée est seul à traduire cette exception en code de
/// sortie 2.
/// </summary>
internal sealed class PanneExecutionException : Exception
{
    public PanneExecutionException(string message) : base(message)
    {
    }

    public PanneExecutionException(string message, Exception cause) : base(message, cause)
    {
    }
}
