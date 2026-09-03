#!/usr/bin/env python3
"""Reporte le plan de travail versionné vers les issues GitHub du dépôt.

Le plan de travail (`docs/gestion-projet/plan-de-travail.md`) est la source de
vérité ; les issues créées par ce script n'en sont que le report. Toute
correction de fond se fait dans le plan, jamais dans les issues.

Ce script lit, sans jamais rien y coder en dur :
  - les tâches et leur périmètre d'écriture dans le plan de travail ;
  - le modèle de ticket et la taxonomie de libellés dans
    `docs/gestion-projet/methode-de-ticket.md` ;
  - les fiches d'écran existantes sous
    `docs/conception/interface/wireframes/`.

Quatre opérations, dans cet ordre : les libellés, les jalons (milestones),
puis une issue par tâche, puis un compte rendu.

Par défaut, rien n'est écrit : le script affiche ce qu'il ferait. Passer
`--appliquer` pour agir réellement. `--aide` affiche cette notice.
"""

import argparse
import json
import os
import re
import shutil
import subprocess
import sys
from pathlib import Path

CHEMIN_PLAN_RELATIF = "docs/gestion-projet/plan-de-travail.md"
CHEMIN_METHODE_RELATIF = "docs/gestion-projet/methode-de-ticket.md"
CHEMIN_WIREFRAMES_RELATIF = "docs/conception/interface/wireframes"

# La maille du plan (§2) n'est jamais recopiée ici : elle est lue dans le plan
# et dans les sources qu'il désigne. Voir extraire_maille().

# ──────────────────────────────────────────────────────────────────────────
# Lecture du dépôt et des sous-processus
# ──────────────────────────────────────────────────────────────────────────

class ErreurScript(Exception):
    """Erreur qui doit interrompre le script proprement, sans trace Python."""


def executer(commande, verifier=True):
    resultat = subprocess.run(commande, capture_output=True, text=True)
    if verifier and resultat.returncode != 0:
        raise ErreurScript(
            f"la commande {' '.join(commande)!r} a échoué : {resultat.stderr.strip()}"
        )
    return resultat


def trouver_racine_depot():
    resultat = executer(["git", "rev-parse", "--show-toplevel"])
    return Path(resultat.stdout.strip())


def lire_url_origin():
    resultat = executer(["git", "remote", "get-url", "origin"])
    return resultat.stdout.strip()


def lire_branche_courante():
    resultat = executer(["git", "rev-parse", "--abbrev-ref", "HEAD"], verifier=False)
    branche = resultat.stdout.strip()
    if resultat.returncode != 0 or not branche or branche == "HEAD":
        return "main"
    return branche


def lire_commit_courant():
    """Révision immuable de HEAD : sert de base au lien de tâche, là où une branche
    est un ref mutable qui ferait vieillir silencieusement le renvoi."""
    return executer(["git", "rev-parse", "HEAD"]).stdout.strip()


def lire_revision_plan():
    """Révision du plan de travail au sens de `methode-de-ticket.md` §1 — pas HEAD,
    mais le dernier commit qui a modifié le fichier lui-même : « git log -1
    --format=%H -- docs/gestion-projet/plan-de-travail.md donne le commit courant
    du fichier ». C'est cette valeur, gelée au moment de la génération (§6, clause
    du gel assumé), que l'estampille de provenance porte."""
    resultat = executer(["git", "log", "-1", "--format=%H", "--", CHEMIN_PLAN_RELATIF])
    revision = resultat.stdout.strip()
    if not revision:
        raise ErreurScript(
            f"aucun historique git pour {CHEMIN_PLAN_RELATIF} : impossible de dater "
            "l'estampille de provenance"
        )
    return revision


def commit_present_sur_distant(commit, branche):
    """Constate, sans contacter le distant, si `commit` est déjà atteignable depuis
    `origin/<branche>` ou, à défaut, `origin/main`, tel que connu localement. Un lien
    de tâche construit sur une révision non poussée renverrait une page introuvable ;
    ce constat évite de le fabriquer en silence."""
    for ref in (f"origin/{branche}", "origin/main"):
        existe = executer(["git", "rev-parse", "--verify", "-q", ref], verifier=False)
        if existe.returncode != 0:
            continue
        ancetre = executer(["git", "merge-base", "--is-ancestor", commit, ref], verifier=False)
        return ancetre.returncode == 0
    return False


def deduire_base_https(url_origin):
    url = url_origin.strip()
    if url.endswith(".git"):
        url = url[:-4]
    correspondance = re.match(r"^git@([^:]+):(.+)$", url)
    if correspondance:
        return f"https://{correspondance.group(1)}/{correspondance.group(2)}"
    return url


