using Rubberduck.UI.Command;
using Rubberduck.UI.Shell;
using System;
using System.Windows;
using System.Windows.Input;

namespace Rubberduck.Editor.Shell
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ShellWindow : Window
    {
        public ShellWindow()
        {
            InitializeComponent();
            SystemCommandHandlers = new SystemCommandHandlers(this);
            var systemCommands = new CommandBinding[]
            {
                new(SystemCommands.CloseWindowCommand, 
                    SystemCommandHandlers.CloseWindowCommandBinding_Executed, SystemCommandHandlers.CloseWindowCommandBinding_CanExecute),
                new(SystemCommands.MaximizeWindowCommand, 
                    SystemCommandHandlers.MaximizeWindowCommandBinding_Executed, SystemCommandHandlers.MaximizeWindowCommandBinding_CanExecute),
                new(SystemCommands.MinimizeWindowCommand, 
                    SystemCommandHandlers.MinimizeWindowCommandBinding_Executed, SystemCommandHandlers.MinimizeWindowCommandBinding_CanExecute),
                new(SystemCommands.RestoreWindowCommand, 
                    SystemCommandHandlers.RestoreWindowCommandBinding_Executed, SystemCommandHandlers.RestoreWindowCommandBinding_CanExecute),
                new(SystemCommands.ShowSystemMenuCommand,
                    SystemCommandHandlers.ShowSystemMenuCommandBinding_Executed,SystemCommandHandlers.ShowSystemMenuCommandBinding_CanExecute)
            };

            var ownerType = typeof(ShellWindow);
            foreach (var commandBinding in systemCommands)
            {
                CommandManager.RegisterClassCommandBinding(ownerType, commandBinding);
            }

            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            foreach(var commandBinding in ViewModel.FileCommandHandlers.CreateCommandBindings())
            {
                CommandManager.RegisterClassCommandBinding(typeof(ShellWindow), commandBinding);
            }
        }

        private void OnContentRendered(object sender, EventArgs e)
        {
            InvalidateVisual();
        }

        private SystemCommandHandlers SystemCommandHandlers { get; }
        private ShellWindowViewModel ViewModel => (ShellWindowViewModel)DataContext;

        private void OnResizeGripDragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            var newHeight = Height + e.VerticalChange;
            var newWidth = Width + e.HorizontalChange;

            Height = Math.Max(MinHeight, newHeight);
            Width = Math.Max(MinWidth, newWidth);

            e.Handled = true;
        }
    }
}
