using Extensibility;
using NLog;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO.Abstractions;
using System.Reflection;
using System.Windows.Threading;
using System.Windows.Forms;
using Rubberduck.Core;
using Rubberduck.Core.Splash;
using Rubberduck.Root;
using Rubberduck.Settings;
using Rubberduck.VBEditor.ComManagement;
using Rubberduck.VBEditor.ComManagement.TypeLibs;
using Rubberduck.VBEditor.Events;
using Rubberduck.VBEditor.SafeComWrappers.Abstract;
using Rubberduck.VersionCheck;
using Rubberduck.InternalApi.WindowsApi;
using Rubberduck.UI.Abstract;
using Rubberduck.UI.WinForms;
using Microsoft.Extensions.DependencyInjection;
using Rubberduck.InternalApi;
using System.IO;
using System.Threading.Tasks;
using System.Threading;

namespace Rubberduck
{
    internal interface IVBIDEAddIn
    {
        /// <summary>
        /// Initializes the add-in. Any subsequent invocation should be no-op.
        /// </summary>
        /// <remarks>
        /// Some hosts connect VBE add-ins in different ways, more or less compliant with how IDTExtensibility2 intended it.
        /// </remarks>
        Task InitializeAsync();

        /// <summary>
        /// Shuts down the add-in. Any subsequent invocation should be no-op.
        /// </summary>
        /// <remarks>
        /// Some hosts disconnect VBE add-ins in different ways, more or less compliant with how IDTExtensibility2 intended it.
        /// </remarks>
        Task ShutdownAsync();
    }

    internal class RubberduckAddIn : IVBIDEAddIn
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private GeneralSettings _initialSettings;

        private IVBE _vbe;
        private IAddIn _addin;

        private IServiceScope _serviceScope;

        private bool _isInitialized;

        private App _app;

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

        public async Task InitializeAsync()
        {
            if (_isInitialized)
            {
                // The add-in is already initialized. See:
                // The strange case of the add-in initialized twice
                // http://msmvps.com/blogs/carlosq/archive/2013/02/14/the-strange-case-of-the-add-in-initialized-twice.aspx
                return;
            }

            Splash splash = null;
            ISplashViewModel splashModel = null;

            try
            {
                var tokenSource = new CancellationTokenSource();

                var builder = new RubberduckServicesBuilder()
                    .WithAddIn(_vbe, _addin)
                    .WithApplication()
                    .WithAssemblyInfo()
                    .WithFileSystem(_vbe)
                    .WithSettingsProvider()
                    .WithNativeServices(_vbe)
                    .WithParser()
                    .WithCommands()
                    .WithMsoCommandBarMenu()
                    .WithVersionCheck()
                    .WithRubberduckEditor()
                    .WithLanguageClient(ServerPlatform.TransportType.Pipe);

                var provider = builder.Build();
                var scope = provider.CreateScope();
                _serviceScope = scope;

                await builder.InitializeLanguageClientAsync(provider, tokenSource.Token);
                await InitializeSettingsAsync(scope);

                if (_initialSettings?.CanShowSplash ?? false)
                {
                    //TODO start update server process instead
                    var versionCheckService = _serviceScope.ServiceProvider.GetRequiredService<IVersionCheckService>();
                    //TODO pass a version string instead
                    splashModel = new SplashViewModel(versionCheckService);

                    splash = ShowSplash(splashModel);
                }

                await StartupAsync(splashModel);
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
            var configProvider = scope.ServiceProvider.GetRequiredService<GeneralConfigProvider>();
            _initialSettings = await configProvider.ReadAsync();

            if (_initialSettings is object)
            {
                try
                {
                    var cultureInfo = CultureInfo.GetCultureInfo(_initialSettings.Language.Code);
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
            else
            {
                Debug.Assert(false, "Settings could not be initialized.");
            }
        }

        private async Task StartupAsync(IStatusUpdate statusViewModel)
        {
            try
            {
                var currentDomain = AppDomain.CurrentDomain;
                currentDomain.UnhandledException += HandleAppDomainException;
                currentDomain.AssemblyResolve += LoadFromSameFolder;

                statusViewModel?.UpdateStatus("Resolving services...");
                _app = _serviceScope.ServiceProvider.GetRequiredService<App>();

                statusViewModel?.UpdateStatus("Starting add-in...");
                await _app.StartupAsync();
                
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
                    await _app.ShutdownAsync();
                    _app = null;
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
                    _serviceScope = null;
                }
            }
            catch (Exception e)
            {
                _logger.Error(e);
                //throw; // <<~ uncomment to crash the process
            }

            try
            {
                _logger.Trace("Disposing COM safe...");
                ComSafeManager.DisposeAndResetComSafe();
                _addin = null;
                _vbe = null;

                _isInitialized = false;
                _logger.Info("No exceptions were thrown.");
            }
            catch (Exception e)
            {
                _logger.Error(e);
                _logger.Warn("Exception disposing the ComSafe has been swallowed.");
                //throw; // <<~ uncomment to crash the process
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
                return null;
            }

            var assembly = Assembly.LoadFile(assemblyPath);
            return assembly;
        }
    }
}