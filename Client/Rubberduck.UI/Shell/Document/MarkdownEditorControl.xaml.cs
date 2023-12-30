using System;
using System.Windows;
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
            DataContextChanged += OnDataContextChanged;

            Editor = (ThunderFrame.Content as DependencyObject)?.GetChildOfType<BindableTextEditor>() ?? throw new InvalidOperationException();
            Editor.TextArea.SelectionChanged += OnSelectionChanged;
            Editor.TextArea.Caret.PositionChanged += OnCaretPositionChanged;
            Editor.TextChanged += OnTextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ViewModel = (IDocumentTabViewModel)e.NewValue;
            UpdateStatusInfo();
        }

        private BindableTextEditor Editor { get; }
        private IDocumentTabViewModel ViewModel { get; set; }

        private void UpdateStatusInfo()
        {
            ViewModel.Status.CaretOffset = Editor.TextArea.Caret.Offset;
            ViewModel.Status.CaretLine = Editor.TextArea.Caret.Position.Line;
            ViewModel.Status.CaretColumn = Editor.TextArea.Caret.Position.Column;

            ViewModel.Status.DocumentLength = Editor.TextArea.Document.TextLength;
            ViewModel.Status.DocumentLines = Editor.TextArea.Document.LineCount;
        }

        private void OnCaretPositionChanged(object? sender, EventArgs e)
        {
            UpdateStatusInfo();
        }

        private void OnTextChanged(object? sender, EventArgs e)
        {
            UpdateStatusInfo();
        }

        private void OnSelectionChanged(object? sender, EventArgs e)
        {
            UpdateStatusInfo();
        }

        private void OnResizePreviewPanelDragDelta(object sender, DragDeltaEventArgs e)
        {
            var thumb = (Thumb)sender;
            var panel = (DockPanel)thumb.Parent;

            var newWidth = Math.Max(200, panel.ActualWidth - e.HorizontalChange);
            panel.Width = Math.Min(ActualWidth - 32, newWidth);

            e.Handled = true;
        }
    }
}
