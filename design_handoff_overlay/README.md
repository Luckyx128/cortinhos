# Handoff: Cortinhos — overlay em tela cheia

## O que muda
Hoje o `Ctrl+Alt+L` abre a **janela centralizada** e ela funciona. Este pacote troca a superfície principal: o hotkey passa a abrir um **overlay em tela cheia** — a tela escurece com vinheta e as funcionalidades viram **ilhas flutuantes** que o usuário arrasta e o app salva.

**A janela não é deletada.** Ela continua sendo a casa de *gerenciar* (cadastrar atalhos, regravar hotkey, settings) e passa a ser aberta pela bandeja ou pela ilha de engrenagem. O overlay é a cara do uso diário.

Decisão registrada em `reference/DESIGN-SYSTEM.md` → seção *Window vs. overlay — decided*.

## Direção escolhida (não explorar de novo)
| Eixo | Escolha |
|---|---|
| Arranjo | **A "Constelação"** — ilhas encostadas nas bordas, grid de atalhos como cinturão inferior, centro da tela livre pro jogo |
| Escurecimento | **Vinheta** — escurece as bordas, deixa o centro do jogo respirando |
| Notch | **Separado** — a pílula continua sendo mobília da taskbar; com o overlay aberto ela encolhe pra identidade + status |
| Layout | Usuário arrasta e o app salva |
| Click-through | O jogo continua jogável; só as ilhas capturam clique |

O protótipo permite alternar os outros arranjos/escurecimentos — isso é **registro da exploração**, não escopo. Implemente só a direção escolhida.

## Sobre os arquivos deste pacote
Tudo em `reference/` é **referência de design feita em HTML/React** — mostra aparência e comportamento pretendidos, **não é código de produção pra copiar**. Recrie no ambiente real (WPF / XAML). Nada de WebView2 embutindo o HTML.

Ordem de leitura:
1. Este README.
2. **`PHASE-OVERLAY.md`** — o spec completo: arquitetura de janelas, geometria das ilhas em DIPs, cada ilha por dentro, motion, foco/teclado, persistência e wiring em C#.
3. `TOKENS-WPF.md` — todos os tokens como `ResourceDictionary` (idêntico ao do pacote do notch; se você já fez, pule).
4. `reference/ui_kits/overlay/README.md` — notas de desenho do protótipo.

Pra ver o protótipo: abra `reference/ui_kits/overlay/index.html` num navegador. (As fontes React estão com sufixo `.txt` de propósito — é referência de design, não código pra compilar; o `index.html` já as carrega assim.) Teclas: `L` editar layout · `O` abrir/fechar o overlay · `V` nota de viabilidade · `H` esconder a barra de exploração.

## Fidelidade
**Hi-fi.** Cores, tipografia, espaçamento, raios, sombras e transições são finais e medidos. Todos os px são **DIPs** — o WPF escala pra 125/150/175/200% sozinho.

Uma divergência **intencional** entre protótipo e produção: no HTML as ilhas usam `backdrop-filter` (acrylic por elemento). No WPF isso não existe por elemento — a solução está no `PHASE-OVERLAY.md` §2 (cada ilha é a própria janela, com `DesktopAcrylicBackdrop`). Se a fase 1 sair com ilhas **opacas**, está aceito; o acrylic entra na fase 2.

## Fases

| # | Escopo | Entrega |
|---|---|---|
| **1** | **Casca + scrim + grid** | `OverlayScrimWindow` (tela cheia, click-through, vinheta) + `IslandWindow` genérica + a ilha do **grid de atalhos** funcionando com o `ShortcutStore` que já existe. `Ctrl+Alt+L` abre o overlay em vez da janela. |
| **2** | **Ilhas restantes** | Busca, mic, favoritos, Discord Rich Presence, desempenho, notificações, engrenagem. Acrylic por ilha. |
| **3** | **Arrastar + salvar** | Modo de edição (borda tracejada + chip de rótulo), encaixe nas bordas, `overlay-layout.json`. |
| **4** | **Pílula reduzida** | A `NotchWindow` do outro pacote passa a encolher pra identidade + ponto de status enquanto o overlay está aberto. |
| **5** | **Janela como gerenciador** | A `MainWindow` deixa de abrir no hotkey; passa a abrir pela bandeja e pela ilha de engrenagem. Ela mantém o chrome nativo da fase 3 do pacote do notch. |

