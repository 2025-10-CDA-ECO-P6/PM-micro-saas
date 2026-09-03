#!/usr/bin/env python3
"""État du corpus : liens relatifs, ancres, fichiers orphelins, blocs de code.
Couvre les fichiers suivis ET les fichiers neufs non encore commités."""
import os, re, sys, unicodedata, subprocess
from collections import defaultdict
from urllib.parse import unquote

ROOT = os.path.abspath(sys.argv[1] if len(sys.argv) > 1 else ".")

def fichiers_md():
    suivis = subprocess.run(["git","-C",ROOT,"ls-files","*.md"],
                            capture_output=True, text=True, check=True).stdout.splitlines()
    neufs = subprocess.run(["git","-C",ROOT,"ls-files","--others","--exclude-standard","*.md"],
                           capture_output=True, text=True, check=True).stdout.splitlines()
    return sorted({p for p in suivis + neufs if p})

LIEN = re.compile(r'\[(?:[^\[\]]|\[[^\[\]]*\])*\]\(([^()\s]+(?:\([^()\s]*\)[^()\s]*)*)\)')
ATX = re.compile(r'^(#{1,6})\s+(.*?)\s*#*\s*$')
FENCE = re.compile(r'^\s*(```+|~~~+)')

def slug(texte):
    t = texte.strip()
    t = re.sub(r'`([^`]*)`', r'\1', t)
    t = re.sub(r'!?\[([^\]]*)\]\([^)]*\)', r'\1', t)
    t = re.sub(r'<[^>]+>', '', t)
    t = re.sub(r'[*_~]', '', t).lower()
    t = unicodedata.normalize('NFC', t)
    return ''.join(c if (c.isalnum() or c in '-_') else ('-' if c in ' \t' else '') for c in t)

def ancres_et_fences(chemin):
    ancres, vues = set(), defaultdict(int)
    dans, tok = False, None
    for ligne in open(chemin, encoding='utf-8'):
        m = FENCE.match(ligne)
        if m:
            t = m.group(1)
            if not dans: dans, tok = True, t[0]*3
            elif t[0]*3 == tok and ligne.strip() == t: dans, tok = False, None
            continue
        if dans: continue
        h = ATX.match(ligne)
        if h:
            base = slug(h.group(2))
            if base:
                vues[base] += 1
                ancres.add(base if vues[base] == 1 else f"{base}-{vues[base]-1}")
        for a in re.findall(r'<a\s+(?:id|name)="([^"]+)"', ligne):
            ancres.add(a.lower())
    return ancres, 1 if dans else 0

fichiers = fichiers_md()
index, desequilibres = {}, []
for rel in fichiers:
    a, ub = ancres_et_fences(os.path.join(ROOT, rel))
    index[rel] = a
    if ub: desequilibres.append(rel)

n_rel = n_anc = 0
cible_ko, ancre_ko = [], []
entrants = defaultdict(set)
for rel in fichiers:
    src = os.path.dirname(rel)
    txt = open(os.path.join(ROOT, rel), encoding='utf-8').read()
    txt = re.sub(r'^(```+|~~~+).*?^\1\s*$', '', txt, flags=re.S | re.M)
    for cible in LIEN.findall(txt):
        if re.match(r'^(https?:|mailto:|tel:|data:|ftp:)', cible, re.I): continue
        chemin, _, frag = cible.partition('#')
        if not chemin:
            n_anc += 1
            if frag and frag.lower() not in index[rel]: ancre_ko.append((rel, cible))
            continue
        n_rel += 1
        if frag: n_anc += 1
        tgt = os.path.normpath(os.path.join(src, unquote(chemin)))
        abs_tgt = os.path.join(ROOT, tgt)
        if not os.path.exists(abs_tgt):
            cible_ko.append((rel, cible)); continue
        if tgt.endswith('.md'):
            entrants[tgt].add(rel)
            if frag:
                connues = index.get(tgt)
                if connues is None:
                    connues, _ = ancres_et_fences(abs_tgt); index[tgt] = connues
                if unquote(frag).lower() not in connues: ancre_ko.append((rel, cible))

orphelins = [f for f in fichiers if f not in entrants and f != "README.md"]

print(f"fichiers markdown             : {len(fichiers)}  (sous docs/ : {sum(1 for f in fichiers if f.startswith('docs/'))})")
print(f"liens relatifs                : {n_rel}")
print(f"liens portant une ancre       : {n_anc}")
print(f"liens à cible inexistante     : {len(cible_ko)}")
print(f"liens à ancre inexistante     : {len(ancre_ko)}")
print(f"fichiers orphelins            : {len(orphelins)}")
print(f"blocs de code déséquilibrés   : {len(desequilibres)}")
for lab, items in (("CIBLE", cible_ko), ("ANCRE", ancre_ko)):
    for s, t in items[:30]: print(f"  [{lab} CASSÉE] {s} -> {t}")
for o in orphelins[:30]: print(f"  [ORPHELIN] {o}")
for u in desequilibres[:30]: print(f"  [FENCE] {u}")
