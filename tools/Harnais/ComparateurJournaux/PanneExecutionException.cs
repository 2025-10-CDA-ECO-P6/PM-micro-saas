namespace ComparateurJournaux;

/// Panne anticipée d'exécution (fichier journal introuvable, ligne
/// illisible…) — distincte d'une divergence entre journaux, qui est un
/// verdict, pas une panne.
internal sealed class PanneExecutionException(string message) : Exception(message);
