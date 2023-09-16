using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using Rubberduck.InternalApi;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.ServerPlatform
{
    public class LanguageServerProcess : ServerProcess<ILanguageClient>
    {
        public LanguageServerProcess(IServiceProvider provider) : base(provider)
        {
        }

        public override string Path { get; } = ServerPlatformSettings.LanguageServerExecutable;

        protected async override Task StartAsync(ILanguageClient client)
        {
            var cts = new CancellationTokenSource(10000);
            await client.Initialize(cts.Token);
        }
    }
}