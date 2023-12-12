using Rubberduck.SettingsProvider.Model.Editor.Tools;
using System.Windows.Input;

namespace Rubberduck.UI.Windows
{
    public interface IToolWindowViewModel : ITabViewModel
    {
        DockingLocation DockingLocation { get; set; }

        ICommand ShowSettingsCommand { get; }
        string SettingKey { get; }
    }
}
