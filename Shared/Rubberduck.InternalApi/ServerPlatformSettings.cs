namespace Rubberduck.InternalApi
{
    public static class ServerPlatformSettings
    {
        public static readonly string DatabaseServerConnectionString = "Data Source=Rubberduck.db;";

        public static readonly string DatabaseServerExecutable = "Rubberduck.DatabaseServer.exe";
        public static readonly string LanguageServerExecutable = "Rubberduck.LanguageServer.exe";
        public static readonly string TelemetryServerExecutable = "Rubberduck.TelemetryServer.exe";
        public static readonly string UpdateServerExecutable = "Rubberduck.UpdateServer.exe";

        public static readonly string DatabaseServerPipeName = @"\\.\Rubberduck.DatabaseServer\RPC";
        public static readonly string LanguageServerPipeName = @"\\.\Rubberduck.LanguageServer\RPC";
        public static readonly string TelemetryServerPipeName = @"\\.\Rubberduck.TelemetryServer\RPC";
        public static readonly string UpdateServerPipeName = @"\\.\Rubberduck.UpdateServer\RPC";

        public static readonly string DatabaseServerName = "Rubberduck.DatabaseServer";
        public static readonly string LanguageServerName = "Rubberduck.LanguageServer";
        public static readonly string TelemetryServerName = "Rubberduck.TelemetryServer";
        public static readonly string UpdateServerName = "Rubberduck.UpdateServer";
    }
}