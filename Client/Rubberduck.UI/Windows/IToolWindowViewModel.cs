using Rubberduck.SettingsProvider.Model.Editor.Tools;
using System.Windows.Input;

namespace Rubberduck.UI.Windows
{
    public interface IToolWindowViewModel : ITabViewModel
    {
        DockingLocation DockingLocation { get; set; }

        ICommand ShowSettingsCommand { get; }
        string ShowSettingsCommandParameter { get; }
        ICommand CloseToolWindowCommand { get; }
        string SettingKey { get; }

        bool ShowPinButton { get; }
        bool ShowGearButton { get; }
        bool ShowCloseButton { get; }
    }
}
