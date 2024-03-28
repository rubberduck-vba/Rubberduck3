using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Rendering;
using Rubberduck.UI.Services;
using Rubberduck.UI.Services.Abstract;
using Rubberduck.Unmanaged.Model;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.TextFormatting;

namespace Rubberduck.UI.Shell.Document
{
    /// <summary>
    /// See https://github.com/AvaloniaUI/AvaloniaEdit/blob/master/src/AvaloniaEdit/Editing/LineNumberMargin.cs for how-to
    /// </summary>
    public class TextMarkersMargin : AbstractMargin
    {
        private readonly ITextMarkerService _service;

        private ImageSource HintIcon { get; }
        private ImageSource InfoIcon { get; }
        private ImageSource WarningIcon { get; }
        private ImageSource ErrorIcon { get; }

        public TextMarkersMargin(ITextMarkerService service)
        {
            _service = service;
            HintIcon = new BitmapImage(new Uri("pack://application:,,,/Rubberduck.UI;component/Resources/FugueIcons/information-white.png"));
            InfoIcon = new BitmapImage(new Uri("pack://application:,,,/Rubberduck.UI;component/Resources/FugueIcons/information.png"));
            WarningIcon = new BitmapImage(new Uri("pack://application:,,,/Rubberduck.UI;component/Resources/FugueIcons/exclamation-diamond.png"));
            ErrorIcon = new BitmapImage(new Uri("pack://application:,,,/Rubberduck.UI;component/Resources/FugueIcons/cross-circle.png"));
        }

        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
        {
            return new PointHitTestResult(this, hitTestParameters.HitPoint);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            return new Size(MarginWidth, 0);
        }

        private static readonly int MarginWidth = 24;
        private static readonly int IconWidth = 16;
        private static readonly int IconPositionX = (MarginWidth - IconWidth) / 2;

        protected override void OnRender(DrawingContext drawingContext)
        {
            TextView textView = base.TextView;
            Size renderSize = base.RenderSize;
            if (textView == null || !textView.VisualLinesValid)
            {
                return;
            }

            drawingContext.DrawRectangle(new SolidColorBrush(Color.FromRgb(224, 224, 224)), null, new Rect(0,0, ActualWidth, ActualHeight));

            var markersByLine = _service.TextMarkers
                .GroupBy(e => Document.GetLineByOffset(e.StartOffset).LineNumber)
                .ToDictionary(e => e.Key, e => e.AsEnumerable());

            foreach (var visualLine in TextView.VisualLines)
            {
                var documentLine = visualLine.FirstDocumentLine;
                if (markersByLine.TryGetValue(documentLine.LineNumber, out var markers))
                {
                    var marker = markers.First();
                    var visualYPosition = visualLine.GetTextLineVisualYPosition(visualLine.TextLines[0], VisualYPosition.TextTop);
                    var rect = new Rect
                    {
                        Width = IconWidth,
                        Height = IconWidth, // not a typo, it's 16x16
                        X = IconPositionX,
                        Y = visualYPosition - textView.VerticalOffset
                    };

                    var icon = WarningIcon;
                    if (marker.MarkerTypes.HasFlag(TextMarkerTypes.DottedUnderline))
                    {
                        icon = HintIcon;
                    }
                    else if (marker.MarkerTypes.HasFlag(TextMarkerTypes.SquigglyUnderline))
                    {
                        if (marker.MarkerColor == TextMarkerExtensions.InformationMarkerColor)
                        {
                            icon = InfoIcon;
                        }
                        else if (marker.MarkerColor == TextMarkerExtensions.WarningMarkerColor)
                        {
                            icon = WarningIcon;
                        }
                        else if (marker.MarkerColor == TextMarkerExtensions.ErrorMarkerColor)
                        {
                            icon = ErrorIcon;
                        }
                    }
                    drawingContext.DrawImage(icon, rect);
                }
            }
        }

        private TextArea? _textArea;

        protected override void OnTextViewChanged(TextView oldTextView, TextView newTextView)
        {
            if (oldTextView != null)
            {
                oldTextView.VisualLinesChanged -= TextViewVisualLinesChanged;
            }

            base.OnTextViewChanged(oldTextView, newTextView);
            if (newTextView != null)
            {
                newTextView.VisualLinesChanged += TextViewVisualLinesChanged;
                _textArea = newTextView.GetService(typeof(TextArea)) as TextArea;
            }
            else
            {
                _textArea = null;
            }

            InvalidateVisual();
        }

        private void TextViewVisualLinesChanged(object? sender, EventArgs e)
        {
            InvalidateVisual();
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (e.Handled || base.TextView == null || _textArea == null)
            {
                return;
            }

            e.Handled = true;
            _textArea.Focus();

            if (CaptureMouse())
            {
                // TODO
            }
        }
    }
}
