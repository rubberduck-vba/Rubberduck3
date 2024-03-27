using OmniSharp.Extensions.LanguageServer.Protocol.Server;
using OmniSharp.Extensions.LanguageServer.Server;

namespace Rubberduck.InternalApi.Services;

public class LanguageServerProvider
{
    public static LanguageServer? Server { get; set; }

    public ILanguageServer? LanguageServer => Server;
}