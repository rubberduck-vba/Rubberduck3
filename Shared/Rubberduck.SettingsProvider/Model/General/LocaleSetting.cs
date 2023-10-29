namespace Rubberduck.SettingsProvider.Model
{
    public record class LocaleSetting : RubberduckSetting<string>
    {
        public static string DefaultSettingValue { get; } = "en-US";

        // TODO localize. Yes, irony :D
        private static readonly string _description = "Determines the display language of localized user interface elements.";

        public LocaleSetting() : this(DefaultSettingValue) { }

        public LocaleSetting(string value)
            : base(nameof(LocaleSetting), value, SettingDataType.TextSetting, DefaultSettingValue) { }
    }
}
