#nullable enable

using System;

namespace MVVM
{
    public static class ViewExtensions
    {
        public static void AddToDeactivate(this IDisposable disposable, AbstractView view)
        {
            view.DisposableDeactivate.Add(disposable);
        }

        public static void AddToDispose(this IDisposable disposable, AbstractView view)
        {
            view.DisposableDeinitialize.Add(disposable);
        }

        public static void AddToDeactivate(this IDisposable disposable, AbstractViewModel viewModel)
        {
            viewModel.DisposableDeactivate.Add(disposable);
        }

        public static void AddToDispose(this IDisposable disposable, AbstractViewModel viewModel)
        {
            viewModel.DisposableDeinitialize.Add(disposable);
        }
    }
}
