using Rubberduck.InternalApi.ServerPlatform;
using Rubberduck.InternalApi.Settings;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rubberduck.SettingsProvider.Model.ServerStartup
{
    public interface IServerStartupSettings
    {
        string ServerExecutablePath { get; }
        TransportType ServerTransportType { get; }
        string ServerPipeName { get; }
        MessageMode ServerMessageMode { get; }

        ServerTraceLevel ServerTraceLevel { get; }

        TimeSpan ClientHealthCheckInterval { get; }
    }

    public abstract record class ServerStartupSettings : SettingGroup, IServerStartupSettings,
        IProcessStartInfoArgumentProvider,
        IHealthCheckSettingsProvider
    {
        protected ServerStartupSettings(string name, string description)
            : base(name, description) 
        {
            Settings = new RubberduckSetting[]
            {
                new ServerExecutablePathSetting($"{name}.{nameof(ServerStartupSettings)}", new Uri(DefaultServerExecutablePath)),
                new ServerTransportTypeSetting($"{name}.{nameof(ServerTransportTypeSetting)}", TransportType.StdIO),
                new ServerPipeNameSetting($"{name}.{nameof(ServerPipeNameSetting)}", string.Empty),
                new ServerMessageModeSetting($"{name}.{nameof(ServerMessageModeSetting)}", MessageMode.Message),
                new ServerTraceLevelSetting($"{name}.{nameof(ServerTraceLevelSetting)}", ServerTraceLevel.Verbose),
                new ClientHealthCheckIntervalSetting($"{name}.{nameof(ClientHealthCheckIntervalSetting)}", TimeSpan.FromSeconds(10)),
            };
        }

        protected abstract string DefaultServerExecutablePath { get; }
        public string ServerExecutablePath => Values[nameof(ServerExecutablePathSetting)];

        public TransportType ServerTransportType => Enum.Parse<TransportType>(Values[nameof(ServerTransportTypeSetting)]);

        protected abstract string DefaultServerPipeName { get; }
        public string ServerPipeName => Values[nameof(ServerPipeNameSetting)];
        public MessageMode ServerMessageMode => Enum.Parse<MessageMode>(Values[nameof(ServerMessageModeSetting)]);

        public ServerTraceLevel ServerTraceLevel => Enum.Parse<ServerTraceLevel>(Values[nameof(ServerTraceLevelSetting)]);

        public TimeSpan ClientHealthCheckInterval => TimeSpan.Parse(Values[nameof(ClientHealthCheckIntervalSetting)]);

        protected override IEnumerable<RubberduckSetting> Settings { get; init; }

        public override string ToString() => ToProcessStartInfoArguments(0);


        ServerTraceLevel IHealthCheckSettingsProvider.ServerTraceLevel => ServerTraceLevel;

        TimeSpan IHealthCheckSettingsProvider.ClientHealthCheckInterval => ClientHealthCheckInterval;


        ServerTraceLevel IProcessStartInfoArgumentProvider.ServerTraceLevel => ServerTraceLevel;

        string IProcessStartInfoArgumentProvider.ServerExecutablePath => ServerExecutablePath;

        private string ToProcessStartInfoArguments(long clientProcessId)
        {
            var builder = new StringBuilder();

            switch (ServerTransportType)
            {
                case TransportType.Pipe:
                    builder.Append($" Pipe --mode {ServerMessageMode}");

                    // NOTE: server will use the default without a --name argument.
                    if (!string.IsNullOrWhiteSpace(ServerPipeName))
                    {
                        // client process ID in the name so different hosts/instances get their own pipe,
                        builder.Append($" --name \"{ServerPipeName}__{clientProcessId}\"");
                    }
                    break;

                case TransportType.StdIO:
                    builder.Append(" StdIO");
                    break;
            }

            switch (ServerTraceLevel)
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

        string IProcessStartInfoArgumentProvider.ToProcessStartInfoArguments(long clientProcessId)
        {
            throw new NotImplementedException();
        }
    }
}
