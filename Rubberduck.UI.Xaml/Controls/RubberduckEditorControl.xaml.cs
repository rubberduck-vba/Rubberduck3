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
using Rubberduck.UI.Abstract;

namespace Rubberduck.UI.Xaml.Controls
{
    public interface IEnterKeyStrategy
    {
        bool IsActive { get; set; }
        bool HandleEvent(TextDocument document, ref int caretOffset);
    }

    public class MemberEditor
    {
        public static MemberType GetMemberType(string line)
        {
            if (line.StartsWith("Private", StringComparison.InvariantCultureIgnoreCase))
            {
                switch (line.Substring(8, 4).ToLowerInvariant())
                {
                    case "cons":
                        return MemberType.ConstPrivate;
                    case "enum":
                        return MemberType.EnumPrivate;
                    case "even":
                        return MemberType.EventPrivate;
                    case "func":
                        return MemberType.FunctionPrivate;
                    case "sub ":
                        return MemberType.ProcedurePrivate;
                    case "type":
                        return MemberType.UserDefinedTypePrivate;
                    case "prop":
                        switch (line.Substring(17, 3).ToLowerInvariant())
                        {
                            case "get":
                                return MemberType.PropertyGetPrivate;
                            case "let":
                                return MemberType.PropertyLetPrivate;
                            case "set":
                                return MemberType.PropertySetPrivate;
                            default:
                                return MemberType.None;
                        }
                    default:
                        return MemberType.FieldPrivate;
                }
            }
            else if (line.StartsWith("Friend", StringComparison.InvariantCultureIgnoreCase))
            {
                switch (line.Substring(7, 4).ToLowerInvariant())
                {
                    case "cons":
                        return MemberType.ConstFriend;
                    case "enum":
                        return MemberType.EnumFriend;
                    case "even":
                        return MemberType.EventFriend;
                    case "func":
                        return MemberType.FunctionFriend;
                    case "sub ":
                        return MemberType.ProcedureFriend;
                    case "type":
                        return MemberType.UserDefinedTypeFriend;
                    case "prop":
                        switch (line.Substring(16, 3).ToLowerInvariant())
                        {
                            case "get":
                                return MemberType.PropertyGetFriend;
                            case "let":
                                return MemberType.PropertyLetFriend;
                            case "set":
                                return MemberType.PropertySetFriend;
                            default:
                                return MemberType.None;
                        }
                    default:
                        return MemberType.FieldFriend;
                }
            }
            else
            {
                switch (line.Substring(0, 4).ToLowerInvariant())
                {
                    case "cons":
                        return MemberType.Const;
                    case "enum":
                        return MemberType.Enum;
                    case "even":
                        return MemberType.Event;
                    case "func":
                        return MemberType.Function;
                    case "sub ":
                        return MemberType.Procedure;
                    case "type":
                        return MemberType.UserDefinedType;
                    case "prop":
                        switch (line.Substring(16, 3).ToLowerInvariant())
                        {
                            case "get":
                                return MemberType.PropertyGet;
                            case "let":
                                return MemberType.PropertyLet;
                            case "set":
                                return MemberType.PropertySet;
                            default:
                                return MemberType.None;
                        }
                    case "dim ":
                        return MemberType.Field;

                    default:
                        return MemberType.None;
                }
            }
        }
    }

    public class KeepCurrentLineTogetherEnterKeyStrategy : IEnterKeyStrategy
    {
        public bool IsActive { get; set; } = true; // TODO make it configurable

        private static HashSet<MemberType> ApplicableMemberTypes = new HashSet<MemberType>(new []
        {
            MemberType.Procedure,
            MemberType.ProcedurePrivate,
            MemberType.ProcedureFriend,
            MemberType.Function,
            MemberType.FunctionPrivate,
            MemberType.FunctionFriend,
            MemberType.PropertyGet,
            MemberType.PropertyGetPrivate,
            MemberType.PropertyGetFriend,
            MemberType.PropertyLet,
            MemberType.PropertyLetPrivate,
            MemberType.PropertyLetFriend,
            MemberType.PropertySet,
            MemberType.PropertySetPrivate,
            MemberType.PropertySetFriend,
        });

