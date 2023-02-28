using System;

namespace Rubberduck.RPC.Platform.Exceptions
{
    /// <summary>
    /// An <c>Exit</c> notification was received from a client, but the server was not in <c>ShuttingDown</c> state.
    /// </summary>
    public class UnexpectedExitNotificationException : ServerException
    {
        public UnexpectedExitNotificationException(string source, string message, string verbose = null) 
            : base(source, message, verbose) { }
    }
}