namespace Rubberduck.SettingsProvider.Model
{
    public record class BooleanRubberduckSetting : TypedRubberduckSetting<bool>
    {
        public BooleanRubberduckSetting()
        {
            SettingDataType = SettingDataType.BooleanSetting;
        }
    }
}
