namespace Rubberduck.InternalApi.Model.Workspace
{
    public record class File
    {
        /// <summary>
        /// The name of the file; must be unique across the entire workspace.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The location of the module in the workspace, relative to the source root.
        /// </summary>
        public string Uri { get; set; }
        /// <summary>
        /// <c>true</c> if the module should open when the workspace is loaded in the Rubberduck Editor.
        /// </summary>
        public bool IsAutoOpen { get; set; }
    }
}
