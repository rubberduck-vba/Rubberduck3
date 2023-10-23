﻿namespace Rubberduck.SettingsProvider.Model
{
    public record class DisableInitialLogLevelResetSetting : RubberduckSetting<bool>
    {
        public static bool DefaultSettingValue { get; } = false;

        private static readonly string _description = "Allow the minimum log level to remain at a verbose TRACE level after a successful initialization and shutdown; logging normally gets automatically disabled otherwise.";


        public DisableInitialLogLevelResetSetting() : this(DefaultSettingValue) { }

        public DisableInitialLogLevelResetSetting(bool value)
            : base(SettingDataType.BooleanSetting, nameof(DisableInitialLogLevelResetSetting), _description, DefaultSettingValue, value)
        {
        }
    }
}
