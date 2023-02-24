
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.RPC.Platform.Model;
using System;

namespace Rubberduck.Server.LocalDb.RPC.Info
{
    public class InfoResult : IServerState
    {
        public string Name { get; set; }
        public int ProcessId { get; set; }
        public DateTime? StartTime { get; set; }
        public string Version { get; set; }

        public ClientInfo[] Clients { get; set; }

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
