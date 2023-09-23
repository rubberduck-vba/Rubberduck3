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
using Rubberduck.SettingsProvider.Model;
using Rubberduck.Core.Settings;
using Microsoft.Extensions.Logging;

namespace Rubberduck.Core
{
    public sealed class App : IDisposable
    {
        private static readonly string _title = "Rubberduck";

        private readonly IMessageBox _messageBox;
        private readonly ISettingsService<RubberduckSettings> _settingsService;
        private readonly IAppMenu _appMenus;

        private readonly ILogger<App> _logger;
        private readonly IFileSystem _filesystem;

        public App(ILogger<App> logger,
            IMessageBox messageBox,
            ISettingsService<RubberduckSettings> settingsService,
            IAppMenu appMenus,
            IFileSystem filesystem)
        {
            _logger = logger;
            _messageBox = messageBox;
            _settingsService = settingsService;
            _appMenus = appMenus;

            _settingsService.SettingsChanged += HandleSettingsServiceSettingsChanged;
            _filesystem = filesystem;

            UiContextProvider.Initialize();
        }

#pragma warning disable VSTHRD100 // Avoid async void methods
        private async void HandleSettingsServiceSettingsChanged(object sender, SettingsChangedEventArgs<RubberduckSettings> e)
        {
            try
            {
                if (!string.Equals(e.OldValue.Locale, e.NewValue.Locale, StringComparison.InvariantCultureIgnoreCase))
                {
                    await ApplyCultureConfigAsync();
                }
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error handling SettingsChanged event.");
            }
        }
#pragma warning restore VSTHRD100 // Avoid async void methods

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
            //LogLevelHelper.SetMinimumLogLevel(LogLevel.FromOrdinal(_config.UserSettings.GeneralSettings.MinimumLogLevel));
        }

        /// <summary>
        /// Ensure that log level is changed to "none" after a successful
        /// run of Rubberduck for first time. By default, we ship with 
        /// log level set to Trace (0) but once it's installed and has
        /// ran without problem, it should be set to None (6)
        /// </summary>
        private async Task UpdateLoggingLevelOnShutdownAsync()
        {
            if (_settingsService.Value.IsDefaultLogLevelChanged || _settingsService.Value.LogLevel != LogLevel.Trace)
            {
                return;
            }

            var vm = new RubberduckSettingsViewModel(_settingsService.Value);
            vm.LogLevel = LogLevel.None;
            await _settingsService.WriteToFileAsync(vm.ToSettings());
        }

        public async Task StartupAsync(string version)
        {
            EnsureLogFolderPathExists();
            EnsureTempPathExists();
            await ApplyCultureConfigAsync();

            LogRubberduckStart(version);
            UpdateLoggingLevel();

            await CheckForLegacyIndenterSettingsAsync();
            _appMenus.Initialize();
            _appMenus.Localize();
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
            var currentCulture = Resources.RubberduckUI.Culture;
            try
            {
                CultureManager.UICulture = CultureInfo.GetCultureInfo(_settingsService.Value.Locale);
                LocalizeResources(CultureManager.UICulture);

                _appMenus.Localize();
            }
            catch (CultureNotFoundException exception)
            {
                _logger.LogError(exception, "Error Setting Culture for Rubberduck");
                // not accessing resources here, because setting resource culture literally just failed.
                _messageBox.NotifyWarn(exception.Message, "Rubberduck");

                var vm = new RubberduckSettingsViewModel(_settingsService.Value);
                vm.Locale = currentCulture.Name;
                await _settingsService.WriteToFileAsync(vm.ToSettings());
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
                _logger.LogTrace("Checking for legacy Smart Indenter settings.");
                if (_settingsService.Value.IsSmartIndenterPrompted /*||
                    !_config.UserSettings.IndenterSettings.LegacySettingsExist()*/)
                {
                    return;
                }
                if (_messageBox.Question(Resources.RubberduckUI.SmartIndenter_LegacySettingPrompt, "Rubberduck"))
                {
                    _logger.LogTrace("Attempting to load legacy Smart Indenter settings.");
                    //_config.UserSettings.IndenterSettings.LoadLegacyFromRegistry();
                }

                var vm = new RubberduckSettingsViewModel(_settingsService.Value);
                vm.IsSmartIndenterPrompted = true;
                await _settingsService.WriteToFileAsync(vm.ToSettings());
            }
            catch (Exception e)
            {
                _messageBox.NotifyError("The operation failed.", "Rubberduck", e.ToString());
            }
        }

        public void LogRubberduckStart(string version)
        {
            //GlobalDiagnosticsContext.Set("RubberduckVersion", version.ToString());

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

            _logger.LogInformation(string.Join(Environment.NewLine, headers));
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
