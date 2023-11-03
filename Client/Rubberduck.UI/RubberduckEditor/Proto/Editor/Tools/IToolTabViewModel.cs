using Rubberduck.UI.RubberduckEditor.Proto.Editor;

namespace Rubberduck.UI.RubberduckEditor.Proto.Editor.Tools
{
    public interface IToolTabViewModel
    {
        IEditorShellViewModel Shell { get; set; }
    }
}
