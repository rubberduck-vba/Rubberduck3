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
            Handlers = new SystemCommandHandlers(this);

            var systemCommands = new CommandBinding[]
            {
                new CommandBinding(SystemCommands.CloseWindowCommand, 
                    Handlers.CloseWindowCommandBinding_Executed, 
                    Handlers.CloseWindowCommandBinding_CanExecute),
                new CommandBinding(SystemCommands.MaximizeWindowCommand, 
                    Handlers.MaximizeWindowCommandBinding_Executed, 
                    Handlers.MaximizeWindowCommandBinding_CanExecute),
                new CommandBinding(SystemCommands.MinimizeWindowCommand, 
                    Handlers.MinimizeWindowCommandBinding_Executed, 
                    Handlers.MinimizeWindowCommandBinding_CanExecute),
                new CommandBinding(SystemCommands.RestoreWindowCommand, 
                    Handlers.RestoreWindowCommandBinding_Executed, 
                    Handlers.RestoreWindowCommandBinding_CanExecute),
                new CommandBinding(SystemCommands.ShowSystemMenuCommand,
                    Handlers.ShowSystemMenuCommandBinding_Executed,
                    Handlers.ShowSystemMenuCommandBinding_CanExecute)
            };

            var ownerType = typeof(ShellWindow);
            foreach (var commandBinding in systemCommands)
            {
                CommandManager.RegisterClassCommandBinding(ownerType, commandBinding);
            }
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            InvalidateVisual();
        }

        private SystemCommandHandlers Handlers { get; }

        private void ResizeGrip_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            var newHeight = Height + e.VerticalChange;
            var newWidth = Width + e.HorizontalChange;

            Height = Math.Max(MinHeight, newHeight);
            Width = Math.Max(MinWidth, newWidth);

            e.Handled = true;
        }
    }
}
