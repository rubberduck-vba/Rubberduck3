using Rubberduck.SettingsProvider.Model;
using System.ComponentModel;

namespace Rubberduck.UI.Shared.Settings.Abstract
{
    public interface ISettingViewModel : INotifyPropertyChanged
    {
        string Key { get; }
        SettingDataType SettingDataType { get; }
        string Name { get; }
        string Description { get; }
        SettingTags Tags { get; }
        RubberduckSetting ToSetting();
    }

    public interface ISettingViewModel<TValue> : ISettingViewModel
    {
        TValue Value { get; set; }
    }
}
