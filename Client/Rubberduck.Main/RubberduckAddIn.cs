using Extensibility;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO.Abstractions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
using Rubberduck.Core;
using Rubberduck.Root;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.SettingsProvider;
using Rubberduck.Unmanaged;
using Rubberduck.Unmanaged.WindowsApi;
using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Common;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.Unmanaged.Abstract.SafeComWrappers.VB;
using Rubberduck.Unmanaged.TypeLibs.Public;

namespace Rubberduck
{
    internal class RubberduckAddIn : IVBIDEAddIn
    {
        private RubberduckSettings _initialSettings;

        private IVBE _vbe = null!;
        private IAddIn _addin = null!;
        private IServiceScope _serviceScope = null!;
        private ILogger _logger = null!;
        private App _app = null!;

        private bool _isInitialized;

        //private readonly CancellationTokenSource _tokenSource = new();
        
        //private Process? _languageServerProcess;
        //private NamedPipeClientStream? _languageServerPipeStream;
        //private LanguageClient? _languageClient;
        //private IDisposable? _languageClientInitializeTask;

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

        public async Task InitializeAsync()
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

                var builder = new RubberduckServicesBuilder()
                    .WithAddIn(_vbe, _addin) // TODO clean this up, this "fluent" API makes no sense.
                    .WithSettingsProviders()
                    .WithApplication()
                    .WithAssemblyInfo()
                    .WithFileSystem(_vbe)
                    .WithNativeServices(_vbe)
                    .WithCommands()
                    .WithRubberduckMenu();
                    //.WithRubberduckEditor()

                var provider = builder.Build();
                var scope = provider.CreateScope();

                _serviceScope = scope;
                _logger = scope.ServiceProvider.GetRequiredService<ILogger<RubberduckAddIn>>();

                InitializeSettings(scope);
                sw.Start();

