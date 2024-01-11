using Rubberduck.UI.Shell;
using Rubberduck.UI.Windows;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace Rubberduck.UI.Shell.Tools.WorkspaceExplorer
{
    /// <summary>
    /// Interaction logic for WorkspaceExplorerControl.xaml
    /// </summary>
    public partial class WorkspaceExplorerControl : UserControl
    {
        public WorkspaceExplorerControl()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is ITabViewModel vm)
            {
                vm.ContentControl = this;
            }

            if (e.NewValue is ICommandBindingProvider provider)
            {
                var bindings = provider.CommandBindings.ToArray();
                CommandBindings.AddRange(bindings);
                foreach (var commandBinding in bindings)
                {
                    CommandManager.RegisterClassCommandBinding(typeof(WorkspaceExplorerControl), commandBinding);
                }
            }

            InvalidateVisual();
        }

        private void OnFileDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var uri = ((DataContext as IWorkspaceExplorerViewModel)?.Selection)?.Uri;
            if (uri != null)
            {
                WorkspaceExplorerCommands.OpenFileCommand.Execute(uri, this);
            }
        }
    }
}
