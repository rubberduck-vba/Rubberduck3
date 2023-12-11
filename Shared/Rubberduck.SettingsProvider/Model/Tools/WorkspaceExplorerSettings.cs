using Rubberduck.InternalApi.Settings;
using System.Linq;

namespace Rubberduck.SettingsProvider.Model.Tools
{
    public record class WorkspaceExplorerSettings : ToolWindowSettings, IDefaultSettingsProvider<WorkspaceExplorerSettings>
    {
        private static new readonly RubberduckSetting[] DefaultSettings = 
            ToolWindowSettings.DefaultSettings.Concat(new RubberduckSetting[]
            {
                // add new workspace explorer settings here
            }).ToArray();

        public static WorkspaceExplorerSettings Default { get; } = new WorkspaceExplorerSettings() { Value = DefaultSettings, DefaultValue = DefaultSettings };
        WorkspaceExplorerSettings IDefaultSettingsProvider<WorkspaceExplorerSettings>.Default => Default;
    }
}
