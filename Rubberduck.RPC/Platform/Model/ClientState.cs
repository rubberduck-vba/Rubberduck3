namespace Rubberduck.RPC.Platform.Model
{
    /// <summary>
    /// Describes the current state of a RPC client.
    /// </summary>
    public enum ClientState
    {
        /// <summary>
        /// Client has sent an <c>Initialize</c> request to the server.
        /// </summary>
        Connecting,
        /// <summary>
        /// Client has sent an <c>Initialized</c> request, signaling it is ready to send requests and accept notifications.
        /// </summary>
        Initialized,
        /// <summary>
        /// Client has sent a <c>Shutdown</c> notification, signaling its disconnection from the server.
        /// </summary>
        Disconnecting,
    }
}
