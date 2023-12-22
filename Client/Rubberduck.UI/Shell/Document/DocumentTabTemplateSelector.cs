using System;
using System.Windows;
using System.Windows.Controls;

namespace Rubberduck.UI.Shell.Document
{
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
                        return TextDocumentTemplate;
                    case SupportedDocumentType.MarkdownDocument:
                        return MarkdownDocumentTemplate;
                    case SupportedDocumentType.ProjectFile:
                        return ProjectFileTemplate;
                    case SupportedDocumentType.SourceFile:
                        return SourceFileTemplate;
                    default:
                        throw new NotSupportedException($"IDocumentTabViewModel.DocumentType value '{vm.DocumentType}' is not supported.");
                }
            }

            throw new NotSupportedException($"item type {item?.GetType().Name ?? "null"} does not implement IDocumentTabViewModel.");
        }
    }
}
