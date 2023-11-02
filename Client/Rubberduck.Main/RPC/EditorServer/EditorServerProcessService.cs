using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Settings;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.Main.Commands.ShowRubberduckEditor;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.Unmanaged.WindowsApi;
using System;
using System.Diagnostics;
using Rubberduck.InternalApi.Common;

namespace Rubberduck.Main.RPC.EditorServer
{
    class EditorServerProcessService : IEditorServerProcessService, IDisposable
    {
        private Process? _process;
        private readonly ILogger _logger;
        private readonly ISettingsProvider<RubberduckSettings> _settingsProvider;

        public EditorServerProcessService(ILogger<EditorServerProcessService> logger, ISettingsProvider<RubberduckSettings> settingsProvider)
        {
            _logger = logger;
            _settingsProvider = settingsProvider;
        }

        private TraceLevel TraceLevel => _settingsProvider.Settings.GeneralSettings.TraceLevel.ToTraceLevel();

        public Exception? ShowEditor()
        {
            if (_process is null)
            {
                _settingsProvider.ClearCache();
                return StartEditor();
            }

            return BringToFront();
        }

        private Exception? StartEditor()
        {
            if (TimedAction.TryRun(() =>
            {
                var helper = new EditorServerProcess(_logger);
                var startupOptions = _settingsProvider.Settings.LanguageClientSettings.StartupSettings;
                var currentProcessId = Environment.ProcessId;
                _process = helper.Start(currentProcessId, startupOptions);
            }, out var elapsed, out var exception))
            {
                _logger.LogPerformance(TraceLevel, "Started Rubberduck Editor process.", elapsed);
            }
            else if (exception is not null)
            {
                _logger.LogError(TraceLevel, exception);
                return exception;
            }

            return null;
        }

        private Exception? BringToFront()
        {
            if (TimedAction.TryRun(() =>
            {
                var process = _process ?? throw new InvalidOperationException();
                User32.SetForegroundWindow(process.MainWindowHandle);
            }, out var elapsed, out var exception))
            {
                _logger.LogPerformance(TraceLevel, "Brought Rubberduck Editor process main window to foreground.", elapsed);
            }
            else if (exception is not null)
            {
                _logger.LogError(TraceLevel, exception);
                return exception;
            }

            return null;
        }

        public void Dispose()
        {
            _process?.Dispose();
        }
    }
}