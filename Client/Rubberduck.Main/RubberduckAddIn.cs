using Extensibility;
using Microsoft.Extensions.DependencyInjection;
using NLog;
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
using Rubberduck.Core.Splash;
using Rubberduck.InternalApi.ServerPlatform;
using Rubberduck.Root;
using Rubberduck.UI.Abstract;
using Rubberduck.UI.WinForms;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.SettingsProvider;
using Rubberduck.Unmanaged.Abstract.SafeComWrappers;
using Rubberduck.Unmanaged;
using Rubberduck.Unmanaged.Events;
using Rubberduck.Unmanaged.TypeLibs;

namespace Rubberduck
{
    internal class RubberduckAddIn : IVBIDEAddIn
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private RubberduckSettings _initialSettings;

        private IVBE _vbe = null!;
        private IAddIn _addin = null!;
        private IServiceScope _serviceScope = null!;
        private App _app = null!;

        private bool _isInitialized;

        private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();
        private Process? _serverProcess;
        private NamedPipeClientStream? _pipeStream;

        private LanguageClient? _languageClient;

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

            Splash? splash = null;
            ISplashViewModel? splashModel = null;

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
                    .WithMsoCommandBarMenu()
                    .WithRubberduckEditor();

                var provider = builder.Build();
                var scope = provider.CreateScope();
                _serviceScope = scope;

                await InitializeSettingsAsync(scope).ConfigureAwait(false);

                var version = GetVersionString();
                if (_initialSettings.ShowSplash)
                {
                    splashModel = new SplashViewModel(version);
                    splash = ShowSplash(splashModel);
                }

