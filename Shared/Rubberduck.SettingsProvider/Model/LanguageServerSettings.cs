using Rubberduck.InternalApi.ServerPlatform;
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

    public readonly struct LanguageServerSettings : IProcessStartInfoArgumentProvider
    {
        public static LanguageServerSettings Default { get; } = new LanguageServerSettings
        {
            TransportType = TransportType.StdIO,
            TraceLevel = ServerTraceLevel.Verbose,
            Mode = MessageMode.Message,
            PipeName = "Rubberduck.LanguageServer.Pipe",
            Path = @".\",
        };

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
    }
}
