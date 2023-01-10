using ProtoBuf;

namespace Rubberduck.InternalApi.RPC.LSP.Capabilities
{
    [ProtoContract(Name = "diagnosticOptions")]
    public class DiagnosticOptions : WorkDoneProgressOptions
    {
        /// <summary>
        /// An optional identifier under which the diagnostics are managed by the client.
        /// </summary>
        [ProtoMember(2, Name = "identifier")]
        public string Identifier { get; set; }

        /// <summary>
        /// Whether the language has inter-file dependencies.
        /// </summary>
        [ProtoMember(3, Name = "interFileDependencies")]
        public bool InterFileDependencies { get; set; } = true;

        /// <summary>
        /// Whether the server provides support for workspace diagnostics as well.
        /// </summary>
        [ProtoMember(4, Name = "workspaceDiagnostics")]
        public bool WorkspaceDiagnostics { get; set; } = true;
    }
}
