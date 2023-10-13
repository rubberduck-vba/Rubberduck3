using Rubberduck.UI.RubberduckEditor.Proto.Editor;

namespace Rubberduck.UI.RubberduckEditor.Proto.Editor.Tools
{
    public interface IShellToolTabSetting
    {
        ToolTabLocation TabPanelLocation { get; set; }
        bool IsLoadedAtStartup { get; set; }
    }
}