                await StartupAsync(splashModel, version).ConfigureAwait(false);
            }
            catch (Win32Exception)
            {
                MessageBox.Show(Resources.RubberduckUI.RubberduckReloadFailure_Message,
                    Resources.RubberduckUI.RubberduckReloadFailure_Title,
                    MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            catch (Exception exception)
            {
                _logger.Fatal(exception);
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
                splash?.Dispose();
            }
        }

        private Splash ShowSplash(ISplashViewModel model)
        {
            model.UpdateStatus("Open-Source VBIDE Add-In");
            var splash = new Splash(model);

            splash.Show();
            splash.Refresh();

            return splash;
        }

        private async Task InitializeSettingsAsync(IServiceScope scope)
        {
            var configProvider = scope.ServiceProvider.GetRequiredService<ISettingsService<RubberduckSettings>>();
            var currentSettings = await configProvider.ReadFromFileAsync();
            
            _initialSettings = currentSettings.Settings;

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

            //try
            //{
            //    if (_initialSettings.SetDpiUnaware)
            //    {
            //        SHCore.SetProcessDpiAwareness(PROCESS_DPI_AWARENESS.Process_DPI_Unaware);
            //    }
            //}
            //catch (Exception)
            //{
            //    Debug.Assert(false, "Could not set DPI awareness.");
            //}
        }

        private async Task StartupAsync(IStatusUpdate? statusViewModel, string version)
        {
            try
            {
                var currentDomain = AppDomain.CurrentDomain;
                currentDomain.UnhandledException += HandleAppDomainException;
                currentDomain.AssemblyResolve += LoadFromSameFolder;

                statusViewModel?.UpdateStatus("Resolving services...");

                _app = _serviceScope.ServiceProvider.GetRequiredService<App>();

                statusViewModel?.UpdateStatus("Starting add-in...");

                await _app.StartupAsync(version).ConfigureAwait(false);

                statusViewModel?.UpdateStatus("Starting language server...");

                var settings = LanguageServerSettings.Default;
                var clientProcessId = Process.GetCurrentProcess().Id;
                _serverProcess = new LanguageServerProcess().Start(clientProcessId, settings);
                
                statusViewModel?.UpdateStatus("Starting language client...");

                LanguageClientOptions clientOptions;
                switch (settings.TransportType)
                {
                    case TransportType.StdIO:
                        clientOptions = LanguageClientService.ConfigureLanguageClient(Assembly.GetExecutingAssembly(), _serverProcess, InitializeTrace.Verbose);
                        break;

                    case TransportType.Pipe:
                        var name = settings.PipeName ?? ServerPlatformSettings.LanguageServerDefaultPipeName;
                        _pipeStream = new NamedPipeClientStream(".", $"{name}__{Process.GetCurrentProcess().Id}", PipeDirection.InOut, PipeOptions.Asynchronous);
                        await _pipeStream.ConnectAsync(Convert.ToInt32(TimeSpan.FromSeconds(10).TotalMilliseconds));
                        clientOptions = LanguageClientService.ConfigureLanguageClient(Assembly.GetExecutingAssembly(), _pipeStream, InitializeTrace.Verbose);
                        break;

                    default:
                        throw new NotSupportedException();
                }

                _languageClient = LanguageClient.Create(clientOptions, _serviceScope.ServiceProvider);

                statusViewModel?.UpdateStatus("Initializing language server protocol...");
                await _languageClient.Initialize(_tokenSource.Token);

                statusViewModel?.UpdateStatus("Connection established.");
                _isInitialized = true;
            }
            catch (Exception e)
            {
                _logger.Fatal(e, "Startup sequence threw an unexpected exception.");
                throw new Exception($"'{ServerPlatformSettings.LanguageServerExecutable}' - Rubberduck's startup sequence threw an unexpected exception. Please check the Rubberduck logs for more information and report an issue if necessary", e);
            }
        }

        public async Task ShutdownAsync()
        {
            if (!_isInitialized)
            {
                return;
            }

            _logger.Info("Rubberduck is shutting down...");

            try
            {
                if (_languageClient != null)
                {
                    _logger.Trace("Sending LSP shutdown notification...");
                    _languageClient.SendShutdown(new ShutdownParams());
                }
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }

            try
            {
                _logger.Trace("Unhooking VBENativeServices events...");
                VbeNativeServices.UnhookEvents();
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }

            try
            {
                _logger.Trace("Terminating VbeProvier...");
                VbeProvider.Terminate();
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }

            try
            {
                _logger.Trace("Releasing dockable hosts...");
                using (var windows = _vbe.Windows)
                {
                    windows.ReleaseDockableHosts();
                }
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }

            try
            {
                if (_app != null)
                {
                    _logger.Trace("Initiating App.Shutdown...");
                    await _app.ShutdownAsync().ConfigureAwait(false);
                    _app = null!;
                }
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }

            try
            {
                if (_serviceScope != null)
                {
                    _logger.Trace("Disposing service scope...");
                    _serviceScope.Dispose();
                    _serviceScope = null!;
                }
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }

            try
            {
                if (_languageClient != null)
                {
                    _logger.Trace("Sending LSP exit notification...");
                    _languageClient.SendExit(new ExitParams());
                }
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }

            try
            {
                if (_pipeStream != null)
                {
                    _logger.Trace("Disposing pipe stream...");
                    await _pipeStream.DisposeAsync();
                    _pipeStream = null!;
                }
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }

            try
            {
                if (_languageClient != null)
                {
                    _logger.Trace("Disposing language client...");
                    _languageClient.SendExit();
                    _languageClient.Dispose();
                }
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }

            try
            {
                if (_serverProcess != null)
                {
                    _logger.Trace("Disposing language server process...");
                    _serverProcess.Dispose();
                    _serverProcess = null!;
                }
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }

            try
            {
                if (_languageClient != null)
                {
                    _logger.Trace("Disposing language client...");
                    _languageClient.Dispose();
                    _languageClient = null!;
                }
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }

            try
            {
                _logger.Trace("Disposing COM safe...");
                ComSafeManager.DisposeAndResetComSafe();
                _addin = null!;
                _vbe = null!;

                _isInitialized = false;
                _logger.Info("No exceptions were thrown.");
            }
            catch (Exception e)
            {
                _logger.Warn("Exception disposing the ComSafe has been swallowed.");
                _logger.Error(e);
            }

            try
            {
                _logger.Trace("Unregistering AppDomain handlers....");
                AppDomain.CurrentDomain.AssemblyResolve -= LoadFromSameFolder;
                AppDomain.CurrentDomain.UnhandledException -= HandleAppDomainException;
            }
            finally
            {
                _logger.Trace("Rubberduck shutdown completed. Quack!");
                _isInitialized = false;
            }
        }

        private void HandleAppDomainException(object sender, UnhandledExceptionEventArgs e)
        {
            var message = e.IsTerminating
                ? "An unhandled exception occurred. The runtime is shutting down."
                : "An unhandled exception occurred. The runtime continues running.";

            if (e.ExceptionObject is Exception exception)
            {
                _logger.Fatal(exception, message);

            }
            else
            {
                _logger.Fatal(message);
            }
        }

        private Assembly LoadFromSameFolder(object sender, ResolveEventArgs args)
        {
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