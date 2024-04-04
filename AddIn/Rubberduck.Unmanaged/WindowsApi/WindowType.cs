using System;

namespace Rubberduck.Unmanaged.WindowsApi
{
    [Flags]
    public enum WindowType : uint
    {
        Indeterminate = 0u,
        Project = 1u,
        CodePane = 1u << 2 | VbaWindow,
        DesignerWindow = 1u << 3 | VbaWindow,
        Immediate = 1u << 4 | VbaWindow,
        ObjectBrowser = 1u << 5 | VbaWindow,
        Locals = 1u << 6 | VbaWindow,
        Watches = 1u << 7 | VbaWindow,
        IntelliSense = 1u << 8,
        VbaWindow = 1u << 31
    }
}
