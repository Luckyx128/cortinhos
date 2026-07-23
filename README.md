# Shortcut Launcher

App nativo Windows (WPF / .NET 8) — launcher de atalhos configurável, com ícone na bandeja e atalho global.

## Como rodar

**Pré-requisitos:**
- Visual Studio 2022 (Community serve) com a workload **".NET desktop development"**
- .NET 8 SDK (o instalador do VS já pergunta se quer instalar)

**Passos:**
1. Abra a pasta `ShortcutLauncher` no Visual Studio (`Arquivo > Abrir > Pasta`, ou clique duas vezes no `.csproj`)
2. Aguarde o restore automático de dependências (é tudo biblioteca nativa do .NET, não usa NuGet externo)
3. `F5` para rodar

Na primeira execução, o app cria um `shortcuts.json` na pasta de saída (copiado do `shortcuts.sample.json`) — edite os caminhos de exemplo (Discord, Steam, etc.) ou já adicione os seus pela própria UI.

## O que já funciona (v1)

- Grid de atalhos configuráveis: app, pasta, link ou comando
- Adicionar / editar / remover atalhos pela interface (sem editar JSON na mão)
- Ícone na bandeja do sistema (clique duplo ou menu com botão direito)
- Atalho global **Ctrl+Alt+L** — abre/fecha o launcher de qualquer lugar, mesmo com um jogo em foco
- Fechar a janela (X) só minimiza pra bandeja; "Sair" no menu da bandeja encerra de verdade

## Roadmap sugerido (próximas fases)

1. **Hotkeys customizáveis por atalho** — hoje só existe o hotkey global de abrir o launcher; dá pra estender `GlobalHotkey` pra registrar uma combinação por item (ex: `Ctrl+Alt+1` abre direto o Discord)
2. **Controle rápido de áudio** — trocar dispositivo de saída/microfone padrão via `MMDeviceEnumerator` (biblioteca `NAudio`, ou P/Invoke puro no `IPolicyConfig` não documentado)
3. **Integração com Discord**
   - Rich Presence customizado via [Discord Game SDK](https://discord.com/developers/docs/rich-presence/overview) ou lib `DiscordRPC` no C#
   - Toggle de mute/deafen: não existe API pública oficial pra controlar o cliente Discord por fora; o caminho realista é simular a hotkey que você já configurou dentro do próprio Discord (nas configs de voz), disparada pelo seu app
4. **Perfis de atalhos** — ex: perfil "Jogando", perfil "Trampo", trocando o grid inteiro
5. **Ícones customizados por atalho** (hoje usa o mesmo ícone genérico pra todos)

Cada um desses dá pra ir pra um branch/feature separado — boa forma de ir versionando e documentando no seu portfólio.
