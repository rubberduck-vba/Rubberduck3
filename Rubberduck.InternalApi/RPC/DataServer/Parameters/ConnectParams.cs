using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubberduck.InternalApi.RPC.DataServer.Parameters
{
    [ProtoContract(Name = "connectParams")]
    public class ConnectParams
    {
        [ProtoMember(1, Name = "processId")]
        public int ProcessId { get; set; }

        [ProtoMember(2, Name = "name")]
        public string Name { get; set; }
    }

    [ProtoContract(Name = "disconnectParams")]
    public class DisconnectParams
    {
        [ProtoMember(1, Name = "processId")]
        public int ProcessId { get; set; }
    }
}
