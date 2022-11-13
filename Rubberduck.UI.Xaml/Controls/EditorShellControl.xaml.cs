using ICSharpCode.AvalonEdit.Folding;
using Rubberduck.UI.RubberduckEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace Rubberduck.UI.Xaml.Controls
{
    /// <summary>
    /// Interaction logic for EditorControl.xaml
    /// </summary>
    public partial class EditorShellControl : UserControl
    {
        public IFoldingStrategy FoldingStrategy { get; } = new VBFoldingStrategy();
        public IEnumerable<IBlockCompletionStrategy> BlockCompletionStrategies { get; } 
            = VBFoldingStrategy.BlockInfo.Values.Select(e => new BlockCompletionStrategy(e)).ToList();

        private readonly FoldingManager _foldingManager;
        private readonly BlockCompletion _blockCompletion;

        public EditorShellControl(IEnumerable<IBlockCompletionStrategy> blockCompletionStrategies) : this()
        {
            BlockCompletionStrategies = blockCompletionStrategies;
        }

        public EditorShellControl()
        {
            InitializeComponent();

            EditorPane.TextChanged += EditorPane_TextChanged;
            _foldingManager = FoldingManager.Install(EditorPane.TextArea);
            _blockCompletion = new BlockCompletion(BlockCompletionStrategies);
        }

        private void EditorPane_TextChanged(object sender, EventArgs e)
        {
            FoldingStrategy?.UpdateFoldings(_foldingManager, EditorPane.Document);
            if (_blockCompletion.CanComplete(EditorPane.TextArea.Caret, EditorPane.Document, _foldingManager, out var completionStrategy, out var text))
            {
                completionStrategy.Complete(EditorPane.TextArea.Caret, text, EditorPane.Document);
            }
        }
    }
}
