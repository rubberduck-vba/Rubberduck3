using Rubberduck.SettingsProvider.Model.Editor.Tools;
using Rubberduck.UI.Command.Abstract;
using Rubberduck.UI.Command.SharedHandlers;
using Rubberduck.UI.Services;
using Rubberduck.UI.Services.Abstract;
using Rubberduck.UI.Shell.Tools.ServerTrace;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Rubberduck.Editor.Shell.Tools.ServerTrace
{
    public class ServerTraceViewModel : ToolWindowViewModelBase, IServerTraceViewModel
    {
        private readonly StringBuilder _builder = new();

        public ServerTraceViewModel(UIServiceHelper service,
            ShowRubberduckSettingsCommand showSettingsCommand,
            CloseToolWindowCommand closeToolWindowCommand,
            OpenLogFileCommand openLogFileCommand)
            : base(DockingLocation.DockBottom, showSettingsCommand, closeToolWindowCommand)
        {
            OpenLogFileCommand = openLogFileCommand;
            CopyContentCommand = new DelegateCommand(service, param => Clipboard.SetText(TextContent), param => TextContent.Length > 0);
            ClearContentCommand = new DelegateCommand(service, param =>
            {
                _builder.Clear();
                TextContent = _builder.ToString();
            }, param => TextContent.Length > 0);
        }

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

        
        public void OnServerTrace(string message, string? verbose)
        {
            if (_isPaused) 
            {
                return; 
            }

            var line = message;
            verbose = _showVerbose ? verbose : null;
            if (!string.IsNullOrWhiteSpace(verbose))
            {
                line = $"{line} | {verbose}";
            }

            _builder.AppendLine(line);
            TextContent = _builder.ToString();
        }
    }
}
