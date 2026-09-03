"""Utilitaires partagés par les tests de caractérisation des outils de `tools/`.

Aucun de ces utilitaires n'écrit jamais dans le dépôt réel : tout dépôt git de
fixture est construit dans un répertoire temporaire propre à chaque test
(`tempfile` / `tmp_path`-like), jamais dans `tools/` ni ailleurs sous le dépôt
suivi par git.
"""
import os
import shutil
import subprocess
import sys
import tempfile
import importlib.util

ICI = os.path.dirname(os.path.abspath(__file__))
RACINE_OUTILS = os.path.dirname(ICI)
RACINE_DEPOT = os.path.dirname(RACINE_OUTILS)


def chemin_outil(nom_fichier):
    return os.path.join(RACINE_OUTILS, nom_fichier)


def charger_module(nom_fichier, nom_module):
    """Charge un script à nom à trait d'union comme module Python — même
    convention que `creer-projet.py::charger_outil_issues()` : sys.argv est
    neutralisé le temps du chargement pour qu'aucun argparse du module chargé
    ne heurte les arguments propres au test en cours. Le module chargé sous ce
    nom n'est jamais `__main__` : le bloc `if __name__ == "__main__"` en pied
    de fichier ne s'exécute donc pas — seules les définitions sont importées."""
    chemin = chemin_outil(nom_fichier)
    spec = importlib.util.spec_from_file_location(nom_module, chemin)
    module = importlib.util.module_from_spec(spec)
    memoire = sys.argv
    sys.argv = [sys.argv[0]]
    try:
        spec.loader.exec_module(module)
    finally:
        sys.argv = memoire
    return module


def executer_script(nom_fichier, args, cwd=None, env=None, timeout=30):
    """Exécute un script de `tools/` en sous-processus et renvoie le
    CompletedProcess (stdout/stderr texte, jamais d'exception sur code de
    sortie non nul — c'est aux tests d'adjuger le code)."""
    commande = [sys.executable, chemin_outil(nom_fichier), *args]
    return subprocess.run(
        commande, cwd=cwd, env=env, capture_output=True, text=True, timeout=timeout
    )


def repertoire_bin_sans_gh(racine_temp):
    """Construit un répertoire ne portant que des liens vers `git` et
    `python3` réels — jamais vers `gh`. Utilisé comme PATH d'un sous-processus
    pour garantir qu'aucun appel réseau vers l'API GitHub n'est possible,
    quel que soit l'état d'authentification `gh` de la machine hôte."""
    bin_dir = os.path.join(racine_temp, "bin-sans-gh")
    os.makedirs(bin_dir, exist_ok=True)
    for outil in ("git", "python3"):
        reel = shutil.which(outil)
        if reel is None:
            raise RuntimeError(f"outil requis introuvable sur cette machine : {outil}")
        cible = os.path.join(bin_dir, outil)
        if not os.path.exists(cible):
            os.symlink(reel, cible)
    return bin_dir


def executer_git(depot, *args, verifier=True):
    resultat = subprocess.run(
        ["git", "-C", depot, *args], capture_output=True, text=True
    )
    if verifier and resultat.returncode != 0:
        raise RuntimeError(
            f"git {' '.join(args)} a échoué dans {depot} : {resultat.stderr.strip()}"
        )
    return resultat


def initialiser_depot_git(depot):
    """Crée un dépôt git minimal, identité locale fixée (aucune dépendance à
    la config git globale de la machine), sans aucun remote."""
    os.makedirs(depot, exist_ok=True)
    executer_git(depot, "-c", "init.defaultBranch=main", "init", "-q")
    executer_git(depot, "config", "user.email", "test-writer@haversack.invalid")
    executer_git(depot, "config", "user.name", "Test Writer (fixture)")


def ecrire(chemin, contenu):
    os.makedirs(os.path.dirname(chemin), exist_ok=True)
    with open(chemin, "w", encoding="utf-8") as f:
        f.write(contenu)


def commiter_tout(depot, message="fixture"):
    executer_git(depot, "add", "-A")
    executer_git(depot, "commit", "-q", "-m", message)
    return executer_git(depot, "rev-parse", "HEAD").stdout.strip()
