using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Folding;
using Rubberduck.UI.RubberduckEditor;
using Rubberduck.UI.RubberduckEditor.TextTransform;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;

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
        private readonly BlockCompletionService _blockCompletion;
        private readonly ITextMarkerService _textMarkerService;

        public EditorShellControl(IEnumerable<IBlockCompletionStrategy> blockCompletionStrategies) : this()
        {
            BlockCompletionStrategies = blockCompletionStrategies;
        }

        public EditorShellControl()
        {
            InitializeComponent();

            EditorPane.TextChanged += EditorPane_TextChanged;
            _foldingManager = FoldingManager.Install(EditorPane.TextArea);
            _blockCompletion = new BlockCompletionService(BlockCompletionStrategies);
            
            var markerService = new TextMarkerService(EditorPane.Document);
            Initialize(markerService);

            _textMarkerService = markerService;
        }

        private void Initialize(TextMarkerService service)
        {
            EditorPane.TextArea.TextView.BackgroundRenderers.Add(service);
            EditorPane.TextArea.TextView.LineTransformers.Add(service);
            var services = (IServiceContainer)EditorPane.Document.ServiceProvider.GetService(typeof(IServiceContainer));
            if (services != null)
            {
                services.AddService(typeof(ITextMarkerService), service);
            }
        }

        private void EditorPane_TextChanged(object sender, EventArgs e)
        {
            FoldingStrategy?.UpdateFoldings(_foldingManager, EditorPane.Document);
            if (_blockCompletion.CanComplete(EditorPane.TextArea.Caret, EditorPane.Document, _foldingManager, out var completionStrategy, out var text))
            {
                completionStrategy.Complete(EditorPane.TextArea.Caret, text, EditorPane.Document);
            }

            if (EditorPane.Text.Length > 20 
                && !_textMarkerService.TextMarkers.Any(marker => marker.StartOffset == 0)
                && !EditorPane.Text.Contains("Option Explicit"))
            {
                AddInspectionErrorMarker(0, 1);
            }
            else if (EditorPane.Text.Length > 30
                && !_textMarkerService.TextMarkers.Any(marker => marker.StartOffset == 0)
                && !EditorPane.Text.Contains("Public"))
            {
                AddInspectionSuggestionMarker(23, 11);
            }
            else
            {
                _textMarkerService.RemoveAll(marker => true);
            }
        }

        //public void AddInspectionMarker(IInspectionResult inspectionResult)
        //{

        //}
        
        public void AddInspectionErrorMarker(int startOffset, int length)
        {
            var marker = _textMarkerService.Create(startOffset, length);
            marker.MarkerTypes = TextMarkerTypes.SquigglyUnderline | TextMarkerTypes.ScrollBarRightTriangle;
            marker.MarkerColor = Colors.Red;
            // TODO use IInspectionResult.Description
            marker.ToolTip = "Error-level inspection result";
        }

        public void AddInspectionWarningMarker(int startOffset, int length)
        {
            var marker = _textMarkerService.Create(startOffset, length);
            marker.MarkerTypes = TextMarkerTypes.SquigglyUnderline;
            marker.MarkerColor = Colors.Blue;
            // TODO use IInspectionResult.Description
            marker.ToolTip = "Warning-level inspection result";
        }

        public void AddInspectionSuggestionMarker(int startOffset, int length)
        {
            var marker = _textMarkerService.Create(startOffset, length);
            marker.MarkerTypes = TextMarkerTypes.SquigglyUnderline;
            marker.MarkerColor = Colors.Green;
            // TODO use IInspectionResult.Description
            marker.ToolTip = "Suggestion-level inspection result";
        }

        public void AddInspectionHintMarker(int startOffset, int length)
        {
            var marker = _textMarkerService.Create(startOffset, length);
            marker.MarkerTypes = TextMarkerTypes.DottedUnderline;
            marker.MarkerColor = Colors.Black;
            // TODO use IInspectionResult.Description
            marker.ToolTip = "Hint-level inspection result";
        }
    }
}
