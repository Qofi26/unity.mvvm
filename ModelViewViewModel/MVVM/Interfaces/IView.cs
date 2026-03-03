// ReSharper disable InconsistentNaming

#nullable enable

using System;
using UnityEngine;

namespace MVVM
{
    public interface IView
    {
        public GameObject gameObject { get; }

        public string name { get; }

        public bool IsInitialized { get; }
        public bool ActivateWithParent { get; }
        public bool IsActive { get; }

        public void Initialize(IViewFactory? viewFactory, IViewModelFactory? viewModelFactory);
        public void Deinitialize();

        public bool SetActive(bool isActive);
        public void Activate();
        public void Deactivate();

        public bool TryGetView<TView>(out TView view, bool recursive) where TView : IView;
    }

    public interface IView<TArgs> : IView
    {
        public TArgs Args { get; }
        public void SetArgs(TArgs args);
        public void Activate(TArgs args);

        public Type ArgsType => typeof(TArgs);
    }
}
