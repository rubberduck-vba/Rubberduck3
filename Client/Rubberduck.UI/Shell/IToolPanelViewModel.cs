using Rubberduck.InternalApi.Settings.Model.Editor.Tools;

namespace Rubberduck.UI.Shell
{
    public interface IToolPanelViewModel
    {
        /// <summary>
        /// The location of the tool panel.
        /// </summary>
        DockingLocation PanelLocation { get; }

        /// <summary>
        /// Whether the tool panel is currently expanded.
        /// </summary>
        bool IsExpanded { get; set; }

        /// <summary>
        /// Whether the tool panel remains expanded on mouse leave.
        /// </summary>
        bool IsPinned { get; set; }
        bool IsDocked => PanelLocation != DockingLocation.None;
    }
}
