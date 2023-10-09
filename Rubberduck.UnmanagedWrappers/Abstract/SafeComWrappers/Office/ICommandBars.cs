using Rubberduck.Unmanaged.Abstract.SafeComWrappers.Office.Enums;

namespace Rubberduck.Unmanaged.Abstract.SafeComWrappers.Office
{
    // Abstraction of the CommandBars coclass interface in the interop assemblies for Office.v8 and Office.v12
    public interface ICommandBars : ISafeComWrapper, IComCollection<ICommandBar>
    {
        ICommandBar Add(string name);
        ICommandBar Add(string name, CommandBarPosition position);
        ICommandBarControl FindControl(int id);
        ICommandBarControl FindControl(ControlType type, int id);
    }
}