using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Window;
using Rubberduck.LanguageServer.Configuration;
using Rubberduck.ServerPlatform;
using System.Reflection;

namespace Rubberduck.LanguageServer
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
            Console.WriteLine(ServerSplash.GetRenderString(Assembly.GetExecutingAssembly().GetName()));

            var tokenSource = new CancellationTokenSource();

            var builder = Host.CreateDefaultBuilder()
                .UseConsoleLifetime()
                .ConfigureLogging((context, logging) =>
                {
                    logging.AddConsole();
                })
                .ConfigureServices(provider => ConfigureServices(provider, tokenSource))
            ;

            var host = builder.Build();
            await host.StartAsync(tokenSource.Token);

            var canStart = false;

            try
            {
                var app = host.Services.GetRequiredService<Application>();
                await app.StartAsync(tokenSource.Token);

                canStart = true;
            }
            catch (Exception exception)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine($"FATAL: {exception}");
            }

            if (canStart)
            {
                await host.WaitForShutdownAsync();
            }
            else
            {
                tokenSource.Cancel();
            }
        }

        private static void ConfigureServices(IServiceCollection services, CancellationTokenSource tokenSource)
        {
            // example LSP server: https://github.com/OmniSharp/csharp-language-server-protocol/blob/master/sample/SampleServer/Program.cs
            var config = new ServerCapabilities
            {
                /*TODO*/
            };
            services.AddRubberduckServerServices(config, tokenSource);
        }
    }
}