using Rubberduck.UI.Abstract.Editor;

namespace Rubberduck.UI.Abstract.Editor.Tools
{
    public interface IShellToolTabSetting
    {
        ToolTabLocation TabPanelLocation { get; set; }
        bool IsLoadedAtStartup { get; set; }
    }
}
