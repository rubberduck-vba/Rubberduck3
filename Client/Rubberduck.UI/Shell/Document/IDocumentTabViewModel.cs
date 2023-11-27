using System;

namespace Rubberduck.UI.Shell.Document
{
    public enum SupportedDocumentType
    {
        TextDocument,
        MarkdownDocument,
        ProjectFile,
        SourceFile,
    }

    public interface IDocumentTabViewModel
    {
        public Uri DocumentUri { get; set; }
        public string Language { get; set; }

        public string Title { get; set; }
        public object Content { get; set; }

        public bool IsReadOnly { get; set; }

        public SupportedDocumentType DocumentType { get; }
    }
}
