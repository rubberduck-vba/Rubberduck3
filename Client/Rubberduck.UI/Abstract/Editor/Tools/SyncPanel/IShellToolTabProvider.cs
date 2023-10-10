using Rubberduck.UI.Abstract.Editor.Tools;
using System.Collections.Generic;

namespace Rubberduck.UI.Abstract.Editor.Tools.SyncPanel
{
    public interface IShellToolTabProvider
    {
        IEnumerable<IShellToolTab> GetShellToolTabs();
    }
}