def deduire_owner_repo(url_origin):
    correspondance = re.search(r"github\.com[:/]+([^/]+)/([^/.]+?)(?:\.git)?/?$", url_origin.strip())
    if not correspondance:
        raise ErreurScript(
            f"impossible de déduire le propriétaire et le dépôt depuis l'URL distante {url_origin!r}"
        )
    return correspondance.group(1), correspondance.group(2)


# ──────────────────────────────────────────────────────────────────────────
# Lecture du plan de travail et de la méthode de ticket
# ──────────────────────────────────────────────────────────────────────────

def analyser_lignes_tableau(bloc):
    """Renvoie les paires (colonne 1, colonne 2) des lignes de tableau Markdown
    d'un bloc de texte, in-tête et ligne de séparation exclues."""
    lignes = []
    for ligne in bloc.splitlines():
        ligne = ligne.strip()
        if not (ligne.startswith("|") and ligne.endswith("|")):
            continue
        cellules = [c.strip() for c in ligne[1:-1].split("|")]
        if len(cellules) < 2:
            continue
        premiere = cellules[0]
        if premiere in ("Champ", "Valeur") or (premiere and set(premiere) <= {"-"}):
            continue
        lignes.append((cellules[0], cellules[1]))
    return lignes


def extraire_valeurs_taxonomie(bloc):
    return [premiere for premiere, _ in analyser_lignes_tableau(bloc)]


def extraire_taxonomie(contenu_methode):
    """Lit les trois axes de libellés (§3 de methode-de-ticket.md) : leurs
    valeurs, jamais codées en dur ici, sont celles lues dans le document."""
    i_jalon = contenu_methode.find("**Par jalon**")
    i_couche = contenu_methode.find("**Par nature de tranche**")
    i_nature = contenu_methode.find("**Par nature de vérification**")
    i_fin = contenu_methode.find("**Interdiction nommée")
    if -1 in (i_jalon, i_couche, i_nature, i_fin) or not (i_jalon < i_couche < i_nature < i_fin):
        raise ErreurScript(
            "la structure attendue de la taxonomie de libellés (§3 de "
            f"{CHEMIN_METHODE_RELATIF}) est introuvable — le document a changé de forme, "
            "ce script doit être corrigé avant de continuer"
        )
    return {
        "jalon": extraire_valeurs_taxonomie(contenu_methode[i_jalon:i_couche]),
        "nature_tranche": extraire_valeurs_taxonomie(contenu_methode[i_couche:i_nature]),
        "nature": extraire_valeurs_taxonomie(contenu_methode[i_nature:i_fin]),
    }


def extraire_maille(contenu_plan, racine):
    """Lit les natures de module de la maille (§2 du plan) qu'elle a la charge
    de résoudre — jamais recopiées en dur ici. La nature « fiche d'écran »
    (Surface) n'en fait pas partie : elle est résolue ailleurs, par
    mapper_nature(), via les slugs de wireframes lus sur disque. Renvoie un
    dictionnaire nature -> ensemble de noms."""
    import glob as _glob

    # (a) agrégats : le §2 renvoie au modèle de domaine, on l'y lit
    agregats = set()
    for chemin in sorted(_glob.glob(os.path.join(racine, "docs/conception/domain/*.md"))):
        sous_agregats = False
        for ligne in open(chemin, encoding="utf-8"):
            titre2 = re.match(r"^##\s+(.*?)\s*$", ligne)
            if titre2 is not None:
                sous_agregats = titre2.group(1).startswith("Agrégats")
                unique = re.match(r"^Agrégat unique\s*:\s*`?([A-Za-z]+)`?", titre2.group(1))
                if unique is not None:
                    agregats.add(unique.group(1))
                continue
            titre3 = re.match(r"^###\s+([A-Za-z]+)\s*(?:\(([^)]*)\))?\s*$", ligne)
            if titre3 is not None and sous_agregats:
                qualificatif = titre3.group(2) or ""
                if " dans " in qualificatif:
                    continue
                if qualificatif == "" or "agrégat" in qualificatif or "référence" in qualificatif:
                    agregats.add(titre3.group(1))

    # (b) modules de la couche cliente : première colonne du tableau du §2
    debut = contenu_plan.find("**Les six modules de la couche cliente et leur ancrage.**")
    fin = contenu_plan.find("**L'agrégat couvre ses deux moitiés.**", debut) if debut != -1 else -1
    couche_cliente = set(re.findall(r"^\|\s*`([^`]+)`\s*\|", contenu_plan[debut:fin], re.M)) \
        if -1 not in (debut, fin) else set()

    # (c) modules d'outillage et (d) modules transverses : les puces du §2
    def noms_de_puce(marqueur):
        ligne = next((l for l in contenu_plan.splitlines() if marqueur in l), None)
        if ligne is None:
            return set()
        # Un renvoi de document est un libellé de lien markdown `` [`texte`](url) `` :
        # sa forme le distingue d'un nom de module, qui n'est jamais un lien.
        # On retire cette forme avant d'extraire les noms entre accents graves.
        sans_renvois = re.sub(r"\[`[^`]+`\]\([^)]*\)", "", ligne)
        return set(re.findall(r"`([^`]+)`", sans_renvois))
    outillage = noms_de_puce("un **module d'outillage**")
    transverses = noms_de_puce("un **module transverse nommé**")

    if not (agregats and couche_cliente and outillage and transverses):
        raise ErreurScript(
            f"la maille de désignation de {CHEMIN_PLAN_RELATIF} §2 n'a pas pu être lue "
            "en entier — le document a changé de forme, ce script doit être corrigé "
            f"(agrégats={len(agregats)}, couche cliente={len(couche_cliente)}, "
            f"outillage={len(outillage)}, transverses={len(transverses)})"
        )
    return {
        "Comportement": agregats,
        "Couche cliente": couche_cliente,
        "Outillage": outillage,
        "Socle": transverses,
    }


