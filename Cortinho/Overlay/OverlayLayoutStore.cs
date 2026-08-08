using Cortinho.Models;
using System.IO;
using System.Text.Json;

namespace Cortinho.Overlay
{
    /// <summary>overlay-layout.json (PHASE-OVERLAY.md §8), ao lado do shortcuts.json.</summary>
    public static class OverlayLayoutStore
    {
        private static readonly string ConfigPath = Path.Combine(AppContext.BaseDirectory, "overlay-layout.json");

        private static readonly JsonSerializerOptions Options = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true,
        };

        /// <returns>null quando não há layout salvo ou a versão é desconhecida — o chamador cai no
        /// arranjo A embutido (§8: "Arquivo ausente ou version desconhecida → cai no arranjo A").</returns>
        public static OverlayLayout? Load()
        {
            try
            {
                if (!File.Exists(ConfigPath)) return null;

                var layout = JsonSerializer.Deserialize<OverlayLayout>(File.ReadAllText(ConfigPath), Options);
                if (layout is null || layout.Version != OverlayLayout.CurrentVersion) return null;

                return layout;
            }
            catch
            {
                return null;
            }
        }

        public static void Save(OverlayLayout layout)
        {
            try { File.WriteAllText(ConfigPath, JsonSerializer.Serialize(layout, Options)); }
            catch { /* disco cheio/somente-leitura — o arranjo em memória segue válido nesta sessão */ }
        }

        /// <summary>"Restaurar arranjo" (§9) — apaga o arquivo pra voltar ao arranjo A embutido.</summary>
        public static void Reset()
        {
            try { if (File.Exists(ConfigPath)) File.Delete(ConfigPath); }
            catch { /* idem */ }
        }
    }

    /// <summary>Conversão âncora+offset ⇄ posição de tela (PHASE-OVERLAY.md §3/§8). Guardar âncora em
    /// vez de coordenada absoluta é o que faz o arranjo sobreviver a troca de resolução.</summary>
    public static class OverlayAnchors
    {
        public const double EdgeMargin = 24;
        public const double SnapDistance = 14;

        /// <summary>Âncora + offset → posição de tela da ilha.</summary>
        public static (double Left, double Top) Resolve(
            OverlayIslandLayout layout, double islandWidth, double islandHeight, double screenWidth, double screenHeight)
        {
            var (horizontal, vertical) = Split(layout.Anchor);

            double left = horizontal switch
            {
                "right" => screenWidth - islandWidth - layout.Dx,
                "center" => (screenWidth - islandWidth) / 2 + layout.Dx,
                _ => layout.Dx,
            };

            double top = vertical == "bottom"
                ? screenHeight - islandHeight - layout.Dy
                : layout.Dy;

            return (left, top);
        }

        /// <summary>Posição de tela → âncora + offset. A âncora sai da metade/terço da tela em que a
        /// ilha caiu, pra que o offset guardado fique sempre pequeno e relativo à borda mais próxima.</summary>
        public static OverlayIslandLayout Derive(
            double left, double top, double islandWidth, double islandHeight, double screenWidth, double screenHeight)
        {
            double centerX = left + islandWidth / 2;
            double centeredLeft = (screenWidth - islandWidth) / 2;

            string horizontal;
            double dx;

            // "center" só quando está de fato perto do centro — senão o offset guardado viraria um
            // número grande e o arranjo deixaria de sobreviver a uma troca de resolução.
            if (Math.Abs(left - centeredLeft) <= screenWidth * 0.08)
            {
                horizontal = "center";
                dx = left - centeredLeft;
            }
            else if (centerX > screenWidth / 2)
            {
                horizontal = "right";
                dx = screenWidth - islandWidth - left;
            }
            else
            {
                horizontal = "left";
                dx = left;
            }

            bool bottom = top + islandHeight / 2 > screenHeight / 2;

            return new OverlayIslandLayout
            {
                Anchor = (bottom ? "bottom" : "top") + "-" + horizontal,
                Dx = Math.Round(dx),
                Dy = Math.Round(bottom ? screenHeight - islandHeight - top : top),
            };
        }

        /// <summary>Encaixe do §3: bordas, centro horizontal e topo rente, a ≤14 DIPs. Sempre clampeia
        /// pra dentro da tela.</summary>
        public static (double Left, double Top) Snap(
            double left, double top, double islandWidth, double islandHeight, double screenWidth, double screenHeight)
        {
            double[] leftCandidates =
            {
                EdgeMargin,
                screenWidth - islandWidth - EdgeMargin,
                (screenWidth - islandWidth) / 2,
            };

            double[] topCandidates =
            {
                0, // topo rente — o spec reserva pra pílula, mas deixar disponível não atrapalha
                EdgeMargin,
                screenHeight - islandHeight - EdgeMargin,
            };

            foreach (double candidate in leftCandidates)
            {
                if (Math.Abs(left - candidate) <= SnapDistance) { left = candidate; break; }
            }

            foreach (double candidate in topCandidates)
            {
                if (Math.Abs(top - candidate) <= SnapDistance) { top = candidate; break; }
            }

            left = Math.Clamp(left, 0, Math.Max(0, screenWidth - islandWidth));
            top = Math.Clamp(top, 0, Math.Max(0, screenHeight - islandHeight));

            return (left, top);
        }

        private static (string Horizontal, string Vertical) Split(string anchor)
        {
            var parts = (anchor ?? "").Split('-');
            return parts.Length == 2 ? (parts[1], parts[0]) : ("left", "top");
        }
    }
}
