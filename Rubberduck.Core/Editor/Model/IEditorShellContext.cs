using Rubberduck.UI.Abstract;

namespace Rubberduck.Core.Editor
{
    public interface IEditorShellContext
    {
        IEditorShellViewModel Shell { get; }
    }
}