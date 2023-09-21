namespace Rubberduck.ServerPlatform
{
    public enum TransportType
    {
        /// <summary>
        /// Client/server messages are exchanged through standard input/output streams.
        /// </summary>
        StdIO,

        /// <summary>
        /// Client/server messages are exchanged through a bidirectional named pipe stream.
        /// </summary>
        Pipe,

        /// <summary>
        /// Client/server messages are exchanged over raw socket.
        /// </summary>
        /// <remarks>Firewall rules may prevent opening the port, even in RPC-reserved ranges.</remarks>
        RawSocket,
    }
}