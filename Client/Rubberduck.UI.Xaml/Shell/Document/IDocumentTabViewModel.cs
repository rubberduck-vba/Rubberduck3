using System;

namespace Rubberduck.UI.Xaml.Shell.Document
{
    public interface IDocumentTabViewModel
    {
        public Uri DocumentUri { get; set; }
        public string Title { get; set; }
        public object Content { get; set; }
    }

    public class DocumentTabDesignViewModel : IDocumentTabViewModel
    {
        public Uri DocumentUri { get; set; }
        public string Title { get; set; }
        public object Content { get; set; }
    }
}
