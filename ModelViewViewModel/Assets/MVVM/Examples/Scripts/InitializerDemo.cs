using UnityEngine;

namespace Erem.MVVM.Examples
{
    public class InitializerDemo : MonoBehaviour
    {
        [SerializeField]
        private HudViewDemo _hudView;

        private void Awake()
        {
            _hudView.Initialize(null);
            _hudView.Activate();
        }
    }
}