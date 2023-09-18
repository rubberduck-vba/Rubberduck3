using System;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using Rubberduck.UI.Abstract;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Editing;

namespace Rubberduck.UI.Xaml.Controls
{
    public class CompletionInfo : ICompletionData
    {
        public CompletionInfo(IMemberInfoViewModel memberInfo)
        {
            Text = memberInfo.Name;
            Content = memberInfo.Name; // TODO make a nice XAML control for this
        }

        public ImageSource Image { get; }

        public string Text { get; }

        public object Content { get; }

        public object Description { get; }

        public double Priority { get; }

        public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
        {
            textArea.Document.Replace(completionSegment, Text);
        }
    }
}
