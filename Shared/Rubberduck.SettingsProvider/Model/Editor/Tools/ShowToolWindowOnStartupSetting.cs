namespace Rubberduck.SettingsProvider.Model.Editor.Tools
{
    public record class ShowToolWindowOnStartupSetting : BooleanRubberduckSetting
    {
        public static bool DefaultSettingValue { get; } = false;

        public ShowToolWindowOnStartupSetting()
        {
            DefaultValue = DefaultSettingValue;
        }
    }
}