def extraire_champs_tache(bloc):
    champs = {}
    for cle, valeur in analyser_lignes_tableau(bloc):
        champs[cle] = valeur
    return champs


def extraire_taches(contenu_plan):
    motif_titre = re.compile(r"^##### (TB-\d+) — (.+)$", re.MULTILINE)
    correspondances = list(motif_titre.finditer(contenu_plan))
    taches = []
    for i, m in enumerate(correspondances):
        debut = m.end()
        fin = correspondances[i + 1].start() if i + 1 < len(correspondances) else len(contenu_plan)
        bloc = contenu_plan[debut:fin]
        # une tâche se termine avant le prochain filet horizontal isolé
        # (« --- » seul sur sa ligne), qui marque une frontière de section —
        # jamais avant une ligne de séparation de tableau (« |---|---| »)
        limite = re.search(r"^---\s*$", bloc, re.MULTILINE)
        if limite:
            bloc = bloc[: limite.start()]
        taches.append({
            "id": m.group(1),
            "titre": m.group(2).strip(),
            "champs": extraire_champs_tache(bloc),
        })
    return taches


def lister_slugs_wireframes(racine):
    base = racine / CHEMIN_WIREFRAMES_RELATIF
    slugs = set()
    if base.is_dir():
        for groupe in base.iterdir():
            if groupe.is_dir():
                for fiche in groupe.iterdir():
                    if fiche.is_dir():
                        slugs.add(fiche.name)
    return slugs


# ──────────────────────────────────────────────────────────────────────────
# Résolution jalon / couche d'une tâche
# ──────────────────────────────────────────────────────────────────────────

def resoudre_jalon(valeur_champ, jalons_valides):
    correspondance = re.match(r"^(J[0-3])\b", valeur_champ.strip())
    if correspondance and correspondance.group(1) in jalons_valides:
        return correspondance.group(1), None
    return None, f"valeur de jalon non résolue dans la taxonomie : {valeur_champ!r}"


def decouper_perimetre(valeur_champ):
    """Un champ dont la valeur commence par le marqueur d'absence « TROU »
    est indivisible : son texte d'explication peut lui-même contenir des
    points-virgules, qui ne sont alors pas des séparateurs de module."""
    valeur = valeur_champ.strip()
    if valeur.startswith("HORS MAILLE"):
        return [], "hors_maille"
    if valeur.startswith("TROU"):
        return [valeur], "trou"
    segments = [s.strip() for s in re.split(r"\s*;\s*", valeur) if s.strip()]
    return segments, None


