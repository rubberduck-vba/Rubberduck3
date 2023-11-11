using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.ServerPlatform;

namespace Rubberduck.Main.RPC
{
    public class UpdateServerProcess : ServerProcess
    {
        public UpdateServerProcess(ILogger logger)
            : base(logger) { }

        protected override string ExecutableFileName { get; } = ServerPlatformSettings.UpdateServerExecutable;
    }
}