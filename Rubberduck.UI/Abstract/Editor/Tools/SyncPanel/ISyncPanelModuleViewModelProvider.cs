
using Rubberduck.InternalApi.Model;

namespace Rubberduck.UI.Abstract
{
    public interface ISyncPanelModuleViewModelProvider
    {
        ISyncPanelModuleViewModel Create(IQualifiedModuleName info);
    }
}