        public bool HandleEvent(TextDocument document, ref int caretOffset)
        {
            var offset = caretOffset;
            var line = document.GetText(document.GetLineByOffset(offset));
            if (string.IsNullOrWhiteSpace(line))
            {
                return false;
            }
            var type = MemberEditor.GetMemberType(line);

            var nextChar = document.GetCharAt(offset); 
            if (nextChar == '(' && ApplicableMemberTypes.Contains(type))
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
        public IFoldingStrategy FoldingStrategy { get; }
        public IEnumerable<IBlockCompletionStrategy> BlockCompletionStrategies { get; }
            = VBFoldingStrategy.BlockInfo.Values.Select(e => new BlockCompletionStrategy(e)).ToList();

        public TextLocation CaretLocation { get; private set; }

        private readonly FoldingManager _foldingManager;
        private readonly BlockCompletionService _blockCompletion;
        private readonly ITextMarkerService _textMarkerService;

        public ICodePaneViewModel ViewModel
        {
            get => DataContext as ICodePaneViewModel;
            set
            {
                DataContext = value;
                if (DataContext is ICodePaneViewModel context)
                {
                    context.SelectedMemberProviderChanged += OnSelectedMemberProviderChanged;
                    context.SelectedMemberProvider = context.MemberProviders.FirstOrDefault();
                }
            }
        }

        private void OnSelectedMemberProviderChanged(object sender, EventArgs e)
        {
            ViewModel.SelectedMemberProvider.MemberSelected += OnMemberSelected;
        }

        public RubberduckEditorControl()
        {
            var foldingStrategy = new VBFoldingStrategy();
            FoldingStrategy = foldingStrategy;

            InitializeComponent();
            EditorPane.PreviewKeyDown += OnPreviewKeyDown;
            EditorPane.TextChanged += OnTextChanged;
            EditorPane.MouseHover += OnMouseHover;
            
            EditorPane.TextArea.Caret.PositionChanged += OnCaretPositionChanged;

            EditorPane.Document.LineTrackers.Add(new LineTracker());
            _foldingManager = FoldingManager.Install(EditorPane.TextArea);
            _blockCompletion = new BlockCompletionService(BlockCompletionStrategies);

            var markerService = new TextMarkerService(EditorPane.Document);
            Initialize(markerService);
            
            _textMarkerService = markerService;
        }

        private void OnCaretPositionChanged(object sender, EventArgs e)
        {
            var markers = _textMarkerService.GetMarkersAtOffset(EditorPane.CaretOffset);
            if (!markers.Any())
            {
                HideEditorPaneToolTip();
            }
            else
            {
                var markerRect = EditorPane.TextArea.Caret.CalculateCaretRectangle();
                markerRect.Offset(-5, 0);

                DuckyMenu.PlacementTarget = EditorPane;
                DuckyMenu.PlacementRectangle = markerRect;
                DuckyMenu.IsOpen = true;
            }

            var position = EditorPane.Document.GetLocation(EditorPane.CaretOffset);
            CaretLocation = position;

            var maxOffset = EditorPane.Document.TextLength;
            foreach (var provider in ViewModel.MemberProviders)
            {
                foreach (var member in provider.Members.Where(m => m.HasImplementation))
                {
                    try
                    {
                        var startLine = EditorPane.Document.GetLineByOffset(Math.Min(maxOffset, member.StartOffset));
                        var endLine = EditorPane.Document.GetLineByOffset(Math.Min(maxOffset, member.EndOffset));

                        if (startLine.LineNumber <= position.Line && endLine.LineNumber >= position.Line)
                        {
                            ViewModel.SelectedMemberProvider = provider;
                            provider.CurrentMember = member;
                            break;
                        }
                    }
                    catch 
                    { 
                    }
                }
            }

            ViewModel.UpdateStatus($"L{position.Line} C{position.Column}");
        }

        private void OnMemberSelected(object sender, NavigateToMemberEventArgs e)
        {
            if (EditorPane.TextArea.Caret.Line != e.MemberInfo.StartLine)
            {
                EditorPane.CaretOffset = e.MemberInfo.StartOffset;
                EditorPane.ScrollTo(e.MemberInfo.StartLine, 1);
            }

            EditorPane.Focus();

            if (!EditorPane.TextArea.Focus())
            {
                return;
            }
        }

        private void OnMouseHover(object sender, MouseEventArgs e)
        {
            var textPosition = EditorPane.GetPositionFromPoint(e.GetPosition(EditorPane));
            if (textPosition.HasValue)
            {
                var offset = EditorPane.Document.GetOffset(textPosition.Value.Location);
                ShowMarkerToolTip(offset);
                return;
            }

            HideEditorPaneToolTip();
        }

        private bool ShowMarkerToolTip(int offset)
        {
            var result = false;

            var marker = _textMarkerService
                .GetMarkersAtOffset(offset)
                .OfType<TextMarker>()
                .FirstOrDefault();

            if (!(marker is null))
            {
                var tooltip = (ToolTip)marker.ToolTip;

                var markerRect = BackgroundGeometryBuilder.GetRectsForSegment(EditorPane.TextArea.TextView, marker).First();
                markerRect.Offset(2d, 1d);

                tooltip.PlacementRectangle = markerRect;

                tooltip.IsOpen = true;
                EditorPane.ToolTip = tooltip;

                result = true;
            }

            return result;
        }

        private void HideEditorPaneToolTip()
        {
            if (EditorPane.ToolTip is ToolTip toolTip)
            {
                toolTip.IsOpen = false;
                EditorPane.ToolTip = null;
            }

            DuckyMenu.IsOpen = false;
        }

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
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

        private void OnTextChanged(object sender, EventArgs e)
        {
            if (!_foldingManager.AllFoldings.Any(f => EditorPane.Document.GetLineByOffset(f.StartOffset).LineNumber == EditorPane.TextArea.Caret.Line) && _blockCompletion.CanComplete(EditorPane.TextArea.Caret, EditorPane.Document, ViewModel.SelectedMemberProvider.CurrentMember, out var completionStrategy, out var text))
            {
                completionStrategy.Complete(EditorPane.TextArea.Caret, text, EditorPane.Document);
            }

            var infos = FoldingStrategy?.UpdateFoldings(_foldingManager, EditorPane.Document);
            UpdateMembers(infos);

            // PoC code >>>>>>>>>>>>>>>>>>>>>>>>>
            if (EditorPane.Text.Length > 20
                && !_textMarkerService.TextMarkers.Any(marker => marker.StartOffset == 0)
                && !EditorPane.Text.Contains("Option Explicit"))
            {
                AddInspectionErrorMarker(0, 1);
            }
            else if (EditorPane.Text.Length > 30
                && !_textMarkerService.TextMarkers.Any(marker => marker.StartOffset == 17)
                && !EditorPane.Text.Contains("Public Sub "))
            {
                AddInspectionHintMarker(17, 3);
            }
            else
            {
                _textMarkerService.RemoveAll(marker => true);
            }
            // <<<<<<<<<<<<<<<<<<<<<<<<< PoC code
        }

        private void UpdateMembers(IEnumerable<(int StartOffset, int EndOffset, MemberType MemberType, string Name)> infos)
        {
            var allNames = infos.Select(i => i.Name).ToHashSet();
            var existingNames = ViewModel.SelectedMemberProvider.Members.Where(m => m.HasImplementation).Select(m => m.Name).ToHashSet();

            var deletedNames = existingNames.Except(allNames);
            foreach (var name in deletedNames)
            {
                var member = ViewModel.SelectedMemberProvider.Members.SingleOrDefault(m => m.HasImplementation && m.Name == name);
                if (member != null)
                {
                    ViewModel.SelectedMemberProvider.Members.Remove(member);
                    break;
                }
            }

            var newNames = allNames.Except(existingNames);
            foreach (var name in newNames)
            {
                var candidates = infos.Where(i => i.Name == name).ToArray();
                if (candidates.Length == 1)
                {
                    var info = candidates[0];
                    ViewModel.SelectedMemberProvider.AddMember(name, info.MemberType, info.StartOffset, info.EndOffset);
                    break;
                }
            }
        }

        private ToolTip CreateTooltip(string title, string text)
        {
            var vm = new { TipTitle = title, TipText = text }; // TODO extract class
            var tooltip = new TextMarkerToolTip
            {
                DataContext = vm,
                PlacementTarget = EditorPane
            };
            return tooltip;
        }

        #region TODO move to view-layer service?
        public void AddInspectionErrorMarker(int startOffset, int length)
        {
            var marker = _textMarkerService.Create(startOffset, length);
            if (marker != null)
            {
                marker.MarkerTypes = TextMarkerTypes.SquigglyUnderline;
                marker.MarkerColor = Colors.Red;
                // TODO use IInspectionResult.Description
                marker.ToolTip = CreateTooltip("Option Explicit", "'Option Explicit' is not specified in 'Module1'.");
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
                marker.ToolTip = CreateTooltip("Inspection name here", "Warning-level inspection result");
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
                marker.ToolTip = CreateTooltip("Implicit Public Member", "Member 'DoSomething' is implicitly public.");
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
                marker.ToolTip = CreateTooltip("Implicit Public Member", "Member 'DoSomething' is implicitly public.");
            }
        }
        #endregion
    }
}
