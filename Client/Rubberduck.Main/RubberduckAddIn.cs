using Extensibility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.General;
using Rubberduck.Core;
using Rubberduck.Editor.RPC.LanguageServerClient;
using Rubberduck.InternalApi.Common;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.ServerPlatform;
using Rubberduck.Main.Root;
using Rubberduck.Main.RPC.EditorServer;
using Rubberduck.Resources;
using Rubberduck.SettingsProvider;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.SettingsProvider.Model.LanguageClient;
using Rubberduck.UI.Message;
using Rubberduck.Unmanaged;
using Rubberduck.Unmanaged.Abstract.SafeComWrappers.VB;
using Rubberduck.Unmanaged.TypeLibs.Public;
using Rubberduck.VBEditor.UI.OfficeMenus;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO.Abstractions;
using System.IO.Pipes;
using System.Reflection;
using System.Threading;
using System.Windows.Threading;
using EditorClient = OmniSharp.Extensions.LanguageServer.Client.LanguageClient;
using EditorClientOptions = OmniSharp.Extensions.LanguageServer.Client.LanguageClientOptions;

namespace Rubberduck.Main
{
    internal class RubberduckAddIn : IVBIDEAddIn
    {
        private RubberduckSettings? _initialSettings;
        private RubberduckSettings InitialSettings => _initialSettings ?? new RubberduckSettings();
        private TraceLevel TraceLevel => InitialSettings.LanguageServerSettings.TraceLevel.ToTraceLevel();

        private IVBE _vbe = null!;
        private IAddIn _addin = null!;
        private IServiceScope _serviceScope = null!;
        private ILogger _logger = null!;
        private App _app = null!;

        private bool _isInitialized;

        private IMessageService? _messageService;
        private IMessageService MessageBox => _messageService!;

        private readonly CancellationTokenSource _tokenSource = new();

        private IShowRubberduckEditorCommand? _showEditorCommand;
        private Process? _editorServerProcess;
        private NamedPipeClientStream? _editorServerPipeStream;
        private EditorClient? _editorClient;
        private IDisposable? _editorClientInitializeTask;

        //private Process? _telemetryServerProcess;
        //private NamedPipeClientStream? _telemetryServerPipeStream;
        //private LanguageClient? _telemetryClient;
        //private IDisposable? _telemetryClientInitializeTask;

        //private Process? _updateServerProcess;
        //private NamedPipeClientStream? _updateServerPipeStream;
        //private LanguageClient? _updateClient;
        //private IDisposable? _updateClientInitializeTask;

        internal RubberduckAddIn(IDTExtensibility2 extAddin, IVBE vbeWrapper, IAddIn addinWrapper)
        {
            _vbe = vbeWrapper;
            _addin = addinWrapper;
            _addin.Object = extAddin;

            SetAddInApiObject();
        }

        [Conditional("DEBUG")]
        private void SetAddInApiObject()
        {
            // FOR DEBUGGING/DEVELOPMENT PURPOSES, ALLOW ACCESS TO SOME VBETypeLibsAPI FEATURES FROM VBA
            _addin.Object = new VBETypeLibsAPI_Object(_vbe);
        }

        public static string GetVersionString()
        {
            var isDebugBuild = false;
#if DEBUG
            isDebugBuild = true;
#endif
            var version = Assembly.GetExecutingAssembly().GetName().Version!;
            return isDebugBuild
                ? $"{version.Major}.{version.Minor}.{version.Build}.x (debug)"
                : version.ToString();
        }

