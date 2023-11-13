using Microsoft.Extensions.Logging;
using Rubberduck.SettingsProvider;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.UI.Command;
using System;
using System.Windows;

namespace Rubberduck.UI
{
    public abstract class DialogService<TView, TViewModel> : ServiceBase, IDialogService<TViewModel>
        where TView : Window
        where TViewModel : IDialogWindowViewModel
    {
        private readonly IWindowFactory<TView, TViewModel> _factory;
        private readonly MessageActionsProvider _actionsProvider;

        protected DialogService(ILogger logger, IWindowFactory<TView, TViewModel> factory, RubberduckSettingsProvider settings, MessageActionsProvider actionsProvider)
            : base(logger, settings)
        {
            _factory = factory;
            _actionsProvider = actionsProvider;
        }

        protected abstract TViewModel CreateViewModel(RubberduckSettings settings, MessageActionsProvider actions);

        public virtual TViewModel ShowDialog()
        {
            TViewModel viewModel = default!;
            TView view = default!;

            var actions = _actionsProvider;
            var verbosity = TraceLevel;

            if (TryRunAction(() =>
            {
                viewModel = CreateViewModel(Settings, actions)
                    ?? throw new ArgumentNullException(nameof(viewModel), $"CreateViewModel returned null.");

                view = _factory.Create(viewModel)
                    ?? throw new ArgumentNullException(nameof(view), $"ViewFactory.Create returned null.");
            }))
            {
                TryRunAction(() => view.ShowDialog());
                return viewModel;
            }

            throw new InvalidOperationException();
        }
    }
}
