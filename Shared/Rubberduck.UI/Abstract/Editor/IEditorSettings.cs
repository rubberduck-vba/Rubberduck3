namespace Rubberduck.UI.Abstract.Editor
{
    public interface IEditorSettings
    {
        string FontFamily { get; set; }
        string FontSize { get; set; }
        bool ShowLineNumbers { get; set; }

        double IdleTimeoutSeconds { get; set; }
    }
}
