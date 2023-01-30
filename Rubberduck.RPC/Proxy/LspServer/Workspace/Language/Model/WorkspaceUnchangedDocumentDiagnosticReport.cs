using Rubberduck.RPC.Platform;

namespace Rubberduck.RPC.Proxy.LspServer.Workspace.Language.Model
{
    public class WorkspaceUnchangedDocumentDiagnosticReport : WorkspaceFullDocumentDiagnosticReport 
    {
        public override Constants.DocumentDiagnosticReportKind.AsStringEnum Kind { get; set; } 
            = Constants.DocumentDiagnosticReportKind.AsStringEnum.Unchanged;
    }
}
