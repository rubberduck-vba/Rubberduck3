using Rubberduck.InternalApi.Extensions;
using System;
using System.Collections.Generic;

namespace Rubberduck.InternalApi.Services;

public interface IWorkspaceStateManager
{
    IWorkspaceState GetWorkspace(Uri workspaceRoot);
    IEnumerable<IWorkspaceState> Workspaces { get; }
    /// <summary>
    /// Gets the currently selected/active workspace/project.
    /// </summary>
    IWorkspaceState? ActiveWorkspace { get; }
    IWorkspaceState AddWorkspace(Uri workspaceRoot);
    void Unload(Uri workspaceRoot);
}
