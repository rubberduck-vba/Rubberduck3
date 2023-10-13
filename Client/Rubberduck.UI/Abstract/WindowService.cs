using System.ComponentModel;
using System.Windows;

namespace Rubberduck.UI.Abstract
{
    public abstract class WindowService<TView, TViewModel> : IPresenter
        where TView : Window
        where TViewModel : class, INotifyPropertyChanged
    {
        private TView? _view;

        protected WindowService(TViewModel viewModel)
        {
            Model = viewModel;
        }

        public TViewModel Model { get; }
        protected abstract TView CreateWindow(TViewModel model);

        public void Close() => _view?.Close();

        public void Hide() => _view?.Hide();

        public void Show()
        {
            _view ??= CreateWindow(Model);
            _view.Show();
        }
    }
}
