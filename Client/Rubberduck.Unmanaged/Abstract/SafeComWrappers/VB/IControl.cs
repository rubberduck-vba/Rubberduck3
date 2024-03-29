using System;
using System.ComponentModel;

namespace Rubberduck.Unmanaged.Abstract.SafeComWrappers.VB
{
    public interface IControl : ISafeComWrapper, IEquatable<IControl>
    {
        string Name { get; set; }   // Not on VB6 VBControl
    }

    public static class ControlExtensions
    {
        public static string TypeName(this IControl control)
        {
            try
            {
                return TypeDescriptor.GetClassName(control.Target);
            }
            catch
            {
                return "Control";
            }
        }
    }
}