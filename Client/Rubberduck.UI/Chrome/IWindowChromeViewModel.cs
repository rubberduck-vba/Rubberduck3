namespace Rubberduck.UI.Chrome
{
    public interface IWindowChromeViewModel
    {
        bool ExtendWindowChrome { get; }
        bool ShowChromeCaptionBar { get; }
        bool CanMaximize { get; }
        bool CanMinimize { get; }
    }
}
