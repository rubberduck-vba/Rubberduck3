using System;

namespace Rubberduck.InternalApi.Model.Abstract
{
    public interface IWindowViewModel
    {
        string Title { get; }

        object InterTabClient { get; }
        object Partition { get; }
        void ClosingTabItemHandler(object sender, EventArgs e);
    }
}
