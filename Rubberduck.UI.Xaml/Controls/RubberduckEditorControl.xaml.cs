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
using System.Threading;
using System.Runtime;
using Rubberduck.Parsing;
using Rubberduck.Parsing.Model;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Editing;

namespace Rubberduck.UI.Xaml.Controls
{
    public class CompletionInfo : ICompletionData
    {
        public CompletionInfo(IMemberInfoViewModel memberInfo)
        {
            Text = memberInfo.Name;
            Content = memberInfo.Name; // TODO make a nice XAML control for this
        }

        public ImageSource Image { get; }

        public string Text { get; }

        public object Content { get; }

        public object Description { get; }

        public double Priority { get; }

        public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
        {
            textArea.Document.Replace(completionSegment, Text);
        }
    }

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

    /// <summary>
    /// Interaction logic for RubberduckEditorControl.xaml
    /// </summary>
    public partial class RubberduckEditorControl : UserControl
    {
        private readonly Timer _timer;
        private readonly FoldingManager _foldingManager;
        private readonly ITextMarkerService _textMarkerService;

        private CompletionWindow _completionWindow = null;

        public RubberduckEditorControl()
        {
            InitializeComponent();
            EditorPane.PreviewKeyDown += OnPreviewKeyDown;
            EditorPane.MouseHover += OnMouseHover;

            EditorPane.TextChanged += OnTextChanged;
            EditorPane.TextArea.Caret.PositionChanged += OnCaretPositionChanged;

            EditorPane.TextArea.TextEntering += TextArea_TextEntering;
            EditorPane.TextArea.TextEntered += TextArea_TextEntered;

            _foldingManager = FoldingManager.Install(EditorPane.TextArea);

            var markerService = new TextMarkerService(EditorPane.Document);
            Initialize(markerService);

            _textMarkerService = markerService;
            _timer = new Timer(OnIdle, null, Timeout.Infinite, Timeout.Infinite);

            DataContextChanged += OnDataContextChanged;
        }

        private void TextArea_TextEntering(object sender, TextCompositionEventArgs e)
        {
            if (e.Text == ".")
            {
                ShowCompletionList(ViewModel.SelectedMemberProvider.Members); // TODO be smarter than that
            }
        }

        private void TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length > 0 && _completionWindow != null)
            {
                if (!char.IsLetterOrDigit(e.Text[0]))
                {
                    _completionWindow.CompletionList.RequestInsertion(e);
                }
            }
        }

        public ICodePaneViewModel ViewModel
        {
            get => DataContext as ICodePaneViewModel;
            set
            {
                DataContext = value;
            }
        }

        public IEnterKeyStrategy[] EnterKeyStrategies { get; } = new[] 
        { 
            new KeepCurrentLineTogetherEnterKeyStrategy() 
        };

        private TextLocation _caretLocation;
        public TextLocation CaretLocation 
        {
            get => _caretLocation;
            private set
            {
                if (_caretLocation != value)
                {
                    _caretLocation = value;
                    ViewModel.Status.CaretColumn = value.Column;
                    ViewModel.Status.CaretLine = value.Line;
                    ViewModel.Status.CaretOffset = EditorPane.CaretOffset;
                }
            }
        }

        private void OnParseTreeChanged(object sender, ParseTreeEventArgs e)
        {
            var foldingInfo = e.BlockFoldingInfo.Select(i => new NewFolding { Name = i.Name, StartOffset = i.Offset.Start, EndOffset = i.Offset.End, IsDefinition = i.IsDefinition });
            _foldingManager.UpdateFoldings(foldingInfo, e.SyntaxErrors.OrderBy(i => i.StartOffset).FirstOrDefault()?.StartOffset ?? -1);

            _textMarkerService.RemoveAll(m => true);
            foreach (var error in e.SyntaxErrors)
            {
                AddSyntaxErrorMarker(error);
            }
        }

        private void ShowCompletionList(IEnumerable<IMemberInfoViewModel> members)
        {
            _completionWindow = new CompletionWindow(EditorPane.TextArea);
            foreach (var member in members)
            {
                _completionWindow.CompletionList.CompletionData.Add(new CompletionInfo(member));
            }
            _completionWindow.Show();
            _completionWindow.Closed += (o, e) => _completionWindow = null;
        }

        private void OnSelectedMemberProviderChanged(object sender, EventArgs e)
        {
            ViewModel.SelectedMemberProvider.MemberSelected += OnMemberSelected;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext is ICodePaneViewModel context)
            {
                context.Document = EditorPane.Document;
                context.SelectedMemberProviderChanged += OnSelectedMemberProviderChanged;
                context.SelectedMemberProvider = context.MemberProviders.FirstOrDefault();
                if (context.SelectedMemberProvider != null)
                {
                    context.SelectedMemberProvider.CurrentMember = context.SelectedMemberProvider?.Members.FirstOrDefault();
                }
                context.ParseTreeChanged += OnParseTreeChanged;
            }
        }

        private async void OnIdle(object obj)
        {
            await Dispatcher.InvokeAsync(ParseDocumentAsync);
        }

        private async Task ParseDocumentAsync()
        {
            using (var reader = EditorPane.Document.CreateReader())
            {
                await ViewModel.ParseAsync(reader);
            }
        }

        private void ResetIdleTimer() => _timer.Change((int)((ViewModel?.EditorSettings?.IdleTimeoutSeconds * 1000) ?? Timeout.Infinite), Timeout.Infinite);

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
                        var startLine = EditorPane.Document.GetLineByOffset(Math.Min(maxOffset, member.Offset.Start));
                        var endLine = EditorPane.Document.GetLineByOffset(Math.Min(maxOffset, member.Offset.End));

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
        }

        private void OnMemberSelected(object sender, NavigateToMemberEventArgs e)
        {
            //if (e.MemberInfo != null && EditorPane.TextArea.Caret.Line != e.MemberInfo.StartLine)
            //{
            //    EditorPane.CaretOffset = e.MemberInfo.Offset.Start; // FIXME need MemberBodyOffset
            //    EditorPane.ScrollTo(e.MemberInfo.StartLine, 1);
            //}

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
            ResetIdleTimer();
            if (ViewModel != null)
            {
                ViewModel.Status.DocumentLines = EditorPane.Document.LineCount;
                ViewModel.Status.DocumentLength = EditorPane.Document.TextLength;
            }
        }

        private ToolTip CreateTooltip(string title, string text, string location)
        {
            var vm = new { TipTitle = title, TipText = text, LocationText = location, IsError = true }; // TODO extract class
            var tooltip = new TextMarkerToolTip
            {
                DataContext = vm,
                PlacementTarget = EditorPane
            };
            return tooltip;
        }

        private ToolTip CreateTooltip(string title, string text)
        {
            var vm = new { TipTitle = title, TipText = text, IsInsight = true }; // TODO extract class
            var tooltip = new TextMarkerToolTip
            {
                DataContext = vm,
                PlacementTarget = EditorPane
            };
            return tooltip;
        }

        #region TODO move to view-layer service?
        public void AddSyntaxErrorMarker(ISyntaxErrorViewModel vm)
        {
            var marker = _textMarkerService.Create(vm.StartOffset, vm.Length);
            if (marker != null)
            {
                marker.MarkerTypes = TextMarkerTypes.SquigglyUnderline;
                marker.MarkerColor = Colors.DarkRed;
                
                marker.ToolTip = CreateTooltip("Syntax Error", vm.Message, vm.LocationMessage);
            }
        }

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
