using Rubberduck.InternalApi.ServerPlatform;
using System;
using System.Text;

namespace Rubberduck.SettingsProvider.Model
{
    public enum ServerTraceLevel
    {
        Verbose,
        Message,
        Off
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
            Path = @".\",
        };

        LanguageServerSettings IDefaultSettingsProvider<LanguageServerSettings>.Default => LanguageServerSettings.Default;

        public string Path { get; init; }

        public TransportType TransportType { get; init; }
        public string PipeName { get; init; }
        public MessageMode Mode { get; init; }

        public ServerTraceLevel TraceLevel { get; init; }

        public string ToProcessStartInfoArguments(long clientProcessId)
        {
            var builder = new StringBuilder();

            switch (TransportType)
            {
                case TransportType.Pipe:
                    builder.Append($"Pipe --client {clientProcessId} --mode {Mode}");
                    
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

        public override bool Equals(object obj)
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
    }
}
