using Rubberduck.UI.Abstract.Editor;

namespace Rubberduck.UI.RubberduckEditor.Proto
{
    public class EditorShellContext : IEditorShellContext
    {
        public EditorShellContext(IEditorShellViewModel shell)
        {
            Shell = shell;
        }

        public static EditorShellContext Current { get; set; }
        public IEditorShellViewModel Shell { get; set; }
    }
}