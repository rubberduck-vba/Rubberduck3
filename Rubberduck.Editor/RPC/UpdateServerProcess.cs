using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.ServerPlatform;
using Rubberduck.InternalApi.ServerPlatform.UpdateServer;

namespace Rubberduck.Editor.RPC
{
    public class UpdateServerProcess : ServerProcess<IUpdateClient>
    {
        public UpdateServerProcess(ILogger logger)
            : base(logger) { }

        protected override string ExecutableFileName { get; } = ServerPlatformSettings.UpdateServerExecutable;
    }
}