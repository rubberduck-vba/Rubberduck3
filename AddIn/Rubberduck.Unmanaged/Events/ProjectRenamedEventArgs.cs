using System;

namespace Rubberduck.Unmanaged.Events
{
    public class ProjectRenamedEventArgs : ProjectEventArgs
    {
        public ProjectRenamedEventArgs(Uri workspaceUri, string projectName, string oldName) : base(workspaceUri, projectName)
        {
            OldName = oldName;
        }

        public string OldName { get; }
    }
}