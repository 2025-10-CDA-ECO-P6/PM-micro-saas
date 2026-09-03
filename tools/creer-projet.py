#!/usr/bin/env python3
"""Reporte le plan de travail vers un projet GitHub (Projects v2).

Crée le projet, ses champs, y ajoute les issues déjà ouvertes par
creer-issues.py, et renseigne les deux seuls champs que la méthode de ticket
autorise à vivre hors du plan :

  - « Statut »  : le cycle de vie de methode-de-ticket.md §5, six états.
                  L'état « Prêt » est CALCULÉ depuis les cinq conditions du §2,
                  jamais estimé.
  - « Épique »  : classification transverse, déduite du plan.

Ce qu'il ne fait pas, et pourquoi :
  - il ne porte ni taille, ni dépendances, ni périmètre, ni critères : le §1 de
    la méthode les nomme comme champs qu'un ticket ne recopie pas ;
  - il ne crée aucune vue : `gh project` n'a pas de sous-commande pour cela.
    Les vues se créent dans l'interface, la recette est en fin de ce fichier.

Sans --appliquer : mode à blanc, rien n'est écrit sur GitHub.
"""
import argparse, importlib.util, json, os, re, subprocess, sys

ICI = os.path.dirname(os.path.abspath(__file__))
# Le titre porte le dépôt et le produit : le dépôt s'appelle PM-micro-saas,
# le corpus nomme le produit Haversack. C'est le titre réellement porté par le
# projet sur GitHub — le modifier ici sans le modifier là-bas ferait créer un
# doublon à la prochaine exécution.
TITRE_PROJET = "PM-micro-saas - Haversack"

# ── réutilisation de l'analyse du plan, jamais dupliquée ────────────────────
def charger_outil_issues():
    chemin = os.path.join(ICI, "creer-issues.py")
    spec = importlib.util.spec_from_file_location("outil_issues", chemin)
    module = importlib.util.module_from_spec(spec)
    # le module chargé ne doit pas lire NOS arguments — mais il faut les lui
    # rendre ensuite, sinon argparse ne verra jamais --appliquer.
    memoire = sys.argv
    sys.argv = [sys.argv[0]]
    try:
        spec.loader.exec_module(module)
    finally:
        sys.argv = memoire
    return module

O = charger_outil_issues()

# ── états, lus dans la méthode plutôt que codés ici ─────────────────────────
def lire_etats(racine):
    chemin = os.path.join(racine, "docs/gestion-projet/methode-de-ticket.md")
    contenu = open(chemin, encoding="utf-8").read()
    debut = contenu.find("**États et transitions**")
    if debut == -1:
        raise SystemExit("les états du cycle de vie sont introuvables dans methode-de-ticket.md §5")
    bloc = contenu[debut:contenu.find("\n- **", debut)]
    morceaux = bloc.split("→")
    etats, vus = [], set()
    for rang, brut in enumerate(morceaux):
        lignes = [l.strip().strip("`|_↑ ") for l in brut.split("\n") if l.strip().strip("`|_↑ ")]
        if not lignes:
            continue
        # le premier morceau porte le préambule : l'état est sa DERNIÈRE ligne ;
        # les suivants portent l'état juste après la flèche : sa PREMIÈRE ligne.
        nom = (lignes[-1] if rang == 0 else lignes[0]).strip(" *,:")
        if nom and nom[0].isupper() and nom not in vus:
            vus.add(nom); etats.append(nom)
    return etats

def gh(*args, json_sortie=False):
    r = subprocess.run(["gh", *args], capture_output=True, text=True)
    if r.returncode != 0:
        raise SystemExit(f"gh {' '.join(args[:3])}… a échoué : {r.stderr.strip()}")
    return json.loads(r.stdout) if json_sortie else r.stdout.strip()

