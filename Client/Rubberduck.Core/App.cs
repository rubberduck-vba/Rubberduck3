using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Abstractions;
using System.Linq;
using Rubberduck.Interaction.MessageBox;
using Rubberduck.Resources;
using Rubberduck.SettingsProvider;
using Application = System.Windows.Forms.Application;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.Core.Settings;
using Microsoft.Extensions.Logging;
using Rubberduck.Unmanaged.UIContext;
using Rubberduck.VBEditor.UI.OfficeMenus.RubberduckMenu;
using System.Windows;
using Rubberduck.UI;
using System.Threading.Tasks;
using System.Reactive.Concurrency;

namespace Rubberduck.Core
{
    public sealed class App : IDisposable
    {
        private static readonly string _title = "Rubberduck";
        private readonly Version _version;

        private readonly IPresenter _presenter;

        private readonly IMessageBoxService _messageBox;
        private readonly ISettingsService<RubberduckSettings> _settingsService;
        private readonly IRubberduckMenu _appMenus;

        private readonly ILogger<App> _logger;
        private readonly ILogLevelService _logLevelService;
        private readonly IFileSystem _filesystem;

        public App(Version version,
            ILogger<App> logger, 
            ILogLevelService logLevelService,
            IMessageBoxService messageBox,
            ISettingsService<RubberduckSettings> settingsService,
            IRubberduckMenu appMenu,
            IFileSystem filesystem,
            IPresenter presenter)
        {
            _version = version;

            _logger = logger;
            _logLevelService = logLevelService;

            _messageBox = messageBox;
            _settingsService = settingsService;
            _appMenus = appMenu;

            _settingsService.SettingsChanged += HandleSettingsServiceSettingsChanged;
            _filesystem = filesystem;
            _presenter = presenter;
        }

