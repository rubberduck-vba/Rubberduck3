// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Threading;
using System.Linq;
using System.Windows;
using System.Diagnostics;

namespace Rubberduck.UI.RubberduckEditor.TextTransform
{
    public class TextMarkerService : DocumentColorizingTransformer, IBackgroundRenderer, ITextMarkerService, ITextViewConnect
    {
        private readonly TextSegmentCollection<TextMarker> _markers = new();
        private readonly HashSet<(int startOffset, int length)> _markerPositions = new();
        private readonly List<TextView> _textViews = new();
        private readonly TextDocument _document;

        public event EventHandler RedrawRequested = delegate { };

        public TextMarkerService(TextDocument document)
        {
            _document = document ?? throw new ArgumentNullException(nameof(document));
        }

        #region ITextMarkerService
        public ITextMarker Create(int startOffset, int length)
        {
            var textLength = _document.TextLength;
            if (startOffset < 0 || startOffset > textLength)
            {
                throw new ArgumentOutOfRangeException(nameof(startOffset), $"Value must be between 0 and {textLength}");
            }
            if (length < 0 || startOffset + length > textLength)
            {
                startOffset = textLength - length;
            }

            if (_markerPositions.Add((startOffset, length)))
            {
                var marker = new TextMarker(this, startOffset, length);
                _markers.Add(marker);

                return marker;
            }

            return null!;
        }

        public IEnumerable<ITextMarker> GetMarkersAtOffset(int offset)
        {
            return _markers.FindSegmentsContaining(offset);
        }

        public IEnumerable<ITextMarker> TextMarkers => _markers;

        public void RemoveAll(Predicate<ITextMarker> predicate)
        {
            if (predicate is null)
            {
                throw new ArgumentNullException(nameof(predicate), "Predicate cannot be null");
            }
            foreach (var marker in _markers.ToList())
            {
                if (predicate.Invoke(marker))
                {
                    Remove(marker);
                    _markerPositions.Remove((marker.StartOffset, marker.Length));
                }
            }
        }

        public void Remove(ITextMarker marker)
        {
            if (marker is null)
            {
                throw new ArgumentNullException(nameof(marker), "Marker cannot be null");
            }
            if (marker is TextMarker textMarker && _markers.Remove(textMarker))
            {
                Redraw(textMarker);
                textMarker.OnDeleted();
            }
        }

