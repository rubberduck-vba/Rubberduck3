using Rubberduck.InternalApi.Extensions;
using System;
using System.Linq;

namespace Rubberduck.InternalApi.Model.Workspace
{
    public record class WorkspaceFileInfo
    {
        public static WorkspaceFileInfo MissingFile(WorkspaceFileUri uri)
            => new()
            {
                Uri = uri,
                Content = string.Empty,
                IsMissing = true,
            };
        public static WorkspaceFileInfo LoadError(WorkspaceFileUri uri)
            => new()
            {
                Uri = uri,
                Content = string.Empty,
                IsLoadError = true,
            };

        public WorkspaceFileUri Uri { get; init; } = default!;
        public string FileExtension => System.IO.Path.GetExtension(Uri.FileName);
        public string Name => Uri.FileNameWithoutExtension;
        public string Content { get; set; } = string.Empty;

        private string _originalContent = string.Empty;
        public string OriginalContent => _originalContent;

        private bool _isModified;
        public bool IsModified => _isModified = _isModified || Content != OriginalContent;
        public bool IsSourceFile { get; init; }
        public bool IsMissing { get; init; }
        public bool IsLoadError { get; init; }
        public bool IsOpened { get; set; }

        public void ResetModifiedState()
        {
            _originalContent = Content;
            _isModified = false;
        }
    }
}
