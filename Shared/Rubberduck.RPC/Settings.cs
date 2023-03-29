namespace Rubberduck.ServerPlatform
{
    public static class Settings
    {
        public static readonly string DatabaseServerConnectionString = "Data Source=Rubberduck.db;";

        public static readonly string DatabaseServerExecutableLocation = @"..\..\..\..\Rubberduck.DatabaseServer\bin\Debug\net6.0\Rubberduck.DatabaseServer.exe";
        public static readonly string LanguageServerExecutableLocation = @"..\..\..\..\Rubberduck.LanguageServer\bin\Debug\net6.0\Rubberduck.LanguageServer.exe";
        public static readonly string TelemetryServerExecutableLocation = @"..\..\..\..\Rubberduck.TelemetryServer\bin\Debug\net6.0\Rubberduck.TelemetryServer.exe";
        public static readonly string UpdateServerExecutableLocation = @"..\..\..\..\Rubberduck.UpdateServer\bin\Debug\net6.0\Rubberduck.UpdateServer.exe";

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