using Microsoft.Extensions.Logging;
using Rubberduck.Editor.FileMenu;
using Rubberduck.Editor.Message;
using Rubberduck.InternalApi.Common;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Settings;
using Rubberduck.SettingsProvider.Model;
using System;
using System.Windows;

namespace Rubberduck.Editor.Shell
{
    public abstract class DialogService<TView, TViewModel> : IDialogService<TViewModel>
        where TView : Window
        where TViewModel : IDialogWindowViewModel
    {
        private readonly ILogger _logger;
        private readonly IWindowFactory<TView, TViewModel> _factory;
        private readonly ISettingsProvider<RubberduckSettings> _settingsProvider;
        private readonly MessageActionsProvider _actionsProvider;

        protected DialogService(ILogger logger, IWindowFactory<TView, TViewModel> factory, ISettingsProvider<RubberduckSettings> settingsProvider, MessageActionsProvider actionsProvider)
        {
            _logger = logger;
            _factory = factory;
            _settingsProvider = settingsProvider;
            _actionsProvider = actionsProvider;
        }

        protected abstract TViewModel CreateViewModel(RubberduckSettings settings, MessageActionsProvider actions);

        public virtual TViewModel ShowDialog()
        {
            TViewModel viewModel = default!;
            TView view = default!;

            var actions = _actionsProvider;
            var settings = _settingsProvider.Settings;
            var trace = settings.TelemetryServerSettings.TraceLevel.ToTraceLevel();

            if (TimedAction.TryRun(() =>
            {
                viewModel = CreateViewModel(settings, actions)
                    ?? throw new ArgumentNullException(nameof(viewModel), $"CreateViewModel returned null.");

                view = _factory.Create(viewModel) 
                    ?? throw new ArgumentNullException(nameof(view), $"View factory returned null.");

            }, out var elapsed, out var exception))
            {
                _logger.LogPerformance(trace, $"Created view ({typeof(TView).Name}) and view model ({typeof(TViewModel).Name}) instances.", elapsed);

                if (TimedAction.TryRun(() =>
                {
                    view.ShowDialog();
                }, out var elapsedShown, out var exceptionShown))
                {
                    _logger.LogPerformance(trace, $"ShowDialog ({typeof(TView).Name}) completed.", elapsedShown);
                }
                else if(exceptionShown is not null)
                {
                    _logger.LogError(trace, exceptionShown);
                }
            }
            else if (exception is not null)
            {
                _logger.LogCritical(trace, exception);
                // TODO return a vm with a default SelectedAction instead?
                throw new InvalidOperationException("The requested dialog could not be shown; an invalid view model cannot be returned.", exception);
            }

            return viewModel;
        }
    }
}
