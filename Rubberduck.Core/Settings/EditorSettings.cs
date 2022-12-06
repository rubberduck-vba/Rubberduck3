using Rubberduck.UI.Abstract;

namespace Rubberduck.Settings
{
    public class EditorSettings : IEditorSettings
    {
        public string FontFamily { get; set; } = "Consolas";
        public string FontSize { get; set; }
        public bool ShowLineNumbers { get; set; } = true;
        public double IdleTimeoutSeconds { get; set; } = 0.35;
    }
}
