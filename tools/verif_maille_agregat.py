#!/usr/bin/env python3
"""Contrôle la maille de désignation « agrégat » proposée pour les tranches verticales.

Extrait la liste fermée des agrégats depuis le modèle de domaine (source de vérité),
puis vérifie que chaque item de `Périmètre d'écriture` du plan se résout dans l'une
des natures admises. Recalcule `En conflit avec` par intersection et le compare au
champ déclaré.

Usage : python3 docs/audit/verif_maille_agregat.py [racine] [plan.md]
"""
import os, re, sys, glob

ROOT = os.path.abspath(sys.argv[1] if len(sys.argv) > 1 else ".")
PLAN = sys.argv[2] if len(sys.argv) > 2 else "docs/gestion-projet/plan-de-travail.md"

# --- 1. Liste fermée des agrégats, extraite du modèle de domaine -------------
def agregats():
    trouves = {}
    for f in sorted(glob.glob(os.path.join(ROOT, "docs/conception/domain/*.md"))):
        sous_agregats = False
        for ligne in open(f, encoding="utf-8"):
            h2 = re.match(r'^##\s+(.*?)\s*$', ligne)
            if h2:
                titre = h2.group(1)
                sous_agregats = titre.startswith("Agrégats")
                m = re.match(r'^Agrégat unique\s*:\s*`?([A-Za-z]+)`?', titre)
                if m:
                    trouves[m.group(1)] = os.path.basename(f)
                continue
            h3 = re.match(r'^###\s+([A-Za-z]+)\s*(?:\(([^)]*)\))?\s*$', ligne)
            if h3 and sous_agregats:
                nom, qual = h3.group(1), (h3.group(2) or "")
                if " dans " in qual:            # entité / VO interne : pas une unité d'écriture
                    continue
                if qual == "" or "agrégat" in qual or "référence" in qual:
                    trouves[nom] = os.path.basename(f)
    return trouves

# --- 2. Autres natures admises ----------------------------------------------
def fiches_ecran():
    base = os.path.join(ROOT, "docs/conception/interface/wireframes")
    return {os.path.basename(d) for p in glob.glob(base + "/sv*/*") if os.path.isdir(p) for d in [p]}

def stores_locaux():
    f = os.path.join(ROOT, "docs/architecture/decisions/ADR-017-modele-indexeddb-local.md")
    if not os.path.exists(f): return set()
    return set(re.findall(r'`([a-z][a-z_]+)`', open(f, encoding="utf-8").read()))

TRANSVERSES = {"SharedKernel", "Haversack.Infrastructure.Notifications"}

# Modules de la couche cliente : les six préoccupations nommées par le corpus (§2, tableau).
CLIENT = {
    "service d'accès au store", "châssis", "bandeaux transversaux",
    "sanitisation", "politique de sécurité de contenu", "projection d'export",
}

# Module d'outillage : liste fermée à quatre libellés (§2, entre guillemets typographiques `…`).
OUTILLAGE = {
    "le projet de test d'architecture", "le projet de test de bout en bout",
    "la configuration de la solution", "la configuration d'intégration continue",
}

# Namespaces de bounded context : n'apparaissent comme module que sur la seule tranche qui les
# crée — TB-003, nommée en toutes lettres par le §2 lui-même.
NAMESPACES_BC = {"IdentityAccess", "SpaceManagement", "ContentLibrary", "SessionConduct"}
TRANCHE_CREATION_NAMESPACES = "TB-003"

# Projet .NET désigné par son rôle tant que son nom n'est pas tranché (§2, convention de
# désignation de structure-projets.md § Convention de désignation). Cette convention ne couvre
# pas les quatre couches de façon identique :
#   - Domaine et Application sont des PROJETS UNIQUES, tranchés par ADR-008 (structure-projets.md
#     §3 « Domaine et Application : projets uniques ») : seul leur nom reste ouvert, pas leur
#     nombre ni leur rôle. La convention cite elle-même les deux formes canoniques exactes qui
#     désignent ce rôle — « le projet Domaine unique », « le projet Application unique » — donc
#     ces deux libellés, et seulement eux, se résolvent ici. Toute autre mention de ces deux
#     couches (un nom fantaisiste, une faute) n'est pas une variante légitime de la convention :
#     c'est un défaut, et doit rester NON RÉSOLU.
#   - Infrastructure et Présentation restent MULTI-PROJETS (structure-projets.md §3 « Infrastructure
#     et Présentation : multi-projets conservé ») : « combien de projets, lesquels » relève de J0,
#     donc aucune forme canonique n'existe encore pour les désigner — la convention ne peut pas en
#     donner une, il n'y en a pas. Une mention de rôle sur ces deux couches ne se clôture donc pas
#     mécaniquement ; elle est signalée à part plutôt qu'absorbée dans « résolu ».
PROJET_ROLE_CLOS = {"le projet Domaine unique", "le projet Application unique"}
RE_PROJET_ROLE_OUVERT = re.compile(r"^(le|les)\s+(autres\s+)?projets?\b.*(Infrastructure|Présentation)")
OUVERT_J0 = "désignation de rôle non clôturable avant J0 (Infrastructure/Présentation, multi-projets)"

