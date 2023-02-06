using System.IO;

namespace Rubberduck.RPC.Platform
{
    /// <summary>
    /// An object responsible for creating an RPC stream.
    /// </summary>
    /// <typeparam name="TStream">The type of stream returned by this factory.</typeparam>
    public interface IRpcStreamFactory<out TStream> where TStream : Stream
    {
        /// <summary>
        /// Creates and configures a new RPC stream instance.
        /// </summary>
        /// <returns>The configured RPC stream.</returns>
        TStream CreateNew();
    }

}
