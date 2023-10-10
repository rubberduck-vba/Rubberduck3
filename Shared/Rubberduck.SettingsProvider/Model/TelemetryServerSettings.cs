using Rubberduck.InternalApi.ServerPlatform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rubberduck.SettingsProvider.Model
{
    public readonly struct TelemetryServerSettings :
        IProcessStartInfoArgumentProvider,
        IHealthCheckSettingsProvider,
        IDefaultSettingsProvider<TelemetryServerSettings>,
        IEquatable<TelemetryServerSettings>
    {
        public static TelemetryServerSettings Default => new TelemetryServerSettings
        {
            TransportType = TransportType.StdIO,
            TraceLevel = ServerTraceLevel.Verbose,
            Mode = MessageMode.Message,
            PipeName = "Rubberduck.TelemetryServer.Pipe",
#if DEBUG
            Path = @"",
#else
            Path = @"",
#endif
            ClientHealthCheckInterval = TimeSpan.FromSeconds(10),
            SendEventTelemetry = true,
            SendExceptionTelemetry = true,
            SendMetricTelemetry = true,
            SendTraceTelemetry = true,
            EventTelemetryConfig = Enum.GetValues<EventTelemetryName>().Distinct().Select(e => (key:e.ToString(), value: true)).ToDictionary(e => e.key, e => e.value)
        };

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

        TelemetryServerSettings IDefaultSettingsProvider<TelemetryServerSettings>.Default => TelemetryServerSettings.Default;

        public bool SendEventTelemetry { get; init; }
        public bool SendExceptionTelemetry { get; init; }
        public bool SendMetricTelemetry { get; init; }
        public bool SendTraceTelemetry { get; init; }

        public Dictionary<string, bool> EventTelemetryConfig { get; init; }

        public bool Equals(TelemetryServerSettings other)
        {
            throw new NotImplementedException();
        }
    }
}