        public void Initialize()
        {
            if (_isInitialized)
            {
                // The add-in is already initialized. See:
                // The strange case of the add-in initialized twice
                // http://msmvps.com/blogs/carlosq/archive/2013/02/14/the-strange-case-of-the-add-in-initialized-twice.aspx
                return;
            }

            var sw = new Stopwatch();
            try
            {
                var tokenSource = new CancellationTokenSource();

                var provider = new RubberduckServicesBuilder(_vbe, _addin).Build();
                var scope = provider.CreateScope();

                _serviceScope = scope;
                _logger = scope.ServiceProvider.GetRequiredService<ILogger<RubberduckAddIn>>();
                _messageService = scope.ServiceProvider.GetRequiredService<IMessageService>();

                _initialSettings = GetInitialSettings(scope);
                _showEditorCommand = scope.ServiceProvider.GetRequiredService<IShowRubberduckEditorCommand>();
                _showEditorCommand.Executed += ShowRubberduckEditorCommand_Executed;

                var version = GetVersionString();

                sw.Start();
                Startup();
            }
            catch (StartupFailedException exception) when (exception.InnerException is ServerStartupFailedException serverException)
            {
                // server process did not start. most likely this is a debug build with a service resolution failure.
                // see server logs for details; break here to inspect process info in serverException.
                MessageBox.ShowError(nameof(RubberduckUI.RubberduckReloadFailure_Title), exception, LogLevel.Warning);
            }
            catch (Win32Exception exception)
            {
                MessageBox.ShowError(nameof(RubberduckUI.RubberduckReloadFailure_Message), exception, LogLevel.Warning);

                // add-in may have partially loaded, we want to clean up our mess as much as possible here.
                Shutdown(force: true);
            }
            catch (Exception exception)
            {
                _logger.LogCritical(TraceLevel, exception);
                MessageBox.ShowError(nameof(RubberduckUI.RubberduckLoadFailure), exception, LogLevel.Critical);
            }
            finally
            {
                sw.Stop();
                _logger.LogPerformance(TraceLevel, "Initialization completed.", sw.Elapsed);
            }
        }

        private RubberduckSettings GetInitialSettings(IServiceScope scope)
        {
            var configProvider = scope.ServiceProvider.GetRequiredService<ISettingsService<RubberduckSettings>>();
            var currentSettings = configProvider.Read();

            var initialSettings = currentSettings;

            try
            {
                var cultureInfo = CultureInfo.GetCultureInfo(initialSettings.GeneralSettings.Locale);
                CultureInfo.CurrentUICulture = cultureInfo;
                Dispatcher.CurrentDispatcher.Thread.CurrentUICulture = cultureInfo;
            }
            catch (CultureNotFoundException)
            {
                _logger.LogWarning("Could not find CultureInfo for locale string '{locale}'. InvariantCulture ('en-US') will be used.", initialSettings.GeneralSettings.Locale);
            }

            try
            {
                //if (InitialSettings.GeneralSettings.SetDpiUnaware)
                //{
                //    SHCore.SetProcessDpiAwareness(PROCESS_DPI_AWARENESS.Process_DPI_Unaware);
                //}
            }
            catch (Exception)
            {
                Debug.Assert(false, "Could not set DPI awareness.");
            }

            return initialSettings;
        }

        /// <exception cref="StartupFailedException"></exception>
        private void Startup()
        {
            try
            {
                var currentDomain = AppDomain.CurrentDomain;
                currentDomain.UnhandledException += HandleAppDomainException;
                currentDomain.AssemblyResolve += LoadFromSameFolder;

                _app = _serviceScope.ServiceProvider.GetRequiredService<App>();
                _app.Startup();

                _isInitialized = true;
            }
            catch (Exception exception)
            {
                _logger.LogError(TraceLevel, exception);
                throw new StartupFailedException(exception);
            }
        }

        private void ShowRubberduckEditorCommand_Executed(object? sender, EventArgs args)
        {
            var clientProcessId = Environment.ProcessId;
            StartEditorClient(clientProcessId, string.Empty, InitialSettings.LanguageClientSettings.StartupSettings);
        }

