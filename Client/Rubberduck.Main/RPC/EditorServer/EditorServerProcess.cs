using Microsoft.Extensions.Logging;
using Rubberduck.ServerPlatform;
using Rubberduck.SettingsProvider;

namespace Rubberduck.Main.RPC.EditorServer
{
    public class EditorServerProcess : ServerProcess
    {
        public EditorServerProcess(ILogger logger)
            : base(logger) { }

        protected override string ExecutableFileName { get; } = ServerPlatformSettings.EditorServerExecutable;
    }
}