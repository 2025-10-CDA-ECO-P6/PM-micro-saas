#!/usr/bin/env python3
"""Cherche les identifiants définis plus d'une fois. Usage : python3 -I controle-doublons-identifiants.py [racine docs, défaut : docs]
Code de sortie : 0 aucun doublon, 1 au moins un.
Définition = forme déclarée au catalogue (.claude/corpus/modules) pour chaque famille :
  RB            ligne  '- RB-NN-KK :'
  US-NN-KK      titre  '### US-NN-KK —'
  TB, LOT, EP   titre  '#+ TB-nnn' / 'LOT-nn' / 'EP-nn'
  AR, UC-F, risques [TPJD]-NN, RUPTURE   titre '#+ ID'
  UC, US-UC, UJ-UC, NFR, ADR, persona    préfixe du nom de fichier + titre H1 de forme 'ID —'
  CR-UCNN-KK, CR-TRANS, PR, PO, IR       ligne de tableau '| ID |'
Les familles à portée 'instance' (A<n>, E<n>, C<n>) ne se comparent qu'au sein d'un fichier :
elles ne sont pas scrutées ici.
Exception EP : une même épique dont le titre `### EP-NN` se répète sous des sections `## ` de jalon
différentes (plan-de-travail.md, EP-04 sous J0 puis sous J1) n'est pas une double définition, mais
une épique qui traverse des jalons. Un titre EP n'est un doublon que s'il se répète dans la même
section `## `, ou dans deux fichiers différents."""
import os, re, sys, collections
root = sys.argv[1] if len(sys.argv) > 1 else 'docs'
SKIP = ('workstreams', os.path.join('project', 'backlog'))
defs = collections.defaultdict(list)   # (famille, id, portée) -> [(fichier, ligne)]
H = r'^#{1,6}\s+'
heading = [
  ('US', re.compile(H + r'(US-\d{2}-\d{2})\b')),
  ('TB', re.compile(H + r'(TB-\d{3})\b')),
  ('LOT', re.compile(H + r'(LOT-\d{2})\b')),
  ('EP', re.compile(H + r'(EP-\d{2})\b')),
  ('AR', re.compile(H + r'(AR-\d{2})\b')),
  ('ARB', re.compile(H + r'(ARB-\d{2})\b')),
  ('UC-F', re.compile(H + r'(UC-F\d+)\b')),
  ('RISQUE', re.compile(H + r'([TPJD]-\d{2})\b')),
  ('RUPTURE', re.compile(H + r'(RUPTURE-\d+)\b')),
]
bullet = [('RISQUE', re.compile(r'^\*\*([TPJD]-\d{2}) —')), ('RB', re.compile(r'^- (RB-\d{2}-\d{2}) ?:'))]
row = [('CR', re.compile(r'^\|\s*(CR-(?:UC\d{2}|TRANS)-\d{2})\s*\|')),
       ('PR', re.compile(r'^\|\s*(PR-\d{2})\s*\|')), ('PO', re.compile(r'^\|\s*(PO-\d{2})\s*\|')),
       ('IR', re.compile(r'^\|\s*(IR-\d{2})\s*\|')),
       ('RISQUE-LIGNE', re.compile(r'^\|\s*([TPJD]-\d{2})\s*\|'))]
fam_file = re.compile(r'^(UC-\d{2}|US-UC-\d{2}|UJ-UC-\d{2}|NFR-[A-Z0-9]+-\d{2}|ADR-\d{3}|persona-\d{2})-')
h1 = re.compile(r'^#\s+(UC-\d{2}|US-UC-\d{2}|UJ-UC-\d{2}|NFR-[A-Z0-9]+-\d{2}|ADR-\d{3}|persona-\d{2})\s+—')
for d, _, fs in os.walk(root):
    rel = os.path.relpath(d, root)
    if any(rel == s or rel.startswith(s + os.sep) for s in SKIP): continue
    for f in fs:
        p = os.path.join(d, f); r = os.path.relpath(p, root)
        m = fam_file.match(f)
        if m: defs[('FICHIER', m.group(1), '')].append((r, 0))
        if not f.endswith('.md'): continue
        fence = False; h2 = ''
        for n, line in enumerate(open(p, encoding='utf-8', errors='replace'), 1):
            line = line.rstrip('\r\n')
            if line.lstrip().startswith('```'): fence = not fence; continue
            if fence: continue
            if line.startswith('## '): h2 = line
            m = h1.match(line)
            if m: defs[('TITRE-H1', m.group(1), '')].append((r, n))
            for fam, rx in heading + bullet + row:
                m = rx.match(line)
                if m: defs[(fam, m.group(1), '')].append((r, n, h2 if fam == 'EP' else ''))
n = 0
for (fam, i, _), v in sorted(defs.items()):
    if fam == 'EP':
        # exception EP (voir l'en-tête) : un seul fichier, une définition par section de jalon
        sections = [(x[0], x[2]) for x in v]
        if len({x[0] for x in v}) == 1 and len(set(sections)) == len(sections): continue
    if len(v) > 1:
        # un même fichier cité deux fois par un titre H1 et son nom de fichier n'est pas un doublon
        if fam in ('FICHIER', 'TITRE-H1') and len({x[0] for x in v}) == 1: continue
        n += 1; print(f'DOUBLON {fam} {i}: ' + ' ; '.join(f'{x[0]}:{x[1]}' for x in v))
fams = collections.Counter(f for (f, _, _) in defs)
print('familles scrutées (définitions trouvées) :', dict(sorted(fams.items())))
print(f'doublons : {n}')
sys.exit(1 if n else 0)
