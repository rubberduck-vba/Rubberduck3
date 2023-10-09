namespace Rubberduck.UI.Xaml.Shell.Document
{
    public interface ISyntaxErrorViewModel
    {
        string Message { get; }
        int StartOffset { get; }
        int Length { get; }
        string LocationMessage { get; }
        string ModuleName { get; }
        int Line { get; }
        int Column { get; }
    }
}
