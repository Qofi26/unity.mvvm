using UnityEngine;
using UnityEngine.UI;

namespace Erem.MVVM.Examples
{
    public class SettingsViewDemo : AbstractView<SettingsViewModelDemo>
    {
        [SerializeField]
        private Toggle _soundsToggle;

        [SerializeField]
        private Toggle _musicToggle;

        [SerializeField]
        private Toggle _notificationToggle;

        [SerializeField]
        private Button _closeButton;

        [SerializeField]
        private Button _resetButton;

        protected override void OnActivate()
        {
            _soundsToggle.onValueChanged.AddListener(ViewModel.SetSoundsState);
            _musicToggle.onValueChanged.AddListener(ViewModel.SetMusicState);
            _notificationToggle.onValueChanged.AddListener(ViewModel.SetNotificationsState);

            _closeButton.onClick.AddListener(Deactivate);
            _resetButton.onClick.AddListener(ViewModel.Deactivate);
        }

        protected override void OnDeactivate()
        {
            _soundsToggle.onValueChanged.RemoveListener(ViewModel.SetSoundsState);
            _musicToggle.onValueChanged.RemoveListener(ViewModel.SetMusicState);
            _notificationToggle.onValueChanged.RemoveListener(ViewModel.SetNotificationsState);

            _closeButton.onClick.RemoveListener(Deactivate);
            _resetButton.onClick.RemoveListener(ViewModel.Deactivate);

            ViewModel.SaveSettings();
        }
    }
}