using OmniSharp.Extensions.LanguageServer.Protocol.Client;
using Rubberduck.InternalApi.ServerPlatform;
using Rubberduck.SettingsProvider.Model;

namespace Rubberduck.Client
{
    public class LanguageServerProcess : ServerProcess<ILanguageClient>
    {
        protected override string ExecutableFileName { get; } = ServerPlatformSettings.LanguageServerExecutable;
    }
}