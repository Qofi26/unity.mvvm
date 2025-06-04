using QModules.MVVM;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Erem.MVVM.Examples
{
    public class HudViewDemo : AbstractView<EmptyViewModelDemo>
    {
        [SerializeField]
        private InfoViewDemo _infoView;

        [SerializeField]
        private SettingsViewDemo _settingsView;

        [SerializeField]
        private Button _openInfoViewButton;

        [SerializeField]
        private Button _openSettingsButton;

        protected override void OnActivate()
        {
            _openInfoViewButton.onClick.AddListener(HandleOpenInfoViewClick);
            _openSettingsButton.onClick.AddListener(HandleOpenSettingsViewClick);
        }

        protected override void OnDeactivate()
        {
            _openInfoViewButton.onClick.RemoveListener(HandleOpenInfoViewClick);
            _openSettingsButton.onClick.RemoveListener(HandleOpenSettingsViewClick);
        }

        private void HandleOpenInfoViewClick()
        {
            var value = Random.Range(byte.MinValue, byte.MaxValue);
            var text = $"Random value: {value}";
            _infoView.Activate(text);
        }

        private void HandleOpenSettingsViewClick()
        {
            _settingsView.Activate();
        }
    }
}