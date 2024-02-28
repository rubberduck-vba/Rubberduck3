using Rubberduck.InternalApi.Extensions;
using System;
using System.Collections.Concurrent;

namespace Rubberduck.InternalApi.ServerPlatform.LanguageServer;

/// <summary>
/// A service that is responsible for knowing everything about the current/latest version of all documents.
/// </summary>
public class ConcurrentContentStore<TContent>
{
    protected ConcurrentDictionary<string, TContent> Store = new();

    public bool Exists(WorkspaceUri documentUri) => Store.ContainsKey(documentUri.AbsoluteLocation.LocalPath);

    public void AddOrUpdate(WorkspaceUri documentUri, TContent content)
    {
        Store.AddOrUpdate(documentUri.AbsoluteLocation.LocalPath, content, (key, value) => content);
    }

    public bool TryRemove(WorkspaceUri documentUri)
    {
        return Store.TryRemove(documentUri.AbsoluteLocation.LocalPath, out _);
    }

    /// <exception cref="UnknownUriException"></exception>
    public TContent GetDocument(WorkspaceUri uri)
    {
        return Store.TryGetValue(uri.AbsoluteLocation.LocalPath, out var content)
            ? content
            : throw new UnknownUriException(uri);
    }

    public bool TryGetDocument(WorkspaceUri uri, out TContent? content)
    {
        return Store.TryGetValue(uri.AbsoluteLocation.LocalPath, out content);
    }
}

public class UnknownUriException : ApplicationException
{
    public UnknownUriException(Uri uri)
    {
        UnknownUri = uri;
    }

    public Uri UnknownUri { get; }
}