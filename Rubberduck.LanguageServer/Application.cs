using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using Rubberduck.ServerPlatform;
using Rubberduck.ServerPlatform.Model;
using Rubberduck.ServerPlatform.RPC;
using Rubberduck.ServerPlatform.RPC.DatabaseServer;
using System.Diagnostics;

namespace Rubberduck.LanguageServer
{
    internal class Application : IServerApplication
    {
        private readonly ILanguageServer _languageServer;
        private readonly IJsonRpcServer _databaseServer;
        
        public Application(ILanguageServer languageServer, IJsonRpcServer databaseServer)
        {
            _languageServer = languageServer;
            _databaseServer = databaseServer;
        }

        public async Task StartAsync(CancellationToken token)
        {
            var processId = Process.GetCurrentProcess().Id;
            var assemnbly = typeof(Application).Assembly.GetName() ?? throw new InvalidOperationException("Could not retrieve assembly name/version.");

            ServerProcessClientHelper.StartDatabaseServer(hidden: false);

            Console.WriteLine($"Connecting...");
            var client = new ClientProcessInfo
            {
                Name = ServerPlatform.Settings.LanguageServerName,
                Version = assemnbly.Version?.ToString(3),
                ProcessId = processId
            };
            
            _ = await _databaseServer.SendRequest(JsonRpcMethods.DatabaseServer.Connect, client).Returning<ConnectResult>(token);

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