using System.Collections.Generic;

namespace Rubberduck.UI.Abstract
{
    public interface IShellToolTabProvider
    {
        IEnumerable<IShellToolTab> GetShellToolTabs();
    }
}
