# Fase overlay — spec de implementação

Fonte da verdade da geometria: `reference/ui_kits/overlay/OverlayApp.jsx.txt` (posições, scrim, pílula) e `reference/ui_kits/overlay/Islands.jsx.txt` (conteúdo interno). Abra `reference/ui_kits/overlay/index.html` e meça o que precisar — os números abaixo saíram de lá.

Todos os px são **DIPs**. O protótipo usa um monitor de referência de **1440 × 860 DIPs**; posições são dadas como **âncora + offset**, não como porcentagem (ver §3).

---

## 1. Comportamento, em uma frase

`Ctrl+Alt+L` escurece o monitor escolhido com uma vinheta e faz aparecer, em spring de 220ms, um conjunto de ilhas flutuantes encostadas nas bordas. O centro da tela fica livre. Fora das ilhas o clique **passa pro jogo**. `Esc` ou o mesmo hotkey fecham.

---

## 2. Arquitetura de janelas — a decisão mais importante

**Uma janela por ilha, mais uma janela de scrim.** Não é uma janela de tela cheia com controles dentro.

```
OverlayScrimWindow      tela cheia · WS_EX_TRANSPARENT permanente · pinta só a vinheta
IslandWindow × 9        uma por ilha · aceita clique normalmente · acrylic nativo
```

Por que assim, e não uma janela só:

| | Janela única | Uma janela por ilha |
|---|---|---|
| Click-through | precisa de hook global de mouse (`WH_MOUSE_LL`) ligando/desligando `WS_EX_TRANSPARENT` conforme a posição do cursor | **de graça** — o scrim é sempre transparente ao clique, as ilhas são janelas normais |
| Acrylic por ilha | impossível (WPF não tem `backdrop-filter` por elemento) | **`DesktopAcrylicBackdrop` nativo** em cada `IslandWindow` |
| Arrastar | matemática de layout + recalcular região | mover a janela |
| Custo | 1 HWND, composição de tela cheia todo frame | 10 HWNDs, cada um pintando só a própria área |

A nota de viabilidade original (`reference/ui_kits/overlay/Feasibility.jsx.txt`, item 2) recomendava o hook de mouse. **Isto substitui aquela recomendação** — o hook fica como plano B se as 10 janelas derem problema de z-order.

### Flags de janela

**`OverlayScrimWindow`**
```
WindowStyle=None  AllowsTransparency=True  ShowInTaskbar=False  Topmost=True
WS_EX_LAYERED | WS_EX_TRANSPARENT | WS_EX_TOOLWINDOW | WS_EX_NOACTIVATE
Left/Top/Width/Height = bounds do monitor escolhido (o de settings, não o do jogo)
```
`WS_EX_TRANSPARENT` é permanente aqui — o scrim **nunca** captura clique.

**`IslandWindow`** (uma classe, nove instâncias)
```
WindowStyle=None  ShowInTaskbar=False  Topmost=True  ResizeMode=NoResize
WS_EX_TOOLWINDOW | WS_EX_NOACTIVATE
SystemBackdrop = DesktopAcrylicBackdrop      (fase 2; fase 1 pode sair opaca)
```
`WS_EX_NOACTIVATE` mantém o foco no jogo — **exceto na ilha de busca**, ver §6.

Se a fase 1 sair sem acrylic, use fill **opaco** `#1E232B` (que é `--surface-overlay` composto sobre a vinheta). É a divergência aceita do README.

### Z-order
Mostrar na ordem: scrim → ilhas. Todas `Topmost`. A `NotchWindow` (pílula) fica **acima** das ilhas. Se o z-order brigar, reafirme com `SetWindowPos(HWND_TOPMOST)` na ordem certa depois de todas aparecerem, uma vez.

### Fora do overlay
Quando fechado, **todas as dez janelas somem** (`Hide()`, não `Close()` — criar 10 HWNDs por abertura é lento). Só a `NotchWindow` continua de pé.

---

## 3. Geometria — arranjo A "Constelação"

Monitor de referência 1440 × 860. **Tamanhos são fixos em DIPs** (uma ilha de 264 DIPs não deve virar 500 num monitor 4K); **posições são âncora + offset**, medidos da borda ancorada.

