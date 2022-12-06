using Rubberduck.UI.Abstract;
using Rubberduck.VBEditor;

namespace Rubberduck.Core.Editor
{
    public interface ICodePaneViewModelProvider
    {
        ICodePaneViewModel GetViewModel(IEditorShellViewModel shell, QualifiedModuleName module, string content);
    }
}