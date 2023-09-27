using Rubberduck.InternalApi.Model;
using Rubberduck.Unmanaged.Model;
using Rubberduck.Unmanaged.Model.Abstract;

namespace Rubberduck.VBEditor.Utility
{
    public interface ISelectionService : ISelectionProvider
    {
        bool TryActivate(IQualifiedModuleName module);
        bool TrySetActiveSelection(IQualifiedModuleName module, Selection selection);
        bool TrySetActiveSelection(QualifiedSelection selection);
        bool TrySetSelection(IQualifiedModuleName module, Selection selection);
        bool TrySetSelection(QualifiedSelection selection);
    }
}