| Ilha | Âncora | Offset (dip) | Tamanho (dip) |
|---|---|---|---|
| Pílula (notch) | top-center | `0, 0` | auto × **48** |
| Busca | top-center | `0, 140` | **460** × auto |
| Mic + áudio | top-left | `24, 206` | **264** × auto |
| Notificações | top-left | `24, 286` | **264 × 200** |
| Favoritos | top-left | `24, 502` | **264** × auto |
| Desempenho | top-right | `24, 336` | **264** × auto |
| Discord | top-right | `24, 206` | **264** × auto |
| Config (engrenagem) | top-right | `24, 24` | **44 × 44** |
| Grid de atalhos | bottom-center | `0, 18` | **848 × 364** |

Duas regras que caem daí:
- **A faixa central fica vazia.** Nada entre `x = 288` e `x = 1152` acima de `y = 478`, tirando a pílula e a busca no topo. Isso é o ponto do arranjo — não preencha.
- **A margem das bordas é 24 DIPs** em tudo, exceto o grid, que respira 18 do rodapé.

### Encaixe (fase 3)
Ao arrastar, encaixa quando o candidato está a **≤ 14 DIPs**:
- borda esquerda `x = 24` · borda direita `x = larguraTela − larguraIlha − 24`
- centro horizontal `x = (larguraTela − larguraIlha) / 2`
- topo `y = 24` · rodapé `y = alturaTela − alturaIlha − 24` · **topo rente `y = 0`** (só pra pílula)

Sempre clampeie pra dentro da tela.

---

## 4. Materiais e chrome

### Scrim — vinheta
Uma `RadialGradientBrush` na `OverlayScrimWindow`:
```xml
<RadialGradientBrush Center="0.5,0.48" GradientOrigin="0.5,0.48"
                     RadiusX="0.31" RadiusY="0.275">
  <GradientStop Color="#1A06080B" Offset="0"/>
  <GradientStop Color="#DB06080B" Offset="1"/>
</RadialGradientBrush>
```
(CSS: `radial-gradient(ellipse 62% 55% at 50% 48%, rgba(6,8,11,.10), rgba(6,8,11,.86))`. CSS dá diâmetro, WPF dá raio — daí 62%→0.31 e 55%→0.275.)

Fade de entrada/saída: **220ms `--ease-out`** = `cubic-bezier(.22,.61,.36,1)`. Nunca linear.

### Chrome da ilha
| Propriedade | Valor |
|---|---|
| Padding interno | **16** em volta |
| Fill | `--surface-overlay` `#B81C2129` (= rgba(28,33,41,.72)) sobre acrylic |
| Borda | 1 DIP `--border-subtle` = branco @ 8% |
| Raio | `--radius-lg` = **8** |
| Sombra | `--elev-4` = `0 14 40 rgba(0,0,0,.5)` → `DropShadowEffect BlurRadius=40 ShadowDepth=14 Direction=270 Opacity=.5` |

### Pílula reduzida (com o overlay aberto)
A `NotchWindow` **não** mostra mic, Discord e contagem enquanto o overlay está aberto — as ilhas já dizem isso. Ela cai pra identidade:

| Propriedade | Valor |
|---|---|
| Altura | `--taskbar-height` **48** |
| Padding | `0 12` → `0 14` no hover |
| Fill | `--taskbar-tint` `#C7202020` (= rgba(32,32,32,.78)) |
| Backdrop | `--material-acrylic-thin` — acrylic mais fino que o das ilhas |
| Borda | 1 DIP subtle, **sem borda no topo** |
| Raio | `0 0 8 8` — só os cantos de baixo, imitando o corte físico |
| Sombra | `--elev-2` + hairline interno no **rodapé** (`inset 0 -1 0 --taskbar-hairline`) |
| Conteúdo | `logo-mini.svg` 22×22 · ponto 7 DIPs · gap 8 |
| Ponto | `--success` verde · **`--warning` âmbar se o mic estiver mutado** |
| Hover | revela a legenda "Overlay ativo" (caption, medium, `--text-secondary`) |

Com o overlay **fechado** ela volta ao `compact` completo do pacote do notch — ali é a única superfície, então precisa falar.

---

## 5. As ilhas por dentro

