using Rubberduck.SettingsProvider.Model;
using Rubberduck.SettingsProvider.Model.Logging;
using Rubberduck.SettingsProvider.Model.ServerStartup;
using Rubberduck.UI.Settings.ViewModels.Abstract;
using System;

namespace Rubberduck.UI.Services.Settings
{
    public interface ISettingViewModelFactory
    {
        ISettingViewModel CreateViewModel(BooleanRubberduckSetting setting);
        ISettingViewModel CreateViewModel(NumericRubberduckSetting setting);
        ISettingViewModel CreateViewModel(StringRubberduckSetting setting);
        ISettingViewModel CreateViewModel(UriRubberduckSetting setting);

        ISettingViewModel CreateViewModel(LogLevelSetting setting);
        ISettingViewModel CreateViewModel(TraceLevelSetting setting);
        ISettingViewModel CreateViewModel(ServerTransportTypeSetting setting);
        ISettingViewModel CreateViewModel(ServerMessageModeSetting setting);
        ISettingViewModel CreateViewModel(TypedRubberduckSetting<TimeSpan> setting);
        ISettingViewModel CreateViewModel(TypedRubberduckSetting<string[]> setting);

        ISettingGroupViewModel CreateViewModel(TypedSettingGroup settingGroup);
    }
}
