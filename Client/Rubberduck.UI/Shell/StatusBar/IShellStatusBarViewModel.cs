namespace Rubberduck.UI.Shell.StatusBar
{
    /// <summary>
    /// An interface representing the shell window status bar.
    /// </summary>
    public interface IShellStatusBarViewModel : 
        ILanguageServerStatusViewModel, 
        IProgressStatusViewModel, 
        INotificationStatusViewModel
    {
        IDocumentStatusViewModel ActiveDocumentStatus { get; }
    }
}
