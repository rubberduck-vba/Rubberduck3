namespace Rubberduck.SettingsProvider.Model.Editor
{
    /// <summary>
    /// If enabled, the editor window will replace the native Windows chrome and caption bar with a custom one that can be themed.
    /// </summary>
    public record class ExtendWindowChromeSetting : BooleanRubberduckSetting
    {
        public static bool DefaultSettingValue { get; } = true;

        public ExtendWindowChromeSetting()
        {
            DefaultValue = DefaultSettingValue;
        }
    }
}
