# Tokens → WPF

Fonte: `reference/tokens/*.css` (dark = default, light = `[data-theme="light"]`). Todos os px são **DIPs** — o WPF escala para 125/150/175/200% sozinho. Nunca hard-code pixel físico.

Estrutura sugerida:
```
Theme/Tokens.Dark.xaml      cores + overlays
Theme/Tokens.Light.xaml     mesmos keys, valores do light
Theme/Tokens.Shared.xaml    raios, espaçamento, tipografia, sombras, plataforma
Theme/Controls.xaml         estilos dos primitivos
```
Trocar tema = swappar o dicionário de cores em `Application.Current.Resources.MergedDictionaries[0]`.

---

## Cor — dark (default)

| Token | Hex / valor | Uso |
|---|---|---|
| `--bg-base` | `#14171c` | fundo base (nunca preto puro) |
| `--bg-window` | `#171b21` | fill da janela principal (Mica por cima) |
| `--surface-1` | `#1c2129` | card / tile em repouso |
| `--surface-2` | `#232a33` | hover de card, inputs, linhas do notch |
| `--surface-3` | `#2b333d` | pressed, menu elevado, track de barra |
| `--surface-overlay` | `rgba(28,33,41,.72)` | painéis acrylic sobre conteúdo |
| `--border-subtle` | `#FFFFFF` @ 8% | hairline padrão |
| `--border-strong` | `#FFFFFF` @ 15% | hover de tile, chrome ativo |
| `--text-primary` | `#eceef2` | |
| `--text-secondary` | `#9aa3ad` | |
| `--text-tertiary` | `#6b7480` | labels, eyebrow |
| `--text-on-accent` | `#0a1119` | texto sobre fill accent |
| `--success` | `#22c55e` | "rodando" |
| `--danger` | `#f0503c` | mic mutado, destrutivo |
| `--danger-soft` | `rgba(240,80,60,.15)` | fill do status error |
| `--warning` | `#f0a92c` | |
| `--hover-overlay` | `#FFFFFF` @ 6% | wash de superfície |
| `--press-overlay` | `#000000` @ 18% | |
| `--scrim` | `rgba(6,8,11,.6)` | backdrop de modal (+ `blur(2px)`) |

## Cor — light (`[data-theme="light"]`)
`bg-base #eef0f3` · `bg-window #f5f6f8` · `surface-1 #ffffff` · `surface-2 #f1f3f6` · `surface-3 #e7eaee` · `surface-overlay rgba(255,255,255,.78)` · border subtle `#0f141c` @10% / strong @18% · text `#151a21` / `#586170` / `#8a929e` · on-accent `#ffffff` · success `#15a349` · danger `#d63a26` (soft 12%) · warning `#c9820c` · hover `#0f141c` @5% · press @9% · scrim `rgba(20,25,32,.35)`.

## Accent — controlado pelo usuário

```
--accent = system accent ?? --brand-accent
--brand-accent = #3ea6ff (dark) / #1f7fe0 (light)
```
O host lê `UISettings.GetColorValue(UIColorType.Accent)`; o hex da marca é só fallback. **Nunca hard-code o azul na UI** — derive tudo de uma variável só:

| Variante | Receita (CSS `color-mix in oklch`) | Em C# |
|---|---|---|
| `--accent-hover` | accent 82% + white | clarear ~18% (mix linear em sRGB é aceitável) |
| `--accent-press` | accent 86% + black | escurecer ~14% |
| `--accent-soft` | accent 14% + transparent | mesma cor, alpha `0.14` |
| `--accent-ring` | accent 45% + transparent | alpha `0.45` |

O **logo mantém o azul da marca** mesmo quando a UI assume a cor do usuário.

## Raios (casados com o Windows 11)
`--radius-sm 4` inputs, chips, kbd, slots · `--radius-md 6` tiles, botões · `--radius-lg 8` janela, diálogos · `--radius-pill 999` badges, toggles · `--radius-notch 6` **só os cantos de baixo** do notch.

## Espaçamento — base 4px
`space-1..12` = 4 · 8 · 12 · 16 · 20 · 24 · 32 · 40 · 48.
Layout: `--grid-gap 14` · `--tile-min 150` (coluna mínima do grid denso) · `--card-pad 16` · `--window-pad 24`.

## Tipografia

| | Família | Uso |
|---|---|---|
| `--font-display` | **Chakra Petch** | display, headings, nomes de tile, wordmark, números de perf |
| `--font-body` | **IBM Plex Sans** (variável) | corpo e UI |
| `--font-mono` | **IBM Plex Mono** | hotkeys, paths, comandos, timestamps |

Escala (px / line-height): display **28/32** · title **20/26** · subtitle **15/20** · body **13/18** · caption **11/14** · kbd **11**.
Pesos: 400 / 500 / 600 / 700. Tracking: display `.01em`, caps `.08em`.
Se adicionar um peso à escala, adicione a face — senão o WPF sintetiza e fica errado. Chakra Petch são 4 faces estáticas; IBM Plex Sans é uma variável (100–700).

## Elevação e movimento

