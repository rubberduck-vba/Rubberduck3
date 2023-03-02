using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using Rubberduck.Server.LSP.Configuration;
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
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
            var tokenSource = new CancellationTokenSource();

            var builder = Host.CreateDefaultBuilder()
                .UseConsoleLifetime()
                .ConfigureServices(provider => ConfigureServices(provider, tokenSource));

            await builder.RunConsoleAsync(tokenSource.Token);
        }

        private static void ConfigureServices(IServiceCollection services, CancellationTokenSource cts)
        {
            var config = new ServerCapabilities
            { 
                /*TODO*/
            };
            services.AddRubberduckServerServices(config, cts);
        }
    }
}
