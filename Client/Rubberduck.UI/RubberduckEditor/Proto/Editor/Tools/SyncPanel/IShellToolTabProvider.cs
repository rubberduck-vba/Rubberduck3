using Rubberduck.UI.RubberduckEditor.Proto.Editor.Tools;
using System.Collections.Generic;

namespace Rubberduck.UI.RubberduckEditor.Proto.Editor.Tools.SyncPanel
{
    public interface IShellToolTabProvider
    {
        IEnumerable<IShellToolTab> GetShellToolTabs();
    }
}
