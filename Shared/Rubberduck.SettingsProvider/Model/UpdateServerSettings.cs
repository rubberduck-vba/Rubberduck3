using System;

namespace Rubberduck.SettingsProvider.Model
{
    public readonly struct UpdateServerSettings : IProcessStartInfoArgumentProvider
    {
        public static UpdateServerSettings Default { get; } = new UpdateServerSettings
        {
            CheckVersionOnStartup = true,
            IncludePreReleases = true,
            RubberduckWebApiBaseUrl = "https://api.rubberduckvba.com/api/v1",
            Path = @".\",
        };

        public bool CheckVersionOnStartup { get; init; }
        public bool IncludePreReleases { get; init; }
        public string RubberduckWebApiBaseUrl { get; init; }

        public string Path {get; init;}

        public string ToProcessStartInfoArguments(long clientProcessId)
        {
            throw new NotImplementedException();
        }
    }
}
