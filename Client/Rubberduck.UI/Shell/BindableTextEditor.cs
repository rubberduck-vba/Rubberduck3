using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Xml;
using System.Windows;

namespace Rubberduck.UI.Shell
{
    //see http://stackoverflow.com/a/20823917/4088852
    public class BindableTextEditor : TextEditor, INotifyPropertyChanged
    {
        public BindableTextEditor()
        {
            WordWrap = false;

            var highlighter = LoadHighlighter("Rubberduck.Editor.vba-default.xshd");
            SyntaxHighlighting = highlighter;

            ////Style hyperlinks so they look like comments. Note - this needs to move if used for user code.
            //TextArea.TextView.LinkTextUnderline = false;
            //TextArea.TextView.LinkTextForegroundBrush = new SolidColorBrush(Colors.Green);
            Options.RequireControlModifierForHyperlinkClick = true;
            Options.EnableHyperlinks = true;
            Options.EnableEmailHyperlinks = false;
            Options.AllowScrollBelowDocument = true; // should be false if folding is disabled
            Options.ConvertTabsToSpaces = true;
            Options.HighlightCurrentLine = true;
            Options.HideCursorWhileTyping = true;
            Options.ShowColumnRuler = false;
        }

        public new string Text
        {
            get => base.Text;
            set => base.Text = value;
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(BindableTextEditor), new PropertyMetadata((obj, args) =>
            {
                var target = (BindableTextEditor)obj;
                target.Text = (string)args.NewValue;
            }));

        protected override void OnTextChanged(EventArgs e)
        {
            RaisePropertyChanged(nameof(Text));
            base.OnTextChanged(e);
        }

        public void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private static IHighlightingDefinition LoadHighlighter(string resource)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream(resource);
            if (stream is null)
            {
                return null!;
            }

            using var reader = new XmlTextReader(stream);
            return HighlightingLoader.Load(reader, HighlightingManager.Instance);
        }
    }
}
