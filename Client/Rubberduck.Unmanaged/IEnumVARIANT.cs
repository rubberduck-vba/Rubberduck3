using System;
using System.Runtime.InteropServices;

namespace Rubberduck.Unmanaged
{
    [ComImport(), Guid("00020404-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IEnumVARIANT
    {
        // rgVar is technically an unmanaged array here, but we only ever call with celt=1, so this is compatible.
        [PreserveSig] int Next([In] uint celt, [Out] out object rgVar, [Out] out uint pceltFetched);

        [PreserveSig] int Skip([In] uint celt);
        [PreserveSig] int Reset();
        [PreserveSig] int Clone([Out] out IEnumVARIANT retval);
    }
}