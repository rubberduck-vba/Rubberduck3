using Rubberduck.InternalApi.ServerPlatform;
using Rubberduck.InternalApi.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Rubberduck.SettingsProvider.Model
{
    public static class TraceLevelExtensions
    {
        private static readonly IDictionary<ServerTraceLevel, TraceLevel> _map = new Dictionary<ServerTraceLevel, TraceLevel>
        {
            [ServerTraceLevel.Off] = TraceLevel.Off,
            [ServerTraceLevel.Verbose] = TraceLevel.Verbose,
            [ServerTraceLevel.Message] = TraceLevel.Info,
        };

        public static TraceLevel ToTraceLevel(this ServerTraceLevel value) => _map[value];
    }

    public readonly record struct LanguageClientSettings : IDefaultSettingsProvider<LanguageClientSettings>
    {
        public static LanguageClientSettings Default { get; } = new();

        public string[] DisabledMessageKeys { get; init; }

        LanguageClientSettings IDefaultSettingsProvider<LanguageClientSettings>.Default => LanguageClientSettings.Default;
    }

    public readonly record struct LanguageServerSettings :
        IDefaultSettingsProvider<LanguageServerSettings>,
        IHealthCheckSettingsProvider,
        IProcessStartInfoArgumentProvider
    {
        public static LanguageServerSettings Default { get; } = new()
        {
            Path = "",
            TransportType = TransportType.StdIO,
            Mode = MessageMode.Message,
            PipeName = ServerPlatformSettings.LanguageServerDefaultPipeName,

            TraceLevel = ServerTraceLevel.Verbose,
            ClientHealthCheckInterval = TimeSpan.FromSeconds(10),
        };

        LanguageServerSettings IDefaultSettingsProvider<LanguageServerSettings>.Default => LanguageServerSettings.Default;

        public string Path { get; init; }
        public TransportType TransportType { get; init; }
        public string PipeName { get; init; }
        public MessageMode Mode { get; init; }

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
    }
}
