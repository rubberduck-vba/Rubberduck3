using Dragablz;
using System;
using System.Collections.ObjectModel;

namespace Rubberduck.UI.Windows
{
    public interface IDragablzWindowViewModel : IWindowViewModel
    {
        ObservableCollection<object> Tabs { get; }

        IInterTabClient InterTabClient { get; }
        string Partition { get; }
        void ClosingTabItemHandler(object sender, EventArgs e);
    }
}
