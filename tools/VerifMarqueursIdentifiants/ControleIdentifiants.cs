using System.Text.RegularExpressions;

namespace VerifMarqueursIdentifiants;

/// <summary>
/// Signale tout identifiant défini deux fois (ADR-019 § 2 : un identifiant n'a
/// qu'une origine). Une <i>définition</i> est la ligne ou le fichier qui pose
/// l'identifiant ; un renvoi qui le cite n'en est pas une. Les schémas viennent
/// du registre des identifiants de <c>docs/conventions.md</c> ; chacun est
/// apparié à la forme que prend sa définition dans le corpus :
/// <list type="bullet">
/// <item><b>nom de fichier</b> — <c>UC-05-organiser-dossiers.md</c> pose UC-05 ;</item>
/// <item><b>titre</b> — <c>### US-06-04 — …</c> ;</item>
/// <item><b>puce</b> — <c>- RB-06-25 : …</c> (le gras est toléré) ;</item>
/// <item><b>cellule</b> — première cellule d'une ligne de tableau, seule dans la cellule ;</item>
/// <item><b>gras</b> — <c>**T-04 — …**</c> en tête de ligne, suivi d'un tiret long
/// (<c>- **T-04 (reclaim)** …</c> est un renvoi).</item>
/// </list>
/// Une famille dont les définitions ne se trouvent que dans un fichier (un
/// tableau d'identifiants cité ailleurs en première cellule) est bornée à ce
/// fichier. Les familles propres à un document (H, A, E, C, S, L, #, SV, ARB,
/// journey, RUPTURE) se répètent d'un document à l'autre par construction : elles
/// ne sont pas contrôlées.
/// </summary>
internal static class ControleIdentifiants
{
    private enum Forme
    {
        NomDeFichier,
        Titre,
        Puce,
        Cellule,
        Gras,
    }

    private sealed record Famille(string Nom, string Motif, Forme Forme, string? Fichier = null);

    private static readonly Famille[] Familles =
    [
        new("persona-NN", @"persona-\d{2}", Forme.NomDeFichier),
        new("UC-NN", @"UC-\d{2}", Forme.NomDeFichier),
        new("UC-F<n>", @"UC-F\d+", Forme.Titre),
        new("US-UC-NN", @"US-UC-\d{2}", Forme.NomDeFichier),
        new("US-NN-KK", @"US-\d{2}-\d{2}", Forme.Titre),
        new("RB-NN-KK", @"RB-\d{2}-\d{2}", Forme.Puce),
        new("UJ-UC-NN", @"UJ-UC-\d{2}", Forme.NomDeFichier),
        new("NFR-<FAM>-NN", @"NFR-[A-Z0-9]+-\d{2}", Forme.NomDeFichier),
        new("AR-NN", @"AR-\d{2}", Forme.Titre),
        new("ADR-NNN", @"ADR-\d{3}", Forme.NomDeFichier),
        new("CR-UCNN-KK", @"CR-UC\d{2}-\d{2}", Forme.Cellule, "docs/assurance/strategie-de-test.md"),
        new("CR-TRANS-NN", @"CR-TRANS-\d{2}", Forme.Cellule, "docs/assurance/strategie-de-test.md"),
        new("PO-NN", @"PO-\d{2}", Forme.Cellule, "docs/assurance/strategie-de-test.md"),
        new("PR-NN", @"PR-\d{2}", Forme.Cellule, "docs/assurance/securite/registre-des-traitements.md"),
        new("IR-NN", @"IR-\d{2}", Forme.Cellule, "docs/assurance/securite/analyse-d-impact.md"),
        new("TB-NNN", @"TB-\d{3}", Forme.Titre),
        new("EP-NN", @"EP-\d{2}", Forme.Titre),
        new("LOT-NN", @"LOT-\d{2}", Forme.Titre),
        new("<FAM>-NN", @"[TPJD]-\d{2}", Forme.Gras, "docs/projet/registre-des-risques.md"),
    ];

    private const string Borne = @"(?![\w-])";

    private static readonly Regex[] MotifsDeFamille = Familles.Select(MotifDe).ToArray();

    public static List<Constat> Controler(string racine, IReadOnlyList<string> fichiers)
    {
        var definitions = new Dictionary<string, List<Constat>>(StringComparer.Ordinal);

        void Poser(string identifiant, string rel, int ligne)
        {
            if (!definitions.TryGetValue(identifiant, out var lieux))
            {
                lieux = [];
                definitions[identifiant] = lieux;
            }
            lieux.Add(new Constat(identifiant, rel, ligne, $"{identifiant} défini ici"));
        }

        foreach (var rel in fichiers)
        {
            string nomDeFichier = rel[(rel.LastIndexOf('/') + 1)..];
            for (int i = 0; i < Familles.Length; i++)
            {
                var famille = Familles[i];
                if (famille.Forme != Forme.NomDeFichier) continue;
                var m = MotifsDeFamille[i].Match(nomDeFichier);
                if (m.Success) Poser(m.Groups["id"].Value, rel, 1);
            }

            var familles = Enumerable.Range(0, Familles.Length)
                .Where(i => Familles[i].Forme != Forme.NomDeFichier
                    && (Familles[i].Fichier is null || Familles[i].Fichier == rel))
                .ToArray();
            if (familles.Length == 0) continue;

            foreach (var (numero, ligne) in CorpusMarkdown.LignesHorsCode(racine, rel))
            {
                foreach (int i in familles)
                {
                    var m = MotifsDeFamille[i].Match(ligne);
                    if (m.Success) Poser(m.Groups["id"].Value, rel, numero);
                }
            }
        }

        var constats = new List<Constat>();
        foreach (var (identifiant, lieux) in definitions.OrderBy(d => d.Key, StringComparer.Ordinal))
        {
            if (lieux.Count < 2) continue;
            foreach (var lieu in lieux)
            {
                constats.Add(lieu with { Message = $"{identifiant} défini ici ({lieux.Count} définitions)" });
            }
        }
        return constats;
    }

    private static Regex MotifDe(Famille famille)
    {
        string id = $"(?<id>{famille.Motif})";
        string motif = famille.Forme switch
        {
            Forme.NomDeFichier => $@"^{id}(?=[-.])",
            Forme.Titre => $@"^#{{1,6}}\s+(?:\*\*|`)?{id}{Borne}(?:\*\*|`)?\s*(?:—|–|:|\.|$)",
            Forme.Puce => $@"^\s*[-*+]\s+(?:\*\*)?{id}{Borne}(?:\*\*)?\s?:",
            Forme.Cellule => $@"^\s*\|\s*(?:\*\*|`)?{id}{Borne}(?:\*\*|`)?\s*\|",
            Forme.Gras => $@"^\*\*{id}\s*—",
            _ => throw new ArgumentOutOfRangeException(nameof(famille)),
        };
        return new Regex(motif, RegexOptions.CultureInvariant);
    }
}
