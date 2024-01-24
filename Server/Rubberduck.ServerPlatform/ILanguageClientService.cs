using OmniSharp.Extensions.LanguageServer.Client;
using System.Reflection;
using Rubberduck.InternalApi.ServerPlatform;
using Rubberduck.InternalApi.Settings.Model;

namespace Rubberduck.ServerPlatform
{
    public interface ILanguageClientService : ILanguageServerConnectionStatusProvider
    {
        LanguageClientOptions ConfigureLanguageClient(LanguageClientOptions options, Assembly clientAssembly, long clientProcessId, RubberduckSettings settings, string workspaceRoot);
    }
}
