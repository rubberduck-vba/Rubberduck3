using System;
using System.Linq;

namespace Rubberduck.Core.ProjectFile
{
    public record ProjectFile
    {
        public ProjectFile(Version rubberduck, Uri uri, Uri[] references, Uri[] components)
        {
            Rubberduck = rubberduck.ToString(3);
            Uri = uri.ToString();
            References = references.Select(e => e.ToString()).ToArray();
            Components = components.Select(e => e.ToString()).ToArray();
        }

        public string Rubberduck { get; init; }
        public string Uri { get; init; }

        public string[] References { get; init; }
        public string[] Components { get; init; }
    }
}
