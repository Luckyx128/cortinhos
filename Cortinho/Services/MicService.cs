using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Linq;
using Cortinho.Models;

namespace Cortinho.Services
{
    public class MicService : IDisposable
    {
        public static event Action<string>? DeviceChanged;

        public static void NotifyGlobalDeviceChanged(string deviceId)
        {
            DeviceChanged?.Invoke(deviceId);
        }

        private readonly object _lock = new();
        private readonly bool _followGlobal;
        private MMDevice? _device;
        private bool _lastMuted;
        private float _lastVolume;
        private bool _disposed;

        public event Action<bool>? MuteChanged;
        public event Action<bool>? MuteStateChanged;
        public event Action<float>? VolumeChanged;

        public string? DeviceId => _device?.ID;
        public string? DeviceName => _device?.FriendlyName;

        public MicService(string? deviceId = null)
        {
            _followGlobal = deviceId == null;
            string? targetId = deviceId ?? AppSettingsStore.Load().MicDeviceId;
            InitializeDevice(targetId);

            if (_followGlobal)
            {
                DeviceChanged += OnGlobalDeviceChanged;
            }
        }

        private void OnGlobalDeviceChanged(string newDeviceId)
        {
            SelectDevice(newDeviceId);
        }

        public void SelectDevice(string? deviceId)
        {
            lock (_lock)
            {
                CleanupDevice();
                InitializeDevice(deviceId);
            }

            NotifyMuteChanged(IsMuted);
            NotifyVolumeChanged(Volume);
        }

        private void InitializeDevice(string? deviceId)
        {
            try
            {
                using var enumerator = new MMDeviceEnumerator();
                if (!string.IsNullOrWhiteSpace(deviceId))
                {
                    try
                    {
                        _device = enumerator.GetDevice(deviceId);
                    }
                    catch
                    {
                        // Fallback para o microfone padrão do sistema
                        _device = enumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Multimedia);
                    }
                }
                else
                {
                    _device = enumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Multimedia);
                }
            }
            catch
            {
                _device = null;
            }

            if (_device != null)
            {
                try
                {
                    _lastMuted = _device.AudioEndpointVolume.Mute;
                    _lastVolume = _device.AudioEndpointVolume.MasterVolumeLevelScalar;
                    _device.AudioEndpointVolume.OnVolumeNotification += OnAudioEndpointVolumeNotification;
                }
                catch
                {
                    // Falha ao obter controles de volume do endpoint
                }
            }
        }

        public bool IsMuted
        {
            get
            {
                if (_device is null) return false;
                try
                {
                    return _device.AudioEndpointVolume.Mute;
                }
                catch
                {
                    return _lastMuted;
                }
            }
        }

        public float Volume
        {
            get
            {
                if (_device is null) return 0f;
                try
                {
                    return _device.AudioEndpointVolume.MasterVolumeLevelScalar;
                }
                catch
                {
                    return _lastVolume;
                }
            }
            set
            {
                if (_device is null) return;
                try
                {
                    _device.AudioEndpointVolume.MasterVolumeLevelScalar = Math.Clamp(value, 0f, 1f);
                }
                catch { }
            }
        }

        public void ToggleMute()
        {
            if (_device is null) return;
            try
            {
                bool newMute = !_device.AudioEndpointVolume.Mute;
                _device.AudioEndpointVolume.Mute = newMute;

                bool changed = false;
                lock (_lock)
                {
                    if (_lastMuted != newMute)
                    {
                        _lastMuted = newMute;
                        changed = true;
                    }
                }

                if (changed)
                {
                    NotifyMuteChanged(newMute);
                }
            }
            catch
            {
                // Erro de hardware ou COM
            }
        }

        public void SetMute(bool mute)
        {
            if (_device is null) return;
            try
            {
                _device.AudioEndpointVolume.Mute = mute;

                bool changed = false;
                lock (_lock)
                {
                    if (_lastMuted != mute)
                    {
                        _lastMuted = mute;
                        changed = true;
                    }
                }

                if (changed)
                {
                    NotifyMuteChanged(mute);
                }
            }
            catch
            {
                // Erro de hardware ou COM
            }
        }

        private void OnAudioEndpointVolumeNotification(AudioVolumeNotificationData data)
        {
            bool muteChanged = false;
            bool volumeChanged = false;
            bool currentMute = data.Muted;
            float currentVolume = data.MasterVolume;

            lock (_lock)
            {
                if (currentMute != _lastMuted)
                {
                    _lastMuted = currentMute;
                    muteChanged = true;
                }

                if (Math.Abs(currentVolume - _lastVolume) > 0.001f)
                {
                    _lastVolume = currentVolume;
                    volumeChanged = true;
                }
            }

            if (muteChanged)
            {
                NotifyMuteChanged(currentMute);
            }

            if (volumeChanged)
            {
                NotifyVolumeChanged(currentVolume);
            }
        }

        private void NotifyMuteChanged(bool isMuted)
        {
            var dispatcher = System.Windows.Application.Current?.Dispatcher;
            if (dispatcher != null && !dispatcher.CheckAccess())
            {
                dispatcher.BeginInvoke(new Action(() => NotifyMuteChanged(isMuted)));
                return;
            }

            try
            {
                MuteChanged?.Invoke(isMuted);
            }
            catch { }

            try
            {
                MuteStateChanged?.Invoke(isMuted);
            }
            catch { }
        }

        private void NotifyVolumeChanged(float volume)
        {
            var dispatcher = System.Windows.Application.Current?.Dispatcher;
            if (dispatcher != null && !dispatcher.CheckAccess())
            {
                dispatcher.BeginInvoke(new Action(() => NotifyVolumeChanged(volume)));
                return;
            }

            try
            {
                VolumeChanged?.Invoke(volume);
            }
            catch { }
        }

        public static IEnumerable<(string Id, string Name)> GetCaptureDevices()
        {
            var list = new List<(string Id, string Name)>();
            try
            {
                using var enumerator = new MMDeviceEnumerator();
                var endpoints = enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active);
                foreach (var endpoint in endpoints)
                {
                    try
                    {
                        list.Add((endpoint.ID, endpoint.FriendlyName));
                    }
                    catch
                    {
                        // Falha ao obter dados do endpoint específico
                    }
                }
            }
            catch
            {
                // Falha geral ao enumerar dispositivos
            }
            return list;
        }

        public IEnumerable<(string Id, string Name)> GetAvailableDevices() => GetCaptureDevices();

        private void CleanupDevice()
        {
            if (_device != null)
            {
                try
                {
                    _device.AudioEndpointVolume.OnVolumeNotification -= OnAudioEndpointVolumeNotification;
                }
                catch { }

                try
                {
                    _device.Dispose();
                }
                catch { }

                _device = null;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            _disposed = true;

            if (disposing)
            {
                if (_followGlobal)
                {
                    DeviceChanged -= OnGlobalDeviceChanged;
                }
                CleanupDevice();
            }
        }
    }

    public static class MicServiceExtensions
    {
        public static IEnumerable<(string Id, string Name)> GetCaptureDevices(this MicService _) => MicService.GetCaptureDevices();
    }
}
