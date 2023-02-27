using System;

namespace Rubberduck.RPC.Platform.Exceptions
{
    /// <summary>
    /// A common base class for all custom exceptions.
    /// </summary>
    /// <remarks>
    /// These exceptions should be caught at the command level and logged at <c>Error</c> level.
    /// All other exceptions should be logged and rethrown.
    /// </remarks>
    public abstract class ServerException : ApplicationException
    {
        protected ServerException(string source, string message, string verbose = null)
            : base(message)
        {
            Source = source;
            Verbose = verbose;
        }

        /// <summary>
        /// Gets the verbose part of the message, if applicable.
        /// </summary>
        public virtual string Verbose { get; }
        public override string Message => $"{base.Message} {Verbose}".TrimEnd();
    }
}
