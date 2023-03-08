namespace Rubberduck.ServerPlatform.Model
{
    public class ServerProcessInfo
    {
        /// <summary>
        /// The ID of the server process.
        /// </summary>
        public int ProcessId { get; set; }
        /// <summary>
        /// The name of the server.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The server application version.
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// The start date/time of this server. <c>null</c> if the server wasn't started.
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// The clients connected to this server.
        /// </summary>
        public ClientProcessInfo[] Clients { get; set; }
        /// <summary>
        /// The number of messages received by this server.
        /// </summary>
        public int MessagesReceived { get; set; }
        /// <summary>
        /// The number of messages sent by this server.
        /// </summary>
        public int MessagesSent { get; set; }
        /// <summary>
        /// Peak working set memory, in bytes.
        /// </summary>
        public long PeakWorkingSet { get; set; }
        /// <summary>
        /// The current status state of this server.
        /// </summary>
        public ServerState Status { get; set; }
        /// <summary>
        /// The number of threads in the host process.
        /// </summary>
        public int Threads { get; set; }
        /// <summary>
        /// Working set memory, in bytes.
        /// </summary>
        public long WorkingSet { get; set; }
        /// <summary>
        /// The approximate number of bytes currently allocated.
        /// </summary>
        public long GC { get; set; }
    }
}
