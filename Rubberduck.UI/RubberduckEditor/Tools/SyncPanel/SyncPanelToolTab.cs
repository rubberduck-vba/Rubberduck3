using Rubberduck.UI;
using Rubberduck.UI.Abstract;
using System.Windows.Input;

namespace Rubberduck.Core.Editor.Tools
{
    public class SyncPanelToolTab : ViewModelBase, ISyncPanelToolTab
    {
        public SyncPanelToolTab(ISyncPanelViewModel viewModel, IShellToolTabSetting settings)
        {
            Name = "Sync"; // TODO get from resources
            ViewModel = viewModel;
            Settings = settings;
        }

        public string Name { get; }
        public object ViewModel { get; }

        public IShellToolTabSetting Settings { get; }
    }
}