Fase 1 é a única que muda comportamento existente (o destino do hotkey). Recomendo deixá-la atrás de uma flag em settings — *"Abrir como overlay"* — durante o desenvolvimento.

## O que já existe no repo (não reescrever)
- **`HotkeyManager.cs`** — `RegisterHotKey` + `HwndHook`. Só troca o alvo: hoje chama `ShowLauncher()`, passa a chamar `OverlayController.Toggle()`.
- **`App.xaml.cs`** — `NotifyIcon` da bandeja, `ShowLauncher()`, `ShowNotification()`. É aqui que o `OverlayController` é criado no `OnStartup`. `ShowLauncher()` continua existindo, agora só pra bandeja/engrenagem.
- **`ShortcutStore.cs` / `ShortcutItem.cs`** — `shortcuts.json`. A ilha do grid consome direto. Precisa de `bool Pinned` (favoritos) — mesmo campo que a fase 1 do pacote do notch já pede.
- **`GamePresenceWatcher.cs`** — poll de 5s. A ilha do Discord precisa que ele **exponha o app ativo** (evento ou propriedade), não só chame o `DiscordPresenceService`.
- **`MainWindow.xaml.cs`** — não mexer nesta fase. Ela vira o gerenciador na fase 5.

## Assets
- `reference/assets/logo-mini.svg` — a marca da **pílula reduzida** (22px) e do ícone da bandeja (16px). Sem monograma.
- `reference/assets/logo.svg` — marca completa, pra `.ico`.
- `reference/assets/fonts/` — Chakra Petch (400/500/600/700), IBM Plex Sans (variável), IBM Plex Mono (400/500/600). OFL, **embutir** (`Resources/Fonts/`, `pack://application:,,,/Resources/Fonts/#Chakra Petch`).
- Ícones: **Solar Bold Duotone** (CC-BY) + o par `mic`/`mic-off` do **Hugeicons**. Paths em `reference/components/core/Icon.jsx.txt` → extraia pra `Geometry` XAML. Duotone = silhueta em `opacity .5` + detalhe em cima, **uma cor só**.

## Arquivos de referência
| Caminho | O que é |
|---|---|
| `reference/ui_kits/overlay/index.html` | **Abra isto.** O protótipo interativo do overlay. |
| `reference/ui_kits/overlay/OverlayApp.jsx.txt` | Palco, scrim, arraste/encaixe, pílula reduzida, presets — **fonte da verdade da geometria**. |
| `reference/ui_kits/overlay/Islands.jsx.txt` | Conteúdo de cada ilha — fonte da verdade das medidas internas. |
| `reference/ui_kits/overlay/Feasibility.jsx.txt` | A nota técnica que embasou as decisões (tecla `V` no protótipo). |
| `reference/components/core/ShortcutTile.jsx.txt` / `.d.ts.txt` | O tile 3:4 do grid — medidas finais. |
| `reference/components/surface/Notch.jsx.txt` / `.d.ts.txt` | O notch, pra fase 4. |
| `reference/tokens/*.css` | Tokens brutos. `platform.css` é o específico de Windows. |
| `reference/DESIGN-SYSTEM.md` | O design system inteiro: voz, marca, ícones, materiais, notas de plataforma. |

## Fora de escopo, por decisão
- **Fullscreen exclusivo.** O overlay aparece em *borderless windowed* e em fullscreen com as otimizações do Win10/11 — que é a maioria dos jogos hoje. Cobrir exclusivo exigiria hook de swapchain, que anti-cheat (EAC, BattlEye) trata como injeção. **Não fazer.** Dizer isso na tela de settings.
- **FPS no módulo de desempenho.** Mesmo problema: exige hook de `Present`. CPU e GPU seguem (PDH / LibreHardwareMonitor). Se não houver provider de FPS, **esconda o campo** em vez de mostrar zero.
- **Blur global sobre o jogo.** Inviável e caro. Vinheta lisa + acrylic só nas ilhas.
- **High contrast do Windows.** Já estava fora de escopo no design system.
