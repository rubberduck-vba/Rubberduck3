using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MediatR.Registration;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using Rubberduck.RPC.Platform;
using Rubberduck.RPC.Platform.Model;
using Rubberduck.RPC.Properties;
using Rubberduck.Server.LSP.Configuration;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.Server
{
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static async Task<int> Main(string[] args)
        {
            /*TODO accept command-line arguments*/
            await StartAsync();

            return 0;
        }

        private static async Task StartAsync()
        {
            Splash();
            var tokenSource = new CancellationTokenSource();

            var builder = Host.CreateDefaultBuilder()
                .UseConsoleLifetime(options => options.SuppressStatusMessages = false)
                .ConfigureServices(provider => ConfigureServices(provider, tokenSource))
            ;
            
            var host = builder.Build();            
            var serverTask = host.RunAsync(tokenSource.Token);

            await serverTask;
        }

        private static void ConfigureServices(IServiceCollection services, CancellationTokenSource cts)
        {
            // example LSP server: https://github.com/OmniSharp/csharp-language-server-protocol/blob/master/sample/SampleServer/Program.cs

            var config = new ServerCapabilities
            { 
                /*TODO*/
            };
            services.AddRubberduckServerServices(config, cts);
        }

        private static void Splash()
        {
            var info = typeof(Program).Assembly.GetName();

            Console.WriteLine(ServerSplash.GetRenderString());
            Console.WriteLine($"{info.Name} v{info.Version.ToString(3)}");
            Console.WriteLine($"Initializing...");
        }
    }

    internal class Application
    {
        private readonly IJsonRpcServer _server;

        public Application(IJsonRpcServer server)
        {
            _server = server;
        }

        public async Task RunAsync(CancellationToken token)
        {
            var processId = Process.GetCurrentProcess().Id;
            var assemnbly = typeof(Application).Assembly.GetName();

            Console.WriteLine($"Connecting...");
            var client = new RpcClientInfo
            {
                Name = Settings.Default.LanguageServerName,
                Version = assemnbly.Version.ToString(3),
                ProcessId = processId
            };
            _server.SendNotification(JsonRpcMethods.Database.Connect, client);

            Console.WriteLine($"Requesting database server info...");
            var info = await _server.SendRequest(JsonRpcMethods.Database.Info, client).Returning<InfoResult>(token);

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