Cabeçalho comum a todas (menos busca e engrenagem): rótulo em **caption 11 / semibold / uppercase / `--tracking-caps` / `--text-tertiary`**, `margin-bottom: 10`. Quando há contagem, vira `Atalhos · 14` com o número em tertiary a 70% de opacidade.

### 5.1 Grid de atalhos — 848 × 364
- **7 colunas × 2 linhas**, `gap 10`, alinhado ao topo, scroll vertical se passar de 14.
- Cada tile é o `ShortcutTile` que já está especificado (`reference/components/core/ShortcutTile.jsx.txt`): capsule **3:4**, raio 6, borda subtle, `--elev-1` em repouso. Na largura útil de 816 dá **108 × 144** por tile.
- Hover: `translateY(-3)`, `--elev-3`, contorno accent 2 DIPs, brilho 1.08 na arte, e revela o botão *Abrir* sobre o gradiente de proteção. Press: `scale(.985)`.
- Sem arte real, o tile usa o gradiente do tipo + glifo watermark. **Isso é placeholder** — quando houver ícone extraído do executável, ele entra como `cover`.
- Botão direito no tile → o mesmo `ContextMenu` (Abrir / Editar / Remover) que a janela já tem. Editar/Remover **abrem a janela gerenciadora**, não um diálogo dentro do overlay.

### 5.2 Busca — 460 × auto
- Linha de **44** de altura: ícone `search` 18 tertiary · `TextBox` fs-subtitle 15 primary · chip `Enter` (mono caption, `--surface-2`, borda subtle, raio 4, padding 3/7).
- Placeholder: **"Buscar atalho, pasta ou comando…"**
- Com texto digitado, cresce: separador de 1 DIP subtle, `padding-top 6`, até **4 resultados**. Cada linha: padding `7 8`, raio 4, ícone do tipo 16 accent, nome fs-body primary, hotkey em mono caption tertiary à direita. **A primeira linha tem fill `--accent-soft`** e é a que o `Enter` dispara. ↑/↓ movem.
- Filtra por `Contains`, case-insensitive, sobre o nome. O grid filtra junto — é a mesma consulta.

### 5.3 Mic + áudio — 264 × auto
- Dois botões de 34 de altura, padding `0 11`, gap 7, raio 4, `--surface-2`, borda subtle: **Mic** (`mic`/`mic-off`) e **Áudio**. Ícone 16, label fs-body medium.
- Mutado/surdo: fill `--danger-soft`, borda e texto `--danger`, glifo troca pro `-off`.
- À direita, medidor de 5 barras (largura 3, raio 2, altura útil 22, `--success`). Alturas em repouso: 35 / 70 / 100 / 55 / 20 %. Mutado → todas a 12% e `--text-tertiary`.
- **É controle, não leitura.** Clicar muta de verdade.

### 5.4 Notificações — 264 × 200
- Cabeçalho `Notificações · N` + **Limpar** à direita (accent, caption, medium).
- Cada item: `--surface-2`, borda subtle, raio 4, padding `7 9`, gap 8. Ícone 15 accent. Título caption semibold primary com ellipsis; hora em mono 9.5 tertiary à direita; corpo caption secondary com ellipsis.
- Rola dentro da ilha. Vazio: "Sem notificações." em fs-body tertiary.
- **Sem allow-list** — toda notificação do Windows chega aqui, como já estava decidido.

### 5.5 Favoritos — 264 × auto
- Até **5** botões de **42 × 42**, gap 6, `--surface-2` (hover `--surface-3`), borda subtle, raio 4, glifo do tipo 19 accent.
- Tooltip com o nome, abaixo. São os `ShortcutItem` com `Pinned = true`.

### 5.6 Discord Rich Presence — 264 × auto
- Cabeçalho `Discord` + toggle **34 × 20** (pill, knob branco 16; `--accent` ligado, `--surface-3` desligado).
- Corpo: quadrado **44 × 44** raio 4 com `linear-gradient(150°, #1F4A73, #132B45)`, borda subtle, glifo `gamepad` 22 (`--accent` ligado, `--text-tertiary` desligado).
- Ao lado: nome do app em **display 15 semibold**; abaixo, ponto de 6 (`--success` / tertiary) + tempo em **mono caption secondary**.
- Desligado, o nome vira "Rastreio desativado" e o tempo `--:--`. O nome do app vem do `GamePresenceWatcher`.

