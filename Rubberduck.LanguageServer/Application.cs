using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using Rubberduck.ServerPlatform;
using Rubberduck.ServerPlatform.Model;
using Rubberduck.ServerPlatform.RPC;
using Rubberduck.ServerPlatform.RPC.DatabaseServer;

namespace Rubberduck.LanguageServer
{
    internal class Application : IServerApplication
    {
        private readonly ILanguageServer _languageServer;
        private readonly JsonRpcServer _databaseServer;
        
        public Application(ILanguageServer languageServer, JsonRpcServer databaseServer)
        {
            _languageServer = languageServer;
            _databaseServer = databaseServer;
        }

        public async Task StartAsync(CancellationToken token)
        {
            var processId = Environment.ProcessId;
            var assemnbly = typeof(Application).Assembly.GetName() ?? throw new InvalidOperationException("Could not retrieve assembly name/version.");

            ServerProcessClientHelper.StartDatabaseServer(hidden: false);

            Console.WriteLine($"Initializing database server...");
            await _databaseServer.Initialize(token);
            
            Console.WriteLine($"Connecting...");
            var client = new ClientProcessInfo
            {
                Name = ServerPlatform.Settings.LanguageServerName,
                Version = assemnbly.Version?.ToString(3),
                ProcessId = processId
            };

            var connect = await _databaseServer.SendRequest(JsonRpcMethods.DatabaseServer.Connect, client).Returning<ConnectResult>(token);

            Console.WriteLine($"Requesting database server info...");
            var info = (await _databaseServer.SendRequest(JsonRpcMethods.DatabaseServer.Info, client).Returning<InfoResult>(token)).ServerInfo;
            
            Console.WriteLine($"*** Rubberduck.Server.Database state @{DateTime.Now:u}:");
            Console.WriteLine($"Name: {info.Name}\tVersion: {info.Version}");
            Console.WriteLine($"Process ID: {info.ProcessId}");
            Console.WriteLine($"State: {info.Status}");
            Console.WriteLine($"StartTime: {(info.StartTime.HasValue ? info.StartTime.Value.ToString("u") : string.Empty)}");
            Console.WriteLine($"Clients: {info.Clients}\tThreads: {info.Threads}");
            Console.WriteLine($"GC: {info.GC}\tWorkingSet: {info.WorkingSet:N0} bytes\tPeak: {info.PeakWorkingSet:N0} bytes");
            Console.WriteLine($"Messages received: {info.MessagesReceived:N0}\tSent: {info.MessagesSent:N0}");
            Console.WriteLine($"*** END OF RESPONSE");

            Console.WriteLine("\n*** Press <ENTER> to await language server task");
        }
    }
}