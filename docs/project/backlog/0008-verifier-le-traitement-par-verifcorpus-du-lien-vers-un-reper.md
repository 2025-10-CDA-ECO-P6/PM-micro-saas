# 0008 — Vérifier le traitement par VerifCorpus du lien vers un répertoire

- **Id** : 0008
- **Date** : 2026-10-07
- **Statut** : fait — commit de la tâche backlog-decisions-architecture-assurance (chantier conformite-corpus)
- **Appliqué dans** : docs/projet/guide-de-lecture.md:26 (lien `../architecture/specs/`) : aucun fichier modifié (vérification).
- **Conclusion** : Un lien vers un répertoire existant est compté puis accepté, sans effet sur les orphelins. `tools/VerifCorpus/VerificateurCorpus.cs` : le lien est compté dans « liens relatifs » (`nRel++`, l. 97) ; `Path.Exists` (l. 102) accepte un répertoire comme un fichier ; si le répertoire n'existe pas, le lien est rangé en CIBLE CASSÉE. Le test `.EndsWith(".md")` (l.108) écarte ensuite toute cible qui n'est pas un fichier markdown : aucune ancre n'est vérifiée et aucun référent entrant n'est enregistré, donc un répertoire cité ne fait jamais sortir un fichier de la liste des orphelins. Preuve : lecture du source, et `dotnet run --project tools/VerifCorpus` sur le worktree (335 fichiers, 2080 liens relatifs, 0 cible inexistante, 0 ancre inexistante, 0 orphelin), où le lien `../architecture/specs/` du guide de lecture est présent.

Vérifier le traitement par VerifCorpus du lien vers un répertoire. Lien ../architecture/specs/ à docs/gestion-projet/guide-lecture-par-fonctionnalite.md:26 : compté, ignoré ou accepté ? (voir tools/VerifCorpus/VerificateurCorpus.cs:108)

**Raison.** Trou de lecture de l'analyse d'onboarding du 2026-10-07, classé B (à vérifier) par l'opérateur
