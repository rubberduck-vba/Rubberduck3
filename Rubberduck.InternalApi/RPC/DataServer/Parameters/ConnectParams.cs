using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubberduck.InternalApi.RPC.DataServer.Parameters
{
    public class ConnectParams
    {
        public int ProcessId { get; set; }
        public string Name { get; set; }
    }

    public class DisconnectParams
    {
        public int ProcessId { get; set; }
    }
}
