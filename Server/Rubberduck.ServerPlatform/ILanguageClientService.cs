using OmniSharp.Extensions.LanguageServer.Client;
using System.Reflection;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.InternalApi.ServerPlatform;

namespace Rubberduck.ServerPlatform
{
    public interface ILanguageClientService : ILanguageServerConnectionStatusProvider
    {
        LanguageClientOptions ConfigureLanguageClient(LanguageClientOptions options, Assembly clientAssembly, long clientProcessId, RubberduckSettings settings, string workspaceRoot);
    }
}
