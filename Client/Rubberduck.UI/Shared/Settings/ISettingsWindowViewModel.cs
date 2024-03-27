using Rubberduck.UI.Shared.Settings.Abstract;

namespace Rubberduck.UI.Shared.Settings
{
    public interface ISettingsWindowViewModel : ICommandBindingProvider
    {
        ISettingGroupViewModel Settings { get; }
        ISettingGroupViewModel Selection { get; set; }
    }
}
