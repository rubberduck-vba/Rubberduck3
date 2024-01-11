using Rubberduck.UI.Shell.Document;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Rubberduck.Editor.Shell.Document
{
    public class DocumentTabTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? TextDocumentTemplate { get; set; }
        public DataTemplate? MarkdownDocumentTemplate { get; set; }
        public DataTemplate? CodeDocumentTemplate { get; set; }
        public DataTemplate? ProjectDocumentTemplate { get; set; }

        public override DataTemplate? SelectTemplate(object item, DependencyObject container)
        {
            DataTemplate? result = null;
            if (item is IDocumentTabViewModel vm)
            {
                result = vm.DocumentType switch
                {
                    SupportedDocumentType.TextDocument => TextDocumentTemplate,
                    SupportedDocumentType.MarkdownDocument => MarkdownDocumentTemplate,
                    SupportedDocumentType.SourceFile => CodeDocumentTemplate,
                    SupportedDocumentType.ProjectFile => ProjectDocumentTemplate,
                    _ => base.SelectTemplate(item, container)
                } ?? throw new NotSupportedException();
            }

            return result;
        }
    }
}
