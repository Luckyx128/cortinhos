using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Cortinho.Services
{
    internal class MicService
    {
        private readonly MMDevice? _device;

        public MicService()
        {
            try
            {
                var enumerator = new MMDeviceEnumerator();
                _device = enumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Multimedia);
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
