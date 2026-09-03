namespace FauxGh;

/// Lit l'entrée standard — seulement si elle est redirigée. Un terminal
/// interactif ne referme jamais l'entrée standard : la lire sans condition
/// bloquerait indéfiniment un faux gh lancé à la main, ou invoqué par un
/// outil qui n'a rien à lui envoyer sur ce canal (cas de `creer-issues.py`,
/// qui n'écrit jamais sur l'entrée standard de `gh`).
internal static class LecteurEntreeStandard
{
    public static string Lire() => Console.IsInputRedirected ? Console.In.ReadToEnd() : "";
}
