using Microsoft.Extensions.Logging;
using Rubberduck.ServerPlatform;
using Rubberduck.SettingsProvider;

namespace Rubberduck.Main.RPC.EditorServer
{
    public class EditorServerProcess : RubberduckServerProcess
    {
        public EditorServerProcess(ILogger logger)
            : base(logger) { }

        protected override string ExecutableFileName { get; } = ServerPlatformSettings.EditorServerExecutable;
    }
}