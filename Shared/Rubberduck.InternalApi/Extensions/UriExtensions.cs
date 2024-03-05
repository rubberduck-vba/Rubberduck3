using OmniSharp.Extensions.LanguageServer.Protocol;
using System;

namespace Rubberduck.InternalApi.Extensions;

public static class UriExtensions
{
    /// <summary>
    /// Gets a URI for a child symbol of the symbol at the provided parent URI.
    /// </summary>
    /// <remarks>
    /// If the URI has a '#' fragment, it becomes a '/' segment and the provided <c>name</c> becomes the fragment segment.
    /// </remarks>
    public static WorkspaceFileUri GetChildSymbolUri(this WorkspaceUri uri, string name)
    {
        var parentUriString = System.IO.Path.Combine(
            System.IO.Path.GetDirectoryName(uri.OriginalString) ?? string.Empty,
            System.IO.Path.GetFileNameWithoutExtension(uri.OriginalString));

        var fragmentIndex = parentUriString.IndexOf('#');
        if (fragmentIndex > 0)
        {
            parentUriString = parentUriString.Replace('#', '/');
        }
        
        return new WorkspaceFileUri(parentUriString + $"#{name}", uri.WorkspaceRoot);
    }

    public static WorkspaceFileUri AsWorkspaceUri(this DocumentUri uri, WorkspaceUri root)
    {
        return new WorkspaceFileUri(uri.ToUri().OriginalString, root.WorkspaceRoot);
    }
}
