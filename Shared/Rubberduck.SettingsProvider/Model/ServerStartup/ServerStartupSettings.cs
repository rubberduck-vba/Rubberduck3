using Rubberduck.InternalApi.ServerPlatform;
using Rubberduck.InternalApi.Settings;
using System;
using System.Text;
using System.Text.Json.Serialization;

namespace Rubberduck.SettingsProvider.Model.ServerStartup
{
    /// <summary>
    /// The base type for server startup settings.
    /// </summary>
    public abstract record class ServerStartupSettings : TypedSettingGroup, 
        IProcessStartInfoArgumentProvider, 
        IHealthCheckSettingsProvider
    {
        protected static RubberduckSetting[] GetDefaultSettings(string pipe, string path)
        {
            return new RubberduckSetting[]
            {
                new ServerExecutablePathSetting { Value = new Uri(path) },
                new ServerTransportTypeSetting { Value = TransportType.StdIO },
                new ServerPipeNameSetting { Value = pipe },
                new ServerMessageModeSetting { Value = MessageMode.Message },
                new TraceLevelSetting { Value = MessageTraceLevel.Verbose },
                new ClientHealthCheckIntervalSetting { Value = TimeSpan.FromSeconds(10) },
            };
        }

        protected ServerStartupSettings()
        {
            SettingDataType = SettingDataType.SettingGroup;
            Tags = SettingTags.ReadOnlyRecommended;
        }

        [JsonIgnore]
        public string ServerExecutablePath => GetSetting<ServerExecutablePathSetting>().TypedValue.AbsolutePath;
        [JsonIgnore]
        public TransportType ServerTransportType => GetSetting<ServerTransportTypeSetting>().TypedValue;

        [JsonIgnore]
        public string ServerPipeName => GetSetting<ServerPipeNameSetting>().TypedValue;
        [JsonIgnore]
        public MessageMode ServerMessageMode => GetSetting<ServerMessageModeSetting>().TypedValue;

        [JsonIgnore]
        public MessageTraceLevel ServerTraceLevel => GetSetting<TraceLevelSetting>().TypedValue;

        [JsonIgnore]
        public TimeSpan ClientHealthCheckInterval => GetSetting<ClientHealthCheckIntervalSetting>().TypedValue;

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
