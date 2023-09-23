using System;

namespace Rubberduck.SettingsProvider.Model
{
    public readonly struct UpdateServerSettings : IProcessStartInfoArgumentProvider, IDefaultSettingsProvider<UpdateServerSettings>, IEquatable<UpdateServerSettings>
    {
        public static UpdateServerSettings Default { get; } = new UpdateServerSettings
        {
            CheckVersionOnStartup = true,
            IncludePreReleases = true,
            RubberduckWebApiBaseUrl = "https://api.rubberduckvba.com/api/v1",
            Path = @".\",
        };

        UpdateServerSettings IDefaultSettingsProvider<UpdateServerSettings>.Default => UpdateServerSettings.Default;

        public bool CheckVersionOnStartup { get; init; }
        public bool IncludePreReleases { get; init; }
        public string RubberduckWebApiBaseUrl { get; init; }

        public string Path {get; init;}

        public string ToProcessStartInfoArguments(long clientProcessId)
        {
            throw new NotImplementedException();
        }

        public bool Equals(UpdateServerSettings other)
        {
            return CheckVersionOnStartup == other.CheckVersionOnStartup
                && IncludePreReleases == other.IncludePreReleases
                && string.Equals(RubberduckWebApiBaseUrl, other.RubberduckWebApiBaseUrl, StringComparison.InvariantCultureIgnoreCase)
                && string.Equals(Path, other.Path, StringComparison.InvariantCultureIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            if (obj is null || obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((UpdateServerSettings)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(CheckVersionOnStartup, IncludePreReleases, RubberduckWebApiBaseUrl.ToLowerInvariant(), Path.ToLowerInvariant());
        }
    }
}
