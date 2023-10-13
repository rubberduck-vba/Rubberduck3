using Rubberduck.InternalApi.Model.Abstract;
using System;

namespace Rubberduck.Editor.Document.Tabs
{
    public abstract class DocumentTabViewModel : ViewModelBase, IDocumentTabViewModel
    {

        public DocumentTabViewModel(Uri documentUri, string title, object content)
        {
            DocumentUri = documentUri;
            Title = title;
            Content = content;
        }

        public Uri DocumentUri { get; set; }
        public string Title { get; set; }
        public object Content { get; set; }
    }
}
