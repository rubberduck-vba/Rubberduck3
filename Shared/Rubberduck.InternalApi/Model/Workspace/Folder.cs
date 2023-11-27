namespace Rubberduck.InternalApi.Model.Workspace
{
    public record class Folder
    {
        /// <summary>
        /// The name of the folder.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The location of the module in the workspace, relative to the source root.
        /// </summary>
        public string Uri { get; set; }
    }
}
