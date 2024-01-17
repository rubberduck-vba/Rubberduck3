using Microsoft.Extensions.Logging;
using Rubberduck.UI.Command.Abstract;
using Rubberduck.UI.Services;
using Rubberduck.UI.Shared.Message;
using Rubberduck.UI.Shell.Tools.ServerTrace;
using System;
using System.Diagnostics;
using System.IO.Abstractions;
using System.Linq;
using System.Threading.Tasks;

namespace Rubberduck.UI.Command.SharedHandlers
{
    public class OpenLogFileCommand : CommandBase
    {
        private readonly IFileSystem _fileSystem;
        private readonly IMessageService _messages;

        public OpenLogFileCommand(UIServiceHelper service, IFileSystem fileSystem, IMessageService messages)
            : base(service)
        {
            _fileSystem = fileSystem;
            _messages = messages;
        }

        protected async override Task OnExecuteAsync(object? parameter)
        {
            string? path = default;
            if (parameter is ILanguageServerTraceViewModel)
            {
                path = Service.Settings.LanguageServerSettings.StartupSettings.ServerExecutablePath;
            }
            // TODO parameter is IUpdateServerTraceViewModel, ITelemetryServerTraceViewModel

            if (path is null)
            {
                _messages.ShowMessage(new MessageModel
                {
                    Key = nameof(OpenLogFileCommand) + "_NoServerPath",
                    Level = LogLevel.Error,
                    Title = "Invalid Configuration",
                    Message = "**ServerExecutablePath** configuration is unexpectedly `null`; could not locate log directory.",
                });
                return;
            }
            var logDirectory = _fileSystem.DirectoryInfo.New(_fileSystem.Path.Combine(path, "logs"));

            var currentLog = logDirectory
                .GetFiles("*.log", System.IO.SearchOption.TopDirectoryOnly)
                .OrderByDescending(file => file.LastWriteTime)
                .FirstOrDefault();

            if (currentLog is null)
            {
                _messages.ShowMessage(new MessageModel
                {
                    Key = nameof(OpenLogFileCommand) + "_NoLogFile",
                    Level = LogLevel.Warning,
                    Title = "Nothing to see here...",
                    Message = "Server log directory was found, but it appears to not contain any log files... for now."
                });
                return;
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = "explorer",
                Arguments = "\"" + currentLog.FullName + "\""
            };
            Process.Start(startInfo);
        }
    }
}
