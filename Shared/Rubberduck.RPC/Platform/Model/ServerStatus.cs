namespace Rubberduck.RPC.Platform.Model
{
    /// <summary>
    /// Describes the current state of a RPC server.
    /// </summary>
    public enum ServerStatus
    {
        /// <summary>
        /// The initial server state.
        /// </summary>
        Starting,
        /// <summary>
        /// The server is configured and listening, but no client has connected yet.
        /// </summary>
        Started,
        /// <summary>
        /// Received an <c>Initialize</c> request, awaiting client <c>Initialized</c> notification.
        /// </summary>
        AwaitingInitialized,
        /// <summary>
        /// Client has sent an <c>Initialized</c> request, signaling it is ready to send requests and accept notifications.
        /// </summary>
        Initialized,
        /// <summary>
        /// Server is shutting down.
        /// </summary>
        ShuttingDown,
        /// <summary>
        /// Server process is terminating.
        /// </summary>
        Terminating,
    }
}
