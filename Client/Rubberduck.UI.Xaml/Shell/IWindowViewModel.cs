using Dragablz;
using System;

namespace Rubberduck.UI.Xaml.Shell
{
    public interface IWindowViewModel
    {
        string Title { get; }

        IInterTabClient InterTabClient { get; }
        object Partition { get; }
        void ClosingTabItemHandler(object sender, EventArgs e);
    }
}
