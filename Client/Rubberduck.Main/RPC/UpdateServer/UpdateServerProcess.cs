using Microsoft.Extensions.Logging;
using Rubberduck.ServerPlatform;
using Rubberduck.SettingsProvider;

namespace Rubberduck.Main.RPC
{
    public class UpdateServerProcess : ServerProcess
    {
        public UpdateServerProcess(ILogger logger)
            : base(logger) { }

        protected override string ExecutableFileName { get; } = ServerPlatformSettings.UpdateServerExecutable;
    }
}