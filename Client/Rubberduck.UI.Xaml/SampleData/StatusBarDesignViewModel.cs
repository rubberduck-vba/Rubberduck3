using Rubberduck.UI.Abstract;

///<summary>
///This file provides design-time data for the <see cref="StatusBarControl.xaml"/> control.
///This is only to support working in the XAML designer and nothing in this file should be used otherwise.
///</summary>
namespace Rubberduck.UI.Xaml.Controls
{
    internal class StatusBarDesignViewModel : IStatusBarViewModel
    {
        public int DocumentLines { get; set; } = 100;
        public int DocumentLength { get; set; } = 1000;
        public int CaretOffset { get; set; } = 5;
        public int CaretLine { get; set; } = 1;
        public int CaretColumn { get; set; } = 5;
        public int IssuesCount { get; set; } = 3;
        public string ParserState { get; set; } = "Ready";
        public int ProgressValue { get; set; } = 50;
        public int ProgressMaxValue { get; set; } = 100;
        public bool ShowDocumentStatusItems { get; set; } = true;
        public string CodeLocation { get; set; } = "Module1";
    }
}
