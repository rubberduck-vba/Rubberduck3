using Rubberduck.Unmanaged.TypeLibs.Utility;
using System;
using System.Text.Json.Serialization;

namespace Rubberduck.Editor.ProjectFile
{
    public record class ProjectFile
    {
        public ProjectFile(Version rubberduck, Uri uri, Project vBProject)
        {
            Rubberduck = rubberduck.ToString(3);
            Uri = uri;
            VBProject = vBProject;
        }

        public ProjectFile(Version rubberduck, Uri uri, string name, Reference[] references, Module[] modules)
        {
            Rubberduck = rubberduck.ToString(3);
            Uri = uri;
            VBProject = new()
            {
                Name = name,
                References = references,
                Modules = modules,
            };
        }

        public string Rubberduck { get; init; }

        [JsonIgnore]
        public Uri Uri { get; init; }

        public Project VBProject { get; init; }

        public record class Project
        {
            public string Name { get; init; }
            public Reference[] References { get; init; }
            public Module[] Modules { get; init; }
        }

        public record class Reference
        {
            public string Name { get; init; }
            public Uri Uri { get; init; }
            public Guid Guid { get; init; } = Guid.Empty;
            public Uri? TypeLibInfoUri { get; init; } = null;
        }

        public record class Module
        {
            public string Name { get; init; }
            public DocClassType? Super { get; init; }
            public Uri Uri { get; init; }
        }
    }
}
