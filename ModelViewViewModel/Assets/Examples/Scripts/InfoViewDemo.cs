using QModules.MVVM;
using UnityEngine;
using UnityEngine.UI;

namespace Erem.MVVM.Examples
{
    public class InfoViewDemo : AbstractView<InfoViewModelDemo, string>
    {
        [SerializeField]
        private Text _text;

        [SerializeField]
        private Button _closeButton;

        private string Message => Args;

        protected override void OnActivate()
        {
            _text.text = Message;

            _closeButton.onClick.AddListener(Deactivate);
        }

        protected override void OnDeactivate()
        {
            _closeButton.onClick.RemoveListener(Deactivate);
        }
    }
}