using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Services;
using Rubberduck.InternalApi.Settings;
using Rubberduck.Resources;
using Rubberduck.ServerPlatform;
using System;
using System.IO.Abstractions;
using System.Windows.Forms;
using Env = System.Environment;
using ServerPlatformServiceBase = Rubberduck.ServerPlatform.ServerPlatformServiceBase;

namespace Rubberduck.Environment
{
    public readonly record struct HostInfo
    {
        public Version RubberduckVersion { get; init; }
        public string OperatingSystem { get; init; }
        public string HostApplication { get; init; }
        public string HostVersion { get; init; }
        public string HostExecutable { get; init; }
    }

    public interface IHostInfoService
    {
        HostInfo GetHostInfo(Application application);
    }

    public class HostInfoService : ServerPlatformServiceBase, IHostInfoService
    {
        private readonly Version _rubberduckVersion;

        public HostInfoService(ILogger<HostInfoService> logger, 
            RubberduckSettingsProvider settings, 
            IWorkDoneProgressStateService workdone, 
            Version rubberduckVersion, PerformanceRecordAggregator performance)
            : base(logger, settings, workdone, performance)
        {
            _rubberduckVersion = rubberduckVersion;
        }

        public HostInfo GetHostInfo(Application application)
        {
            static string ToBitnessString(bool x64) => x64 ? "x64" : "x86";

            var osVersion = Env.OSVersion.VersionString;
            var osBitness = ToBitnessString(Env.Is64BitOperatingSystem);

            var result = new HostInfo
            {
                RubberduckVersion = _rubberduckVersion,
                OperatingSystem = $"{osVersion} ({osBitness})",
            };

            TryRunAction(() =>
            {
                try
                {
                    result = result with
                    {
                        HostApplication = $"{Application.ProductName} ({ToBitnessString(Env.Is64BitProcess)})",
                        HostVersion = Application.ProductVersion,
                        HostExecutable = Application.ProductName?.ToUpper() ?? string.Empty,
                    };
                }
                catch (Exception exception)
                {
                    LogException(exception, "Could not retrieve host application info.");
                }
            });

            return result;
        }
    }

    public interface IRubberduckFoldersService
    {
        void EnsureRubberduckRootPathExists();
        void EnsureLogFolderPathExists();
        void EnsureTemplatesFolderPathExists();
        void EnsureDefaultWorkspacePathExists();
    }

    public class RubberduckFoldersService : ServerPlatformServiceBase, IRubberduckFoldersService
    {
        private readonly IFileSystem _fileSystem;

        public RubberduckFoldersService(ILogger<RubberduckFoldersService> logger, RubberduckSettingsProvider settings, IWorkDoneProgressStateService workdone, IFileSystem fileSystem, PerformanceRecordAggregator performance)
            : base(logger, settings, workdone, performance)
        {
            _fileSystem = fileSystem;
        }

        public void EnsureRubberduckRootPathExists() => TryRunAction(() =>
        {
            if (!_fileSystem.Directory.Exists(ApplicationConstants.RUBBERDUCK_FOLDER_PATH))
            {
                _fileSystem.Directory.CreateDirectory(ApplicationConstants.RUBBERDUCK_FOLDER_PATH);
                LogInformation("Created Rubberduck root folder.", ApplicationConstants.RUBBERDUCK_FOLDER_PATH);
            }
        });

        public void EnsureLogFolderPathExists() => TryRunAction(() =>
        {
            if (!_fileSystem.Directory.Exists(ApplicationConstants.LOG_FOLDER_PATH))
            {
                _fileSystem.Directory.CreateDirectory(ApplicationConstants.LOG_FOLDER_PATH);
                LogInformation("Created Rubberduck logs folder.", ApplicationConstants.LOG_FOLDER_PATH);
            }
        });

        public void EnsureTemplatesFolderPathExists() => TryRunAction(() =>
        {
            if (!_fileSystem.Directory.Exists(ApplicationConstants.TEMPLATES_FOLDER_PATH))
            {
                _fileSystem.Directory.CreateDirectory(ApplicationConstants.TEMPLATES_FOLDER_PATH);
                LogInformation("Created Rubberduck project templates folder.", ApplicationConstants.TEMPLATES_FOLDER_PATH);
            }
        });

        public void EnsureDefaultWorkspacePathExists() => TryRunAction(() =>
        {
            var defaultWorkspaceRoot = Settings.LanguageClientSettings.WorkspaceSettings.DefaultWorkspaceRoot;
            if (!_fileSystem.Directory.Exists(defaultWorkspaceRoot.LocalPath))
            {
                _fileSystem.Directory.CreateDirectory(defaultWorkspaceRoot.LocalPath);
                LogInformation("Created Rubberduck default workspace folder.", defaultWorkspaceRoot.LocalPath);
            }
        });
    }
}
