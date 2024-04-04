using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.Settings;
using Rubberduck.ServerPlatform;

namespace Rubberduck.Main.RPC.EditorServer
{
    public class EditorServerProcess : RubberduckServerProcess
    {
        public EditorServerProcess(ILogger logger)
            : base(logger) { }

        protected override string ExecutableFileName { get; } = ServerPlatformSettings.EditorServerExecutable;
    }
}