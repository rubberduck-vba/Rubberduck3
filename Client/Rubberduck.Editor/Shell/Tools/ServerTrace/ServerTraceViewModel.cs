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
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Rubberduck.Editor.Shell.Tools.ServerTrace
{
    public class ServerTraceViewModel : ToolWindowViewModelBase, IServerTraceViewModel
    {
        private readonly UIServiceHelper _service;

        public ServerTraceViewModel(UIServiceHelper service,
            ShowRubberduckSettingsCommand showSettingsCommand,
            CloseToolWindowCommand closeToolWindowCommand,
            OpenLogFileCommand openLogFileCommand)
            : base(DockingLocation.DockBottom, showSettingsCommand, closeToolWindowCommand)
        {
            _service = service;

            OpenLogFileCommand = openLogFileCommand;
            CopyContentCommand = new DelegateCommand(service, param => Clipboard.SetText(string.Join('\n', LogMessages.Select(e => e.ToString()))), param => LogMessages.Any());
            ClearContentCommand = new DelegateCommand(service, param =>
            {
                Application.Current.Dispatcher.Invoke(() => LogMessages.Clear());
            }, param => LogMessages.Any());
            ShutdownServerCommand = new DelegateCommand(service, param => { /*TODO*/});

            CommandBindings = [
                new CommandBinding(ServerTraceCommands.ClearContentCommand, ((CommandBase)ClearContentCommand).ExecutedRouted(), ((CommandBase)ClearContentCommand).CanExecuteRouted()),
                new CommandBinding(ServerTraceCommands.CopyContentCommand, ((CommandBase)CopyContentCommand).ExecutedRouted(), ((CommandBase)CopyContentCommand).CanExecuteRouted()),
                new CommandBinding(ServerTraceCommands.OpenLogFileCommand, ((CommandBase)OpenLogFileCommand).ExecutedRouted(), ((CommandBase)OpenLogFileCommand).CanExecuteRouted()),
                new CommandBinding(ServerTraceCommands.ShutdownServerCommand, ((CommandBase)ShutdownServerCommand).ExecutedRouted(), ((CommandBase)ShutdownServerCommand).CanExecuteRouted()),
            ];
        }

        public override IEnumerable<CommandBinding> CommandBindings { get; }

        public ICommand CopyContentCommand { get; }
        public ICommand ClearContentCommand { get; }
        public ICommand OpenLogFileCommand { get; }
        public ICommand PauseResumeTraceCommand { get; }
        public ICommand ShutdownServerCommand { get; }

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

        public ObservableCollection<LogMessageViewModel> LogMessages { get; } = [];

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
                while (LogMessages.Count >= max)
                {
                    LogMessages.Remove(LogMessages[0]);
                }
                LogMessages.Add(new(payload));
            });
        }
    }
}
