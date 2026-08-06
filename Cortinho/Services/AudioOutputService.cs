using NAudio.CoreAudioApi;

namespace Cortinho.Services
{
    /// <summary>Espelha MicService (mesmo NAudio, mesmo Role.Multimedia) mas pro endpoint de saída
    /// (DataFlow.Render) — controle real de mute do "Áudio" na ilha de Mic+áudio (PHASE-OVERLAY.md §5.3).</summary>
    internal class AudioOutputService
    {
        private readonly MMDevice? _device;

        public AudioOutputService()
        {
            try
            {
                var enumerator = new MMDeviceEnumerator();
                _device = enumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            }
            catch
            {
                _device = null;
            }
        }

        public bool IsMuted => _device?.AudioEndpointVolume.Mute ?? false;

        public void ToggleMute()
        {
            if (_device is null) return;
            _device.AudioEndpointVolume.Mute = !_device.AudioEndpointVolume.Mute;
        }
    }
}
