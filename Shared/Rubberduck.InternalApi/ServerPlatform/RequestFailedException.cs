using System;

namespace Rubberduck.InternalApi.ServerPlatform
{
    public class RequestFailedException : Exception
    {
        public RequestFailedException(string name, Exception? inner)
            : base($"{name} request failed, see inner exception for details.", inner) { }
    }
}
