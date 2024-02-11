using System;

namespace Rubberduck.Unmanaged.Events
{
    public class ProjectEventArgs : EventArgs
    {
        public ProjectEventArgs(Uri workspaceUri, string projectName)
        {
            WorkspaceUri = workspaceUri;
            ProjectName = projectName;
        }

        public Uri WorkspaceUri { get; }

        public string ProjectName { get; }
    }
}