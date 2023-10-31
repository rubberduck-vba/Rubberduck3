namespace Rubberduck.SettingsProvider.Model.General
{
    /// <summary>
    /// Determines the display language of localized user interface elements.
    /// </summary>
    public record class LocaleSetting : StringRubberduckSetting
    {
        public static string DefaultSettingValue { get; } = "en-US";

        public LocaleSetting()
        {
            DefaultValue = DefaultSettingValue;
            IsRequired = true;
            RegEx = @"[a-z]{2}\-[A-Z]{2}";
        }
    }
}
