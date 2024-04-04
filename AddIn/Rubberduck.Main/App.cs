using Microsoft.Extensions.Logging;
using Rubberduck.Environment;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Settings;
using Rubberduck.InternalApi.Settings.Model;
using Rubberduck.InternalApi.Settings.Model.Logging;
using Rubberduck.Resources;
using Rubberduck.UI.Shared.Message;
using Rubberduck.Unmanaged.UIContext;
using Rubberduck.VBEditor.UI.OfficeMenus.RubberduckMenu;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Abstractions;
using System.Linq;
using Application = System.Windows.Forms.Application;
using Env = System.Environment;

namespace Rubberduck
{
    public sealed class App : IDisposable
    {
        private readonly Version _version;
        private readonly IRubberduckFoldersService _foldersService;

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
            IFileSystem filesystem,
            IRubberduckFoldersService folders)
        {
            _version = version;
            _foldersService = folders;

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
                _logger.LogError(exception, "Unexpected error while handling SettingsChanged event.");
            }
        }

        private void UpdateLoggingLevel()
        {
            _logLevelService.SetMinimumLogLevel(_settingsService.Settings.LoggerSettings.LogLevel);
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
            if (currentSettings.LoggerSettings.DisableInitialLogLevelReset || currentSettings.LoggerSettings.LogLevel != LogLevel.Trace)
            {
                return;
            }

            currentSettings.LoggerSettings.GetSetting<LogLevelSetting>().WithValue(LogLevel.None);
            currentSettings.LoggerSettings.GetSetting<DisableInitialLogLevelResetSetting>().Value = true;

            _settingsService.Write(currentSettings);
        }

        public void Startup()
        {
            UiContextProvider.Initialize();
            CheckRubberduckFolders();
            ApplyCultureConfig();

            LogRubberduckStart(_version);
            UpdateLoggingLevel();

            _appMenus.Initialize();
            _appMenus.Localize();
        }

        private void CheckRubberduckFolders()
        {
            _foldersService.EnsureRubberduckRootPathExists();
            _foldersService.EnsureLogFolderPathExists();
            _foldersService.EnsureDefaultWorkspacePathExists();
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

                currentSettings.LoggerSettings.GetSetting<LogLevelSetting>().Value = LogLevel.None;
                _settingsService.Write(currentSettings);
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
                $"\tOperating System: {Env.OSVersion.VersionString} {(Env.Is64BitOperatingSystem ? "x64" : "x86")}"
            };
            try
            {
                headers.AddRange(new []
                {
                    $"\tHost Product: {Application.ProductName} {(Env.Is64BitProcess ? "x64" : "x86")}",
                    $"\tHost Version: {Application.ProductVersion}",
                    $"\tHost Executable: {_filesystem.Path.GetFileName(Application.ExecutablePath).ToUpper()}", // .ToUpper() used to convert ExceL.EXE -> EXCEL.EXE
                });
            }
            catch
            {
                headers.Add("\tHost could not be determined.");
            }

            _logger.LogInformation("{message}", string.Join(Env.NewLine, headers));
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
