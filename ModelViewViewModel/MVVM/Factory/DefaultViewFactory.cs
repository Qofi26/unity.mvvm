#nullable enable

using UnityEngine;
using Object = UnityEngine.Object;

namespace MVVM
{
    internal sealed class DefaultViewFactory : IViewFactory
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
                Destroy(instance);
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

            Destroy(viewGameObject);
        }

        private void Destroy(Object gameObject)
        {
            if (Application.isPlaying)
            {
                Object.Destroy(gameObject);
            }
            else
            {
                Object.DestroyImmediate(gameObject);
            }
        }
    }
}
