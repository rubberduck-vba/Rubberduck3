using Rubberduck.InternalApi.Model;
using Rubberduck.Unmanaged.Model.Abstract;

namespace Rubberduck.UI.Abstract.Editor.Tools.SyncPanel
{
    public interface ISyncPanelModuleViewModelProvider
    {
        ISyncPanelModuleViewModel Create(IQualifiedModuleName info);
    }
}
