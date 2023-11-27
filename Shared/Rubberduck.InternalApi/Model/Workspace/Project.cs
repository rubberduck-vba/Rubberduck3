using System;
using System.Linq;

namespace Rubberduck.InternalApi.Model.Workspace
{
    public record class Project
    {
        /// <summary>
        /// The name of the project.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Project references.
        /// </summary>
        public Reference[] References { get; set; } = Array.Empty<Reference>();
        /// <summary>
        /// Project source files that synchronize with a host VBA project.
        /// </summary>
        public Module[] Modules { get; set; } = Array.Empty<Module>();
        /// <summary>
        /// Any other files in the workspace, whether for code or non-code content.
        /// </summary>
        /// <remarks>
        /// For example a <c>README.md</c> or <c>LICENSE.md</c> markdown file could be part of the project (/repository) without synchronizing with the VBE.
        /// </remarks>
        public File[] OtherFiles { get; set; } = Array.Empty<File>();
        /// <summary>
        /// All folders in the project, whether they contain any files or not.
        /// </summary>
        public Folder[] Folders { get; set; } = Array.Empty<Folder>();
        public File[] AllFiles => Modules.Concat(OtherFiles).ToArray();
    }
}
