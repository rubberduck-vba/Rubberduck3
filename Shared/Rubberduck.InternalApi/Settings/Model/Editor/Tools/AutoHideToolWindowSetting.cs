namespace Rubberduck.InternalApi.Settings.Model.Editor.Tools
{
    public record class AutoHideToolWindowSetting : BooleanRubberduckSetting
    {
        public static bool DefaultSettingValue { get; } = false;

        public AutoHideToolWindowSetting()
        {
            DefaultValue = DefaultSettingValue;
        }
    }
}
