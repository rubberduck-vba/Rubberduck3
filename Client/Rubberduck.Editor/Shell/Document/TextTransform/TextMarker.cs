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
using System;
using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;

namespace Rubberduck.Editor.Shell.Document.TextTransform
{
    public sealed class TextMarker : TextSegment, ITextMarker
    {
        private readonly TextMarkerService _service;

        public TextMarker(TextMarkerService service, int startOffset, int length)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _markerTypes = TextMarkerTypes.None;

            StartOffset = startOffset;
            Length = length;
        }

        public event EventHandler Deleted = delegate { };

        public bool IsDeleted
        {
            get { return !IsConnectedToCollection; }
        }

        public void Delete()
        {
            _service.Remove(this);
        }

        internal void OnDeleted()
        {
            Deleted?.Invoke(this, EventArgs.Empty);
        }

        void Redraw()
        {
            _service.Redraw(this);
        }

        private Color? _backgroundColor;

        public Color? BackgroundColor
        {
            get { return _backgroundColor; }
            set
            {
                if (_backgroundColor != value)
                {
                    _backgroundColor = value;
                    Redraw();
                }
            }
        }

        private Color? _foregroundColor;

        public Color? ForegroundColor
        {
            get { return _foregroundColor; }
            set
            {
                if (_foregroundColor != value)
                {
                    _foregroundColor = value;
                    Redraw();
                }
            }
        }

        private FontWeight? _fontWeight;

        public FontWeight? FontWeight
        {
            get { return _fontWeight; }
            set
            {
                if (_fontWeight != value)
                {
                    _fontWeight = value;
                    Redraw();
                }
            }
        }

        private FontStyle? _fontStyle;

        public FontStyle? FontStyle
        {
            get { return _fontStyle; }
            set
            {
                if (_fontStyle != value)
                {
                    _fontStyle = value;
                    Redraw();
                }
            }
        }

        public object? Tag { get; set; }

        private TextMarkerTypes _markerTypes;

        public TextMarkerTypes MarkerTypes
        {
            get { return _markerTypes; }
            set
            {
                if (_markerTypes != value)
                {
                    _markerTypes = value;
                    Redraw();
                }
            }
        }

        private Color _markerColor;

        public Color MarkerColor
        {
            get { return _markerColor; }
            set
            {
                if (_markerColor != value)
                {
                    _markerColor = value;
                    Redraw();
                }
            }
        }

        public object? ToolTip { get; set; }
    }
}
