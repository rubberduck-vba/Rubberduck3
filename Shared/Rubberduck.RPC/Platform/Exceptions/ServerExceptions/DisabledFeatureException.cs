using System;

namespace Rubberduck.RPC.Platform.Exceptions
{
    /// <summary>
    /// An exception that is thrown when a feature is disabled through client and/or server options.
    /// </summary>
    public class DisabledFeatureException : ServerException
    {
        public DisabledFeatureException(string name, string verbose = null)
            : base(name, $"Feature is not currently enabled.", verbose) 
        {
        }
    }
}
