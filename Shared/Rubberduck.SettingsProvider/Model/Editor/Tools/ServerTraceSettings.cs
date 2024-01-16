using Rubberduck.InternalApi.Settings;
using System.Text.Json.Serialization;

namespace Rubberduck.SettingsProvider.Model.Editor.Tools
{
    public record class ServerTraceSettings : ToolWindowSettings, IDefaultSettingsProvider<ServerTraceSettings>
    {
        private static new readonly RubberduckSetting[] DefaultSettings =
            [
                new ShowToolWindowOnStartupSetting { DefaultValue = true },
                new AutoHideToolWindowSetting(),
                new DefaultToolWindowLocationSetting { DefaultValue = DockingLocation.DockLeft },

                new MaximumMessagesSetting()
            ];

        public ServerTraceSettings()
        {
            Value = DefaultValue = DefaultSettings;
        }

        [JsonIgnore]
        public double MaximumMessages
        {
            get
            {
                var value = GetSetting<MaximumMessagesSetting>()?.TypedValue ?? MaximumMessagesSetting.DefaultSettingValue;
                if (value == default)
                {
                    return MaximumMessagesSetting.DefaultSettingValue;
                }
                return value;
            }
        }

        public static ServerTraceSettings Default { get; } = new() { Value = DefaultSettings, DefaultValue = DefaultSettings };

        ServerTraceSettings IDefaultSettingsProvider<ServerTraceSettings>.Default => Default;
    }

    /// <summary>
    /// The maximum number of messages rendered in the ServerTrace toolwindow.
    /// </summary>
    public record class MaximumMessagesSetting : NumericRubberduckSetting
    {
        public static double DefaultSettingValue { get; } = 255;

        public MaximumMessagesSetting()
        {
            DefaultValue = DefaultSettingValue;
            Tags = SettingTags.Common;
        }
    }
}
