#nullable enable
namespace MVVM
{
    public interface IViewModelFactory
    {
        public IViewModel GetEmpty();
        public T Create<T>() where T : IViewModel, new();
    }
}
