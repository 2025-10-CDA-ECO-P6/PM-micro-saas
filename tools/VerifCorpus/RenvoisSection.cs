using System.Text;
using System.Text.RegularExpressions;

namespace VerifCorpus;

/// <summary>
/// Renvoi « § section » repéré dans un document : le fichier source est porté par
/// l'appelant, la cible est le chemin tel qu'écrit dans le lien (sans ancre).
/// <c>Section</c> est la forme affichée (le numéro seul quand la section en commence
/// par un) ; <c>Texte</c> est la section telle qu'écrite, qui sert à l'appariement.
/// <c>Libelle</c> est le libellé de section écrit après le numéro, ou <c>null</c> : sans
/// libellé, le renvoi n'est contrôlé que sur son numéro.
/// </summary>
internal sealed record RenvoiSection(int Ligne, string Cible, string Section, string Texte, string? Libelle);

/// <summary>
/// Sections d'un document : <c>Entrees</c> sert à l'appariement d'un renvoi à une section,
/// <c>Numerotees</c> garde, pour chaque titre ou libellé en gras qui commence par un numéro,
/// de quoi vérifier le libellé d'un renvoi à ce numéro.
/// </summary>
internal sealed record IndexSections(List<string> Entrees, List<SectionNumerotee> Numerotees);

/// <summary>
/// Section numérotée : <c>Titres</c> est son titre sans le numéro et, s'il en porte un, la
/// partie qui suit le tiret long (normalisés) ; <c>Contenu</c> reprend de même les titres
/// plus profonds et les libellés en gras qu'elle contient, car un renvoi « §5 — Libellé »
/// peut viser un libellé interne à la section 5 ; <c>Brut</c> est le titre tel qu'écrit.
/// </summary>
internal sealed record SectionNumerotee(string Numero, List<string> Titres, List<string> Contenu, string Brut)
{
    public int Niveau { get; init; }
}

