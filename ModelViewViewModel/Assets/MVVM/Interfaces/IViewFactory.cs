﻿#nullable enable

using UnityEngine;

namespace QModules.MVVM
{
    public interface IViewFactory
    {
        public T InstantiateView<T>(T prefab, Transform? parent) where T : IView;
        public void DestroyView(IView view);
    }
}
