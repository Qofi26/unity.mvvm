using System;

namespace Erem.MVVM.Examples
{
    public class SettingsViewModelDemo : AbstractViewModel
    {
        public event Action OnViewModelChanged;

        public bool EnableSounds => _currentSettingsData.EnableSounds;
        public bool EnableMusic => _currentSettingsData.EnableMusic;
        public bool EnableNotifications => _currentSettingsData.EnableNotifications;

        private SettingsData _currentSettingsData;
        private SettingsData? _savedSettingsData;

        public void SetSoundsState(bool isEnable)
        {
            _currentSettingsData.EnableSounds = isEnable;
            NotifyViewModelChanged();
        }

        public void SetMusicState(bool isEnable)
        {
            _currentSettingsData.EnableMusic = isEnable;
            NotifyViewModelChanged();
        }

        public void SetNotificationsState(bool isEnable)
        {
            _currentSettingsData.EnableNotifications = isEnable;
            NotifyViewModelChanged();
        }

        public void SaveSettings()
        {
            _savedSettingsData = _currentSettingsData;
        }

        public void ResetSettings()
        {
            _currentSettingsData = _savedSettingsData ?? new SettingsData();
            NotifyViewModelChanged();
        }

        protected override void OnActivate()
        {
            _savedSettingsData ??= new SettingsData();
        }

        private void NotifyViewModelChanged()
        {
            OnViewModelChanged?.Invoke();
        }

        private struct SettingsData
        {
            public bool EnableSounds;
            public bool EnableMusic;
            public bool EnableNotifications;
        }
    }
}