using Microsoft.Extensions.Logging;
using Rubberduck.Main.Commands.ShowRubberduckEditor;
using Rubberduck.ServerPlatform;
using Rubberduck.SettingsProvider;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.Unmanaged.WindowsApi;
using System;
using System.Diagnostics;
using Env = System.Environment;

namespace Rubberduck.Main.RPC.EditorServer
{
    class EditorServerProcessService : ServerPlatform.ServiceBase, IEditorServerProcessService, IDisposable
    {
        private readonly ILogger _logger;
        private Process? _process;

        public EditorServerProcessService(ILogger<EditorServerProcessService> logger, IRubberduckSettingsProvider settingsProvider, IWorkDoneProgressStateService workdone)
            : base(logger, settingsProvider, workdone)
        {
            _logger = logger;
        }

        public Exception? ShowEditor()
        {
            if (_process is null)
            {
                (Settings as ISettingsService<RubberduckSettings>)?.ClearCache();
                return StartEditor();
            }

            return BringToFront();
        }

        private Exception? StartEditor()
        {
            TryRunAction(() =>
            {
                var helper = new EditorServerProcess(_logger);
                var startupOptions = Settings.LanguageClientSettings.StartupSettings;
                var currentProcessId = Env.ProcessId;
                _process = helper.Start(currentProcessId, startupOptions);
            }, out var exception);

            return exception;
        }

        private Exception? BringToFront()
        {
            // TODO move this to a server-side command
            TryRunAction(() =>
            {
                var process = _process ?? throw new InvalidOperationException();
                User32.SetForegroundWindow(process.MainWindowHandle);
            }, out var exception);

            return exception;
        }

        public void Dispose()
        {
            _process?.Dispose();
        }
    }
}