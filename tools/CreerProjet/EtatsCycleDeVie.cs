using System.Globalization;
using System.Text;
using CreerIssues;

namespace CreerProjet;

/// <summary>
/// Lit les états du cycle de vie d'un ticket dans <c>methode-de-ticket.md §5</c>,
/// jamais codés en dur ici : le nom de chaque état est la DERNIÈRE ligne
/// significative du texte qui précède la première flèche « → » du schéma, ou
/// la PREMIÈRE ligne significative du texte qui suit chacune des flèches
/// suivantes. Reproduit <c>lire_etats()</c> du script Python d'origine
/// (<c>creer-projet.py</c>) à l'identique, découpage et filtres compris — y
/// compris ce qu'ils écartent
/// (une ligne de légende du schéma comme « en cours si elle ne le clôt pas) »
/// ne commence pas par une capitale et n'est jamais retenue comme un état).
/// </summary>
internal static class EtatsCycleDeVie
{
    private const string MarqueurEtatsTransitions = "**États et transitions**";

    // Caractères dépouillés en périphérie de chaque ligne du schéma, après le
    // dépouillement des blancs Python : accent grave, barre verticale,
    // tiret bas, flèche montante (U+2191, la branche de retour du schéma) et
    // espace — jamais une sous-chaîne, un jeu de points de code.
    private const string GarnitureLigne = "`|_↑ ";

    // Caractères dépouillés en périphérie du nom d'état une fois isolé :
    // astérisque (gras Markdown), virgule, deux-points et espace.
    private const string GarnitureNom = " *,:";

    public static List<string> Lire(string racine)
    {
        string chemin = Path.Combine(racine, Chemins.MethodeRelatif);
        string contenu = LecteurTexte.LireTexteIntegral(chemin);

        int debut = contenu.IndexOf(MarqueurEtatsTransitions, StringComparison.Ordinal);
        if (debut == -1)
        {
            throw new ErreurCorpusException(
                "les états du cycle de vie sont introuvables dans methode-de-ticket.md §5");
        }

        int trouve = contenu.IndexOf("\n- **", debut, StringComparison.Ordinal);
        // Python : `bloc = contenu[debut:contenu.find("\n- **", debut)]` — un
        // marqueur introuvable (-1) tronquerait, par la sémantique de slice
        // négative de Python, le DERNIER caractère du document entier plutôt
        // que d'aller jusqu'à sa fin. Jamais rencontré sur le corpus réel, où
        // la liste à puces suit toujours le schéma — reproduit tel quel
        // plutôt que « réparé » en silence ; la garde `fin < debut` ne fait
        // que rendre un bloc vide là où Python rendrait aussi une tranche
        // vide (`contenu[debut:fin]` avec `fin < debut`), sans lever.
        int fin = trouve != -1 ? trouve : contenu.Length - 1;
        if (fin < debut) fin = debut;
        string bloc = contenu[debut..fin];

        var etats = new List<string>();
        var vus = new HashSet<string>(StringComparer.Ordinal);
        var morceaux = bloc.Split('→');

        for (int rang = 0; rang < morceaux.Length; rang++)
        {
            var lignes = new List<string>();
            foreach (var brut in morceaux[rang].Split('\n'))
            {
                string ligne = Depouiller(TexteUnicode.StripPython(brut), GarnitureLigne);
                if (ligne.Length > 0)
                {
                    lignes.Add(ligne);
                }
            }
            if (lignes.Count == 0)
            {
                continue;
            }

            // Le premier morceau porte le préambule : l'état est sa DERNIÈRE
            // ligne ; les suivants portent l'état juste après la flèche : sa
            // PREMIÈRE ligne.
            string nom = Depouiller(rang == 0 ? lignes[^1] : lignes[0], GarnitureNom);
            if (nom.Length > 0 && PremierRuneEstMajuscule(nom) && vus.Add(nom))
            {
                etats.Add(nom);
            }
        }
        return etats;
    }

    /// <summary>
    /// Équivalent de <c>str.strip(caracteres)</c> Python : retire, à chaque
    /// extrémité, tout point de code présent dans <paramref name="caracteres"/> —
    /// jamais une sous-chaîne à retirer telle quelle.
    /// </summary>
    private static string Depouiller(string texte, string caracteres)
    {
        var garniture = new HashSet<int>();
        foreach (var r in caracteres.EnumerateRunes())
        {
            garniture.Add(r.Value);
        }

        var runes = texte.EnumerateRunes().ToList();
        int debut = 0, fin = runes.Count;
        while (debut < fin && garniture.Contains(runes[debut].Value)) debut++;
        while (fin > debut && garniture.Contains(runes[fin - 1].Value)) fin--;

        var sb = new StringBuilder();
        for (int i = debut; i < fin; i++) sb.Append(runes[i]);
        return sb.ToString();
    }

    /// <summary>Équivalent de <c>nom[0].isupper()</c> : vrai du premier point de code de <paramref name="nom"/>.</summary>
    private static bool PremierRuneEstMajuscule(string nom)
    {
        var premier = nom.EnumerateRunes().First();
        return CharUnicodeInfo.GetUnicodeCategory(premier.Value) == UnicodeCategory.UppercaseLetter;
    }
}
