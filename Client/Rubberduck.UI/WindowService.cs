using Microsoft.Extensions.Logging;
using Rubberduck.SettingsProvider;
using System.ComponentModel;
using System.Windows;

namespace Rubberduck.UI
{
    public abstract class WindowService<TView, TViewModel> : ServiceBase, IPresenter
        where TView : Window
        where TViewModel : class, INotifyPropertyChanged
    {
        private TView? _view;

        protected WindowService(ILogger<WindowService<TView, TViewModel>> logger, IRubberduckSettingsProvider settings, TViewModel viewModel)
            : base(logger, settings)
        {
            Model = viewModel;
        }

        public TViewModel Model { get; }
        protected abstract TView CreateWindow(TViewModel model);

        public void Close() => _view?.Close();

        public void Hide() => _view?.Hide();

        protected virtual bool PreconditionCheck() => true;

        public void Show()
        {
            TryRunAction(() =>
            {
                if (PreconditionCheck())
                {
                    _view ??= CreateWindow(Model);
                    _view.Show();
                }
                else
                {
                    LogDebug("Precondition check returned false; window will not be displayed.");
                }
            });
        }
    }
}
