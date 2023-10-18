using Rubberduck.InternalApi.Model.Abstract;
using Rubberduck.UI;
using System;

namespace Rubberduck.Editor.Document.Tabs
{
    /// <summary>
    /// The base implementation of a view model for a document tab that may contain anything.
    /// </summary>
    public abstract class DocumentTabViewModel : ViewModelBase, IDocumentTabViewModel
    {
        public DocumentTabViewModel(Uri documentUri, string language, string title, object content, bool isReadOnly)
        {
            DocumentUri = documentUri;
            Language = language;
            Title = title;
            Content = content;
            IsReadOnly = isReadOnly;
        }

        public Uri DocumentUri { get; set; }
        public string Language { get; set; }

        public string Title { get; set; }
        public object Content { get; set; }
        public bool IsReadOnly { get; set; }
    }
}
