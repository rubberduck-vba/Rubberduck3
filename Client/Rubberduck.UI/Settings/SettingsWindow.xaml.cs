using Rubberduck.UI.Shell;
using System;
using System.Windows;
using System.Windows.Input;

namespace Rubberduck.UI.Settings
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private readonly SystemCommandHandlers _systemCommandHandlers;

        public SettingsWindow(ISettingsWindowViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }

        public SettingsWindow()
        {
            InitializeComponent();
            MouseDown += OnMouseDown;

            _systemCommandHandlers = new SystemCommandHandlers(this);

            var bindings = new CommandBinding[]
            {
                new(SystemCommands.CloseWindowCommand, _systemCommandHandlers.CloseWindowCommandBinding_Executed, _systemCommandHandlers.CloseWindowCommandBinding_CanExecute),
                //new(NavigationCommands.Search, _filterCommandExecute, _filterCommandCanExecute),
            };

            CommandBindings.AddRange(bindings);
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        private void OnResizeGripDragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            var newHeight = Height + e.VerticalChange;
            var newWidth = Width + e.HorizontalChange;

            Height = Math.Min(MaxHeight, Math.Max(MinHeight, newHeight));
            Width = Math.Min(MaxWidth, Math.Max(MinWidth, newWidth));

            e.Handled = true;
        }
    }
}