def mapper_nature(segments, marqueur, maille, slugs_wireframes, natures_valides):
    """Déduit le libellé de nature de tranche depuis le périmètre d'écriture.
    Renvoie (libellés déduits, segments non résolus)."""
    if marqueur == "hors_maille":
        libelles = {"Hors maille"}
        return ({l for l in libelles if l in natures_valides},
                [] if libelles <= natures_valides
                else ["correspondance déduite vers « Hors maille », absente de la taxonomie lue"])
    if marqueur == "trou":
        return set(), []

    libelles, residuels = set(), []
    for segment in segments:
        correspondance = re.match(r"^fiche\s+`([^`]+)`$", segment)
        if correspondance is not None:
            if correspondance.group(1) in slugs_wireframes:
                libelles.add("Surface")
            else:
                residuels.append(f"{segment} — slug de fiche d'écran non reconnu sur disque")
            continue
        for nature, noms in maille.items():
            if segment in noms:
                libelles.add(nature)
                break
        else:
            # un projet .NET désigné par son rôle, non énuméré par la puce du §2
            if segment.startswith(("le projet ", "les autres projets")):
                libelles.add("Socle")
            else:
                residuels.append(segment)

    libelles_valides = {l for l in libelles if l in natures_valides}
    for l in libelles - libelles_valides:
        residuels.append(f"correspondance déduite vers « {l} », absente de la taxonomie lue")
    return libelles_valides, residuels


# ──────────────────────────────────────────────────────────────────────────
# Appels à `gh`
# ──────────────────────────────────────────────────────────────────────────

def gh_present():
    return shutil.which("gh") is not None


def gh_authentifie():
    resultat = subprocess.run(["gh", "auth", "status"], capture_output=True, text=True)
    return resultat.returncode == 0


def decoder_json_concatene(texte):
    """`gh api --paginate` peut renvoyer plusieurs documents JSON concaténés
    (une page par appel). On les décode un à un et on aplatit les listes,
    sans dépendre de jq."""
    decodeur = json.JSONDecoder()
    resultats = []
    position = 0
    texte = texte.strip()
    while position < len(texte):
        while position < len(texte) and texte[position] in " \t\r\n":
            position += 1
        if position >= len(texte):
            break
        objet, fin = decodeur.raw_decode(texte, position)
        resultats.append(objet)
        position = fin
    aplatis = []
    for objet in resultats:
        if isinstance(objet, list):
            aplatis.extend(objet)
        else:
            aplatis.append(objet)
    return aplatis


def gh_api_liste(chemin_api):
    resultat = subprocess.run(
        ["gh", "api", chemin_api, "--paginate"], capture_output=True, text=True
    )
    if resultat.returncode != 0:
        raise ErreurScript(f"lecture de {chemin_api} échouée : {resultat.stderr.strip()}")
    return decoder_json_concatene(resultat.stdout)


def gh_api_creer(chemin_api, champs_texte):
    commande = ["gh", "api", "-X", "POST", chemin_api]
    for cle, valeur in champs_texte.items():
        commande += ["-f", f"{cle}={valeur}"]
    resultat = subprocess.run(commande, capture_output=True, text=True)
    if resultat.returncode != 0:
        raise ErreurScript(resultat.stderr.strip() or "échec sans détail renvoyé par gh")
    return json.loads(resultat.stdout) if resultat.stdout.strip() else {}


# ──────────────────────────────────────────────────────────────────────────
# Compte rendu
# ──────────────────────────────────────────────────────────────────────────

class CompteRendu:
    def __init__(self):
        self.crees = []
        self.existants = []
        self.echecs = []

    def creation(self, libelle):
        self.crees.append(libelle)

    def existant(self, libelle):
        self.existants.append(libelle)

    def echec(self, libelle, raison):
        self.echecs.append((libelle, raison))

    def afficher(self, titre):
        print(f"\n{titre}")
        print(f"  créés       : {len(self.crees)}")
        for e in self.crees:
            print(f"    + {e}")
        print(f"  déjà présents : {len(self.existants)}")
        for e in self.existants:
            print(f"    = {e}")
        if self.echecs:
            print(f"  échecs      : {len(self.echecs)}")
            for e, raison in self.echecs:
                print(f"    ! {e} — {raison}")


# ──────────────────────────────────────────────────────────────────────────
# Corps du programme
# ──────────────────────────────────────────────────────────────────────────

MARQUEUR_SEPARATION_CORPS = (
    "── engendré — régénérable, écrasé à chaque exécution — "
    "sous cette ligne : écrit à la main ──"
)

ZONE_MANUELLE_PAR_DEFAUT = "\n\n## Pris par\n\n## Notes d'exécution\n\n## Écart constaté\n"


