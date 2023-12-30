namespace Rubberduck.UI.Shell.StatusBar
{
    public interface ISourceFileStatusViewModel : IDocumentStatusViewModel
    {
        IDiagnosticViewModel[] Diagnostics { get; set; }
    }
}
