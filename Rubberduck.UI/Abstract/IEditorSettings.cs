namespace Rubberduck.UI.Abstract
{
    public interface IEditorSettings
    {
        string FontFamily { get; set; }
        string FontSize { get; set; }
        bool ShowLineNumbers { get; set; }
    }
}
