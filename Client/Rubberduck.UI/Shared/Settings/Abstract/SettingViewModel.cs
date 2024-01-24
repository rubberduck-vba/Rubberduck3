﻿using Rubberduck.InternalApi.Settings.Model;
using System;

namespace Rubberduck.UI.Shared.Settings.Abstract
{
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

        public bool IsHidden => Tags.HasFlag(SettingTags.Hidden);

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
}
