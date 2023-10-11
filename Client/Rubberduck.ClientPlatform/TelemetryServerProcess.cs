using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using Rubberduck.InternalApi.ServerPlatform;

namespace Rubberduck.Client
{
    public class TelemetryServerProcess : ServerProcess<ILanguageClient>
    {
        public TelemetryServerProcess(ILogger logger) 
            : base(logger)
        {
        }

        protected override string ExecutableFileName { get; } = ServerPlatformSettings.TelemetryServerExecutable;
    }
}