### 5.7 Desempenho — 264 × auto
- Três cards em linha, gap 6, `--surface-2`, borda subtle, raio 4, padding `7 9`.
- Chave em **mono 9.5 tertiary**, tracking `.06em`. Valor em **display 17 semibold**.
- `%` ganha barra de 2 DIPs, raio 1, `margin-top 4`, track `--surface-3`, fill `--accent`.
- **Acima de 85% o valor e a barra viram `--danger`.**
- **FPS está fora de escopo** (§ README). Se não houver provider, mostre só CPU e GPU — dois cards, não três com zero.

### 5.8 Config — 44 × 44
- Botão único, mesmo chrome de ilha (acrylic, borda subtle, raio 8, `--elev-4`), glifo `settings` 20 `--text-secondary`.
- Clique **abre a `MainWindow` gerenciadora** e fecha o overlay. É a ponte entre as duas superfícies.

---

## 6. Foco e teclado

`WS_EX_NOACTIVATE` em tudo mantém o jogo com foco — mas então **nenhuma ilha recebe tecla**. O contrato:

1. O overlay abre **sem roubar foco**. O jogo continua recebendo input.
2. Quando o usuário **clica na ilha de busca** (ou digita depois de abrir com o hotkey), a `IslandWindow` da busca chama `SetForegroundWindow` **nela mesma** e remove `WS_EX_NOACTIVATE`.
3. Ao fechar o overlay, ou ao a busca perder o foco, devolva o foco à janela do jogo — guarde o `GetForegroundWindow()` de **antes** de abrir e restaure com `SetForegroundWindow`.
4. `Esc` fecha o overlay de qualquer ilha. O hotkey global (`RegisterHotKey`, já existe) funciona dos dois lados.

Foco de teclado é obrigatório e visível: `--focus-native` (traço de dois tons, ver `reference/tokens/platform.css`) em todo tile, botão e campo. O launcher abre por hotkey — a maioria das sessões é só teclado.

Tab order: busca → grid (ordem de leitura) → favoritos → mic → Discord → desempenho → notificações → config.

---

## 7. Motion

| O que | Duração | Easing |
|---|---|---|
| Vinheta aparecendo/saindo | 220ms (`--dur-med`) | `--ease-out` `cubic-bezier(.22,.61,.36,1)` |
| Ilhas entrando | 220ms, **`--ease-spring`** `cubic-bezier(.34,1.56,.64,1)`, com `translateY(8)` → `0` + fade | spring |
| Ilha reposicionada (encaixe) | 220ms spring em left/top | spring |
| Ilha sendo arrastada | **sem transição**, `scale(1.02)` enquanto segura | — |
| Hover de tile / botão | 150ms (`--dur-fast`) | spring no transform, out na cor |
| Pílula 12→14 de padding no hover | 220ms | spring |

**Entrada escalonada:** as ilhas entram com `stagger` de ~20ms na ordem grid → busca → laterais. Nada de todas de uma vez.

`prefers-reduced-motion` equivalente no Windows: se `SystemParameters.ClientAreaAnimation == false`, **todas as durações vão a 1ms** e as easings viram linear. O overlay passa a ser uma troca de estado instantânea. Não é opcional.

---

## 8. Persistência

`overlay-layout.json`, ao lado do `shortcuts.json` que já existe:

```json
{
  "version": 1,
  "monitor": "\\\\.\\DISPLAY1",
  "islands": {
    "grid":     { "visible": true,  "anchor": "bottom-center", "dx": 0,  "dy": 18 },
    "search":   { "visible": true,  "anchor": "top-center",    "dx": 0,  "dy": 140 },
    "mic":      { "visible": true,  "anchor": "top-left",      "dx": 24, "dy": 206 },
    "notif":    { "visible": true,  "anchor": "top-left",      "dx": 24, "dy": 286 },
    "quick":    { "visible": true,  "anchor": "top-left",      "dx": 24, "dy": 502 },
    "discord":  { "visible": true,  "anchor": "top-right",     "dx": 24, "dy": 206 },
    "perf":     { "visible": true,  "anchor": "top-right",     "dx": 24, "dy": 336 },
    "settings": { "visible": true,  "anchor": "top-right",     "dx": 24, "dy": 24 }
  }
}
```