        /// <summary>
        /// Redraws the specified text segment.
        /// </summary>
        internal void Redraw(ISegment segment)
        {
            foreach (var view in _textViews)
            {
                view.Redraw(segment, DispatcherPriority.Normal);
            }
            RedrawRequested?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region DocumentColorizingTransformer
        protected override void ColorizeLine(DocumentLine line)
        {
            var lineStart = line.Offset;
            var lineEnd = lineStart + line.Length;

            foreach (var marker in _markers.FindOverlappingSegments(lineStart, line.Length))
            {
                Brush foregroundBrush = null!;
                if (marker.ForegroundColor != null)
                {
                    foregroundBrush = new SolidColorBrush(marker.ForegroundColor.Value);
                    foregroundBrush.Freeze();
                }
                ChangeLinePart(Math.Max(marker.StartOffset, lineStart), Math.Min(marker.EndOffset, lineEnd),
                    element =>
                    {
                        if (foregroundBrush != null)
                        {
                            element.TextRunProperties.SetForegroundBrush(foregroundBrush);
                        }
                        var tf = element.TextRunProperties.Typeface;
                        var typeFace = new Typeface(tf.FontFamily, marker.FontStyle ?? tf.Style, marker.FontWeight ?? tf.Weight, tf.Stretch);
                        element.TextRunProperties.SetTypeface(typeFace);
                    });
            }
        }
        #endregion

        #region IBackgroundRenderer
        public KnownLayer Layer => KnownLayer.Selection;

        private static readonly TextMarkerTypes _underlineMarkerTypes = TextMarkerTypes.SquigglyUnderline 
                                                                      | TextMarkerTypes.NormalUnderline 
                                                                      | TextMarkerTypes.DottedUnderline;

        public void Draw(TextView textView, DrawingContext drawingContext)
        {
            textView = textView ?? throw new ArgumentNullException(nameof(textView));
            drawingContext = drawingContext ?? throw new ArgumentNullException(nameof(drawingContext));

            var visualLines = textView.VisualLines;
            if (visualLines.Count == 0)
            {
                return;
            }

            var viewStart = visualLines.First().FirstDocumentLine.Offset;
            var viewEnd = visualLines.Last().LastDocumentLine.EndOffset;

            foreach (var marker in _markers.FindOverlappingSegments(viewStart, viewEnd - viewStart))
            {
                if (marker.BackgroundColor != null)
                {
                    var geoBuilder = new BackgroundGeometryBuilder
                    {
                        AlignToWholePixels = true,
                        CornerRadius = 3
                    };
                    geoBuilder.AddSegment(textView, marker);
                    
                    var geo = geoBuilder.CreateGeometry();
                    if (geo != null)
                    {
                        var color = marker.BackgroundColor.Value;
                        var brush = new SolidColorBrush(color);
                        brush.Freeze();
                        drawingContext.DrawGeometry(brush, null, geo);
                    }
                }

                if ((marker.MarkerTypes & _underlineMarkerTypes) != 0)
                {
                    foreach (var rect in BackgroundGeometryBuilder.GetRectsForSegment(textView, marker))
                    {
                        var startPoint = rect.BottomLeft;
                        var endPoint = rect.BottomRight;

                        var markerBrush = new SolidColorBrush(marker.MarkerColor);
                        markerBrush.Freeze();

                        if ((marker.MarkerTypes & TextMarkerTypes.SquigglyUnderline) != 0)
                        {
                            var offset = 2.25d;
                            var count = Math.Max((int)((endPoint.X - startPoint.X) / offset) + 1, 4);
                            
                            var geometry = new StreamGeometry();
                            using (var context = geometry.Open())
                            {
                                context.BeginFigure(startPoint, false, false);
                                context.PolyLineTo(CreatePoints(startPoint, endPoint, offset, count).ToArray(), true, false);
                            }
                            geometry.Freeze();

                            var markerPen = new Pen(markerBrush, 1);
                            markerPen.Freeze();

                            drawingContext.DrawGeometry(Brushes.Transparent, markerPen, geometry);
                        }
                        if ((marker.MarkerTypes & TextMarkerTypes.NormalUnderline) != 0)
                        {
                            var markerPen = new Pen(markerBrush, 0.5);
                            markerPen.Freeze();
                            drawingContext.DrawLine(markerPen, startPoint, endPoint);
                        }
                        if ((marker.MarkerTypes & TextMarkerTypes.DottedUnderline) != 0)
                        {
                            var markerPen = new Pen(markerBrush, 0.5)
                            {
                                DashStyle = DashStyles.Dash
                            };
                            markerPen.Freeze();
                            drawingContext.DrawLine(markerPen, startPoint, endPoint);
                        }
                    }
                }
            }
        }

        private static IEnumerable<Point> CreatePoints(Point start, Point _, double offset, int count)
        {
            for (var i = 0; i < count; i++)
            {
                yield return new Point(start.X + i * offset, start.Y - ((i + 1) % 2 == 0 ? offset : 0));
            }
        }
        #endregion

        #region ITextViewConnect
        void ITextViewConnect.AddToTextView(TextView textView)
        {
            textView = textView ?? throw new ArgumentNullException(nameof(textView));

            if (!_textViews.Contains(textView))
            {
                Debug.Assert(textView.Document == _document);
                _textViews.Add(textView);
            }
        }

        void ITextViewConnect.RemoveFromTextView(TextView textView)
        {
            textView = textView ?? throw new ArgumentNullException(nameof(textView));

            Debug.Assert(textView.Document == _document);
            _textViews.Remove(textView);
        }
        #endregion
    }
}
