using System.Text;
using System.Text.RegularExpressions;

namespace VerifCorpus;

/// <summary>
/// Renvoi « § section » repéré dans un document : le fichier source est porté par
/// l'appelant, la cible est le chemin tel qu'écrit dans le lien (sans ancre).
/// <c>Section</c> est la forme affichée (le numéro seul quand la section en commence
/// par un) ; <c>Texte</c> est la section telle qu'écrite, qui sert à l'appariement.
/// </summary>
internal sealed record RenvoiSection(int Ligne, string Cible, string Section, string Texte);

/// <summary>
/// Contrôle des renvois « § section » (ADR-019, § 4). Un renvoi contrôlable est un lien
/// Markdown dont le texte porte « § &lt;section&gt; », ou que « § &lt;section&gt; » suit
/// immédiatement sur la même ligne. La section est appariée aux titres ATX et aux
/// libellés en gras de la cible, jamais aux seuls titres : un renvoi vers un libellé en
/// gras est juste. Un numéro seul désigne le titre ou le libellé qui commence par ce
/// numéro.
/// </summary>
internal static class RenvoisSection
{
    // Numéro en tête de section : « 8.4 », « 5bis », « A4 ». Le garde-fou final refuse
    // « 5 » devant « 5.1 » ou « 5bis » : un numéro n'est pas le préfixe d'un autre.
    private static readonly Regex MotifNumero = new(
        @"^[A-Za-z]?\d+(?:\.[0-9A-Za-z]+)*[a-z]*(?![\p{L}\p{N}]|\.[\p{L}\p{N}])",
        RegexOptions.CultureInvariant);

    private static readonly Regex MotifGras = new(
        @"\*\*(.+?)\*\*",
        RegexOptions.CultureInvariant);

    private static readonly Regex MotifAtx = new(
        @"^#{1,6}\s+(.*?)\s*#*\s*$",
        RegexOptions.CultureInvariant);

    private static readonly Regex MotifLienMarkdown = new(
        @"\[([^\]]*)\]\([^)]*\)",
        RegexOptions.CultureInvariant);

    // « § » collé à la fin du lien, à trois caractères de séparation près.
    private static readonly Regex MotifApresLien = new(
        @"\G[\s,`]{0,3}§(?<section>[^\n]*)",
        RegexOptions.CultureInvariant);

    // Numéro et ponctuation qui précèdent le titre d'une section numérotée : « 11. », « 8.4 ».
    private static readonly Regex MotifNumeroDeTitre = new(
        @"^[a-z]?\d+(?:\.[0-9a-z]+)*[a-z]*[.)]?\s+(?:—\s+)?",
        RegexOptions.CultureInvariant);

    private static readonly Regex MotifEspaces = new(
        @"\s+",
        RegexOptions.CultureInvariant);

    private static readonly string[] Separateurs = [" — ", " – ", " - ", " : ", " (", ", ", "; ", ". "];

    // Ce qui entoure une section citée sans en faire partie : blancs, ponctuation de fin
    // de phrase, guillemets, accent grave.
    private static readonly char[] CaracteresDeBord = [' ', '\t', '.', ',', ';', ':', '`', '«', '»', '"', '\''];

    /// <summary>
    /// Repère les renvois « § » d'un texte dont les blocs de code ont été vidés de leur
    /// contenu sans perdre leurs lignes : le numéro de ligne rendu est celui du fichier.
    /// Les liens à schéma externe sont écartés ; l'existence du fichier cible est
    /// l'affaire de l'appelant.
    /// </summary>
    public static List<RenvoiSection> Reperer(string texteLignesPreservees, Regex motifLien, Regex motifSchemeExterne)
    {
        var renvois = new List<RenvoiSection>();
        foreach (Match m in motifLien.Matches(texteLignesPreservees))
        {
            string cible = m.Groups[1].Value;
            if (motifSchemeExterne.IsMatch(cible)) continue;

            int diese = cible.IndexOf('#');
            string chemin = diese < 0 ? cible : cible[..diese];

            string? brute = ExtraireSection(m, texteLignesPreservees);
            if (brute is null) continue;

            string texte = brute.Trim(CaracteresDeBord);
            if (texte.Length == 0) continue;
            string section = SectionPourAffichage(texte);

            int ligne = 1 + texteLignesPreservees.AsSpan(0, m.Index).Count('\n');
            renvois.Add(new RenvoiSection(ligne, chemin, section, texte));
        }
        return renvois;
    }

    private static string? ExtraireSection(Match lien, string texte)
    {
        string texteDuLien = lien.Groups["texte"].Value;
        int marque = texteDuLien.IndexOf('§');
        if (marque >= 0) return texteDuLien[(marque + 1)..];

        var apres = MotifApresLien.Match(texte, lien.Index + lien.Length);
        if (!apres.Success) return null;

        // Hors du texte du lien, la section n'a pas de borne : on la coupe avant la
        // ponctuation qui ouvre une parenthèse, une énumération ou un autre lien.
        string reste = apres.Groups["section"].Value;
        int coupure = reste.IndexOfAny([',', ';', '(', ')', '|', '`', '[']);
        return coupure < 0 ? reste : reste[..coupure];
    }

