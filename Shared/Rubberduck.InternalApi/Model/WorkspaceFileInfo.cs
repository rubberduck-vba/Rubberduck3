using System;

namespace Rubberduck.InternalApi.Model
{
    public record class WorkspaceFileInfo
    {
        public static WorkspaceFileInfo MissingFile(Uri uri, int version = 1)
            => new()
            { 
                Uri = uri, 
                Version = version, 
                Content = string.Empty, 
                IsMissing = true, 
            };
        public static WorkspaceFileInfo LoadError(Uri uri, int version = 1)
            => new()
            {
                Uri = uri,
                Version = version,
                Content = string.Empty,
                IsLoadError = true,
            };

        public Uri Uri { get; init; }
        public int Version { get; init; }
        public string Content { get; init; }
        public bool IsSourceFile { get; init; }
        public bool IsMissing { get; init; }
        public bool IsLoadError { get; init; }

        public bool IsOpened { get; set; }
    }
}
