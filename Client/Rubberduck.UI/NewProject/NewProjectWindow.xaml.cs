using Rubberduck.UI.Command;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Rubberduck.UI.NewProject
{
    /// <summary>
    /// Interaction logic for NewProjectWindow.xaml
    /// </summary>
    public partial class NewProjectWindow : System.Windows.Window
    {
        public NewProjectWindow(INewProjectWindowViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }

        public NewProjectWindow()
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
    }
}
