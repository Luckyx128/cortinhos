using Cortinho.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Cortinho.Overlay.Islands
{
    /// <summary>Ilha Mic + áudio (PHASE-OVERLAY.md §5.3) — controle real (não leitura): clicar muta de
    /// verdade, tanto o microfone (MicService, mesmo NAudio do notch) quanto a saída (AudioOutputService,
    /// novo, mesmo padrão espelhado pro Role.Multimedia/DataFlow.Render). O medidor de 5 barras é
    /// decorativo (sem provider de nível de áudio real) — só reflete o estado mutado do mic.</summary>
    public partial class MicIsland : System.Windows.Controls.UserControl
    {
        // Alturas em repouso (PHASE-OVERLAY.md §5.3): 35/70/100/55/20% de 22 DIPs úteis.
        private static readonly double[] RestHeights = { 22 * 0.35, 22 * 0.70, 22 * 1.00, 22 * 0.55, 22 * 0.20 };

        private readonly MicService _micService = new();
        private readonly AudioOutputService _audioService = new();

        public MicIsland()
        {
            InitializeComponent();
            Refresh();
        }

        public void Refresh()
        {
            UpdateMicVisual();
            UpdateAudioVisual();
        }

        private void MicButton_Click(object sender, MouseButtonEventArgs e)
        {
            _micService.ToggleMute();
            UpdateMicVisual();
        }

        private void AudioButton_Click(object sender, MouseButtonEventArgs e)
        {
            _audioService.ToggleMute();
            UpdateAudioVisual();
        }

        private void UpdateMicVisual()
        {
            bool muted = _micService.IsMuted;

            // O rótulo NÃO muda com o estado (§5.3 troca só cor e glifo) — trocar o texto por
            // "Áudio mutado" alargava o botão e empurrava o medidor pra fora da ilha.
            MicOnIcon.Visibility = muted ? Visibility.Collapsed : Visibility.Visible;
            MicOffIcon.Visibility = muted ? Visibility.Visible : Visibility.Collapsed;
            MicButton.Background = muted ? ResourceBrush("DangerSoftBrush") : ResourceBrush("Surface2Brush");
            MicButton.BorderBrush = muted ? ResourceBrush("DangerBrush") : ResourceBrush("BorderSubtleBrush");
            MicLabel.Foreground = muted ? ResourceBrush("DangerBrush") : ResourceBrush("TextPrimaryBrush");

            var bars = new[] { Bar1, Bar2, Bar3, Bar4, Bar5 };
            for (int i = 0; i < bars.Length; i++)
            {
                bars[i].Height = muted ? 22 * 0.12 : RestHeights[i];
                bars[i].Background = muted ? ResourceBrush("TextTertiaryBrush") : ResourceBrush("SuccessBrush");
            }
        }

        private void UpdateAudioVisual()
        {
            bool muted = _audioService.IsMuted;

            AudioOnIcon.Visibility = muted ? Visibility.Collapsed : Visibility.Visible;
            AudioOffIcon.Visibility = muted ? Visibility.Visible : Visibility.Collapsed;
            AudioButton.Background = muted ? ResourceBrush("DangerSoftBrush") : ResourceBrush("Surface2Brush");
            AudioButton.BorderBrush = muted ? ResourceBrush("DangerBrush") : ResourceBrush("BorderSubtleBrush");
            AudioLabel.Foreground = muted ? ResourceBrush("DangerBrush") : ResourceBrush("TextPrimaryBrush");
        }

        private static System.Windows.Media.Brush ResourceBrush(string key) => (System.Windows.Media.Brush)System.Windows.Application.Current.FindResource(key);
    }
}
