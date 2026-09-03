using System.Text;

namespace CreerIssues;

/// <summary>
/// Reproduit la frontière de mot <c>\b</c> de Python : elle suit
/// <c>str.isalnum()</c> (chiffres non décimaux — catégories Unicode Nl/No —
/// compris), jamais la notion de caractère de mot du moteur d'expressions
/// régulières .NET (<c>\w</c>, qui exclut Nl/No et inclut des marques
/// combinantes que Python exclut). Mesuré : un identifiant de jalon suivi
/// d'un chiffre romain (catégorie Nl, ex. « J0Ⅴ ») est refusé côté Python —
/// le chiffre romain appartient au mot — et accepté côté moteur .NET nu, qui
/// ne le voit pas comme un caractère de mot. Sites : la résolution de jalon
/// (<see cref="ResolutionMaille.ResoudreJalon"/>), le découpage des
/// identifiants de tranche référencés par un champ (<see cref="TranchesReferences"/>),
/// et l'appariement d'un identifiant en tête du titre d'une issue GitHub déjà
/// existante (<see cref="Orchestrateur"/>) — le motif de titre de tâche du
/// plan lui-même (<see cref="PlanDeTravail"/>) ne porte, lui, aucune
/// frontière de mot dans le script Python d'origine.
///
/// Arbitrage ouvert, non corrigé : cette divergence n'a jamais été signalée
/// comme un défaut du script Python d'origine — elle est reproduite ici
/// délibérément, à l'identique, plutôt que « réparée » en silence.
/// </summary>
internal static class LimiteMotPython
{
    private static bool EstCaractereDeMot(Rune r) => TexteUnicode.EstAlphanumeriquePython(r) || r.Value == '_';

    /// <summary>
    /// Vrai si la position <paramref name="indexUtf16"/> (index de caractère
    /// UTF-16 dans <paramref name="texte"/>) est une frontière de mot Python :
    /// soit la fin de la chaîne, soit le point de code qui y commence n'est
    /// pas un caractère de mot Python.
    /// </summary>
    public static bool FrontiereApres(string texte, int indexUtf16)
    {
        if (indexUtf16 >= texte.Length) return true;

        char c = texte[indexUtf16];
        int pointDeCode = char.IsHighSurrogate(c) && indexUtf16 + 1 < texte.Length && char.IsLowSurrogate(texte[indexUtf16 + 1])
            ? char.ConvertToUtf32(c, texte[indexUtf16 + 1])
            : c;
        return !EstCaractereDeMot(new Rune(pointDeCode));
    }
}