# --- 3. Lecture du plan ------------------------------------------------------
def taches():
    t = open(os.path.join(ROOT, PLAN), encoding="utf-8").read()
    out = {}
    for b in re.split(r'\n#{4,6}\s+(?=TB-)', t)[1:]:
        i = re.match(r'(TB-\d{3})', b).group(1)
        ch = {k.strip(): v.strip() for k, v in re.findall(r'^\|\s*([^|]+?)\s*\|\s*(.*?)\s*\|\s*$', b, re.M)}
        per = ch.get("Périmètre d'écriture", "")
        # HORS MAILLE : la tranche ne vise aucun module de code (§2) — ce n'est pas un item qui
        # se résout, c'est l'absence d'item. Exclue de la liste au même titre que TROU, et donc
        # exclue par construction du recalcul des conflits en section 4 : deux tranches HORS
        # MAILLE ne peuvent plus produire de conflit fantôme, quel que soit leur libellé.
        hors_maille = "HORS MAILLE" in per
        trou = "TROU" in per
        items = [] if (hors_maille or trou) else [x.strip() for x in per.split(";") if x.strip() not in ("", "—", "-")]
        out[i] = {
            "items": items,
            "hors_maille": hors_maille,
            "trou": trou,
            "conflits": set(re.findall(r'TB-\d{3}', ch.get("En conflit avec", ""))),
        }
    return out

AG, FI, ST = agregats(), fiches_ecran(), stores_locaux()
TB = taches()

def resout(item, tache_id):
    """Nature de l'item, ou None s'il ne se résout pas dans la maille."""
    if item in AG: return "agrégat"
    if item in TRANSVERSES: return "transverse nommé"
    if item in NAMESPACES_BC:
        return "namespace de bounded context" if tache_id == TRANCHE_CREATION_NAMESPACES else None
    if item in CLIENT: return "module de la couche cliente"
    if item in OUTILLAGE: return "module d'outillage"
    if item in PROJET_ROLE_CLOS: return "projet .NET (rôle, unicité tranchée)"
    if RE_PROJET_ROLE_OUVERT.match(item): return OUVERT_J0
    m = re.match(r'^fiche\s+`?([a-z0-9-]+)`?$', item)
    if m: return "fiche d'écran" if m.group(1) in FI else None
    m = re.match(r'^store local\s+`?([a-z_]+)`?$', item)
    if m: return "object store" if m.group(1) in ST else None
    return None

print(f"agrégats extraits du modèle de domaine ({len(AG)}) :")
for n, f in sorted(AG.items()): print(f"    {n:20} ← {f}")
print(f"fiches d'écran indexées      : {len(FI)}")
print(f"object stores indexés        : {len(ST)}")
print()

non_resolus, trous, hors_mailles, en_attente_j0, ok = [], [], [], [], 0
for i, d in sorted(TB.items()):
    if d["hors_maille"]:
        hors_mailles.append(i); continue
    if d["trou"]:
        trous.append(i); continue
    natures = [(x, resout(x, i)) for x in d["items"]]
    mauvais = [x for x, n in natures if n is None]
    ouverts = [x for x, n in natures if n == OUVERT_J0]
    # « résolu » doit rester une affirmation soutenable : un item non clôturable avant J0
    # n'est pas une erreur (mauvais), mais ce n'est pas davantage un module vérifié — la tranche
    # ne peut pas non plus compter dans « entièrement résolu ».
    if mauvais: non_resolus.append((i, mauvais))
    elif ouverts: en_attente_j0.append((i, ouverts))
    else: ok += 1

print(f"tâches du plan                        : {len(TB)}")
print(f"  périmètre entièrement résolu        : {ok}")
print(f"  périmètre HORS MAILLE (exclu)       : {len(hors_mailles)}")
print(f"  périmètre TROU (non nommable)       : {len(trous)}")
print(f"  périmètre en attente de J0 (rôle)   : {len(en_attente_j0)}")
print(f"  item ne se résolvant pas la maille  : {len(non_resolus)}")
for i, m in non_resolus[:40]: print(f"    [NON RÉSOLU] {i} -> {m}")
for i, m in en_attente_j0[:40]: print(f"    [EN ATTENTE J0] {i} -> {m}")

# --- 4. Recalcul des conflits par intersection ------------------------------
ecarts = []
for a in sorted(TB):
    for b in sorted(TB):
        if a >= b: continue
        if TB[a]["trou"] or TB[b]["trou"] or TB[a]["hors_maille"] or TB[b]["hors_maille"]: continue
        partage = set(TB[a]["items"]) & set(TB[b]["items"])
        declare = b in TB[a]["conflits"] or a in TB[b]["conflits"]
        if bool(partage) != declare:
            ecarts.append((a, b, sorted(partage), declare))
print(f"\nécarts entre `En conflit avec` déclaré et l'intersection recalculée : {len(ecarts)}")
for a, b, p, d in ecarts[:40]:
    print(f"    [ÉCART] {a} ↔ {b} : partagé={p or '∅'} ; déclaré={d}")
