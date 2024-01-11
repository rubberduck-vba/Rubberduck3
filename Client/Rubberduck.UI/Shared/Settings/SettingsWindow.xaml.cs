using Rubberduck.UI.Command.SharedHandlers;
using System;
using System.Linq;
using System.Windows;

namespace Rubberduck.UI.Shared.Settings
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow(ISettingsWindowViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }

        public SettingsWindow()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is ICommandBindingProvider provider)
            {
                var systemHandlers = new SystemCommandHandlers(this);
                CommandBindings.Clear();
                CommandBindings.AddRange(systemHandlers.CreateCommandBindings().Concat(provider.CommandBindings).ToArray());
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
