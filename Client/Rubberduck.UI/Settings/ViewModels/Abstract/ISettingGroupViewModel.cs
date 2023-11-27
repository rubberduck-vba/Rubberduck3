using System.Collections.ObjectModel;

namespace Rubberduck.UI.Settings.ViewModels.Abstract
{
    public interface ISettingGroupViewModel : ISettingViewModel
    {
        public ObservableCollection<ISettingViewModel> Items { get; }
    }
}
