namespace Rubberduck.InternalApi.Settings.Model;

public record class BooleanRubberduckSetting : TypedRubberduckSetting<bool>
{
    public BooleanRubberduckSetting()
    {
        SettingDataType = SettingDataType.BooleanSetting;
    }
}
