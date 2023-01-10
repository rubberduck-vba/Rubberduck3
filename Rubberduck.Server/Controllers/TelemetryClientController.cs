using Rubberduck.InternalApi.RPC.LSP.Client;
using System;
using System.Threading.Tasks;

namespace Rubberduck.Server.Controllers
{
    public class TelemetryClientController : ITelemetryClient
    {
        public Task TelemetryEvent(object parameters)
        {
            throw new NotImplementedException();
        }
    }
}
