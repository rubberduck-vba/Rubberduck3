using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.ServerPlatform;
using Rubberduck.InternalApi.ServerPlatform.TelemetryServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rubberduck.SettingsProvider.Model
{
    public readonly record struct TelemetryServerSettings :
        IProcessStartInfoArgumentProvider,
        IHealthCheckSettingsProvider,
        IDefaultSettingsProvider<TelemetryServerSettings>
    {
        public static TelemetryServerSettings Default => new()
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

            StreamTransmission = true,
            QueueSize = 20,

            SendEventTelemetry = true,
            SendExceptionTelemetry = true,
            SendMetricTelemetry = true,
            SendTraceTelemetry = true,

            IsEnabled = true,

            EventTelemetryConfig = Enum.GetValues<EventTelemetryName>().Select(e => new EventTelemetrySetting { Id = e, IsEnabled = true }).ToDictionary(e => e.Key),
            ExceptionTelemetryConfig = Enum.GetValues<LogLevel>().Select(e => new ExceptionTelemetrySetting { Id = e, IsEnabled = true }).ToDictionary(e => e.Key),
            MetricTelemetryConfig = Enum.GetValues<MetricTelemetryName>().Select(e => new MetricTelemetrySetting { Id = e, IsEnabled = true }).ToDictionary(e => e.Key),
            TraceTelemetryConfig = Enum.GetValues<LogLevel>().Select(e => new TraceTelemetrySetting { Id = e, IsEnabled = true, Verbose = true }).ToDictionary(e => e.Key),
        };

        public abstract record class TelemetrySetting
        {
            public abstract string Key { get; }
            public bool IsEnabled { get; init; }
        }

        public record class EventTelemetrySetting : TelemetrySetting
        {
            public override string Key => Id.ToString();
            public EventTelemetryName Id { get; init; }
        }

        public record class ExceptionTelemetrySetting : TelemetrySetting
        {
            public override string Key => Id.ToString();
            public LogLevel Id { get; init; }
        }

        public record class MetricTelemetrySetting : TelemetrySetting
        {
            public override string Key => Id.ToString();
            public MetricTelemetryName Id { get; init; }
        }

        public record class TraceTelemetrySetting : TelemetrySetting
        {
            public override string Key => Id.ToString();
            public LogLevel Id { get; init; }
            public bool Verbose { get; init; }
        }

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

        public bool IsEnabled { get; init; }
        public bool StreamTransmission { get; init; }
        public int QueueSize { get; init; }

        public bool SendEventTelemetry { get; init; }
        public bool SendExceptionTelemetry { get; init; }
        public bool SendMetricTelemetry { get; init; }
        public bool SendTraceTelemetry { get; init; }

        public Dictionary<string, EventTelemetrySetting> EventTelemetryConfig { get; init; }
        public Dictionary<string, ExceptionTelemetrySetting> ExceptionTelemetryConfig { get; init; }
        public Dictionary<string, MetricTelemetrySetting> MetricTelemetryConfig { get; init; }
        public Dictionary<string, TraceTelemetrySetting> TraceTelemetryConfig { get; init; }
    }
}
