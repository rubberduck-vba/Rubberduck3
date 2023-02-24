using Rubberduck.Settings;
using Rubberduck.VBEditor.UI.OfficeMenus;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rubberduck.Common.Hotkeys
{
    public class HotkeyFactory
    {
        private readonly IEnumerable<IMenuCommand> _commands;

        public HotkeyFactory(IEnumerable<IMenuCommand> commands)
        {
            _commands = commands;
        }

        public Hotkey Create(HotkeySetting setting, IntPtr hWndVbe)
        {
            if (setting == null)
            {
                return null;
            }

            var commandToBind = _commands.FirstOrDefault(command => command.GetType().Name == setting.CommandTypeName);

            return commandToBind == null ? null : new Hotkey(hWndVbe, setting.ToString(), commandToBind);
        }
    }
}