| Token | Valor | Uso |
|---|---|---|
| `--elev-1` | `0 1px 2px rgba(0,0,0,.28)` | tile em repouso |
| `--elev-2` | `0 2px 8px rgba(0,0,0,.34)` | notch compact |
| `--elev-3` | `0 6px 18px rgba(0,0,0,.42)` | hover de tile |
| `--elev-4` | `0 14px 40px rgba(0,0,0,.5)` | diálogos, context menu, notch arrastando |
| `--elev-notch` | `0 10px 34px rgba(0,0,0,.55)` + ring 1px subtle | painel expandido |
| `--ring` | `0 0 0 3px accent@45%` | foco de ponteiro/texto |
| `--ease-spring` | `cubic-bezier(.34,1.56,.64,1)` | transforms, crescimento do notch → `BackEase A=0.35 EaseOut` |
| `--ease-out` | `cubic-bezier(.22,.61,.36,1)` | fades → `CubicEase EaseOut` |
| `--dur-fast` / `--dur-med` | `150ms` / `220ms` | |

WPF não tem `box-shadow` múltiplo: use `DropShadowEffect` (BlurRadius ≈ 2× o blur do CSS, ShadowDepth = offset Y, Opacity = alpha) + um `Border` extra para o ring.

## Plataforma (`reference/tokens/platform.css`)

**Materiais** — `Mica` para superfícies de vida longa (janela principal); `Acrylic` para transitórias (context menu, scrim de diálogo, notch expandido). `DwmSetWindowAttribute(DWMWA_SYSTEMBACKDROP_TYPE)`: `2 = Mica`, `3 = Acrylic/Transient`, `4 = MicaAlt`.

**Foco de janela** — Windows escurece o chrome inativo. Aplique `--window-inactive-opacity .62` **só no conteúdo da title bar** (nunca na janela toda) e troque `border-strong` → `border-subtle`.

**Focus nativo** — `--focus-native` = `2px #ffffff` + `3px #000000` (invertido no light). Obrigatório em tile, botão e campo.

**Disabled é par de tokens, não `Opacity`** — `--text-disabled #5b636d` · `--surface-disabled #FFFFFF @4%` · `--border-disabled #FFFFFF @6%` (light: `#9aa2ac` / `#0f141c @4%` / `@7%`).

**Taskbar** (o vocabulário do notch) — `height 48` · `tint rgba(32,32,32,.78)` (light `rgba(243,243,243,.82)`) · `hairline #FFFFFF @7%` na borda que encara o conteúdo · `slot 40` · `icon 22` · `gap 4` · `slot-radius 4` · `hover @8%` · `active @12%` · indicador `16×3`, `20` no focado.

**Caption buttons** — `46×32`, **raio zero** (sangram no canto da janela), glifos geométricos. Hover `#FFFFFF @6%`, press `@9%`. Close é o único que colore: vermelho fixo do sistema `#c42b1c` (press `#b2231a`) — **não** `--danger`. Sem bolinhas estilo macOS.

**Tray** — `--tray-icon-size 16`, silhueta, `logo-mini.svg`.

**Movimento do OS** — `SystemParameters.ClientAreaAnimation == false` → transições instantâneas.

**High contrast** — **fora de escopo por decisão** (exigiria um terceiro escopo de tema).

---

## Gramática compartilhada entre as 3 superfícies

Launcher, notch compact e notch expanded precisam ler como um app só, sem fingir que são a mesma coisa.

**Nunca divergir:** o **underline accent** (`3px`, raio 2) é a marca de selecionado/rodando em todo lugar — sob o slot no notch e sob a aba selecionada no launcher (**nunca** pill com fill accent). Washes de hover 8% → press 12% em qualquer alvo do tamanho de ícone. Raios 4/6/8. Uma família de tipo, um `--accent`, um set de glifos.

**Divergir de propósito:** launcher = Mica sem blur; notch compact = tint da taskbar + acrylic fino; notch expanded = acrylic forte. O compact ser de um cinza diferente da janela é a intenção — ele pertence à taskbar.

## Snippet de partida

```xml
<!-- Theme/Tokens.Dark.xaml -->
<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">
  <Color x:Key="BgBaseColor">#FF14171C</Color>
  <SolidColorBrush x:Key="BgBase" Color="{StaticResource BgBaseColor}"/>
  <SolidColorBrush x:Key="Surface2" Color="#FF232A33"/>
  <SolidColorBrush x:Key="TextPrimary" Color="#FFECEEF2"/>
  <SolidColorBrush x:Key="TextTertiary" Color="#FF6B7480"/>
  <SolidColorBrush x:Key="BorderSubtle" Color="#14FFFFFF"/>
  <SolidColorBrush x:Key="TaskbarTint" Color="#C7202020"/>
  <SolidColorBrush x:Key="TaskbarHover" Color="#14FFFFFF"/>
  <SolidColorBrush x:Key="Danger" Color="#FFF0503C"/>
  <!-- Accent: sobrescrito em runtime pelo accent do Windows -->
  <SolidColorBrush x:Key="Accent" Color="#FF3EA6FF"/>
  <CornerRadius x:Key="RadiusSm">4</CornerRadius>
  <CornerRadius x:Key="RadiusNotch">0,0,6,6</CornerRadius>
  <system:Double xmlns:system="clr-namespace:System;assembly=mscorlib" x:Key="TaskbarSlot">40</system:Double>
</ResourceDictionary>
```
Alpha em hex ARGB: 4% = `0A`, 6% = `0F`, 7% = `12`, 8% = `14`, 9% = `17`, 12% = `1F`, 15% = `26`, 45% = `73`, 72% = `B8`, 78% = `C7`.
