using Microsoft.Extensions.Logging;
using Rubberduck.ServerPlatform;
using Rubberduck.SettingsProvider;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.Unmanaged.WindowsApi;
using System;
using System.Diagnostics;
using Env = System.Environment;

namespace Rubberduck.Main.RPC.EditorServer
{
    class EditorServerProcessService : ServerPlatform.ServiceBase, IServerProcessService
    {
        private readonly ILogger _logger;
        private Process? _process;

        public EditorServerProcessService(ILogger<EditorServerProcessService> logger, RubberduckSettingsProvider settingsProvider, IWorkDoneProgressStateService workdone, PerformanceRecordAggregator performance)
            : base(logger, settingsProvider, workdone, performance)
        {
            _logger = logger;
        }

        public bool StartServerProcess()
        {
            if (_process is null)
            {
                (Settings as ISettingsService<RubberduckSettings>)?.ClearCache();
                StartEditor();
                return true;
            }

            BringToFront();
            return false;
        }

        public Process Process => _process ?? throw new InvalidOperationException("Process is not initialized.");

        private void StartEditor()
        {
            var helper = new EditorServerProcess(_logger);
            var startupOptions = Settings.LanguageClientSettings.StartupSettings;
            var currentProcessId = Env.ProcessId;
            _process = helper.Start(currentProcessId, startupOptions, HandleServerProcessExit);
        }

        private void HandleServerProcessExit(object? sender, EventArgs e)
        {
            LogWarning($"EditorServer process has exited.", $"ExitCode: {(sender as Process)?.ExitCode}");
        }

        private void BringToFront()
        {
            // TODO move this to a server-side command
            TryRunAction(() =>
            {
                var process = _process ?? throw new InvalidOperationException();
                User32.SetForegroundWindow(process.MainWindowHandle);
            });
        }

        public void Dispose()
        {
            _process?.Dispose();
        }
    }
}