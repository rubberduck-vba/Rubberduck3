namespace Rubberduck.SettingsProvider.Model
{
    public record class ShowSplashSetting : RubberduckSetting<bool>
    {
        public static bool DefaultSettingValue { get; } = true;

        private static readonly string _description = "Determines whether or not to display a splash screen showing ongoing operations while initializing the Rubberduck Editor.";

        public ShowSplashSetting() : this(DefaultSettingValue) { }

        public ShowSplashSetting(bool value)
            : base(nameof(ShowSplashSetting), value, SettingDataType.BooleanSetting, DefaultSettingValue) { }
    }
}
