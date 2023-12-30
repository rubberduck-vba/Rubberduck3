using System.Windows;
using System.Windows.Controls;

namespace Rubberduck.UI.Shell.StatusBar
{
    public class DocumentStatusTemplateSelector : DataTemplateSelector
    {
        public DataTemplate SourceFileTemplate { get; set; }
        public DataTemplate TextFileTemplate { get; set; }
        public DataTemplate DefaultDocumentTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return item switch
            {
                ISourceFileStatusViewModel => SourceFileTemplate,
                IDocumentStatusViewModel => TextFileTemplate,
                null => null,
                _ => DefaultDocumentTemplate
            };
        }
    }
}
