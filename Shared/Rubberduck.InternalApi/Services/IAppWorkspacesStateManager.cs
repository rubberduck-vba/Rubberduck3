using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.ServerPlatform.LanguageServer;
using System;
using System.Collections.Generic;

namespace Rubberduck.InternalApi.Services;

public interface IAppWorkspacesStateManager
{
    event EventHandler<WorkspaceFileUriEventArgs> WorkspaceFileStateChanged;
    IWorkspaceState GetWorkspace(Uri workspaceRoot);
    IEnumerable<IWorkspaceState> Workspaces { get; }
    /// <summary>
    /// Gets the currently selected/active workspace/project.
    /// </summary>
    IWorkspaceState? ActiveWorkspace { get; }
    IWorkspaceState AddWorkspace(Uri workspaceRoot);
    void Unload(Uri workspaceRoot);
}