        private void HandleSettingsServiceSettingsChanged(object? sender, SettingsChangedEventArgs<RubberduckSettings>? e)
        {
            try
            {
                if (e is not null && !string.Equals(e.OldValue.Locale, e.NewValue.Locale, StringComparison.InvariantCultureIgnoreCase))
                {
                    ApplyCultureConfig();
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error handling SettingsChanged event.");
            }
        }

        #region TODO move to something like ShellEnvironment.cs
        private void EnsureLogFolderPathExists()
        {
            try
            {
                if (!_filesystem.Directory.Exists(ApplicationConstants.LOG_FOLDER_PATH))
                {
                    _filesystem.Directory.CreateDirectory(ApplicationConstants.LOG_FOLDER_PATH);
                }
            }
            catch (Exception e)
            {
                _messageBox.NotifyError("", _title, e.ToString());
            }
        }

        private void EnsureTempPathExists()
        {
            // This is required by the parser - allow this to throw. 
            if (!_filesystem.Directory.Exists(ApplicationConstants.RUBBERDUCK_TEMP_PATH))
            {
                _filesystem.Directory.CreateDirectory(ApplicationConstants.RUBBERDUCK_TEMP_PATH);
            }

            // The parser swallows the error if deletions fail - clean up any temp files on startup
            SafeDeleteTempFiles();
        }

        private void SafeDeleteTempFiles()
        {
            try
            {
                var tempFolder = _filesystem.DirectoryInfo.New(ApplicationConstants.RUBBERDUCK_TEMP_PATH);
                foreach (var file in tempFolder.GetFiles())
                {
                    file.Delete();
                }
            }
            catch
            {
                // do not throw
            }
        }
        #endregion

        private void UpdateLoggingLevel()
        {
            _logLevelService.SetMinimumLogLevel(_settingsService.Settings.LogLevel);
        }

        /// <summary>
        /// Ensure that log level is changed to "none" after a successful
        /// run of Rubberduck for first time. By default, we ship with 
        /// log level set to Trace (0) but once it's installed and has
        /// ran without problem, it should be set to None (6)
        /// </summary>
        private void UpdateLoggingLevelOnShutdown()
        {
            var currentSettings = _settingsService.Settings;
            if (currentSettings.IsInitialLogLevelChanged || currentSettings.LogLevel != LogLevel.Trace)
            {
                return;
            }

            var vm = new RubberduckSettingsViewModel(currentSettings)
            {
                LogLevel = LogLevel.None
            };

            _settingsService.Write(vm.ToSettings());
        }

        public void Startup()
        {
            UiContextProvider.Initialize();

            EnsureLogFolderPathExists();
            EnsureTempPathExists();
            ApplyCultureConfig();

            LogRubberduckStart(_version);
            UpdateLoggingLevel();
            //CheckForLegacyIndenterSettings();
            _appMenus.Initialize();
            _appMenus.Localize();

            _presenter.Show();
        }

        public void Shutdown()
        {
            try
            {
                UpdateLoggingLevelOnShutdown();
            }
            catch
            {
                // Won't matter anymore since we're shutting everything down anyway.
            }
        }

        private void ApplyCultureConfig()
        {
            var currentCulture = Resources.RubberduckUI.Culture;
            var currentSettings = _settingsService.Settings;

            try
            {
                var uiCulture = CultureInfo.GetCultureInfo(currentSettings.Locale);
                LocalizeResources(uiCulture);

                _appMenus.Localize();
            }
            catch (CultureNotFoundException exception)
            {
                _logger.LogError(exception, "Error Setting Culture for Rubberduck");
                // not accessing resources here, because setting resource culture literally just failed.
                _messageBox.NotifyWarn(exception.Message, "Rubberduck");

                var vm = new RubberduckSettingsViewModel(currentSettings)
                {
                    Locale = currentCulture.Name
                };

                _settingsService.Write(vm.ToSettings());
            }
        }

        private static void LocalizeResources(CultureInfo culture)
        {
            var localizers = AppDomain.CurrentDomain.GetAssemblies()
                .SingleOrDefault(assembly => assembly.GetName()?.Name == "Rubberduck.Resources")
                ?.DefinedTypes.SelectMany(type => type.DeclaredProperties.Where(prop =>
                    prop.CanWrite && prop.Name.Equals("Culture") && prop.PropertyType == typeof(CultureInfo) &&
                    (prop.SetMethod?.IsStatic ?? false)));

            if (localizers is null)
            {
                return;
            }

            var args = new object[] { culture };
            foreach (var localizer in localizers)
            {
                localizer.SetMethod!.Invoke(null, args);
            }
        }

        private void CheckForLegacyIndenterSettings()
        {
            try
            {
                var currentSettings = _settingsService.Settings;
                _logger.LogTrace("Checking for legacy Smart Indenter settings.");
                if (currentSettings.IsSmartIndenterPrompted /*||
                    !_config.UserSettings.IndenterSettings.LegacySettingsExist()*/)
                {
                    return;
                }
                if (_messageBox.Question(Resources.RubberduckUI.SmartIndenter_LegacySettingPrompt, "Rubberduck"))
                {
                    _logger.LogTrace("Attempting to load legacy Smart Indenter settings.");
                    //_config.UserSettings.IndenterSettings.LoadLegacyFromRegistry();
                }

                var vm = new RubberduckSettingsViewModel(currentSettings)
                {
                    IsSmartIndenterPrompted = true
                };

                _settingsService.Write(vm.ToSettings());
            }
            catch (Exception e)
            {
                _messageBox.NotifyError("The operation failed.", "Rubberduck", e.ToString());
            }
        }

        public void LogRubberduckStart(Version version)
        {
            //GlobalDiagnosticsContext.Set("RubberduckVersion", version.ToString());

            var headers = new List<string>
            {
                $"\r\n\tRubberduck version {version.ToString(3)} loading:",
                $"\tOperating System: {Environment.OSVersion.VersionString} {(Environment.Is64BitOperatingSystem ? "x64" : "x86")}"
            };
            try
            {
                headers.AddRange(new []
                {
                    $"\tHost Product: {Application.ProductName} {(Environment.Is64BitProcess ? "x64" : "x86")}",
                    $"\tHost Version: {Application.ProductVersion}",
                    $"\tHost Executable: {_filesystem.Path.GetFileName(Application.ExecutablePath).ToUpper()}", // .ToUpper() used to convert ExceL.EXE -> EXCEL.EXE
                });
            }
            catch
            {
                headers.Add("\tHost could not be determined.");
            }

            _logger.LogInformation("{message}", string.Join(Environment.NewLine, headers));
        }

        private bool _disposed;
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            if (_settingsService != null)
            {
                _settingsService.SettingsChanged -= HandleSettingsServiceSettingsChanged;
            }

            UiDispatcher.Shutdown();

            _disposed = true;
        }
    }
}
