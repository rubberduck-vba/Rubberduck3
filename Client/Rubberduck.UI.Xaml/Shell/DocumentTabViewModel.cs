using System;

namespace Rubberduck.UI.Xaml.Shell
{
    public abstract class DocumentTabViewModel : ViewModelBase
    {

        public DocumentTabViewModel(Uri documentUri, string title, string content)
        {
            DocumentUri = documentUri;
            Title = title;
            Content = content;
        }

        public Uri DocumentUri { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }
}
