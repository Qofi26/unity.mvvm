#nullable enable

using System;

namespace QModules.MVVM
{
    public static class ViewExtensions
    {
        public static void DisposeOnDeactivate(this IDisposable disposable, AbstractView view)
        {
            view.DisposableDeactivate.Add(disposable);
        }

        public static void DisposeOnDeinitialize(this IDisposable disposable, AbstractView view)
        {
            view.DisposableDeinitialize.Add(disposable);
        }

        public static void DisposeOnDeactivate(this IDisposable disposable, AbstractViewModel viewModel)
        {
            viewModel.DisposableDeactivate.Add(disposable);
        }

        public static void DisposeOnDeinitialize(this IDisposable disposable, AbstractViewModel viewModel)
        {
            viewModel.DisposableDeinitialize.Add(disposable);
        }
    }
}
