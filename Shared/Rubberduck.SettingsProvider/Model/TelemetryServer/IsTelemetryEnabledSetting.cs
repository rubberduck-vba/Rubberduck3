﻿namespace Rubberduck.SettingsProvider.Model
{
    public record class IsTelemetryEnabledSetting : RubberduckSetting<bool>
    {
        // TODO localize
        private static readonly string _description = "Determines whether telemetry is enabled at all.";

        public IsTelemetryEnabledSetting(bool defaultValue) : this(defaultValue, defaultValue) { }
        public IsTelemetryEnabledSetting(bool defaultValue, bool value)
            : base(SettingDataType.BooleanSetting, nameof(IsTelemetryEnabledSetting), _description, defaultValue, value)
        {
        }
    }
}
