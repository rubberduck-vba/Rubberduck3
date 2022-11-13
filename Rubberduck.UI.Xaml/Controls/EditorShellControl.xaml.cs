using ICSharpCode.AvalonEdit.Folding;
using Rubberduck.UI.RubberduckEditor;
using System;
using System.Windows.Controls;

namespace Rubberduck.UI.Xaml.Controls
{
    /// <summary>
    /// Interaction logic for EditorControl.xaml
    /// </summary>
    public partial class EditorShellControl : UserControl
    {
        public IFoldingStrategy FoldingStrategy { get; } = new VBFoldingStrategy();

        private readonly FoldingManager _foldingManager;

        public EditorShellControl()
        {
            InitializeComponent();

            EditorPane.TextChanged += EditorPane_TextChanged;
            _foldingManager = FoldingManager.Install(EditorPane.TextArea);
        }

        private void EditorPane_TextChanged(object sender, EventArgs e)
        {
            FoldingStrategy?.UpdateFoldings(_foldingManager, EditorPane.Document);
        }
    }
}
