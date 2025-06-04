#nullable enable

namespace Erem.MVVM
{
    public interface IViewModel
    {
        public bool IsInitialized { get; }
        public bool IsActive { get; }

        public void Initialize();
        public void Deinitialize();

        public void Activate();
        public void Deactivate();

        public void UpdateViewModel();

        public void Update(float deltaTime);
    }

    public interface IViewModel<T> : IViewModel
    {
        public T Args { get; }
        public void Activate(T args);
        public void SetArgs(T args);
    }
}
