namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    /// <summary>
    /// The <c>Initialized</c> notification is sent <em>from the client to the server</em> after the client received an <c>InitializeResult</c>,
    /// but before the client is sending any other requests or notification to the server.
    /// </summary>
    /// <remarks>
    /// Per LSP an <c>Initialized</c> notification may only be sent once (assumed: <em>per client</em>).
    /// </remarks>
    public class InitializedParams 
    {
    }
}
