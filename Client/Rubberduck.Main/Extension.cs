﻿using Castle.Windsor;
using Extensibility;
using NLog;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO.Abstractions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using System.Windows.Forms;
using Rubberduck.Core;
using Rubberduck.Core.Splash;
using Rubberduck.Resources;
using Rubberduck.Resources.Registration;
using Rubberduck.Root;
using Rubberduck.Settings;
using Rubberduck.SettingsProvider;
using Rubberduck.VBEditor.ComManagement;
using Rubberduck.VBEditor.ComManagement.TypeLibs;
using Rubberduck.VBEditor.Events;
using Rubberduck.VBEditor.SafeComWrappers.Abstract;
using Rubberduck.VBEditor.VbeRuntime;
using Rubberduck.VersionCheck;
using Rubberduck.InternalApi.WindowsApi;
using Rubberduck.UI.Abstract;
using Rubberduck.UI.WinForms;
using Microsoft.Extensions.DependencyInjection;

namespace Rubberduck
{
    /// <remarks>
    /// Special thanks to Carlos Quintero (MZ-Tools) for providing the general structure here.
    /// </remarks>
    [
        ComVisible(true),
        Guid(RubberduckGuid.ExtensionGuid),
        ProgId(RubberduckProgId.ExtensionProgId),
        ClassInterface(ClassInterfaceType.None),
        ComDefaultInterface(typeof(IDTExtensibility2)),
        EditorBrowsable(EditorBrowsableState.Never)
    ]
    public class Extension : IDTExtensibility2
    {
        private IVBE _vbe;
        private IAddIn _addin;
        private IFileSystem _fileSystem;
        private bool _isInitialized;
        private bool _isBeginShutdownExecuted;

        private GeneralSettings _initialSettings;

        private IServiceScope _serviceScope;

        private IWindsorContainer _container;
        private App _app;

        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public void OnAddInsUpdate(ref Array custom) { }

        public void OnConnection(object Application, ext_ConnectMode ConnectMode, object AddInInst, ref Array custom)
        {
            try
            {
                _vbe = RootComWrapperFactory.GetVbeWrapper(Application);
                _addin = RootComWrapperFactory.GetAddInWrapper(AddInInst);
                _addin.Object = this;
                SetAddInObject();

                switch (ConnectMode)
                {
                    case ext_ConnectMode.ext_cm_Startup:
                        // normal execution path - don't initialize just yet, wait for OnStartupComplete to be called by the host.
                        break;
                    case ext_ConnectMode.ext_cm_AfterStartup:
                        _isBeginShutdownExecuted = false;   //When we reconnect after having been unloaded, the variable might no longer have its initial value.
                        InitializeAddIn();
                        break;
                }
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }
        }

        [Conditional("DEBUG")]
        private void SetAddInObject()
        {
            // FOR DEBUGGING/DEVELOPMENT PURPOSES, ALLOW ACCESS TO SOME VBETypeLibsAPI FEATURES FROM VBA
            _addin.Object = new VBETypeLibsAPI_Object(_vbe);
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

        public void OnStartupComplete(ref Array custom)
        {
            InitializeAddIn();
        }

        public void OnBeginShutdown(ref Array custom)
        {
            _isBeginShutdownExecuted = true;
            ShutdownAddIn();
        }

        public void OnDisconnection(ext_DisconnectMode RemoveMode, ref Array custom)
        {
            switch (RemoveMode)
            {
                case ext_DisconnectMode.ext_dm_UserClosed:
                    ShutdownAddIn();
                    break;

                case ext_DisconnectMode.ext_dm_HostShutdown:
                    if (_isBeginShutdownExecuted)
                    {
                        // this is the normal case: nothing to do here, we already ran ShutdownAddIn.
                    }
                    else
                    {
                        // some hosts do not call OnBeginShutdown: this mitigates it.
                        ShutdownAddIn();
                    }
                    break;
            }
        }

        private void InitializeAddIn()
        {
            Splash splash = null;
            ISplashViewModel splashModel = null;
            try
            {
                if (_isInitialized)
                {
                    // The add-in is already initialized. See:
                    // The strange case of the add-in initialized twice
                    // http://msmvps.com/blogs/carlosq/archive/2013/02/14/the-strange-case-of-the-add-in-initialized-twice.aspx
                    return;
                }

                var services = new RubberduckServicesBuilder()
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
                    .WithLanguageServer()
                    .Build();
                _serviceScope = services.CreateScope();

                InitializeSettings();

                if (_initialSettings?.CanShowSplash ?? false)
                {
                    var versionCheckService = _serviceScope.ServiceProvider.GetRequiredService<IVersionCheckService>();
                    splashModel = new SplashViewModel(versionCheckService);

                    splash = ShowSplash(splashModel);
                }

                Startup(splashModel);
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

        private void InitializeSettings()
        {
            var configProvider = _serviceScope.ServiceProvider.GetService<GeneralConfigProvider>();

            _initialSettings = configProvider.Read();
            if (_initialSettings != null)
            {
                try
                {
                    var cultureInfo = CultureInfo.GetCultureInfo(_initialSettings.Language.Code);
                    CultureInfo.CurrentUICulture = cultureInfo;
                    Dispatcher.CurrentDispatcher.Thread.CurrentUICulture = cultureInfo;
                }
                catch (CultureNotFoundException)
                {
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

        private void Startup(ISplashViewModel model)
        {
            try
            {
                var currentDomain = AppDomain.CurrentDomain;
                currentDomain.UnhandledException += HandleAppDomainException;
                currentDomain.AssemblyResolve += LoadFromSameFolder;

                model?.UpdateStatus("Initializing IoC container...");

                //_container = new WindsorContainer().Install(new RubberduckIoCInstaller(_vbe, _addin, _initialSettings, _vbeNativeApi, _beepInterceptor));

                model?.UpdateStatus("Resolving dependencies...");
                _app = _serviceScope.ServiceProvider.GetRequiredService<App>(); //_container.Resolve<App>();

                model?.UpdateStatus("Starting add-in...");
                _app.Startup();

                model?.UpdateStatus("Starting language server...");
                

                model?.UpdateStatus("Rubberduck is ready. Are you?");
                _isInitialized = true;
            }
            catch (Exception e)
            {
                _logger.Fatal(e, "Startup sequence threw an unexpected exception.");
                throw new Exception("Rubberduck's startup sequence threw an unexpected exception. Please check the Rubberduck logs for more information and report an issue if necessary", e);
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

        private void ShutdownAddIn()
        {
            _logger.Info("Rubberduck is shutting down.");

            var currentDomain = AppDomain.CurrentDomain;
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
                    _app.Shutdown();
                    _app = null;
                }
            }
            catch (Exception e)
            {
                _logger.Error(e);
            }

            try
            {
                if (_container != null)
                {
                    _logger.Trace("Disposing IoC container...");
                    _serviceScope.Dispose();
                    _serviceScope = null;
                    //_container.Dispose();
                    //_container = null;
                }
            }
            catch (Exception e)
            {
                _logger.Error(e);
                _logger.Warn("Exception is swallowed.");
                //throw; // <<~ uncomment to crash the process
            }
            finally
            {
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
                finally
                {
                    _logger.Trace("Unregistering AppDomain handlers....");
                    currentDomain.AssemblyResolve -= LoadFromSameFolder;
                    currentDomain.UnhandledException -= HandleAppDomainException;
                    _logger.Trace("Done. Main Shutdown completed. Toolwindows follow. Quack!");
                    _isInitialized = false;
                }
            }
        }
    }
}
