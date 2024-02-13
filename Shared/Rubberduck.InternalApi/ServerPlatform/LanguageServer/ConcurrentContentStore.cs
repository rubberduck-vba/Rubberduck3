using System;
using System.Collections.Concurrent;

namespace Rubberduck.InternalApi.ServerPlatform.LanguageServer;

/// <summary>
/// A service that is responsible for knowing everything about the current/latest version of all documents.
/// </summary>
public class ConcurrentContentStore<TContent>
{
    protected ConcurrentDictionary<Uri, TContent> Store = new();

    public bool Exists(Uri documentUri) => Store.ContainsKey(documentUri);

    public void AddOrUpdate(Uri documentUri, TContent content)
    {
        Store.AddOrUpdate(documentUri, content, (uri, content) => content);
    }

    public bool TryRemove(Uri documentUri)
    {
        return Store.TryRemove(documentUri, out _);
    }

    /// <exception cref="UnknownUriException"></exception>
    public TContent GetDocument(Uri uri)
    {
        return Store.TryGetValue(uri, out var content)
            ? content
            : throw new UnknownUriException(uri);
    }

    public bool TryGetDocument(Uri uri, out TContent? content)
    {
        return Store.TryGetValue(uri, out content);
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