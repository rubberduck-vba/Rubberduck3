namespace Rubberduck.UI.Abstract
{
    public interface IShellToolTabSetting
    {
        ToolTabLocation TabPanelLocation { get; set; }
        bool IsLoadedAtStartup { get; set; }
    }
}
