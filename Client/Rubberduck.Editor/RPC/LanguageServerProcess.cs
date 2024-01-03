using Microsoft.Extensions.Logging;
using Rubberduck.ServerPlatform;
using Rubberduck.SettingsProvider;

namespace Rubberduck.Editor.RPC
{
    public class LanguageServerProcess : ServerProcess
    {
        public LanguageServerProcess(ILogger logger)
            : base(logger) { }

        protected override string ExecutableFileName { get; } = ServerPlatformSettings.LanguageServerExecutable;
    }
}