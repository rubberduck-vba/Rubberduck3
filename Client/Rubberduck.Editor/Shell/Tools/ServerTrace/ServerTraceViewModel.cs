using Rubberduck.Editor.Commands;
using Rubberduck.InternalApi.ServerPlatform;
using Rubberduck.SettingsProvider.Model.Editor.Tools;
using Rubberduck.UI.Command.Abstract;
using Rubberduck.UI.Command.SharedHandlers;
using Rubberduck.UI.Services;
using Rubberduck.UI.Services.Abstract;
using Rubberduck.UI.Shell.Tools.ServerTrace;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Rubberduck.Editor.Shell.Tools.ServerTrace
{
    public class ServerTraceViewModel : ToolWindowViewModelBase, IServerTraceViewModel
    {
        private readonly UIServiceHelper _service;

        public ServerTraceViewModel(UIServiceHelper service,
            ShowRubberduckSettingsCommand showSettingsCommand,
            CloseToolWindowCommand closeToolWindowCommand,
            OpenLogFileCommand openLogFileCommand,
            ShutdownServerCommand shutdownCommand)
            : base(DockingLocation.DockBottom, showSettingsCommand, closeToolWindowCommand)
        {
            _service = service;
            
            var itemsView = CollectionViewSource.GetDefaultView(_messages);
            itemsView.Filter = e => Filters.Filters.Contains(((LogMessageViewModel)e).Level);
            LogMessages = itemsView;

            Filters.FiltersChanged += OnFiltersChanged;

            OpenLogFileCommand = openLogFileCommand;
            
            CopyContentCommand = new DelegateCommand(service, param =>
            {
                Application.Current.Dispatcher.Invoke(() => Clipboard.SetText(string.Join('\n', _messages.Select(e => e.AsJsonString()))));
            }, param => _messages.Any());

            ClearContentCommand = new DelegateCommand(service, param =>
            {
                Application.Current.Dispatcher.Invoke(() => _messages.Clear());
            }, param => _messages.Any());
            
            ShutdownServerCommand = shutdownCommand;

            ClearFiltersCommand = new DelegateCommand(service, param =>
            {
                Application.Current.Dispatcher.Invoke(() => Filters.Clear());
            }, param => Filters.Filters.Length != 5);

            CommandBindings = [
                new CommandBinding(ServerTraceCommands.ClearContentCommand, ((CommandBase)ClearContentCommand).ExecutedRouted(), ((CommandBase)ClearContentCommand).CanExecuteRouted()),
                new CommandBinding(ServerTraceCommands.CopyContentCommand, ((CommandBase)CopyContentCommand).ExecutedRouted(), ((CommandBase)CopyContentCommand).CanExecuteRouted()),
                new CommandBinding(ServerTraceCommands.OpenLogFileCommand, ((CommandBase)OpenLogFileCommand).ExecutedRouted(), ((CommandBase)OpenLogFileCommand).CanExecuteRouted()),
                new CommandBinding(ServerTraceCommands.ShutdownServerCommand, ((CommandBase)ShutdownServerCommand).ExecutedRouted(), ((CommandBase)ShutdownServerCommand).CanExecuteRouted()),
            ];
        }

        private void OnFiltersChanged(object? sender, System.EventArgs e)
        {
            LogMessages.Refresh();
            OnPropertyChanged(nameof(ShownCount));
            OnPropertyChanged(nameof(Filters));
        }

        public override IEnumerable<CommandBinding> CommandBindings { get; }

        public ICommand CopyContentCommand { get; }
        public ICommand ClearContentCommand { get; }
        public ICommand OpenLogFileCommand { get; }
        public ICommand PauseResumeTraceCommand { get; }
        public ICommand ShutdownServerCommand { get; }
        public ICommand ClearFiltersCommand { get; }

        private bool _isPaused = false;
        public bool IsPaused
        {
            get => _isPaused;
            set
            {
                if (_isPaused != value)
                {
                    _isPaused = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _showVerbose = true;
        public bool ShowVerbose
        {
            get => _showVerbose;
            set
            {
                if (_showVerbose != value)
                {
                    _showVerbose = value;
                    OnPropertyChanged();
                }
            }
        }

        public int MessageCount => _messages.Count;
        public int ShownCount => LogMessages.OfType<LogMessageViewModel>().Count();

        private ObservableCollection<LogMessageViewModel> _messages = [];
        public ICollectionView LogMessages { get; }

        public LogMessageFiltersViewModel Filters { get; } = new();

        public void OnServerMessage(LogMessagePayload payload)
        {
            if (_isPaused)
            {
                return;
            }

            var max = (int)_service.Settings.EditorSettings.ToolsSettings.ServerTraceSettings.MaximumMessages;
            Application.Current.Dispatcher.Invoke(() =>
            {
                while (_messages.Count >= max)
                {
                    _messages.Remove(_messages[0]);
                }

                _messages.Add(new(payload));
                OnPropertyChanged(nameof(MessageCount));
                OnPropertyChanged(nameof(ShownCount));
            });
        }
    }
}
