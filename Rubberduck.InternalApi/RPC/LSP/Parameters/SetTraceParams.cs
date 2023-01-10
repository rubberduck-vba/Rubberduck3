using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Parameters
{
    [ProtoContract(Name = "setTraceParams")]
    public class SetTraceParams
    {
        /// <summary>
        /// The new value that should be assigned to the trace setting. See <c>Constants.TraceValue</c>.
        /// </summary>
        [ProtoMember(1, Name = "value")]
        public string Value { get; set; }
    }

    [ProtoContract(Name = "logTraceParams")]
    public class LogTraceParams
    {
        /// <summary>
        /// The message to be logged.
        /// </summary>
        [ProtoMember(1, Name = "message")]
        public string Message { get; set; }
    }

    [ProtoContract(Name = "verboseLogTraceParams")]
    public class VerboseLogTraceParams : LogTraceParams
    {
        /// <summary>
        /// Additional information that can be computed when the trace level is set to 'verbose'.
        /// </summary>
        [ProtoMember(2, Name = "verbose")]
        public string Verbose { get; set; }
    }
}
