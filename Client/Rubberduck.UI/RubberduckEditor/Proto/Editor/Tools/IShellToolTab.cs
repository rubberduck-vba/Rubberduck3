namespace Rubberduck.UI.RubberduckEditor.Proto.Editor.Tools
{
    public interface IShellToolTab
    {
        string Name { get; }
        object ViewModel { get; }
        IShellToolTabSetting Settings { get; }
    }
}
