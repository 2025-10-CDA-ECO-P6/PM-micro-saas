# Politique de sanitisation HTML et CSP

> Spec pré-build, cluster sécurité & contrats API. Dérivée fidèlement de **ADR-016** (Sérialisation locale et contrat de migration, l.148, l.239) et **ADR-017** (Modèle IndexedDB local et sécurité du mode local, l.159, l.161, l.169-173, l.187, l.255, l.256). Statut : ADR-016 mixte à dominante pré-implémentation, ADR-017 mixte à dominante pré-implémentation (en-têtes des deux ADR) — à confirmer à l'entrée en build.
>
> **Portée** : cette spec formalise le plancher de sanitisation opposable et la posture CSP actée par les deux ADR. Aucune valeur laissée ouverte n'est fixée ici — tout point non tranché est marqué `[À TRANCHER — <ticket>]`. Les mentions de bibliothèques (`Konscious.Security.Cryptography`, `DOMPurify`, etc.) figurant dans les ADR sont **illustratives**, non reprises ici comme décision.

---

## 1. Plancher opposable — liste blanche positive des deux côtés

Le mécanisme de sanitisation est identique en posture côté serveur et côté client : une **liste blanche positive**, jamais une liste noire.

**Côté serveur** (ADR-016:148) :
> « La sanitisation est réalisée par **liste blanche positive** — seules les balises explicitement autorisées sont conservées. Les balises `<script>` et les attributs gestionnaires d'événements (`on*`) sont **interdits et supprimés sans exception**. Ce plancher constitue un critère d'acceptation non négociable, indépendamment de la bibliothèque ou de la liste exacte de balises choisie. »

**Côté client** (ADR-017:159) :
> « La sanitisation utilise `DomSanitizer` d'Angular avec une politique de **liste blanche positive** : seules les balises explicitement autorisées sont conservées ; les balises `<script>` et les attributs gestionnaires d'événements (`on*`) sont **supprimés sans exception**. »

**Symétrie actée** (ADR-017:161) :
> « Ce plancher est identique en posture à celui d'ADR-016 §2.4 (côté serveur) : même politique de liste blanche positive, mêmes interdictions absolues. »

### Interdictions absolues (sans exception, quelle que soit l'implémentation retenue)

- Balise `<script>` : interdite/supprimée des deux côtés (ADR-016:148, ADR-017:159).
- Attributs gestionnaires d'événements (`on*` — `onclick`, `onerror`, etc.) : interdits/supprimés des deux côtés (ADR-016:148, ADR-017:159).

Ces deux interdictions sont un **critère d'acceptation non négociable** (ADR-016:148) — elles ne dépendent pas du choix de bibliothèque ni de la liste exacte de balises retenue (§4).

---

## 2. Ordre de traitement à l'import JSON — validation puis sanitisation

L'ordre est impératif et fait partie du plancher opposable, pas d'un détail d'implémentation.

> ADR-017:169-173 — « Avant toute écriture dans IndexedDB, le fichier importé est soumis à deux vérifications :
> 1. **Validation structurelle** : le fichier est validé contre la structure attendue du payload (présence de `schemaVersion`, format des champs, types). Un fichier malformé est rejeté avec un message d'erreur.
> 2. **Sanitisation de contenu** : le contenu de chaque `document_block.content` est sanitisé selon le même plancher (…) — liste blanche positive, interdiction absolue des balises et attributs dangereux — avant écriture dans le store. »

> ADR-017:187 — « L'ordre est impératif : **valider la structure en premier** (éviter d'exécuter de la sanitisation sur un document structurellement incohérent), **sanitiser le contenu en second** (avant persistance, pas après lecture). Un fichier JSON importé n'est jamais écrit tel quel dans IndexedDB. »

Cette séquence — structure puis sanitisation, sanitisation avant écriture — s'applique à l'import JSON local (UC-01 A4). Elle est distincte du parcours de migration serveur (ADR-016 §2.4), qui applique sa propre sanitisation à l'import serveur, sur le même plancher.

---

## 3. Sanitisation côté client — moment et mécanisme

**Mécanisme** : `DomSanitizer` d'Angular (ADR-017:159, l.161).

**Deux moments de sanitisation, tous deux avant l'événement qu'ils protègent** :

- **Avant injection DOM** : le rendu de `document_block.content` est sanitisé côté client Angular **avant injection dans le DOM** (ADR-017 §4.1, l.159) — ferme le vecteur XSS lors de l'affichage en session.
- **Avant persistance IndexedDB** : le contenu importé via fichier JSON est sanitisé **avant écriture dans le store** (ADR-017:173, cohérent avec §2 ci-dessus) — évite qu'un contenu XSS non sanitisé soit persisté puis traverse la frontière locale→cloud à la migration.

