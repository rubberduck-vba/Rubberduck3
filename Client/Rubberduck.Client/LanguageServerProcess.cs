using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using Rubberduck.InternalApi;
using System.Threading;
using System.Threading.Tasks;

namespace Rubberduck.ServerPlatform
{
    public class LanguageServerProcess : ServerProcess<ILanguageClient>
    {
        protected override string RelativePath => @"Server\Rubberduck.LanguageServer";
        protected override string ExecutableFileName { get; } = ServerPlatformSettings.LanguageServerExecutable;

        protected async override Task InitializeAsync(ILanguageClient client, CancellationToken token)
        {
            await client.Initialize(token);
        }
    }
}