#nullable enable
namespace MVVM
{
    internal class DefaultViewModelFactory : IViewModelFactory
    {
        private static readonly IViewModel _emptyViewModel = new EmptyViewModel();

        public IViewModel GetEmpty()
        {
            return _emptyViewModel;
        }

        public TViewModel Create<TViewModel>() where TViewModel : IViewModel, new()
        {
            return new TViewModel();
        }
    }
}
