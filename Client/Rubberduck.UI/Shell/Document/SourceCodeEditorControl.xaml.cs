using ICSharpCode.AvalonEdit.Folding;
using Rubberduck.UI.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Rubberduck.UI.Shell.Document
{
    /// <summary>
    /// Interaction logic for SourceCodeEditorControl.xaml
    /// </summary>
    public partial class SourceCodeEditorControl : UserControl
    {
        private FoldingManager _foldings;

        public SourceCodeEditorControl()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;

            Editor = (ThunderFrame.Content as DependencyObject)?.GetChildOfType<BindableTextEditor>() ?? throw new InvalidOperationException();
            Editor.TextArea.SelectionChanged += OnSelectionChanged;
            Editor.TextArea.Caret.PositionChanged += OnCaretPositionChanged;
            Editor.TextChanged += OnTextChanged;

            _foldings = FoldingManager.Install(Editor.TextArea);
        }

        private async void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            await Task.Run(async () =>
            {
                ViewModel = (IDocumentTabViewModel)e.NewValue;
                if (ViewModel.DocumentType == SupportedDocumentType.SourceFile)
                {
                    var foldings = await ViewModel.RequestFoldingsAsync();
                    await Dispatcher.InvokeAsync(() =>
                    {
                        _foldings.Clear();
                        _foldings.UpdateFoldings(foldings.Select(e => e.ToNewFolding(Editor.Document)).OrderBy(e => e.StartOffset), -1); // TODO account for syntax errors instead of passing -1?
                    });
                }
                UpdateStatusInfo();
            });
        }

        private BindableTextEditor Editor { get; }
        private IDocumentTabViewModel ViewModel { get; set; } = default!;

        private void UpdateStatusInfo()
        {
            Dispatcher.Invoke(() =>
            {
                ViewModel.Status.CaretOffset = Editor.TextArea.Caret.Offset;
                ViewModel.Status.CaretLine = Editor.TextArea.Caret.Position.Line;
                ViewModel.Status.CaretColumn = Editor.TextArea.Caret.Position.Column;

                ViewModel.Status.DocumentLength = Editor.TextArea.Document.TextLength;
                ViewModel.Status.DocumentLines = Editor.TextArea.Document.LineCount;
            });
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
