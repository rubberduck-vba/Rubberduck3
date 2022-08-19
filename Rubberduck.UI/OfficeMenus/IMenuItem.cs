using System;

namespace Rubberduck.UI.OfficeMenus
{
    public interface IMenuItem
    {
        string Key { get; }
        Func<string> Caption { get; }
        Func<string> ToolTipText { get; }
        bool BeginGroup { get; }
        int DisplayOrder { get; }
    }
}
