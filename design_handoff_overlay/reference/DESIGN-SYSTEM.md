# Cortinhos Design System

Design system for **Cortinhos** — a native Windows 11 desktop app (WPF / .NET 8): a system-tray **shortcut launcher for gamers**. It shows a dense grid of clickable tiles (apps, folders, URLs, commands), opens with a global hotkey (`Ctrl+Alt+L`), and adds a floating always-on-top **notch** surface (mic state, Discord Rich Presence, quick shortcuts) inspired by the macOS Dynamic Island.

Product name shown to users: **cortinhos** (the repo/binary is "Shortcut Launcher").

## Sources used

- **GitHub:** `Luckyx128/cortinhos` — https://github.com/Luckyx128/cortinhos The functional WPF codebase (C#). Read `README.md`, `MainWindow.xaml.cs`, `AddShortcutWindow.xaml.cs`, `ShortcutItem.cs`, `shortcuts.sample.json`, `App.xaml.cs`. Explore it further to ground new designs in the app's real behaviour (hotkey manager, tray icon, Discord presence watcher).

> ⚠️ The source repo ships **no visual design**: no XAML resource dictionaries, no colors, fonts, logo, or icons — only functional code and default Windows chrome. **This entire visual system was created from scratch** with the user, informed by an explicit brief (Windows 11 Fluent look + Steam-library grid density + macOS-notch behaviour). Treat every color, font, and radius here as a new decision, not a recovered brand.

## Design brief (agreed with user)

Windows 11 **Fluent Design** feel (rounded corners, subtle Mica translucency, single system accent) — **not** a 1:1 copy of Steam or macOS. Borrowed on purpose: Steam's **grid density + hover-to-open** and the macOS **floating-notch** behaviour, rendered in Cortinhos' own consistent language. Dark theme is the default; light is the secondary surface. Copy is mixed PT-BR / English, casual and gamer-friendly.

---

## Content fundamentals

- **Language:** Brazilian Portuguese first, with English kept where it's already idiomatic to gamers/devs (App, Link, Discord, Rich Presence, hotkey). Don't translate established product/tech terms.
- **Voice:** casual, direct, first-person-plural or impersonal — like a friend who codes. Mirrors the repo's own tone: *"Fecha a janela (X) só minimiza pra bandeja"*, *"Não consegui registrar o atalho global de…"*, *"Escolha outra combinação."* Contractions and dropped subjects are fine (*"pra"*, *"tá"*-adjacent register).
- **Casing:** Sentence case for UI copy and labels. Uppercase **only** for small eyebrow/caption labels (with `--tracking-caps`) and the optional uppercase wordmark lockup. Never ALL-CAPS body text.
- **I vs you:** address the user impersonally or as *você* implicitly ("Buscar atalho…", "Adicionar"). Error/system messages speak as the app in first person ("Não consegui…").
- **Buttons/labels:** short imperative verbs — *Adicionar, Salvar, Cancelar, Editar, Remover, Procurar, Abrir, Regravar, Abrir tudo*.
- **Emoji:** none. This is a native-app aesthetic — no emoji in UI, ever.
- **Numbers/keys:** keyboard shortcuts always in the mono font, `Ctrl+Alt+L` form (modifier order Ctrl → Alt → Shift → Win).
- **Vibe:** competent, quiet, gamer-adjacent. The UI stays out of the way; personality lives in the wordmark, the accent, and the notch's motion — not in chatty copy.

## Visual foundations

