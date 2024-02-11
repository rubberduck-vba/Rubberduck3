using Rubberduck.InternalApi.Model.Workspace;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Rubberduck.InternalApi.Extensions;

/// <summary>
/// Represents a strongly-typed relative <c>Uri</c> pointing to a workspace <strong>file</strong> location.
/// </summary>
public class WorkspaceFileUri : WorkspaceUri
{
    public WorkspaceFileUri([StringSyntax("Uri")] string relativeUriString, Uri workspaceRoot) 
        : base(relativeUriString, workspaceRoot) { }

    /// <summary>
    /// The relative <c>Uri</c> location of the parent folder of the file location this <c>WorkspaceUri</c> is pointing to.
    /// </summary>
    public WorkspaceFolderUri WorkspaceFolder
    {
        get
        {
            var uri = RelativeUriString ?? throw new InvalidOperationException("♪ I went through the desert on a ..file with no name? ♪");
            return new WorkspaceFolderUri(uri[..^System.IO.Path.GetFileName(uri).Length], WorkspaceRoot);
        }
    }

    /// <summary>
    /// Gets the file name, without its extension.
    /// </summary>
    public string FileNameWithoutExtension => System.IO.Path.GetFileNameWithoutExtension(AbsoluteLocation.AbsolutePath);
    /// <summary>
    /// Gets the file name, including its extension.
    /// </summary>
    public string FileName => System.IO.Path.GetFileName(AbsoluteLocation.AbsolutePath);
}

/// <summary>
/// Represents a strongly-typed relative <c>Uri</c> pointing to a workspace <strong>folder</strong> location.
/// </summary>
public class WorkspaceFolderUri : WorkspaceUri
{
    public WorkspaceFolderUri([StringSyntax("Uri")] string? relativeUriString, Uri workspaceRoot) 
        : base(relativeUriString, workspaceRoot) 
    {
    }

    /// <summary>
    /// Gets a name representing this workspace folder.
    /// </summary>
    public string FolderName => IsSrcRoot 
        ? WorkspaceRoot.Segments[^1] // this should be the workspace name
        : System.IO.Path.GetFileName(AbsoluteLocation.AbsolutePath);
}

/// <summary>
/// Represents a strongly-typed relative <c>Uri</c>.
/// </summary>
public abstract class WorkspaceUri : Uri
{
    private readonly string? _relativeUri;
    private readonly Uri _root;
    private readonly Uri _srcRoot;

    public WorkspaceUri([StringSyntax("Uri")] string? relativeUriString, Uri workspaceRoot)
        : base(relativeUriString ?? ProjectFile.SourceRoot, UriKind.Relative)
    {
        _root = workspaceRoot;
        _srcRoot = new(System.IO.Path.Combine(workspaceRoot.LocalPath, ProjectFile.SourceRoot));
        IsSrcRoot = relativeUriString is null;

        var localRoot = _srcRoot.LocalPath;
        _relativeUri = (relativeUriString ??= ProjectFile.SourceRoot).StartsWith(localRoot)
            ? relativeUriString.Substring(localRoot.Length, relativeUriString.Length - localRoot.Length)
            : relativeUriString;
    }

    /// <summary>
    /// <c>true</c> if this <c>Uri</c> represents the source root folder of the workspace.
    /// </summary>
    public bool IsSrcRoot { get; }
    public string? RelativeUriString => _relativeUri;

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
    public virtual Uri AbsoluteLocation => IsSrcRoot ? _srcRoot : new(_srcRoot.LocalPath + _relativeUri);
}
