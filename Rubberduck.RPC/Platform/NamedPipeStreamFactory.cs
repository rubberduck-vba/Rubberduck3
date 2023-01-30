using System.IO;
using System.IO.Pipes;

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

    public class NamedPipeStreamFactory : IRpcStreamFactory<NamedPipeServerStream>
    {
        private readonly string _pipeName;
        private readonly int _maxConcurrentRequests;

        public NamedPipeStreamFactory(string pipeName, int maxConcurrentRequests)
        {
            _pipeName = pipeName;
            _maxConcurrentRequests = maxConcurrentRequests;
        }

        public NamedPipeServerStream CreateNew()
        {
            return new NamedPipeServerStream(_pipeName, PipeDirection.InOut, _maxConcurrentRequests, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
        }
    }
}
