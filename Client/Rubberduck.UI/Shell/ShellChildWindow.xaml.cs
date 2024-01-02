using Dragablz;
using Rubberduck.UI.Windows;
using System;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace Rubberduck.UI.Shell
{
    public partial class ShellChildWindow : Window, IShellChildWindow
    {
        public ShellChildWindow(IDragablzWindowViewModel vm) : this()
        {
            DataContext = vm;
        }

        public TabablzControl Tabs => DocumentPaneTabs;

        public ShellChildWindow()
        {
            InitializeComponent();
        }

        private void OnResizeGripDragDelta(object sender, DragDeltaEventArgs e)
        {
            var newHeight = Height + e.VerticalChange;
            var newWidth = Width + e.HorizontalChange;

            Height = Math.Max(MinHeight, newHeight);
            Width = Math.Max(MinWidth, newWidth);

            e.Handled = true;
        }

        private void OnContentRendered(object sender, EventArgs e)
        {
            InvalidateVisual();
        }
    }
}
