using Rubberduck.InternalApi.Model.Abstract;
using System.Collections.Generic;

namespace Rubberduck.UI.Settings.Templates
{
    public interface IEditorShellViewModel : IWindowViewModel
    {
        IEnumerable<IDocumentTabViewModel> Documents { get; }
    }
}
