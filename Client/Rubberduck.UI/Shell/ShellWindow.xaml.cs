using Dragablz;
using Rubberduck.UI.Command.SharedHandlers;
using Rubberduck.UI.Windows;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Rubberduck.UI.Shell
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

            var ownerType = typeof(ShellWindow);
            DataContextChanged += OnDataContextChanged;

            LeftPaneExpander.MouseEnter += ToolPaneExpanderMouseEnter;
            RightPaneExpander.MouseEnter += ToolPaneExpanderMouseEnter;
            BottomPaneExpander.MouseEnter += ToolPaneExpanderMouseEnter;

            LeftPaneExpander.MouseLeave += ToolPaneExpanderMouseLeave;
            RightPaneExpander.MouseLeave += ToolPaneExpanderMouseLeave;
            BottomPaneExpander.MouseLeave += ToolPaneExpanderMouseLeave;
        }

        private IToolPanelViewModel GetToolPanelModel(Expander expander)
        {
            var vm = (IShellWindowViewModel)DataContext;

            if (expander == LeftPaneExpander)
            {
                return vm.LeftToolPanel;
            }
            else if (expander == RightPaneExpander)
            {
                return vm.RightToolPanel;
            }
            else if (expander == BottomPaneExpander)
            {
                return vm.BottomToolPanel;
            }

            throw new NotSupportedException();
        }

        private void ToolPaneExpanderMouseLeave(object sender, MouseEventArgs e)
        {
            if (sender is Expander expander)
            {
                var vm = GetToolPanelModel(expander);
                expander.IsExpanded = vm.IsPinned;
            }
        }

        private void ToolPaneExpanderMouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Expander expander)
            {
                var hasTools = false;
                var vm = GetToolPanelModel(expander);
                var shell = (IShellWindowViewModel)DataContext;
                hasTools = shell.ToolWindows.Any(e => e.DockingLocation == vm.PanelLocation);

                expander.IsExpanded = hasTools || e.LeftButton == MouseButtonState.Pressed;
            }
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is ICommandBindingProvider provider)
            {
                var bindings = SystemCommandHandlers.CreateCommandBindings().Concat(provider.CommandBindings).ToArray();
                CommandBindings.AddRange(bindings);
                foreach (var commandBinding in bindings)
                {
                    CommandManager.RegisterClassCommandBinding(typeof(ShellWindow), commandBinding);
                }
            }
        }

        private void OnContentRendered(object sender, EventArgs e)
        {
            InvalidateVisual();
        }

        private SystemCommandHandlers SystemCommandHandlers { get; }

        private void OnResizeGripDragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            var newHeight = Height + e.VerticalChange;
            var newWidth = Width + e.HorizontalChange;

            Height = Math.Max(MinHeight, newHeight);
            Width = Math.Max(MinWidth, newWidth);

            e.Handled = true;
        }

        private void LeftPanelToolsSource_Filter(object sender, FilterEventArgs e)
        {
            e.Accepted = false;
            if (e.Item is IToolWindowViewModel vm)
            {
                e.Accepted = vm.DockingLocation == DockingLocation.DockLeft;
            }
        }

        private void RightPanelToolsSource_Filter(object sender, FilterEventArgs e)
        {
            e.Accepted = false;
            if (e.Item is IToolWindowViewModel vm)
            {
                e.Accepted = vm.DockingLocation == DockingLocation.DockRight;
            }
        }

        private void BottomPanelToolsSource_Filter(object sender, FilterEventArgs e)
        {
            e.Accepted = false;
            if (e.Item is IToolWindowViewModel vm)
            {
                e.Accepted = vm.DockingLocation == DockingLocation.DockBottom;
            }
        }
    }
}
