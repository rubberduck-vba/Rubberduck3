namespace Rubberduck.RPC.Platform
{
    /// <summary>
    /// A marker interface for a type to register as a JsonRpc target.
    /// </summary>
    /// <remarks>
    /// Might turn into an attribute.
    /// </remarks>
    public interface IJsonRpcTarget { }

    /// <summary>
    /// A marker interface for a type to register as a StreamJsonRpc client proxy.
    /// </summary>
    /// <remarks>
    /// Might turn into an attribute.
    /// </remarks>
    public interface IJsonRpcSource { }
}
