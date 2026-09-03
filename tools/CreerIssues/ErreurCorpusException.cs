namespace CreerIssues;

/// <summary>
/// Signale que l'outil a pu s'exécuter mais rapporte quelque chose sur quoi
/// l'opérateur doit agir : corpus malformé, configuration git ne se
/// résolvant pas en propriétaire/dépôt GitHub, identifiants inconnus, révision
/// non joignable sous <c>--appliquer</c>, appel `gh` en échec. Équivalent de
/// l'exception <c>ErreurScript</c> du script Python d'origine
/// (<c>creer-issues.py</c>), dont chaque levée interrompait le script
/// proprement, sans trace Python. C'est cette exception — jamais
/// <see cref="PanneEnvironnementException"/> — que <see cref="OrigineGit"/>
/// lève quand l'URL distante ne se déduit pas en propriétaire/dépôt : `git`
/// a réussi, c'est ce qu'il a renvoyé qui ne convient pas.
/// </summary>
internal sealed class ErreurCorpusException : Exception
{
    public ErreurCorpusException(string message) : base(message)
    {
    }
}
