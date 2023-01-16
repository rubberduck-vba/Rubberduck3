using Rubberduck.InternalApi.RPC.LSP.Capabilities;
using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.RPC.LSP.Response
{
    public class InitializeResult : InitializeResult<ServerCapabilities>
    {
    }
}