                var version = GetVersionString();
                await StartupAsync().ConfigureAwait(false);
            }
            catch (StartupFailedException exception) when (exception.InnerException is ServerStartupFailedException serverException)
            {
                // server process did not start. most likely this is a debug build with a service resolution failure.
                // see server logs for details; break here to inspect process info in serverException.
                MessageBox.Show(serverException.Message, Resources.RubberduckUI.RubberduckReloadFailure_Title,
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            catch (Win32Exception)
            {
                MessageBox.Show(Resources.RubberduckUI.RubberduckReloadFailure_Message,
                    Resources.RubberduckUI.RubberduckReloadFailure_Title,
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                // add-in may have partially loaded, we want to clean up our mess as much as possible here.
                Shutdown(force: true);
            }
            catch (Exception exception)
            {
                var traceLevel = _initialSettings.LanguageServerSettings.TraceLevel.ToTraceLevel();
                _logger.LogCritical(traceLevel, exception);
                // TODO Use Rubberduck Interaction instead and provide exception stack trace as
                // an optional "more info" collapsible section to eliminate the conditional.
                MessageBox.Show(
#if DEBUG
                    exception.ToString(),
#else
                    exception.Message.ToString(),
#endif
                    Resources.RubberduckUI.RubberduckLoadFailure, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                sw.Stop();
                _logger.LogPerformance(_initialSettings.LanguageServerSettings.TraceLevel.ToTraceLevel(), "Initialization completed.", sw.Elapsed);
            }
        }

        private void InitializeSettings(IServiceScope scope)
        {
            var configProvider = scope.ServiceProvider.GetRequiredService<ISettingsService<RubberduckSettings>>();
            var currentSettings = configProvider.Read();
            
            _initialSettings = currentSettings;

            try
            {
                var cultureInfo = CultureInfo.GetCultureInfo(_initialSettings.GeneralSettings.Locale);
                CultureInfo.CurrentUICulture = cultureInfo;
                Dispatcher.CurrentDispatcher.Thread.CurrentUICulture = cultureInfo;
            }
            catch (CultureNotFoundException)
            {
                // will load "invariant" (en-US) resources
            }

            try
            {
                //if (_initialSettings.GeneralSettings.SetDpiUnaware)
                //{
                //    SHCore.SetProcessDpiAwareness(PROCESS_DPI_AWARENESS.Process_DPI_Unaware);
                //}
            }
            catch (Exception)
            {
                Debug.Assert(false, "Could not set DPI awareness.");
            }
        }

        private async Task StartupAsync()
        {
            try
            {
                var currentDomain = AppDomain.CurrentDomain;
                currentDomain.UnhandledException += HandleAppDomainException;
                currentDomain.AssemblyResolve += LoadFromSameFolder;

                _app = _serviceScope.ServiceProvider.GetRequiredService<App>();
                _app.Startup();

                var clientProcessId = Environment.ProcessId;
                var projectPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                // TODO wire up LSP clients in the editor process instead.
                // TODO configure add-in RPC server (+editor client)

                /*
                await StartLanguageClientAsync(clientProcessId, projectPath, _initialSettings.LanguageServerSettings);

                if (_initialSettings.UpdateServerSettings.IsEnabled)
                {
                    await StartUpdateClientAsync(clientProcessId, projectPath, _initialSettings.UpdateServerSettings);
                }

                if (_initialSettings.TelemetryServerSettings.IsEnabled)
                {
                    await StartTelemetryClientAsync(clientProcessId, projectPath, _initialSettings.TelemetryServerSettings);
                }

                // TODO trigger LSP init from a menu/command in the add-in

                //if (_languageClient is not null)
                //{
                //    _languageClientInitializeTask = _languageClient.Initialize(_tokenSource.Token);
                //    splashService.UpdateStatus("Connection established.");
                //}
                //else
                //{
                //    throw new InvalidOperationException("Language client could be initialized.");
                //}
                */
                _isInitialized = true;
            }
            catch (Exception exception)
            {
                var traceLevel = _initialSettings.LanguageServerSettings.TraceLevel.ToTraceLevel();
                _logger.LogError(traceLevel, exception);
                throw new StartupFailedException(exception);
            }
        }

        #region TODO move to .Editor
        //private async Task StartLanguageClientAsync(int clientProcessId, string projectPath, LanguageServerSettings settings)
        //{
        //    _languageServerProcess = new LanguageServerProcess(_logger).Start(clientProcessId, settings);

        //    LanguageClientOptions clientOptions;
        //    switch (settings.TransportType)
        //    {
        //        case TransportType.StdIO:
        //            clientOptions = LanguageClientService.ConfigureLanguageClient(Assembly.GetExecutingAssembly(), _languageServerProcess!, clientProcessId, _initialSettings, projectPath);
        //            break;

        //        case TransportType.Pipe:
        //            var name = settings.PipeName ?? ServerPlatformSettings.LanguageServerDefaultPipeName;
        //            _languageServerPipeStream = new NamedPipeClientStream(".", $"{name}__{Environment.ProcessId}", PipeDirection.InOut, PipeOptions.Asynchronous);
        //            await _languageServerPipeStream.ConnectAsync(Convert.ToInt32(TimeSpan.FromSeconds(10).TotalMilliseconds)); // stuck here
        //            clientOptions = LanguageClientService.ConfigureLanguageClient(Assembly.GetExecutingAssembly(), _languageServerPipeStream, clientProcessId, _initialSettings, projectPath);
        //            break;

        //        default:
        //            throw new NotSupportedException();
        //    }

        //    if (_languageServerProcess!.HasExited)
        //    {
        //        throw new ServerStartupFailedException(_languageServerProcess);
        //    }

        //    _languageClient = LanguageClient.Create(clientOptions, _serviceScope.ServiceProvider);
        //}

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
        #endregion

        public void Shutdown(bool force = false)
        {
            if (!force && !_isInitialized)
            {
                return;
            }

            _logger.LogInformation("Rubberduck is shutting down...");

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

            _isInitialized = false;
            _logger.LogInformation("Rubberduck shutdown completed. Quack!");
        }

        private void RunShutdownAction(string message, Action action)
        {
            var traceLevel = _initialSettings.LanguageServerSettings.TraceLevel.ToTraceLevel();
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

            var traceLevel = _initialSettings.LanguageServerSettings.TraceLevel.ToTraceLevel();
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