#!/usr/bin/env python3
"""Controle de la table de deplacement du chantier conformite-corpus.

Rejouable, sans effet de bord : lit le depot, n'ecrit rien.

    python3 -I docs/workstreams/conformite-corpus/controle-table-deplacement.py
        [--root DIR] [--table FILE] [--selection FILE] [--plan FILE]

A rejouer sur l'etat qui precede les deplacements : apres un `git mv`, l'ancien chemin n'est plus
suivi et le plan calcule n'est plus le meme.

Controles (verites 1 a 4 de la tache) :
  1. couverture   chaque fichier suivi de docs/ (hors docs/workstreams/ et docs/project/) est dans
                  la colonne « ancien » ; aucun « ancien » n'est un fichier inconnu du depot.
  2. unicite      aucun « ancien » ni aucun « nouveau » n'apparait deux fois.
  3. inexistence  aucun « nouveau » n'existe deja dans le depot, sauf la ligne qui laisse un fichier
                  en place (ancien = nouveau) ; aucun « nouveau » n'est un dossier d'un autre
                  « nouveau » ni ne tombe sous docs/workstreams/ ou docs/project/.
  4. ecarts       chaque ligne de `conform.sh plan --map` est dans la table ou porte une ligne
                  `# ECART <ancien> | plan: <nouveau du plan> | ...` dans la table.
Controles de garde :
  5. identifiants les identifiants des noms de fichiers (ADR-NNN, UC-NN, NFR-XXX-NN, US-UC-NN,
                  UJ-UC-NN, persona-NN, parcours-NN) sont les memes avant et apres.
  6. selection    chaque chemin de selection-cible.txt designe un « nouveau » (fichier) ou un
                  dossier qui en contient un (le segment <contexte> s'arrete a son parent).
Sortie : code 0 si tout passe, 1 sinon ; chaque defaut est nomme.
"""
import argparse
import os
import re
import subprocess
import sys

HERE = os.path.dirname(os.path.abspath(__file__))
DEFAULT_ROOT = os.path.abspath(os.path.join(HERE, "..", "..", ".."))
EXCLUDED = ("docs/workstreams/", "docs/project/")
ID_RE = re.compile(r"(?:US-UC|UJ-UC|UC|ADR|NFR-[A-Z0-9]+|persona|parcours)-\d+")

failures = []


def fail(check, msg):
    failures.append((check, msg))


def git_files(root):
    out = subprocess.run(["git", "-c", "core.quotepath=off", "ls-files", "docs"], cwd=root,
                         capture_output=True, text=True, check=True).stdout
    return [l for l in out.splitlines() if l]


def read_table(path):
    rows, comments = [], []
    with open(path, encoding="utf-8") as fh:
        for n, line in enumerate(fh, 1):
            line = line.rstrip("\n").rstrip("\r")
            if not line.strip():
                continue
            if line.startswith("#"):
                comments.append(line)
                continue
            parts = line.split("\t")
            if len(parts) != 2 or not parts[0] or not parts[1]:
                fail("format", "ligne %d : ni commentaire ni « ancien<TAB>nouveau » : %r" % (n, line))
                continue
            rows.append((parts[0], parts[1], n))
    return rows, comments


def read_plan(root, plan_file):
    """Les lignes `ancien nouveau` du plan : `plan --map` en direct, ou un fichier (map ou plan)."""
    if plan_file:
        with open(plan_file, encoding="utf-8") as fh:
            text = fh.read()
    else:
        res = subprocess.run(["bash", ".claude/skills/onboard/conform.sh", "plan", "--map", "--root", root],
                             cwd=root, capture_output=True, text=True)
        if res.returncode != 0:
            fail("plan", "conform.sh plan --map sort en code %d : %s" % (res.returncode, res.stderr.strip()[:200]))
            return []
        text = res.stdout
    plan = []
    for line in text.splitlines():
        if line.startswith("F1 ") and " → " in line:
            o, n = line[3:].split(" → ", 1)
            plan.append((o, n))
        elif "\t" in line and not line.startswith("#"):
            o, n = line.split("\t")[:2]
            plan.append((o, n))
    return plan


