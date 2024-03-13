using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Rendering;
using Rubberduck.UI.Services.Abstract;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Rubberduck.UI.Shell.Document
{
    /// <summary>
    /// TODO see https://github.com/AvaloniaUI/AvaloniaEdit/blob/master/src/AvaloniaEdit/Editing/LineNumberMargin.cs for how-to
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
            return new Size(24, 0);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            drawingContext.DrawRectangle(new SolidColorBrush(Color.FromRgb(224, 224, 224)), null, new Rect(0,0, ActualWidth, ActualHeight));

            var markersByLine = _service.TextMarkers
                .GroupBy(e => Document.GetLineByOffset(e.StartOffset).LineNumber)
                .ToDictionary(e => e.Key, e => e.AsEnumerable());

            foreach (var line in TextView.VisualLines)
            {
                var documentLine = line.FirstDocumentLine;
                if (markersByLine.TryGetValue(documentLine.LineNumber, out var markers))
                {
                    var marker = markers.First();
                    var visualColumn = line.GetVisualColumn(marker.StartOffset);

                    var visualPosition = line.GetVisualPosition(visualColumn, VisualYPosition.LineTop);
                    var rect = new Rect
                    {
                        Width = 16,
                        Height = 16,
                        X = 1,
                        Y = visualPosition.Y
                    };
                    
                    drawingContext.DrawImage(WarningIcon, rect);
                }
            }
        }
    }
}
