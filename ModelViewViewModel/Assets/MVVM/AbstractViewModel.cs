using UnityEngine;

namespace Erem.MVVM
{
    public abstract class AbstractViewModel : IViewModel
    {
        public bool IsActive { get; private set; }
        public bool IsInitialized { get; private set; }

        public void Initialize()
        {
            if (IsInitialized)
            {
                return;
            }

            IsInitialized = true;
            OnInitialize();
        }

        public void Shutdown()
        {
            if (!IsInitialized)
            {
                return;
            }

            IsInitialized = false;
            OnShutdown();
        }

        public void Activate()
        {
            IsActive = true;
            OnActivate();
        }

        public void Deactivate()
        {
            IsActive = false;
            OnDeactivate();
        }

        public void Update(float deltaTime)
        {
            OnUpdate(deltaTime);
        }

        protected virtual void OnInitialize()
        {
        }

        protected virtual void OnShutdown()
        {
        }

        protected virtual void OnActivate()
        {
        }

        protected virtual void OnDeactivate()
        {
        }

        protected virtual void OnUpdate(float deltaTime)
        {
        }
    }

    public abstract class AbstractViewModel<T> : AbstractViewModel, IViewModel<T>
    {
        public T Args { get; private set; }

        public void Activate(T args)
        {
            Args = args;
            Activate();
        }

        public void SetArgs(T args)
        {
            Args = args;
            OnArgsChanged();
        }

        protected virtual void OnArgsChanged()
        {
        }
    }
}