using Rubberduck.InternalApi.ServerPlatform;
using System;
using System.Text;

namespace Rubberduck.SettingsProvider.Model
{
    public readonly struct UpdateServerSettings : 
        IProcessStartInfoArgumentProvider, 
        IHealthCheckSettingsProvider,
        IDefaultSettingsProvider<UpdateServerSettings>, 
        IEquatable<UpdateServerSettings>
    {
        public static UpdateServerSettings Default { get; } = new UpdateServerSettings
        {
            IsEnabled = true,
            IncludePreReleases = true,
            RubberduckWebApiBaseUrl = "https://api.rubberduckvba.com/api/v1",

            TransportType = TransportType.StdIO,
            TraceLevel = ServerTraceLevel.Verbose,
            Mode = MessageMode.Message,
            PipeName = "Rubberduck.UpdateServer.Pipe",
#if DEBUG
            Path = @"",
#else
            Path = @"",
#endif
            ClientHealthCheckInterval = TimeSpan.FromSeconds(10),
        };

        UpdateServerSettings IDefaultSettingsProvider<UpdateServerSettings>.Default => UpdateServerSettings.Default;

        public bool IsEnabled { get; init; }
        public bool IncludePreReleases { get; init; }
        public string RubberduckWebApiBaseUrl { get; init; }

        public string Path {get; init;}
        public TransportType TransportType { get; init; }
        public MessageMode Mode { get; init; }
        public string PipeName { get; init; }

        public ServerTraceLevel TraceLevel { get; init; }

        public TimeSpan ClientHealthCheckInterval { get; init; }

        public string ToProcessStartInfoArguments(long clientProcessId)
        {
            var builder = new StringBuilder();

            switch (TransportType)
            {
                case TransportType.Pipe:
                    builder.Append($" Pipe --mode {Mode}");

                    // NOTE: server will use LanguageServerSettings.Default.PipeName without a --name argument.
                    if (!string.IsNullOrWhiteSpace(PipeName))
                    {
                        // client process ID in the name so different hosts/instances get their own pipe,
                        builder.Append($" --name \"{PipeName}__{clientProcessId}\"");
                    }
                    break;

                case TransportType.StdIO:
                    builder.Append(" StdIO");
                    break;
            }

            switch (TraceLevel)
            {
                case ServerTraceLevel.Verbose:
                    builder.Append(" --verbose");
                    break;

                case ServerTraceLevel.Off:
                    builder.Append(" --silent");
                    break;
            }

            builder.Append($" --client {clientProcessId}");

            return builder.ToString();
        }

        public bool Equals(UpdateServerSettings other)
        {
            return IsEnabled == other.IsEnabled
                && IncludePreReleases == other.IncludePreReleases
                && string.Equals(RubberduckWebApiBaseUrl, other.RubberduckWebApiBaseUrl, StringComparison.InvariantCultureIgnoreCase)
                && string.Equals(Path, other.Path, StringComparison.InvariantCultureIgnoreCase);
        }

        public override bool Equals(object? obj)
        {
            if (obj is null || obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((UpdateServerSettings)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(IsEnabled, IncludePreReleases, RubberduckWebApiBaseUrl.ToLowerInvariant(), Path.ToLowerInvariant());
        }
    }
}
