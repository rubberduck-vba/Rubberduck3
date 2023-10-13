using Rubberduck.InternalApi.Model;
using Rubberduck.Unmanaged.Model.Abstract;

namespace Rubberduck.UI.RubberduckEditor.Proto.Editor.Tools.SyncPanel
{
    public interface ISyncPanelModuleViewModelProvider
    {
        ISyncPanelModuleViewModel Create(IQualifiedModuleName info);
    }
}
