using System;

namespace Rubberduck.InternalApi.ServerPlatform
{
    public class InvalidRequestParamsException : ArgumentException
    {
        public InvalidRequestParamsException(string name, object request)
            : base($"{request.GetType().Name} is missing a required member.", name)
        {
            Data.Add("request", request);
        }
    }
}
