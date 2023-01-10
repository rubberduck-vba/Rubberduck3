namespace Rubberduck.InternalApi.RPC.LSP
{
    public class WorkDoneProgressEnd
    {
        public string Kind { get; set; } = "end";

        /// <summary>
        /// A final message indicating the outcome of the operation.
        /// </summary>
        public string Message { get; set; }
    }
}
