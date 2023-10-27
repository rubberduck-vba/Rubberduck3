using Rubberduck.InternalApi.ServerPlatform;
using Rubberduck.InternalApi.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rubberduck.SettingsProvider.Model.ServerStartup
{
    public interface IServerStartupSettings
    {
        string ServerExecutablePath { get; }
        TransportType ServerTransportType { get; }
        string ServerPipeName { get; }
        MessageMode ServerMessageMode { get; }

        MessageTraceLevel ServerTraceLevel { get; }

        TimeSpan ClientHealthCheckInterval { get; }
    }

    public abstract record class ServerStartupSettings : SettingGroup, IServerStartupSettings,
        IProcessStartInfoArgumentProvider,
        IHealthCheckSettingsProvider
    {
        protected ServerStartupSettings(ServerStartupSettings original, IEnumerable<RubberduckSetting>? settings = null)
            : base(original)
        {
            Name = original.Name;
            Description = original.Description;
            Settings = settings ?? GetDefaultSettings(original.Name, DefaultServerExecutablePath);
        }

        protected ServerStartupSettings(string name, string description)
            : base(name, description) 
        {
            Settings = GetDefaultSettings(name, DefaultServerExecutablePath);
        }

        private static RubberduckSetting[] GetDefaultSettings(string name, string path)
        {
            return new RubberduckSetting[]
            {
                new ServerExecutablePathSetting($"{name}.{nameof(ServerStartupSettings)}", new Uri(path)),
                new ServerTransportTypeSetting($"{name}.{nameof(ServerTransportTypeSetting)}", TransportType.StdIO),
                new ServerPipeNameSetting($"{name}.{nameof(ServerPipeNameSetting)}", string.Empty),
                new ServerMessageModeSetting($"{name}.{nameof(ServerMessageModeSetting)}", MessageMode.Message),
                new TraceLevelSetting($"{name}.{nameof(TraceLevelSetting)}", MessageTraceLevel.Verbose),
                new ClientHealthCheckIntervalSetting($"{name}.{nameof(ClientHealthCheckIntervalSetting)}", TimeSpan.FromSeconds(10)),
            };
        }

        protected abstract string DefaultServerExecutablePath { get; }
        public string ServerExecutablePath => Settings.OfType<ServerExecutablePathSetting>().Single().Value.LocalPath;

        public TransportType ServerTransportType => Settings.OfType<ServerTransportTypeSetting>().Single().Value;

        protected abstract string DefaultServerPipeName { get; }
        public string ServerPipeName => Settings.OfType<ServerPipeNameSetting>().Single().Value;
        public MessageMode ServerMessageMode => Settings.OfType<ServerMessageModeSetting>().Single().Value;

        public MessageTraceLevel ServerTraceLevel => Settings.OfType<TraceLevelSetting>().Single().Value;

        public TimeSpan ClientHealthCheckInterval => Settings.OfType<ClientHealthCheckIntervalSetting>().Single().Value;

        public override string ToString() => ToProcessStartInfoArguments(0);


        MessageTraceLevel IHealthCheckSettingsProvider.ServerTraceLevel => ServerTraceLevel;

        TimeSpan IHealthCheckSettingsProvider.ClientHealthCheckInterval => ClientHealthCheckInterval;


        MessageTraceLevel IProcessStartInfoArgumentProvider.ServerTraceLevel => ServerTraceLevel;

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

        string IProcessStartInfoArgumentProvider.ToProcessStartInfoArguments(long clientProcessId) => ToProcessStartInfoArguments(clientProcessId);
    }
}
