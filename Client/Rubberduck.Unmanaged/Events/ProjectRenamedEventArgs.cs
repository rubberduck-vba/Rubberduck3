namespace Rubberduck.Unmanaged.Events
{
    public class ProjectRenamedEventArgs : ProjectEventArgs
    {
        public ProjectRenamedEventArgs(string projectId, string projectName, string oldName) : base(projectId, projectName)
        {
            OldName = oldName;
        }

        public string OldName { get; }
    }
}