#nullable enable

using UnityEngine;
using Object = UnityEngine.Object;

namespace QModules.MVVM
{
    public class DefaultViewFactory : IViewFactory
    {
        public T InstantiateView<T>(T prefab, Transform? parent) where T : IView
        {
            if (prefab is not Object obj)
            {
                return default!;
            }

            var instance = Object.Instantiate(obj, parent);

            if (instance is not T view)
            {
                return default!;
            }

            view.gameObject.SetActive(false);
            return view;
        }

        public void DestroyView(IView view)
        {
            var viewGameObject = view.gameObject;
            if (!viewGameObject)
            {
                return;
            }

            if (Application.isPlaying)
            {
                Object.Destroy(viewGameObject);
            }
            else
            {
                Object.DestroyImmediate(viewGameObject);
            }
        }
    }
}
