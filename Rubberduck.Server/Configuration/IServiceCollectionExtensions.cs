using Microsoft.Extensions.DependencyInjection;
using OmniSharp.Extensions.JsonRpc;
using OmniSharp.Extensions.LanguageServer.Protocol.Server.Capabilities;
using OmniSharp.Extensions.LanguageServer.Server;
using Rubberduck.Server.LSP.RPC.Info;
using Rubberduck.Server.Properties;
using System;
using System.IO;
using System.IO.Pipes;
using System.Threading;

namespace Rubberduck.Server.LSP.Configuration
{
    internal static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddRubberduckServerServices(this IServiceCollection services, ServerCapabilities config, CancellationTokenSource cts) => 
            services.AddLanguageServer(ConfigureLSP)
                    //.AddOtherServicesHere()
            ;

        private static void ConfigureLSP(LanguageServerOptions lsp)
        {
            var (input, output) = WithAsyncNamedPipeTransport(Settings.Default.JsonRpcServerPath);
            
            lsp.WithInput(input)
               .WithOutput(output)
               //add handlers here
            ;
        }

        private static (Stream input, Stream output) WithAsyncNamedPipeTransport(string name)
        {
            var input = new NamedPipeServerStream(name, PipeDirection.InOut, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
            var output = new NamedPipeClientStream(".", name, PipeDirection.InOut, PipeOptions.Asynchronous);
            return (input, output);
        }
    }
}
