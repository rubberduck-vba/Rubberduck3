using OmniSharp.Extensions.LanguageServer.Protocol;
using Rubberduck.InternalApi.Extensions;
using System;
using System.Collections.Concurrent;

namespace Rubberduck.InternalApi.ServerPlatform.LanguageServer;

public class WorkspaceFileUriEventArgs : EventArgs
{
    public WorkspaceFileUriEventArgs(WorkspaceFileUri uri)
    {
        Uri = uri;
    }

    public WorkspaceFileUri Uri { get; }
}

/// <summary>
/// A service that is responsible for knowing everything about the current/latest version of all documents.
/// </summary>
public class ConcurrentContentStore<TContent>
{
    protected ConcurrentDictionary<WorkspaceUri, TContent> Store = new();
    public event EventHandler<WorkspaceFileUriEventArgs> DocumentStateChanged = delegate { };

    public void AddOrUpdate(WorkspaceFileUri uri, TContent content)
    {
        Store.AddOrUpdate(uri, content, (key, value) => content);
        DocumentStateChanged.Invoke(this, new(uri));
    }

    public void AddOrUpdate(WorkspaceUri documentUri, TContent content)
    {
        var uri = new WorkspaceFileUri(documentUri.AbsoluteLocation.AbsolutePath, documentUri.WorkspaceRoot);
        Store.AddOrUpdate(uri, content, (key, value) => content);

        DocumentStateChanged.Invoke(this, new(uri));
    }

    public bool TryRemove(WorkspaceFileUri uri)
    {
        var removed = Store.TryRemove(uri, out _);

        DocumentStateChanged.Invoke(this, new(uri));
        return removed;
    }
    public bool TryRemove(WorkspaceUri documentUri)
    {
        var uri = new WorkspaceFileUri(documentUri.AbsoluteLocation.AbsolutePath, documentUri.WorkspaceRoot);
        var removed = Store.TryRemove(uri, out _);

        DocumentStateChanged.Invoke(this, new(uri));
        return removed;
    }

    /// <exception cref="UnknownUriException"></exception>
    public TContent GetDocument(WorkspaceFileUri uri)
    {
        return Store.TryGetValue(uri, out var content)
            ? content : throw new UnknownUriException(uri);
    }

    /// <exception cref="UnknownUriException"></exception>
    public TContent GetDocument(WorkspaceUri documentUri)
    {
        var uri = new WorkspaceFileUri(documentUri.AbsoluteLocation.AbsolutePath, documentUri.WorkspaceRoot);
        return Store.TryGetValue(uri, out var content)
            ? content : throw new UnknownUriException(uri);
    }

    public bool TryGetDocument(WorkspaceUri documentUri, out TContent? content)
    {
        var uri = new WorkspaceFileUri(Uri.UnescapeDataString(documentUri.AbsoluteLocation.AbsolutePath), documentUri.WorkspaceRoot);
        var result = Store.TryGetValue(uri, out content);

        return result;
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