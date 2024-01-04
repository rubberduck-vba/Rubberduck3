namespace Rubberduck.SettingsProvider.Model.Editor
{
    public record class ShowWelcomeTabSetting : BooleanRubberduckSetting
    {
        public static bool DefaultSettingValue { get; } = true;

        public ShowWelcomeTabSetting()
        {
            DefaultValue = DefaultSettingValue;
        }
    }
}
