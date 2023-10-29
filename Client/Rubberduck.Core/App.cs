using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Settings;
using Rubberduck.Resources;
using Rubberduck.SettingsProvider;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.UI;
using Rubberduck.UI.Message;
using Rubberduck.Unmanaged.UIContext;
using Rubberduck.VBEditor.UI.OfficeMenus.RubberduckMenu;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Abstractions;
using System.Linq;
using Application = System.Windows.Forms.Application;

namespace Rubberduck.Core
{
    public sealed class App : IDisposable
    {
        private readonly Version _version;

        private readonly IMessageService _messageBox;
        private readonly ISettingsService<RubberduckSettings> _settingsService;
        private readonly IRubberduckMenu _appMenus;

        private readonly ILogger<App> _logger;
        private readonly ILogLevelService _logLevelService;
        private readonly IFileSystem _filesystem;

        public App(Version version,
            ILogger<App> logger, 
            ILogLevelService logLevelService,
            IMessageService messageBox,
            ISettingsService<RubberduckSettings> settingsService,
            IRubberduckMenu appMenu,
            IFileSystem filesystem)
        {
            _version = version;

            _logger = logger;
            _logLevelService = logLevelService;

            _messageBox = messageBox;
            _settingsService = settingsService;
            _appMenus = appMenu;

            _settingsService.SettingsChanged += HandleSettingsServiceSettingsChanged;
            _filesystem = filesystem;
        }

        private void HandleSettingsServiceSettingsChanged(object? sender, SettingsChangedEventArgs<RubberduckSettings>? e)
        {
            try
            {
                if (e is not null && !string.Equals(e.OldValue.GeneralSettings.Locale, e.NewValue.GeneralSettings.Locale, StringComparison.InvariantCultureIgnoreCase))
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
            catch (Exception exception)
            {
                _logger.LogError(exception, "Could not create log folder."); // TODO disable logging then
                _messageBox.ShowMessage(MessageModel.For(exception));
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
            _logLevelService.SetMinimumLogLevel(_settingsService.Settings.GeneralSettings.LogLevel);
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
            if (currentSettings.GeneralSettings.DisableInitialLogLevelReset || currentSettings.GeneralSettings.LogLevel != LogLevel.Trace)
            {
                return;
            }

            var updatedSettings = new RubberduckSettings(
                new GeneralSettings(currentSettings.GeneralSettings, new IRubberduckSetting[] { new LogLevelSetting(LogLevel.None) }),
                currentSettings.LanguageClientSettings,
                currentSettings.LanguageServerSettings,
                currentSettings.UpdateServerSettings,
                currentSettings.TelemetryServerSettings);

            _settingsService.Write(updatedSettings);
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
            var currentCulture = RubberduckUI.Culture;
            var currentSettings = _settingsService.Settings;

            try
            {
                var uiCulture = CultureInfo.GetCultureInfo(currentSettings.GeneralSettings.Locale);
                LocalizeResources(uiCulture);

                _appMenus.Localize();
            }
            catch (CultureNotFoundException exception)
            {
                _logger.LogError(exception, "Error Setting Culture for Rubberduck");
                // not accessing resources here, because setting resource culture literally just failed.
                _messageBox.ShowMessage(MessageModel.For(exception));

                var updatedSettings = new RubberduckSettings(
                    new GeneralSettings(currentSettings.GeneralSettings, new IRubberduckSetting[] { new LogLevelSetting(LogLevel.None) }),
                    currentSettings.LanguageClientSettings,
                    currentSettings.LanguageServerSettings,
                    currentSettings.UpdateServerSettings,
                    currentSettings.TelemetryServerSettings);

                _settingsService.Write(updatedSettings);
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

        /*
        private void CheckForLegacyIndenterSettings()
        {
            try
            {
                var currentSettings = _settingsService.Settings;
                _logger.LogTrace("Checking for legacy Smart Indenter settings.");
                if (currentSettings.IsSmartIndenterPrompted ||
                    !_config.UserSettings.IndenterSettings.LegacySettingsExist())
                {
                    return;
                }

                var action = _messageBox.ShowMessageRequest(MessageRequestModel.For(LogLevel.Information, RubberduckUI.SmartIndenter_LegacySettingPrompt, new[] { MessageAction.AcceptAction, MessageAction.CancelAction }));
                if (action.MessageAction == MessageAction.AcceptAction)
                {
                    _logger.LogTrace("Attempting to load legacy Smart Indenter settings.");
                    //_config.UserSettings.IndenterSettings.LoadLegacyFromRegistry();
                }

                if (!action.IsEnabled)
                {
                    // TODO disable this message key
                }

                var vm = new RubberduckSettingsViewModel(currentSettings)
                {
                    IsSmartIndenterPrompted = true
                };

                _settingsService.Write(vm.ToSettings());
            }
            catch (Exception exception)
            {
                _messageBox.ShowMessage(MessageModel.For(exception));
            }
        }
        */

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
