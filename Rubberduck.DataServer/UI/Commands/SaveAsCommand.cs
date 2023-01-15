using NLog;
using Rubberduck.RPC.Platform;
using Rubberduck.UI.Command;
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Win32;
using Rubberduck.UI.Abstract;

namespace Rubberduck.Client.LocalDb.UI.Commands
{
    public interface IFileNameProvider
    {
        bool GetSaveAsFileName(string title, string filter, string defaultValue, string defaultExt, out string result);
    }

    public class FileNameProvider : IFileNameProvider
    {
        public bool GetSaveAsFileName(string title, string filter, string defaultValue, string defaultExt, out string result)
        {
            var dialog = new SaveFileDialog
            {
                Title = title,
                Filter = filter,
                DefaultExt = defaultExt,
                FileName = defaultValue
            };
            
            if (dialog.ShowDialog() ?? false)
            {
                result = dialog.FileName;
                return true;
            }

            result = null;
            return false;
        }
    }

    public class SaveAsCommand : CommandBase
    {
        private readonly IJsonRpcServer _server;
        private readonly IFileNameProvider _fileNameProvider;
        private readonly int _processId;

        public SaveAsCommand(IJsonRpcServer server, IFileNameProvider fileNameProvider) : base(LogManager.GetCurrentClassLogger())
        {
            _server = server;
            _processId = server.ProcessId;
            _fileNameProvider = fileNameProvider;
        }

        protected override void OnExecute(object parameter)
        {
            _server.Console.Log(LogLevel.Info, $"Executing {nameof(SaveAsCommand)}...");

            if (parameter is IEnumerable<IConsoleMesssageViewModel> messages)
            {
                if (_fileNameProvider.GetSaveAsFileName("SaveAs", "Log files |*.log", $"Rubberduck.Client.LocalDb.PID{_processId}.log", "log", out var path))
                {
                    File.WriteAllText(path, string.Join(Environment.NewLine, messages));
                }
            }
        }
    }
}
