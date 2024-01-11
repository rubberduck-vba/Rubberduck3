using System.Collections.ObjectModel;

namespace Rubberduck.UI.Shared.Settings.Abstract
{
    public interface ISettingGroupViewModel : ISettingViewModel
    {
        public ObservableCollection<ISettingViewModel> Items { get; }
    }
}
