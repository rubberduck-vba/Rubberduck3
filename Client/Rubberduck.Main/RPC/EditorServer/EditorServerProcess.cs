using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.ServerPlatform;

namespace Rubberduck.Main.RPC.EditorServer
{
    public class EditorServerProcess : ServerProcess
    {
        public EditorServerProcess(ILogger logger)
            : base(logger) { }

        protected override string ExecutableFileName { get; } = ServerPlatformSettings.EditorServerExecutable;
    }
}