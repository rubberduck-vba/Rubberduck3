using Rubberduck.UI.Command.SharedHandlers;
using System.Linq;
using System.Windows;

namespace Rubberduck.UI.Shared.NewProject
{
    /// <summary>
    /// Interaction logic for NewProjectWindow.xaml
    /// </summary>
    public partial class NewProjectWindow : Window
    {
        public NewProjectWindow(INewProjectWindowViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }

        public NewProjectWindow()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var systemHandlers = new SystemCommandHandlers(this);
            if (e.NewValue is ICommandBindingProvider provider)
            {
                CommandBindings.Clear();
                CommandBindings.AddRange(systemHandlers.CreateCommandBindings().Concat(provider.CommandBindings).ToArray());
            }
        }
    }
}