        private void StartEditorClient(int clientProcessId, string projectPath, LanguageClientStartupSettings settings)
        {
            if (_showEditorCommand is not null)
            {
                _showEditorCommand.Executed -= ShowRubberduckEditorCommand_Executed;
            }

            _editorServerProcess = new EditorServerProcess(_logger).Start(clientProcessId, settings);
            EditorClientOptions clientOptions;
            switch (settings.ServerTransportType)
            {
                case TransportType.StdIO:
                    clientOptions = EditorClientService.ConfigureEditorClient(Assembly.GetExecutingAssembly(), _editorServerProcess, clientProcessId, InitialSettings, projectPath);
                    break;

                case TransportType.Pipe:
                    var name = settings.ServerPipeName ?? ServerPlatformSettings.LanguageServerDefaultPipeName;
                    _editorServerPipeStream = new NamedPipeClientStream(".", $"{name}__{Environment.ProcessId}", PipeDirection.InOut, PipeOptions.Asynchronous);
                    //await _editorServerPipeStream.ConnectAsync(Convert.ToInt32(TimeSpan.FromSeconds(10).TotalMilliseconds)); // stuck here
                    clientOptions = EditorClientService.ConfigureEditorClient(Assembly.GetExecutingAssembly(), _editorServerPipeStream, clientProcessId, InitialSettings, projectPath);
                    break;

                default:
                    throw new NotSupportedException();
            }

            if (_editorServerProcess!.HasExited)
            {
                throw new ServerStartupFailedException(_editorServerProcess);
            }

            _editorClient = EditorClient.Create(clientOptions, _serviceScope.ServiceProvider);
            _editorClientInitializeTask = _editorClient.Initialize(_tokenSource.Token);
        }

        //private async Task StartTelemetryClientAsync(int clientProcessId, string projectPath, TelemetryServerSettings settings)
        //{
        //    _telemetryServerProcess = new TelemetryServerProcess(_logger).Start(clientProcessId, settings);

        //    LanguageClientOptions clientOptions;
        //    switch (settings.TransportType)
        //    {
        //        case TransportType.StdIO:
        //            clientOptions = LanguageClientService.ConfigureLanguageClient(Assembly.GetExecutingAssembly(), _telemetryServerProcess!, clientProcessId, _initialSettings, projectPath);
        //            break;

        //        case TransportType.Pipe:
        //            var name = settings.PipeName ?? ServerPlatformSettings.TelemetryServerDefaultPipeName;
        //            _telemetryServerPipeStream = new NamedPipeClientStream(".", $"{name}__{Environment.ProcessId}", PipeDirection.InOut, PipeOptions.Asynchronous);
        //            await _telemetryServerPipeStream.ConnectAsync(Convert.ToInt32(TimeSpan.FromSeconds(10).TotalMilliseconds)); // stuck here
        //            clientOptions = LanguageClientService.ConfigureLanguageClient(Assembly.GetExecutingAssembly(), _telemetryServerPipeStream, clientProcessId, _initialSettings, projectPath);
        //            break;

        //        default:
        //            throw new NotSupportedException();
        //    }

        //    if (_telemetryServerProcess!.HasExited)
        //    {
        //        throw new ServerStartupFailedException(_telemetryServerProcess);
        //    }

        //    _telemetryClient = LanguageClient.Create(clientOptions, _serviceScope.ServiceProvider);
        //}

        //private async Task StartUpdateClientAsync(int clientProcessId, string projectPath, UpdateServerSettings settings)
        //{
        //    _updateServerProcess = new UpdateServerProcess(_logger).Start(clientProcessId, settings);

        //    LanguageClientOptions clientOptions;
        //    switch (settings.TransportType)
        //    {
        //        case TransportType.StdIO:
        //            clientOptions = LanguageClientService.ConfigureLanguageClient(Assembly.GetExecutingAssembly(), _updateServerProcess!, clientProcessId, _initialSettings, projectPath);
        //            break;

