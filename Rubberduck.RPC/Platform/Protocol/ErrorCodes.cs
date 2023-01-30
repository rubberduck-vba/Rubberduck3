namespace Rubberduck.RPC.Platform.Protocol
{
    public static class ErrorCodes
    {
        /* defined by JSON-RPC */

        public const int ParseError = -32700;
        public const int InvalidRequest = -32600;
        public const int MethodNotFound = -32601;
        public const int InvalidParams = -32602;
        public const int InternalError = -32603;

        public const int JsonRpcReservedErrorRangeStart = -32099;
        public const int JsonRpcReservedErrorRangeEnd = -32000;

        /* LSP error codes */

        /// <summary>
        /// Error code indicating that a server received a notification or request before the server has received the 'initialize' request.
        /// </summary>
        public const int ServerNotInitialized = -32002;
        public const int UnknownErrorCode = -32001;

        /// <summary>
        /// A request failed but it was syntactically correct, e.g. the method name was known and the parameters were valid.
        /// </summary>
        /// <remarks>The error message should contain human readable information about why the request failed.</remarks>
        public const int RequestFailed = -32803;

        /// <summary>
        /// The server cancelled the request. This error code should only be used for requests that explicitly support being server cancellable.
        /// </summary>
        public const int ServerCancelled = -32802;

        /// <summary>
        /// The server detected that the content of a document got modified outside normal conditions.
        /// A server should NOT send this error code if it detects a content change in its unprocessed messages.
        /// The result even computed on an older state might still be useful for the client.
        /// </summary>
        /// <remarks>
        /// If a client determines that a result is not of any use anymore, the client should cancel the request.
        /// </remarks>
        public const int ContentModified = -32801;

        /// <summary>
        /// The client has cancelled a request and a server has detected the cancellation.
        /// </summary>
        public const int RequestCancelled = -32800;
    }
}
