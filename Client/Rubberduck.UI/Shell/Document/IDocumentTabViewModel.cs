using Rubberduck.UI.Windows;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Rubberduck.UI.Shell.Document
{
    public enum SupportedDocumentType
    {
        /// <summary>
        /// A plain text document, with encoding options.
        /// </summary>
        TextDocument,
        /// <summary>
        /// A markdown-formatted text document.
        /// </summary>
        MarkdownDocument,
        /// <summary>
        /// A JSON specification for a Rubberduck project.
        /// </summary>
        ProjectFile,
        /// <summary>
        /// A text file that gets synchronized with the VBE as part of the host's corresponding VBA project.
        /// </summary>
        SourceFile,
    }

    public interface IDocumentTabViewModel : ITabViewModel
    {
        public Uri DocumentUri { get; set; }
        public string Language { get; set; }
        public string Title { get; set; }
        public bool IsReadOnly { get; set; }

        public SupportedDocumentType DocumentType { get; }
    }

    public class DocumentTabTemplateSelector : DataTemplateSelector
    {
        public DataTemplate TextDocumentTemplate { get; set; } = new();
        public DataTemplate MarkdownDocumentTemplate { get; set; } = new();
        public DataTemplate ProjectFileTemplate { get; set; } = new();
        public DataTemplate SourceFileTemplate { get; set; } = new();

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is IDocumentTabViewModel vm)
            {
                switch (vm.DocumentType)
                {
                    case SupportedDocumentType.TextDocument:
                        break;
                    case SupportedDocumentType.MarkdownDocument:
                        break;
                    case SupportedDocumentType.ProjectFile:
                        break;
                    case SupportedDocumentType.SourceFile:
                        break;
                    default:
                        throw new NotSupportedException($"IDocumentTabViewModel.DocumentType value '{vm.DocumentType}' is not supported.");
                }
            }

            throw new NotSupportedException($"item type {item?.GetType().Name ?? "null"} does not implement IDocumentTabViewModel.");
        }
    }
}
