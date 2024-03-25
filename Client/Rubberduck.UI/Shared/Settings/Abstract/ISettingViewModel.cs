using Rubberduck.InternalApi.Settings.Model;
using System.ComponentModel;

namespace Rubberduck.UI.Shared.Settings.Abstract
{
    public interface ISettingViewModel : INotifyPropertyChanged, ISearchable
    {
        bool IsSettingGroup { get; }
        string Key { get; }
        string SettingGroupKey { get; set; }
        bool ShowSettingGroup { get; set; }
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
