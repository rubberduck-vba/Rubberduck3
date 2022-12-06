using Rubberduck.UI.Abstract;
using Rubberduck.VBEditor.Events;

namespace Rubberduck.UI.Abstract
{
    public interface ISyncPanelModuleViewModelProvider
    {
        ISyncPanelModuleViewModel Create(ComponentEventArgs info);
    }
}
