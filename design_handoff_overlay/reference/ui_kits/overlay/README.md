# Cortinhos Overlay — exploração

Estuda a hipótese: **e se a janela centralizada sumisse e o app virasse um overlay de tela cheia**, no espírito do overlay do Discord — a tela escurece e todas as funcionalidades viram ilhas flutuantes que o usuário arrasta e salva.

## Direção escolhida
**Arranjo A (Constelação) + escurecimento Vinheta + notch Separado.** É o estado inicial do protótipo; os outros eixos continuam alternáveis como registro da exploração.

Junto com ela, a recomendação da nota técnica (item 9): **o overlay é a cara do uso diário; a janela fica só pra gerenciar** atalhos, hotkeys e configs.

## Arquivos
- `index.html` — host (carrega `_ds_bundle.js` + os três .jsx).
- `Islands.jsx` — conteúdo de cada ilha (busca, grid, mic, Discord, perf, notificações, favoritos).
- `OverlayApp.jsx` — palco, scrim, arraste/encaixe, presets, barra de exploração.
- `Feasibility.jsx` — a nota técnica de viabilidade em WPF (tecla `V`).

## O que dá pra alternar
| Eixo | Opções |
|---|---|
| Arranjo | **A Constelação** (bordas livres, grid como cinturão) · **B Painel direito** (grid alto à direita) · **C Lista** (coluna estreita, tela quase toda livre) |
| Escurecimento | Uniforme · Vinheta · Halo local · Nenhum |
| Notch | **Ilha-mãe** (expandido, absorve mic/perf/Discord/notificações) · Separado · Oculto |

Teclas: `L` editar layout · `O` abrir/fechar overlay · `V` viabilidade · `H` esconder a barra · `Esc` fecha.

## Notas de desenho
- O palco é um "monitor" fixo de **1440×860** escalado pra caber no preview. Posições das ilhas são px de projeto; num app real seriam **%** do monitor (ver nota técnica, item 6).
- **Edição de layout** copia a gramática do Discord: borda tracejada em volta de cada ilha + chip de rótulo com engrenagem e pin. Fora do modo de edição não existe borda tracejada nenhuma.
- **O notch expandido mede ~414px de altura aqui**, não os 357 do card de spec — porque no contexto do overlay ele recebe 3 notificações, perf, 5 favoritos e o CTA ao mesmo tempo. Se a "ilha-mãe" virar a direção escolhida, o spec do notch precisa de uma variante de altura.
- Materiais: scrim liso + **acrylic só nas ilhas**. Blur global sobre o jogo é inviável (nota técnica, item 3) — o desenho já respeita isso.
- O fundo é um **placeholder** de gameplay (hachura + legenda), não arte.

## Resolvido: a pílula encolhe
Com o overlay **aberto** no modo Separado, o notch deixa de repetir o que as ilhas já dizem e cai pra **identidade + um ponto de status**: a marca mini (`assets/logo-mini.svg`), o ponto verde (âmbar se o mic está mutado) e, no hover, a legenda "Overlay ativo". Mesma gramática do notch — `--taskbar-tint`, acrylic fino, raio só nos cantos de baixo, 48px de altura.

Com o overlay **fechado** (`O`) a pílula volta ao `compact` normal, com mic, Discord e contagem de notificações — ali ela é a única superfície, então precisa falar.
