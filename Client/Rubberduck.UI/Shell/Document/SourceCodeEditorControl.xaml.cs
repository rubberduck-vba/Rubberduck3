using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Rendering;
using Microsoft.Xaml.Behaviors.Core;
using Rubberduck.InternalApi.Model;
using Rubberduck.UI.Services;
using Rubberduck.UI.Services.Abstract;
using System;
using System.ComponentModel.Design;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Rubberduck.UI.Shell.Document;

/// <summary>
/// Interaction logic for SourceCodeEditorControl.xaml
/// </summary>
public partial class SourceCodeEditorControl : UserControl
{
    private static readonly int IdleMillisecondsSettingValue = 1500; // TODO make this a setting

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

        IdleTimer = new Timer(IdleTimerCallback, null, IdleMillisecondsSettingValue, Timeout.Infinite);

        ExpandAllCommand = new ActionCommand(ExpandAllFoldings);
        CollapseAllCommand = new ActionCommand(CollapseAllFoldings);
    }

    private void Initialize(TextMarkerService service)
    {
        Editor.TextArea.TextView.BackgroundRenderers.Add(service);
        Editor.TextArea.TextView.LineTransformers.Add(service);

        var services = (IServiceContainer)Editor.Document.ServiceProvider.GetService(typeof(IServiceContainer))!;
        services?.AddService(typeof(ITextMarkerService), service);
    }

    /// <summary>
    /// A timer that runs between keypresses to evaluate idle time; 
    /// callback is invoked if/when a configurable threshold is met, to notify the server of document changes.
    /// </summary>
    private Timer IdleTimer { get; }
    private void IdleTimerCallback(object? state) => ViewModel.NotifyDocumentChanged();

    /// <summary>
    /// Resets the idle timer to fire a callback in <c>IdleMillisecondsSettingValue</c> milliseconds.
    /// </summary>
    private void ResetIdleTimer() => IdleTimer.Change(IdleMillisecondsSettingValue, Timeout.Infinite);

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


    public ICommand ExpandAllCommand { get; }
    public ICommand CollapseAllCommand { get; }

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
        ViewModel.DocumentStateChanged += OnServerDocumentStateChanged;
        try
        {
            await HandleDataContextChangedAsync().ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            // TODO get a logger over here
        }
    }

    private async void OnServerDocumentStateChanged(object? sender, EventArgs e)
    {
        try
        {
            await UpdateFoldingsAsync().ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            //
        }
    }

    private async Task HandleDataContextChangedAsync()
    {
        if (ViewModel.DocumentType == SupportedDocumentType.SourceFile)
        {
            Editor.Text = ViewModel.TextContent;
            await UpdateFoldingsAsync().ConfigureAwait(false);
        }

        UpdateStatusInfo();
    }

    private async Task UpdateFoldingsAsync()
    {
        var foldings = ViewModel.DocumentState.Foldings;
        var diagnostics = ViewModel.DocumentState.Diagnostics;

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
                firstErrorOffset = Editor.Document.GetOffset(firstErrorRange.Start.Line, 1);
            }

            _foldings.UpdateFoldings(newFoldings, firstErrorOffset);

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
        UpdateStatusInfo();
        HideMarkerToolTip();
    }

    private void OnTextChanged(object? sender, EventArgs e)
    {
        ViewModel.TextContent = Editor.Text; // binding to source isn't working
        ResetIdleTimer();

        UpdateStatusInfo();
        HideMarkerToolTip();
    }

    private void OnSelectionChanged(object? sender, EventArgs e)
    {
        UpdateStatusInfo();
        HideMarkerToolTip();
    }
}
