using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.ServerPlatform;
using Rubberduck.ServerPlatform;

namespace Rubberduck.Editor.RPC
{
    public class LanguageServerProcess : ServerProcess
    {
        public LanguageServerProcess(ILogger logger)
            : base(logger) { }

        protected override string ExecutableFileName { get; } = ServerPlatformSettings.LanguageServerExecutable;
    }
}