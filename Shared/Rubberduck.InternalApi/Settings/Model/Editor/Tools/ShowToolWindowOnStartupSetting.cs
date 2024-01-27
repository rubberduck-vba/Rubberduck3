using Rubberduck.InternalApi.Settings.Model;

namespace Rubberduck.InternalApi.Settings.Model.Editor.Tools;

public record class ShowToolWindowOnStartupSetting : BooleanRubberduckSetting
{
    public static bool DefaultSettingValue { get; } = false;

    public ShowToolWindowOnStartupSetting()
    {
        DefaultValue = DefaultSettingValue;
    }
}