def main():
    a = argparse.ArgumentParser(description=__doc__,
                                formatter_class=argparse.RawDescriptionHelpFormatter)
    a.add_argument("--appliquer", action="store_true",
                   help="crée réellement le projet et ses champs (sans : mode à blanc)")
    args = a.parse_args()

    racine = O.trouver_racine_depot()
    owner, repo = O.deduire_owner_repo(O.lire_url_origin())
    plan = open(os.path.join(racine, O.CHEMIN_PLAN_RELATIF), encoding="utf-8").read()
    taches = O.extraire_taches(plan)
    etats = lire_etats(racine)
    epiques = sorted({t["champs"].get("Épique", "").strip() for t in taches} - {""})

    print(f"Dépôt          : {owner}/{repo}")
    print(f"Projet         : {TITRE_PROJET}")
    print(f"Tranches lues  : {len(taches)}")
    print(f"États lus dans methode-de-ticket §5 ({len(etats)}) : {' → '.join(etats)}")
    print(f"Épiques lues dans le plan ({len(epiques)}) : {', '.join(epiques)}")
    print()
    print("Champs qui NE seront PAS créés, et pourquoi : taille, dépendances,")
    print("périmètre d'écriture, critères d'acceptation — methode-de-ticket §1 les")
    print("nomme comme champs qu'un ticket ne recopie pas.")
    print()

    if not args.appliquer:
        print("MODE À BLANC — rien n'est créé sur GitHub.")
        print(f"  + projet « {TITRE_PROJET} » sous {owner}")
        print(f"  + lien du projet au dépôt {repo}")
        print(f"  + champ « Statut » (liste, {len(etats)} valeurs)")
        print(f"  + champ « Épique » (liste, {len(epiques)} valeurs)")
        issues = json.loads(subprocess.run(
            ["gh", "issue", "list", "--repo", f"{owner}/{repo}", "--limit", "300",
             "--state", "all", "--json", "number,title"],
            capture_output=True, text=True).stdout or "[]")
        presentes = {m.group(1) for i in issues
                     if (m := re.match(r"(TB-\d{3})", i["title"]))}
        au_plan = {t["id"] for t in taches}
        print(f"  + {len(presentes & au_plan)} élément(s) ajouté(s) — une issue existe "
              f"pour {len(presentes & au_plan)} des {len(au_plan)} tranches du plan")
        if presentes - au_plan:
            print(f"  ! issues sans tranche correspondante : {sorted(presentes - au_plan)}")
        manquantes = sorted(au_plan - presentes)
        if manquantes:
            print(f"  = {len(manquantes)} tranche(s) sans issue : elles n'entreront pas "
                  f"dans le projet tant que leur issue n'est pas ouverte")
        return 0

    return appliquer(owner, repo, racine, taches, etats, epiques)


# ══════════════════════════════════════════════════════════════════════════
# Écriture
# ══════════════════════════════════════════════════════════════════════════

COULEURS = ["GRAY", "BLUE", "GREEN", "YELLOW", "ORANGE", "RED", "PURPLE", "PINK"]


def graphql(requete, variables):
    """gh api graphql via --input : les valeurs à virgule passent intactes,
    ce que « --single-select-options » de gh project ne permet pas
    (« Ouvert, non prêt » y serait coupé en deux options)."""
    charge = json.dumps({"query": requete, "variables": variables}, ensure_ascii=False)
    r = subprocess.run(["gh", "api", "graphql", "--input", "-"],
                       input=charge, capture_output=True, text=True)
    if r.returncode != 0:
        raise SystemExit(f"GraphQL a échoué : {r.stderr.strip()}")
    reponse = json.loads(r.stdout)
    if "errors" in reponse:
        raise SystemExit(f"GraphQL a renvoyé une erreur : {reponse['errors']}")
    return reponse["data"]


def creer_champ_liste(id_projet, nom, valeurs):
    options = [{"name": v, "color": COULEURS[i % len(COULEURS)], "description": ""}
               for i, v in enumerate(valeurs)]
    data = graphql("""
        mutation($p:ID!,$n:String!,$o:[ProjectV2SingleSelectFieldOptionInput!]!){
          createProjectV2Field(input:{projectId:$p,dataType:SINGLE_SELECT,name:$n,singleSelectOptions:$o}){
            projectV2Field{ ... on ProjectV2SingleSelectField { id name options { id name } } } } }
    """, {"p": id_projet, "n": nom, "o": options})
    champ = data["createProjectV2Field"]["projectV2Field"]
    return champ["id"], {o["name"]: o["id"] for o in champ["options"]}


def statut_calcule(tache, etats_par_tb, terminaux, en_cours):
    """Applique les conditions de methode-de-ticket §2. Renvoie « Prêt » ou
    « Ouvert, non prêt » — jamais une estimation."""
    champs = tache["champs"]
    perimetre = champs.get("Périmètre d'écriture", "")
    criteres = champs.get("Critères d'acceptation", "")
    if perimetre.startswith("TROU"):                       # condition 5
        return "Ouvert, non prêt"
    if "TROU" in criteres:                                 # condition 4
        return "Ouvert, non prêt"
    deps = re.findall(r"TB-\d{3}", champs.get("Dépend de", ""))
    if any(etats_par_tb.get(d) not in terminaux for d in deps):   # condition 2
        return "Ouvert, non prêt"
    conflits = re.findall(r"TB-\d{3}", champs.get("En conflit avec", ""))
    if any(etats_par_tb.get(c) in en_cours for c in conflits):    # condition 3
        return "Ouvert, non prêt"
    return "Prêt"


