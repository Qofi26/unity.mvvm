#nullable enable

using System;
using UnityEngine;

namespace QModules.MVVM
{
    public interface IView
    {
        public bool IsInitialized { get; }
        public bool ActivateWithParent { get; }

        public void Initialize(IViewFactory? viewFactory = null);
        public void Deinitialize();

        public bool SetActive(bool isActive);
        public void Activate();
        public void Deactivate();

        // ReSharper disable once InconsistentNaming
        public GameObject gameObject { get; }

        // ReSharper disable once InconsistentNaming
        public string name { get; }
    }

    public interface IView<TArgs> : IView
    {
        public TArgs Args { get; }
        public void SetArgs(TArgs args);
        public void Activate(TArgs args);

        public Type ArgsType => typeof(TArgs);
    }
}
