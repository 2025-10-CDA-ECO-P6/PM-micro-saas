namespace CreerIssues;

/// <summary>
/// Accumule les créations, les éléments déjà présents et les échecs d'une
/// des trois opérations (libellés, jalons, issues), puis les affiche sur le
/// <see cref="TextWriter"/> fourni — jamais directement sur la console.
/// </summary>
internal sealed class CompteRendu
{
    private readonly List<string> _crees = new();
    private readonly List<string> _existants = new();
    private readonly List<(string Libelle, string Raison)> _echecs = new();

    public int Echecs => _echecs.Count;

    public void Creation(string libelle) => _crees.Add(libelle);
    public void Existant(string libelle) => _existants.Add(libelle);
    public void Echec(string libelle, string raison) => _echecs.Add((libelle, raison));

    public void Afficher(TextWriter sortie, string titre)
    {
        sortie.WriteLine($"\n{titre}");
        sortie.WriteLine($"  créés       : {_crees.Count}");
        foreach (var e in _crees) sortie.WriteLine($"    + {e}");
        sortie.WriteLine($"  déjà présents : {_existants.Count}");
        foreach (var e in _existants) sortie.WriteLine($"    = {e}");
        if (_echecs.Count > 0)
        {
            sortie.WriteLine($"  échecs      : {_echecs.Count}");
            foreach (var (e, raison) in _echecs) sortie.WriteLine($"    ! {e} — {raison}");
        }
    }
}
