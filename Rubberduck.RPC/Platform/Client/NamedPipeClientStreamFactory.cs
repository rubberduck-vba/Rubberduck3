using System.IO.Pipes;

namespace Rubberduck.RPC.Platform
{
    public class NamedPipeClientStreamFactory : IRpcStreamFactory<NamedPipeClientStream>
    {
        private readonly string _pipeName;

        public NamedPipeClientStreamFactory(string pipeName)
        {
            _pipeName = pipeName;
        }

        public NamedPipeClientStream CreateNew()
        {
            return new NamedPipeClientStream(".", _pipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
        }
    }

}
