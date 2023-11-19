using Ookii.Dialogs.Wpf;
using Rubberduck.UI.Command;
using Rubberduck.UI.Shell;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Rubberduck.UI.NewProject
{
    /// <summary>
    /// Interaction logic for NewProjectWindow.xaml
    /// </summary>
    public partial class NewProjectWindow : Window
    {
        public NewProjectWindow(NewProjectWindowViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }

        private static readonly ExecutedRoutedEventHandler _browseCommandExecute = DialogCommandHandlers.BrowseLocationCommandBinding_Executed;
        private static readonly CanExecuteRoutedEventHandler _browseCommandCanExecute = DialogCommandHandlers.BrowseLocationCommandBinding_CanExecute;

        private readonly SystemCommandHandlers _systemCommandHandlers;

        public NewProjectWindow()
        {
            InitializeComponent();
            MouseDown += OnMouseDown;

            _systemCommandHandlers = new SystemCommandHandlers(this);

            var bindings = new CommandBinding[]
            {
                new(SystemCommands.CloseWindowCommand, _systemCommandHandlers.CloseWindowCommandBinding_Executed, _systemCommandHandlers.CloseWindowCommandBinding_CanExecute),
                new(NavigationCommands.Search, _browseCommandExecute, _browseCommandCanExecute),
            };

            CommandBindings.AddRange(bindings);
        }

        private NewProjectWindowViewModel ViewModel => (NewProjectWindowViewModel)DataContext;

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }
    }
}
