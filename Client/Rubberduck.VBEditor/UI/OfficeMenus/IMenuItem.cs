using System;

namespace Rubberduck.VBEditor.UI.OfficeMenus
{
    public interface IMenuItem
    {
        string ResourceKey { get; }
        Func<string> Caption { get; }
        Func<string> ToolTipText { get;  }
        bool BeginGroup { get; set; }
        int DisplayOrder { get; set; }
    }
}
