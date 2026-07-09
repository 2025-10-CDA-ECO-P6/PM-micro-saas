/* ═══════════════════════════════════════════════════════════════════════
   overlay.js — mécanisme générique d'ouverture/fermeture de sur-couches
   (modale, panneau latéral, menu déroulant) pour le prototype statique
   Haversack.

   JS vanilla, LOCAL, zéro dépendance (aucun réseau, aucun framework) —
   la contrainte du prototype est l'usage hors-ligne, pas l'absence de JS.

   Deux mécanismes cohabitent ici, tous deux pilotés par délégation
   d'événements sur `document` (aucun `querySelectorAll` à maintenir par
   page) :

   1. Modale / panneau latéral — `.modal` (site.css §4) et `.side-panel`
      (site.css §4), toutes deux pilotées par la classe d'état `is-open`
      posée sur la sur-couche elle-même (`#<id>` déclaré par le
      déclencheur).
   2. Menu déroulant (popover) — `.menu` (site.css §4), même classe d'état
      `is-open`, mais fermeture et positionnement différents (voir plus
      bas) : PAS de backdrop, ancré en `position:absolute` sous son
      déclencheur.

   ───────────────────────────────────────────────────────────────────────
   USAGE (à charger en <script defer> après site.css, ex. Phase 2) :

   A. Modale / panneau latéral

   1. Déclencheur d'ouverture — tout élément portant `data-open="<id>"` :
        <button type="button" data-open="mon-panneau">Ouvrir</button>
      ouvre l'élément `#mon-panneau` en lui ajoutant la classe `is-open`.

   2. Fermeture — trois façons équivalentes, toutes gérées ici :
        - un élément `[data-close]` À L'INTÉRIEUR de la sur-couche ouverte
          (ex. bouton "✕", bouton "Fermer") ;
        - un clic sur `.modal__backdrop` (ferme sa modale parente) ;
        - la touche Échap (ferme la sur-couche ouverte la plus récente,
          au sens de l'ordre du document).

   B. Menu déroulant (popover)

   1. Déclencheur — tout élément portant `data-menu="<id>"` :
        <span class="account-chip" data-menu="account-menu">…</span>
      ouvre/bascule l'élément `#<id>` (une `.menu`, site.css §4) en lui
      ajoutant/retirant `is-open`. Le menu est nichée en DOM à l'intérieur
      de son déclencheur (ancrage CSS `position:relative` posé sur le
      déclencheur, `position:absolute` posé sur `.menu` — voir site.css
      §4), ce qui fait que "cliquer en dehors du menu ET de son
      déclencheur" revient à "cliquer en dehors du déclencheur".

   2. Fermeture — trois façons équivalentes :
        - un clic sur un `.menu__item` (item du menu) : le menu se ferme,
          la navigation du lien suit son cours normal (pas de
          `preventDefault`) ;
        - un clic en dehors du menu et de son déclencheur ;
        - la touche Échap.

      Contrairement à la modale/au panneau, PAS de backdrop : le menu est
      un popover léger, pas une sur-couche bloquante.

   Aucune logique métier modélisée ici (pas de validation de formulaire, pas
   de filtrage de résultats) — uniquement la bascule visuelle `is-open`.
   Un agent qui ajoute une nouvelle sur-couche ou un nouveau menu n'a rien
   à écrire en JS : poser `data-open`/`data-close`/`data-menu` aux bons
   endroits suffit, ce script les câble automatiquement.
   ═══════════════════════════════════════════════════════════════════════ */
(function () {
  "use strict";

  var OVERLAY_SELECTOR = ".modal, .side-panel";
  var MENU_SELECTOR = ".menu";

  function openOverlay(id) {
    var overlay = document.getElementById(id);
    if (overlay) {
      overlay.classList.add("is-open");
    }
  }

  function closeOverlay(overlay) {
    overlay.classList.remove("is-open");
  }

  function closeMenu(menu) {
    menu.classList.remove("is-open");
  }

  function closeAllMenus() {
    var openMenus = document.querySelectorAll(MENU_SELECTOR + ".is-open");
    for (var i = 0; i < openMenus.length; i++) {
      closeMenu(openMenus[i]);
    }
  }

  function toggleMenu(id) {
    var menu = document.getElementById(id);
    if (!menu) {
      return;
    }
    var wasOpen = menu.classList.contains("is-open");
    closeAllMenus();
    if (!wasOpen) {
      menu.classList.add("is-open");
    }
  }

  document.addEventListener("click", function (event) {
    var opener = event.target.closest("[data-open]");
    if (opener) {
      event.preventDefault();
      openOverlay(opener.getAttribute("data-open"));
      return;
    }

    var closer = event.target.closest("[data-close]");
    if (closer) {
      event.preventDefault();
      var overlayToClose = closer.closest(OVERLAY_SELECTOR);
      if (overlayToClose) {
        closeOverlay(overlayToClose);
      }
      return;
    }

    var menuItem = event.target.closest(".menu__item");
    if (menuItem) {
      var parentMenu = menuItem.closest(MENU_SELECTOR);
      if (parentMenu) {
        closeMenu(parentMenu);
      }
      return; // pas de preventDefault : le lien de l'item navigue normalement
    }

    var menuTrigger = event.target.closest("[data-menu]");
    if (menuTrigger) {
      event.preventDefault();
      toggleMenu(menuTrigger.getAttribute("data-menu"));
      return;
    }

    // Clic ailleurs sur la page : ferme tout menu déroulant ouvert.
    closeAllMenus();
  });

  document.addEventListener("keydown", function (event) {
    if (event.key !== "Escape") {
      return;
    }

    var openMenus = document.querySelectorAll(MENU_SELECTOR + ".is-open");
    if (openMenus.length > 0) {
      closeAllMenus();
      return;
    }

    var openOverlays = document.querySelectorAll(".modal.is-open, .side-panel.is-open");
    if (openOverlays.length === 0) {
      return;
    }
    closeOverlay(openOverlays[openOverlays.length - 1]);
  });
})();
