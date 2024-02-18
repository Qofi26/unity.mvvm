#nullable enable

namespace Erem.MVVM
{
    public interface IView
    {
        public IView? Owner { get; }
        public bool IsInitialized { get; }
        public bool ActivateWithParent { get; }
        public void Initialize(IView owner);
        public void Shutdown();
        public void Activate();
        public void Deactivate();
    }

    public interface IView<TArgs> : IView
    {
        public TArgs Args { get; }
        public void Activate(TArgs args);
    }
}