using Rubberduck.SettingsProvider.Model;
using System;
using System.Collections.Generic;

namespace Rubberduck.UI.Shared.Settings.Abstract
{
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

    public abstract class EnumValueSettingViewModel<TEnum> : EnumValueSettingViewModel, ISettingViewModel<TEnum>
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
}
