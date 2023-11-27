using Rubberduck.UI.Command;
using Rubberduck.UI.Services.Settings;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Rubberduck.UI.Settings
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : System.Windows.Window
    {
        public SettingsWindow(ISettingsWindowViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }

        public SettingsWindow()
        {
            InitializeComponent();

            MouseDown += OnMouseDown;
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is ICommandBindingProvider provider)
            {
                CommandBindings.Clear();
                CommandBindings.AddRange(provider.CommandBindings.ToArray());
            }
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
