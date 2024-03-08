using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Rendering;
using Microsoft.Xaml.Behaviors.Core;
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

    public SourceCodeEditorControl()
    {
        InitializeComponent();

        Editor = (ThunderFrame.Content as DependencyObject)?.GetChildOfType<BindableTextEditor>() ?? throw new InvalidOperationException();
        Editor.TextArea.SelectionChanged += OnSelectionChanged;
        Editor.TextArea.Caret.PositionChanged += OnCaretPositionChanged;
        Editor.TextChanged += OnTextChanged;

        Editor.MouseHover += OnMouseHover;

        _foldings = FoldingManager.Install(Editor.TextArea);
        _markers = new TextMarkerService(Editor.Document);
        Initialize(_markers);

        _idleTimer = new Timer(OnIdle, null, Timeout.Infinite, Timeout.Infinite);

        ExpandAllCommand = new ActionCommand(ExpandAllFoldings);
        CollapseAllCommand = new ActionCommand(CollapseAllFoldings);

        DataContextChanged += OnDataContextChanged;
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

    private void OnIdle(object? obj)
    {

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
        var document = Editor.Document;
        var service = _markers;

        if (ViewModel.DocumentType == SupportedDocumentType.SourceFile)
        {
            await Dispatcher.InvokeAsync(async () =>
            {
                var foldings = (await ViewModel.RequestFoldingsAsync())
                    .Select(e => e.ToNewFolding(document))
                    .OrderBy(e => e.StartOffset)
                    .ToArray();

                _foldings.Clear();
                _foldings.UpdateFoldings(foldings, -1); // TODO account for syntax errors instead of passing -1?

                foreach (var diagnostic in ViewModel.DocumentState.Diagnostics)
                {
                    diagnostic.WithTextMarker(Editor, service);
                }
            });
        }

        UpdateStatusInfo();
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
    }

    private void OnTextChanged(object? sender, EventArgs e)
    {
        ResetIdleTimer();
        UpdateStatusInfo();
    }

    private void OnSelectionChanged(object? sender, EventArgs e)
    {
        ResetIdleTimer();
        UpdateStatusInfo();
    }
}
