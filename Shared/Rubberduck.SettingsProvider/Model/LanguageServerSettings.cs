using Rubberduck.InternalApi.ServerPlatform;
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

    public enum ServerTraceLevel
    {
        Off = TraceLevel.Off,
        Verbose = TraceLevel.Verbose,
        Message = TraceLevel.Info,
    }

    public enum MessageMode
    {
        Message,
        Byte
    }

    public interface IProcessStartInfoArgumentProvider
    {
        string Path { get; }
        string ToProcessStartInfoArguments(long clientProcessId);
    }

    public readonly struct LanguageServerSettings : IProcessStartInfoArgumentProvider, IDefaultSettingsProvider<LanguageServerSettings>, IEquatable<LanguageServerSettings>
    {
        public static LanguageServerSettings Default { get; } = new LanguageServerSettings
        {
            TransportType = TransportType.StdIO,
            TraceLevel = ServerTraceLevel.Verbose,
            Mode = MessageMode.Message,
            PipeName = "Rubberduck.LanguageServer.Pipe",
#if DEBUG
            Path = @"",
#else
            Path = @"",
#endif
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
            builder.Append($" --client {clientProcessId}");

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
                    builder.Append("StdIO");
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

            return builder.ToString();
        }

        public bool Equals(LanguageServerSettings other)
        {
            return other.TraceLevel == TraceLevel
                && other.TransportType == TransportType
                && other.Mode == Mode
                && string.Equals(other.PipeName, PipeName, StringComparison.InvariantCultureIgnoreCase)
                && string.Equals(other.Path, Path, StringComparison.InvariantCultureIgnoreCase);
        }

        public override bool Equals(object? obj)
        {
            if (obj is null || obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((LanguageServerSettings)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(TraceLevel, TransportType, Mode, PipeName.ToLowerInvariant(), Path.ToLowerInvariant());
        }

        public static bool operator ==(LanguageServerSettings left, LanguageServerSettings right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(LanguageServerSettings left, LanguageServerSettings right)
        {
            return !(left == right);
        }
    }
}
