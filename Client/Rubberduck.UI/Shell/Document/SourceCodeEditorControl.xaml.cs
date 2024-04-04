using AsyncAwaitBestPractices;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Rendering;
using Rubberduck.InternalApi.Model;
using Rubberduck.UI.Command.Abstract;
using Rubberduck.UI.Services;
using Rubberduck.UI.Services.Abstract;
using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Rubberduck.UI.Shell.Document;

/// <summary>
/// Interaction logic for SourceCodeEditorControl.xaml
/// </summary>
public partial class SourceCodeEditorControl : UserControl
{
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

        var service = UIServiceHelper.Instance ?? throw new InvalidOperationException();
        ExpandAllCommand = new DelegateCommand(service, parameter => ExpandAllFoldings(), parameter => _foldings.AllFoldings.Any(folding => folding.IsFolded)) { Name = nameof(ExpandAllCommand) };
        CollapseAllCommand = new DelegateCommand(service, parameter => CollapseAllFoldings(), parameter => _foldings.AllFoldings.Any(folding => !folding.IsFolded)) { Name = nameof(CollapseAllCommand) };
        GoToHelpUrlCommand = new DelegateCommand(service, ExecuteGoToPageCommand);
    }

    private void Initialize(TextMarkerService service)
    {
        Editor.TextArea.TextView.BackgroundRenderers.Add(service);
        Editor.TextArea.TextView.LineTransformers.Add(service);

        var services = (IServiceContainer)Editor.Document.ServiceProvider.GetService(typeof(IServiceContainer))!;
        services?.AddService(typeof(ITextMarkerService), service);
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

    private void ExecuteGoToPageCommand(object? parameter)
    {
        var uri = (Uri)parameter;
        new WebNavigator().Navigate(uri);
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

        if (marker?.ToolTip is Popup tooltip)
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
        if (Editor.ToolTip is Popup toolTip)
        {
            toolTip.IsOpen = false;
            Editor.ToolTip = null;
        }
    }

    public ICommand GoToHelpUrlCommand { get; }
    public ICommand ExpandAllCommand { get; }
    public ICommand CollapseAllCommand { get; }

    private void ExpandAllFoldings()
    {
        Dispatcher.Invoke(() =>
        {
            foreach (var folding in _foldings.AllFoldings)
            {
                folding.IsFolded = false;
            }
        });
    }

    private void CollapseAllFoldings()
    {
        Dispatcher.Invoke(() =>
        {
            foreach (var folding in _foldings.AllFoldings)
            {
                folding.IsFolded = true;
            }
        });
    }

    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        ViewModel = (ICodeDocumentTabViewModel)e.NewValue;
        ViewModel.CodeDocumentStateChanged += ViewModelDocumentStateChanged;
        DataContextChanged -= OnDataContextChanged;

        Editor.Text = ViewModel.TextContent;
        ViewModelDocumentStateChanged(null, EventArgs.Empty);
    }

    private void ViewModelDocumentStateChanged(object? sender, EventArgs e)
    {
        UpdateFoldingsAsync().SafeFireAndForget();
        UpdateDiagnostics();
        UpdateStatusInfo();
    }

    private async Task UpdateFoldingsAsync()
    {
        var foldings = ViewModel.CodeDocumentState.Foldings;
        var diagnostics = ViewModel.CodeDocumentState.Diagnostics;

        var firstErrorRange = diagnostics
            .FirstOrDefault(e => e.Code?.String == RubberduckDiagnosticId.SyntaxError.Code())?.Range;

        var firstErrorOffset = -1;

        await Dispatcher.InvokeAsync(() =>
        {
            var newFoldings = foldings
                .Select(e => e.ToNewFolding(Editor.Document))
                .Where(e => e.EndOffset < Editor.Document.TextLength)
                .OrderBy(e => e.StartOffset)
                .ToArray();
            if (firstErrorRange != null)
            {
                firstErrorOffset = Editor.Document.GetOffset(firstErrorRange.Start.Line, 1);
            }

            try
            {
                _foldings.UpdateFoldings(newFoldings, firstErrorOffset);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(exception.ToString());
                _foldings.Clear();
            }
        });
    }

    private void UpdateDiagnostics()
    {
        Dispatcher.Invoke(() =>
        {
            _markers.RemoveAll(e => true);
            foreach (var diagnostic in ViewModel.CodeDocumentState.Diagnostics)
            {
                diagnostic.WithTextMarker(Editor, _markers, ViewModel.ShowSettingsCommand!, GoToHelpUrlCommand);
            }
            _margin.InvalidateVisual();
        });
    }

    private BindableTextEditor Editor { get; }
    private ICodeDocumentTabViewModel ViewModel { get; set; } = default!;

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
        UpdateStatusInfo();
        HideMarkerToolTip();
    }

    private void OnSelectionChanged(object? sender, EventArgs e)
    {
        UpdateStatusInfo();
        HideMarkerToolTip();
    }
}
