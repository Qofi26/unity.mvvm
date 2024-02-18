namespace Erem.MVVM
{
    public interface IViewModel
    {
        public bool IsActive { get; }
        public bool IsInitialized { get; }
        public void Initialize();
        public void Shutdown();
        public void Activate();
        public void Deactivate();
        public void Update(float deltaTime);
    }

    public interface IViewModel<T> : IViewModel
    {
        public T Args { get; }
        public void Activate(T args);
        public void SetArgs(T args);
    }
}