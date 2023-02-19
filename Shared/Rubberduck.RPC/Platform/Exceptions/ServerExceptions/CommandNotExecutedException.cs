namespace Rubberduck.RPC.Platform.Exceptions
{
    /// <summary>
    /// An exception that may be thrown when a command was unexpectedly not executed.
    /// </summary>
    public class CommandNotExecutedException : ServerException 
    {
        /// <summary>
        /// An exception that may be thrown when a command was unexpectedly not executed.
        /// </summary>
        public CommandNotExecutedException(string source, string message = null)
            : base(source, message ?? "Command was not executed.")
        {
        }
    }
}