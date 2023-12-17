using Rubberduck.SettingsProvider.Model.Editor.Tools;
using Rubberduck.UI.Command.Abstract;
using Rubberduck.UI.Command.SharedHandlers;
using Rubberduck.UI.Services;
using Rubberduck.UI.Services.Abstract;
using System.Windows;
using System.Windows.Input;

namespace Rubberduck.UI.LanguageServerTrace
{
    public class LanguageServerTraceViewModel : ServerTraceViewModel
    {
        public LanguageServerTraceViewModel(UIServiceHelper service, 
            ShowRubberduckSettingsCommand showSettingsCommand, 
            CloseToolWindowCommand closeToolWindowCommand) 
            : base(service, showSettingsCommand, closeToolWindowCommand)
        {
        }

        public override string Title { get; } = "Language Server Trace";
    }

    public class ServerTraceViewModel : ToolWindowViewModelBase, IServerTraceViewModel
    {
        public ServerTraceViewModel(UIServiceHelper service, 
            ShowRubberduckSettingsCommand showSettingsCommand, 
            CloseToolWindowCommand closeToolWindowCommand)
            : base(DockingLocation.DockBottom, showSettingsCommand, closeToolWindowCommand)
        {
            CopyContentCommand = new DelegateCommand(service, param => Clipboard.SetText(_consoleContent), param => ConsoleContent.Length > 0);
            ClearContentCommand = new DelegateCommand(service, param => ConsoleContent = string.Empty, param => ConsoleContent.Length > 0);
        }

        public override string Title { get; } = "Server Trace";
        public override string SettingKey { get; } //= nameof(ServerTraceToolSettings);

        public ICommand CopyContentCommand { get; }
        public ICommand ClearContentCommand { get; }
        public ICommand OpenLogFileCommand { get; }

        private bool _isPaused = true;
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

        private string _consoleContent = string.Empty;
        public string ConsoleContent
        {
            get => _consoleContent;
            set
            {
                if (_consoleContent != value)
                {
                    _consoleContent = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
