namespace VerifCorpus;

/// <summary>
/// Signale une panne d'exécution au sens du contrat de code de sortie arbitré
/// (git absent ou en échec, chemin introuvable, UTF-8 invalide) — distincte d'un
/// défaut rapporté sur le corpus lui-même. Le point d'entrée est seul à traduire
/// cette exception en code de sortie 2.
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
