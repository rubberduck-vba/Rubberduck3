using Dragablz;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace Rubberduck.UI.Windows
{
    public interface IDragablzWindowViewModel
    {
        string Title { get; }
        ObservableCollection<object> Tabs { get; }

        IInterTabClient InterTabClient { get; }
        string Partition { get; }
        TabEmptiedResponse OnTabControlEmptied(TabablzControl tabControl, Window window);
    }

    public interface ITabViewModel : INotifyPropertyChanged
    {
        object ContentControl { get; set; }

        string Title { get; }
        object Header { get; set; }
        string TextContent { get; set; }
        bool IsSelected { get; set; }
        bool IsPinned { get; set; }
    }
}