    /// <summary>
    /// Forme affichée et clé de la liste de référence : le numéro seul quand la section
    /// en commence par un, sinon le texte.
    /// </summary>
    private static string SectionPourAffichage(string texte)
    {
        var numero = MotifNumero.Match(texte);
        return numero.Success ? numero.Value : texte;
    }

    /// <summary>
    /// Sections d'un document : titres ATX et libellés en gras, hors blocs de code,
    /// normalisés pour l'appariement.
    /// </summary>
    public static List<string> Indexer(string texteSansCode)
    {
        var index = new List<string>();
        foreach (string ligne in texteSansCode.Split('\n'))
        {
            var atx = MotifAtx.Match(ligne);
            if (atx.Success) Ajouter(index, atx.Groups[1].Value);
            foreach (Match gras in MotifGras.Matches(ligne)) Ajouter(index, gras.Groups[1].Value);
        }
        return index;
    }

    // Une section se cite aussi par son titre sans son numéro (« 11. Matrice » citée
    // « Matrice ») ou par la partie qui suit le tiret long (« 8. Domaine — Conventions »
    // citée « Conventions »). L'entrée complète reste indexée : un numéro seul s'apparie
    // dessus.
    private static void Ajouter(List<string> index, string brut)
    {
        string n = Normaliser(brut);
        if (n.Length == 0) return;
        index.Add(n);

        string sansNumero = MotifNumeroDeTitre.Replace(n, "", 1);
        if (sansNumero.Length > 0 && sansNumero != n) index.Add(sansNumero);

        int tiret = sansNumero.IndexOf(" — ", StringComparison.Ordinal);
        if (tiret > 0) index.Add(sansNumero[(tiret + 3)..].Trim(CaracteresDeBord));
    }

    private static string Normaliser(string texte)
    {
        string t = MotifLienMarkdown.Replace(texte, "$1");
        t = t.Replace('’', '\'');
        var sb = new StringBuilder(t.Length);
        foreach (char c in t)
        {
            if (c is '*' or '`' or '~' or '«' or '»' or '"') continue;
            sb.Append(c);
        }
        t = MotifEspaces.Replace(sb.ToString(), " ").Trim(CaracteresDeBord);
        return TexteUnicode.AbaisserCasseComplet(t).Normalize(NormalizationForm.FormC);
    }

    /// <summary>
    /// Vrai si la section citée existe dans l'index de la cible. Une section en plusieurs
    /// niveaux (« A › B ») exige chaque niveau ; un numéro en tête suffit à désigner sa
    /// section, et à défaut le titre qui le suit (« S6 — Arbitrages ») ; sinon le titre
    /// cité peut être suivi d'un complément libre (« Décision — détail »), n'être que le
    /// début du titre réel, ou en nommer deux (« A et B »).
    /// </summary>
    public static bool Existe(string texte, List<string> index)
    {
        foreach (string niveau in texte.Split(" › ", StringSplitOptions.RemoveEmptyEntries))
        {
            if (!NiveauExiste(Normaliser(niveau), index)) return false;
        }
        return true;
    }

    private static bool NiveauExiste(string niveau, List<string> index)
    {
        if (niveau.Length == 0) return true;

        var numero = MotifNumero.Match(niveau);
        if (numero.Success)
        {
            if (index.Exists(e => CommenceParNumero(e, numero.Value))) return true;
            niveau = niveau[numero.Length..].TrimStart(' ', '—', '–', '-', ':', '.', ')');
            if (niveau.Length == 0) return false;
        }

        if (TitreExiste(niveau, index)) return true;

        string[] parties = niveau.Split(" et ");
        return parties.Length > 1 && parties.All(p => TitreExiste(p.Trim(CaracteresDeBord), index));
    }

    private static bool TitreExiste(string titre, List<string> index)
    {
        foreach (string candidat in Candidats(titre))
        {
            if (index.Exists(e => Apparie(e, candidat))) return true;
        }
        return false;
    }

    private static bool CommenceParNumero(string entree, string numero)
    {
        var m = MotifNumero.Match(entree);
        return m.Success && string.Equals(m.Value, numero, StringComparison.Ordinal);
    }

    private static IEnumerable<string> Candidats(string niveau)
    {
        yield return niveau;
        foreach (string separateur in Separateurs)
        {
            int position = niveau.IndexOf(separateur, StringComparison.Ordinal);
            while (position > 0)
            {
                string tete = niveau[..position].Trim(CaracteresDeBord);
                if (tete.Length > 0) yield return tete;
                position = niveau.IndexOf(separateur, position + 1, StringComparison.Ordinal);
            }
        }
    }

    private static bool Apparie(string entree, string candidat)
    {
        if (string.Equals(entree, candidat, StringComparison.Ordinal)) return true;
        return CommenceAuMot(entree, candidat) || (entree.Length >= 3 && CommenceAuMot(candidat, entree));
    }

    // « longue » commence par « courte » et la coupure tombe sur une frontière de mot.
    private static bool CommenceAuMot(string longue, string courte)
    {
        if (courte.Length == 0 || longue.Length <= courte.Length) return false;
        if (!longue.StartsWith(courte, StringComparison.Ordinal)) return false;
        return !char.IsLetterOrDigit(longue[courte.Length]);
    }
}
