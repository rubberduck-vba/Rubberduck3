namespace Rubberduck.SettingsProvider.Model.Editor.Tools
{
    public record class ShowWorkspaceExplorerOnProjectOpenSetting : BooleanRubberduckSetting
    {
        public static bool DefaultSettingValue { get; } = true;

        public ShowWorkspaceExplorerOnProjectOpenSetting()
        {
            DefaultValue = DefaultSettingValue;
        }
    }
}
