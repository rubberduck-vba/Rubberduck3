using System.Windows.Forms;
using Rubberduck.VBEditor.UI.OfficeMenus;

namespace Rubberduck.Common.Hotkeys
{
    public interface IHotkey : IAttachable
    {
        string Key { get; }
        IMenuCommand Command { get; }
        HotkeyInfo HotkeyInfo { get; }
        Keys Combo { get; }
        Keys SecondKey { get; }
        bool IsTwoStepHotkey { get; }
    }
}
