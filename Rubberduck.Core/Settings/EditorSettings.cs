//using Rubberduck.UI.CodeExplorer;
//using Rubberduck.UI.Inspections;
//using Rubberduck.UI.ToDoItems;
//using Rubberduck.UI.UnitTesting;

using Rubberduck.UI.Abstract;

namespace Rubberduck.Settings
{
    public class EditorSettings : IEditorSettings
    {
        public string FontFamily { get; set; } = "Consolas";
        public string FontSize { get; set; }
        public bool ShowLineNumbers { get; set; } = true;
    }
}
