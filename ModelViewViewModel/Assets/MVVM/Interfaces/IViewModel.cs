namespace Erem.MVVM
{
    public interface IViewModel
    {
        public bool IsInitialized { get; }
        public bool IsActive { get; }

        public void Initialize();
        public void Shutdown();

        public void Activate();
        public void Deactivate();

        public void Update(float deltaTime);
    }

    public interface IViewModel<T> : IViewModel
    {
        public T Args { get; }
        public void SetArgs(T args);
    }
}