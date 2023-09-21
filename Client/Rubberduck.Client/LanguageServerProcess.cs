using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using Rubberduck.InternalApi.ServerPlatform;

namespace Rubberduck.Client
{
    public class LanguageServerProcess : ServerProcess<ILanguageClient>
    {
        protected override string RelativePath => @"Server\Rubberduck.LanguageServer";
        protected override string ExecutableFileName { get; } = ServerPlatformSettings.LanguageServerExecutable;
    }
}