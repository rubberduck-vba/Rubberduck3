using Rubberduck.SettingsProvider.Model;
using Rubberduck.UI.Shared.Settings.Abstract;
using System;

namespace Rubberduck.UI.Shared.Settings
{
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
}
