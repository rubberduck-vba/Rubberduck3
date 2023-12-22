using System;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Rubberduck.UI.Shell.Document
{
    /// <summary>
    /// Interaction logic for MarkdownEditorControl.xaml
    /// </summary>
    public partial class MarkdownEditorControl : UserControl
    {
        public MarkdownEditorControl()
        {
            InitializeComponent();
        }

        private void OnResizePreviewPanelDragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            var thumb = (Thumb)sender;
            var panel = (DockPanel)thumb.Parent;

            var newWidth = Math.Max(200, panel.ActualWidth - e.HorizontalChange);
            panel.Width = Math.Min(ActualWidth - 32, newWidth);

            e.Handled = true;
        }
    }
}
