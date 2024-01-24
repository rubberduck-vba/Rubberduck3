﻿using Dragablz;
using Rubberduck.UI.Command.SharedHandlers;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
                var vm = (IShellWindowViewModel)DataContext;
                var toolPanel = GetToolPanelModel(expander);
                var toolwindows = vm.LeftPanelToolWindows 
                    .Concat(vm.RightPanelToolWindows)
                    .Concat(vm.BottomPanelToolWindows)
                    .Where(e => e.DockingLocation == toolPanel.PanelLocation);

                toolPanel.IsPinned = toolwindows.Any(e => e.IsPinned);
                expander.IsExpanded = toolPanel.IsPinned;
            }
        }

        private void ToolPaneExpanderMouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Expander expander)
            {
                var hasTools = false;
                var vm = GetToolPanelModel(expander);
                var shell = (IShellWindowViewModel)DataContext;
                hasTools = shell.LeftPanelToolWindows
                    .Concat(shell.RightPanelToolWindows)
                    .Concat(shell.BottomPanelToolWindows).Any(e => e.DockingLocation == vm.PanelLocation);

                expander.IsExpanded = hasTools || e.LeftButton == MouseButtonState.Pressed; // FIXME not just pressed, but dragging something
            }
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is IShellWindowViewModel vm)
            {
                ((InterTabController)Resources["InterToolTabController"]).InterTabClient = vm.InterToolTabClient;
                ((InterTabController)Resources["InterTabController"]).InterTabClient = vm.InterTabClient;
            }
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

        private void OnResizeGripDragDelta(object sender, DragDeltaEventArgs e)
        {
            var newHeight = Height + e.VerticalChange;
            var newWidth = Width + e.HorizontalChange;

            Height = Math.Max(MinHeight, newHeight);
            Width = Math.Max(MinWidth, newWidth);

            e.Handled = true;
        }

        private void OnResizeLeftPanelDragDelta(object sender, DragDeltaEventArgs e)
        {
            var thumb = (Thumb)sender;
            var panel = (DockPanel)thumb.Parent;

            var newWidth = Math.Max(100, panel.ActualWidth + e.HorizontalChange);
            panel.Width = Math.Min(ActualWidth - 32, newWidth);

            e.Handled = true;
        }

        private void OnResizeRightPanelDragDelta(object sender, DragDeltaEventArgs e)
        {
            var thumb = (Thumb)sender;
            var panel = (DockPanel)thumb.Parent;

            var newWidth = Math.Max(100, panel.ActualWidth - e.HorizontalChange);
            panel.Width = Math.Min(ActualWidth - 32, newWidth);

            e.Handled = true;
        }

        private void OnResizeBottomPanelDragDelta(object sender, DragDeltaEventArgs e)
        {
            var thumb = (Thumb)sender;
            var panel = (DockPanel)thumb.Parent;

            var newHeight = Math.Max(100, panel.ActualHeight - e.VerticalChange);
            panel.Height = Math.Min(ActualHeight - 32, newHeight);

            e.Handled = true;
        }

        private void OnToolPanelExpanderDrop(object sender, DragEventArgs e)
        {
            var data = e.Data.GetData(typeof(object));
        }

        private void OnToolPanelExpanderPreviewDrop(object sender, DragEventArgs e)
        {
            var data = e.Data.GetData(typeof(object));
        }
    }
}
