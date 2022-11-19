using ICSharpCode.AvalonEdit.Folding;
using Rubberduck.UI.RubberduckEditor.TextTransform;
using Rubberduck.UI.RubberduckEditor;
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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel.Design;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;

namespace Rubberduck.UI.Xaml.Controls
{
    public interface IEnterKeyStrategy
    {
        bool IsActive { get; set; }
        bool HandleEvent(TextDocument document, ref int caretOffset);
    }

    public class KeepCurrentLineTogetherEnterKeyStrategy : IEnterKeyStrategy
    {
        public bool IsActive { get; set; } = true; // TODO make it configurable

        public bool HandleEvent(TextDocument document, ref int caretOffset)
        {
            var offset = caretOffset;
            var nextChar = document.GetCharAt(offset);
            if (nextChar == '(')
            {
                var currentLine = document.GetLineByOffset(offset);
                var nextLineOffset = currentLine.NextLine.Offset; // TODO account for indentation
                caretOffset = nextLineOffset;
                return true;
            }

            return false;
        }
    }

    public class LineTracker : ILineTracker
    {
        public void BeforeRemoveLine(DocumentLine line)
        {
        }

        public void ChangeComplete(DocumentChangeEventArgs e)
        {
        }

        public void LineInserted(DocumentLine insertionPos, DocumentLine newLine)
        {
        }

        public void RebuildDocument()
        {
        }

        public void SetLineLength(DocumentLine line, int newTotalLength)
        {
        }
    }

    /// <summary>
    /// Interaction logic for RubberduckEditorControl.xaml
    /// </summary>
    public partial class RubberduckEditorControl : UserControl
    {
        public IEnterKeyStrategy[] EnterKeyStrategies { get; } = new[] 
        { 
            new KeepCurrentLineTogetherEnterKeyStrategy() 
        };
        public IFoldingStrategy FoldingStrategy { get; } = new VBFoldingStrategy();
        public IEnumerable<IBlockCompletionStrategy> BlockCompletionStrategies { get; }
            = VBFoldingStrategy.BlockInfo.Values.Select(e => new BlockCompletionStrategy(e)).ToList();

        public TextLocation CaretLocation { get; private set; }

        private readonly FoldingManager _foldingManager;
        private readonly BlockCompletionService _blockCompletion;
        private readonly ITextMarkerService _textMarkerService;

        public RubberduckEditorControl()
        {
            InitializeComponent();
            EditorPane.PreviewKeyDown += EditorPane_PreviewKeyDown;
            EditorPane.TextChanged += EditorPane_TextChanged;
            EditorPane.MouseHover += EditorPane_MouseHover;

            EditorPane.Document.LineTrackers.Add(new LineTracker());
            _foldingManager = FoldingManager.Install(EditorPane.TextArea);
            _blockCompletion = new BlockCompletionService(BlockCompletionStrategies);

            var markerService = new TextMarkerService(EditorPane.Document);
            Initialize(markerService);
            
            _textMarkerService = markerService;
        }

        private void EditorPane_MouseHover(object sender, MouseEventArgs e)
        {
            var textPosition = EditorPane.GetPositionFromPoint(e.GetPosition(EditorPane));
            if (textPosition.HasValue)
            {
                var offset = EditorPane.Document.GetOffset(textPosition.Value.Location);
                var markers = _textMarkerService.GetMarkersAtOffset(offset);
                if (markers.Any())
                {
                    var marker = markers.First() as TextMarker;
                    var markerRect = BackgroundGeometryBuilder.GetRectsForSegment(EditorPane.TextArea.TextView, marker).First();
                    var tooltip = (ToolTip)marker.ToolTip;
                    markerRect.Offset(2d, 5d);
                    tooltip.PlacementRectangle = markerRect;
                    tooltip.IsOpen = true;
                    EditorPane.ToolTip = tooltip;
                    return;
                }
            }

            HideEditorPaneToolTip();
        }

        private void HideEditorPaneToolTip()
        {
            if (EditorPane.ToolTip is ToolTip toolTip)
            {
                toolTip.IsOpen = false;
                EditorPane.ToolTip = null;
            }
        }

        private void EditorPane_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (EnterKeyStrategies.Any() && e.Key == Key.Enter)
            {
                var offset = EditorPane.CaretOffset;
                if (offset + 2 < EditorPane.Document.TextLength)
                {
                    foreach (var strategy in EnterKeyStrategies)
                    {
                        if (strategy.HandleEvent(EditorPane.Document, ref offset))
                        {
                            EditorPane.CaretOffset = offset;
                            e.Handled = true;
                            break;
                        }
                    }
                }
            }
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

            // PoC code >>>>>>>>>>>>>>>>>>>>>>>>>
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
                AddInspectionSuggestionMarker(17, 3);
            }
            else
            {
                _textMarkerService.RemoveAll(marker => true);
            }
            // <<<<<<<<<<<<<<<<<<<<<<<<< PoC code
        }

        //public void AddInspectionMarker(IInspectionResult inspectionResult)
        //{

        //}

        private ToolTip CreateTooltip(string text)
        {
            var tooltip = new ToolTip
            {
                Content = new TextBlock { Text = text },
                PlacementTarget = EditorPane
            };
            return tooltip;
        }

        public void AddInspectionErrorMarker(int startOffset, int length)
        {
            var marker = _textMarkerService.Create(startOffset, length);
            if (marker != null)
            {
                marker.MarkerTypes = TextMarkerTypes.SquigglyUnderline;
                marker.MarkerColor = Colors.Red;
                // TODO use IInspectionResult.Description
                marker.ToolTip = CreateTooltip("Error-level inspection result");
            }
        }

        public void AddInspectionWarningMarker(int startOffset, int length)
        {
            var marker = _textMarkerService.Create(startOffset, length);
            if (marker != null)
            {
                marker.MarkerTypes = TextMarkerTypes.SquigglyUnderline;
                marker.MarkerColor = Colors.Blue;
                // TODO use IInspectionResult.Description
                marker.ToolTip = CreateTooltip("Warning-level inspection result");
            }
        }

        public void AddInspectionSuggestionMarker(int startOffset, int length)
        {
            var marker = _textMarkerService.Create(startOffset, length);
            if (marker != null)
            {
                marker.MarkerTypes = TextMarkerTypes.SquigglyUnderline;
                marker.MarkerColor = Colors.Green;
                // TODO use IInspectionResult.Description
                marker.ToolTip = CreateTooltip("Suggestion-level inspection result");
            }
        }

        public void AddInspectionHintMarker(int startOffset, int length)
        {
            var marker = _textMarkerService.Create(startOffset, length);
            if (marker != null)
            {
                marker.MarkerTypes = TextMarkerTypes.DottedUnderline;
                marker.MarkerColor = Colors.Black;
                // TODO use IInspectionResult.Description
                marker.ToolTip = CreateTooltip("Hint-level inspection result");
            }
        }
    }
}