        //        case TransportType.Pipe:
        //            var name = settings.PipeName ?? ServerPlatformSettings.UpdateServerDefaultPipeName;
        //            _updateServerPipeStream = new NamedPipeClientStream(".", $"{name}__{Environment.ProcessId}", PipeDirection.InOut, PipeOptions.Asynchronous);
        //            await _updateServerPipeStream.ConnectAsync(Convert.ToInt32(TimeSpan.FromSeconds(10).TotalMilliseconds)); // stuck here
        //            clientOptions = LanguageClientService.ConfigureLanguageClient(Assembly.GetExecutingAssembly(), _updateServerPipeStream, clientProcessId, _initialSettings, projectPath);
        //            break;

        //        default:
        //            throw new NotSupportedException();
        //    }

        //    if (_updateServerProcess!.HasExited)
        //    {
        //        throw new ServerStartupFailedException(_updateServerProcess);
        //    }

        //    _updateClient = LanguageClient.Create(clientOptions, _serviceScope.ServiceProvider);
        //}

        public void Shutdown(bool force = false)
        {
            if (!force && !_isInitialized)
            {
                return;
            }

            _logger.LogInformation("Rubberduck is shutting down...");

            RunShutdownAction("Sending shutdown notification to Rubberduck Editor...", () =>
            {
                _editorClient?.SendShutdown(new());
            });
            RunShutdownAction("Terminating VbeProvider...", () =>
            {
                VbeProvider.Terminate();
            });
            RunShutdownAction("Initiating App.Shutdown...", () =>
            {
                _app?.Shutdown();
                _app = null!;
            });
            RunShutdownAction("Disposing service scope...", () =>
            {
                _serviceScope?.Dispose();
                _serviceScope = null!;
            });
            RunShutdownAction("Disposing COM safe...", () =>
            {
                ComSafeManager.DisposeAndResetComSafe();
                _addin = null!;
                _vbe = null!;
            });
            RunShutdownAction("Deregistering AppDomain handlers....", () =>
            {
                AppDomain.CurrentDomain.AssemblyResolve -= LoadFromSameFolder;
                AppDomain.CurrentDomain.UnhandledException -= HandleAppDomainException;
            });
            RunShutdownAction("Sending exit notification to Rubberduck Editor...", () =>
            {
                _editorClient?.SendExit(new());
            });
            RunShutdownAction("Disposing Rubberduck Editor task...", () =>
            {
                _editorClientInitializeTask?.Dispose();
            });

            _isInitialized = false;
            _logger.LogInformation("Rubberduck shutdown completed. Quack!");
        }

        private void RunShutdownAction(string message, Action action)
        {
            var traceLevel = TraceLevel;
            if (TimedAction.TryRun(action, out var elapsed, out var exception))
            {
                _logger.LogPerformance(traceLevel, $"RunShutdownAction: {message}", elapsed);
            }
            else if (exception is not null)
            {
                _logger.LogError(traceLevel, exception);
            }
        }

        private void HandleAppDomainException(object sender, UnhandledExceptionEventArgs e)
        {
            var message = e.IsTerminating
                ? "An unhandled exception occurred. The runtime is shutting down."
                : "An unhandled exception occurred. The runtime continues running.";

            var traceLevel = TraceLevel;
            if (e.ExceptionObject is Exception exception)
            {
                _logger.LogCritical(traceLevel, exception);
            }
            else
            {
                _logger.LogCritical(traceLevel, message);
            }
        }

        private Assembly? LoadFromSameFolder(object? sender, ResolveEventArgs? args)
        {
            if (args is null)
            {
                return null!;
            }
            var fileSystem = _serviceScope.ServiceProvider.GetRequiredService<IFileSystem>();
            var folderPath = fileSystem.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
            var assemblyPath = fileSystem.Path.Combine(folderPath, new AssemblyName(args.Name).Name + ".dll");
            if (!fileSystem.File.Exists(assemblyPath))
            {
                return null!;
            }

            var assembly = Assembly.LoadFile(assemblyPath);
            return assembly;
        }
    }
}