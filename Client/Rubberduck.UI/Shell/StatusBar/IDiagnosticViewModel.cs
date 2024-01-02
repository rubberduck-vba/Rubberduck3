using Rubberduck.InternalApi.Model;

namespace Rubberduck.UI.Shell.StatusBar
{
    public interface IDiagnosticViewModel
    {
        string Title { get; set; }
        string Description { get; set; }
        int Severity { get; set; } // TODO import InspectionSeverity enum
        DocumentOffset DocumentOffset { get; set; }
    }
}
