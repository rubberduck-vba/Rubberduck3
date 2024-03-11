using OmniSharp.Extensions.LanguageServer.Protocol;
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

    public void AddOrUpdate(WorkspaceUri documentUri, TContent content)
    {
        var uri = new WorkspaceFileUri(documentUri.AbsoluteLocation.AbsolutePath, documentUri.WorkspaceRoot);
        Store.AddOrUpdate(uri.ToString().Replace("\\", "/"), content, (key, value) => content);
    }

    public bool TryRemove(WorkspaceUri documentUri)
    {
        var uri = new WorkspaceFileUri(documentUri.AbsoluteLocation.AbsolutePath, documentUri.WorkspaceRoot);
        return Store.TryRemove(uri.ToString(), out _);
    }

    /// <exception cref="UnknownUriException"></exception>
    public TContent GetDocument(WorkspaceUri documentUri)
    {
        var uri = new WorkspaceFileUri(documentUri.AbsoluteLocation.AbsolutePath, documentUri.WorkspaceRoot);
        return Store.TryGetValue(uri.ToString(), out var content)
            ? content
            : throw new UnknownUriException(uri);
    }

    public bool TryGetDocument(WorkspaceUri documentUri, out TContent? content)
    {
        var uri = new WorkspaceFileUri(documentUri.AbsoluteLocation.AbsolutePath, documentUri.WorkspaceRoot);
        var result = Store.TryGetValue(uri.ToString(), out content);

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