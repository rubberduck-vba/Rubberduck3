using System.IO.Pipes;

namespace Rubberduck.RPC.Platform
{

    public class NamedPipeServerStreamFactory : IRpcStreamFactory<NamedPipeServerStream>
    {
        private readonly string _pipeName;
        private readonly int _maxConcurrentRequests;

        public NamedPipeServerStreamFactory(string pipeName, int maxConcurrentRequests)
        {
            _pipeName = pipeName;
            _maxConcurrentRequests = maxConcurrentRequests;
        }

        public NamedPipeServerStream CreateNew()
        {
            return new NamedPipeServerStream(_pipeName, PipeDirection.InOut, _maxConcurrentRequests, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
        }
    }

}
