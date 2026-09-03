namespace FauxGh;

/// Calcule le rang (0-indexé) de l'invocation courante au sein d'une même
/// exécution de l'outil sous harnais — le nombre d'entrées déjà présentes
/// dans le journal avant que cette invocation n'y ajoute la sienne. Ce rang
/// est ce qui permet à <see cref="ReponseFauxGh"/> de servir une réponse
/// différente par appel, dans l'ordre où l'outil sous harnais les émet.
///
/// Utilise toujours un fichier `JOURNAL_GH` neuf par exécution capturée :
/// un fichier réutilisé d'une capture précédente décale les rangs, et donc
/// les réponses qui en dépendent.
internal static class RangAppel
{
    public static int Calculer(string? cheminJournal)
    {
        if (string.IsNullOrEmpty(cheminJournal) || !File.Exists(cheminJournal))
        {
            return 0;
        }
        return File.ReadAllLines(cheminJournal).Count(ligne => ligne.Trim().Length > 0);
    }
}
