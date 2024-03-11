using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Rendering;
using Microsoft.Xaml.Behaviors.Core;
using Rubberduck.InternalApi.Model;
using Rubberduck.UI.Command.Abstract;
using Rubberduck.UI.Services;
using Rubberduck.UI.Services.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace Rubberduck.UI.Shell.Document;

/// <summary>
/// Interaction logic for SourceCodeEditorControl.xaml
/// </summary>
public partial class SourceCodeEditorControl : UserControl
{
    private readonly int IdleMillisecondsSettingValue = 2500;
    private readonly Timer _idleTimer;

    private readonly FoldingManager _foldings;
    private readonly TextMarkerService _markers;
    private readonly TextMarkersMargin _margin;

    public SourceCodeEditorControl()
    {
        DataContextChanged += OnDataContextChanged;
        InitializeComponent();

        Editor = (ThunderFrame.Content as DependencyObject)?.GetChildOfType<BindableTextEditor>() ?? throw new InvalidOperationException();
        Editor.Loaded += OnEditorLoaded;

        _foldings = FoldingManager.Install(Editor.TextArea);
        _markers = new TextMarkerService(Editor.Document);
        Initialize(_markers);

        _margin = new TextMarkersMargin(_markers);
        Editor.TextArea.LeftMargins.Insert(0, _margin);

        _idleTimer = new Timer(OnIdle, null, Timeout.Infinite, Timeout.Infinite);

        ExpandAllCommand = new ActionCommand(ExpandAllFoldings);
        CollapseAllCommand = new ActionCommand(CollapseAllFoldings);
    }

    private void OnEditorLoaded(object sender, RoutedEventArgs e)
    {
        Editor.TextChanged += OnTextChanged;
        Editor.MouseHover += OnMouseHover;

        Editor.TextArea.SelectionChanged += OnSelectionChanged;
        Editor.TextArea.Caret.PositionChanged += OnCaretPositionChanged;

        Editor.TextArea.TextView.ScrollOffsetChanged += OnTextViewScrollOffsetChanged;
    }

    private void OnTextViewScrollOffsetChanged(object? sender, EventArgs e)
    {
        HideMarkerToolTip();
        _margin.UpdateLayout();
    }

    private void OnMouseHover(object sender, MouseEventArgs e)
    {
        var textPosition = Editor.GetPositionFromPoint(e.GetPosition(Editor));
        if (textPosition.HasValue)
        {
            var offset = Editor.Document.GetOffset(textPosition.Value.Location);
            ShowMarkerToolTip(offset);
            return;
        }

        HideMarkerToolTip();
    }

    private bool ShowMarkerToolTip(int offset)
    {
        var result = false;

        var marker = _markers
            .GetMarkersAtOffset(offset)
            .OfType<TextMarker>()
            .FirstOrDefault();

        if (marker?.ToolTip is ToolTip tooltip)
        {
            var markerRect = BackgroundGeometryBuilder.GetRectsForSegment(Editor.TextArea.TextView, marker).First();
            markerRect.Offset(2d, 1d);

            tooltip.PlacementRectangle = markerRect;

            tooltip.IsOpen = true;
            Editor.ToolTip = tooltip;

            result = true;
        }

        return result;
    }

    private void HideMarkerToolTip()
    {
        if (Editor.ToolTip is ToolTip toolTip)
        {
            toolTip.IsOpen = false;
            Editor.ToolTip = null;
        }
    }

    private void Initialize(TextMarkerService service)
    {
        Editor.TextArea.TextView.BackgroundRenderers.Add(service);
        Editor.TextArea.TextView.LineTransformers.Add(service);

        var services = (IServiceContainer)Editor.Document.ServiceProvider.GetService(typeof(IServiceContainer))!;
        services?.AddService(typeof(ITextMarkerService), service);
    }

    public ICommand ExpandAllCommand { get; }
    public ICommand CollapseAllCommand { get; }

    private bool _didChange = false;
    private void OnIdle(object? obj)
    {
        if (_didChange)
        {
            _idleTimer.Change(Timeout.Infinite, Timeout.Infinite);
            ViewModel.NotifyDocumentChanged();
            _didChange = false;
        }
    }

    private void ResetIdleTimer() => _idleTimer.Change(IdleMillisecondsSettingValue, Timeout.Infinite);

    private void ExpandAllFoldings()
    {
        foreach (var folding in _foldings.AllFoldings)
        {
            folding.IsFolded = false;
        }
    }

    private void CollapseAllFoldings()
    {
        foreach (var folding in _foldings.AllFoldings)
        {
            folding.IsFolded = true;
        }
    }

    private async void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        ViewModel = (IDocumentTabViewModel)e.NewValue;
        try
        {
            await HandleDataContextChangedAsync();
        }
        catch (Exception exception)
        {
            // TODO get a logger over here
        }
    }

    private async Task HandleDataContextChangedAsync()
    {
        if (ViewModel.DocumentType == SupportedDocumentType.SourceFile)
        {
            await UpdateFoldingsAsync();
        }

        UpdateStatusInfo();
    }

    private async Task UpdateFoldingsAsync()
    {
        var foldings = await ViewModel.RequestFoldingsAsync();
        var diagnostics = await ViewModel.RequestDiagnosticsAsync();

        var firstErrorRange = diagnostics
            .FirstOrDefault(e => e.Code?.String == RubberduckDiagnosticId.SyntaxError.Code())?.Range;

        var firstErrorOffset = -1;

        await Dispatcher.InvokeAsync(() =>
        {
            var newFoldings = foldings
                .Select(e => e.ToNewFolding(Editor.Document))
                .OrderBy(e => e.StartOffset)
                .ToArray();
            if (firstErrorRange != null)
            {
                firstErrorOffset = Editor.Document.GetOffset(firstErrorRange.Start.Line, firstErrorRange.Start.Character);
            }

            _foldings.Clear();
            _foldings.UpdateFoldings(newFoldings, firstErrorOffset);

            _markers.RemoveAll(e => true);
            foreach (var diagnostic in ViewModel.DocumentState.Diagnostics)
            {
                diagnostic.WithTextMarker(Editor, _markers);
            }
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
        ResetIdleTimer();
        
        UpdateStatusInfo();
        HideMarkerToolTip();
    }

    private void OnTextChanged(object? sender, EventArgs e)
    {
        _didChange = true;
        ViewModel.TextContent = Editor.Text; // binding to source isn't working
        ResetIdleTimer();

        UpdateStatusInfo();
        HideMarkerToolTip();
    }

    private void OnSelectionChanged(object? sender, EventArgs e)
    {
        ResetIdleTimer();
        
        UpdateStatusInfo();
        HideMarkerToolTip();
    }
}
