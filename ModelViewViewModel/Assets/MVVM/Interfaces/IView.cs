#nullable enable

namespace Erem.MVVM
{
    public interface IView
    {
        public bool IsInitialized { get; }
        public IView? Owner { get; }
        public bool ActivateWithParent { get; }
        public float TickInterval { get; set; }

        public void Initialize(IView? owner);
        public void Shutdown();

        public bool SetActive(bool isActive);
        public void Activate();
        public void Deactivate();

        public bool TryGetView<TView>(out TView view);
    }

    public interface IView<TArgs> : IView
    {
        public TArgs Args { get; }
        public void SetArgs(TArgs args);
    }
}