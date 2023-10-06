using Rubberduck.Core.WebApi;
using Rubberduck.Core.WebApi.Model;
using Rubberduck.Settings;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.SettingsProvider;

namespace Rubberduck.VersionCheck
{
    public class VersionCheckService : IVersionCheckService
    {
        private readonly ISettingsProvider<UpdateServerSettings> _config;
        private readonly IPublicApiClient _api;

        /// <param name="version">That would be the version of the assembly for the <c>_Extension</c> class.</param>
        public VersionCheckService(ISettingsProvider<UpdateServerSettings> settingsProvider, IPublicApiClient api, Version version)
        {
            _config = settingsProvider;
            _api = api;

            CurrentVersion = version;
#if DEBUG
            IsDebugBuild = true;
#endif
            VersionString = IsDebugBuild
                ? $"{version.Major}.{version.Minor}.{version.Build}.x (debug)"
                : version.ToString();
        }

        private Tag? _latestTag;

        public async Task<Version?> GetLatestVersionAsync(CancellationToken token = default)
        {
            if (_latestTag != default) 
            { 
                return _latestTag.Version; 
            }

            try
            {
                var latestTags = await _api.GetLatestTagsAsync();
                var settings = _config.Settings;

                _latestTag = latestTags
                    .Where(tag => tag != null 
                        && (!tag.IsPreRelease || settings.IncludePreReleases))
                    .OrderByDescending(tag => tag.Version)
                    .FirstOrDefault();

                return _latestTag?.Version;
            }
            catch
            {
                return CurrentVersion;
            }
        }

        public Version CurrentVersion { get; }
        public bool IsDebugBuild { get; }
        public string VersionString { get; }
    }
}