def decouper_identifiants_tranches(valeur_champ, separateur):
    """Découpe un champ `Dépend de` ou `En conflit avec` (methode-de-ticket.md §1)
    en identifiants `TB-nnn` référencés, sans retenir le texte explicatif qui les
    accompagne (« — module partagé : … », « recouvrement à confirmer à J0 »).
    Un segment qui ne commence pas par un identifiant (le cas « recouvrement à
    confirmer à J0 » du plan, §1 règle (c)) n'ajoute aucune cible : il qualifie
    toujours une tranche déjà nommée par un segment précédent du même champ,
    jamais une tranche supplémentaire. Renvoie une liste vide si le champ ne
    référence aucune tranche (valeur `—`)."""
    valeur = valeur_champ.strip()
    if not valeur or valeur == "—":
        return []
    identifiants = []
    for segment in valeur.split(separateur):
        correspondance = re.match(r"^\s*(TB-\d+)\b", segment)
        if correspondance:
            identifiants.append(correspondance.group(1))
    return identifiants


def construire_renvois_tranches(ids_tranches, numero_issue_par_tb):
    """Projette une liste d'identifiants `TB-nnn` en renvois `#<numéro>` —
    cliquables, et dont GitHub affiche l'état ouvert/fermé au premier coup d'œil
    (methode-de-ticket.md §1, « Ce que la zone engendrée porte »). La table
    TB-nnn -> numéro d'issue n'est jamais stockée (§6) : `numero_issue_par_tb`
    est reconstruite à chaque exécution depuis les titres d'issue existants.
    Une tranche référencée sans issue reste lisible, jamais un lien mort ni un
    numéro inventé."""
    if not ids_tranches:
        return "—"
    renvois = []
    deja_vus = set()
    for tb_id in ids_tranches:
        if tb_id in deja_vus:
            continue
        deja_vus.add(tb_id)
        numero = numero_issue_par_tb.get(tb_id)
        renvois.append(f"{tb_id} (#{numero})" if numero is not None else f"{tb_id} (pas encore d'issue)")
    return ", ".join(renvois)


def extraire_zone_manuelle(corps_existant):
    """Isole ce qui suit le marqueur de séparation dans un corps existant : cette
    zone n'est jamais régénérée (methode-de-ticket.md §1). Si le marqueur est
    absent (corps antérieur à ce modèle), le corps existant est reporté intégralement
    plutôt qu'écrasé — mieux vaut le préserver en entier que risquer d'effacer des
    notes déjà écrites à la main faute de savoir où elles commencent."""
    if not corps_existant:
        return ZONE_MANUELLE_PAR_DEFAUT
    index = corps_existant.find(MARQUEUR_SEPARATION_CORPS)
    if index == -1:
        return "\n\n" + corps_existant.strip("\n") + "\n"
    return corps_existant[index + len(MARQUEUR_SEPARATION_CORPS):]


def construire_corps_issue(tb_id, url_blob_plan, revision_plan, champs, numero_issue_par_tb,
                            corps_existant=None):
    """Construit le corps d'une issue conforme au modèle de `methode-de-ticket.md`
    §1 : une zone engendrée (renvoi vers la tâche de plan, renvois dérivés vers
    les tickets de `Dépend de` et `En conflit avec`, estampille de provenance),
    le marqueur de séparation, puis la zone écrite à la main — préservée telle
    quelle si `corps_existant` en porte une."""
    renvois_depend_de = construire_renvois_tranches(
        decouper_identifiants_tranches(champs.get("Dépend de", "—"), ","), numero_issue_par_tb
    )
    renvois_en_conflit = construire_renvois_tranches(
        decouper_identifiants_tranches(champs.get("En conflit avec", "—"), ";"), numero_issue_par_tb
    )
    zone_engendree = (
        f"**Tâche de plan** : [`{CHEMIN_PLAN_RELATIF}`]({url_blob_plan}) — {tb_id}\n"
        f"**Dépend de** : {renvois_depend_de}\n"
        f"**En conflit avec** : {renvois_en_conflit}\n"
        f"**Estampille de provenance** : {CHEMIN_PLAN_RELATIF}@{revision_plan} — {tb_id}"
    )
    return f"{zone_engendree}\n\n{MARQUEUR_SEPARATION_CORPS}{extraire_zone_manuelle(corps_existant)}"