- **Color:** Deep petrol-graphite dark surfaces (never pure black — base `#14171c`), a stepped surface ramp (`bg-base → surface-1/2/3`) for elevation-by-lightness. A **single** vibrant accent, reserved for primary actions, selection, and focus. It is **user-controlled**: `--accent` resolves to `var(--system-accent, var(--brand-accent))`, so the Windows accent colour wins when the host supplies it; light azure `#3ea6ff` (dark) / `#1f7fe0` (light) is only the brand fallback. Never hard-code the hex — read `--accent` and let the derived hover/press/soft/ring variants follow. Green `#22c55e` = success/"rodando", warm red `#f0503c` = danger/mic-muted, amber `#f0a92c` = warning. Light theme mirrors the same roles on white/soft-grey with a slightly deeper accent for contrast.
- **Type:** **Chakra Petch** (techy, slightly condensed) for display/headings/tile names/wordmark — its "C" is the logo monogram, so changing it means redesigning the mark. **IBM Plex Sans** for body/UI (built for software; carries more character than Inter without losing legibility at 13px). **IBM Plex Mono** for keyboard shortcuts, paths, commands — the matched family means hotkeys sit naturally beside body copy. Scale: display 28 / title 20 / subtitle 15 / body 13 / caption 11.
- **Spacing:** 4px base unit (`--space-1…12`). Dense grid: `--tile-min` 150px columns, `--grid-gap` 14px, `--card-pad` 16px.
- **Backgrounds — chrome:** flat fills, no photographic imagery, **no gradients as decoration** on chrome or surfaces (one subtle top-lit window gradient at most). A very-faint grid texture was explored and **not** adopted — surfaces stay clean. Mica-style translucency + blur appears on floating layers (expanded notch, dialog scrim), never on base surfaces.
- **Backgrounds — tile cover art:** the "no decorative gradients" rule stops at the chrome. `ShortcutTile` takes a `cover` prop that stands in for per-app artwork, and the launcher renders `linear-gradient(150deg,…)` covers in **deliberately arbitrary hues** (blue, amber, green, purple) so a dense grid stays scannable — the same job Steam's box art does. These are placeholders for real icons/artwork, not a decorative flourish, and they are the one sanctioned use of colourful gradients in the system. Keep chrome flat; let covers carry the colour.
- **Cards / tiles:** `--radius-md` (6px), 1px `--border-subtle`, resting shadow `--elev-1`. Cover ratio **3:4** (Steam-like). Hover lifts (`translateY(-3px)`, `--elev-3`, border → `--border-strong`) and reveals the *Abrir* action over a bottom protection gradient. Press dips (`scale(.985)`).
- **Elevation:** soft, deep shadows tuned for dark. Ramp: `elev-1` rest → `elev-2` → `elev-3` hover → `elev-4` dialogs/menus → `elev-notch` floating notch (shadow + hairline ring).
- **Radii — matched to Windows 11:** sm 4 (inputs, chips, kbd — the OS control radius) · md 6 (tiles, buttons) · lg 8 (window, dialogs — the OS window radius) · pill 999 (badges, toggles) · **notch 8, applied to the bottom corners only** to imitate a physical notch cut (the OS flyout radius).
- **Motion:** light **spring** easing (`cubic-bezier(.34,1.56,.64,1)`), 150–250ms; `ease-out` for fades. Never linear/abrupt. Notch transitions grow width first, then height. Tiles use spring on transform.
- **Hover states:** surfaces lighten via `--hover-overlay` (6% white on dark); tiles lift; buttons brighten the accent (`--accent-hover`). **Press:** darker accent (`--accent-press`) + slight `scale` shrink.
- **Borders:** hairline `rgba(255,255,255,.08)` subtle / `.15` strong on dark; focus is the accent + a 3px `--accent-ring` glow.
- **Transparency & blur:** used only to signal a floating layer above everything — the expanded notch (`blur(24px)`) and dialog scrim (`blur(2px)`). Base app surfaces are opaque.
- **Iconography:** see below.

## Iconography

