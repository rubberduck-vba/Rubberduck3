using System;
using System.Windows;
using System.Windows.Controls;

namespace Rubberduck.UI.WorkspaceExplorer
{
    public class WorkspaceNodeTemplateSelector : DataTemplateSelector
    {
        public DataTemplate? WorkspaceRootTemplate { get; set; }
        public DataTemplate? FolderTemplate { get; set; }
        public DataTemplate? SourceFileTemplate { get; set; }
        public DataTemplate? WorkspaceFileTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            switch (item)
            {
                case IWorkspaceSourceFileViewModel:
                    return SourceFileTemplate ?? throw new InvalidOperationException($"{nameof(SourceFileTemplate)} is not set.");
                case IWorkspaceFileViewModel:
                    return WorkspaceFileTemplate ?? throw new InvalidOperationException($"{nameof(WorkspaceFileTemplate)} is not set.");
                case IWorkspaceViewModel:
                    return WorkspaceRootTemplate ?? throw new InvalidOperationException($"{nameof(WorkspaceRootTemplate)} is not set.");
                case IWorkspaceTreeNode:
                    return FolderTemplate ?? throw new InvalidOperationException($"{nameof(FolderTemplate)} is not set.");
                default:
                    throw new NotSupportedException($"No template was found for type '{item.GetType().Name}'");
            }
        }
    }
}
