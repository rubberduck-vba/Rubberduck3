using Rubberduck.InternalApi.Model;
using Rubberduck.Unmanaged.Model.Abstract;

namespace Rubberduck.UI.Abstract
{
    public interface ISyncPanelModuleViewModelProvider
    {
        ISyncPanelModuleViewModel Create(IQualifiedModuleName info);
    }
}
