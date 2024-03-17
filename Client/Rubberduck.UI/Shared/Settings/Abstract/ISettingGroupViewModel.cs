using System.Collections.ObjectModel;

namespace Rubberduck.UI.Shared.Settings.Abstract
{
    public interface ISettingGroupViewModel : ISettingViewModel
    {
        ObservableCollection<ISettingViewModel> Items { get; }
        bool IsExpanded { get; }
    }
}
