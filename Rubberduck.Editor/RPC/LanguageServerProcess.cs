using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using Rubberduck.InternalApi.ServerPlatform;

namespace Rubberduck.Editor.RPC
{
    public class LanguageServerProcess : ServerProcess<ILanguageClient>
    {
        public LanguageServerProcess(ILogger logger)
            : base(logger) { }

        protected override string ExecutableFileName { get; } = ServerPlatformSettings.LanguageServerExecutable;
    }
}