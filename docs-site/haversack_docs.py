"""Hooks MkDocs pour publier le corpus Haversack tel qu'il est écrit pour GitHub.

1. Ancres : suffixe des titres dupliqués à la GitHub (`-1`, `-2`) au lieu de `_1` (Python-Markdown).
2. Markdown : GitHub accepte une liste ou un tableau collé au paragraphe qui le précède,
   Python-Markdown non. On insère la ligne vide manquante, hors blocs de code.
3. Navigation : les titres de section viennent des noms de dossiers (« Cas d utilisation »,
   « Sv1 transversaux », sans accents) ; la table SECTION_TITLES les remplace. Un dossier absent de la
   table garde le titre par défaut de MkDocs : rien ne casse, c'est seulement moins joli.
4. Liens GitHub : un lien qui sort de `docs/` (`../README.md`) ou qui vise un dossier (`specs/`) est valide
   sur GitHub, pas pour MkDocs. Le premier devient une URL `blob/` du dépôt ; le second pointe vers le
   `README.md` du dossier s'il existe, sinon vers l'URL `tree/` du dépôt.
5. Maquettes : le hub `interface/ecrans/hub.html` renvoie à `./README.md`, que le site publie en `./`.
"""
import posixpath
import re
from pathlib import Path

import markdown.extensions.toc as toc

# -- 1. ancres ---------------------------------------------------------------


def _unique_github(id, ids):
    """Rend `id` unique dans `ids` et l'y inscrit : `modes`, `modes-1`, `modes-2`, comme GitHub."""
    candidate = id
    n = 0
    while not candidate or candidate in ids:
        n += 1
        candidate = f"{id}-{n}"
    ids.add(candidate)
    return candidate


toc.unique = _unique_github

# -- 2. listes et tableaux ---------------------------------------------------

BULLET = re.compile(r"^\s*([-*+]|1[.)])\s+\S")  # un item « ordinaire » ou une liste ordonnée commençant à 1
ANY_ITEM = re.compile(r"^\s*([-*+]|\d+[.)])\s")
TABLE_ROW = re.compile(r"^\s*\|")
FENCE = re.compile(r"^\s*(```|~~~)")


def _is_plain_paragraph_line(line):
    """Vrai pour une ligne de paragraphe : ni vide, ni indentée, ni item, ni citation, ni titre, ni tableau, ni HTML."""
    if not line.strip() or line[0] in " \t>|<#":
        return False
    return not ANY_ITEM.match(line) and not re.match(r"^(-{3,}|\*{3,}|_{3,}|={3,})\s*$", line)


LINK = re.compile(r"\]\(([^)\s#:]+)((?:#[^)\s]*)?)\)")  # cible relative sans schéma, avec ou sans ancre


def _rewrite_links(line, page, config, files):
    """Voir le point 4 de l'en-tête. L'URL du dépôt vient de `repo_url` (mkdocs.yml), seul endroit où elle est écrite."""
    repo = config["repo_url"].rstrip("/")
    folder = posixpath.dirname(page.file.src_uri)

    def fix(match):
        target, anchor = match.group(1), match.group(2)
        if target.startswith("/") or not (target.endswith(".md") or target.endswith("/")):
            return match.group(0)
        resolved = posixpath.normpath(posixpath.join(folder, target))
        if resolved.startswith(".."):  # sort de docs/ : le dépôt est un niveau au-dessus
            return f"]({repo}/blob/main/{resolved[3:]}{anchor})"
        if target.endswith("/"):
            if files.get_file_from_path(posixpath.join(resolved, "README.md")) is not None:
                return f"]({target}README.md{anchor})"
            return f"]({repo}/tree/main/docs/{resolved}{anchor})"
        return match.group(0)

    return LINK.sub(fix, line)


def on_page_markdown(markdown, page, config, files):
    out, in_fence, prev = [], False, ""
    for line in markdown.split("\n"):
        if FENCE.match(line):
            in_fence = not in_fence
        elif not in_fence:
            line = _rewrite_links(line, page, config, files)
            if (BULLET.match(line) or TABLE_ROW.match(line)) and _is_plain_paragraph_line(prev):
                out.append("")
        out.append(line)
        prev = line
    return "\n".join(out)


# -- 3. titres de section ----------------------------------------------------

SECTION_TITLES = {
    "architecture": "Architecture",
    "architecture/deploiement": "Déploiement",
    "architecture/specs": "Spécifications pré-build",
    "assurance": "Assurance",
    "assurance/conformite": "Conformité",
    "assurance/conformite/cadres": "Cadres de conformité",
    "assurance/conformite/public": "Documents publics",
    "assurance/ecoconception": "Écoconception",
    "assurance/securite": "Sécurité",
    "besoin": "Besoin",
    "besoin/cas-d-utilisation": "Cas d'utilisation",
    "besoin/decouverte": "Découverte",
    "besoin/decouverte/personas": "Personas",
    "besoin/nfr": "Exigences non fonctionnelles",
    "besoin/parcours-utilisateur": "Parcours utilisateur",
    "besoin/parcours-utilisateur/bout-en-bout": "Parcours de bout en bout",
    "besoin/stories": "User stories",
    "besoin/vision": "Vision",
    "besoin/vision/perimetre": "Périmètre",
    "decisions": "Décisions (ADR)",
    "domaine": "Domaine",
    "domaine/content-library": "Content Library",
    "domaine/core": "Noyau partagé",
    "domaine/identity-access": "Identity & Access",
    "domaine/session-conduct": "Session Conduct",
    "domaine/space-management": "Space Management",
    "interface": "Interface",
    "interface/ecrans": "Écrans",
    "interface/ecrans/sv-session": "Session (sv-session)",
    "interface/ecrans/sv1-transversaux": "SV1 Transversaux",
    "interface/ecrans/sv2-entree-espace": "SV2 Entrée en espace",
    "interface/ecrans/sv3-preparation": "SV3 Préparation",
    "interface/ecrans/sv4-espace-personnel": "SV4 Espace personnel",
    "interface/ecrans/sv5-joueur": "SV5 Joueur",
    "projet": "Projet",
    "projet/revues": "Revues",
    "reference": "Documents de référence",
}


def _first_page(section):
    for child in section.children:
        if child.is_page:
            return child
        found = _first_page(child)
        if found:
            return found
    return None


def _retitle(items, depth=0):
    for item in items:
        if item.is_section:
            page = _first_page(item)
            if page:
                # le dossier de la section est le préfixe de son premier fichier, à la profondeur de la section
                folder = "/".join(page.file.src_uri.split("/")[: depth + 1])
                if folder in SECTION_TITLES:
                    item.title = SECTION_TITLES[folder]
            _retitle(item.children, depth + 1)


def on_nav(nav, config, files):
    _retitle(nav.items)
    return nav


# -- 5. hub des maquettes ----------------------------------------------------


def on_post_build(config):
    """Le hub (HTML copié tel quel) renvoie à `./README.md`, devenu la page d'accueil `./` du site."""
    hub = Path(config["site_dir"]) / "interface" / "ecrans" / "hub.html"
    hub.write_text(hub.read_text(encoding="utf-8").replace('href="./README.md"', 'href="./"'), encoding="utf-8")
