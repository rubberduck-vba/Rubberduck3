using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubberduck.InternalApi.RPC.DataServer.Response
{
    [ProtoContract(Name = "connectResult")]
    public class ConnectResult
    {
        [ProtoMember(1, Name = "connected")]
        public bool Connected { get; set; }
    }

    [ProtoContract(Name = "disconnectResult")]
    public class DisconnectResult
    {
        [ProtoMember(1, Name = "disconnected")]
        public bool Disconnected { get; set; }

        [ProtoMember(2, Name = "shuttingDown")]
        public bool ShuttingDown { get; set; }
    }
}
