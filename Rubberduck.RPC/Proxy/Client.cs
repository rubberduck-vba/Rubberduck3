namespace Rubberduck.RPC.Proxy
{
    public class Client
    {
        /// <summary>
        /// The client's host process ID.
        /// </summary>
        public int ProcessId { get; set; }

        /// <summary>
        /// A friendly name for this client.
        /// </summary>
        public string Name { get; set; }
    }
}
