using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Rubberduck.UI.Behaviors
{
    //adapted from https://serialseb.com/blog/2007/09/03/wpf-tips-6-preventing-scrollviewer-from/
    public class ScrollViewerCorrector
    {
        public static bool GetFixScrolling(DependencyObject obj)
        {
            return (bool)obj.GetValue(FixScrollingProperty);
        }

        public static void SetFixScrolling(DependencyObject obj, bool value)
        {
            obj.SetValue(FixScrollingProperty, value);
        }

        public static readonly DependencyProperty FixScrollingProperty =
            DependencyProperty.RegisterAttached(nameof(FixScrollingProperty), typeof(bool), typeof(ScrollViewerCorrector), new FrameworkPropertyMetadata(false, OnFixScrollingPropertyChanged));

        public static void OnFixScrollingPropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is not ScrollViewer viewer)
            {
                throw new NotSupportedException("The dependency property can only be attached to a ScrollViewer");
            }

            if ((bool)e.NewValue == true)
            {
                viewer.PreviewMouseWheel += HandlePreviewMouseWheel;
            }
            else
            {
                viewer.PreviewMouseWheel -= HandlePreviewMouseWheel;
            }
        }

        private static readonly List<MouseWheelEventArgs> _avoidReentry = new();

        private static void HandlePreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!e.Handled && sender is ScrollViewer scrollControl && !_avoidReentry.Contains(e))
            {
                var previewEventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
                {
                    RoutedEvent = UIElement.PreviewMouseWheelEvent,
                    Source = sender
                };

                _avoidReentry.Add(previewEventArg);
                if (e.OriginalSource is UIElement originalSource)
                {
                    originalSource.RaiseEvent(previewEventArg);
                }
                _avoidReentry.Remove(previewEventArg);

                bool isAlreadyAtTop = e.Delta > 0 && scrollControl.VerticalOffset == 0;
                bool isAlreadyAtBottom = e.Delta <= 0 && scrollControl.VerticalOffset >= scrollControl.ExtentHeight - scrollControl.ViewportHeight;
                if (!previewEventArg.Handled && (isAlreadyAtTop || isAlreadyAtBottom))
                {
                    var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta)
                    {
                        RoutedEvent = UIElement.MouseWheelEvent,
                        Source = sender
                    };
                    var parent = ((Control)sender).Parent as UIElement;
                    parent?.RaiseEvent(eventArg);
                }
            }
        }
    }
}
