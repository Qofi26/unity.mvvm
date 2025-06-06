#nullable enable

using System;
using System.Collections.Generic;

namespace QModules.MVVM
{
    public abstract class AbstractViewModel : IViewModel
    {
        public bool IsInitialized { get; private set; }
        public bool IsActive { get; private set; }

        internal readonly ICollection<IDisposable> DisposableDeinitialize = new HashSet<IDisposable>();
        internal readonly ICollection<IDisposable> DisposableDeactivate = new HashSet<IDisposable>();

        public void Initialize()
        {
            if (IsInitialized)
            {
                return;
            }

            IsInitialized = true;
            OnInitialize();
        }

        public void Deinitialize()
        {
            if (!IsInitialized)
            {
                return;
            }

            IsInitialized = false;
            OnDeinitialize();

            foreach (var disposable in DisposableDeinitialize)
            {
                disposable.Dispose();
            }

            DisposableDeinitialize.Clear();
        }

        public void Activate()
        {
            if (IsActive)
            {
                return;
            }

            IsActive = true;
            OnActivate();
            UpdateViewModel();
        }

        public void Deactivate()
        {
            if (!IsActive)
            {
                return;
            }

            IsActive = false;
            OnDeactivate();

            foreach (var disposable in DisposableDeactivate)
            {
                disposable.Dispose();
            }

            DisposableDeactivate.Clear();
        }

        public void Update(float deltaTime)
        {
            OnUpdate(deltaTime);
        }

        public void UpdateViewModel()
        {
            OnUpdatedViewModel();
        }

        protected virtual void OnUpdatedViewModel() { }

        protected virtual void OnInitialize() { }

        protected virtual void OnDeinitialize() { }

        protected virtual void OnActivate() { }

        protected virtual void OnDeactivate() { }

        protected virtual void OnUpdate(float deltaTime) { }
    }

    public abstract class AbstractViewModel<T> : AbstractViewModel, IViewModel<T>
    {
        public T Args { get; private set; } = default!;

        public void Activate(T args)
        {
            if (IsActive)
            {
                SetArgs(args);
                return;
            }

            Args = args;
            Activate();
        }

        public void SetArgs(T args)
        {
            Args = args;
            OnArgsChanged();
            UpdateViewModel();
        }

        protected virtual void OnArgsChanged() { }
    }

    public class EmptyViewModel : AbstractViewModel { }

    public class EmptyViewModel<T> : AbstractViewModel<T> { }
}
