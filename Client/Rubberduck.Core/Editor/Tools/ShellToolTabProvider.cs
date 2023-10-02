using Rubberduck.UI.Abstract;
using System.Collections.Generic;

namespace Rubberduck.Core.Editor.Tools
{
    class ShellToolTabProvider : IShellToolTabProvider
    {
        //private readonly ISyncPanelToolTab _syncPanel;
        // TODO add new tool tabs here

        public ShellToolTabProvider(/*ISyncPanelToolTab syncPanel*/)
        {
            //_syncPanel = syncPanel;
        }

        public IEnumerable<IShellToolTab> GetShellToolTabs()
        {
            return new IShellToolTab[] 
            { 
                //_syncPanel 
            };
        }
    }
}
