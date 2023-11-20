using System;
using System.Text.Json.Serialization;

namespace Rubberduck.InternalApi.Model
{
    /// <summary>
    /// An enumeration used for identifying the type of a VBA document class
    /// </summary>
    /// <remarks>
    /// Mirrors the <c>Rubberduck.Unmanaged.TypeLibs.Utility.DocClassType</c> enum type.
    /// </remarks>
    public enum DocClassType
    {
        Unrecognized = 0,
        ExcelWorkbook = 1,
        ExcelWorksheet = 2,
        AccessForm = 3,
        AccessReport = 4,
    }

    public record class ProjectFile
    {
        public const string FileName = ".rdproj";
        public const string SourceRoot = ".src";

        /// <summary>
        /// The workspace root location where the project file is.
        /// </summary>
        /// <remarks>This property is not serialized.</remarks>
        [JsonIgnore]
        public Uri Uri { get; set; }

        /// <summary>
        /// The Rubberduck version that created the file.
        /// </summary>
        public string Rubberduck { get; set; }

        /// <summary>
        /// Information about the VBA project.
        /// </summary>
        public Project VBProject { get; set; }

        public record class Project
        {
            /// <summary>
            /// The name of the project.
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// Project references.
            /// </summary>
            public Reference[] References { get; set; }
            /// <summary>
            /// Project modules <em>that can be synchronized</em> with the VBIDE.
            /// </summary>
            public Module[] Modules { get; set; }
            /// <summary>
            /// Any other files in the workspace, code or non-code content.
            /// </summary>
            /// <remarks>
            /// For example a <c>README.md</c> or <c>LICENSE.md</c> markdown file could be part of the project (/repository) without synchronizing with the VBE.
            /// </remarks>
            public Uri[] OtherFiles { get; set; }
        }

        public record class Reference
        {
            public static Reference VisualBasicForApplications { get; } = new()
            {
                Name = "VBA",
                Uri = new Uri("C:\\Program Files\\Common Files\\Microsoft Shared\\VBA\\VBA7.1\\VBE7.DLL"),
            };

            public string Name { get; set; }
            public Uri? Uri { get; set; }
            public Guid? Guid { get; set; }
            public Uri? TypeLibInfoUri { get; set; }
        }

        public record class Module
        {
            public string Name { get; set; }
            public Uri Uri { get; set; }
            public DocClassType? Super { get; set; }
        }
    }
}
