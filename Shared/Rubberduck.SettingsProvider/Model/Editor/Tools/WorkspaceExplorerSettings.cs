using Rubberduck.InternalApi.Settings;
using System.Linq;

namespace Rubberduck.SettingsProvider.Model.Editor.Tools
{
    public record class WorkspaceExplorerSettings : ToolWindowSettings, IDefaultSettingsProvider<WorkspaceExplorerSettings>
    {
        private static new readonly RubberduckSetting[] DefaultSettings =
            [
                new ShowToolWindowOnStartupSetting(),
                new AutoHideToolWindowSetting(),
                new DefaultToolWindowLocationSetting(),
            ];

        public WorkspaceExplorerSettings()
        {
            Value = DefaultValue = DefaultSettings;
        }

        public static WorkspaceExplorerSettings Default { get; } = new WorkspaceExplorerSettings() { Value = DefaultSettings, DefaultValue = DefaultSettings };
        WorkspaceExplorerSettings IDefaultSettingsProvider<WorkspaceExplorerSettings>.Default => Default;
    }
}
