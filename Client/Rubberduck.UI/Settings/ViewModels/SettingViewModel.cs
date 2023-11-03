using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.ServerPlatform;
using Rubberduck.InternalApi.Settings;
using Rubberduck.SettingsProvider.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Rubberduck.UI.Settings.ViewModels
{
    public interface ISettingViewModel : INotifyPropertyChanged
    {
        string Name { get; }
        string Description { get; }
        SettingTags Tags { get; }
        RubberduckSetting ToSetting();
    }

    public interface ISettingViewModel<TValue> : ISettingViewModel
    {
        TValue Value { get; set; }
    }

    public interface ISettingGroupViewModel : ISettingViewModel
    {
        public IEnumerable<ISettingViewModel> Items { get; }
    }

    public abstract class SettingViewModel<TValue> : ViewModelBase, ISettingViewModel<TValue>
    {
        private readonly TypedRubberduckSetting<TValue> _setting;

        protected SettingViewModel(TypedRubberduckSetting<TValue> setting) 
        {
            _setting = setting;
            _value = setting.TypedValue;
        }

        public string Name => _setting.Key; // TODO fetch from resources
        public string Description => _setting.Key; // TODO fetch from resources
        public SettingTags Tags => _setting.Tags;


        private TValue _value;
        public TValue Value 
        {
            get => _value;
            set
            {
                if (!_value.Equals(value))
                {
                    _value = value ?? throw new ArgumentNullException(nameof(value));
                    OnPropertyChanged();
                }
            }
        }

        public RubberduckSetting ToSetting() => _setting with { Value = Value };
    }

    public class StringValueViewModel : ViewModelBase
    {
        public StringValueViewModel(string value)
        {
            _value = value;
        }

        private string _value;
        public string Value 
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    _value = value;
                    OnPropertyChanged();
                }
            }
        }
    }

    public class ListSettingViewModel : SettingViewModel<string[]>
    {
        public ListSettingViewModel(TypedRubberduckSetting<string[]> setting) : base(setting)
        {
        }
    }

    public class BooleanSettingViewModel : SettingViewModel<bool>
    {
        public BooleanSettingViewModel(BooleanRubberduckSetting setting) : base(setting)
        {
        }
    }

    public class StringSettingViewModel : SettingViewModel<string>
    {
        public StringSettingViewModel(StringRubberduckSetting setting) : base(setting)
        {
        }
    }

    public class TimeSpanSettingViewModel : SettingViewModel<TimeSpan>
    {
        public TimeSpanSettingViewModel(TypedRubberduckSetting<TimeSpan> setting) : base(setting)
        {
        }
    }

    public class NumericSettingViewModel : SettingViewModel<double>
    {
        public NumericSettingViewModel(NumericRubberduckSetting setting) : base(setting)
        {
        }
    }

    public class UriSettingViewModel : SettingViewModel<Uri>
    {
        public UriSettingViewModel(UriRubberduckSetting setting) : base(setting)
        {
        }
    }

    public abstract class EnumValueSettingViewModel<TEnum> : SettingViewModel<TEnum>
        where TEnum : struct, Enum
    {
        protected static IEnumerable<TEnum> Members { get; } = Enum.GetValues<TEnum>();

        protected EnumValueSettingViewModel(TypedRubberduckSetting<TEnum> setting) : base(setting)
        {
        }

        public IEnumerable<TEnum> Values { get; } = Members;
    }

    public class LogLevelSettingViewModel : EnumValueSettingViewModel<LogLevel>
    {
        public LogLevelSettingViewModel(TypedRubberduckSetting<LogLevel> setting) : base(setting)
        {
        }
    }

    public class ServerMessageModeSettingViewModel : EnumValueSettingViewModel<MessageMode>
    {
        public ServerMessageModeSettingViewModel(TypedRubberduckSetting<MessageMode> setting) : base(setting)
        {
        }
    }

    public class ServerTransportTypeSettingViewModel : EnumValueSettingViewModel<TransportType>
    {
        public ServerTransportTypeSettingViewModel(TypedRubberduckSetting<TransportType> setting) : base(setting)
        {
        }
    }

    public class MessageTraceLevelSettingViewModel : EnumValueSettingViewModel<MessageTraceLevel>
    {
        public MessageTraceLevelSettingViewModel(TypedRubberduckSetting<MessageTraceLevel> setting) : base(setting)
        {
        }
    }

    public class SettingGroupViewModel : ViewModelBase, ISettingGroupViewModel
    {
        private readonly TypedSettingGroup _settingGroup;

        public SettingGroupViewModel(TypedSettingGroup settingGroup, IEnumerable<ISettingViewModel> items)
        {
            _settingGroup = settingGroup;
            Items = items;
        }

        public IEnumerable<ISettingViewModel> Items { get; init; }

        public string Name => _settingGroup.Key; // TODO fetch from resources

        public string Description => _settingGroup.Key; // TODO fetch from resources

        public SettingTags Tags => _settingGroup.Tags;

        public RubberduckSetting ToSetting() => _settingGroup with { Value = Items.Select(e => e.ToSetting()).ToArray() };
    }
}
