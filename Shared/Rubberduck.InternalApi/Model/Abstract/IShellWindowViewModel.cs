using System.Collections.Generic;

namespace Rubberduck.InternalApi.Model.Abstract
{
    public interface IShellWindowViewModel
    {
        string Title { get; }
        IStatusBarViewModel StatusBar { get; }
        IEnumerable<IDocumentTabViewModel> Documents { get; }
    }
}
