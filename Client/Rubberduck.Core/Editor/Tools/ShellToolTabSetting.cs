using Rubberduck.UI;
using Rubberduck.UI.Abstract;

namespace Rubberduck.Core.Editor.Tools
{
    public class ShellToolTabSetting : ViewModelBase, IShellToolTabSetting
    {
        private ToolTabLocation _tabPanelLocation;
        public ToolTabLocation TabPanelLocation
        {
            get => _tabPanelLocation;
            set
            {
                if (_tabPanelLocation != value)
                {
                    _tabPanelLocation = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isLoadedAtStartup;
        public bool IsLoadedAtStartup 
        {
            get => _isLoadedAtStartup;
            set
            {
                if (_isLoadedAtStartup != value)
                {
                    _isLoadedAtStartup = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
