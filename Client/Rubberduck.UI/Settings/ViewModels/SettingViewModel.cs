using Microsoft.Extensions.Logging;
using Rubberduck.InternalApi.ServerPlatform;
using Rubberduck.InternalApi.Settings;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.UI.Command;
using Rubberduck.UI.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace Rubberduck.UI.Settings.ViewModels
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

    public interface ISettingGroupViewModel : ISettingViewModel
    {
        public ObservableCollection<ISettingViewModel> Items { get; }
    }

    public abstract class SettingViewModel<TValue> : ViewModelBase, ISettingViewModel<TValue>
    {
        private readonly TypedRubberduckSetting<TValue> _setting;

        protected SettingViewModel(TypedRubberduckSetting<TValue> setting) 
        {
            _setting = setting;
            _value = setting.TypedValue;

            IsEnabled = !IsReadOnlyRecommended;
        }

        public SettingDataType SettingDataType => _setting.SettingDataType;
        public string Key => _setting.Key;
        public string Name => _setting.Key; // TODO fetch from resources
        public string Description => _setting.Key; // TODO fetch from resources
        
        public SettingTags Tags => _setting.Tags;
        public bool IsReadOnlyRecommended => Tags.HasFlag(SettingTags.ReadOnlyRecommended);
        public bool IsAdvancedSetting => Tags.HasFlag(SettingTags.Advanced);
        public bool IsExperimental => Tags.HasFlag(SettingTags.Experimental);

        private bool _isEnabled;
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    OnPropertyChanged();
                }
            }
        }


        private TValue _value;
        public TValue Value 
        {
            get => _value;
            set
            {
                if (_value is null || !_value.Equals(value))
                {
                    _value = value ?? throw new ArgumentNullException(nameof(value), "Value cannot be null.");
                    OnPropertyChanged();
                }
            }
        }

        public RubberduckSetting ToSetting() => _setting with { Value = Value ?? throw new InvalidOperationException("Value is unexpectedly null.") };
    }

    public class ListSettingViewModel : SettingViewModel<string[]>
    {
        public ListSettingViewModel(UIServiceHelper service, TypedRubberduckSetting<string[]> setting) : base(setting)
        {
            ListItems = new ObservableCollection<string>(setting.TypedValue);
            RemoveListSettingItemCommand = new DelegateCommand(service, ExecuteRemoveListSettingItemCommand);
        }

        public ObservableCollection<string> ListItems { get; }

        public ICommand RemoveListSettingItemCommand { get; }

        private void ExecuteRemoveListSettingItemCommand(object? parameter)
        {
            if (parameter is string value)
            {
                ListItems.Remove(value);
                Value = ListItems.ToArray();
            }
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
            var value = setting.TypedValue;
            _hours = value.Hours;
            _minutes = value.Minutes;
            _seconds = value.Seconds;
            _milliseconds = value.Milliseconds;
        }

        private void UpdateValue()
        {
            Value = new TimeSpan(0, _hours, _minutes, _seconds, _milliseconds);
        }

        private int _hours;
        public int Hours
        {
            get => _hours;
            set
            {
                if (_hours != value)
                {
                    _hours = value;
                    OnPropertyChanged();
                    UpdateValue();
                }
            }
        }

        private int _minutes;
        public int Minutes
        {
            get => _minutes;
            set
            {
                if (_minutes != value)
                {
                    _minutes = value;
                    OnPropertyChanged();
                    UpdateValue();
                }
            }
        }

        private int _seconds;
        public int Seconds
        {
            get => _seconds;
            set
            {
                if (_seconds != value)
                {
                    _seconds = value;
                    OnPropertyChanged();
                    UpdateValue();
                }
            }
        }

        private int _milliseconds;
        public int Milliseconds
        {
            get => _milliseconds;
            set
            {
                if (_milliseconds != value)
                {
                    _milliseconds = value;
                    OnPropertyChanged();
                    UpdateValue();
                }
            }
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

    public abstract class EnumValueSettingViewModel : ViewModelBase, ISettingViewModel
    {
        private readonly RubberduckSetting _setting;

        protected EnumValueSettingViewModel(RubberduckSetting setting)
        {
            _setting = setting;
            IsEnabled = !IsReadOnlyRecommended;
        }

        public SettingDataType SettingDataType => _setting.SettingDataType;
        public string Key => _setting.Key;
        public string Name => _setting.Key; // TODO fetch from resources
        public string Description => _setting.Key; // TODO fetch from resources
        public SettingTags Tags => _setting.Tags;

        private bool _isEnabled;
        public bool IsEnabled 
        {
            get => _isEnabled;
            set
            {
                if (_isEnabled != value) 
                {
                    _isEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        public abstract RubberduckSetting ToSetting();

        public abstract IEnumerable<string> Values { get; }

        public bool IsReadOnlyRecommended => Tags.HasFlag(SettingTags.ReadOnlyRecommended);
        public bool IsAdvancedSetting => Tags.HasFlag(SettingTags.Advanced);
        public bool IsExperimental => Tags.HasFlag(SettingTags.Experimental);
    }

    public abstract class EnumValueSettingViewModel<TEnum> :  EnumValueSettingViewModel, ISettingViewModel<TEnum>
        where TEnum : struct, Enum
    {
        protected static IEnumerable<string> Members { get; } = Enum.GetNames<TEnum>();
        private readonly TypedRubberduckSetting<TEnum> _setting;

        protected EnumValueSettingViewModel(TypedRubberduckSetting<TEnum> setting)
            : base(setting)
        {
            _setting = setting;
            _selection = setting.Value.ToString()!;
        }

        public override IEnumerable<string> Values { get; } = Members;

        private string _selection;
        public string Selection
        {
            get => _selection;
            set
            {
                if (_selection != value)
                {
                    _selection = value;
                    OnPropertyChanged();
                    Value = Enum.Parse<TEnum>(value);
                }
            }
        }

        private TEnum _value;
        public TEnum Value 
        {
            get => Enum.Parse<TEnum>(_selection);
            set
            {
                if (!Equals(_value, value))
                {
                    _value = value;
                    OnPropertyChanged();
                }
            }
        }

        public override RubberduckSetting ToSetting() => _setting with { Value = Value };
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
        private readonly RubberduckSetting _settingGroup;

        internal SettingGroupViewModel()
        {
            /* designer */
        }

        public SettingGroupViewModel(TypedSettingGroup settingGroup, IEnumerable<ISettingViewModel> items)
        {
            _settingGroup = settingGroup;
            Items = new ObservableCollection<ISettingViewModel>(items);
        }

        public SettingGroupViewModel(TypedRubberduckSetting<BooleanRubberduckSetting[]> settingGroup, IEnumerable<ISettingViewModel> items)
        {
            _settingGroup = settingGroup;
            Items = new ObservableCollection<ISettingViewModel>(items);
        }

        public ObservableCollection<ISettingViewModel> Items { get; init; }

        public SettingDataType SettingDataType => _settingGroup.SettingDataType;
        public string Key => _settingGroup.Key;
        public string Name => _settingGroup.Key; // TODO fetch from resources

        public string Description => _settingGroup.Key; // TODO fetch from resources

        public SettingTags Tags => _settingGroup.Tags;
        public bool IsReadOnlyRecommended => Tags.HasFlag(SettingTags.ReadOnlyRecommended);
        public bool IsAdvancedSetting => Tags.HasFlag(SettingTags.Advanced);
        public bool IsExperimental => Tags.HasFlag(SettingTags.Experimental);

        private bool _isEnabled;
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        public RubberduckSetting ToSetting() => _settingGroup with { Value = Items.Select(e => e.ToSetting()).ToArray() };
    }
}
