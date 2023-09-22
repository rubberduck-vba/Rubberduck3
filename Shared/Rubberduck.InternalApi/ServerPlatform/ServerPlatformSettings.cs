namespace Rubberduck.InternalApi.ServerPlatform
{
    public static class ServerPlatformSettings
    {
        public const TransportType DefaultTransportType = TransportType.StdIO;
        public const string DefaultTraceLevel = "Verbose";

        public const string DatabaseServerConnectionString = "Data Source=Rubberduck.db;";

        public const string DatabaseServerExecutable = "Rubberduck.DatabaseServer.exe";
        public const string LanguageServerExecutable = "Rubberduck.LanguageServer.exe";
        public const string TelemetryServerExecutable = "Rubberduck.TelemetryServer.exe";
        public const string UpdateServerExecutable = "Rubberduck.UpdateServer.exe";

        public const string DatabaseServerPipeName = @"\\.\Rubberduck.DatabaseServer\RPC";
        public const string LanguageServerPipeName = @"\\.\Rubberduck.LanguageServer\RPC";
        public const string TelemetryServerPipeName = @"\\.\Rubberduck.TelemetryServer\RPC";
        public const string UpdateServerPipeName = @"\\.\Rubberduck.UpdateServer\RPC";

        public const string DatabaseServerName = "Rubberduck.DatabaseServer";
        public const string LanguageServerName = "Rubberduck.LanguageServer";
        public const string TelemetryServerName = "Rubberduck.TelemetryServer";
        public const string UpdateServerName = "Rubberduck.UpdateServer";

        public const string DatabaseServerDefaultPipeName = "Rubberduck.DatabaseServer.Pipe";
        public const string LanguageServerDefaultPipeName = "Rubberduck.LanguageServer.Pipe";
        public const string TelemetryServerDefaultPipeName = "Rubberduck.TelemetryServer.Pipe";
        public const string UpdateServerDefaultPipeName = "Rubberduck.UpdateServer.Pipe";
    }
}