namespace Rubberduck.InternalApi.Model.Workspace
{
    public class VBProjectInfo
    {
        public string Name { get; set; }
        public string ProjectId { get; set; }
        public string? Location { get; set; }
        public bool IsLocked { get; set; }
        public bool HasWorkspace { get; set; }
    }
}
