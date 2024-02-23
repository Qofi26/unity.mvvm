using UnityEngine;

namespace Erem.MVVM.Examples
{
    public class Initializer : MonoBehaviour
    {
        [SerializeField]
        private HudView _hudView;

        private void Awake()
        {
            _hudView.Initialize(null);
            _hudView.Activate();
        }
    }
}