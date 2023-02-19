namespace Rubberduck.UI.Abstract
{
    public interface IShellToolTab
    {
        string Name { get; }
        object ViewModel { get; }
        IShellToolTabSetting Settings { get; }
    }
}
