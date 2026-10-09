namespace VerifMarqueursIdentifiants;

/// <summary>
/// Un écart relevé par un contrôle. <paramref name="Cle"/> est la clé sous
/// laquelle la liste de référence l'accepte : plusieurs constats peuvent la
/// partager, et c'est leur nombre que la liste fixe.
/// </summary>
internal sealed record Constat(string Cle, string Fichier, int Ligne, string Message)
{
    public string Emplacement => $"{Fichier}:{Ligne}";
}