def executer_operations(args):
    racine = trouver_racine_depot()
    chemin_plan = racine / CHEMIN_PLAN_RELATIF
    chemin_methode = racine / CHEMIN_METHODE_RELATIF
    for chemin, nom in ((chemin_plan, "plan de travail"), (chemin_methode, "méthode de ticket")):
        if not chemin.is_file():
            raise ErreurScript(f"{nom} introuvable : {chemin}")

    contenu_plan = chemin_plan.read_text(encoding="utf-8")
    contenu_methode = chemin_methode.read_text(encoding="utf-8")

    taxonomie = extraire_taxonomie(contenu_methode)
    maille = extraire_maille(contenu_plan, racine)
    slugs_wireframes = lister_slugs_wireframes(racine)
    taches = extraire_taches(contenu_plan)

    print(f"Racine du dépôt          : {racine}")
    print(f"Tâches lues dans le plan : {len(taches)}")
    print(f"Libellés « par jalon »   : {taxonomie['jalon']}")
    print(f"Libellés « par nature de tranche » : {taxonomie['nature_tranche']}")
    print(f"Libellés « par nature »  : {taxonomie['nature']}")
    print(f"Slugs de wireframes lus sur disque ({len(slugs_wireframes)}) : {sorted(slugs_wireframes)}")

    # résolution jalon / couche de chaque tâche, une fois pour toutes
    taches_sans_couche = []
    taches_segments_residuels = []
    for tache in taches:
        valeur_jalon = tache["champs"].get("Jalon", "")
        jalon, anomalie_jalon = resoudre_jalon(valeur_jalon, taxonomie["jalon"])
        tache["jalon"] = jalon
        tache["anomalie_jalon"] = anomalie_jalon

        valeur_perimetre = tache["champs"].get("Périmètre d'écriture", "")
        segments, marqueur = decouper_perimetre(valeur_perimetre)
        libelles_couche, residuels = mapper_nature(
            segments, marqueur, maille, slugs_wireframes, set(taxonomie["nature_tranche"])
        )
        tache["couches"] = libelles_couche
        tache["segments_residuels"] = residuels

        if not libelles_couche:
            raison = valeur_perimetre if marqueur else "aucun segment reconnu dans le périmètre d'écriture"
            taches_sans_couche.append((tache["id"], raison))
        elif residuels:
            taches_segments_residuels.append((tache["id"], residuels))

    print(f"\nTâches sans libellé de nature déductible ({len(taches_sans_couche)}) :")
    for tb_id, raison in taches_sans_couche:
        print(f"  - {tb_id} : {raison}")
    if taches_segments_residuels:
        print(f"\nTâches avec un périmètre partiellement déductible ({len(taches_segments_residuels)}) :")
        for tb_id, residuels in taches_segments_residuels:
            for r in residuels:
                print(f"  - {tb_id} : {r}")

    print("\nAucun libellé de nature n'est posé sur les issues : les fiches de "
          "tâche ne portent aucun champ correspondant. Les libellés de cet axe "
          "sont créés pour un usage ultérieur, non déductible ici.")

    if not args.appliquer:
        print("\nMODE À BLANC — rien n'est créé ni modifié sur GitHub.")

    disponible = gh_present()
    authentifie = disponible and gh_authentifie()

    if args.appliquer:
        if not disponible:
            raise ErreurScript(
                "gh (GitHub CLI) n'est pas installé. Installez-le "
                "(voir https://cli.github.com/) puis relancez ce script."
            )
        if not authentifie:
            raise ErreurScript(
                "gh n'est pas authentifié. Lancez « gh auth login » puis relancez ce script."
            )
    else:
        if not disponible:
            print("\ngh n'est pas installé : l'existant sur GitHub ne peut pas être "
                  "vérifié. L'aperçu ci-dessous ne reflète que le plan et la méthode "
                  "de ticket, pas ce qui existe déjà côté GitHub.")
        elif not authentifie:
            print("\ngh n'est pas authentifié : l'existant sur GitHub ne peut pas être "
                  "vérifié. L'aperçu ci-dessous ne reflète que le plan et la méthode "
                  "de ticket, pas ce qui existe déjà côté GitHub.")

    peut_verifier_existant = disponible and authentifie

    url_origin = lire_url_origin()
    owner, repo = deduire_owner_repo(url_origin)
    branche = lire_branche_courante()
    base_https = deduire_base_https(url_origin)
    commit = lire_commit_courant()
    url_blob_plan = f"{base_https}/blob/{commit}/{CHEMIN_PLAN_RELATIF}"
    revision_plan = lire_revision_plan()
    print(f"\nDépôt distant             : {owner}/{repo}")
    print(f"Lien de tâche utilisé      : {url_blob_plan} (identifiant de tâche en tête de titre)")
    print(f"Révision du plan (estampille de provenance) : {revision_plan}")

    if not commit_present_sur_distant(commit, branche):
        refs_verifiees = " ou ".join(dict.fromkeys([f"origin/{branche}", "origin/main"]))
        message = (
            f"la révision {commit} n'est pas (encore) présente sur le distant "
            f"({refs_verifiees}) : le lien de tâche ci-dessus renverrait "
            "une page introuvable tant qu'elle n'aura pas été poussée."
        )
        if args.appliquer:
            raise ErreurScript(
                message + " Poussez cette révision avant de créer des tickets, ou "
                "relancez sans --appliquer pour un aperçu."
            )
        print(f"\nAVERTISSEMENT : {message}")

    # ── 1. Libellés ──────────────────────────────────────────────────────
    toutes_valeurs_libelles = (
        [(v, "jalon") for v in taxonomie["jalon"]]
        + [(v, "nature_tranche") for v in taxonomie["nature_tranche"]]
        + [(v, "nature") for v in taxonomie["nature"]]
    )
    libelles_existants = set()
    if peut_verifier_existant:
        for item in gh_api_liste(f"repos/{owner}/{repo}/labels"):
            libelles_existants.add(item["name"])

    compte_libelles = CompteRendu()
    for nom_libelle, axe in toutes_valeurs_libelles:
        if nom_libelle in libelles_existants:
            compte_libelles.existant(f"{nom_libelle} ({axe})")
            continue
        if not args.appliquer:
            compte_libelles.creation(f"{nom_libelle} ({axe}) [à créer]")
            continue
        try:
            gh_api_creer(f"repos/{owner}/{repo}/labels", champs_texte={"name": nom_libelle})
            compte_libelles.creation(f"{nom_libelle} ({axe})")
        except ErreurScript as erreur:
            compte_libelles.echec(f"{nom_libelle} ({axe})", str(erreur))
    compte_libelles.afficher("Libellés")

    # ── 2. Jalons (milestones) ──────────────────────────────────────────
    jalons_existants = {}
    if peut_verifier_existant:
        for item in gh_api_liste(f"repos/{owner}/{repo}/milestones?state=all"):
            jalons_existants[item["title"]] = item["number"]

    compte_jalons = CompteRendu()
    numero_jalon = dict(jalons_existants)
    for nom_jalon in taxonomie["jalon"]:
        if nom_jalon in jalons_existants:
            compte_jalons.existant(nom_jalon)
            continue
        if not args.appliquer:
            compte_jalons.creation(f"{nom_jalon} [à créer]")
            continue
        try:
            cree = gh_api_creer(f"repos/{owner}/{repo}/milestones", champs_texte={"title": nom_jalon})
            numero_jalon[nom_jalon] = cree["number"]
            compte_jalons.creation(nom_jalon)
        except ErreurScript as erreur:
            compte_jalons.echec(nom_jalon, str(erreur))
    compte_jalons.afficher("Jalons")

    # ── 3. Issues, une par tâche ─────────────────────────────────────────
    ids_issues_existantes = set()
    # Jamais stockée (methode-de-ticket.md §6) : reconstruite à chaque exécution
    # depuis les titres d'issue existants, pour projeter `Dépend de` et
    # `En conflit avec` en renvois cliquables (construire_renvois_tranches).
    numero_issue_par_tb = {}
    corps_existant_par_tb = {}
    if peut_verifier_existant:
        for item in gh_api_liste(f"repos/{owner}/{repo}/issues?state=all&per_page=100"):
            if "pull_request" in item:
                continue
            correspondance = re.match(r"^(TB-\d+)\b", item.get("title", ""))
            if correspondance:
                tb_id_trouve = correspondance.group(1)
                ids_issues_existantes.add(tb_id_trouve)
                numero_issue_par_tb[tb_id_trouve] = item["number"]
                corps_existant_par_tb[tb_id_trouve] = item.get("body") or ""

    compte_issues = CompteRendu()

    retenues = None
    if args.seulement:
        retenues = {s.strip() for s in args.seulement.split(",") if s.strip()}
        connues = {x["id"] for x in taches}
        inconnues = sorted(retenues - connues)
        if inconnues:
            raise ErreurScript(
                "ces identifiants ne désignent aucune tâche du plan : " + ", ".join(inconnues))
        print(f"\nRestriction demandée : {len(retenues)} tâche(s) sur {len(taches)} "
              f"— {', '.join(sorted(retenues))}")
    for tache in taches:
        tb_id = tache["id"]
        if retenues is not None and tb_id not in retenues:
            continue
        titre_issue = f"{tb_id} — {tache['titre']}"

        corps = construire_corps_issue(
            tb_id, url_blob_plan, revision_plan, tache["champs"], numero_issue_par_tb,
            corps_existant=corps_existant_par_tb.get(tb_id),
        )
        if args.seulement:
            etat_corps = "issue existante — corps régénéré" if tb_id in ids_issues_existantes else "à créer"
            print(f"\n── Corps de l'issue {tb_id} ({etat_corps}) ──\n{corps}\n── fin du corps {tb_id} ──")

        if tb_id in ids_issues_existantes:
            compte_issues.existant(titre_issue)
            continue

        libelles_issue = sorted(tache["couches"])
        if tache["jalon"]:
            libelles_issue.append(tache["jalon"])

        if not args.appliquer:
            compte_issues.creation(f"{titre_issue} [à créer — libellés : {libelles_issue or 'aucun'}]")
            continue

        # gh api attend un « -f labels[]=valeur » répété pour un tableau ;
        # un dict à clé unique ne peut pas porter plusieurs valeurs pour
        # « labels[] », d'où la commande construite directement ici plutôt
        # que via gh_api_creer.
        try:
            commande = ["gh", "api", "-X", "POST", f"repos/{owner}/{repo}/issues",
                        "-f", f"title={titre_issue}", "-f", f"body={corps}"]
            for libelle in libelles_issue:
                commande += ["-f", f"labels[]={libelle}"]
            if tache["jalon"] and tache["jalon"] in numero_jalon:
                commande += ["-F", f"milestone={numero_jalon[tache['jalon']]}"]
            resultat = subprocess.run(commande, capture_output=True, text=True)
            if resultat.returncode != 0:
                raise ErreurScript(resultat.stderr.strip() or "échec sans détail renvoyé par gh")
            compte_issues.creation(titre_issue)
        except ErreurScript as erreur:
            compte_issues.echec(titre_issue, str(erreur))
    compte_issues.afficher("Issues (une par tâche)")

    print("\nRègle rappelée : le plan de travail reste la source de vérité ; "
          "aucune issue existante n'a été modifiée, fermée ou réétiquetée par ce script.")

    return len(compte_libelles.echecs) + len(compte_jalons.echecs) + len(compte_issues.echecs)


