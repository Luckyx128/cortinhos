using Cortinho.Models;
using Cortinho.Native;
using Cortinho.Overlay.Islands;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Cortinho.Overlay
{
    /// <summary>Abre/fecha o overlay: scrim + ilhas, foco (PHASE-OVERLAY.md §6), stagger de entrada.
    /// Fase 2 completa: grid + as 6 ilhas "simples" (Config/Favoritos/Mic/Discord/Desempenho/Notificações)
    /// + Busca — a única que tira foco do jogo, via IslandWindow.Activate()/Deactivate().
    /// Fase 3: modo de edição de layout (tecla L), arrastar/encaixar e overlay-layout.json.</summary>
    public class OverlayController
    {
        private readonly MainWindow _mainWindow;

        private readonly OverlayScrimWindow _scrim = new();
        private readonly OverlayEditBarWindow _editBar = new();
        private readonly Services.GlobalInputWatcher _inputWatcher = new();

        // Lazy, nunca resolvido no construtor: a NotchWindow só existe depois que App.OnStartup retorna
        // e o WPF navega o StartupUri — mesmo padrão/mesma ressalva do DiscordIsland.Notch.
        private NotchWindow? _notch;
        private NotchWindow? Notch => _notch ??= System.Windows.Application.Current.Windows.OfType<NotchWindow>().FirstOrDefault();

        private readonly SearchIsland _searchContent = new();
        private readonly IslandWindow _searchIslandWindow;

        private readonly ShortcutGridIsland _gridContent = new();
        private readonly IslandWindow _gridIslandWindow;

        private readonly MicIsland _micContent = new();
        private readonly IslandWindow _micIslandWindow;

        private readonly NotificationsIsland _notifContent = new();
        private readonly IslandWindow _notifIslandWindow;

        private readonly FavoritesIsland _favContent = new();
        private readonly IslandWindow _favIslandWindow;

        private readonly DiscordIsland _discordContent = new();
        private readonly IslandWindow _discordIslandWindow;

        private readonly PerfIsland _perfContent = new();
        private readonly IslandWindow _perfIslandWindow;

        private readonly ConfigIsland _configContent = new();
        private readonly IslandWindow _configIslandWindow;

        private readonly List<IslandWindow> _allIslands;

        private bool _isOpen;
        private bool _isEditMode;
        private IntPtr _previousForegroundWindow;

        /// <summary>Layout salvo em disco (null = arranjo A embutido). Recarregado a cada Open pra
        /// refletir um "Restaurar" feito numa sessão anterior.</summary>
        private OverlayLayout? _layout;

        /// <summary>Posições acumuladas durante o modo de edição, ainda não persistidas — só viram
        /// disco no "Salvar" (§9). Sair sem salvar descarta.</summary>
        private readonly Dictionary<string, OverlayIslandLayout> _pendingLayout = new();

        public OverlayController(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;

            _searchIslandWindow = new IslandWindow(_searchContent, "search", "Busca", 460, double.NaN);
            _gridIslandWindow = new IslandWindow(_gridContent, "grid", "Atalhos", 848, 364);
            _micIslandWindow = new IslandWindow(_micContent, "mic", "Mic", 264, double.NaN);
            _notifIslandWindow = new IslandWindow(_notifContent, "notif", "Notificações", 264, 200);
            _favIslandWindow = new IslandWindow(_favContent, "quick", "Favoritos", 264, double.NaN);
            _discordIslandWindow = new IslandWindow(_discordContent, "discord", "Discord", 264, double.NaN);
            _perfIslandWindow = new IslandWindow(_perfContent, "perf", "Desempenho", 264, double.NaN);
            _configIslandWindow = new IslandWindow(_configContent, "settings", "Config", 44, 44, padding: 6);

            _allIslands = new List<IslandWindow>
            {
                _searchIslandWindow, _gridIslandWindow, _micIslandWindow, _notifIslandWindow,
                _favIslandWindow, _discordIslandWindow, _perfIslandWindow, _configIslandWindow,
            };

            foreach (var island in _allIslands)
                island.Dragged += OnIslandDragged;

            _configContent.ConfigClicked += () =>
            {
                _mainWindow.ShowLauncher();
                Close();
            };

            // "O grid filtra junto — é a mesma consulta" (PHASE-OVERLAY.md §5.2).
            _searchContent.QueryChanged += query => _gridContent.Filter(query);

            // Fecha o overlay ao lançar um atalho, de qualquer uma das 3 ilhas que lançam — sem isso
            // o overlay ficava por cima competindo pelo foco com o app recém-aberto (achado em teste
            // ao vivo do usuário, não estava no PHASE-OVERLAY.md).
            _searchContent.ShortcutLaunched += Close;
            _gridContent.ShortcutLaunched += Close;
            _favContent.ShortcutLaunched += Close;

            // Contrato do §6: ao a busca perder o foco (clique no jogo, alt-tab...), devolve o estado
            // NOACTIVATE. Não chama SetForegroundWindow aqui — se for o Close() causando isso, quem
            // devolve o foco à janela salva é o próprio Close(), logo abaixo.
            _searchIslandWindow.Deactivated += (_, _) => _searchIslandWindow.Deactivate();

            _editBar.SaveRequested += () => ExitEditMode(save: true);
            _editBar.RestoreRequested += RestoreArrangement;

            _inputWatcher.EscapePressed += () =>
            {
                if (!_isOpen) return;
                if (_isEditMode) ExitEditMode(save: false); // Esc sai do modo de edição antes de fechar o overlay
                else Close();
            };

            _inputWatcher.KeyPressed += OnGlobalKeyPressed;
        }

        public void Toggle()
        {
            if (_isOpen) Close();
            else Open();
        }

        private void Open()
        {
            if (_isOpen) return;
            _isOpen = true;

            _previousForegroundWindow = NativeMethods.GetForegroundWindow();
            Notch?.SetOverlayOpen(true);

            _layout = OverlayLayoutStore.Load();
            _pendingLayout.Clear();

            // Reset() só dispara QueryChanged("") (que filtra o grid de volta) se havia texto de uma
            // sessão anterior — no primeiro Open() a TextBox já está vazia e nada dispara, por isso o
            // Refresh() abaixo continua necessário como carga inicial do grid.
            _searchContent.Reset();
            _gridContent.Refresh();
            _favContent.Refresh();
            _micContent.Refresh();
            _notifContent.Refresh();

            bool discordAvailable = _discordContent.IsAvailable;
            if (discordAvailable) _discordContent.Refresh();

            bool perfAvailable = _perfContent.IsAvailable;
            if (perfAvailable) _perfContent.Start();

            bool animate = SystemParameters.ClientAreaAnimation;

            _scrim.Show();
            _scrim.FadeIn(animate);

            // Mostrar ANTES de posicionar: as ilhas de altura automática (SizeToContent) só têm
            // ActualHeight válido depois de entrarem na árvore visual, e o empilhamento da coluna
            // esquerda depende dessa altura. Opacity=0 no Prepare evita o flash de um frame na
            // posição antiga — quem devolve a opacidade é o AnimateIn logo abaixo.
            var visible = new List<IslandWindow> { _searchIslandWindow, _gridIslandWindow, _micIslandWindow, _notifIslandWindow, _favIslandWindow };
            if (discordAvailable) visible.Add(_discordIslandWindow);
            if (perfAvailable) visible.Add(_perfIslandWindow);
            visible.Add(_configIslandWindow);

            foreach (var window in visible)
            {
                window.Opacity = 0;
                window.Show();
                window.UpdateLayout();
            }

            PositionIslands();

            // Stagger de ~20ms, ordem busca → grid → laterais (PHASE-OVERLAY.md §7, mesma ordem do
            // tab order do §6).
            for (int i = 0; i < visible.Count; i++)
                visible[i].AnimateIn(TimeSpan.FromMilliseconds(20 * i), animate);
        }

        private void Close()
        {
            if (!_isOpen) return;
            _isOpen = false;

            bool animate = SystemParameters.ClientAreaAnimation;

            if (_isEditMode) ExitEditMode(save: false);

            _perfContent.Stop();
            _discordContent.Stop();
            Notch?.SetOverlayOpen(false);

            // Defensivo: garante NOACTIVATE de volta mesmo se a busca ainda estava ativa no instante
            // do fechamento (Esc/hotkey), antes do Deactivated ter chance de disparar sozinho.
            _searchIslandWindow.Deactivate();

            foreach (var island in _allIslands)
            {
                if (island.IsVisible) HideIsland(island, animate);
            }

            _scrim.FadeOut(animate, () =>
            {
                _scrim.Hide();
                if (_previousForegroundWindow != IntPtr.Zero)
                    NativeMethods.SetForegroundWindow(_previousForegroundWindow);
            });
        }

        private static void HideIsland(IslandWindow window, bool animate) =>
            window.AnimateOut(animate, () => window.Hide());

        // ---- Modo de edição de layout (PHASE-OVERLAY.md §9)

        private const uint VK_L = 0x4C;

        /// <summary>Entrada no modo de edição não está especificada no handoff — o protótipo usa a tecla
        /// L ("Teclas: L editar layout", README) e é isso que está implementado aqui. Ignorada enquanto
        /// a busca tem foco, senão digitar "l" numa consulta entraria em modo de edição.</summary>
        private void OnGlobalKeyPressed(uint vkCode)
        {
            if (!_isOpen || vkCode != VK_L) return;
            if (_searchIslandWindow.IsActive) return;

            if (_isEditMode) ExitEditMode(save: false);
            else EnterEditMode();
        }

        private void EnterEditMode()
        {
            if (_isEditMode) return;
            _isEditMode = true;

            bool animate = SystemParameters.ClientAreaAnimation;

            _scrim.SetEditMode(true, animate);
            foreach (var island in _allIslands)
                island.SetEditMode(true);

            _editBar.Show();
            _editBar.CenterAtBottom();
        }

        private void ExitEditMode(bool save)
        {
            if (!_isEditMode) return;
            _isEditMode = false;

            bool animate = SystemParameters.ClientAreaAnimation;

            if (save) SaveLayout();
            else _pendingLayout.Clear();

            _scrim.SetEditMode(false, animate);
            foreach (var island in _allIslands)
                island.SetEditMode(false);

            _editBar.Hide();

            if (!save) PositionIslands(); // descarta os arrastes desta sessão de edição
        }

        private void OnIslandDragged(IslandWindow island)
        {
            _pendingLayout[island.IslandId] = OverlayAnchors.Derive(
                island.IslandLeft, island.IslandTop, island.IslandWidth, island.IslandHeight,
                SystemParameters.PrimaryScreenWidth, SystemParameters.PrimaryScreenHeight);
        }

        private void SaveLayout()
        {
            var layout = _layout ?? new OverlayLayout();

            // Só as ilhas efetivamente arrastadas entram/são atualizadas — as intocadas continuam
            // seguindo o arranjo A embutido, o que mantém o arquivo pequeno e legível à mão.
            foreach (var (id, entry) in _pendingLayout)
                layout.Islands[id] = entry;

            layout.Version = OverlayLayout.CurrentVersion;
            layout.Monitor = System.Windows.Forms.Screen.PrimaryScreen?.DeviceName ?? "";

            OverlayLayoutStore.Save(layout);
            _layout = layout;
            _pendingLayout.Clear();
        }

        private void RestoreArrangement()
        {
            OverlayLayoutStore.Reset();
            _layout = null;
            _pendingLayout.Clear();
            PositionIslands();
        }

        private const double ColumnGap = 12;

        /// <summary>Arranjo A "Constelação" (PHASE-OVERLAY.md §3). Só o monitor primário por enquanto —
        /// mesma divergência aceita já registrada na fase 1 (settings de monitor do overlay é fase futura).
        ///
        /// Divergência dos offsets Y do §3: a tabela do spec (mic 206 / notif 286 / favoritos 502) assume
        /// uma ilha de mic de ~80 DIPs, mas com o cabeçalho comum do §5 e o padding de 16 ela sai com
        /// ~119 — com offsets fixos as ilhas da coluna esquerda se sobrepunham visivelmente. Aqui só o Y
        /// da PRIMEIRA ilha de cada coluna vem do spec; as de baixo empilham pela altura real medida.
        ///
        /// O layout salvo (fase 3) entra POR CIMA disso, ilha a ilha: quem nunca foi arrastada continua
        /// no arranjo embutido, então um overlay-layout.json parcial nunca deixa uma ilha sem posição.</summary>
        private void PositionIslands()
        {
            double screenWidth = SystemParameters.PrimaryScreenWidth;
            double screenHeight = SystemParameters.PrimaryScreenHeight;

            _searchIslandWindow.SetIslandPosition((screenWidth - _searchIslandWindow.IslandWidth) / 2, 140);

            _gridIslandWindow.SetIslandPosition(
                (screenWidth - _gridIslandWindow.IslandWidth) / 2,
                screenHeight - _gridIslandWindow.IslandHeight - 18);

            _configIslandWindow.SetIslandPosition(screenWidth - _configIslandWindow.IslandWidth - 24, 24);

            StackColumn(24, 206, _micIslandWindow, _notifIslandWindow, _favIslandWindow);
            StackColumn(screenWidth - 264 - 24, 206, _discordIslandWindow, _perfIslandWindow);

            ApplySavedLayout(screenWidth, screenHeight);
        }

        private void ApplySavedLayout(double screenWidth, double screenHeight)
        {
            if (_layout is null) return;

            foreach (var island in _allIslands)
            {
                if (!_layout.Islands.TryGetValue(island.IslandId, out var entry)) continue;

                var (left, top) = OverlayAnchors.Resolve(
                    entry, island.IslandWidth, island.IslandHeight, screenWidth, screenHeight);

                island.SetIslandPosition(left, top);
            }
        }

        /// <summary>Empilha as ilhas visíveis de uma coluna a partir de (left, firstTop), com ColumnGap
        /// entre elas. Ignora as escondidas (Discord/Desempenho somem quando não há provider).</summary>
        private static void StackColumn(double left, double firstTop, params IslandWindow[] windows)
        {
            double top = firstTop;
            foreach (var window in windows)
            {
                if (!window.IsVisible) continue;

                window.SetIslandPosition(left, top);
                top += window.IslandHeight + ColumnGap;
            }
        }

        /// <summary>Chamado só de MainWindow.OnClosed (saída real do app pela bandeja) — libera o
        /// PerformanceCounter próprio do PerfIsland, mesma disciplina do NotchWindow.OnClosed.</summary>
        public void Dispose()
        {
            _perfContent.Dispose();
            _inputWatcher.Dispose();
        }
    }
}
