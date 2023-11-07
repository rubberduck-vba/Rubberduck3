using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Settings;
using Rubberduck.SettingsProvider.Model;
using System;
using System.ComponentModel;
using System.Windows;

namespace Rubberduck.UI
{
    public abstract class WindowService<TView, TViewModel> : IPresenter
        where TView : Window
        where TViewModel : class, INotifyPropertyChanged
    {
        private readonly ILogger _logger;
        private readonly ISettingsProvider<RubberduckSettings> _settings;
        private TView? _view;

        protected WindowService(ILogger<WindowService<TView, TViewModel>> logger, ISettingsProvider<RubberduckSettings> settings, TViewModel viewModel)
        {
            Model = viewModel;
            _logger = logger;
            _settings = settings;
        }

        public TViewModel Model { get; }
        protected abstract TView CreateWindow(TViewModel model);

        public void Close() => _view?.Close();

        public void Hide() => _view?.Hide();

        protected virtual bool PreconditionCheck() => true;

        public void Show()
        {
            if (PreconditionCheck())
            {
                _view ??= CreateWindow(Model);
                _view.Show();
            }
            else
            {
                var trace = _settings.Settings.GeneralSettings.TraceLevel.ToTraceLevel();
                _logger.LogDebug(trace, "Precondition check returned false; window will not be displayed.");
            }
        }
    }
}
