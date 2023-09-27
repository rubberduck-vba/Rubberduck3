using Rubberduck.InternalApi.Model;
using Rubberduck.Unmanaged.Model;
using Rubberduck.Unmanaged.Model.Abstract;
using System.Collections.Generic;

namespace Rubberduck.VBEditor.Utility
{
    public interface ISelectionProvider
    {
        /// <summary>
        /// Gets the QualifiedModuleName for the component that is currently selected in the Project Explorer.
        /// </summary>
        IQualifiedModuleName ProjectExplorerSelection();
        QualifiedSelection? ActiveSelection();
        ICollection<IQualifiedModuleName> OpenModules();
        Selection? Selection(IQualifiedModuleName module);
    }
}