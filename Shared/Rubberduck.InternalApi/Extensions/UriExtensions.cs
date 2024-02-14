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
        var parentUriString = uri.OriginalString;
        var fragmentIndex = parentUriString.IndexOf('#');
        if (fragmentIndex > 0)
        {
            parentUriString = parentUriString.Replace('#', '/');
        }
        
        return new WorkspaceFileUri(parentUriString + $"#{name}", uri.WorkspaceRoot);
    }
}
