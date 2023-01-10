namespace Rubberduck.InternalApi.RPC.LSP.Response
{
    public class RelatedUnchangedDocumentDiagnosticReport : RelatedFullDocumentDiagnosticReport
    {
        public override string Kind { get; set; } = Constants.DocumentDiagnosticReportKind.Unchanged;
    }
}
