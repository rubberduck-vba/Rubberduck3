using Rubberduck.UI.Shell;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Rubberduck.Editor.Shell
{
    /// <summary>
    /// Interaction logic for MenuBarControl.xaml
    /// </summary>
    public partial class MenuBarControl : UserControl
    {
        public MenuBarControl()
        {
            InitializeComponent();
            DataContextChanged += MenuBarControl_DataContextChanged;
        }

        private void MenuBarControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Handlers = new ApplicationCommandHandlers(ViewModel.NewProjectCommand);

            var bindings = new CommandBinding[]
            {
                new CommandBinding(ApplicationCommands.New,
                    Handlers.NewProjectCommandBinding_Executed,
                    Handlers.NewProjectCommandBinding_CanExecute),
            };

            var ownerType = typeof(Window);
            foreach (var binding in bindings)
            {
                CommandManager.RegisterClassCommandBinding(ownerType, binding);
            }
        }

        private ApplicationCommandHandlers Handlers { get; set; }
        private ShellWindowViewModel ViewModel => (ShellWindowViewModel)DataContext;
    }
}