def appliquer(owner, repo, racine, taches, etats, epiques):
    # 1. le projet — réutilisé s'il existe déjà sous ce titre, jamais dupliqué
    existants = gh("project", "list", "--owner", owner, "--limit", "100",
                   "--format", "json", json_sortie=True)["projects"]
    projet = next((p for p in existants if p["title"] == TITRE_PROJET), None)
    if projet is None:
        projet = gh("project", "create", "--owner", owner, "--title", TITRE_PROJET,
                    "--format", "json", json_sortie=True)
        print(f"  + projet #{projet['number']} créé : {projet['url']}")
        gh("project", "link", str(projet["number"]), "--owner", owner,
           "--repo", f"{owner}/{repo}")
        print(f"  + projet lié au dépôt {repo}")
    else:
        print(f"  = projet #{projet['number']} déjà présent, réutilisé : {projet['url']}")
    numero, id_projet, url = projet["number"], projet["id"], projet["url"]

    # 2. les champs — noms et valeurs lus du corpus, réutilisés s'ils existent
    champs = {c["name"]: c for c in gh("project", "field-list", str(numero), "--owner", owner,
                                       "--format", "json", json_sortie=True)["fields"]}

    def champ_liste(nom, valeurs):
        if nom in champs:
            data = graphql("""query($id:ID!){node(id:$id){
                ... on ProjectV2SingleSelectField{ id options{ id name } } } }""",
                {"id": champs[nom]["id"]})["node"]
            print(f"  = champ « {nom} » déjà présent ({len(data['options'])} valeurs)")
            return data["id"], {o["name"]: o["id"] for o in data["options"]}
        idc, opts = creer_champ_liste(id_projet, nom, valeurs)
        print(f"  + champ « {nom} » ({len(valeurs)} valeurs)")
        return idc, opts

    id_statut, opts_statut = champ_liste("Statut", etats)
    id_epique, opts_epique = champ_liste("Épique", epiques)

    # 3. les issues déjà ouvertes
    issues = json.loads(subprocess.run(
        ["gh", "issue", "list", "--repo", f"{owner}/{repo}", "--limit", "300",
         "--state", "all", "--json", "number,title,url,state"],
        capture_output=True, text=True).stdout)
    par_tb = {}
    for i in issues:
        m = re.match(r"(TB-\d{3})", i["title"])
        if m:
            par_tb[m.group(1)] = i
    print(f"  = {len(par_tb)} issue(s) trouvée(s) sur le dépôt, sur {len(taches)} tranches au plan")

    etats_par_tb = {tb: ("Terminé" if i["state"] == "CLOSED" else None)
                    for tb, i in par_tb.items()}

    # éléments déjà dans le projet — la clé « épique » revient de gh avec un octet
    # abîmé, on apparie donc par suffixe et jamais par nom exact
    deja = {}
    for it in gh("project", "item-list", str(numero), "--owner", owner, "--limit", "300",
                 "--format", "json", json_sortie=True)["items"]:
        m = re.match(r"(TB-\d{3})", (it.get("content") or {}).get("title", ""))
        if m:
            deja[m.group(1)] = it["id"]

    ajoutes, reutilises = 0, 0
    for tache in taches:
        tb = tache["id"]
        if tb not in par_tb:
            continue
        if tb in deja:
            id_item, reutilises = deja[tb], reutilises + 1
        else:
            id_item = gh("project", "item-add", str(numero), "--owner", owner,
                         "--url", par_tb[tb]["url"], "--format", "json",
                         json_sortie=True)["id"]
            ajoutes += 1
        epique = tache["champs"].get("Épique", "").strip()
        if epique in opts_epique:
            gh("project", "item-edit", "--id", id_item, "--project-id", id_projet,
               "--field-id", id_epique, "--single-select-option-id", opts_epique[epique])
        statut = statut_calcule(tache, etats_par_tb, {"Terminé"}, {"Pris", "En cours"})
        gh("project", "item-edit", "--id", id_item, "--project-id", id_projet,
           "--field-id", id_statut, "--single-select-option-id", opts_statut[statut])
        print(f"    · {tb} — épique {epique or '—'}, statut « {statut} »")

    print(f"\n  {ajoutes} élément(s) ajouté(s), {reutilises} déjà présent(s) — tous renseignés.")
    print(f"  Projet : {url}")
    print("\n  Les VUES ne sont pas créées : « gh project » n'a aucune sous-commande")
    print("  pour cela. Recette dans l'interface, trois clics chacune — voir le")
    print("  compte rendu de fin d'exécution.")
    return 0

if __name__ == "__main__":
    sys.exit(main())
