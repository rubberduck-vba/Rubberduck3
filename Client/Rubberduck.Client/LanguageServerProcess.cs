using Rubberduck.InternalApi;

namespace Rubberduck.ServerPlatform
{
    public class LanguageServerProcess : ServerProcess
    {
        public override string Path { get; } = ServerPlatformSettings.LanguageServerExecutable;
    }
}