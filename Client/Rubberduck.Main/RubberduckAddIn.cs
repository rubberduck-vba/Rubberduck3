using Extensibility;
using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.General;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO.Abstractions;
using System.IO.Pipes;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
using Rubberduck.Client;
using Rubberduck.Core;
using Rubberduck.InternalApi.ServerPlatform;
using Rubberduck.Root;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.SettingsProvider;
using Rubberduck.Unmanaged;
using Rubberduck.Unmanaged.WindowsApi;
using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Common;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.UI.Splash;
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

        private readonly System.Windows.Application _application = new();

        private readonly CancellationTokenSource _tokenSource = new();
        private Process? _serverProcess;
        private NamedPipeClientStream? _pipeStream;

        private LanguageClient? _languageClient;
        private IDisposable? _clientInitializeTask;

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
            SplashService? splashService = default;

            try
            {
                var tokenSource = new CancellationTokenSource();

                var builder = new RubberduckServicesBuilder()
                    .WithAddIn(_vbe, _addin)
                    .WithSettingsProviders()
                    .WithApplication()
                    .WithAssemblyInfo()
                    .WithFileSystem(_vbe)
                    .WithNativeServices(_vbe)
                    .WithCommands()
                    .WithRubberduckMenu()
                    .WithRubberduckEditor();

                var provider = builder.Build();
                var scope = provider.CreateScope();

                _serviceScope = scope;
                _logger = scope.ServiceProvider.GetRequiredService<ILogger<RubberduckAddIn>>();

                InitializeSettings(scope);
                splashService = scope.ServiceProvider.GetRequiredService<SplashService>();
                if (splashService.CanShowSplash)
                {
                    sw.Start();
                    splashService.Show();
                }
                else
                {
                    _logger.LogTrace(_initialSettings.LanguageServerSettings.TraceLevel.ToTraceLevel(), "Splash was not shown.", $"ShowSplash setting value: {_initialSettings.ShowSplash}");
                }

                var version = GetVersionString();
                await StartupAsync(splashService).ConfigureAwait(false);
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
                splashService?.Close();
                sw.Stop();
                var message = (splashService?.CanShowSplash ?? false) 
                    ? "Initialization completed." 
                    : "Initialization completed; splash window was shown during initialization.";
                _logger.LogPerformance(_initialSettings.LanguageServerSettings.TraceLevel.ToTraceLevel(), message, sw.Elapsed);
            }
        }

        private void InitializeSettings(IServiceScope scope)
        {
            var configProvider = scope.ServiceProvider.GetRequiredService<ISettingsService<RubberduckSettings>>();
            var currentSettings = configProvider.Read();
            
            _initialSettings = currentSettings;

            try
            {
                var cultureInfo = CultureInfo.GetCultureInfo(_initialSettings.Locale);
                CultureInfo.CurrentUICulture = cultureInfo;
                Dispatcher.CurrentDispatcher.Thread.CurrentUICulture = cultureInfo;
            }
            catch (CultureNotFoundException)
            {
                // will load "invariant" (en-US) resources
            }

            try
            {
                if (_initialSettings.SetDpiUnaware)
                {
                    SHCore.SetProcessDpiAwareness(PROCESS_DPI_AWARENESS.Process_DPI_Unaware);
                }
            }
            catch (Exception)
            {
                Debug.Assert(false, "Could not set DPI awareness.");
            }
        }

        private async Task StartupAsync(SplashService splashService)
        {
            try
            {
                var currentDomain = AppDomain.CurrentDomain;
                currentDomain.UnhandledException += HandleAppDomainException;
                currentDomain.AssemblyResolve += LoadFromSameFolder;

                splashService.UpdateStatus("Resolving services...");

                _app = _serviceScope.ServiceProvider.GetRequiredService<App>();

                splashService.UpdateStatus("Starting add-in...");
                
                _app.Startup();
                splashService.UpdateStatus("Starting language server...");

                var settings = LanguageServerSettings.Default;
                var clientProcessId = Environment.ProcessId;
                _serverProcess = new LanguageServerProcess(_logger).Start(clientProcessId, settings);

                splashService.UpdateStatus("Starting language client...");

                using var project = _vbe.ActiveVBProject;
                if (!project.TryGetFullPath(out var projectPath))
                {
                    projectPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                }

                LanguageClientOptions clientOptions;
                switch (settings.TransportType)
                {
                    case TransportType.StdIO:
                        clientOptions = LanguageClientService.ConfigureLanguageClient(Assembly.GetExecutingAssembly(), _serverProcess, clientProcessId, _initialSettings, projectPath);
                        break;

                    case TransportType.Pipe:
                        var name = settings.PipeName ?? ServerPlatformSettings.LanguageServerDefaultPipeName;
                        _pipeStream = new NamedPipeClientStream(".", $"{name}__{Environment.ProcessId}", PipeDirection.InOut, PipeOptions.Asynchronous);
                        await _pipeStream.ConnectAsync(Convert.ToInt32(TimeSpan.FromSeconds(10).TotalMilliseconds)); // stuck here
                        clientOptions = LanguageClientService.ConfigureLanguageClient(Assembly.GetExecutingAssembly(), _pipeStream, clientProcessId, _initialSettings, projectPath);
                        break;
                        
                    default:
                        throw new NotSupportedException();
                }

                if (_serverProcess.HasExited)
                {
                    throw new ServerStartupFailedException(_serverProcess);
                }

                _languageClient = LanguageClient.Create(clientOptions, _serviceScope.ServiceProvider);

                splashService.UpdateStatus("Initializing language server protocol...");
                _clientInitializeTask = _languageClient.Initialize(_tokenSource.Token);

                splashService.UpdateStatus("Connection established.");
                _isInitialized = true;
            }
            catch (Exception exception)
            {
                var traceLevel = _initialSettings.LanguageServerSettings.TraceLevel.ToTraceLevel();
                _logger.LogError(traceLevel, exception);
                throw new StartupFailedException(exception);
            }
        }

        public void Shutdown(bool force = false)
        {
            if (!force && !_isInitialized)
            {
                _logger.LogWarning("Addin is not in an initialized state for shutdown.");
                return;
            }

            _logger.LogInformation("Rubberduck is shutting down...");
            RunShutdownAction("Sending LSP shutdown notification...", () =>
            {
                _languageClient?.SendShutdown(new ShutdownParams());
            });
            RunShutdownAction("Terminating VbeProvider...", () =>
            {
                VbeProvider.Terminate();
            });
            //RunShutdownAction("Releasing dockable hosts...", () =>
            //{
            //    using (var windows = _vbe.Windows)
            //    {
            //        windows.ReleaseDockableHosts();
            //    }
            //});
            RunShutdownAction("Initiating App.Shutdown...", () =>
            {
                _app?.Shutdown();
                _app = null!;
            });
            RunShutdownAction("Sending LSP exit notification...", () =>
            {
                _languageClient?.SendExit();
            });
            RunShutdownAction("Disposing service scope...", () =>
            {
                _serviceScope?.Dispose();
                _serviceScope = null!;
            });
            RunShutdownAction("Disposing pipe stream...", () =>
            {
                _pipeStream?.Dispose();
                _pipeStream = null!;
            });
            RunShutdownAction("Disposing language client...", () =>
            {
                _clientInitializeTask?.Dispose();
                _languageClient?.Dispose();
                _tokenSource?.Cancel();
            });
            RunShutdownAction("Disposing language server process...", () =>
            {
                if (_serverProcess != null)
                {
                    if (!_serverProcess.HasExited)
                    {
                        _serverProcess.WaitForExit(TimeSpan.FromMilliseconds(200));
                    }

                    if (_serverProcess.HasExited)
                    {
                        _logger.LogTrace("Language server process exit code: {code}. Exit time: {exitTime}", _serverProcess.ExitCode, _serverProcess.ExitTime);
                    }
                    else
                    {
                        _logger.LogWarning("Language server process did not exit after 20ms. Review server logs for possible anomalies.");
                    }
                    _serverProcess.Dispose();
                    _serverProcess = null!;
                }
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