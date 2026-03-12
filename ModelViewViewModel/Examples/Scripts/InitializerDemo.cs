using UnityEngine;

namespace MVVM.Examples.Scripts
{
    public class InitializerDemo : MonoBehaviour
    {
        [SerializeField]
        private HudViewDemo _hudView;

        private void Awake()
        {
            _hudView.Initialize(null, null);
            _hudView.Activate();
        }
    }
}
