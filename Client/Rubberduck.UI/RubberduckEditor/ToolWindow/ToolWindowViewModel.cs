namespace Rubberduck.UI.RubberduckEditor.ToolWindow
{
    public abstract class ToolWindowViewModel : ViewModelBase
    {
        protected ToolWindowViewModel(string title)
        {
            Title = title;
        }

        public string Title { get; init; }
    }

    public class WorkspaceExplorerViewModel : ToolWindowViewModel
    {
        public WorkspaceExplorerViewModel()
            : base("Workspace Explorer")
        {
        }
    }

    public class CodeExplorerViewModel : ToolWindowViewModel
    {
        public CodeExplorerViewModel()
            : base("Code Explorer")
        {
        }
    }

    public class TestExplorerViewModel : ToolWindowViewModel
    {
        public TestExplorerViewModel()
            : base("Test Explorer")
        {
        }
    }

    public class DiagnosticsViewModel : ToolWindowViewModel
    {
        public DiagnosticsViewModel()
            : base("Diagnostics")
        {
        }
    }

    public class CodeMetricsViewModel : ToolWindowViewModel
    {
        public CodeMetricsViewModel()
            : base("Code Metrics")
        {
        }
    }

    public class PropertiesViewModel : ToolWindowViewModel
    {
        public PropertiesViewModel()
            : base("Properties")
        {
        }
    }

    public class ObjectBrowserViewModel : ToolWindowViewModel
    {
        public ObjectBrowserViewModel()
            : base("Object Browser")
        {
        }
    }

    public class CallHierarchyViewModel : ToolWindowViewModel
    {
        public CallHierarchyViewModel()
            : base("Call Hierarchy")
        {
        }
    }

    public class TasksViewModel : ToolWindowViewModel
    {
        public TasksViewModel()
            : base("Tasks")
        {
        }
    }

    public class SearchResultsViewModel : ToolWindowViewModel
    {
        public SearchResultsViewModel()
            : base("Search Results")
        {
        }
    }

    public class LanguageServerTraceViewModel : ToolWindowViewModel
    {
        public LanguageServerTraceViewModel()
            : base("Language Server")
        {
        }
    }
    public class UpdateServerTraceViewModel : ToolWindowViewModel
    {
        public UpdateServerTraceViewModel()
            : base("Update Server")
        {
        }
    }
}
