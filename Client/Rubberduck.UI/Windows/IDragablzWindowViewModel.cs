using Dragablz;
using System;

namespace Rubberduck.UI.Windows
{
    public interface IDragablzWindowViewModel : IWindowViewModel
    {
        IInterTabClient InterTabClient { get; }
        string Partition { get; }
        void ClosingTabItemHandler(object sender, EventArgs e);
    }
}
