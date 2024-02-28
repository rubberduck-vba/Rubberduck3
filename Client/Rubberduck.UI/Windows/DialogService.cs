using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using Rubberduck.InternalApi.Settings.Model;
using Rubberduck.UI.Shared.Message;
using System;

namespace Rubberduck.UI.Windows
{
    public abstract class DialogService<TView, TViewModel> : ServiceBase, IDialogService<TViewModel>
        where TView : System.Windows.Window
        where TViewModel : IDialogWindowViewModel
    {
        private readonly IWindowFactory<TView, TViewModel> _factory;
        private readonly MessageActionsProvider _actionsProvider;

        protected DialogService(ILogger logger, IWindowFactory<TView, TViewModel> factory, RubberduckSettingsProvider settings, MessageActionsProvider actionsProvider, PerformanceRecordAggregator performance)
            : base(logger, settings, performance)
        {
            _factory = factory;
            _actionsProvider = actionsProvider;
        }

        protected abstract TViewModel CreateViewModel(RubberduckSettings settings, MessageActionsProvider actions);

        public virtual bool ShowDialog(out TViewModel viewModel)
        {
            TView view = default!;

            var actions = _actionsProvider;
            var verbosity = TraceLevel;

            var result = false;

            viewModel = CreateViewModel(Settings, actions)
                ?? throw new ArgumentNullException(nameof(viewModel), $"CreateViewModel returned null.");

            view = _factory.Create(viewModel)
                ?? throw new ArgumentNullException(nameof(view), $"ViewFactory.Create returned null.");

            var vm = viewModel;
            TryRunAction(() =>
            {
                if (view.ShowDialog() == true)
                {
                    OnDialogAccept(vm);
                    result = true;
                }
                else
                {
                    OnDialogCancel();
                }
            });

            return result;
        }

        protected virtual void OnDialogAccept(TViewModel model)
        {

        }

        protected virtual void OnDialogCancel()
        {

        }
    }
}
