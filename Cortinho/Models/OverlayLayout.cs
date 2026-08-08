using System.Collections.Generic;

namespace Cortinho.Models
{
    /// <summary>Layout persistido do overlay (PHASE-OVERLAY.md §8). Guarda âncora + offset em DIPs,
    /// nunca coordenada absoluta — assim o arranjo sobrevive a troca de resolução e de monitor.</summary>
    public class OverlayLayout
    {
        public const int CurrentVersion = 1;

        public int Version { get; set; } = CurrentVersion;
        public string Monitor { get; set; } = "";
        public Dictionary<string, OverlayIslandLayout> Islands { get; set; } = new();
    }

    public class OverlayIslandLayout
    {
        public bool Visible { get; set; } = true;

        /// <summary>"top-left" | "top-center" | "top-right" | "bottom-left" | "bottom-center" | "bottom-right".
        /// A âncora define de qual borda dx/dy são medidos; em "*-center", dx é o desvio do centro (0 = centralizado).</summary>
        public string Anchor { get; set; } = "top-left";

        public double Dx { get; set; }
        public double Dy { get; set; }
    }
}
