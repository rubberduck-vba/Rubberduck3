using OmniSharp.Extensions.LanguageServer.Client;
using OmniSharp.Extensions.LanguageServer.Protocol.Client.Capabilities;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using OmniSharp.Extensions.LanguageServer.Protocol;
using System.Reflection;
using System.Threading.Tasks;
using System.Threading;
using OmniSharp.Extensions.LanguageServer.Protocol.Client;

namespace Rubberduck.Client
{
    public class LanguageClientService
    {
        private readonly ILanguageClient _client;

        public LanguageClientService(ILanguageClient client) 
        {
            _client = client;
        }

        public async Task InitializeAsync(CancellationToken token) => await _client.Initialize(token);
    }
}