def selection_paths(path):
    out = []
    with open(path, encoding="utf-8") as fh:
        for n, line in enumerate(fh, 1):
            line = line.strip()
            if not line or line.startswith("#"):
                continue
            parts = line.split("|")
            if parts[0] == "module" and len(parts) == 3:
                out.append((parts[1], parts[2]))
            elif parts[0] != "kind":
                fail("selection", "ligne %d : enregistrement non reconnu par ce controle : %r" % (n, line))
    return out


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument("--root", default=DEFAULT_ROOT)
    ap.add_argument("--table", default=os.path.join(HERE, "table-deplacement.tsv"))
    ap.add_argument("--selection", default=os.path.join(HERE, "selection-cible.txt"))
    ap.add_argument("--plan", default=None, help="plan ou map deja calcule, au lieu de `conform.sh plan --map`")
    a = ap.parse_args()

    repo = git_files(a.root)
    repo_set = set(repo)
    scope = [f for f in repo if not f.startswith(EXCLUDED)]
    rows, comments = read_table(a.table)
    olds = [o for o, _, _ in rows]
    news = [n for _, n, _ in rows]
    table = {o: n for o, n, _ in rows}

    # 1. couverture
    for f in scope:
        if f not in table:
            fail("couverture", "fichier suivi absent de la table : " + f)
    for o in olds:
        if o not in repo_set:
            fail("couverture", "ancien chemin inconnu du depot : " + o)
        if o.startswith(EXCLUDED):
            fail("couverture", "ancien chemin hors du perimetre de la table : " + o)

    # 2. unicite
    for col, vals in (("ancien", olds), ("nouveau", news)):
        seen = set()
        for v in vals:
            if v in seen:
                fail("unicite", "valeur « %s » en double : %s" % (col, v))
            seen.add(v)

    # 3. inexistence
    new_set = set(news)
    for o, n, ln in rows:
        if o == n:
            continue
        if n in repo_set:
            fail("inexistence", "ligne %d : le nouveau chemin existe deja dans le depot : %s" % (ln, n))
        if n.startswith(EXCLUDED):
            fail("inexistence", "ligne %d : le nouveau chemin tombe sous un dossier exclu : %s" % (ln, n))
        if not n.startswith("docs/"):
            fail("inexistence", "ligne %d : le nouveau chemin sort de docs/ : %s" % (ln, n))
    prefixes = set()
    for n in new_set:
        parts = n.split("/")
        for i in range(1, len(parts)):
            prefixes.add("/".join(parts[:i]))
    for n in new_set:
        if n in prefixes:
            fail("inexistence", "un nouveau chemin est aussi le dossier d'un autre : " + n)
    for p in prefixes:
        if p in repo_set:
            fail("inexistence", "un dossier de destination est un fichier du depot : " + p)

    # 4. ecarts avec le plan calcule
    plan = read_plan(a.root, a.plan)
    table_pairs = {(o, n) for o, n, _ in rows}
    explained = set()
    for c in comments:
        m = re.match(r"^# ECART (\S+) \| plan: (\S+) \|", c)
        if m:
            explained.add((m.group(1), m.group(2)))
    differing = [(o, n) for o, n in plan if (o, n) not in table_pairs]
    for o, n in differing:
        if (o, n) not in explained:
            fail("ecarts", "ligne du plan qui differe de la table et sans ligne ECART : %s -> %s" % (o, n))
    plan_set = set(plan)
    for o, n in sorted(explained):
        if (o, n) not in plan_set:
            fail("ecarts", "ligne ECART qui ne correspond a aucune ligne du plan : %s -> %s" % (o, n))
    n_ident = len(plan) - len(differing)

    # 5. identifiants
    for o, n, ln in rows:
        io = ID_RE.findall(os.path.basename(o))
        inew = ID_RE.findall(os.path.basename(n))
        if io != inew:
            fail("identifiants", "ligne %d : identifiants du nom modifies : %s -> %s" % (ln, io, inew))

    # 6. selection
    sel = selection_paths(a.selection) if os.path.exists(a.selection) else []
    if not sel:
        fail("selection", "aucun module dans " + a.selection)
    for mod, p in sel:
        if p in ("-", "off", ""):
            continue
        p = p.rstrip("/")
        if "<contexte>" in p:
            p = p.split("/<contexte>")[0]
        if p not in new_set and p not in prefixes:
            fail("selection", "module %s : %s n'est ni un nouveau chemin ni un dossier de nouveaux chemins" % (mod, p))

    moved = sum(1 for o, n in zip(olds, news) if o != n)
    print("table : %d lignes (%d deplacements, %d fichiers laisses en place), %d fichiers suivis dans le perimetre"
          % (len(rows), moved, len(rows) - moved, len(scope)))
    print("plan  : %d lignes, %d identiques a la table, %d ecarts expliques" % (len(plan), n_ident, len(differing) - sum(1 for d in differing if d not in explained)))
    print("selection : %d modules" % len(sel))
    if failures:
        for check, msg in failures:
            print("ECHEC %-12s %s" % (check, msg))
        print("%d defaut(s)" % len(failures))
        return 1
    print("OK : couverture, unicite, inexistence, ecarts, identifiants, selection")
    return 0


if __name__ == "__main__":
    sys.exit(main())
