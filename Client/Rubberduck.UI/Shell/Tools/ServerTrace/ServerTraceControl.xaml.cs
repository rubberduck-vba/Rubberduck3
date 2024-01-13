using Rubberduck.UI.Shell.Tools.WorkspaceExplorer;
using Rubberduck.UI.Windows;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace Rubberduck.UI.Shell.Tools.ServerTrace
{

    /// <summary>
    /// Interaction logic for LanguageServerTraceControl.xaml
    /// </summary>
    public partial class ServerTraceControl : UserControl
    {
        public ServerTraceControl()
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
    }
}
