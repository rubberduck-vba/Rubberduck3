namespace Rubberduck.ServerPlatform
{
    /// <summary>
    /// Thrown at the handler when an operation cannot be completed, and wouldn't otherwise throw an exception to return a proper error response.
    /// </summary>
    /// <remarks>
    /// Use the inner exception whenever possible to preserve details about errors bubbled up or rethrown from a lower abstraction level.
    /// </remarks>
    public class OperationNotCompletedException : ServerException
    {
        public OperationNotCompletedException(string source, string message = "The operation was not completed.", string verbose = null, Exception innerException = null)
            : base(source, message, verbose, innerException)
        {
        }
    }
}