Guardar **âncora + offset em DIPs**, não coordenadas absolutas: o arranjo sobrevive a troca de resolução e a monitores diferentes. `visible` prepara o "ligar/desligar ilha" sem mudar o formato depois. Arquivo ausente ou `version` desconhecida → cai no arranjo A acima.

---

## 9. Modo de edição de layout (fase 3)

Copiado da gramática do overlay do Discord, e **só existe neste modo** — fora dele não há borda tracejada nenhuma.

- Volta de cada ilha: borda **tracejada 1.5 DIP** `--border-strong`, deslocada **8 DIPs pra fora**, raio 14. Enquanto arrasta, a tracejada vira `--accent`.
- Chip de rótulo, **8 à esquerda e 34 acima** do canto da ilha: `--surface-3`, borda subtle, raio 4, padding `3 4 3 8`. Dentro: nome em caption semibold primary · engrenagem 13 em slot 20×20 tertiary · pin 12 em slot 20×20 com fill `--accent` e glifo `--text-on-accent`.
- Cursor `grab` → `grabbing`. `scale(1.02)` enquanto segura.
- Barra no rodapé da tela: "Arraste as ilhas — elas encaixam nas bordas." + **Restaurar arranjo** + **Salvar**. Pill, `--surface-3`, borda strong, `--elev-4`.
- Enquanto o modo está ligado, o scrim pode ir pro escurecimento **uniforme** (`rgba(6,8,11,.62)`) — o usuário está mexendo em layout, não jogando.

---

## 10. Wiring em C#

```
Overlay/
  OverlayController.cs      abre/fecha, ordem de z, foco, stagger de entrada
  OverlayScrimWindow.xaml   tela cheia, vinheta, WS_EX_TRANSPARENT
  IslandWindow.xaml         casca genérica: acrylic, borda, raio, sombra, drag
  OverlayLayoutStore.cs     overlay-layout.json (ler/salvar/migrar)
  Islands/
    ShortcutGridIsland.xaml   consome ShortcutStore
    SearchIsland.xaml         filtra + foco (§6)
    MicIsland.xaml
    NotificationsIsland.xaml
    QuickShortcutsIsland.xaml Pinned == true
    DiscordPresenceIsland.xaml GamePresenceWatcher
    PerfIsland.xaml           CPU/GPU via PDH; FPS fora de escopo
    SettingsIsland.xaml       abre a MainWindow
```

Pontos de contato com o que já existe:

| Onde | O que muda |
|---|---|
| `App.xaml.cs` → `OnStartup` | criar o `OverlayController` (e as 10 janelas escondidas) junto da `MainWindow`, antes do `Hide()` |
| `HotkeyManager.cs` | o handler do `Ctrl+Alt+L` passa de `ShowLauncher()` para `OverlayController.Toggle()` |
| `App.xaml.cs` → `ShowLauncher()` | continua igual, agora só chamado pela bandeja e pela ilha de engrenagem |
| `ShortcutItem.cs` | `+ bool Pinned` (favoritos) |
| `GamePresenceWatcher.cs` | expor o app ativo (evento ou propriedade) pra ilha do Discord |
| `App.xaml.cs` → `ShowNotification()` | passar a alimentar a ilha de notificações também |
| Settings | `+ "Abrir como overlay"` (flag de dev) · `+ monitor do overlay` · aviso de que **não cobre fullscreen exclusivo** |

**Ordem de abertura:** salvar `GetForegroundWindow()` → mostrar o scrim com fade → mostrar as ilhas com stagger → reafirmar topmost uma vez. **Fechar:** esconder ilhas → fade out do scrim → `SetForegroundWindow` na janela guardada.

---

## 11. Aberto

- **Provider de CPU/GPU.** `PerformanceCounter` para CPU, counters PDH "GPU Engine" para GPU, ou LibreHardwareMonitor pra ambos. Escolha do dev. **FPS não** (§ README).
- **Arte dos tiles.** Hoje é gradiente por tipo + watermark. Extrair o ícone do executável (`ExtractAssociatedIcon`) melhoraria muito a varredura do grid — não especificado, vale um spike.
- **Ligar/desligar ilha individual.** O `visible` já está no JSON, mas a UI pra isso (provavelmente dentro da engrenagem do chip de rótulo, no modo de edição) não foi desenhada.