> ADR-017:173 (contexte §4.2, posture CSP couplée à la sanitisation applicative) confirme que la sanitisation applicative et la CSP sont deux défenses complémentaires, pas substituables l'une à l'autre.

---

## 4. Posture CSP actée

> ADR-017:169-173 (§4.2) :
> - `default-src 'self'` : toutes les ressources par défaut restreintes à l'origine de l'application.
> - `script-src 'self'` : pas de script inline, pas de scripts depuis des sources tierces non explicitement autorisées.
> - Sources restreintes : assets, images, styles limités à l'origine et aux CDN explicitement approuvés.

> ADR-017:173 — « Cette CSP sert deux fonctions complémentaires. D'abord, c'est une défense en profondeur anti-XSS (…). Ensuite, elle rend **observable** la règle « aucune donnée envoyée au serveur en mode local » : (…) une CSP stricte avec `connect-src 'self'` (ou restreinte à l'API) permettrait de détecter toute tentative d'envoi non autorisée. »

| Directive | Valeur actée | Fonction |
|---|---|---|
| `default-src` | `'self'` | Restriction par défaut à l'origine de l'application |
| `script-src` | `'self'`, sans inline | Anti-XSS — aucun script inline ni tiers non approuvé |
| `connect-src` | `'self'` (ou restreint à l'API) | Rend observable « aucun envoi serveur en mode local » |

Ces trois directives sont **actées** par ADR-017 comme posture — elles ne sont pas illustratives. Le détail complet (directives exhaustives restantes, valeurs de nonce) reste ouvert (§5).

---

## 5. `[À TRANCHER]` — points laissés ouverts par les deux ADR

| Point ouvert | Ticket | Source |
|---|---|---|
| Liste exhaustive des balises autorisées, côté serveur | **B1.2** | ADR-016 §Points à trancher ; ADR-016:239 (table des dettes) |
| Liste exhaustive des balises autorisées, côté client | **P6** | ADR-017 §Points à trancher ; ADR-017:255-256 (table des dettes) |
| Bibliothèque de sanitisation exacte (serveur et client) | **B1.2** (serveur) / **P6** (client) | ADR-016:239 ; ADR-017:256 |
| Directives CSP complètes (liste exhaustive des directives, valeurs de nonce) | **P6** | ADR-017:255 (table des dettes), ADR-017 §Points à trancher |

Ces points sont explicitement nommés comme dettes non silencieuses dans les deux ADR — ils ne sont pas des omissions, mais des décisions renvoyées à l'implémentation.

---

## 6. Maillons NON-VÉRIFIABLE-EN-CI — à nommer, pas à prétendre couvrir

Ces deux points sont des comportements d'environnement d'exécution réel, non assertables par les tests d'archi CI. Ils ne relèvent pas d'un manque de couverture à corriger, mais d'une limite structurelle des tests applicatifs — cette spec les nomme pour éviter toute fausse confiance dans la couverture CI.

**`navigator.storage.persist()` / éviction IndexedDB** (ADR-017:143) :
> « Le comportement de grant ou de refus de `navigator.storage.persist()` par le navigateur, et le comportement d'éviction IndexedDB sous pression mémoire, sont des comportements d'environnement d'exécution **non assertables en CI**. La logique applicative (lire le booléen retourné, déclencher le bandeau si false) est testable par mock de l'API `navigator.storage`. Le comportement réel du navigateur ne l'est pas. »

**Migration IndexedDB multi-versions en navigateur réel** (ADR-016:229) :
> « La migration réelle depuis un IndexedDB multi-versions en navigateur n'est pas prouvable en CI : le comportement de la fonction de projection `store local → payload` sur différentes versions de l'IndexedDB, dans des navigateurs réels, avec des données persistées par des versions antérieures de l'application, ne peut pas être couvert par les tests d'archi CI. Cette vérification relève de tests e2e navigateur renvoyés à **P7**. »

Ce que ces deux maillons impliquent pour cette spec : **la sanitisation applicative elle-même (§1-§3) est testable en CI** (mock des APIs navigateur, payloads synthétiques) — ce sont les comportements *environnementaux* qui l'entourent (grant/refus réel du navigateur, upgrade IndexedDB réel) qui ne le sont pas. Ne pas confondre les deux : cette spec ne prétend pas que la sanitisation couvre ces deux maillons, ni que les tests d'archi CI les couvrent.

---

## Confirmation de fidélité

- Aucun exemple illustratif d'ADR (bibliothèques nommées à titre d'exemple, `DOMPurify`, `Konscious.Security.Cryptography`) n'est promu ici en décision — ils ne sont pas repris comme choix arrêté.
- Les deux maillons non-vérifiables-en-CI (§6) sont nommés explicitement, conformément aux ADR sources, et ne sont pas présentés comme couverts.
