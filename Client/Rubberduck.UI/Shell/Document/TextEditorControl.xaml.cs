using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Rubberduck.UI.Shell.Document
{
    /// <summary>
    /// Interaction logic for TextEditorControl.xaml
    /// </summary>
    public partial class TextEditorControl : UserControl
    {
        public TextEditorControl()
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
    }
}
