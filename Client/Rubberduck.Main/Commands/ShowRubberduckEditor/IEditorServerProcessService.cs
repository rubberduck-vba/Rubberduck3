using System;

namespace Rubberduck.Main.Commands.ShowRubberduckEditor
{
    public interface IEditorServerProcessService
    {
        Exception? ShowEditor();
    }
}