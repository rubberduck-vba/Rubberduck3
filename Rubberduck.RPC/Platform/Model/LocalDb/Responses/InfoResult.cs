using System;

namespace Rubberduck.RPC.Platform.Model.LocalDb.Responses
{
    public class InfoResult : IServerState
    {
        public string Name { get; set; }
        public int ProcessId { get; set; }
        public DateTime? StartTime { get; set; }
        public string Version { get; set; }

        public RpcClientInfo[] Clients { get; set; }

        public bool IsAlive { get; set; }
        public int MessagesReceived { get; set; }
        public int MessagesSent { get; set; }
        public long PeakWorkingSet { get; set; }
        public ServerStatus Status { get; set; }
        public int Threads { get; set; }
        public long WorkingSet { get; set; }

        public long GC { get; set; }
    }
}