- **Set:** inline **[Solar Bold Duotone](https://icon-sets.iconify.design/solar/)** glyphs (CC-BY, ~7,400 icons), embedded as path data in `components/core/Icon.jsx` — no runtime CDN dependency. 24×24 box, `currentColor`.
- **Duotone means one color reads as two tones:** each glyph is a solid silhouette at `opacity .5` plus a full-opacity detail on top. So `<Icon color="var(--accent)">` gives depth for free — never pass two colors.
- **Exception:** the mic pair (`mic` / `mic-off`) comes from **[Hugeicons](https://icon-sets.iconify.design/hugeicons/)** — Solar ships no muted-microphone glyph. These two are **stroke-drawn, not duotone**, so they read slightly lighter than their neighbours. Accepted trade-off: the pair matches *each other*, which matters more for a toggle. If it ever bothers, swap in `solar:microphone-3-bold-duotone` for `mic` and accept a mismatched off-state instead.
- **Derived glyph:** `tick` (the bare checkmark used in Checkbox and Select rows) is the inner path of Solar's `check-square-bold-duotone`, extracted because Solar ships no standalone tick. `check` remains the full squared container.
- The source repo had **no icon set** (it used `SystemIcons.Application` for the tray), so this is a deliberate choice, not a recovered asset.
- **Shortcut types:** `app` (widget), `folder`, `url` (link-round), `command` (code-square) — one glyph per type, accent-colored in the tile's icon badge and in the type Select.
- **Notch glyphs:** `mic` / `mic-off` (turns `--danger` red when muted), `gamepad` (Discord presence, with a `--success` dot when active).
- **UI actions:** plus, x, edit, trash, search, settings, sun/moon (theme), chevron-down, check, play (Abrir), external, pin.
- **Adding an icon:** pull it from Solar as `solar:<name>-bold-duotone` so the two-tone structure stays consistent — never mix in a line-style icon, and never hand-draw one.
- **Emoji / unicode as icons:** never.

## Logo / brand mark

The source repo had **no logo**. An **original app icon was designed from scratch with the user** (concept "KP4"): a rounded-square of **curtain pleats** (a play on the name *Cortinhos* = "little curtains"), a **notch cutout** at the top edge with the two indicator dots (the signature floating-notch surface), and a white **geometric "C" monogram**. Files:
- `assets/logo.svg` — the icon mark. The "C" is a **traced vector path**, not live text, so it needs no font installed — safe for a production `.ico`/`.png` export. Works on dark + light; legible to 16px.
- `assets/logo-mini.svg` — favicon/tray variant: curtain + notch cutout only, no monogram. Use below ~20px where the "C" would mush.
- `assets/logo-lockup.svg` — mark + "cortinhos" wordmark for headers (wordmark uses Chakra Petch).

Wordmark alone (when the mark isn't used): `cor` in `--text-primary` + `tinhos` in `--accent` (Chakra Petch), or an uppercase `COR·TINHOS` lockup. See `guidelines/brand-icon.card.html` and `guidelines/brand-wordmark.card.html`.

---

## Native Windows platform notes
This is a WPF / .NET 8 app, not a web page. The rules below are the ones a web-only design system never has to state — they live in `tokens/platform.css`.

- **Materials.** Windows 11 has two backdrops and they are not interchangeable. **Mica** (`--material-mica`) is for long-lived surfaces — the main window; it tints the wallpaper, barely animates, and is cheap. **Acrylic** (`--material-acrylic`, `--material-acrylic-blur`) is for transient surfaces — context menu, dialog scrim, the expanded notch; it blurs what is behind the app and costs more. In WPF these come from `SystemBackdropType`; the CSS values here are the preview approximations.
- **Keyboard focus is not optional.** The launcher opens on a global hotkey, so most sessions are keyboard-only. Every tile, button and field must render `--focus-native` — Fluent's two-tone stroke (high-contrast outer ring + contrasting inner hairline) that survives any material. The softer `--ring` glow is for pointer/text-input focus only. Tab order in the grid should follow reading order, and Enter should fire the tile's primary action.
- **Window focus loss.** Windows dims inactive window chrome. Apply `--window-inactive-opacity` to the *title-bar contents* only (never the whole window) and swap `--window-border-active` for `--window-border-inactive`.
- **Disabled uses a token pair, not opacity.** `--text-disabled` / `--surface-disabled` / `--border-disabled` stay legible over any backdrop; a blanket `opacity` does not.
- **The OS motion setting is respected.** `prefers-reduced-motion` collapses `--dur-*` to 1ms and flattens the easings — the notch becomes an instant state change rather than a fast spring. Mirror this in WPF by checking `SystemParameters.ClientAreaAnimation`.
- **Sizes are DIPs.** All px tokens are device-independent pixels, so WPF scales them for 125/150/175/200% DPI automatically. Never hard-code a physical pixel size; never ship a bitmap where the vector mark (`assets/logo.svg`) will do.
- **Tray icon** is 16px and reads as a silhouette — use `assets/logo-mini.svg` (no monogram). See `--tray-icon-size`.

### Platform decisions (settled)
- **Native radii.** Matched to the OS: `--radius-lg` 8px (windows, dialogs), `--radius-md` 6px (tiles, buttons), `--radius-sm` 4px (controls), `--radius-notch` 8px (Win11 flyout radius). We deliberately gave up the friendlier 12/8 pair for native fidelity.
- **System accent, honoured.** `--accent` resolves to `var(--system-accent, var(--brand-accent))`. The WPF host sets `--system-accent` from `UISettings.GetColorValue(UIColorType.Accent)`; `--brand-accent` (#3ea6ff dark / #1f7fe0 light) is the fallback for previews and for hosts that don't provide one. Hover, press, soft and ring variants are derived with `color-mix()` from whichever wins, so honouring the system accent means setting **one** variable — nothing else needs to change. The logo keeps the brand blue, so the mark stays recognisable even when the UI takes on the user's colour.
- **High contrast: intentionally not supported yet.** Windows high-contrast modes would need a third theme scope beside `[data-theme="light"]`. Deferred by decision, not oversight.

### Assets that must not drift
Fonts are **self-hosted** in `assets/fonts/` and declared with `@font-face` in `tokens/fonts.css` — no CDN, works offline. Chakra Petch ships as four static faces (400/500/600/700); IBM Plex Sans is a **variable** font (one file covers 100–700 via `font-weight:100 700`); IBM Plex Mono ships 400/500/600. If you add a weight to the type scale, add the matching face or it will synthesise and look wrong.

## The notch — deep spec
The notch is the system's second surface: a floating, always-on-top pill docked to the top edge, separate from the main window. Full per-state specs live in the **Notch** card group (`guidelines/notch-*.card.html`); layout alternatives for the expanded panel are in `notch-layout-exploration.html`.

**Two independent axes.** Size (how much room it takes) and status (a transient event). A status decorates `compact`; expanding clears it from view.

| Size | Trigger | Width | Content |
|---|---|---|---|
| `compact` | idle | 92px | glyphs only — no text, ever |
| `peek` | hover | 232px min | a caption beside each glyph; read-only |
| `expanded` | click | 400px | real controls, two columns (layout B) |

| Status | Look | Dismissal |
|---|---|---|
| `alert` | `--accent-soft` fill, icon + short label | auto-recedes ~2.6s |
| `progress` | compact silhouette + 2px `--accent` hairline on the bottom edge | at 100% |
| `error` | `--danger-soft` fill, `--danger` border | **click to acknowledge — never auto-hides** |

Priority is `error` > `alert` > `progress`; only one status shows at a time.

**Expanded layout (B).** Left column: mic toggle (a real action, not a readout) + quick shortcuts (up to 5, icon-only + tooltip). Right column: Windows notifications (up to 3, with *Limpar*). Full-width below: performance (CPU/GPU/FPS; the bar turns `--danger` above 85%) → Discord Rich Presence with an inline tracking toggle → *Abrir tudo* with the keyboard hint. 400×357px measured on the real component.

The two columns are paired deliberately: mic+shortcuts (~102px) against the notification list (~120px) keeps them near-equal, so the row absorbs height instead of leaving dead space. Pairing mic+shortcuts against the perf module instead leaves ~71px empty and saves nothing — that was measured, not assumed.

**Notifications are unfiltered** — every app that posts a Windows notification reaches the notch. Up to 3 render in the panel; beyond that only the compact pill's count badge grows. No allow-list.

**Position.** Three anchors, all on the top edge: `top-left`, `top-center` (default), `top-right`. The notch is dragged between them and **snaps** to the nearest — never a free position, so it can't collide with the Windows clock. There is no bottom or side anchor: the inverted bottom-corner radius only reads when it's flush with the top.

**Multi-monitor.** The user picks the display in settings. It deliberately does *not* follow the running game — a surface that jumps screens mid-match is worse than one that stays put. Default is the primary monitor.

**Fullscreen.** Always visible, including over exclusive fullscreen — that is the whole point of the surface. Outside `expanded` the window is click-through except over the pill itself, so it never steals a click from the game.

**Motion.** compact → peek → expanded grows **width first, then height**, on `--dur-med` with `--ease-spring`. Never linear. Under `prefers-reduced-motion` every transition collapses to instant (see Native Windows platform notes).

**Materials.** Expanded uses the full `--material-acrylic` blur so it reads as a layer floating above everything; compact uses the lighter `--material-acrylic-thin`. This is the one place in the system where a surface blurs harder than the app itself.

**Taskbar language.** Compact and peek adopt the Windows 11 taskbar's exact grammar so the notch reads as part of the OS, not a third-party overlay: `--taskbar-height` 48px, `--taskbar-slot` 40px square hit targets with 22px glyphs, `--taskbar-gap` 4px, the taskbar's own `--taskbar-tint` fill (rgba(32,32,32,.78)) rather than `--bg-base`, its 8%-white hover wash at 4px radius, and the **accent underline indicator** — 16px for a running item, 20px for the focused one. See the `6 · Linguagem da taskbar` card, which sets the real component beside a taskbar mock.

### Open notch decisions
- ~~Expanded layout~~ — **decided: B.** Three options were drawn in `notch-layout-exploration.html`, with heights measured from the rendered page rather than estimated: **A** single column 340×481px, **B** two columns 400×357px as implemented (chosen), **C** tabs 340×312px. B wins because an overlay should not occupy half the screen, and C trades away the glanceability that justifies the surface. The exploration file stays as the record — it measures itself on load, so its numbers cannot drift.
- ~~Notification scope~~ — **decided: unfiltered.** Every app reaches the notch.
- **Perf module source.** CPU/GPU/FPS need a real provider (PDH counters, vendor SDK, or an FPS hook). Not specified. Note the overlay exploration's finding: **CPU/GPU are safe (PDH / LibreHardwareMonitor); FPS is not** — measuring it needs a swapchain hook, which anti-cheat treats as injection.

## Window vs. overlay — decided
The main surface becomes a **fullscreen overlay of floating islands** (`ui_kits/overlay/`), and the **window stays for management** (adding shortcuts, hotkeys, settings). Chosen treatment: **arrangement A "Constelação"** (islands hug the edges, shortcut grid as a bottom belt, screen centre left free for the game) · **vignette scrim** (edges darken, the middle of the game keeps breathing) · **notch separate** (the compact pill stays taskbar furniture; the islands are independent surfaces). The other axes remain switchable in the prototype as the record of the exploration.

What the exploration surfaced, and still governs the build:
1. **Technically viable, with two cuts.** Layered fullscreen window + click-through via a low-level mouse hook + acrylic *only on the islands* works. Exclusive-fullscreen coverage and FPS metering do not — both need injection.
2. **Global blur is out.** A scrim over the whole screen is fine; blurring the whole screen behind it is not. The design already assumes flat scrim + island-local acrylic.
3. **The notch and the overlay overlap.** If the overlay shows mic, notifications, perf and Discord as separate islands, four surfaces are duplicated. The "ilha-mãe" mode (notch opens expanded as the top island and absorbs them) resolves it — but then the overlay is "expanded notch + shortcut grid", which may be the real answer.

## Surface consistency — one product, three surfaces
Cortinhos shows three surfaces at once: the **launcher window**, the **compact notch**, and the **expanded notch**. They must read as one app without pretending to be the same thing. The rules that hold them together:

**What is shared (never diverge):**
- **Selection grammar.** An accent underline, `--taskbar-indicator-h` 3px with 2px radius, marks the selected/running thing everywhere: under a taskbar slot in the notch, and under the selected tab in the launcher. No accent-fill pills — that was the old tab treatment and it read as a different product.
- **Hover / press washes.** `--taskbar-hover` (8%) → `--taskbar-active` (12%) on any icon-sized hit target, in both surfaces.
- **Radii.** `--radius-sm` 4 / `--radius-md` 6 / `--radius-lg` 8, matched to Windows 11. Tiles compute 6px; the window and dialogs 8px.
- **Type, accent and icons.** One family set, one `--accent` (user-controlled), one Solar Bold Duotone glyph set.

**What differs on purpose:**
| | Fill | Blur | Why |
|---|---|---|---|
| Launcher window | `--material-mica` | none | Long-lived surface. Mica tints the wallpaper and barely animates. |
| Compact notch | `--taskbar-tint` rgba(32,32,32,.78) | `--material-acrylic-thin` | It deliberately matches the **taskbar**, not the app — it is OS furniture, not an app window. |
| Expanded notch | `--material-acrylic` | `--material-acrylic-blur` | Transient flyout floating above everything, so it blurs harder than the app itself. |

The compact notch being a *different grey* from the window bar is correct, not a bug: it belongs to the taskbar's visual family so the user reads it as part of Windows. The moment it expands it becomes an app flyout and switches to the app's acrylic.

**Native window chrome.** The title bar carries real Windows 11 caption buttons — `--caption-btn-w` 46 × `--caption-btn-h` 32, **zero corner radius** (they bleed into the window corner), minimise / maximise / close as geometric glyphs. Close is the only one that colours on hover, and it uses the fixed system red `--caption-close-hover` #c42b1c, **not** `--danger`. Earlier drafts used three round macOS-style dots; that was the single most un-native detail in the app and is gone.

## Components

Reusable primitives (React, styled via CSS custom properties), under `components/`. Namespace: `window.CortinhosDesignSystem_c0acc5`.

**core/** — `Icon`, `Badge`, `ShortcutTile` **forms/** — `Button`, `TextField`, `Select`, `Checkbox`, `ThemeToggle` **feedback/** — `ContextMenu`, `Tooltip`, `Dialog` **surface/** — `Notch` (see "The notch — deep spec")

The inventory follows the app's real surfaces (tile grid, add/edit dialog, tray menu, settings, notch). **Intentional additions** beyond the raw WPF source: `Icon` (the repo had no icon set — needed a glyph system) and `Badge`/`Tooltip` (needed for tile status, keyboard hints, and notch labels).

## UI kits

- **`ui_kits/launcher/`** — interactive recreation of the launcher window: grid, floating notch, add/edit dialog, right-click menu, tray menu, settings. See its `README.md`.
- **`ui_kits/overlay/`** — **exploration, not a decision.** "What if the window went away and the app became a Discord-style fullscreen overlay?" Dimmed screen, draggable floating islands, three arrangements (Constelação / Painel direito / Lista), four scrim treatments, three notch relationships, plus an in-page WPF feasibility note (`V`). See its `README.md`.

## Foundation cards

Specimen cards populate the Design System tab, under `guidelines/` (Colors, Type, Spacing, Brand) and each component directory (Components, Launcher).

## Root index / manifest

- `styles.css` — global entry; `@import`s the token files only.
- `tokens/` — `fonts.css`, `colors.css`, `typography.css`, `spacing.css`, `elevation.css`, `platform.css` (native Windows).
- `components/` — `core/`, `forms/`, `feedback/`, `surface/` (each: `Name.jsx` + `Name.d.ts` + `Name.prompt.md`, plus one `*.card.html`).
- `guidelines/` — foundation specimen cards (incl. the `notch-*` spec set), plus `font-recommendations.html` (typeface options and pairings).
- `notch-layout-exploration.html` — three layout options for the expanded notch panel.
- `ui_kits/launcher/` — interactive product recreation.
- `thumbnail.html` — homepage tile.
- `SKILL.md` — Agent-Skill entry point.
- `exploration.html` — the initial 3-direction visual exploration (scratch).

## Caveats / substitutions

- **Fonts are self-hosted** in `assets/fonts/` and declared with `@font-face` in `tokens/fonts.css` — no CDN, works offline. All three are OFL. See `guidelines/font-recommendations.html` for the alternatives that were considered and why these won.
- **Icons** are **Solar Bold Duotone** (CC-BY) with the mic pair from **Hugeicons** — chosen with the user; the source had no icon set.
- **Logo** is an original mark designed here with the user (not from the source, which had none) — `assets/logo.svg`.
- All colors, type, spacing, motion are **new decisions**, not recovered from the source.
