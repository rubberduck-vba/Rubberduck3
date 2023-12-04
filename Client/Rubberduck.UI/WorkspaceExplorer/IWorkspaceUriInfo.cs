using System;

namespace Rubberduck.UI.WorkspaceExplorer
{
    public interface IWorkspaceUriInfo
    {
        Uri Uri { get; set; }
        int Version { get; set; }
        string Name { get; set; }
        bool IsInProject { get; set; }
        bool IsOpen { get; set; }
        bool IsFileWatcherEnabled { get; set; }
    }
}
