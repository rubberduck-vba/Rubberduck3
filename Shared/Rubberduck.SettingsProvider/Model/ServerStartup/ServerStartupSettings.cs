using Rubberduck.InternalApi.ServerPlatform;
using Rubberduck.InternalApi.Settings;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rubberduck.SettingsProvider.Model.ServerStartup
{
    public abstract record class ServerStartupSettings : SettingGroup, 
        IProcessStartInfoArgumentProvider, 
        IHealthCheckSettingsProvider
    {
        protected static IRubberduckSetting[] GetDefaultSettings(string name, string pipe, string path)
        {
            return new IRubberduckSetting[]
            {
                new ServerExecutablePathSetting($"{name}.{nameof(ServerExecutablePathSetting)}", new Uri(path)),
                new ServerTransportTypeSetting($"{name}.{nameof(ServerTransportTypeSetting)}", TransportType.StdIO),
                new ServerPipeNameSetting($"{name}.{nameof(ServerPipeNameSetting)}", pipe),
                new ServerMessageModeSetting($"{name}.{nameof(ServerMessageModeSetting)}", MessageMode.Message),
                new TraceLevelSetting($"{name}.{nameof(TraceLevelSetting)}", MessageTraceLevel.Verbose),
                new ClientHealthCheckIntervalSetting($"{name}.{nameof(ClientHealthCheckIntervalSetting)}", TimeSpan.FromSeconds(10)),
            };
        }

        protected ServerStartupSettings(string name, IEnumerable<IRubberduckSetting> settings, IEnumerable<IRubberduckSetting> defaults)
            : base(name, settings, defaults) { }

        public string ServerExecutablePath => GetSetting<ServerExecutablePathSetting>().Value.AbsolutePath;
        public TransportType ServerTransportType => GetSetting<ServerTransportTypeSetting>().Value;

        public string ServerPipeName => GetSetting<ServerPipeNameSetting>().Value;
        public MessageMode ServerMessageMode => GetSetting<ServerMessageModeSetting>().Value;

        public MessageTraceLevel ServerTraceLevel => GetSetting<TraceLevelSetting>().Value;

        public TimeSpan ClientHealthCheckInterval => GetSetting<ClientHealthCheckIntervalSetting>().Value;

        MessageTraceLevel IHealthCheckSettingsProvider.ServerTraceLevel => ServerTraceLevel;
        TimeSpan IHealthCheckSettingsProvider.ClientHealthCheckInterval => ClientHealthCheckInterval;

        MessageTraceLevel IProcessStartInfoArgumentProvider.ServerTraceLevel => ServerTraceLevel;
        string IProcessStartInfoArgumentProvider.ServerExecutablePath => ServerExecutablePath;        
        string IProcessStartInfoArgumentProvider.ToProcessStartInfoArguments(long clientProcessId) => ToProcessStartInfoArguments(clientProcessId);
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
                case MessageTraceLevel.Verbose:
                    builder.Append(" --verbose");
                    break;

                case MessageTraceLevel.Off:
                    builder.Append(" --silent");
                    break;
            }

            builder.Append($" --client {clientProcessId}");

            return builder.ToString();
        }
    }
}
