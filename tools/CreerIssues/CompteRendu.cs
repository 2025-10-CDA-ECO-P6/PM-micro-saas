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
    private readonly List<string> _misAJour = new();
    private readonly List<(string Libelle, string Raison)> _echecs = new();

    public int Echecs => _echecs.Count;

    public void Creation(string libelle) => _crees.Add(libelle);
    public void Existant(string libelle) => _existants.Add(libelle);

    // Distinct de Creation (rien n'est ouvert sur GitHub) et d'Existant (une
    // écriture a bien lieu, ou aurait lieu sous --appliquer) : seule
    // l'opération 3 (issues) appelle cette méthode, quand le corps engendré
    // d'une issue déjà existante diverge de son corps actuel. N'est affiché
    // que si au moins un appelant s'en sert (même convention que les échecs
    // ci-dessous), pour ne rien changer à l'affichage des libellés et des
    // jalons, qui ne l'utilisent jamais.
    public void MiseAJour(string libelle) => _misAJour.Add(libelle);
    public void Echec(string libelle, string raison) => _echecs.Add((libelle, raison));

    public void Afficher(TextWriter sortie, string titre)
    {
        sortie.WriteLine($"\n{titre}");
        sortie.WriteLine($"  créés       : {_crees.Count}");
        foreach (var e in _crees) sortie.WriteLine($"    + {e}");
        sortie.WriteLine($"  déjà présents : {_existants.Count}");
        foreach (var e in _existants) sortie.WriteLine($"    = {e}");
        if (_misAJour.Count > 0)
        {
            sortie.WriteLine($"  mis à jour  : {_misAJour.Count}");
            foreach (var e in _misAJour) sortie.WriteLine($"    ~ {e}");
        }
        if (_echecs.Count > 0)
        {
            sortie.WriteLine($"  échecs      : {_echecs.Count}");
            foreach (var (e, raison) in _echecs) sortie.WriteLine($"    ! {e} — {raison}");
        }
    }
}
