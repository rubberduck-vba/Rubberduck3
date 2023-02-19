using Rubberduck.UI.Abstract;

namespace Rubberduck.UI.RubberduckEditor
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