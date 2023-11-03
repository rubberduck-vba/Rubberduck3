using System;
using System.Windows;
using System.Windows.Controls;

namespace Rubberduck.Editor.Document.Tabs
{
    public class DocumentTabTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? TextDocumentTemplate { get; set; }
        public DataTemplate? MarkdownDocumentTemplate { get; set; }
        public DataTemplate? CodeDocumentTemplate { get; set; }
        public DataTemplate? ProjectDocumentTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return item switch
            {
                TextDocumentTabViewModel => TextDocumentTemplate,
                MarkdownDocumentTabViewModel => MarkdownDocumentTemplate,
                RubberduckProjectDocumentTabViewModel => ProjectDocumentTemplate,
                VB6CodeDocumentTabViewModel => CodeDocumentTemplate,
                VBACodeDocumentTabViewModel => CodeDocumentTemplate,
                _ => base.SelectTemplate(item, container)
            } ?? throw new NotSupportedException($"Received unsupported item type: '{item?.GetType().Name ?? "null"}'.");
        }
    }
}
