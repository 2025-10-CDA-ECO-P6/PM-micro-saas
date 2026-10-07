#!/usr/bin/env bash
# Régénère deux copies de l'outillage, sans la coupe de leur sortie, dans le dossier donné (par défaut, le scratchpad de la session).
#   corpus-checks-complet.sh  : le contrôle `template` imprime TOUS ses écarts (l'original s'arrête à 30 par catégorie)
#   conform-verify-complet.sh : `verify` imprime TOUTES les lignes perdues (l'original s'arrête à 20)
# Contournement du chantier conformite-corpus, sans toucher à .claude/ (outillage à refaire dans un cadrage séparé).
# Usage : bash generer-outils-complets.sh <dossier>
set -eu
R=$(cd "$(dirname "$0")/../../../.." && pwd)
OUT=${1:?dossier de sortie}
mkdir -p "$OUT"
sed -e "s|^_CC_HERE=.*|_CC_HERE=$R/.claude/skills/check|" -e 's/head -n 30 "\$CC_TMP/cat "$CC_TMP/' \
  "$R/.claude/skills/check/corpus-checks.sh" > "$OUT/corpus-checks-complet.sh"
sed -e "s|^HERE=.*|HERE=$R/.claude/skills/onboard|" -e 's/^head -n 20 "\$TMP\/lost.named"/cat "$TMP\/lost.named"/' \
  "$R/.claude/skills/onboard/conform-verify.sh" > "$OUT/conform-verify-complet.sh"
grep -c 'cat "$CC_TMP' "$OUT/corpus-checks-complet.sh" >/dev/null && grep -q 'cat "$TMP/lost.named"' "$OUT/conform-verify-complet.sh" \
  && echo "ok : $OUT/corpus-checks-complet.sh, $OUT/conform-verify-complet.sh"
