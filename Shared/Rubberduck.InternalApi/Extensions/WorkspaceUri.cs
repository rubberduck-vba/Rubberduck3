using Rubberduck.InternalApi.Model.Workspace;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Rubberduck.InternalApi.Extensions
{
    /// <summary>
    /// Represents a strongly-typed relative <c>Uri</c> pointing to a workspace <strong>file</strong> location.
    /// </summary>
    public class WorkspaceFileUri : WorkspaceUri
    {
        public WorkspaceFileUri([StringSyntax("Uri")] string relativeUriString, Uri workspaceRoot) 
            : base(relativeUriString, workspaceRoot) { }

        /// <summary>
        /// The absolute <c>Uri</c> location of the parent folder of the file location this <c>WorkspaceUri</c> is pointing to.
        /// </summary>
        public Uri AbsoluteFolderLocation => new(System.IO.Path.Combine(WorkspaceRoot.LocalPath, string.Join(System.IO.Path.DirectorySeparatorChar, AbsoluteLocation.LocalPath.Split(System.IO.Path.DirectorySeparatorChar)[..^1])));
        /// <summary>
        /// The relative <c>Uri</c> location of the parent folder of the file location this <c>WorkspaceUri</c> is pointing to.
        /// </summary>
        public WorkspaceFolderUri WorkspaceFolder
        {
            get
            {
                return new WorkspaceFolderUri(AbsoluteFolderLocation.LocalPath, WorkspaceRoot);
            }
        }

        /// <summary>
        /// Gets the last segment of this <c>Uri</c>, stripped of any file extension.
        /// </summary>
        public string FileNameWithoutExtension => System.IO.Path.GetFileNameWithoutExtension(FileName);
        /// <summary>
        /// Gets the last segment of this <c>Uri</c>, including any file extension.
        /// </summary>
        public string FileName => AbsoluteLocation.AbsolutePath.Split('/')[^1];
    }

    /// <summary>
    /// Represents a strongly-typed relative <c>Uri</c> pointing to a workspace <strong>folder</strong> location.
    /// </summary>
    public class WorkspaceFolderUri : WorkspaceUri
    {
        public WorkspaceFolderUri([StringSyntax("Uri")] string relativeUriString, Uri workspaceRoot) 
            : base(relativeUriString, workspaceRoot) { }

        /// <summary>
        /// Gets a name representing this workspace folder.
        /// </summary>
        public string FolderName => RelativeUriString.Trim(System.IO.Path.DirectorySeparatorChar).Split(System.IO.Path.DirectorySeparatorChar)[^1];
    }

    /// <summary>
    /// Represents a strongly-typed relative <c>Uri</c>.
    /// </summary>
    public abstract class WorkspaceUri : Uri
    {
        private readonly string _relativeUri;
        private readonly Uri _root;
        private readonly Uri _srcRoot;

        public WorkspaceUri([StringSyntax("Uri")] string relativeUriString, Uri workspaceRoot)
            : base(relativeUriString, UriKind.Relative)
        {
            _root = workspaceRoot;
            _srcRoot = new(System.IO.Path.Combine(workspaceRoot.LocalPath, ProjectFile.SourceRoot));
        }

        protected string RelativeUriString => _relativeUri;

        /// <summary>
        /// The absolute, base <c>Uri</c> for this workspace.
        /// </summary>
        public Uri WorkspaceRoot => _root;
        /// <summary>
        /// The absolute, base <c>Uri</c> for source files in this workspace.
        /// </summary>
        public Uri SourceRoot => _srcRoot;
        /// <summary>
        /// The absolute <c>Uri</c> for the project file of this workspace.
        /// </summary>
        public Uri ProjectFileUri =>new(System.IO.Path.Combine(_root.LocalPath, ProjectFile.FileName));

        /// <summary>
        /// The absolute <c>Uri</c> location this <c>WorkspaceUri</c> is pointing to.
        /// </summary>
        public virtual Uri AbsoluteLocation => new(_srcRoot.LocalPath + _relativeUri);
    }
}
