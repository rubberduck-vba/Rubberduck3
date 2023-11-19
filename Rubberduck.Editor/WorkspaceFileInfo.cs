using System;

namespace Rubberduck.Editor
{
    public record class WorkspaceFileInfo
    {
        public Uri Uri { get; init; }
        public int Version { get; init; }
        public string Content { get; init; }
        public bool IsSourceFile { get; init; }
        public bool IsMissing { get; init; }
        public bool IsLoadError { get; init; }

        public bool IsOpened { get; set; }
    }
}
