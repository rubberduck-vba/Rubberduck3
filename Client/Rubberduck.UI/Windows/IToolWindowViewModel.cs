using Rubberduck.InternalApi.Settings.Model.Editor.Tools;
using System.Windows.Input;

namespace Rubberduck.UI.Windows
{
    public interface IToolWindowViewModel : ITabViewModel
    {
        DockingLocation DockingLocation { get; set; }

        ICommand? ShowSettingsCommand { get; }
        string? ShowSettingsCommandParameter { get; }
        string SettingKey { get; }

        ICommand? CloseToolWindowCommand { get; }

        bool ShowPinButton { get; }
        bool ShowGearButton { get; }
        bool ShowCloseButton { get; }
    }
}
