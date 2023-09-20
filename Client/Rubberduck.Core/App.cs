using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Abstractions;
using System.Linq;
using Rubberduck.Common;
using Rubberduck.Interaction.MessageBox;
using Rubberduck.InternalApi.UIContext;
using Rubberduck.Resources;
using Rubberduck.Settings;
using Rubberduck.SettingsProvider;
using Rubberduck.VersionCheck;
using Application = System.Windows.Forms.Application;
using System.Windows.Input;
using Infralution.Localization.Wpf;
using Rubberduck.VBEditor.UI.OfficeMenus;
using Rubberduck.UI.Command;
using System.Threading.Tasks;
using Rubberduck.InternalApi.Extensions;

namespace Rubberduck.Core
{
    public sealed class App : IDisposable
    {
        private static readonly string _title = "Rubberduck";

        private readonly IMessageBox _messageBox;
        private readonly IConfigurationService<Configuration> _configService;
        private readonly IAppMenu _appMenus;

        // TODO move version check to update server
        private readonly IVersionCheckService _version;
        private readonly ICommand _checkVersionCommand;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private Configuration _config;
        private readonly IFileSystem _filesystem;

        public App(IMessageBox messageBox,
            IConfigurationService<Configuration> configService,
            IAppMenu appMenus,
            IVersionCheckService version,
            VersionCheckCommand checkVersionCommand,
            IFileSystem filesystem)
        {
            _messageBox = messageBox;
            _configService = configService;
            _appMenus = appMenus;
            _version = version;
            _checkVersionCommand = checkVersionCommand;

            _configService.SettingsChanged += HandleConfigServiceSettingsChanged;
            _filesystem = filesystem;

            UiContextProvider.Initialize();
        }

        private async void HandleConfigServiceSettingsChanged(object sender, ConfigurationChangedEventArgs e)
        {
            _config = await _configService.ReadAsync();
            UpdateLoggingLevel();

            if (e.DisplayLanguageChanged)
            {
                await ApplyCultureConfigAsync();
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
                    file.TryDelete();
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
            LogLevelHelper.SetMinimumLogLevel(LogLevel.FromOrdinal(_config.UserSettings.GeneralSettings.MinimumLogLevel));
        }

        /// <summary>
        /// Ensure that log level is changed to "none" after a successful
        /// run of Rubberduck for first time. By default, we ship with 
        /// log level set to Trace (0) but once it's installed and has
        /// ran without problem, it should be set to None (6)
        /// </summary>
        private async Task UpdateLoggingLevelOnShutdownAsync()
        {
            if (_config.UserSettings.GeneralSettings.UserEditedLogLevel ||
                _config.UserSettings.GeneralSettings.MinimumLogLevel != LogLevel.Trace.Ordinal)
            {
                return;
            }

            _config.UserSettings.GeneralSettings.MinimumLogLevel = LogLevel.Off.Ordinal;
            await _configService.SaveAsync(_config);
        }

        public async Task StartupAsync()
        {
            EnsureLogFolderPathExists();
            EnsureTempPathExists();
            await ApplyCultureConfigAsync();

            LogRubberduckStart();
            UpdateLoggingLevel();

            await CheckForLegacyIndenterSettingsAsync();
            _appMenus.Initialize();
            _appMenus.Localize();


            if (_config.UserSettings.GeneralSettings.CanCheckVersion)
            {
                _checkVersionCommand.Execute(null);
            }            
        }

        public async Task ShutdownAsync()
        {
            try
            {
                await UpdateLoggingLevelOnShutdownAsync();
            }
            catch
            {
                // Won't matter anymore since we're shutting everything down anyway.
            }
        }

        private async Task ApplyCultureConfigAsync()
        {
            _config = await _configService.ReadAsync();

            var currentCulture = Resources.RubberduckUI.Culture;
            try
            {
                CultureManager.UICulture = CultureInfo.GetCultureInfo(_config.UserSettings.GeneralSettings.Language.Code);
                LocalizeResources(CultureManager.UICulture);

                _appMenus.Localize();
            }
            catch (CultureNotFoundException exception)
            {
                Logger.Error(exception, "Error Setting Culture for Rubberduck");
                // not accessing resources here, because setting resource culture literally just failed.
                _messageBox.NotifyWarn(exception.Message, "Rubberduck");
                _config.UserSettings.GeneralSettings.Language.Code = currentCulture.Name;
                await _configService.SaveAsync(_config);
            }
        }

        private static void LocalizeResources(CultureInfo culture)
        {
            var localizers = AppDomain.CurrentDomain.GetAssemblies()
                .SingleOrDefault(assembly => assembly.GetName().Name == "Rubberduck.Resources")
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
                localizer.SetMethod.Invoke(null, args);
            }
        }

        private async Task CheckForLegacyIndenterSettingsAsync()
        {
            try
            {
                Logger.Trace("Checking for legacy Smart Indenter settings.");
                if (_config.UserSettings.GeneralSettings.IsSmartIndenterPrompted /*||
                    !_config.UserSettings.IndenterSettings.LegacySettingsExist()*/)
                {
                    return;
                }
                if (_messageBox.Question(Resources.RubberduckUI.SmartIndenter_LegacySettingPrompt, "Rubberduck"))
                {
                    Logger.Trace("Attempting to load legacy Smart Indenter settings.");
                    //_config.UserSettings.IndenterSettings.LoadLegacyFromRegistry();
                }
                _config.UserSettings.GeneralSettings.IsSmartIndenterPrompted = true;
                await _configService.SaveAsync(_config);
            }
            catch (Exception e)
            {
                _messageBox.NotifyError("The operation failed.", "Rubberduck", e.ToString());
            }
        }

        public void LogRubberduckStart()
        {
            var version = _version.CurrentVersion;
            GlobalDiagnosticsContext.Set("RubberduckVersion", version.ToString());

            var headers = new List<string>
            {
                $"\r\n\tRubberduck version {version} loading:",
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

            LogLevelHelper.SetDebugInfo(string.Join(Environment.NewLine, headers));
        }

        private bool _disposed;
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            if (_configService != null)
            {
                _configService.SettingsChanged -= HandleConfigServiceSettingsChanged;
            }

            UiDispatcher.Shutdown();

            _disposed = true;
        }
    }
}