/// <summary>
/// Contrôle des renvois « § section » (ADR-019, § 4). Un renvoi contrôlable est un lien
/// Markdown dont le texte porte « § &lt;section&gt; », ou que « § &lt;section&gt; » suit
/// immédiatement sur la même ligne. La section est appariée aux titres ATX et aux
/// libellés en gras de la cible, jamais aux seuls titres : un renvoi vers un libellé en
/// gras est juste. Un numéro seul désigne le titre ou le libellé qui commence par ce
/// numéro. Quand le renvoi porte aussi un libellé après le numéro, ce libellé doit être
/// celui de la section qui porte ce numéro : une renumérotation de la cible ne laisse
/// pas un renvoi pointer vers une autre section sans que cela se voie.
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
        @"^(#{1,6})\s+(.*?)\s*#*\s*$",
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

    // Libellé écrit après le numéro dans le texte d'un lien : séparé par un tiret, un deux-
    // points ou un point, ou seulement par une espace devant une majuscule ou un guillemet
    // (« §5 Composants »). Une minuscule après l'espace est de la prose (« § 8.1 à 8.4 »).
    private static readonly Regex MotifLibelleDansLien = new(
        @"^(?:\s*[—–:.\-]\s+|\s+(?=[\p{Lu}«]))(?<libelle>.+)$",
        RegexOptions.CultureInvariant | RegexOptions.Singleline);

    // Hors du texte du lien, seul le tiret long ou demi-long désigne un libellé, et le
    // libellé commence par une majuscule ou un guillemet : « §2 — Choix et justification ».
    // Un tiret suivi d'une minuscule est une incise (« §3.2 — résolution du cas »).
    private static readonly Regex MotifLibelleApresLien = new(
        @"^\s*[—–]\s+(?<libelle>[\p{Lu}«].*)$",
        RegexOptions.CultureInvariant | RegexOptions.Singleline);

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

            string? brute = ExtraireSection(m, texteLignesPreservees, out bool dansLeLien);
            if (brute is null) continue;

            string texte = brute.Trim(CaracteresDeBord);
            if (texte.Length == 0) continue;
            string section = SectionPourAffichage(texte);

            int ligne = 1 + texteLignesPreservees.AsSpan(0, m.Index).Count('\n');
            renvois.Add(new RenvoiSection(ligne, chemin, section, texte, ExtraireLibelle(brute, dansLeLien)));
        }
        return renvois;
    }

    private static string? ExtraireSection(Match lien, string texte, out bool dansLeLien)
    {
        string texteDuLien = lien.Groups["texte"].Value;
        int marque = texteDuLien.IndexOf('§');
        dansLeLien = marque >= 0;
        if (dansLeLien) return texteDuLien[(marque + 1)..];

        var apres = MotifApresLien.Match(texte, lien.Index + lien.Length);
        if (!apres.Success) return null;

        // Hors du texte du lien, la section n'a pas de borne : on la coupe avant la
        // ponctuation qui ouvre une parenthèse, une énumération ou un autre lien.
        string reste = apres.Groups["section"].Value;
        int coupure = reste.IndexOfAny([',', ';', '(', ')', '|', '`', '[']);
        return coupure < 0 ? reste : reste[..coupure];
    }

    /// <summary>
    /// Libellé qui suit le numéro de section, ou <c>null</c> : section sans numéro,
    /// numéro seul, énumération (« § 1 et § 9 »), sous-section (« §3 § Titre ») ou prose.
    /// </summary>
    private static string? ExtraireLibelle(string brute, bool dansLeLien)
    {
        var numero = MotifNumero.Match(brute.Trim(CaracteresDeBord));
        if (!numero.Success) return null;

        string reste = brute.Trim(CaracteresDeBord)[numero.Length..];
        if (reste.Contains('§') || reste.Contains('›')) return null;

        var libelle = (dansLeLien ? MotifLibelleDansLien : MotifLibelleApresLien).Match(reste);
        if (!libelle.Success) return null;

        string texte = libelle.Groups["libelle"].Value.Trim(CaracteresDeBord);
        return texte.Any(char.IsLetter) ? texte : null;
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
    public static IndexSections Indexer(string texteSansCode)
    {
        var index = new IndexSections([], []);
        // Sections numérotées ouvertes, de la plus large à la plus profonde : un titre plus
        // profond ou un libellé en gras s'ajoute à leur contenu jusqu'au prochain titre de
        // niveau égal ou supérieur.
        var ouvertes = new List<SectionNumerotee>();
        foreach (string ligne in texteSansCode.Split('\n'))
        {
            var atx = MotifAtx.Match(ligne);
            if (atx.Success)
            {
                int niveau = atx.Groups[1].Length;
                ouvertes.RemoveAll(o => o.Niveau >= niveau);
                Ajouter(index, atx.Groups[2].Value, ouvertes, niveau);
            }
            foreach (Match gras in MotifGras.Matches(ligne)) Ajouter(index, gras.Groups[1].Value, ouvertes, null);
        }
        return index;
    }

    // Une section se cite aussi par son titre sans son numéro (« 11. Matrice » citée
    // « Matrice ») ou par la partie qui suit le tiret long (« 8. Domaine — Conventions »
    // citée « Conventions »). L'entrée complète reste indexée : un numéro seul s'apparie
    // dessus.
    private static void Ajouter(IndexSections index, string brut, List<SectionNumerotee> ouvertes, int? niveauTitre)
    {
        string n = Normaliser(brut);
        if (n.Length == 0) return;
        index.Entrees.Add(n);

        var titres = new List<string>();
        string sansNumero = MotifNumeroDeTitre.Replace(n, "", 1);
        if (sansNumero.Length > 0 && sansNumero != n) index.Entrees.Add(sansNumero);
        titres.Add(sansNumero);

        int tiret = sansNumero.IndexOf(" — ", StringComparison.Ordinal);
        if (tiret > 0)
        {
            string apresTiret = sansNumero[(tiret + 3)..].Trim(CaracteresDeBord);
            index.Entrees.Add(apresTiret);
            titres.Add(apresTiret);
        }

        foreach (var ouverte in ouvertes) ouverte.Contenu.AddRange(titres);

        var numero = MotifNumero.Match(n);
        if (!numero.Success) return;

        var section = new SectionNumerotee(numero.Value, titres, [], MotifEspaces.Replace(brut, " ").Trim())
        {
            Niveau = niveauTitre ?? int.MaxValue,
        };
        index.Numerotees.Add(section);
        // Un libellé en gras numéroté n'a pas d'étendue : il ne contient rien.
        if (niveauTitre is not null) ouvertes.Add(section);
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
    public static bool Existe(string texte, IndexSections sections)
    {
        var index = sections.Entrees;
        foreach (string niveau in texte.Split(" › ", StringSplitOptions.RemoveEmptyEntries))
        {
            if (!NiveauExiste(Normaliser(niveau), index)) return false;
        }
        return true;
    }

    /// <summary>
    /// Vérifie le libellé d'un renvoi « § N » dont la section N existe dans la cible : rend
    /// <c>null</c> quand le renvoi n'est pas à contrôler (sans libellé, ou sans section qui
    /// porte ce numéro, déjà signalé comme section inexistante) ou quand une section
    /// numérotée N porte ce libellé ; sinon les titres réels des sections numérotées N.
    /// Le libellé s'apparie au titre sans son numéro, à la casse, aux espaces, aux emphases
    /// et à la ponctuation de fin près ; il peut n'en être que le début, ou être suivi d'un
    /// complément libre.
    /// </summary>
    public static List<string>? TitresSiLibelleDiscordant(string texte, string? libelle, IndexSections sections)
    {
        if (libelle is null) return null;
        var numero = MotifNumero.Match(Normaliser(texte));
        if (!numero.Success) return null;

        var candidates = sections.Numerotees.Where(s => s.Numero == numero.Value).ToList();
        if (candidates.Count == 0) return null;

        string cite = Normaliser(libelle);
        if (cite.Length == 0) return null;
        if (candidates.Exists(s => LibelleApparie(s, cite))) return null;
        return candidates.Select(s => s.Brut).ToList();
    }

    // Titre : égal, début net, ou début net suivi d'un complément libre. Contenu : égal ou
    // début net seulement — un libellé interne court (« Non ») ne doit pas absorber un
    // libellé plus long qui commence de même.
    private static bool LibelleApparie(SectionNumerotee section, string libelle)
    {
        foreach (string titre in section.Titres)
        {
            if (titre.Length == 0) continue;
            if (string.Equals(titre, libelle, StringComparison.Ordinal)) return true;
            if (CommenceAuMot(titre, libelle) || (titre.Length >= 3 && CommenceAuMot(libelle, titre))) return true;
        }
        return section.Contenu.Exists(c => string.Equals(c, libelle, StringComparison.Ordinal) || CommenceAuMot(c, libelle));
    }

    /// <summary>
    /// Compte les renvois dont le libellé est contrôlable : appelé avant la vérification,
    /// pour que le rapport dise ce qui a réellement été comparé.
    /// </summary>
    public static bool LibelleControlable(string texte, string? libelle, IndexSections sections)
    {
        if (libelle is null) return false;
        var numero = MotifNumero.Match(Normaliser(texte));
        return numero.Success && sections.Numerotees.Exists(s => s.Numero == numero.Value);
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
