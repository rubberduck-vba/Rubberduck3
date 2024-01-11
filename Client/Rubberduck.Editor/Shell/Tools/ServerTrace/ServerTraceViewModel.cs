﻿using Rubberduck.SettingsProvider.Model.Editor.Tools;
using Rubberduck.UI.Command.Abstract;
using Rubberduck.UI.Command.SharedHandlers;
using Rubberduck.UI.Services;
using Rubberduck.UI.Services.Abstract;
using Rubberduck.UI.Shell.Tools.ServerTrace;
using System.Windows;
using System.Windows.Input;

namespace Rubberduck.Editor.Shell.Tools.ServerTrace
{
    public class ServerTraceViewModel : ToolWindowViewModelBase, IServerTraceViewModel
    {
        public ServerTraceViewModel(UIServiceHelper service,
            ShowRubberduckSettingsCommand showSettingsCommand,
            CloseToolWindowCommand closeToolWindowCommand,
            OpenLogFileCommand openLogFileCommand)
            : base(DockingLocation.DockBottom, showSettingsCommand, closeToolWindowCommand)
        {
            CopyContentCommand = new DelegateCommand(service, param => Clipboard.SetText(TextContent), param => TextContent.Length > 0);
            ClearContentCommand = new DelegateCommand(service, param => TextContent = string.Empty, param => TextContent.Length > 0);
            OpenLogFileCommand = openLogFileCommand;
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
    }
}