using Rubberduck.InternalApi.Model.Workspace;
using System;
using System.Collections.Generic;

namespace Rubberduck.UI.Services.NewProject
{
    public interface IWorkspaceSyncService
    {
        /// <summary>
        /// Gets all workspace modules from the specified project loaded in the VBIDE.
        /// </summary>
        /// <param name="scanFolderAnnotations">If <c>true</c>, Rubberduck 2.x <c>@Folder</c> annotations become RD3 workspace folders.</param>
        IEnumerable<Module> GetWorkspaceModules(Uri workspaceRoot, bool scanFolderAnnotations);
        /// <summary>
        /// Exports the specified workspace source files from the VBIDE to the project's workspace folder.
        /// </summary>
        void ExportWorkspaceModules(Uri workspaceRoot, IEnumerable<Module> modules);
        /// <summary>
        /// Imports the specified workspace source files into the VBIDE from the project's workspace folder.
        /// </summary>
        void ImportWorkspaceModules(Uri workspaceRoot, IEnumerable<Module> modules);

        /// <summary>
        /// Gets the references of the specified project from the VBIDE.
        /// </summary>
        IEnumerable<Reference> GetWorkspaceReferences(Uri workspaceRoot);
        /// <summary>
        /// Sets the specified project references as specified.
        /// </summary>
        /// <remarks>
        /// Order of items determines their priority in the references list.
        /// </remarks>
        void SetProjectReferences(Uri workspaceRoot, Reference[] references);
    }
}
