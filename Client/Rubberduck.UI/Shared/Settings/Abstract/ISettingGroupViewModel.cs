using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Rubberduck.UI.Shared.Settings.Abstract
{
    public interface ISearchable
    {
        bool IsSearchResult(string search);
    }


    public interface ISettingGroupViewModel : ISettingViewModel, ISearchable
    {
        ICollectionView ItemsView { get; }
        ObservableCollection<ISettingViewModel> Items { get; }
        bool IsExpanded { get; set; }
        string SearchString { get; set; }

        ISettingViewModel Selection { get; set; }
    }
}