def construire_analyseur():
    analyseur = argparse.ArgumentParser(
        prog="creer-issues.py",
        add_help=False,
        description=(
            "Reporte le plan de travail versionné vers les issues GitHub du dépôt : "
            "libellés, jalons, puis une issue par tâche."
        ),
        epilog=(
            "Ordre d'exécution : (1) création des libellés lus dans la taxonomie de "
            "methode-de-ticket.md §3 (trois axes : jalon, nature de tranche, nature de vérification — aucun axe "
            "de priorité, explicitement interdit par la méthode) ; (2) création des "
            "jalons GitHub correspondant aux jalons de build lus dans la même "
            "taxonomie, via l'API (« gh milestone » n'existe pas dans les versions "
            "récentes de gh) ; (3) création d'une issue par tâche du plan, avec un "
            "titre verbatim (identifiant puis intitulé), un corps qui renvoie "
            "seulement vers le fichier du plan sans recopier son but, ses "
            "dépendances ni ses critères d'acceptation, et les libellés de jalon et "
            "de couche déduits du périmètre d'écriture de la tâche ; (4) un compte "
            "rendu de ce qui a été créé, de ce qui existait déjà, et de ce qui a "
            "échoué.\n\n"
            "Sans --appliquer, rien n'est écrit sur GitHub : le script affiche "
            "exactement ce qu'il créerait, ce qui existe déjà (si gh est disponible "
            "et authentifié), et les tâches pour lesquelles aucun libellé de couche "
            "n'a pu être déduit du plan."
        ),
        formatter_class=argparse.RawDescriptionHelpFormatter,
    )
    analyseur.add_argument(
        "--seulement", metavar="TB-nnn[,TB-nnn…]",
        help="restreint la création d'issues aux seules tâches nommées ; les libellés et "
             "les jalons, eux, sont créés en entier car ils sont partagés")
    analyseur.add_argument("-h", "--aide", action="store_true", help="affiche cette aide et quitte")
    analyseur.add_argument(
        "--appliquer", action="store_true",
        help="crée réellement les libellés, les jalons et les issues (sans ce drapeau : mode à blanc)",
    )
    return analyseur


def main():
    analyseur = construire_analyseur()
    args = analyseur.parse_args()
    if args.aide:
        analyseur.print_help()
        return 0
    try:
        return 1 if executer_operations(args) else 0
    except ErreurScript as erreur:
        print(f"Erreur : {erreur}", file=sys.stderr)
        return 1


if __name__ == "__main__":
    sys.exit(main())
