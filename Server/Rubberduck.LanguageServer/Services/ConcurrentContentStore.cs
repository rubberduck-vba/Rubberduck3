using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Rubberduck.LanguageServer.Services
{
    public class DocumentState
    {
        public DocumentState(string text, int version = 0)
        {
            Text = text;
            Version = version;
        }

        public string Text { get; }
        public int Version { get; }
        public bool IsOpened { get; set; }

        public IEnumerable<FoldingRange> Foldings { get; set; } = [];
        public IEnumerable<Diagnostic> Diagnostics { get; set; } = [];
    }

    public class DocumentContentStore : ConcurrentContentStore<DocumentState> { }

    /// <summary>
    /// A service that is responsible for knowing everything about the current/latest version of all documents.
    /// </summary>
    public class ConcurrentContentStore<TContent>
    {
        private readonly ConcurrentDictionary<Uri, TContent> _store = new();
        
        public void AddOrUpdate(Uri documentUri, TContent content)
        {
            _store.AddOrUpdate(documentUri, content, (uri, content) => content);
        }

        public bool TryRemove(Uri documentUri)
        {
            return _store.TryRemove(documentUri, out _);
        }

        /// <exception cref="UnknownUriException"></exception>
        public TContent GetContent(Uri uri)
        {
            return _store.TryGetValue(uri, out var content) 
                ? content 
                : throw new UnknownUriException(uri);
        }

        public bool TryGetContent(Uri uri, out TContent? content)
        {
            return _store.TryGetValue(uri, out content);
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
}