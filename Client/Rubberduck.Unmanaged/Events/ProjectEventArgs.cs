using System;

namespace Rubberduck.Unmanaged.Events
{
    public class ProjectEventArgs : EventArgs
    {
        public ProjectEventArgs(string projectId, string projectName)
        {
            ProjectId = projectId;
            ProjectName = projectName;
        }

        public string ProjectId { get; }

        public string ProjectName { get; }
    }
}