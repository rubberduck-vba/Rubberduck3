using Rubberduck.UI.Services.WorkspaceExplorer;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Rubberduck.UI.WorkspaceExplorer
{
    public class WorkspaceNodeTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? FolderTemplate { get; set; }
        public DataTemplate? SourceFileTemplate { get; set; }
        public DataTemplate? WorkspaceFileTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            switch (item)
            {
                case WorkspaceSourceFileViewModel:
                    return SourceFileTemplate ?? throw new InvalidOperationException($"{nameof(SourceFileTemplate)} is not set.");
                case WorkspaceFileViewModel:
                    return WorkspaceFileTemplate ?? throw new InvalidOperationException($"{nameof(WorkspaceFileTemplate)} is not set.");
                case WorkspaceTreeNodeViewModel:
                    return FolderTemplate ?? throw new InvalidOperationException($"{nameof(FolderTemplate)} is not set.");
                default:
                    throw new NotSupportedException($"No template was found for type '{item.GetType().Name}'");
            }
        }
    }
}
