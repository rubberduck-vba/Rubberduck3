﻿using ICSharpCode.AvalonEdit.Folding;
using Rubberduck.UI.RubberduckEditor.TextTransform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.ComponentModel.Design;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using Rubberduck.UI.Abstract;
using System.Threading;
using ICSharpCode.AvalonEdit.CodeCompletion;

namespace Rubberduck.UI.Xaml.Controls
{
    /// <summary>
    /// Interaction logic for RubberduckEditorControl.xaml
    /// </summary>
    public partial class RubberduckEditorControl : UserControl
    {
        private readonly Timer _timer;
        //private readonly FoldingManager _foldingManager;
        private readonly ITextMarkerService _textMarkerService;

        private CompletionWindow _completionWindow = default!;

        public RubberduckEditorControl()
        {
            InitializeComponent();
            EditorPane.PreviewKeyDown += OnPreviewKeyDown;
            EditorPane.MouseHover += OnMouseHover;

            EditorPane.TextChanged += OnTextChanged;
            EditorPane.TextArea.Caret.PositionChanged += OnCaretPositionChanged;

            EditorPane.TextArea.TextEntering += TextArea_TextEntering;
            EditorPane.TextArea.TextEntered += TextArea_TextEntered;

            //_foldingManager = FoldingManager.Install(EditorPane.TextArea);

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
        /*
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
        */
        private void ShowCompletionList(IEnumerable<IMemberInfoViewModel> members)
        {
            _completionWindow = new CompletionWindow(EditorPane.TextArea);
            foreach (var member in members)
            {
                _completionWindow.CompletionList.CompletionData.Add(new CompletionInfo(member));
            }
            _completionWindow.Show();
            _completionWindow.Closed += (o, e) => _completionWindow = null!;
        }

        private void OnSelectedMemberProviderChanged(object sender, EventArgs e)
        {
            //ViewModel.SelectedMemberProvider.MemberSelected += OnMemberSelected;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext is ICodePaneViewModel context)
            {
                context.Document = EditorPane.Document;
                //context.SelectedMemberProviderChanged += OnSelectedMemberProviderChanged;
                //context.SelectedMemberProvider = context.MemberProviders.FirstOrDefault();
                //if (context.SelectedMemberProvider != null)
                //{
                //    context.SelectedMemberProvider.CurrentMember = context.SelectedMemberProvider?.Members.FirstOrDefault();
                //}
                //context.ParseTreeChanged += OnParseTreeChanged;
            }
        }

        private async void OnIdle(object obj)
        {
            await Dispatcher.InvokeAsync(SyncDocumentAsync);
        }

        private async Task SyncDocumentAsync()
        {
            using (var reader = EditorPane.Document.CreateReader())
            {
                // TODO send content to language server
            }
            await Task.CompletedTask;
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

        /*
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
        */

        private void OnMouseHover(object sender, MouseEventArgs e)
        {
            var textPosition = EditorPane.GetPositionFromPoint(e.GetPosition(EditorPane));
            if (textPosition.HasValue)
            {
                var offset = EditorPane.Document.GetOffset(textPosition.Value.Location);
                // TODO send hover request to server
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
            //EditorPane.TextArea.TextView.BackgroundRenderers.Add(service);
            //EditorPane.TextArea.TextView.LineTransformers.Add(service);
            //var services = (IServiceContainer)EditorPane.Document.ServiceProvider.GetService(typeof(IServiceContainer));
            //services?.AddService(typeof(ITextMarkerService), service);
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
